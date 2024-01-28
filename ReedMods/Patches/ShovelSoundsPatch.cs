using HarmonyLib;
using UnityEngine;

namespace ReedMods.Patches
{
    [HarmonyPatch(typeof(Shovel))]
    internal class ShovelSoundsPatch
    {
        [HarmonyPatch("HitShovelClientRpc")]
        [HarmonyPatch("HitShovel")]
        [HarmonyPrefix]
        public static void hitSFXPatch(ref AudioClip[] ___hitSFX)
        {
            AudioClip[] newHitSFX = ReedModsBase.shovelSFX;
            ___hitSFX = newHitSFX;
        }
    }
}
