using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mindscapes.miscbehaviours
{
    public class FakeSunController : OWItem
    {
        private OWTriggerVolume hitTrigger = null;
        private bool hasFallen = false;
        private bool isFalling = false;
        private float fallSpeed = 0;
        private float accel = 12;
        private float endY = 0.25f;

        /**
         * Give the correct display name
         */
        public override string GetDisplayName()
        {
            return "Star";
        }

        /**
         * Set up the trigger on awake
         */
        public override void Awake()
        {
            hitTrigger = gameObject.GetComponentInChildren<OWTriggerVolume>();
            hitTrigger.OnEntry += OnHit;
            base.Awake();
            _localDropOffset = new Vector3(0, 0.25f, 0);
        }

        /**
         * When it's picked up, update Chert's dialogue
         */
        public override void PickUpItem(Transform holdTranform)
        {
            base.PickUpItem(holdTranform);
            DialogueConditionManager.SharedInstance.SetConditionState("STAR_RETRIEVED", true);
        }

        /**
         * When hit by the scout, start falling
         */
        private void OnHit(GameObject other)
        {
            if(!hasFallen && other.CompareTag("ProbeDetector"))
            {
                Mindscapes.DebugPrint("star should fall");
                hasFallen = true;
                isFalling = true;
            }
        }

        /**
         * Don't feel like setting up an actual RigidBody, so we're just gonna do a fake fall
         */
        private void Update()
        {
            if (isFalling)
            {
                //First, accellerate
                fallSpeed += accel * Time.deltaTime;

                //Then, actually move
                float y = transform.localPosition.y - (fallSpeed * Time.deltaTime);
                if(y <= endY) //Make sure we don't overshoot
                {
                    isFalling = false;
                    transform.SetLocalPositionY(endY);
                }
                else
                {
                    transform.SetLocalPositionY(y);
                }
            }
        }

        /**
         * Unlink the trigger if we're destroyed
         */
        public override void OnDestroy()
        {
            hitTrigger.OnEntry -= OnHit;
            base.OnDestroy();
        }
    }
}
