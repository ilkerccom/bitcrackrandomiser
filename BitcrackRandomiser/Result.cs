using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcrackRandomiser
{
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

    /// <summary>
    /// Output type for bitcrack app
    /// </summary>
    enum OutputType
    {
        running,
        finished,
        address,
        privateKeyFound,
        gpuModel,
        unknown
    }
}
