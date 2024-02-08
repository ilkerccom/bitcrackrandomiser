using BitcrackRandomiser.Enums;

namespace BitcrackRandomiser.Models
{
    /// <summary>
    /// Result of external app with output.
    /// </summary>
    internal class Result
    {
        /// <summary>
        /// Output type
        /// </summary>
        public OutputType OutputType { get; set; }

        /// <summary>
        /// Content. May be private key or another
        /// </summary>
        public string Content { get; set; } = "";
    }
}
