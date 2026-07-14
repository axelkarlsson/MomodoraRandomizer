using MomodoraRandomizer.Data.Enums;
using System.Collections.Generic;
using System.Diagnostics;

namespace MomodoraRandomizer.Data.Metadata {
    internal static class GameVersionMetadata {
        internal static Dictionary<int, GameVersion> GameVersions = new Dictionary<int, GameVersion>() {
            { 39690240, GameVersion.VERSION_1_05b },
            { 40222720, GameVersion.VERSION_1_07 }
        };

        /// <summary>
        ///     Gets the version of the Game based on the module memory size.
        /// </summary>
        /// <param name="gameProcess">Process whose version is being evaluated</param>
        /// <returns>
        ///     <seealso cref="GameVersion"/> if it's a supported version<br/>
        ///     <seealso cref="GameVersion.NONE"/> if the process is <seealso cref="null"/><br/>
        ///     <seealso cref="GameVersion.UNSUPPORTED"/> otherwise.
        /// </returns>
        internal static GameVersion GetGameVersion(Process gameProcess) {
            if (gameProcess == null) {
                return GameVersion.NONE;
            }

            return GameVersions.TryGetValue(gameProcess.MainModule.ModuleMemorySize, out var gameVersion) ? gameVersion : GameVersion.UNSUPPORTED;
        }

        ///<summary>
        ///     Checks that the version sent is supported
        ///</summary>
        ///<param name="gameVersion">Version of the game to check</param>
        ///<returns>
        ///     <c>true</c> if the version is valid; otherwise, <c>false</c>.
        ///</returns>
        internal static bool IsValidGameVersion(GameVersion gameVersion)
            => gameVersion != GameVersion.NONE && gameVersion != GameVersion.UNSUPPORTED;
    }
}
