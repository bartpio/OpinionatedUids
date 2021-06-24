# Opinionated Uids
## What does it do?
Generates unique identifiers conforming to the Guid CLR type. We avoid the term "Guid" in our naming conventions, to avoid
confusion with more standard ways of generating globally unique identifiers. Collision risk varies greatly, depending
on the configuration.

## How does it work?
We start with 16 cryptographically random bytes, then apply configured transforms (if any), then use configured judges (if any),
to determine if a candidate identifier is acceptable.

If any judge deems a candidate identifier not acceptable, we try again, using a new cryptographically random identifier.

## How to use?
This test code demonstrates a configuration with one opinion:
the first byte of a Guid (when expressed in byte array form) should not be equal to 0x11 (decimal 17).

```cs
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
```

## Raw Guid Source
Raw Guid values are generated via the IRawGuidMaker interface, prior to being transformed and judged (as configured).
The default implementation (RawGuidMaker) uses cryptographically random System.Security.Cryptography.RandomNumberGenerator.
Note that Guid.NewGuid is not used anywhere in the process. If desired, an alternative IRawGuidMaker can be registered.
An alternative IRawGuidMaker implemenation could use Guid.NewGuid, or some other means, to generate starting point identifiers.