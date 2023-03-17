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

        public override ObjectMetadata ModifyObject(ObjectMetadata sourceData)
        {
            return RepositionObject(sourceData);
        }

        public virtual ObjectMetadata RepositionObject(ObjectMetadata sourceData)
        {
            return RepositionObject(sourceData, sourceData.ObjectThisReplaced);
        }

        public virtual ObjectMetadata RepositionObject(ObjectMetadata sourceData, ObjectMetadata replacedObject)
        {
            if (replacedObject == null)
                return sourceData;

            sourceData.MatchPositionOfOther(replacedObject);
            return sourceData;
        }

        public override void Setup(EnemyRandomizerDatabase database)
        {
            base.Setup(database);
            EnemyRandomizer.Instance.enemyReplacer.loadedLogics.Add(this);
        }
    }
}
