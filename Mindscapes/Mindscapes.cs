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

            //Set up funni Gabbro stuff
            GlobalMessenger.AddListener("WakeUp", OnceAwake);
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
                    if (i.name.Contains("RSci")) 
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
                    else if (i.name.Contains("Gabbro"))
                    {
                        InvadeTrigger tmp = i.gameObject.AddComponent<InvadeTrigger>();
                        tmp.systemName = "GabbroSystem";
                    }
                    else if (i.name.Contains("Feldspar"))
                    {
                        InvadeTrigger tmp = i.gameObject.AddComponent<InvadeTrigger>();
                        tmp.systemName = "FeldsparSystem";
                    }
                    else if (i.name.Contains("Esker"))
                    {
                        InvadeTrigger tmp = i.gameObject.AddComponent<InvadeTrigger>();
                        tmp.systemName = "EskerSystem";
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

            //Only do this in Feldspar's mind
            else if (s.Equals("FeldsparSystem"))
            {
                //Need to make all of the goals
                Goal.HardReset();
                Transform goalRoot = newHorizons.GetPlanet("Feldspar").transform.Find("Sector/feldspar_area/goals");
                foreach(Transform goal in goalRoot)
                {
                    goal.gameObject.AddComponent<Goal>();
                }
            }

            //Only do this in Esker's mind
            else if (s.Equals("EskerSystem"))
            {
            }
        }

        /**
         * Make the player able to move when waking in Gabbro's mind
         */
        private void OnceAwake()
        {
            //Thanks Hawkbar & Xen!
            //Makes it so that the player can actually move in Gabbro's tunnel
            if (newHorizons.GetCurrentStarSystem().Equals("GabbroSystem")) {
                ModHelper.Events.Unity.FireOnNextUpdate(() =>
                {
                    Locator.GetPlayerController().EnableZeroGMovement();
                });
            }

            //Messes with the ship when entering Esker's mind
            if (newHorizons.GetCurrentStarSystem().Equals("EskerSystem"))
            {
                ModHelper.Events.Unity.FireOnNextUpdate(() =>
                {
                    //Damage the components
                    ShipDamageController damageController = Locator.GetShipBody().gameObject.GetComponent<ShipDamageController>();
                    foreach (ShipComponent comp in damageController._shipComponents)
                    {
                        if (!comp.isInternalRepairPoint)
                        {
                            comp.SetDamaged(true);
                        }
                    }
                    foreach (ShipHull hull in damageController._shipHulls)
                    {
                        hull.ApplyDebugImpact();
                    }

                    //Shut the hatch
                    Transform hatchRoot = Locator.GetShipBody().transform.Find("Module_Cabin/Systems_Cabin/Hatch");
                    hatchRoot.gameObject.GetComponentInChildren<HatchController>().CloseHatch();
                    hatchRoot.Find("TractorBeam").gameObject.SetActive(false);
                });

                //Listen for ship repairs
                Locator.GetShipBody().GetComponent<ShipDamageController>().OnShipComponentRepaired += OnShipRepair;
                Locator.GetShipBody().GetComponent<ShipDamageController>().OnShipHullRepaired += OnShipRepair;
            }
        }

        /**
         * Some things need to happen weird
         */
        private void Update()
        {
            //Do a super specific fix for Riebeck's platforms
            if(prepPlatFix && !samplePlatform.activeSelf)
            {
                prepPlatFix = false;
                Transform platRoot = newHorizons.GetPlanet("Riebeck").transform.Find("Sector/riebeck_area/platforms");
                foreach(DetachableFragment plat in platRoot.GetComponentsInChildren<DetachableFragment>(true)) 
                {
                    plat.gameObject.SetActive(true);
                }
            }
        }

        /**
         * When ship components and hull are repaired, check if we should advance Esker's dialogue
         */
        private void OnShipRepair(Component garb)
        {
            if(!Locator.GetShipBody().GetComponent<ShipDamageController>().IsDamaged())
            {
                DebugPrint("Ship repaired!");
                DialogueConditionManager.SharedInstance.SetConditionState("SHIP_FIXED", true);
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