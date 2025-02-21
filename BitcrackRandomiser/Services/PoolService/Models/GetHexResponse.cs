using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitcrackRandomiser.Services.PoolService.Models
{
    public record GetHexResponse
    {
        public required string Hex { get; set; }

        public required int PuzzleCode { get; set; }

        public required string WorkloadStart { get; set; }

        public required string WorkloadEnd { get; set; }

        public required string TargetAddress { get; set; }

        public required List<string> ProofOfWorkAddresses { get; set; }
    }
}
