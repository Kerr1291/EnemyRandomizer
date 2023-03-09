using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System;

namespace EnemyRandomizerMod
{
    public static class CameraExtensions
    {
        public static List<RaycastHit> RaycastFromMouse(this Camera fromCamera)
        {
            return fromCamera.RaycastFrom(Input.mousePosition, fromCamera.cullingMask);
        }

        public static List<RaycastHit> RaycastFromMouse(this Camera fromCamera, LayerMask mask)
        {
            return fromCamera.RaycastFrom(Input.mousePosition, mask);
        }

        public static List<RaycastHit> RaycastFrom(this Camera fromCamera, Vector2 screenPoint)
        {
            return fromCamera.RaycastFrom(new Vector3(screenPoint.x, screenPoint.y, 0f), fromCamera.cullingMask);
        }

        public static List<RaycastHit> RaycastFrom(this Camera fromCamera, Vector2 screenPoint, LayerMask mask)
        {
            return fromCamera.RaycastFrom(new Vector3(screenPoint.x, screenPoint.y, 0f), mask);
        }

        public static List<RaycastHit> RaycastFrom(this Camera fromCamera, Vector3 screenPoint)
        {
            Ray cameraRay = fromCamera.ScreenPointToRay(screenPoint);
            return Physics.RaycastAll(cameraRay, fromCamera.farClipPlane, fromCamera.cullingMask).ToList();
        }

        public static List<RaycastHit> RaycastFrom(this Camera fromCamera, Vector3 screenPoint, LayerMask mask)
        {
            Ray cameraRay = fromCamera.ScreenPointToRay(screenPoint);
            return Physics.RaycastAll(cameraRay, fromCamera.farClipPlane, mask).ToList();
        }

        public static bool CanRaycastHit(this Camera fromCamera, LayerMask touchMask)
        {
            if(!fromCamera.gameObject.activeInHierarchy)
                return false;

            //does the touchmask have layers that overlap the camera's rendering masks?
            if(!touchMask.Any(fromCamera.cullingMask))
                return false;

            return true;
        }

        public static bool CanRaycastHit(this Camera fromCamera, GameObject gameObject, LayerMask mask)
        {
            if(!fromCamera.CanRaycastHit(mask))
                return false;

            //is the object drawn by the camera?
            if((fromCamera.cullingMask & gameObject.layer) <= 0)
                return false;

            //object is not in the layer mask, no point trying a raycast
            if(!mask.Any(gameObject.layer))
                return false;

            if(!gameObject.activeInHierarchy)
                return false;

            return true;
        }

        public static bool CanRaycastHit(this Camera fromCamera, Component component, LayerMask mask)
        {
            if(component == null || !fromCamera.CanRaycastHit(component.gameObject, mask))
                return false;

            return true;
        }

        public static bool CanRaycastHit(this Camera fromCamera, MonoBehaviour behaviour, LayerMask mask)
        {
            if(behaviour == null || !fromCamera.CanRaycastHit(behaviour.gameObject, mask))
                return false;

            return behaviour.enabled;
        }



        public static bool CanRaycastHit(this Camera fromCamera, GameObject gameObject)
        {
            if(!fromCamera.gameObject.activeInHierarchy)
                return false;

            //is the object drawn by the camera?
            if((fromCamera.cullingMask & gameObject.layer) <= 0)
                return false;

            if(gameObject == null || !gameObject.activeInHierarchy)
                return false;

            return true;
        }

        public static bool CanRaycastHit(this Camera fromCamera, Component component)
        {
            if(component == null || !fromCamera.CanRaycastHit(component.gameObject))
                return false;

            return true;
        }

        public static bool CanRaycastHit(this Camera fromCamera, MonoBehaviour behaviour)
        {
            if(behaviour == null || !fromCamera.CanRaycastHit(behaviour.gameObject))
                return false;

            return behaviour.enabled;
        }

        public static bool IsMouseOn(this Camera fromCamera, GameObject touchableObject)
        {
            return fromCamera.IsPointOn(Input.mousePosition, touchableObject, fromCamera.cullingMask);
        }

        public static bool IsMouseOn(this Camera fromCamera, GameObject touchableObject, LayerMask mask)
        {
            return fromCamera.IsPointOn(Input.mousePosition, touchableObject, mask);
        }

        public static bool IsPointOn(this Camera fromCamera, Vector2 screenPoint, GameObject touchableObject)
        {
            return fromCamera.IsPointOn(screenPoint, touchableObject, fromCamera.cullingMask);
        }

        public static bool IsPointOn(this Camera fromCamera, Vector2 screenPoint, GameObject touchableObject, LayerMask mask)
        {
            if(!touchableObject.IsTouchableType())
            {
                Debug.LogError("GameObject " + touchableObject.name + " is not a touchable type. Must have a component of type Collider(2D), SpriteRenderer, or RectTransform.");
                return false;
            }

            if(touchableObject.GetComponent<Collider>() != null)
                return fromCamera.IsPointOn(screenPoint, touchableObject.GetComponent<Collider>(), mask);
            if(touchableObject.GetComponent<Collider2D>() != null)
                return fromCamera.IsPointOn(screenPoint, touchableObject.GetComponent<Collider2D>(), mask);
            if(touchableObject.GetComponent<SpriteRenderer>() != null)
                return fromCamera.IsPointOn(screenPoint, touchableObject.GetComponent<SpriteRenderer>(), mask);
            if(touchableObject.GetComponent<RectTransform>() != null)
                return fromCamera.IsPointOn(screenPoint, touchableObject.GetComponent<RectTransform>(), mask);

            return false;
        }

        /// <summary>
        /// Casts a ray from the camera into the scene to see if it collides with the object given.
        /// Note that this uses 3D colliders, so a box collider or similar component needs to be on the testAgainst object.
        /// </summary>
        public static bool IsMouseOn(this Camera fromCamera, Collider collider, LayerMask mask)
        {
            return fromCamera.IsPointOn(Input.mousePosition, collider, mask);
        }

        public static bool IsMouseOn(this Camera fromCamera, Collider collider)
        {
            return fromCamera.IsPointOn(Input.mousePosition, collider, fromCamera.cullingMask);
        }

        public static bool IsPointOn(this Camera fromCamera, Vector2 screenPoint, Collider collider, LayerMask mask)
        {
            if(!fromCamera.CanRaycastHit(collider, mask))
                return false;

            List<RaycastHit> touching = fromCamera.RaycastFrom(screenPoint, mask);

            foreach(RaycastHit r in touching)
            {
                if(r.collider != null && r.collider.gameObject == collider.gameObject)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsMouseOn(this Camera fromCamera, Bounds bounds)
        {
            return fromCamera.IsPointOn(Input.mousePosition, bounds);
        }

        public static bool IsMouseOn(this Camera fromCamera, Bounds bounds, LayerMask mask)
        {
            return fromCamera.IsPointOn(Input.mousePosition, bounds, mask);
        }

        public static bool IsPointOn(this Camera fromCamera, Vector2 screenPoint, Bounds bounds, LayerMask mask)
        {
            if(!fromCamera.CanRaycastHit(mask))
                return false;

            return fromCamera.IsPointOn(screenPoint, bounds);
        }

        public static bool IsPointOn(this Camera fromCamera, Vector2 screenPoint, Bounds bounds)
        {
            Ray cameraRay = fromCamera.ScreenPointToRay(screenPoint);

            //get the distance to the z-plane of this object
            float planeDepth = bounds.center.z;
            float planeDistance = Mathf.Abs(planeDepth - cameraRay.origin.z) / cameraRay.direction.z;

            //get the point where the camera's ray intersects the sprite's plane
            Vector3 pointOnSpritePlane = cameraRay.origin + cameraRay.direction * planeDistance;

            //get the sprite's bounds and test against the projected point
            Bounds spriteBounds = bounds;
            return spriteBounds.Contains(pointOnSpritePlane);
        }


        public static bool IsMouseOn(this Camera fromCamera, SpriteRenderer sprite, LayerMask mask)
        {
            return fromCamera.IsPointOn(Input.mousePosition, sprite, mask);
        }

        public static bool IsMouseOn(this Camera fromCamera, SpriteRenderer sprite)
        {
            return fromCamera.IsPointOn(Input.mousePosition, sprite, fromCamera.cullingMask);
        }

        public static bool IsPointOn(this Camera fromCamera, Vector2 screenPoint, SpriteRenderer sprite)
        {
            return fromCamera.IsPointOn(screenPoint, sprite, fromCamera.cullingMask);
        }

        public static bool IsPointOn(this Camera fromCamera, Vector2 screenPoint, SpriteRenderer sprite, LayerMask mask)
        {
            if(!fromCamera.CanRaycastHit(sprite, mask))
                return false;

            Ray cameraRay = fromCamera.ScreenPointToRay(screenPoint);

            //get the distance to the z-plane of this sprite
            float planeDepth = sprite.transform.position.z;
            float planeDistance = Mathf.Abs(planeDepth - cameraRay.origin.z) / cameraRay.direction.z;

            //get the point where the camera's ray intersects the sprite's plane
            Vector3 pointOnSpritePlane = cameraRay.origin + cameraRay.direction * planeDistance;

            //get the sprite's bounds and test against the projected point
            Bounds spriteBounds = sprite.bounds;
            return spriteBounds.Contains(pointOnSpritePlane);
        }

        public static bool IsMouseOn(this Camera fromCamera, Collider2D collider2d)
        {
            return fromCamera.IsPointOn(Input.mousePosition, collider2d, fromCamera.cullingMask);
        }

        public static bool IsMouseOn(this Camera fromCamera, Collider2D collider2d, LayerMask mask)
        {
            return fromCamera.IsPointOn(Input.mousePosition, collider2d, mask);
        }

        public static bool IsPointOn(this Camera fromCamera, Vector2 screenPoint, Collider2D collider2d)
        {
            return fromCamera.IsPointOn(screenPoint, collider2d, fromCamera.cullingMask);
        }

        public static bool IsPointOn(this Camera fromCamera, Vector2 screenPoint, Collider2D collider2d, LayerMask mask)
        {
            if(!fromCamera.CanRaycastHit(collider2d, mask))
                return false;

            Ray cameraRay = fromCamera.ScreenPointToRay(screenPoint);

            //get the distance to the z-plane of this sprite
            float planeDepth = collider2d.transform.position.z;
            float planeDistance = Mathf.Abs(planeDepth - cameraRay.origin.z) / cameraRay.direction.z;

            //get the point where the camera's ray intersects the sprite's plane
            Vector3 pointOnSpritePlane = cameraRay.origin + cameraRay.direction * planeDistance;

            //get the sprite's bounds and test against the projected point
            Bounds spriteBounds = collider2d.bounds;
            return spriteBounds.Contains(pointOnSpritePlane);
        }

        public static bool IsMouseOn(this Camera fromCamera, RectTransform rectTransform)
        {
            return fromCamera.IsPointOn(Input.mousePosition, rectTransform, fromCamera.cullingMask);
        }

        public static bool IsMouseOn(this Camera fromCamera, RectTransform rectTransform, LayerMask mask)
        {
            return fromCamera.IsPointOn(Input.mousePosition, rectTransform, mask);
        }

        public static bool IsPointOn(this Camera fromCamera, Vector2 screenPoint, RectTransform rectTransform)
        {
            return fromCamera.IsPointOn(screenPoint, rectTransform, fromCamera.cullingMask);
        }
        
        public static bool IsPointOn(this Camera fromCamera, Vector2 screenPoint, RectTransform rectTransform, LayerMask mask)
        {
            if(!fromCamera.CanRaycastHit(rectTransform, mask))
                return false;

            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, fromCamera);
        }
    }
}
