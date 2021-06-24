using System;

namespace OpinionatedUids.Opinions
{
    /// <summary>
    /// judge implementation that adapts a predicate (used to check if uid acceptable)
    /// </summary>
    public sealed class UidJudge : IUidJudge
    {
        /// <summary>
        /// cons, given judgement predicate
        /// </summary>
        /// <param name="predicate">
        /// judgement predicate
        /// </param>
        public UidJudge(Predicate<Guid> predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        /// judgement predicate used to check if uid acceptable
        /// </summary>
        public Predicate<Guid> Predicate { get; }

        /// <summary>
        /// check if uid is alright to use, as per predicate
        /// </summary>
        /// <param name="uid">
        /// candidate uid
        /// </param>
        /// <returns>
        /// true if uid is acceptable for usage
        /// </returns>
        bool IUidJudge.IsUidAlright(Guid uid) => Predicate(uid);
    }
}
