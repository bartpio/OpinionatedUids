using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using OpinionatedUids.Internal;
using OpinionatedUids.Opinions;
using System;
using System.Linq;
using System.Reflection;

namespace OpinionatedUids.Tests
{
    public class Tests
    {
        [Test]
        [Repeat(10_000)]
        public void TestSimpleProhibition()
        {
            var services = new ServiceCollection();
            var buf = new byte[16] { 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            var prefixspec = new Guid(buf);
            var judge = new ProhibitedUidPrefix(prefixspec, 1); // second argument is length of the prefix to consider
            services.AddOpinionatedUids(judge);

            using (var sp = services.BuildServiceProvider())
            {
                var maker = sp.GetRequiredService<IOpinionatedUidMaker>();
                var uid = maker.NewUid();
                Assert.That(uid.ToByteArray().First(), Is.Not.EqualTo(0x11));
            }
        }

        [Test]
        [Repeat(10)]
        public void TestFirstByteJudge([Values(false, true)] bool withHelperTransform)
        {
            var services = new ServiceCollection();
            services.AddOpinionatedUids();

            services.AddTransient<IUidJudge, WantSpecificFirstByteJudge>();
            if (withHelperTransform)
            {
                services.AddTransient<IUidTransform>(isp => new SpecifyFirstByteTransform(0xFF));
            }
            using (var sp = services.BuildServiceProvider())
            {
                var maker = sp.GetRequiredService<IOpinionatedUidMaker>();
                var uid = maker.NewUid();
                Assert.That(uid.ToByteArray().First(), Is.EqualTo(0xFF));
            }
        }

        [Test]
        public void TestFirstByteJudgeImpossibleSituationThrows()
        {
            var services = new ServiceCollection();
            services.AddOpinionatedUids();

            services.AddTransient<IUidJudge, WantSpecificFirstByteJudge>();
            services.AddTransient<IUidTransform>(isp => new SpecifyFirstByteTransform(0xEE));
            using (var sp = services.BuildServiceProvider())
            {
                var maker = sp.GetRequiredService<IOpinionatedUidMaker>();
                Assert.Throws<OpinionatedUidException>(() =>
                {
                    maker.NewUid();
                });
            }
        }

        public static IUidJudge PrefixJudgeByBytes(IServiceProvider isp)
        {
            return new ProhibitedUidPrefix(new byte[] { 0xFF, 0xCC });
        }

        public static IUidJudge PrefixJudgeByGuid(IServiceProvider isp)
        {
            var buf = new byte[] { 0xFF, 0xCC, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            var guid = new Guid(buf);
            return new ProhibitedUidPrefix(guid, 2);
        }

        public static IUidJudge JudgeNothing(IServiceProvider isp)
        {
            return new NullUidJudge();
        }

        [Test]
        [Retry(100)]
        public void TestProhibitedPrefix([Values(false, true)] bool withHelperTransform, [Values(nameof(PrefixJudgeByBytes), nameof(PrefixJudgeByGuid), nameof(JudgeNothing))] string judgeFunction)
        {
            var services = new ServiceCollection();
            services.AddOpinionatedUids();

            // judge #1
            var cjudge = new CountingJudge();
            services.AddSingleton<IUidJudge>(isp => cjudge);

            // judge #2
            var judgeNothing = judgeFunction == nameof(JudgeNothing); // note judgenothing mode flips assertion down below
            {
                var mi = typeof(Tests).GetMethod(judgeFunction, BindingFlags.Public | BindingFlags.Static);
                var de = (Func<IServiceProvider, IUidJudge>)Delegate.CreateDelegate(typeof(Func<IServiceProvider, IUidJudge>), mi);
                services.AddSingleton<IUidJudge>(de);
            }

            // judge #3
            var mockjudge = new Mock<IUidJudge>();
            mockjudge.Setup(x => x.IsUidAlright(It.IsAny<Guid>())).Returns(true);
            services.AddTransient<IUidJudge>(isp => mockjudge.Object);

            if (withHelperTransform)
            {
                services.AddTransient<IUidTransform>(isp => new SpecifyFirstByteTransform(0x11));
            }

            using (var sp = services.BuildServiceProvider())
            {
                var maker = sp.GetRequiredService<IOpinionatedUidMaker>();
                const int cnt = 100_000;
                unsafe
                {
                    Guid guid = Guid.Empty;
                    byte* ptr = (byte*)(&guid);
                    {
                        for (var idx = 0; idx < cnt; idx++)
                        {
                            guid = maker.NewUid();
                            if (*ptr == 0xFF && *(ptr + 1) == 0xCC)
                            {
                                if (!judgeNothing)
                                {
                                    Assert.Fail("unexpected prefix was actually seen");
                                }
                                else
                                {
                                    var passmsg = "the prefix was seen, we're done here";
                                    TestContext.WriteLine(passmsg);
                                    Assert.Pass(passmsg);
                                }
                            }
                        }

                        if (!withHelperTransform)
                        {
                            Assert.That(cjudge.UniquesSeen, Is.GreaterThan(cnt));
                        }
                        else
                        {
                            Assert.That(cjudge.UniquesSeen, Is.EqualTo(cnt));
                        }
                        mockjudge.Verify(x => x.IsUidAlright(It.IsAny<Guid>()), Times.Exactly(cnt));
                    }
                }
            }

            TestContext.WriteLine("FIN");
        }

        public static byte[] Elongate(byte byt, int times)
        {
            return Enumerable.Repeat(byt, times).ToArray();
        }

        [Test]
        public void TestProhibitedPrefixUsingTwoValue([Values(0x11, 0x22)] byte prohibited, [Values(false, true)] bool startRawMakerAtTwo)
        {
            var expected = (byte)((prohibited == 0x11) ? 0x22 : 0x11);
            var expectedUid = new Guid(Elongate(expected, 16));
            for (var len = 1; len <= 16; len++)
            {
                var services = new ServiceCollection();
                services.AddOpinionatedUids(new ProhibitedUidPrefix(Elongate(prohibited, len)));
                var rgm = new TwoValueRawGuidMaker(startRawMakerAtTwo);
                services.AddSingleton<IRawGuidMaker>(rgm);

                using (var sp = services.BuildServiceProvider())
                {
                    var maker = sp.GetRequiredService<IOpinionatedUidMaker>();
                    var uid = maker.NewUid();
                    Assert.That(uid, Is.EqualTo(expectedUid));
                }
            }
        }

        [Test]
        [Sequential]
        public void TestProhibitedPrefixUsingTwoValue(
            [Values(15, 14)] int nonzeroLen,
            [Values("22222222-2222-2222-2222-222222222200", "11111111-1111-1111-1111-111111110000")] string expected)
        {
            var services = new ServiceCollection();
            services.AddOpinionatedUids(new ProhibitedUidPrefix(Elongate(0x11, 15)));
            var rgm = new TwoValueRawGuidMaker(false, nonzeroLen);
            services.AddSingleton<IRawGuidMaker>(rgm);

            using (var sp = services.BuildServiceProvider())
            {
                var maker = sp.GetRequiredService<IOpinionatedUidMaker>();
                var uid = maker.NewUid();
                Assert.That(uid, Is.EqualTo(Guid.Parse(expected)));
            }
        }

        [Test]
        public void TestTransform()
        {
            var services = new ServiceCollection();
            var onez = new Guid(Elongate(0x11, 16));
            var twoz = new Guid(Elongate(0x22, 16));
            services.AddOpinionatedUids(new ProhibitedUidPrefix(onez, 16));
            services.AddOpinionatedUids(new ProhibitedUidPrefix(twoz, 16));

            var mockTransform = new Mock<IUidTransform>();
            mockTransform.Setup(x => x.TransformUid(It.Is<Guid>(x => x == onez))).Returns(Guid.Empty);
            mockTransform.Setup(x => x.TransformUid(It.Is<Guid>(x => x == twoz))).Returns(Guid.Empty);
            services.AddOpinionatedUidTransforms(mockTransform.Object);

            var rgm = new TwoValueRawGuidMaker();
            services.AddSingleton<IRawGuidMaker>(rgm);

            using (var sp = services.BuildServiceProvider())
            {
                var maker = sp.GetRequiredService<IOpinionatedUidMaker>();
                var uid = maker.NewUid();
                Assert.That(uid, Is.EqualTo(Guid.Empty), "empty first time");
                uid = maker.NewUid();
                Assert.That(uid, Is.EqualTo(Guid.Empty), "empty second time");
            }

            mockTransform.Verify(x => x.TransformUid(It.Is<Guid>(x => x == onez)), Times.Once);
            mockTransform.Verify(x => x.TransformUid(It.Is<Guid>(x => x == twoz)), Times.Once);
            mockTransform.VerifyNoOtherCalls();
        }
    }
}