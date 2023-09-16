using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcrackRandomiser
{
    public class Models
    {
        /// <summary>
        /// Proof key model
        /// </summary>
        public class ProofKey
        {
            public string? Key { get; set; }

            public int GPUIndex { get; set; }
        }
    }
}
