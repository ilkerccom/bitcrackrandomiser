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
        running,
        /// <summary>
        /// Scan completed
        /// </summary>
        finished,
        /// <summary>
        /// Found an address (Proof address or target address)
        /// </summary>
        address,
        /// <summary>
        /// Private key found
        /// </summary>
        privateKeyFound,
        /// <summary>
        /// GPU model found on output
        /// </summary>
        gpuModel,
        /// <summary>
        /// Any other types
        /// </summary>
        unknown
    }
}
