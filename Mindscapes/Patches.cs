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

        /**
         * Listen for key conditions being set to alter the game state
         * 
         * @param conditionName The name of the condition being set
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DialogueConditionManager), nameof(DialogueConditionManager.SetConditionState))]
        public static void ConditionListener(string conditionName)
        {
            //Restart the race when the player accepts
            if (conditionName.Equals("RACE_STARTED"))
            {
                Goal.SoftReset();
            }

            //Remove the blocker when the player fools the guard
            else if (conditionName.Equals("GUARDFOOLED"))
            {
                Mindscapes.instance.newHorizons.GetPlanet("Slate").transform.Find("Sector/slate_area/house/blocker").gameObject.SetActive(false);
            }

            //Add the vision torch when Slate gets mad
            else if (conditionName.Equals("SLATEMAD"))
            {
                Transform sectorRoot = Mindscapes.instance.newHorizons.GetPlanet("Slate").transform.Find("Sector");
                sectorRoot.Find("VisionTorch").gameObject.SetActive(true);
                sectorRoot.Find("slate_area/house/torch_light").gameObject.SetActive(true);
            }

            //Open the black hole when Slate says they did
            else if (conditionName.Equals("EXITOPENED"))
            {
                Transform sectorRoot = Mindscapes.instance.newHorizons.GetPlanet("Slate").transform.Find("Sector");
                sectorRoot.Find("slate_singularity").gameObject.SetActive(true);
                sectorRoot.Find("credits_volume").gameObject.SetActive(true);
            }
        }

        /**
         * If any mind projection completes in Slate's system, assume that it was the de-angering one
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(VisionTorchItem), nameof(VisionTorchItem.OnProjectionComplete))]
        public static void DefuseSlate()
        {
            if (Mindscapes.instance.newHorizons.GetCurrentStarSystem().Equals("SlateSystem"))
            {
                DialogueConditionManager.SharedInstance.SetConditionState("SLATETORCHED", true);
            }
        }
    }
}
