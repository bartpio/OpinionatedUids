using System;

namespace OpinionatedUids.Opinions
{
    /// <summary>
    /// null-obj for <see cref="IUidJudge"/>
    /// </summary>
    public sealed class NullUidJudge : IUidJudge
    {
        /// <summary>
        /// check nothing and return always alright
        /// </summary>
        /// <param name="uid">
        /// ignored
        /// </param>
        /// <returns>
        /// always true
        /// </returns>
        bool IUidJudge.IsUidAlright(Guid uid) => true;
    }
}
