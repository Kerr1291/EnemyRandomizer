using UnityEngine;

namespace EnemyRandomizerMod
{
    public class CrystallisedLazerBugControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            //TODO: some logic to determine if it's safe to leave the enemy as invincible
            thisMetadata.EnemyHealthManager.IsInvincible = false;
        }
    }

    public class CrystallisedLazerBugSpawner : DefaultSpawner<CrystallisedLazerBugControl>
    {
    }

    public class CrystallisedLazerBugPrefabConfig : DefaultPrefabConfig<CrystallisedLazerBugControl>
    {
    }
}