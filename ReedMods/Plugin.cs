using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ReedMods.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReedMods
{

    [BepInPlugin(modGUID, modName, modVersion)]
    public class ReedModsBase: BaseUnityPlugin
    {
        // must be unique amongst all other mods
        private const string modGUID = "ReedDoesStuff.ReedMods";
        private const string modName = "ReedMods";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        // singleton
        private static ReedModsBase Instance;

        internal ManualLogSource logSource;

        void Awake() 
        { 
            if (Instance == null)
            {
                Instance = this;
            }

            logSource = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            logSource.LogInfo("ReedMods woke up");

            harmony.PatchAll(typeof(ReedModsBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
        }
    }
}
