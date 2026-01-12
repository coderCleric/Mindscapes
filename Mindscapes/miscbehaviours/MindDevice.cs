using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mindscapes.miscbehaviours
{
    public class MindDevice : OWItem
    {
        private string _displayName;

        public override void Awake()
        {
            base.Awake();

            // We get this value once and cache it instead of doing it in GetDisplayName where it would be called repeatedly
            _displayName = Mindscapes.instance.newHorizons.GetTranslationForOtherText("MINDSCAPES_NOMAI_DEVICE");
        }

        /**
         * Gives the display name
         */
        public override string GetDisplayName()
        {
            return _displayName;
        }

        /**
         * Activates the mind invasion volumes when the item is picked up
         */
        public override void PickUpItem(Transform holdTranform)
        {
            base.PickUpItem(holdTranform);
            InvadeTrigger.ActivateTriggers();
        }

        /**
         * Deactivates the mind invasion volumes when the item is put down
         */
        public override void DropItem(Vector3 position, Vector3 normal, Transform parent, Sector sector, IItemDropTarget customDropTarget)
        {
            base.DropItem(position, normal, parent, sector, customDropTarget);
            InvadeTrigger.DeactivateTriggers();
        }
    }
}
