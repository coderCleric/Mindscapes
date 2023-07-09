using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mindscapes.miscbehaviours
{
    public class InvadeTrigger : MonoBehaviour
    {
        //Instanced things
        private SingleInteractionVolume interactVolume = null;
        private string originalPromptText = null;
        public string systemName = "SlateSystem";

        //Static variables
        public static bool shouldBeActive = false;
        private static List<InvadeTrigger> triggers = new List<InvadeTrigger>();

        /**
         * Do some basic setup on awake
         */
        private void Awake()
        {
            Mindscapes.DebugPrint("Making an invasion trigger in " + this.name + " under " + this.transform.parent.name);

            interactVolume = this.GetRequiredComponent<SingleInteractionVolume>();
            interactVolume.OnPressInteract += OnPressInteract;

            triggers.Add(this);
        }

        /**
         * Change the behaviour depending on if the object is properly active
         */
        private void OnPressInteract()
        {
            if (shouldBeActive)
            {
                Mindscapes.instance.newHorizons.ChangeCurrentStarSystem(systemName); //Warp to this trigger's system
            }
        }

        /**
         * Unlink the trigger if we're destroyed
         */
        private void OnDestroy()
        {
            interactVolume.OnPressInteract -= OnPressInteract;
        }

        /**
         * Needs to be called on system load
         */
        public static void ResetTriggerList()
        {
            shouldBeActive = false;
            triggers.Clear();
        }

        /**
         * Activates the triggers
         */
        public static void ActivateTriggers()
        {
            shouldBeActive = true;
            Mindscapes.DebugPrint("test1");
            foreach(InvadeTrigger trigger in triggers)
            {
                Mindscapes.DebugPrint("test2");
                trigger.originalPromptText = trigger.interactVolume._screenPrompt.GetText();
                trigger.interactVolume._screenPrompt.SetText("Invade Mind");
            }
        }

        /**
         * Deactivates the triggers
         */
        public static void DeactivateTriggers()
        {
            shouldBeActive = false;
            foreach (InvadeTrigger trigger in triggers)
            {
                trigger.interactVolume._screenPrompt.SetText(trigger.originalPromptText);
            }
        }
    }
}
