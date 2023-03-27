namespace EnemyRandomizerMod
{
    public class WhitePalaceFlyControl : DefaultSpawnedEnemyControl
    {
        public override void Setup(ObjectMetadata other)
        {
            base.Setup(other);

            //TODO: some logic to determine if it's safe to leave the enemy as invincible
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
