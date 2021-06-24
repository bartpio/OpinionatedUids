namespace OpinionatedUids
{
    /// <summary>
    /// opinionated uid constants
    /// </summary>
    public static class OpinionatedUidConstants
    {
        /// <summary>
        /// unique ID size, in bytes
        /// </summary>
        public const int UidSize = 16;

        /// <summary>
        /// max tries to come up with an identifier satisfying the judges
        /// </summary>
        internal const int MaxTries = 1_000_000;
    }
}
