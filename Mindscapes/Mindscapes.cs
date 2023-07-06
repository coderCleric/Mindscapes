using OWML.Common;
using OWML.ModHelper;
using UnityEngine.Events;
using UnityEngine;
using Mindscapes.miscbehaviours;

namespace Mindscapes
{
    public class Mindscapes : ModBehaviour
    {
        private static Mindscapes instance;

        /**
         * Need to do some prep work on start
         */
        private void Start()
        {
            // Say that it worked
            ModHelper.Console.WriteLine($"{nameof(Mindscapes)} is loaded!", MessageType.Success);

            instance = this;

            // Get the New Horizons API and load configs
            INewHorizons newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            newHorizons.LoadConfigs(this);

            //Do stuff when the system finishes loading
            UnityEvent<string> loadCompleteEvent = newHorizons.GetStarSystemLoadedEvent();
            loadCompleteEvent.AddListener(PrepSystem);
        }

        /**
         * Do some things when the system loads, mostly prepping more complicated objects
         */
        private void PrepSystem(string s)
        {
            if(s.Equals("SolarSystem")) //Only do things in the base-game system
            {
                //Put the necessary component on the device
                GameObject.Find("TimberHearth_Body/Sector_TH/mind_exhibit/mind_device").AddComponent<MindDevice>();
            }
        }

        /**
         * A simple debug print
         */
        public static void DebugPrint(string msg)
        {
            instance.ModHelper.Console.WriteLine(msg);
        }
    }
}