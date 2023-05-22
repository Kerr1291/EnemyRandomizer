#if !LIBRARY
using UnityEngine;

namespace EnemyRandomizerMod
{
    [RequireComponent(typeof(RectTransform))]
    public class PoolableLogString : PoolableMonoBehaviour<string>
    {
        public static string ObjectPoolKey = "DebugLogString";

        protected DebugLogString content;
        public DebugLogString Content
        {
            get
            {
                return content ?? (content = gameObject.GetComponentInChildren<DebugLogString>(true));
            }
        }

        protected RectTransform _Transform;
        public RectTransform Transform
        {
            get
            {
                return _Transform ?? (_Transform = GetComponent<RectTransform>());
            }
        }

        public float Size
        {
            get
            {
                return transform.localScale.y * Content.Size;
            }
        }

        public virtual void Awake()
        {
            Transform.anchorMax = new Vector2(0f, 1f);
            Transform.anchorMin = new Vector2(0f, 1f);
            Transform.pivot = new Vector2(0f, 1f);

            //if(Application.isPlaying)
            //    GameObject.DontDestroyOnLoad(gameObject);
        }

        public override void OnEnPool()
        {
            Data = null;
            name = "(Pooled Log String)";
            Content.Content = string.Empty;
            transform.localScale = Vector3.zero;
        }

        protected override void Setup(string data)
        {
            base.Setup(data);
            transform.localScale = Vector3.one;
            Content.Content = data;
            name = data;

            if (DevLogger.applicationIsQuitting)
                return;

            float maxLines = DevLogger.Instance.MaxGUILines;
            float logSize = DevLogger.Instance.LogSize.y;
            logSize = logSize / maxLines;

            Transform.anchorMax = Vector2.one;
            Transform.anchorMin = new Vector2(0f, 1f - logSize);
            Transform.sizeDelta = Vector2.zero;
            Transform.anchoredPosition = Vector2.zero;
            Transform.SetTop(0f);
            Transform.SetBottom(0f);
        }
    }
}
#endif