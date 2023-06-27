using UnityEngine;

namespace EnemyRandomizerMod
{
    public class CorpseRemover : SpawnEffect
    {
        public bool useOwner = false;
        public GameObject owner;

        public GameObject corpseTarget;
        public Vector2 lastPos;

        public override bool destroyGameObject => true;

        public bool didSpawn = false;
        public bool preRemoveCorpse = true;

        protected virtual void Update()
        {
            if (isUnloading)
                return;

            if (owner != null)
                lastPos = owner.transform.position;
            CheckRemoveAndSpawn();
        }

        protected virtual void CheckRemoveAndSpawn()
        {
            if (isUnloading)
                return;

            if (didSpawn)
                return;

            if (preRemoveCorpse)
            {
                if (corpseTarget != null)
                {
                    SpawnerExtensions.DestroyObject(corpseTarget);
                    corpseTarget = null;
                }

                if (!useOwner)
                {
                    Spawn(owner, lastPos);
                }
            }

            if (useOwner)
            {
                bool hasHm = owner != null && owner.GetComponent<HealthManager>() != null;
                bool isDead = false;
                if (owner != null &&
                    owner.GetComponent<HealthManager>() != null &&
                    (owner.GetComponent<HealthManager>().hp <= 0 || owner.GetComponent<HealthManager>().isDead))
                    isDead = true;

                if (owner == null || 
                   (!hasHm && !owner.activeInHierarchy) || 
                    isDead)
                {
                    Spawn(owner, lastPos);
                    didSpawn = true;
                }

                //var parent = transform.parent;
                //if (parent == null)
                //{
                //    Spawn(owner, lastPos);
                //    didSpawn = true;
                //}
                //else
                //{
                    //var parentName = parent.name;
                    //var thisName = gameObject.name;

                    //parentName = EnemyRandomizerDatabase.ToDatabaseKey(parentName);
                    //thisName = thisName.Remove("Corpse");
                    //thisName = EnemyRandomizerDatabase.ToDatabaseKey(thisName);

                    //if (!parentName.Contains(thisName))
                    //{
                    //    Spawn(owner, lastPos);
                    //    didSpawn = true;
                    //}
                //}
            }
        }
    }
}
