using BitcrackRandomiser.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcrackRandomiser.Models
{
    /// <summary>
    /// Alternative for telegram. Share progress with your API
    /// </summary>
    public class ApiShare
    {
        // Current status
        public ResultType Status { get; set; }

        // Scanned HEX
        public string HEX { get; set; } = "";

        // Found private key
        public string PrivateKey { get; set; } = "";
    }
}
