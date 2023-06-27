using UnityEngine;

namespace EnemyRandomizerMod
{
    public class ZoteMusicControl : MonoBehaviour
    {
        public SpawnedObjectControl soc;
        public AudioSource audioSource;
        public AudioSource coloMCaudioSource;
        public AudioSource targetAudioSource;
        public AudioSource globalAudioSource;

        public void PlayMusic(float volume = 0.8f, GameObject target = null)
        {
            if (!EnemyRandomizerDatabase.GetGlobalSettings().allowEnemyRandoExtras)
            {
                StopMusic();
                return;
            }

            bool isColoBronze = BattleManager.StateMachine.Value is ColoBronze;
            if (isColoBronze)
            {
                var bronze = BattleManager.StateMachine.Value as ColoBronze;
                var mc = bronze.FSM.gameObject.FindGameObjectInDirectChildren("Music Control");
                coloMCaudioSource = mc.GetComponent<AudioSource>();
                coloMCaudioSource.Stop();
                coloMCaudioSource.clip = audioSource.clip;
                if (coloMCaudioSource.isPlaying)
                {
                    coloMCaudioSource.volume = 1f;
                }
                else
                {
                    coloMCaudioSource.Play();
                    coloMCaudioSource.volume = 0.8f;
                }
            }
            else if(target != null)
            {
                targetAudioSource = target.GetComponent<AudioSource>();
                if(targetAudioSource != null)
                {
                    targetAudioSource.Stop();
                    targetAudioSource.clip = audioSource.clip;
                    if (targetAudioSource.isPlaying)
                    {
                        targetAudioSource.Stop();
                    }
                    {
                        targetAudioSource.Play();
                        targetAudioSource.volume = volume;
                    }
                }
            }
            else
            {
                var musicControl = GameObject.Find("AudioManager");
                if (musicControl == null)
                    return;
                globalAudioSource = musicControl.GetComponentInChildren<AudioSource>(true);

                if (globalAudioSource != null)
                {
                    globalAudioSource.Stop();
                    globalAudioSource.clip = audioSource.clip;
                    if (globalAudioSource.isPlaying)
                    {
                        globalAudioSource.volume = 1f;
                    }
                    else
                    {
                        globalAudioSource.Play();
                        globalAudioSource.volume = volume;
                    }
                }
            }
        }

        public void StopMusic()
        {
            if (coloMCaudioSource != null && coloMCaudioSource.isPlaying)
                coloMCaudioSource.Stop();

            if (targetAudioSource != null && targetAudioSource.isPlaying)
                targetAudioSource.Stop();

            if (globalAudioSource != null && globalAudioSource.isPlaying)
                globalAudioSource.Stop();
        }

        protected virtual void OnDestory()
        {
            if (coloMCaudioSource != null && coloMCaudioSource.isPlaying)
                coloMCaudioSource.Stop();

            if (targetAudioSource != null && targetAudioSource.isPlaying)
                targetAudioSource.Stop();

            if (globalAudioSource != null && globalAudioSource.isPlaying)
                globalAudioSource.Stop();
        }
    }

    public class ZoteMusicSpawner : DefaultSpawner
    {
        public override GameObject Spawn(PrefabObject prefabToSpawn, GameObject objectToReplace)
        {
            var cage = base.Spawn(prefabToSpawn, null);
            var acage = cage.GetOrAddComponent<ZoteMusicControl>();
            acage.soc = cage.GetComponent<SpawnedObjectControl>();
            acage.audioSource = cage.GetComponent<AudioSource>();

            return cage;
        }
    }

    public class ZoteMusicPrefabConfig : DefaultPrefabConfig
    {
        public override void SetupPrefab(PrefabObject p)
        {
            base.SetupPrefab(p);
            p.prefab.AddComponent<ArenaCageSmallControl>();
        }
    }
}
