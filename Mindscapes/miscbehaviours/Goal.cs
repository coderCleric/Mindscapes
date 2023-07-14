using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mindscapes.miscbehaviours
{
    public class Goal : MonoBehaviour
    {
        //Static stuff
        private static List<Goal> goals = new List<Goal>();
        private static Material readyMat = null;
        private static Material unreadyMat = null;
        private static int raceIndex = 0;
        private static float startTime = -1;
        private static float raceDuration = 50;

        //Instanced
        private bool isNext = false;
        private MeshRenderer renderer = null;
        private OWTriggerVolume trigger = null;

        /**
         * When we make one, add it to the list.
         */
        private void Awake()
        {
            goals.Add(this);
            renderer = GetComponent<MeshRenderer>();
            trigger = GetComponent<OWTriggerVolume>();
            trigger.OnEntry += OnEnter;

            //If it's the first one, save the mat
            if(goals.Count == 1)
            {
                readyMat = renderer.material;
            }

            //If it's the second one, just save the mat
            if(goals.Count == 2)
            {
                unreadyMat = renderer.material;
                goals[0].Deactivate();
            }
        }

        /**
         * Activate this goal
         */
        private void Activate()
        {
            isNext = true;
            renderer.material = readyMat;
        }

        /**
         * Deactivate this goal
         */
        private void Deactivate()
        {
            isNext = false;
            renderer.material = unreadyMat;
        }

        /**
         * When the ship enters the correct goal, advance
         */
        private void OnEnter(GameObject other)
        {
            if(other.CompareTag("ShipDetector") && isNext)
            {
                Mindscapes.DebugPrint("Goal hit!");
                startTime = Time.time;
                Deactivate();
                raceIndex++;
                if (raceIndex < goals.Count)
                {
                    goals[raceIndex].Activate();
                }
                else
                {
                    Mindscapes.DebugPrint("race won!");
                    DialogueConditionManager.SharedInstance.SetConditionState("RACE_WON", true);
                }
            }
        }

        /**
         * Unlink the trigger if the component is destroyed
         */
        private void OnDestroy()
        {
            trigger.OnEntry -= OnEnter;
        }

        /**
         * Check if the player has failed the race
         */
        private void Update()
        {
            if (startTime > 0 && Time.time > startTime + raceDuration)
                DisableRace();
        }

        /**
         * Resets the state of the race, good for if the player fails
         */
        public static void SoftReset()
        {
            if (goals.Count == 0)
                return;

            goals[0].Activate();
            raceIndex = 0;
            for(int i = 1; i < goals.Count; i++)
            {
                goals[i].Deactivate();
            }
        }

        /**
         * Resets the entire race, used for loops and such
         */
        public static void HardReset()
        {
            goals = new List<Goal>();
            raceIndex = 0;
        }

        /**
         * Disables the race
         */
        public static void DisableRace()
        {
            raceIndex = 0;
            startTime = -1;
            DialogueConditionManager.SharedInstance.SetConditionState("RACE_FAILED", true);
            foreach(Goal goal in goals)
            {
                goal.Deactivate();
            }
        }
    }
}
