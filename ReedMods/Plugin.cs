using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ReedMods.Patches;
using System.IO;
using System.Reflection;
using UnityEngine;

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

        // Assets
        public static AssetBundle ReedModsAssets;

        internal static AudioClip[]? shovelSFX;

        void Awake() 
        { 
            if (Instance == null)
            {
                Instance = this;
            }

            logSource = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            logSource.LogInfo("ReedMods woke up");
            harmony.PatchAll(typeof(ReedModsBase));

            // player controller patch
            harmony.PatchAll(typeof(PlayerControllerBPatch));

            // load asset bundle
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            ReedModsAssets = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "reedmodsassets"));
            if (ReedModsAssets == null)
            {
                logSource.LogError("Failed to load reedmodsassets assets.");
                return;
            }

            // loading shovel sfx
            shovelSFX = ReedModsAssets.LoadAssetWithSubAssets<AudioClip>("Assets/boing.mp3");
            harmony.PatchAll(typeof(ShovelSoundsPatch));

            logSource.LogInfo("ReedMods finished loading");
        }
    }
}
