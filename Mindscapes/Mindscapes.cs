using OWML.Common;
using OWML.ModHelper;
using UnityEngine.Events;
using UnityEngine;
using Mindscapes.miscbehaviours;
using HarmonyLib;
using System.Reflection;

namespace Mindscapes
{
    public class Mindscapes : ModBehaviour
    {
        public static Mindscapes instance;
        public INewHorizons newHorizons = null;
        private GameObject samplePlatform = null;
        private bool prepPlatFix = false;

        /**
         * Need to do some prep work on start
         */
        private void Start()
        {
            // Say that it worked
            ModHelper.Console.WriteLine($"{nameof(Mindscapes)} is loaded!", MessageType.Success);

            instance = this;

            // Get the New Horizons API and load configs
            newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            newHorizons.LoadConfigs(this);

            //Do stuff when the system finishes loading
            UnityEvent<string> loadCompleteEvent = newHorizons.GetStarSystemLoadedEvent();
            loadCompleteEvent.AddListener(PrepSystem);//Make all of the patches
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            //Make all of the patches
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        }

        /**
         * Do some things when the system loads, mostly prepping more complicated objects
         */
        private void PrepSystem(string s)
        {
            //Only do these things in the base-game system
            if (s.Equals("SolarSystem"))
            {
                //Put the necessary component on the device
                GameObject.Find("TimberHearth_Body/Sector_TH/mind_exhibit/mind_device").AddComponent<MindDevice>();

                //Make the mind invasion zones
                InvadeTrigger.ResetTriggerList(); //Need to clear any previous loop
                foreach(CharacterDialogueTree i in Component.FindObjectsOfType<CharacterDialogueTree>())
                {
                    if (i.name.Contains("RSci") || i.name.Contains("Gabbro") || i.name.Contains("Esker") || i.name.Contains("Feldspar")) 
                    {
                        i.gameObject.AddComponent<InvadeTrigger>();
                    }
                    else if (i.name.Contains("Riebeck"))
                    {
                        InvadeTrigger tmp = i.gameObject.AddComponent<InvadeTrigger>();
                        tmp.systemName = "RiebeckSystem";
                    }
                    else if (i.name.Contains("Chert"))
                    {
                        InvadeTrigger tmp = i.gameObject.AddComponent<InvadeTrigger>();
                        tmp.systemName = "ChertSystem";
                    }
                }
            }

            //Only do this in Riebeck's mind
            else if(s.Equals("RiebeckSystem"))
            {
                //Need to make the platforms actually show up
                Transform platRoot = newHorizons.GetPlanet("Riebeck").transform.Find("Sector/riebeck_area/platforms");
                samplePlatform = platRoot.GetComponentInChildren<DetachableFragment>().gameObject;
                prepPlatFix = true;
            }

            //Only do this in Chert's mind
            else if(s.Equals("ChertSystem"))
            {
                //Need to attach the controller to the fake sun
                newHorizons.GetPlanet("Chert").transform.Find("Sector/chert_area/fakesun").gameObject.AddComponent<FakeSunController>();
            }
        }

        /**
         * Riebeck's platforms are a real pain
         */
        private void Update()
        {
            if(prepPlatFix && !samplePlatform.activeSelf)
            {
                DebugPrint("fixing");
                prepPlatFix = false;
                Transform platRoot = newHorizons.GetPlanet("Riebeck").transform.Find("Sector/riebeck_area/platforms");
                foreach(DetachableFragment plat in platRoot.GetComponentsInChildren<DetachableFragment>(true)) 
                {
                    DebugPrint("fixing internal");
                    plat.gameObject.SetActive(true);
                }
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