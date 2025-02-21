namespace BitcrackRandomiser.Enums
{
    /// <summary>
    /// Output type for external app. (Bitcrack or another)
    /// </summary>
    public enum OutputType
    {
        /// <summary>
        /// App running
        /// </summary>
        running = 0,
        /// <summary>
        /// Scan completed
        /// </summary>
        finished = 1,
        /// <summary>
        /// Found an address (Proof address or target address)
        /// </summary>
        address = 2,
        /// <summary>
        /// Private key found
        /// </summary>
        privateKeyFound = 3,
        /// <summary>
        /// GPU model found on output
        /// </summary>
        gpuModel = 4,
        /// <summary>
        /// Any other types
        /// </summary>
        unknown = 10
    }
}
