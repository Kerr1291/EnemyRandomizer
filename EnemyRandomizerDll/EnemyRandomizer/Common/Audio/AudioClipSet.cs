using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace nv
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioClipSet : MonoBehaviour, IList<AudioClip>
    {
        protected AudioSource source;

        [SerializeField]
        protected List<AudioClip> clips;
        protected List<float?> savedStartTimes;
        
        [SerializeField]
        protected int defaultClip;
      
        public AudioSource Source
        {
            get
            {
                return source ?? (source = GetComponent<AudioSource>());
            }
        }

        public List<AudioClip> Clips
        {
            get
            {
                return clips ?? (clips = new List<AudioClip>());
            }
        }

        public List<float?> SavedStartTimes
        {
            get
            {
                return savedStartTimes ?? (savedStartTimes = new List<float?>());
            }
        }

        public int DefaultClip
        {
            get
            {
                return defaultClip;
            }
            set
            {
                defaultClip = value;
            }
        }

        public float Volume
        {
            get
            {
                return Source.volume;
            }
            set
            {
                Source.volume = Mathf.Max(value, 0.0f);
            }
        }

        public float Time
        {
            get
            {
                return Source.time;
            }
            set
            {
                Source.time = value;
            }
        }

        public float TimeNormalized
        {
            get
            {
                return CurrentClipLength <= 0f ? 0f : Mathf.Clamp01( Time / CurrentClipLength);
            }
            set
            {
                Time = value * CurrentClipLength;
            }
        }

        public AudioClip CurrentClip
        {
            get
            {
                return Source.clip;
            }
        }
        public float CurrentClipLength
        {
            get
            {
                if(CurrentClip == null)
                    return 0f;
                return CurrentClip.length;
            }
        }

        public int? CurrentClipIndex
        {
            get
            {
                int currentIndex = Clips.IndexOf(Source.clip);
                return currentIndex >= 0 ? new int?(currentIndex) : null;
            }
        }

        public float? CurrentClipSavedStartTime
        {
            get
            {
                return CurrentClipIndex != null ? SavedStartTimes[CurrentClipIndex.Value] : null;
            }
            set
            {
                if(CurrentClipIndex != null)
                    SavedStartTimes[CurrentClipIndex.Value] = value;
            }
        }

        public int Count
        {
            get
            {
                return Clips.Count;
            }
        }
        
        public int PreviousClip
        {
            get;
            protected set;
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<AudioClip>)Clips).IsReadOnly;
            }
        }

        public AudioClip this[int index]
        {
            get
            {
                return Clips[index];
            }
            set
            {
                //if setting this will replace the active clip on the source...
                if(CurrentClipIndex != null && index == CurrentClipIndex.Value)
                {
                    Source.Stop();
                }

                Clips[index] = value;
                SavedStartTimes[index] = null;
                Source.clip = value;
            }
        }

        public virtual void Play(bool resume = false, int? index = null)
        {
            if(Clips.Count <= 0)
                return;

            Stop();
            
            if(index != null)
            {
                if(CurrentClipIndex != index.Value)
                    Source.clip = Clips[index.Value];
                if(resume && SavedStartTimes[index.Value] != null)
                    Source.time = SavedStartTimes[index.Value].Value;
                SavedStartTimes[index.Value] = null;
            }
            else if(Source.clip == null)
            {
                Source.clip = Clips[defaultClip];
                if(resume && SavedStartTimes[defaultClip] != null)
                    Source.time = SavedStartTimes[defaultClip].Value;
                SavedStartTimes[defaultClip] = null;
            }

            if(Source.clip == null)
                return;
            
            Source.Play();
            PreviousClip = CurrentClipIndex.Value;
        }

        public virtual void Play(float startTime, int? index = null)
        {
            if(Clips.Count <= 0)
                return;

            Play(false, index);
            Time = startTime;
        }

        public virtual void PlayRandom(bool resume = false, bool canPlayPreviousClip = true)
        {
            int index = PreviousClip;

            if(Count > 1)
            {
                //get an index we haven't used, if required
                do
                {
                    index = UnityEngine.Random.Range(0, Count);
                    if(canPlayPreviousClip)
                        break;
                }
                while(index == PreviousClip);
            }

            Play(resume, index);
        }

        protected virtual void OnDisable()
        {
            Stop();
        }

        /// <summary>
        /// Stop the sound right now.
        /// If the audio source is playing, the time will be saved off and may be used on next play.
        /// </summary>
        public virtual void Stop()
        {
            if(CurrentClipIndex != null && CurrentClipSavedStartTime == null)
                SavedStartTimes[CurrentClipIndex.Value] = Source.time;
            Source.Stop();
        }

        protected virtual void Reset()
        {
#if UNITY_EDITOR
            UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
#endif
            Source.playOnAwake = false;
            savedStartTimes = Clips.Select(x => (float?)null).ToList();
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            //keep the current clip and the unity source clips in sync
            if(defaultClip < Clips.Count && Source.clip != Clips[defaultClip])
            {
                Source.clip = Clips[defaultClip];
            }
            savedStartTimes = Clips.Select(x => (float?)null).ToList();
#endif
        }

        protected virtual void Awake()
        {
            if(SavedStartTimes.Count != Clips.Count)
                savedStartTimes = Clips.Select(x => (float?)null).ToList();
        }

        public int IndexOf(AudioClip item)
        {
            return ((IList<AudioClip>)Clips).IndexOf(item);
        }

        public void Insert(int index, AudioClip item)
        {
            savedStartTimes.Insert(index, null);
            ((IList<AudioClip>)Clips).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            savedStartTimes.RemoveAt(index);
               ((IList<AudioClip>)Clips).RemoveAt(index);
        }

        public void Add(AudioClip item)
        {
            savedStartTimes.Add(null);
            ((IList<AudioClip>)Clips).Add(item);
        }

        public void Clear()
        {
            ((IList<AudioClip>)Clips).Clear();
            savedStartTimes.Clear();
        }

        public bool Contains(AudioClip item)
        {
            return ((IList<AudioClip>)Clips).Contains(item);
        }

        public void CopyTo(AudioClip[] array, int arrayIndex)
        {
            ((IList<AudioClip>)Clips).CopyTo(array, arrayIndex);
        }

        public bool Remove(AudioClip item)
        {
            if(IndexOf(item) >= 0)
                savedStartTimes.RemoveAt(IndexOf(item));
            return ((IList<AudioClip>)Clips).Remove(item);
        }

        public IEnumerator<AudioClip> GetEnumerator()
        {
            return ((IList<AudioClip>)Clips).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<AudioClip>)Clips).GetEnumerator();
        }
    }
}





//#if UNITY_EDITOR
//using UnityEditor;
//using System.Reflection;
//namespace nv.editor
//{
//    [CanEditMultipleObjects]
//    [CustomEditor(typeof(AudioClipSet))]
//    public class AudioClipSet_Editor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            AudioClipSet localTarget = target as AudioClipSet;

//            localTarget = (AudioClipSet)target;
//            EditorGUILayout.BeginHorizontal();
//            if(GUILayout.Button("Play"))
//            {
//                if(Application.isPlaying)
//                    localTarget.Play();
//                else
//                {
//                    StopAllClips();
//                    PlayClip(localTarget.Source.clip);
//                }
//            }

//            string stopButtonText = "";
//            if(Application.isPlaying)
//                stopButtonText = "Stop";
//            else
//                stopButtonText = "Stop All Clips";

//            if(GUILayout.Button(stopButtonText))
//            {
//                if(Application.isPlaying)
//                    localTarget.Stop();
//                else
//                    StopAllClips();
//            }
//            EditorGUILayout.EndHorizontal();

//            if(Application.isPlaying)
//            {
//                EditorGUI.BeginDisabledGroup(true);
//                EditorGUILayout.Toggle("IsPlaying?", localTarget.Source.isPlaying);
//                EditorGUI.EndDisabledGroup();
//            }

//            if(localTarget != null)
//            {
//                localTarget.CurrentIndex = EditorGUILayout.DelayedIntField("Current Index",localTarget.CurrentIndex);
//            }
//            base.OnInspectorGUI();
//        }
//        ////////
//        ///The code snippets below allow for sounds to be played in editor mode.
//        ///Code from: https://forum.unity3d.com/threads/way-to-play-audio-in-editor-using-an-editor-script.132042/
//        ///

//        public static void PlayClip(AudioClip clip)
//        {
//            if(clip == null)
//                return;

//            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
//            System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
//            MethodInfo method = audioUtilClass.GetMethod(
//                "PlayClip",
//                BindingFlags.Static | BindingFlags.Public,
//                null,
//                new System.Type[] {
//                typeof(AudioClip)
//            },
//                null
//            );

//            method.Invoke(null, new object[] { clip });
//        }

//        public static void StopAllClips()
//        {
//            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
//            System.Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
//            MethodInfo method = audioUtilClass.GetMethod(
//                "StopAllClips",
//                BindingFlags.Static | BindingFlags.Public,
//                null,
//                new System.Type[] { },
//                null
//            );

//            method.Invoke(null, new object[] { });
//        }
//        ///
//        ///End imported code.
//        ////////
//    }

//}
//#endif