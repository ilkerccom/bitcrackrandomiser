using BitcrackRandomiser.Enums;
using BitcrackRandomiser.Models;
using BitcrackRandomiser.Services.SettingsService;

namespace BitcrackRandomiser.Services.PoolService
{
    /// <summary>
    /// BitcrackRandomiser flagger class
    /// </summary>
    internal static class Flagger
    {
        /// <summary>
        /// Flag/mark current HEX as scanned on the pool
        /// </summary>
        /// <param name="settings">Active settings model</param>
        /// <param name="hex">HEX to flag/mark</param>
        /// <param name="gpuIndex">GPU index</param>
        /// <param name="proofKey">Proof key (Without hashed)</param>
        /// <param name="gpuName">GPU name</param>
        /// <returns>
        /// true: flag success
        /// false: flag failed
        /// </returns>
        public static bool Flag(Setting settings, string hex, int gpuIndex, string proofKey, string gpuName = "-")
        {
            // Hash all proof keys with SHA256
            string hashedProofKey = Helper.SHA256Hash(proofKey);
            if (settings.AppType == AppType.vanitysearch || settings.AppType == AppType.cpu)
            {
                var proofKeys = Enumerable.Range(0, proofKey.Length / 64).Select(i => proofKey.Substring(i * 64, 64)).OrderBy(p => p).ToArray();
                hashedProofKey = Helper.SHA256Hash(string.Concat(proofKeys));
            }

            // Try flag
            int gpuCount = settings.AppType == AppType.bitcrack || (settings.AppType == AppType.vanitysearch && settings.GPUSeperatedRange) ? 1 : settings.GPUCount;
            string walletAddress = settings.WorkerName;
            if ((settings.AppType == AppType.bitcrack && settings.GPUCount > 1) || settings.AppType == AppType.vanitysearch && settings.GPUSeperatedRange)
                walletAddress += "_" + gpuIndex;
            bool flagUsed = MainService.FlagHex(settings!.UserToken!, hex, walletAddress, hashedProofKey, proofKey, gpuName, gpuCount, settings.ForceContinue, settings!.TargetPuzzle!).Result;

            // Try flagging
            const int maxTries = 6;
            int flagTries = 1;
            while (!flagUsed && flagTries <= maxTries)
            {
                flagUsed = MainService.FlagHex(settings!.UserToken!, hex, settings.WorkerName, hashedProofKey, proofKey, gpuName, gpuCount, settings.ForceContinue, settings!.TargetPuzzle!).Result;
                Helper.WriteLine($"Flag error... Retrying... {flagTries}/{maxTries} [GPU{gpuIndex}]", MessageType.externalApp, gpuIndex: gpuIndex);
                Thread.Sleep(10000);
                flagTries++;
            }

            // Info
            if (flagUsed)
                Helper.WriteLine($"Range [{hex}] scanned successfully at [GPU{gpuIndex}]... Launching again...", MessageType.success, gpuIndex: gpuIndex);
            else
                Helper.WriteLine($"Range [{hex}] scanned with flag error at [GPU{gpuIndex}]... Launching again...", MessageType.info, gpuIndex: gpuIndex);

            return flagUsed;
        }
    }
}
