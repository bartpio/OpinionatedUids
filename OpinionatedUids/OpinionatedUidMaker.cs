using OpinionatedUids.Internal;
using OpinionatedUids.Opinions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpinionatedUids
{
    /// <summary>
    /// opinionated UID maker
    /// </summary>
    public sealed class OpinionatedUidMaker : IOpinionatedUidMaker
    {
        private readonly IRawGuidMaker _raw;
        private readonly List<IUidTransform> _transforms;
        private readonly List<IUidJudge> _judges;

        /// <summary>
        /// construct
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="transforms"></param>
        /// <param name="judges"></param>
        public OpinionatedUidMaker(IRawGuidMaker raw, IEnumerable<IUidTransform> transforms, IEnumerable<IUidJudge> judges)
        {
            if (transforms is null)
            {
                throw new ArgumentNullException(nameof(transforms));
            }
            if (judges is null)
            {
                throw new ArgumentNullException(nameof(judges));
            }

            _raw = raw ?? throw new ArgumentNullException(nameof(raw));
            _transforms = transforms.ToList();
            _judges = judges.ToList();
        }

        /// <summary>
        /// provide a uid transformed as configured, that satisfies all the configured judges
        /// </summary>
        /// <returns>
        /// a uid suitable for use as configured
        /// </returns>
        public Guid NewUid()
        {
            for (var attempt = 0; attempt < OpinionatedUidConstants.MaxTries; attempt++)
            {
                var uid = _raw.NewGuid();
                foreach (var transform in _transforms)
                {
                    uid = transform.TransformUid(uid);
                }

                if (!_judges.Any(x => !x.IsUidAlright(uid)))
                {
                    return uid; // good to go, we can return the uid!
                }
            }

            throw new OpinionatedUidException($"maximum tries ({OpinionatedUidConstants.MaxTries}) exceeded");
        }
    }
}
