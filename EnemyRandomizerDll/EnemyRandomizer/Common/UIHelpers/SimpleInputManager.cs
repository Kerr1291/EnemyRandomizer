using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

namespace nv
{
    public class SimpleInputManager : GameSingleton<SimpleInputManager>
    {
        //=======================================================================
        //Commuication Boilerplate
        CommunicationNode comms = new CommunicationNode();

        protected virtual void Start()
        {
            comms.EnableNode(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            comms.DisableNode();
        }
        //=======================================================================

        [Header("Actions for outside normal game play")]
        public List<InputType> systemInputActions;

        [Header("Actions for inside normal game play")]
        public List<InputType> gameInputActions;

        [Header("Actions always enabled in debug mode")]
        public List<InputType> debugInputActions;

        public virtual bool IsSystemMode
        {
            get
            {
                return (isPaytableTest || isHistory);
            }
        }

        public bool systemInputEnabled = true;
        public bool gameInputEnabled = true;
        public bool debugInputEnabled = true;

        protected bool machineTilt = false;
        protected bool isPaytableTest = false;
        protected bool isHistory = false;

        protected bool recentlyTouched = false;
        protected float recentlyTouchedTime = 0f;
        protected float recentlyTouchedMaxTime = .1f;
        
        protected List<UnityEvent> invokedThisFrame = new List<UnityEvent>();

        public virtual bool RecentlyTouched
        {
            get
            {
                return recentlyTouched;
            }
        }

        protected virtual void UpdateRecentlyTouched()
        {
            if(recentlyTouched)
                return;

            recentlyTouchedTime += Time.deltaTime;

            if(recentlyTouchedTime > recentlyTouchedMaxTime)
                recentlyTouched = true;
        }

        /// <summary>
        /// Register a key to a callback. Example:
        /// GameInput.Register( KeyCode.Mouse0, TryPick );
        /// </summary>
        public virtual void Register(List<InputType> inputActions, KeyCode key, UnityAction action, InputType.KeyState keystate = InputType.KeyState.OnPress, InputType.TouchableObject optionalTouchable = null)
        {
            //unregister first to prevent double-adding of event handlers

            Unregister(inputActions, key, action, keystate);
            for(int i = 0; i < inputActions.Count; ++i)
            {
                if(inputActions[i].key == key)
                {
                    foreach(var pair in inputActions[i].keyEvents)
                    {
                        if((pair.onKeyState & keystate) != InputType.KeyState.None)
                        {
                            pair.events.RemoveListener(action);
                            pair.events.AddListener(action);
                            return;
                        }
                    }

                    //if we reach this point, the pair doesn't exist
                    InputType.KeyEvent newPair = new InputType.KeyEvent();
                    newPair.onKeyState = keystate;
                    newPair.events.AddListener(action);
                    inputActions[i].keyEvents.Add(newPair);
                    break;
                }
            }

            //if we reach this point, the action doesn't exist
            InputType newAction = new InputType();
            InputType.KeyEvent newPairForNewAction = new InputType.KeyEvent();
            newPairForNewAction.onKeyState = keystate;
            newPairForNewAction.events = new UnityEvent();
            newPairForNewAction.events.AddListener(action);
            newAction.keyEvents.Add(newPairForNewAction);
            newAction.key = key;
            newAction.optionalTouchable = optionalTouchable;
            inputActions.Add(newAction);
        }

        /// <summary>
        /// Removes a key from the list of callbacks. Example:
        /// GameInput.Unregister( KeyCode.Mouse0, TryPick );
        /// </summary>
        public virtual void Unregister(List<InputType> inputActions, KeyCode key, UnityAction action, InputType.KeyState keystate = InputType.KeyState.OnPress)
        {
            for(int i = 0; i < inputActions.Count; ++i)
            {
                if(inputActions[i].key == key)
                {
                    for(int j = 0; j < inputActions[i].keyEvents.Count; ++j)
                    {
                        if((inputActions[i].keyEvents[j].onKeyState & keystate) != InputType.KeyState.None)
                        {
                            inputActions[i].keyEvents[j].events.RemoveListener(action);

                            //normally this would be bad, but we're exiting the list iteration right after removing the element(s)
                            if(inputActions[i].keyEvents[j].events.GetPersistentEventCount() <= 0)
                                inputActions[i].keyEvents.RemoveAt(j);

                            if(inputActions[i].keyEvents.Count <= 0)
                                inputActions.RemoveAt(i);

                            return;
                        }
                    }
                    //matching keyEvents not found
                    return;
                }
            }
        }

        public virtual bool CanTouch(Component component, Camera fromCamera, LayerMask mask)
        {
            if(!CanTouch(fromCamera, mask))
                return false;

            //object is not in the layer mask, no point trying a raycast
            if(!LayerMaskContains(mask, component.gameObject.layer))
                return false;

            if(component == null || !component.gameObject.activeInHierarchy)
                return false;

            return true;
        }

        public virtual bool CanTouch(Camera fromCamera, LayerMask touchMask)
        {
            if(!CanTouch())
                return false;

            if(fromCamera != null)
            {
                if(!fromCamera.gameObject.activeInHierarchy)
                    return false;

                //camera doesn't draw these objects, so can't touch them
                if((touchMask.value & fromCamera.cullingMask) <= 0)
                    return false;
            }

            return true;
        }

        public virtual bool CanTouch()
        {
            if(!RecentlyTouched)
                return false;

            if(!gameInputEnabled)
                return false;

            return true;
        }

        public virtual bool IsMouseOn(GameObject touchableObject, Camera fromCamera, LayerMask mask)
        {
            return IsPointOn(Input.mousePosition, touchableObject, fromCamera, mask);
        }

        public virtual bool IsPointOn(Vector2 screenPoint, GameObject touchableObject, Camera fromCamera, LayerMask mask)
        {
            if(!InputType.TouchableObject.IsTouchableType(touchableObject))
            {
                Debug.LogError("GameObject " + touchableObject.name + " is not a touchable type. Must have a component of type Collider(2D), SpriteRenderer, or RectTransform.");
                return false;
            }

            if(touchableObject.GetComponent<Collider>() != null)
                return IsPointOn(screenPoint, touchableObject.GetComponent<Collider>(), fromCamera, mask);
            if(touchableObject.GetComponent<Collider2D>() != null)
                return IsPointOn(screenPoint, touchableObject.GetComponent<Collider2D>(), fromCamera, mask);
            if(touchableObject.GetComponent<SpriteRenderer>() != null)
                return IsPointOn(screenPoint, touchableObject.GetComponent<SpriteRenderer>(), fromCamera, mask);
            if(touchableObject.GetComponent<RectTransform>() != null)
                return IsPointOn(screenPoint, touchableObject.GetComponent<RectTransform>(), fromCamera, mask);

            return false;
        }

        /// <summary>
        /// Casts a ray from the camera into the scene to see if it collides with the object given.
        /// Note that this uses 3D colliders, so a box collider or similar component needs to be on the testAgainst object.
        /// </summary>
        protected virtual bool IsMouseOn(Collider collider, Camera fromCamera, LayerMask mask)
        {
            return IsPointOn(Input.mousePosition, collider, fromCamera, mask);
        }

        protected virtual bool IsPointOn(Vector2 screenPoint, Collider collider, Camera fromCamera, LayerMask mask)
        {
            if(!CanTouch(collider, fromCamera, mask))
                return false;

            List<RaycastHit> touching = RaycastFromCamera(screenPoint, fromCamera, mask);

            foreach(RaycastHit r in touching)
            {
                if(r.collider != null && r.collider.gameObject == collider.gameObject)
                {
                    return true;
                }
            }
            return false;
        }


        protected virtual bool IsMouseOn(SpriteRenderer sprite, Camera fromCamera, LayerMask mask)
        {
            return IsPointOn(Input.mousePosition, sprite, fromCamera, mask);
        }

        protected virtual bool IsPointOn(Vector2 screenPoint, SpriteRenderer sprite, Camera fromCamera, LayerMask mask)
        {
            if(!CanTouch(sprite, fromCamera, mask))
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

        protected virtual bool IsMouseOn(Collider2D collider2d, Camera fromCamera, LayerMask mask)
        {
            return IsPointOn(Input.mousePosition, collider2d, fromCamera, mask);
        }

        protected virtual bool IsPointOn(Vector2 screenPoint, Collider2D collider2d, Camera fromCamera, LayerMask mask)
        {
            if(!CanTouch(collider2d, fromCamera, mask))
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

        protected virtual bool IsMouseOn(RectTransform rectTransform, Camera fromCamera, LayerMask mask)
        {
            return IsPointOn(Input.mousePosition, rectTransform, fromCamera, mask);
        }

        //pass null for a screen space overlay camera
        protected virtual bool IsPointOn(Vector2 screenPoint, RectTransform rectTransform, Camera fromCamera, LayerMask mask)
        {
            if(!CanTouch(rectTransform, fromCamera, mask))
                return false;

            if(fromCamera == null)
                return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint);

            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, fromCamera);            
        }

        protected virtual void UpdateInput(List<InputType> inputActions)
        {
            for(int i = 0; i < inputActions.Count; ++i)
            {
                foreach(var pair in inputActions[i].keyEvents)
                {
                    if(invokedThisFrame.Contains(pair.events))
                        continue;

                    bool willInvoke = false;
                    if((pair.onKeyState & InputType.KeyState.OnPress) != InputType.KeyState.None && Input.GetKeyDown(inputActions[i].key))
                        willInvoke = true;
                    else if((pair.onKeyState & InputType.KeyState.OnHold) != InputType.KeyState.None && Input.GetKey(inputActions[i].key))
                        willInvoke = true;
                    else if((pair.onKeyState & InputType.KeyState.OnRelease) != InputType.KeyState.None && Input.GetKeyUp(inputActions[i].key))
                        willInvoke = true;

                    if(willInvoke)
                    {
                        recentlyTouched = true;

                        if(inputActions[i].optionalTouchable != null && inputActions[i].optionalTouchable.touchable != null)
                        {
                            if(IsMouseOn(inputActions[i].optionalTouchable.touchable, inputActions[i].optionalTouchable.camera, ~0))
                                pair.events.Invoke();
                        }
                        else
                        {
                            pair.events.Invoke();
                        }

                        invokedThisFrame.Add(pair.events);
                    }
                }
            }
        }
        
        /// <summary>
        /// Polls the input actions and invokes any callbacks for keys that are pressed.
        /// </summary>
        protected virtual void Update()
        {
            UpdateRecentlyTouched();

            if(machineTilt)
                return;

            if(systemInputEnabled)
                UpdateInput(systemInputActions);
            if(gameInputEnabled)
                UpdateInput(gameInputActions);
            if(debugInputEnabled)
                UpdateInput(debugInputActions);

            if(invokedThisFrame == null)
                invokedThisFrame = new List<UnityEvent>();

            if(invokedThisFrame.Count > 0)
                invokedThisFrame.Clear();
        }

        protected virtual void EnableGameInput(bool enabled)
        {
            gameInputEnabled = enabled;
        }

        protected virtual void EnableSystemInput(bool enabled)
        {
            systemInputEnabled = enabled;
        }

        [CommunicationCallback]
        protected virtual void Handle(nvEvent.RegisterInputType gameEvent)
        {
            InputType.KeyState onKeyState = InputType.KeyState.None;
            if(gameEvent.onPress)
                onKeyState |= InputType.KeyState.OnPress;
            if(gameEvent.onHold)
                onKeyState |= InputType.KeyState.OnHold;
            if(gameEvent.onRelease)
                onKeyState |= InputType.KeyState.OnRelease;

            if(onKeyState == InputType.KeyState.None)
                return;

            InputType.TouchableObject touchable = null;
            if(gameEvent.touchableObject != null)
            {
                touchable = new InputType.TouchableObject(gameEvent.touchableObject,gameEvent.touchableObjectCamera);
            }

            if(gameEvent.gameAction)
                Register(gameInputActions, gameEvent.key, gameEvent.action, onKeyState, touchable);
            if(gameEvent.systemAction)
                Register(systemInputActions, gameEvent.key, gameEvent.action, onKeyState, touchable);
            if(gameEvent.debugAction)
                Register(debugInputActions, gameEvent.key, gameEvent.action, onKeyState, touchable);
        }

        [CommunicationCallback]
        protected virtual void Handle(nvEvent.UnregisterInputType gameEvent)
        {
            InputType.KeyState onKeyState = InputType.KeyState.None;
            if(gameEvent.onPress)
                onKeyState |= InputType.KeyState.OnPress;
            if(gameEvent.onHold)
                onKeyState |= InputType.KeyState.OnHold;
            if(gameEvent.onRelease)
                onKeyState |= InputType.KeyState.OnRelease;

            if(onKeyState == InputType.KeyState.None)
                return;

            if(gameEvent.gameAction)
                Unregister(gameInputActions, gameEvent.key, gameEvent.action, onKeyState);
            if(gameEvent.systemAction)
                Unregister(systemInputActions, gameEvent.key, gameEvent.action, onKeyState);
            if(gameEvent.debugAction)
                Unregister(debugInputActions, gameEvent.key, gameEvent.action, onKeyState);
        }

        protected virtual void UpdateInputMode()
        {
            EnableGameInput(!IsSystemMode);
            EnableSystemInput(IsSystemMode);
        }

        protected virtual bool LayerMaskContains(LayerMask mask, int layerID)
        {
            return (mask.value & (1 << layerID)) > 0;
        }

        public virtual List<RaycastHit> RaycastFromCamera(Vector2 point, Camera fromCamera, LayerMask mask)
        {
            Ray cameraRay = fromCamera.ScreenPointToRay(Input.mousePosition);

            return Physics.RaycastAll(cameraRay, fromCamera.farClipPlane, mask).ToList();
        }

        protected virtual void OnValidate()
        {
            ValidateTouchables(gameInputActions);
            ValidateTouchables(systemInputActions);
            ValidateTouchables(debugInputActions);
        }

        protected virtual void ValidateTouchables(List<InputType> inputActions)
        {
            for(int i = 0; i < inputActions.Count; ++i)
            {
                if(inputActions[i].optionalTouchable != null && inputActions[i].optionalTouchable.touchable != null)
                {
                    if(!inputActions[i].optionalTouchable.IsValid())
                    {
                        Debug.LogError("Type " + inputActions[i].optionalTouchable.GetType().Name + " is not a touchable type. Must be Collider(2D), SpriteRenderer, or RectTransform.");
                        inputActions[i].optionalTouchable = null;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class InputType
    {
        [System.Flags]
        public enum KeyState
        {
            None = 0,
            OnPress = 1,
            OnHold = 2,
            OnRelease = 4
        };

        [System.Serializable]
        public class KeyEvent
        {
            public KeyState onKeyState = KeyState.OnPress;
            public UnityEvent events;
        }

        [System.Serializable]
        public class TouchableObject
        {
            [Tooltip("If not null, this object will be raycasted against before invoking the event")]
            public GameObject touchable = null;

            [Tooltip("Null camera = screen space overlay camera")]
            public Camera camera = null;

            public TouchableObject() { }

            public TouchableObject(GameObject touchable, Camera camera = null)
            {
                this.touchable = touchable;
                this.camera = camera;
            }

            public bool IsValid()
            {
                if(touchable != null)
                {
                    return IsTouchableType(touchable);
                }
                return false;
            }

            public static bool IsTouchableType(GameObject touchableType)
            {
                if(touchableType.GetComponent<Collider>() != null)
                    return true;
                if(touchableType.GetComponent<Collider2D>() != null)
                    return true;
                if(touchableType.GetComponent<SpriteRenderer>() != null)
                    return true;
                if(touchableType.GetComponent<RectTransform>() != null)
                    return true;
                return false;
            }
        }

        public KeyCode key = KeyCode.Mouse0;
        [Tooltip("Optional object to check before invoking the events")]
        public TouchableObject optionalTouchable = new TouchableObject();
        public List<KeyEvent> keyEvents = new List<KeyEvent>();
    }
}