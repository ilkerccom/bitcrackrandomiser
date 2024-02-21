namespace BitcrackRandomiser.Enums
{
    /// <summary>
    /// Result types for share data. Telegram or API share
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// Worker started to scan
        /// </summary>
        workerStarted,
        /// <summary>
        /// Worker gone offline
        /// </summary>
        workerExited,
        /// <summary>
        /// A range scanned
        /// </summary>
        rangeScanned,
        /// <summary>
        /// Reached of key space
        /// </summary>
        reachedOfKeySpace,
        /// <summary>
        /// Private key found of target address
        /// </summary>
        keyFound,
        /// <summary>
        /// An external reward was found
        /// </summary>
        rewardFound
    }
}
