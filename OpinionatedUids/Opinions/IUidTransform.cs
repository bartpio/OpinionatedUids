using System;

namespace OpinionatedUids.Opinions
{
    /// <summary>
    /// specifies a uid transformation
    /// </summary>
    public interface IUidTransform
    {
        /// <summary>
        /// transform given uid
        /// </summary>
        /// <param name="uid">
        /// uid to transform (might be straight from RawGuidMaker, or other uid transforms may have already been applied)
        /// </param>
        /// <returns>
        /// modified form of the given uid
        /// </returns>
        Guid TransformUid(Guid uid);
    }
}
