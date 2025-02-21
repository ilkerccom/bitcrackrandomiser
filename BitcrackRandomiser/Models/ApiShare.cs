using BitcrackRandomiser.Enums;

namespace BitcrackRandomiser.Models
{
    /// <summary>
    /// Alternative for telegram. Share progress with your custom API
    /// </summary>
    public class ApiShare
    {
        /// <summary>
        /// Current status
        /// </summary>
        public ResultType Status { get; set; }

        /// <summary>
        /// Scanned HEX
        /// </summary>
        public string? HEX { get; set; }

        /// <summary>
        /// Found private key
        /// </summary>
        public string? PrivateKey { get; set; }
    }
}
