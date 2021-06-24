using Microsoft.Extensions.DependencyInjection;
using OpinionatedUids.Internal;
using OpinionatedUids.Opinions;
using System;
using System.Linq;

namespace OpinionatedUids
{
    /// <summary>
    /// service collection extensions
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// register uid maker, without any opinions just yet
        /// </summary>
        /// <param name="services">DI services</param>
        /// <returns>same services as provided, for chain</returns>
        public static IServiceCollection AddOpinionatedUids(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IRawGuidMaker, RawGuidMaker>();
            services.AddSingleton<IOpinionatedUidMaker, OpinionatedUidMaker>();
            return services;
        }

        /// <summary>
        /// register uid maker
        /// </summary>
        /// <param name="services">DI services</param>
        /// <param name="judges">judges used to determine accpetable uids</param>
        /// <returns>same services as provided, for chain</returns>
        public static IServiceCollection AddOpinionatedUids(this IServiceCollection services, params IUidJudge[] judges)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            AddOpinionatedUids(services);
            if (judges is not null)
            {
                foreach (var judge in judges)
                {
                    services.AddSingleton<IUidJudge>(judge);
                }
            }
            return services;
        }

        /// <summary>
        /// register uid maker
        /// </summary>
        /// <param name="services">DI services</param>
        /// <param name="judges">judges used to determine accpetable uids</param>
        /// <returns>same services as provided, for chain</returns>
        public static IServiceCollection AddOpinionatedUids(this IServiceCollection services, params Predicate<Guid>[] judges)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            AddOpinionatedUids(services);
            if (judges is not null)
            {
                AddOpinionatedUids(services, judges.Select(x => new UidJudge(x)).ToArray());
            }

            return services;
        }

        /// <summary>
        /// register uid transforms, to be called on raw guid prior to judges being invoked
        /// use of this feature is optional
        /// </summary>
        /// <param name="services">DI services</param>
        /// <param name="transforms">transforms to apply, prior to judging</param>
        /// <returns>same services as provided, for chain</returns>
        public static IServiceCollection AddOpinionatedUidTransforms(this IServiceCollection services, params IUidTransform[] transforms)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (transforms is not null)
            {
                foreach (var transform in transforms)
                {
                    services.AddSingleton<IUidTransform>(transform);
                }
            }

            return services;
        }
    }
}
