using System;

namespace EnemyRandomizerMod
{
    public class InfiniteEnemy : CustomEnemySpeed
    {
        public const int INFINITE_HEALTH = 9999;

        public double danceSpeedIncreaseDmg { get; private set; } = 10000.0;
        public double startingDanceSpeed { get; private set; } = 1.0;
        public double maxDanceSpeed { get; private set; } = 2.0;
        
        // If you have set the maxHP to the infinite health yet.
        private bool setMaxHP = false;
        
        // This keeps the enemy health infinite by resetting it every time they take damage.
        private void Update()
        {
            if (!active)
                return;
            
            if (cachedHealthManager == null)
            {
                _CalculateNewDanceSpeed();
                cachedHealthManager = gameObject.GetComponent<HealthManager>();
                if (cachedHealthManager == null)
                {
                    SetActive(false);
                    throw new NullReferenceException("Unable to load health manager." +
                                                     " Please set manually, or add a HealthManager to this gameobject." +
                                                     " Setting CustomEnemy to inactive.");
                }                
            }
            
            if (!setMaxHP)
            {
                maxHP = INFINITE_HEALTH;
                cachedHealthManager.hp = INFINITE_HEALTH;
                setMaxHP = true;
            }
            
            
            if (cachedHealthManager.hp == maxHP) return;
            
            damageDone += maxHP - cachedHealthManager.hp;
            cachedHealthManager.hp = maxHP;
            _CalculateNewDanceSpeed();
        }

        // Function hiding is done here to ensure non-weird behavior from setting these with the infinite enemy loaded. 
        public new void OverrideDamageDone(int damage)
        {
            if (active)
            {
                damageDone = damage;
                _CalculateNewDanceSpeed();
            }
            else
            {
                CustomEnemySpeed a = (CustomEnemySpeed) this;
                a.OverrideDamageDone(damage);
            }
        }

        public new void SetEnemyMaxHealth(int health)
        {
            if (active)
            {
                throw new Exception("Unable to set maximum health on an infinite enemy... silly");
            }
            else
            {
                CustomEnemySpeed a = (CustomEnemySpeed) this;
                a.SetEnemyMaxHealth(health);
                setMaxHP = false;
            }
        }

        // These variables are all set indirectly because setting them directly would not cause the dance speed to be
        // updated after changing them.
        public void SetDanceSpeedIncreaseDamage(double dmg)
        {
            danceSpeedIncreaseDmg = dmg;

            if (Math.Abs(danceSpeedIncreaseDmg) <= epsilon)
            {
                throw new Exception("Cannot set dance speed increase damage to " + danceSpeedIncreaseDmg + " because it" +
                                    " would almost certainly cause undefined behavior.");
            }
            _CalculateNewDanceSpeed();
        }

        public void SetStartingDanceSpeed(double speed)
        {
            startingDanceSpeed = speed;
            _CalculateNewDanceSpeed();
        }

        public void SetMaxDanceSpeed(double speed)
        {
            if (speed <= epsilon)
            {
                throw new Exception("Cannot set max dance speed to " + speed + " because it" +
                                    " would almost certainly cause undefined behavior.");
            }
            maxDanceSpeed = speed;
            _CalculateNewDanceSpeed();
        }


        private void _CalculateNewDanceSpeed()
        {
            double dance = startingDanceSpeed + (damageDone / danceSpeedIncreaseDmg);
            if (dance > maxDanceSpeed)
            {
                dance = maxDanceSpeed;
            }
            UpdateDanceSpeed(dance);
        }
        
        
        
    }
}