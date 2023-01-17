using UnityEngine;
using UnityEngine.Events;

namespace nvEvent
{
    public class RegisterInputType
    {
        public RegisterInputType(
            UnityAction action,
            KeyCode key = KeyCode.Mouse0,
            bool onPress = true,
            bool onHold = false,
            bool onRelease = false,
            bool gameAction = true,
            bool systemAction = false,
            bool debugAction = false,
            GameObject touchableObject = null,
            Camera touchableObjectCamera = null)
        {
            this.action = action;
            this.key = key;
            this.onPress = onPress;
            this.onHold = onHold;
            this.onRelease = onRelease;
            this.gameAction = gameAction;
            this.systemAction = systemAction;
            this.debugAction = debugAction;
            this.touchableObject = touchableObject;
            this.touchableObjectCamera = touchableObjectCamera;
        }

        public UnityAction action;
        public KeyCode key = KeyCode.Mouse0;
        public bool onPress = true;
        public bool onHold = false;
        public bool onRelease = false;
        public bool gameAction = true;
        public bool systemAction = false;
        public bool debugAction = false;
        public GameObject touchableObject = null;
        public Camera touchableObjectCamera = null;
    }

    public class UnregisterInputType
    {
        public UnregisterInputType(
            UnityAction action,
            KeyCode key = KeyCode.Mouse0,
            bool onPress = true,
            bool onHold = false,
            bool onRelease = false,
            bool gameAction = true,
            bool systemAction = false,
            bool debugAction = false)
        {
            this.action = action;
            this.key = key;
            this.onPress = onPress;
            this.onHold = onHold;
            this.onRelease = onRelease;
            this.gameAction = gameAction;
            this.systemAction = systemAction;
            this.debugAction = debugAction;
        }

        public UnityAction action;
        public KeyCode key = KeyCode.Mouse0;
        public bool onPress = true;
        public bool onHold = false;
        public bool onRelease = false;
        public bool gameAction = true;
        public bool systemAction = false;
        public bool debugAction = false;
    }
}