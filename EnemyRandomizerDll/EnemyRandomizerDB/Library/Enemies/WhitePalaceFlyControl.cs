
using Satchel;
using Satchel.Futils;

namespace EnemyRandomizerMod
{
    public class WhitePalaceFlyControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            thisMetadata.EnemyHealthManager.hp = 1;
        }


        /// <summary>
        /// This needs to be set each frame to make the palace fly killable
        /// </summary>
        protected virtual void Update()
        {
            if(thisMetadata.EnemyHealthManager.hp > 1)
                thisMetadata.EnemyHealthManager.hp = 1;
        }
    }

    public class WhitePalaceFlySpawner : DefaultSpawner<WhitePalaceFlyControl>
    {
    }

    public class WhitePalaceFlyPrefabConfig : DefaultPrefabConfig<WhitePalaceFlyControl>
    {
    }
}
