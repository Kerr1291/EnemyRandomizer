using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;

namespace EnemyRandomizerMod
{
    public class PositionRandomizedObjects : BaseRandomizerLogic
    {
        public override string Name => "Fix Object Positions";

        public override string Info => "Attempts to correctly reposition objects that have been randomized using the old logic. Does not always work...";

        public override bool EnableByDefault => true;
        public override ObjectMetadata ModifyObject(ObjectMetadata objectToModify, ObjectMetadata originalObject)
        {
            return RepositionObject(objectToModify, originalObject);
        }

        public virtual ObjectMetadata RepositionObject(ObjectMetadata objectToModify, ObjectMetadata originalObject)
        {
            if (originalObject == null)
            {
                if (!objectToModify.IsFlying)
                    objectToModify.PlaceOnGround();
            }
            else
            {
                objectToModify.MatchPositionOfOther(originalObject);
            }

            return objectToModify;
        }

        public override void Setup(EnemyRandomizerDatabase database)
        {
            base.Setup(database);
            EnemyRandomizer.Instance.enemyReplacer.loadedLogics.Add(this);
        }
    }
}
