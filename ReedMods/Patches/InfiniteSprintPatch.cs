using GameNetcodeStuff;
using HarmonyLib;

namespace ReedMods.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class InfiniteSprintPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void infiniteSprintPatch(ref float ___sprintMeter)
        {
            ___sprintMeter = 1f;
        }
    }
}
