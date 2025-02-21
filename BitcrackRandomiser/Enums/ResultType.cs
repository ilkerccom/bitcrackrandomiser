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
        workerStarted = 0,
        /// <summary>
        /// Worker gone offline
        /// </summary>
        workerExited = 1,
        /// <summary>
        /// A range scanned
        /// </summary>
        rangeScanned = 2,
        /// <summary>
        /// Reached of key space
        /// </summary>
        reachedOfKeySpace = 3,
        /// <summary>
        /// Private key found of target address
        /// </summary>
        keyFound = 4,
        /// <summary>
        /// An external reward was found
        /// </summary>
        rewardFound = 5
    }
}
