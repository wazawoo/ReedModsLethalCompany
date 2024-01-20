using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
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
