#if !LIBRARY
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EnemyRandomizerMod
{
    //need to use a type deriving from UIBehavior to capture rect size changes propigated at runtime...
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Text))] 
    [RequireComponent(typeof(ContentSizeFitter))]
    public class DebugLogString : UnityEngine.EventSystems.UIBehaviour
    {
        public System.Action onSizeChanged;

        public bool Loaded
        {
            get; protected set;
        }

        public string Content
        {
            get
            {
                return Text.text;
            }
            set
            {
                Text.text = value;
            }
        }

        public Text Text
        {
            get
            {
                return GetComponent<Text>();
            }
        }

        public RectTransform Transform
        {
            get
            {
                return GetComponent<RectTransform>();
            }
        }

        public ContentSizeFitter CSF
        {
            get
            {
                return GetComponent<ContentSizeFitter>();
            }
        }

        public float Size
        {
            get
            {
                if(!Loaded)
                    return 0f;
                return Transform.sizeDelta.y + Text.lineSpacing;
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            if(onSizeChanged != null)
                onSizeChanged.Invoke();
            Loaded = true;
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            RectTransform rt = gameObject.GetOrAddComponent<RectTransform>();
            Text text = gameObject.GetOrAddComponent<Text>();
            text.color = Color.red;
            text.font = Font.CreateDynamicFontFromOSFont("Arial", 14);
            text.fontSize = 12;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.alignment = TextAnchor.MiddleLeft;

            ContentSizeFitter csf = gameObject.GetOrAddComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            Transform.anchorMax = new Vector2(0f, 1f);
            Transform.anchorMin = new Vector2(0f, 1f);
            Transform.anchoredPosition = new Vector2(0f, 0f);
            Transform.pivot = new Vector2(0f, 1f);
        }
#endif

        //protected override void OnEnable()
        //{
        //    base.OnEnable();
        //    if(Application.isPlaying)
        //        GameObject.DontDestroyOnLoad(gameObject);
        //}
    }
}
#endif