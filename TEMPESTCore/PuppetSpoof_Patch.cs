using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace TEMPESTCore
{
    /// <summary>
    /// HARMONY PATCHHHHH HARMONY MAXIMUN TECHNIQUE RAHHHHHHHHHH
    /// WAIT FUCK THIS DOESNT WORK WHAT THE FUCK AM I DOING :SOBG:
    /// </summary>
    [HarmonyPatch(typeof(EnemyIdentifier))]
    public static class PuppetSpoof_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("PuppetSpawn")]
        static bool CancelPuppetSpawn(EnemyIdentifier __instance)
        {
            return !__instance.TryGetComponent<Ghosted>(out var g) || !g.ghosted;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        [HarmonyPatch("UpdateModifiers")]
        [HarmonyPatch("ProcessDeath")]
        [HarmonyPatch("AfterShock")]
        public static void Prefix(EnemyIdentifier __instance, out bool __state)
        {
            __state = __instance.puppet;

            if (__instance.TryGetComponent<PuppetSpoof>(out var spoof) && spoof.active)
            {
                __instance.puppet = false;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        [HarmonyPatch("UpdateModifiers")]
        [HarmonyPatch("ProcessDeath")]
        [HarmonyPatch("AfterShock")]
        public static void Postfix(EnemyIdentifier __instance, bool __state)
        {
            __instance.puppet = __state;
        }
    }
}
