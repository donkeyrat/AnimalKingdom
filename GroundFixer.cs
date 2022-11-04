using System.Reflection;
using System;
using UnityEngine;
using HarmonyLib;
using TFBGames;
using System.Collections.Generic;

namespace AnimalKingdom {
    [HarmonyPatch(typeof(GroundChecker), "OnCollisionEnter")]
    class GroundFixer {
        [HarmonyPrefix]
        public static bool Prefix(GroundChecker __instance, Collision collision) {

            if (__instance.ignoreRigidbodies && collision.transform.root.name == "Map")
            {
                __instance.ignoreRigidbodies = false;
            }
            return true;
        }
    }
}