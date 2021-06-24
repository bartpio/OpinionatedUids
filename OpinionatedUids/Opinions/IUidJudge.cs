using System;

namespace OpinionatedUids.Opinions
{
    /// <summary>
    /// used to determine if a uid candidate is acceptable for usage
    /// </summary>
    public interface IUidJudge
    {
        /// <summary>
        /// determine if the uid candidate is acceptable for usage
        /// </summary>
        /// <param name="uid">
        /// uid candidate
        /// might be straight from RawGuidMaker
        /// if any <see cref="IUidTransform"/> have been configured, they have already been applied by this point
        /// </param>
        /// <returns>
        /// true if uid is acceptable for usage
        /// </returns>
        bool IsUidAlright(Guid uid);
    }
}
