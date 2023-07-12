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

            //If it's the first one, save the mat and ready it
            if(goals.Count == 1)
            {
                readyMat = renderer.material;
                Activate();
            }

            //If it's the second one, just save the mat
            if(goals.Count == 2)
            {
                unreadyMat = renderer.material;
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
                Deactivate();
                raceIndex++;
                if (raceIndex < goals.Count)
                {
                    goals[raceIndex].Activate();
                }
                else
                {
                    Mindscapes.DebugPrint("race won!");
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
    }
}
