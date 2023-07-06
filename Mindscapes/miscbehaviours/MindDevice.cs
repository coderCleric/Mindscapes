using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Mindscapes.miscbehaviours
{
    internal class MindDevice : OWItem
    {
        /**
         * Gives the display name
         */
        public override string GetDisplayName()
        {
            return "Nomai Device";
        }

        /**
         * Activates the mind invasion volumes when the item is picked up
         */
        public override void PickUpItem(Transform holdTranform)
        {
            base.PickUpItem(holdTranform);
        }

        /**
         * Deactivates the mind invasion volumes when the item is put down
         */
        public override void DropItem(Vector3 position, Vector3 normal, Transform parent, Sector sector, IItemDropTarget customDropTarget)
        {
            base.DropItem(position, normal, parent, sector, customDropTarget);
        }
    }
}
