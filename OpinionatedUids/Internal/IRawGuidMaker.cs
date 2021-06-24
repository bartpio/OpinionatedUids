using System;

namespace OpinionatedUids.Internal
{
    /// <summary>
    /// provides raw globally unique identifiers
    /// </summary>
    public interface IRawGuidMaker
    {
        /// <summary>
        /// provide a globally unique identifier
        /// </summary>
        /// <returns>
        /// a raw globally unique identifier
        /// </returns>
        Guid NewGuid();
    }
}
