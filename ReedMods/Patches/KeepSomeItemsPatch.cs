using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace ReedMods.Patches
{
    // lmao itemId is not a unique id...
    // its like "ItemTypes.LargeAxle" so it kept all of those...
    // leaving that for now. kinda chill.

    [HarmonyPatch]
    internal class KeepSomeItemsPatch
    {
        [HarmonyPatch(typeof(RoundManager), "DespawnPropsAtEndOfRound")]
        [HarmonyPrefix]
        public static void PatchGameOverPre(RoundManager __instance)
        {
            ReedModsBase.Instance.logSource.LogInfo((object)"xxx DespawnPropsAtEndOfRound is about to start");
            if (!__instance.IsServer) {
               ReedModsBase.Instance.logSource.LogInfo((object)"xxx this aint a server, we out ✌️");
               return;
            }

            GrabbableObject[] objects = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();
            ReedModsBase.Instance.logSource.LogInfo((object)("xxx there are  " + objects.Length + "items..."));
            
            List<GrabbableObject> scrapsInShipRoom = objects.Where(
                obj => obj.itemProperties.isScrap && obj.isInShipRoom
            ).ToList();

            ReedModsBase.Instance.logSource.LogInfo((object)("xxx there are  " + scrapsInShipRoom.Count + "scraps in ship..."));

            // keep one item (should probable shuffle)
            if (scrapsInShipRoom.Count > 0) {
                var scrapsToKeep = scrapsInShipRoom
                .Take(1)
                .ToList();

                foreach (var scrap in scrapsToKeep) {
                    ReedModsBase.Instance.logSource.LogInfo((object)("xxx keeping a lovely " + scrap.itemProperties.itemName));
                    scrap.itemProperties.isScrap = false;
                }

                // you're more than just scrap in our hearts
                ReedModsBase.Instance.scrapIDsToKeep = scrapsToKeep
                .Select(keeper => keeper.itemProperties.itemId).ToList();

            } else {
                ReedModsBase.Instance.logSource.LogInfo((object)("xxx nothing to keep, big r.i.p."));
            }
        }

        // DespawnPropsAtEndOfRound START

        //public void DespawnPropsAtEndOfRound(bool despawnAllItems = false)
        //{
        //    if (!base.IsServer)
        //    {
        //        return;
        //    }
        //    GrabbableObject[] array = Object.FindObjectsOfType<GrabbableObject>();
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        if (despawnAllItems || (!array[i].isHeld && !array[i].isInShipRoom) || array[i].deactivated || (StartOfRound.Instance.allPlayersDead && array[i].itemProperties.isScrap))
        //        {
        //            if (array[i].isHeld && array[i].playerHeldBy != null)
        //            {
        //                array[i].playerHeldBy.DropAllHeldItemsAndSync();
        //            }
        //            NetworkObject component = array[i].gameObject.GetComponent<NetworkObject>();
        //            if (component != null && component.IsSpawned)
        //            {
        //                array[i].gameObject.GetComponent<NetworkObject>().Despawn();
        //            }
        //            else
        //            {
        //                Debug.Log("Error/warning: prop '" + array[i].gameObject.name + "' was not spawned or did not have a NetworkObject component! Skipped despawning and destroyed it instead.");
        //                Object.Destroy(array[i].gameObject);
        //            }
        //        }
        //        else
        //        {
        //            array[i].scrapPersistedThroughRounds = true;
        //        }
        //        if (spawnedSyncedObjects.Contains(array[i].gameObject))
        //        {
        //            spawnedSyncedObjects.Remove(array[i].gameObject);
        //        }
        //    }
        //    GameObject[] array2 = GameObject.FindGameObjectsWithTag("TemporaryEffect");
        //    for (int j = 0; j < array2.Length; j++)
        //    {
        //        Object.Destroy(array2[j]);
        //    }
        //}

        // DespawnPropsAtEndOfRound END

        [HarmonyPatch(typeof(RoundManager), "DespawnPropsAtEndOfRound")]
        [HarmonyPostfix]
        public static void PatchGameOverPost(RoundManager __instance)
        {
            ReedModsBase.Instance.logSource.LogInfo((object)"xxx DespawnPropsAtEndOfRound ended");

            GrabbableObject[] objects = UnityEngine.Object.FindObjectsOfType<GrabbableObject>();

            var scrapIDsToKeep = ReedModsBase.Instance.scrapIDsToKeep;

            foreach (var obj in objects) {
                if (scrapIDsToKeep.Where( id => id == obj.itemProperties.itemId).ToList().Count > 0) {
                    obj.itemProperties.isScrap = true;
                    ReedModsBase.Instance.logSource.LogInfo((object)"xxx marking " + obj.itemProperties.itemName + " back to scrap ");
                    ReedModsBase.Instance.scrapIDsToKeep = new();
                }
            }
        }

        // this is just to avoid the overlay saying everyone died and "ALL SCRAP LOST"
        // (code from KeepScrap)

        [HarmonyPatch(typeof(HUDManager), "FillEndGameStats")]
        [HarmonyPrefix]
        public static void PatchHUDPre(HUDManager __instance)
        {
            ReedModsBase.Instance.logSource.LogInfo((object)"FillEndGameStats started");
            ReedModsBase.Instance.logSource.LogInfo((object)("All players dead is " + StartOfRound.Instance.allPlayersDead));
            ReedModsBase.Instance.allPlayersDeadPreviously = StartOfRound.Instance.allPlayersDead;
            StartOfRound.Instance.allPlayersDead = false;
            ReedModsBase.Instance.logSource.LogInfo((object)"Crashed?");
        }

        [HarmonyPatch(typeof(HUDManager), "FillEndGameStats")]
        [HarmonyPostfix]
        public static void PatchHUDPost(HUDManager __instance)
        {
            ReedModsBase.Instance.logSource.LogInfo((object)"FillEndGameStats ended");
            ReedModsBase.Instance.logSource.LogInfo((object)("All players dead is " + StartOfRound.Instance.allPlayersDead));
            ReedModsBase.Instance.logSource.LogInfo((object)("All players dead previously is " + ReedModsBase.Instance.allPlayersDeadPreviously));
            StartOfRound.Instance.allPlayersDead = ReedModsBase.Instance.allPlayersDeadPreviously;
            ReedModsBase.Instance.logSource.LogInfo((object)"Crashed?");
        }

        [HarmonyPatch(typeof(StartOfRound), "Start")]
        [HarmonyPostfix]
        public static void PatchGameOverText(StartOfRound __instance)
        {
            ReedModsBase.Instance.logSource.LogInfo((object)("GameOverDialogue currently is " + __instance.gameOverDialogue[1].bodyText));
            __instance.gameOverDialogue[1].bodyText = "We kept some items for you. xoxo";
            ReedModsBase.Instance.logSource.LogInfo((object)"Changed gameOverDialogue");
        }
    }
}
