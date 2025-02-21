namespace BitcrackRandomiser.Enums
{
    /// <summary>
    /// Message types for console
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Regular message
        /// </summary>
        normal = 0,
        /// <summary>
        /// Success message
        /// </summary>
        success = 1,
        /// <summary>
        /// Information
        /// </summary>
        info = 2,
        /// <summary>
        /// Error message
        /// </summary>
        error = 3,
        /// <summary>
        /// External app's message
        /// </summary>
        externalApp = 4,
        /// <summary>
        /// Seperator for line
        /// </summary>
        seperator = 10
    }
}
