using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Mindscapes.miscbehaviours;
using UnityEngine;

namespace Mindscapes
{
    [HarmonyPatch]
    public static class Patches
    {
        /**
         * Stop dialogue from triggering if there's an active invade trigger
         * 
         * @param __instance The instance of the dialogue tree
         */
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.OnPressInteract))]
        public static bool SuppressDialogue(CharacterDialogueTree __instance)
        {
            return !(__instance.gameObject.GetComponent<InvadeTrigger>() != null && InvadeTrigger.shouldBeActive);
        }
    }
}
