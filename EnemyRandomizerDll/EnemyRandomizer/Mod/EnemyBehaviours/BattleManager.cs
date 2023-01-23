using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using Vasi;
using nv;

namespace EnemyRandomizerMod.Behaviours
{
    public class BattleManager : EnemyBehaviour
    {
        private PlayMakerFSM _control;
        public int currentWave = 0;
        public List<List<GameObject>> waveEnemies;
        public int enemyThreshold = -1;
        public int enemyTotal = -1;
        private int enemiesKilled;
        private bool arenaActive = true;
        public string sceneName;

        public void Awake()
        {
            _control = gameObject.LocateMyFSM("Battle Control");
        }

        public void Update()
        {
            if (arenaActive)
            {
                if (enemiesKilled == waveEnemies[currentWave].Count)
                {
                    enemiesKilled = 0;
                    currentWave++;
                    if (currentWave == waveEnemies.Count)
                    {
                        arenaActive = false;
                        if(sceneName == "White_Palace_02")
                            _control.SetState("Open");
                        else
                            _control.SetState("End");
                        Dev.Log("Sending BG OPEN event");
                    }
                    else
                    {
                        foreach (GameObject enemy in waveEnemies[currentWave])
                        {
                            enemy.SetActive(true);
                        }
                    }
                }
            }
            /*
            if(enemyThreshold == -1)
            {
                enemyThreshold = _control.Fsm.GetFsmInt("Battle Enemies").Value - waveEnemies[0].Count;
                Dev.Log("Enemy Threshold : " + enemyThreshold);
            }
            //bool waveCompleted = true;
            Dev.Log("Checking wave");
            Dev.Log(_control.Fsm.GetFsmInt("Battle Enemies").Value + " : " + enemyThreshold);
            if (_control.Fsm.GetFsmInt("Battle Enemies").Value == enemyThreshold)
            {
                Dev.Log("Incrementing wave");
                currentWave++;
                foreach(GameObject enemy in waveEnemies[currentWave])
                {
                    enemy.SetActive(true);
                }
                enemyThreshold = _control.Fsm.GetFsmInt("Battle Enemies").Value - waveEnemies[currentWave].Count;
            }

            if(waveEnemies.Count == 1)
            {
                Dev.Log("Wave Count is one");
                bool isDone = true;
                Dev.Log("Checking wave count: " + waveEnemies[0].Count);
                foreach(GameObject enemy in waveEnemies[0])
                {
                    if (enemy == null || enemy.GetComponent<HealthManager>() == null)
                        continue;
                    if (enemy.GetComponent<HealthManager>().hp > 0)
                    {
                        Dev.Log("Wave is not complete");
                        isDone = false;
                        break;
                    }
                }
                //Dev.Log(isDone);
                if (isDone)
                {
                    //Dev.Log("Sending BG OPEN event");
                    //_control.SendEvent("BG OPEN");
                    _control.SetState("End");
                    Dev.Log("Sending BG OPEN event");
                }
            }
            //_control.Fsm.GetFsmInt("Battle Enemies").Value
            */
        }

        public void EnemyKilled()
        {
            _control.Fsm.GetFsmInt("Battle Enemies").Value--;
            enemiesKilled++;
            //if(enemyTotal != -1 && enemiesKilled >= enemyTotal)
            //{
            //    _control.SetState("End");
            //}
        }
    }
}