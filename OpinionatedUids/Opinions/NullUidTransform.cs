using System;

namespace OpinionatedUids.Opinions
{
    /// <summary>
    /// null-obj for <see cref="IUidTransform"/>
    /// </summary>
    public sealed class NullUidTransform : IUidTransform
    {
        /// <summary>
        /// return same uid as passed
        /// </summary>
        /// <param name="uid">
        /// a uid
        /// </param>
        /// <returns>
        /// same uid as passed
        /// </returns>
        Guid IUidTransform.TransformUid(Guid uid) => uid;
    }
}
