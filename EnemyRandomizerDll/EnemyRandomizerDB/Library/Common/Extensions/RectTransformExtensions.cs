using UnityEngine;
namespace EnemyRandomizerMod
{
    public static class RectTransformExtensions
    {
        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }
        
        //public static void SetRect(this RectTransform rt, Rect rect, bool centerIsBottomLeft = false)
        //{
        //    if(centerIsBottomLeft)
        //    {
        //        rt.SetLeft(rect.x);
        //        rt.SetRight(rect.x + rect.size.x);
        //        rt.SetTop(rect.y);
        //        rt.SetBottom(rect.y + rect.size.y);
        //    }
        //    else
        //    {
        //        var bl = rect.BottomLeft();
        //        var tr = rect.TopRight();
        //        rt.SetLeft(bl.x);
        //        rt.SetRight(tr.x);
        //        rt.SetTop(tr.y);
        //        rt.SetBottom(bl.y);
        //    }
        //}

        public static bool IsMouseOn(this RectTransform rectTransform, LayerMask mask)
        {
            if(!mask.Any(rectTransform))
                return false;

            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition);
        }

        public static bool IsPointOn(this RectTransform rectTransform, Vector2 screenPoint, LayerMask mask)
        {
            if(!mask.Any(rectTransform))
                return false;

            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint);
        }

        public static bool IsMouseOn(this RectTransform rectTransform)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition);
        }

        public static bool IsPointOn(this RectTransform rectTransform, Vector2 screenPoint)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint);
        }
    }
}