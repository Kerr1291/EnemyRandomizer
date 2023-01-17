using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;
using System.Reflection;
using System;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif

namespace nv
{
    /// <summary>
    /// This class will attempt to reflect the value in the member you bind to it into the into the bound uiElement.
    /// This class is setup to be easily extendable to enable support for new UI types.
    /// </summary>
    [ExecuteInEditMode]
    public class SyncMemberToUI : MonoBehaviour
    {
        [SerializeField]
        protected UnityEngine.Events.UnityEventCallState updateMode = UnityEngine.Events.UnityEventCallState.EditorAndRuntime;

        /// <summary>
        /// The owner of the target (The game object in the case where the target is a component). May be the same reference as target.
        /// </summary>
        [SerializeField]
        protected UnityEngine.Object rootGetTarget;

        /// <summary>
        /// The owner of the member reference.
        /// </summary>
        [SerializeField]
        protected UnityEngine.Object targetGet;

        /// <summary>
        /// The owner of the target (The game object in the case where the target is a component). May be the same reference as target.
        /// </summary>
        [SerializeField]
        protected UnityEngine.Object rootSetTarget;

        /// <summary>
        /// The owner of the member reference.
        /// </summary>
        [SerializeField]
        protected UnityEngine.Object targetSet;

        /// <summary>
        /// The member that will be reflected and used to populate the UI
        /// </summary>
        [SerializeField]
        protected SerializableMemberInfo targetGetRef;

        /// <summary>
        /// The member that will be updated if the target is changed or interacted with. Used for things like input fields, toggles, etc.
        /// May be the same as targetGetRef.
        /// </summary>
        [SerializeField]
        protected SerializableMemberInfo targetSetRef;   

        /// <summary>
        /// The UI element that will display the value of targetRef and provide interaction for targetSetRef.
        /// </summary>
        [SerializeField]
        protected UnityEngine.Object uiElement;

        public virtual void SetUIEelement(UnityEngine.Object uiElement)
        {
            this.uiElement = uiElement;
        }

        public virtual void SetUISourceTarget(GameObject targetOwner, UnityEngine.Object targetComponent, MemberInfo targetMember, UnityEngine.Events.UnityEventCallState updateMode = UnityEngine.Events.UnityEventCallState.RuntimeOnly)
        {
            rootGetTarget = targetOwner;
            targetGet = targetComponent;
            targetGetRef = new SerializableMemberInfo();
            targetGetRef.Info = targetMember;
            this.updateMode = updateMode;
        }

        public virtual void SetUIInputTarget(GameObject targetOwner, UnityEngine.Object targetComponent, MemberInfo targetMember, UnityEngine.Events.UnityEventCallState updateMode = UnityEngine.Events.UnityEventCallState.RuntimeOnly)
        {
            rootSetTarget = targetOwner;
            targetSet = targetComponent;
            targetSetRef = new SerializableMemberInfo();
            targetSetRef.Info = targetMember;
            this.updateMode = updateMode;
        }

        protected virtual void Reset()
        {
            Setup();
            RegisterUICallbacks();
        }

        protected virtual void Awake()
        {
            Setup();
            RegisterUICallbacks();
        }

        protected virtual void OnDestroy()
        {
            UnRegisterUICallbacks();
        }

        protected virtual void Setup()
        {
            uiElement = GetComponents<UIBehaviour>().Where(x =>
            (x as Text != null) ||
            (x as InputField != null) ||
            (x as TMPro.TMP_InputField != null) ||
            (x as Slider != null) ||
            (x as Toggle != null) ||
            (x as Scrollbar != null) ||
            (x as Dropdown != null)).FirstOrDefault();
            if( uiElement == null )
                uiElement = GetComponent<TextMesh>();
            if( uiElement == null )
                uiElement = GetComponent<TMPro.TextMeshPro>();
        }

        protected virtual void SetValueFromTMProInputField(string value)
        {
            object currentValue = targetSetRef.GetValue(targetSet);
            if (currentValue.ToString() == value)
                return;

            var element = uiElement as TMPro.TMP_InputField;

            int? ivalue = null;
            float? fvalue = null;

            if (element.contentType == TMPro.TMP_InputField.ContentType.DecimalNumber)
                fvalue = System.Convert.ToSingle(value);
            else if (element.contentType == TMPro.TMP_InputField.ContentType.IntegerNumber)
                ivalue = System.Convert.ToInt32(value);
            else if (element.contentType == TMPro.TMP_InputField.ContentType.Pin)
                ivalue = System.Convert.ToInt32(value);
            else
            {
                try
                {
                    ivalue = System.Convert.ToInt32(value);
                }
                catch (System.Exception)
                { }

                try
                {
                    fvalue = System.Convert.ToSingle(value);
                }
                catch (System.Exception)
                { }
            }

            if (ivalue != null && ivalue.HasValue)
                targetSetRef.SetValue(targetSet, ivalue.Value);
            else if (fvalue != null && fvalue.HasValue)
                targetSetRef.SetValue(targetSet, fvalue.Value);
            else
                targetSetRef.SetValue(targetSet, value);
        }

        protected virtual void SetValueFromInputField(string value)
        {
            object currentValue = targetSetRef.GetValue(targetSet);
            if(currentValue.ToString() == value)
                return;

            var element = uiElement as InputField;

            int? ivalue = null;
            float? fvalue = null;

            if(element.contentType == InputField.ContentType.DecimalNumber)
                fvalue = System.Convert.ToSingle(value);
            else if(element.contentType == InputField.ContentType.IntegerNumber)
                ivalue = System.Convert.ToInt32(value);
            else if(element.contentType == InputField.ContentType.Pin)
                ivalue = System.Convert.ToInt32(value);
            else
            {
                try
                {
                    ivalue = System.Convert.ToInt32(value);
                }
                catch(System.Exception)
                { }

                try
                {
                    fvalue = System.Convert.ToSingle(value);
                }
                catch(System.Exception)
                { }
            }

            if(ivalue != null && ivalue.HasValue)
                targetSetRef.SetValue(targetSet, ivalue.Value);
            else if(fvalue != null && fvalue.HasValue)
                targetSetRef.SetValue(targetSet, fvalue.Value);
            else
                targetSetRef.SetValue(targetSet, value);
        }

        protected virtual void SetValueFromSliderOrScrollbar(float value)
        {
            //only update if the value has changed
            if(Mathf.Approximately(targetSetRef.GetValue<float>(targetSet),value))
                return;

            targetSetRef.SetValue(targetSet, value);
        }

        protected virtual void SetValueFromToggle(bool value)
        {
            if(targetSetRef.GetValue<bool>(targetSet) == value)
                return;

            targetSetRef.SetValue(targetSet, value);
        }

        protected virtual void SetValueFromDropdown(int value)
        {
            object target = targetGetRef.GetValue(targetGet);
            object listElement = GetValueFromIterableType(target,value);

            if(targetSetRef.GetValue(targetSet) == listElement)
                return;

            targetSetRef.SetValue(targetSet, listElement);
        }




        protected virtual void LateUpdate()
        {
            if(updateMode == UnityEngine.Events.UnityEventCallState.EditorAndRuntime && Application.isEditor && !Application.isPlaying)
            {
                UpdateUIValue();
            }
            else if((updateMode == UnityEngine.Events.UnityEventCallState.EditorAndRuntime || updateMode == UnityEngine.Events.UnityEventCallState.RuntimeOnly) && Application.isPlaying)
            {
                UpdateUIValue();
            }
            else if(updateMode == UnityEngine.Events.UnityEventCallState.Off)
            {
                //???
            }
        }

        protected virtual void UpdateUIValue()
        {
            if(targetGet == null || rootGetTarget == null || targetGetRef.Info == null)
                return;

            object newTargetValue = targetGetRef.GetValue(targetGet);
            object oldTargetValue = null;

            try
            {
                oldTargetValue = targetSetRef.GetValue(targetSet);
            }
            catch(Exception)
            {
                oldTargetValue = GetValueFromUI(newTargetValue);
            }

            TryUpdateTextUI(oldTargetValue, newTargetValue);
            TryUpdateTextMeshUI(oldTargetValue, newTargetValue);
            TryUpdateTextMeshProUI(oldTargetValue, newTargetValue);
            TryUpdateInputFieldUI(oldTargetValue, newTargetValue);
            TryUpdateTMProInputFieldUI(oldTargetValue, newTargetValue);
            TryUpdateToggleUI(oldTargetValue, newTargetValue);
            TryUpdateSliderUI(oldTargetValue, newTargetValue);
            TryUpdateScrollbarUI(oldTargetValue, newTargetValue);
            TryUpdateDropdownUI(oldTargetValue, newTargetValue);
            TryUpdateTMProDropdownUI(oldTargetValue, newTargetValue);
        }

        //TODO: clean this up and cache the lookups into a map
        protected virtual object GetValueFromUI(object newTargetValue)
        {
            uiElement = GetComponents<UIBehaviour>().Where(x =>
            (x as Text != null) ||
            (x as InputField != null) ||  //TODO: enable for password
            (x as TMPro.TMP_InputField != null) ||
            (x as TMPro.TMP_Text != null) ||  //TODO
            (x as Slider != null) ||
            (x as Toggle != null) ||
            (x as Scrollbar != null) ||
            (x as Dropdown != null) ||
            (x as TMPro.TMP_Dropdown != null)   //TODO
            ).FirstOrDefault();
            if(uiElement == null)
                uiElement = GetComponent<TextMesh>();
            if(uiElement == null)
                uiElement = GetComponent<TMPro.TextMeshPro>();

            if (uiElement != null)
            {
                if ((uiElement as Text) != null)
                    return (uiElement as Text).text;
                else if ((uiElement as TextMesh) != null)
                    return (uiElement as TextMesh).text;
                else if ((uiElement as TMPro.TextMeshPro) != null)
                    return (uiElement as TMPro.TextMeshPro).text;
                else if ((uiElement as InputField) != null)
                    return (uiElement as InputField).text;
                else if ((uiElement as TMPro.TMP_InputField) != null)
                    return (uiElement as TMPro.TMP_InputField).text;               
                else if ((uiElement as Slider) != null)
                    return (uiElement as Slider).value;
                else if ((uiElement as Toggle) != null)
                    return (uiElement as Toggle).isOn;
                else if ((uiElement as Scrollbar) != null)
                    return (uiElement as Scrollbar).value;
                else if ((uiElement as Dropdown) != null)
                    return GetValueFromIterableType(newTargetValue, (uiElement as Dropdown).value);
                else if ((uiElement as TMPro.TMP_Dropdown) != null)
                    return GetValueFromIterableType(newTargetValue, (uiElement as TMPro.TMP_Dropdown).value);
            }

            return null;
        }

        protected virtual void TryUpdateScrollbarUI(object oldTargetValue, object newTargetValue)
        {
            var element = uiElement as Scrollbar;
            if(element != null)
            {
                try
                {
                    if(Mathf.Approximately((float)oldTargetValue, (float)newTargetValue))
                        return;
                    element.value = (float)newTargetValue;
                }
                catch(System.InvalidCastException)
                {
                    Debug.Log("Scrollbar needs a float type");
                    element.value = 0f;
                }
            }
        }

        protected virtual void TryUpdateSliderUI(object oldTargetValue, object newTargetValue)
        {
            var element = uiElement as Slider;
            if(element != null)
            {
                bool success = false;
                //try casting it to each primitive type that could go in a slider
                if(!success)
                {
                    try
                    {
                        if(Mathf.Approximately((float)oldTargetValue, (float)newTargetValue))
                            return;
                        element.value = (float)newTargetValue;
                        success = true;
                    }
                    catch(System.InvalidCastException)
                    {
                        element.value = element.minValue;
                    }
                }
                else if(!success)
                {
                    try
                    {
                        if(Mathf.Approximately((int)oldTargetValue, (int)newTargetValue))
                            return;
                        element.value = (int)newTargetValue;
                        success = true;
                    }
                    catch(System.InvalidCastException)
                    {
                        Debug.Log("Slider needs a float or an int type");
                        element.value = element.minValue;
                    }
                }
            }
        }

        protected virtual void TryUpdateToggleUI(object oldTargetValue, object newTargetValue)
        {
            var element = uiElement as Toggle;
            if(element != null)
            {
                bool success = false;
                if(!success)
                {
                    try
                    {
                        if((bool)oldTargetValue == (bool)newTargetValue)
                            return;
                        element.isOn = (bool)newTargetValue;
                        success = true;
                    }
                    catch(System.InvalidCastException)
                    {
                        Debug.Log("Toggle needs a bool type");
                        element.isOn = false;
                    }
                }
            }
        }

        protected virtual void TryUpdateInputFieldUI(object oldTargetValue, object newTargetValue)
        {
            if(uiElement as InputField != null)
            {
                var element = uiElement as InputField;
                try
                {
                    string newValue = newTargetValue == null ? "null" : newTargetValue.ToString();
                    string oldValue = oldTargetValue == null ? "null" : oldTargetValue.ToString();
                    if(string.Compare(oldValue, newValue) != 0)
                    {
                        if(element != null && !element.isFocused && string.Compare(newValue,element.text) != 0)
                        {
                            element.text = newValue;
                        }
                    }
                }
                catch(System.NullReferenceException)
                {
                    element.text = "null";
                }
            }
        }

        protected virtual void TryUpdateTMProInputFieldUI(object oldTargetValue, object newTargetValue)
        {
            if (uiElement as TMPro.TMP_InputField != null)
            {
                var element = uiElement as TMPro.TMP_InputField;
                try
                {
                    string newValue = newTargetValue == null ? "null" : newTargetValue.ToString();
                    string oldValue = oldTargetValue == null ? "null" : oldTargetValue.ToString();
                    if (string.Compare(oldValue, newValue) != 0)
                    {
                        if (element != null && !element.isFocused && string.Compare(newValue, element.text) != 0)
                        {
                            element.text = newValue;
                        }
                    }
                }
                catch (System.NullReferenceException)
                {
                    element.text = "null";
                }
            }
        }

        protected virtual void TryUpdateTextUI(object oldTargetValue, object newTargetValue)
        {
            if(uiElement as Text != null)
            {
                var element = uiElement as Text;
                try
                {
                    string newValue = newTargetValue == null ? "null" : newTargetValue.ToString();
                    string oldValue = oldTargetValue == null ? "null" : oldTargetValue.ToString();
                    if(string.Compare(oldValue, newValue) != 0)
                    {
                        if(element != null && string.Compare(newValue, element.text) != 0)
                        {
                            element.text = newValue;
                        }
                    }
                }
                catch(System.NullReferenceException)
                {
                    element.text = "null";
                }
            }
        }

        protected virtual void TryUpdateTextMeshUI( object oldTargetValue, object newTargetValue)
        {
            if( uiElement as TextMesh != null )
            {
                var element = uiElement as TextMesh;
                try
                {
                    string newValue = newTargetValue == null ? "null" : newTargetValue.ToString();
                    string oldValue = oldTargetValue == null ? "null" : oldTargetValue.ToString();
                    if(string.Compare(oldValue, newValue) != 0)
                    {
                        if(element != null && string.Compare(newValue, element.text) != 0)
                        {
                            element.text = newValue;
                        }
                    }
                }
                catch(System.NullReferenceException)
                {
                    element.text = "null";
                }
            }
        }

        protected virtual void TryUpdateTextMeshProUI( object oldTargetValue, object newTargetValue)
        {
            if( uiElement as TMPro.TextMeshPro != null )
            {
                var element = uiElement as TMPro.TextMeshPro;
                try
                {
                    string newValue = newTargetValue == null ? "null" : newTargetValue.ToString();
                    string oldValue = oldTargetValue == null ? "null" : oldTargetValue.ToString();
                    if(string.Compare(oldValue, newValue) != 0)
                    {
                        if(element != null && string.Compare(newValue, element.text) != 0)
                        {
                            element.text = newValue;
                        }
                    }
                }
                catch(System.NullReferenceException)
                {
                    element.text = "null";
                }
            }
        }

        protected virtual void TryUpdateDropdownUI(object currentTargetValue, object listValue)
        {
            if( uiElement == null )
                uiElement = GetComponent<Dropdown>();
            if(listValue == null)
                return;

            var element = uiElement as Dropdown;
            if(element != null)
            {
                try
                {                    
                    if(listValue != null && listValue.GetType().IsArray)
                    {
                        Array array = listValue as Array;
                        for(int i = 0; i < array.Length || i < element.options.Count; ++i)
                        {
                            if(i < array.Length && i < element.options.Count)
                            {
                                object value = array.GetValue(i);
                                element.options[i].text = (value == null ? "null" : value.ToString());
                            }
                            else if(i < array.Length) //but bigger than the dropdown options...
                            {
                                object value = array.GetValue(i);
                                element.options.Add(new Dropdown.OptionData(value == null ? "null" : value.ToString()));
                            }
                            else if(i < element.options.Count)//but bigger than the array source...
                            {
                                element.options.RemoveRange(i, element.options.Count - i);
                            }
                        }
                    }
                    else if(typeof(IList).IsAssignableFrom(listValue == null ? null : listValue.GetType()))
                    {
                        IList list = listValue as IList;
                        for(int i = 0; i < list.Count || i < element.options.Count; ++i)
                        {
                            if(i < list.Count && i < element.options.Count)
                            {
                                object value = list[i];
                                element.options[i].text = (value == null ? "null" : value.ToString());
                            }
                            else if(i < list.Count) //but bigger than the dropdown options...
                            {
                                object value = list[i];
                                element.options.Add(new Dropdown.OptionData(value == null ? "null" : value.ToString()));
                            }
                            else if(i < element.options.Count)//but bigger than the array source...
                            {
                                element.options.RemoveRange(i, element.options.Count - i);
                            }
                        }
                    }
                    else if(typeof(IEnumerable).IsAssignableFrom(listValue == null ? null : listValue.GetType()))
                    {
                        int enumerableMax = 1000;
                        var iterable = (listValue as IEnumerable);
                        var iterator = iterable.GetEnumerator();
                        bool isGood = iterator.MoveNext();
                        for(int i = 0; isGood || i < element.options.Count; ++i, isGood = iterator.MoveNext())
                        {
                            if(i > enumerableMax)
                                break;

                            if(isGood && i < element.options.Count)
                            {
                                object value = iterator.Current;
                                element.options[i].text = (value == null ? "null" : value.ToString());
                            }
                            else if(isGood) //but bigger than the dropdown options...
                            {
                                object value = iterator.Current;
                                element.options.Add(new Dropdown.OptionData(value == null ? "null" : value.ToString()));
                            }
                            else if(i < element.options.Count)//but bigger than the array source...
                            {
                                element.options.RemoveRange(i, element.options.Count - i);
                            }
                        }
                    }

                    int? index = GetIndexOfValueFromIterableType(listValue, currentTargetValue);
                    element.value = index == null ? 0 : index.Value;
                }
                catch(System.InvalidCastException)
                {
                    Debug.Log("Dropdown needs an int type to select the dropdown index");
                    element.value = 0;
                }
            }
        }

        protected virtual void TryUpdateTMProDropdownUI(object currentTargetValue, object listValue)
        {
            if (uiElement == null)
                uiElement = GetComponent<TMPro.TMP_Dropdown>();

            if (listValue == null)
                return;

            var element = uiElement as TMPro.TMP_Dropdown;
            if (element != null)
            {
                try
                {
                    if (listValue != null && listValue.GetType().IsArray)
                    {
                        Array array = listValue as Array;
                        for (int i = 0; i < array.Length || i < element.options.Count; ++i)
                        {
                            if (i < array.Length && i < element.options.Count)
                            {
                                object value = array.GetValue(i);
                                element.options[i].text = (value == null ? "null" : value.ToString());
                            }
                            else if (i < array.Length) //but bigger than the dropdown options...
                            {
                                object value = array.GetValue(i);
                                element.options.Add(new TMPro.TMP_Dropdown.OptionData(value == null ? "null" : value.ToString()));
                            }
                            else if (i < element.options.Count)//but bigger than the array source...
                            {
                                element.options.RemoveRange(i, element.options.Count - i);
                            }
                        }
                    }
                    else if (typeof(IList).IsAssignableFrom(listValue == null ? null : listValue.GetType()))
                    {
                        IList list = listValue as IList;
                        for (int i = 0; i < list.Count || i < element.options.Count; ++i)
                        {
                            if (i < list.Count && i < element.options.Count)
                            {
                                object value = list[i];
                                element.options[i].text = (value == null ? "null" : value.ToString());
                            }
                            else if (i < list.Count) //but bigger than the dropdown options...
                            {
                                object value = list[i];
                                element.options.Add(new TMPro.TMP_Dropdown.OptionData(value == null ? "null" : value.ToString()));
                            }
                            else if (i < element.options.Count)//but bigger than the array source...
                            {
                                element.options.RemoveRange(i, element.options.Count - i);
                            }
                        }
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(listValue == null ? null : listValue.GetType()))
                    {
                        int enumerableMax = 1000;
                        var iterable = (listValue as IEnumerable);
                        var iterator = iterable.GetEnumerator();
                        bool isGood = iterator.MoveNext();
                        for (int i = 0; isGood || i < element.options.Count; ++i, isGood = iterator.MoveNext())
                        {
                            if (i > enumerableMax)
                                break;

                            if (isGood && i < element.options.Count)
                            {
                                object value = iterator.Current;
                                element.options[i].text = (value == null ? "null" : value.ToString());
                            }
                            else if (isGood) //but bigger than the dropdown options...
                            {
                                object value = iterator.Current;
                                element.options.Add(new TMPro.TMP_Dropdown.OptionData(value == null ? "null" : value.ToString()));
                            }
                            else if (i < element.options.Count)//but bigger than the array source...
                            {
                                element.options.RemoveRange(i, element.options.Count - i);
                            }
                        }
                    }

                    int? index = GetIndexOfValueFromIterableType(listValue, currentTargetValue);
                    element.value = index == null ? 0 : index.Value;
                }
                catch (System.InvalidCastException)
                {
                    Debug.Log("Dropdown needs an int type to select the dropdown index");
                    element.value = 0;
                }
            }
        }

        protected object GetValueFromIterableType(object target, int index)
        {
            object listElement = null;
            if(target != null && target.GetType().IsArray)
            {
                Array array = target as Array;
                listElement = array.GetValue(index);
            }
            else if(typeof(IList).IsAssignableFrom(target == null ? null : target.GetType()))
            {
                if (index >= (target as IList).Count)
                    return null;

                listElement = (target as IList)[index];
            }
            else if(typeof(IEnumerable).IsAssignableFrom(target == null ? null : target.GetType()))
            {
                var iterable = (target as IEnumerable);
                int i = 0;
                foreach(var element in iterable)
                {
                    if(i == index)
                    {
                        listElement = element;
                        break;
                    }
                    ++i;
                }
            }
            return listElement;
        }

        protected int? GetIndexOfValueFromIterableType(object iterableObject, object value)
        {
            if(iterableObject != null && iterableObject.GetType().IsArray)
            {
                Array array = iterableObject as Array;
                for(int i = 0; i < array.Length; ++i)
                {
                    if(array.GetValue(i) == value)
                        return i;
                }
            }
            else if(iterableObject != null && typeof(IList).IsAssignableFrom(iterableObject.GetType()))
            {
                IList list = iterableObject as IList;
                for(int i = 0; i < list.Count; ++i)
                {
                    if(list[i] == value)
                        return i;
                }
            }
            else if(iterableObject != null && typeof(IEnumerable).IsAssignableFrom(iterableObject.GetType()))
            {
                var iterable = (iterableObject as IEnumerable);
                int i = 0;
                foreach(var element in iterable)
                {
                    if(element == value)
                    {
                        return i;
                    }
                    ++i;
                }
            }
            return null;
        }

        protected virtual void RegisterUICallbacks()
        {
            RegisterTMProInputFieldCallbacks();
            RegisterInputFieldCallbacks();
            RegisterToggleCallbacks();
            RegisterSliderCallbacks();
            RegisterScrollbarCallbacks();
            RegisterDropdownCallbacks();
            RegisterTMProDropdownCallbacks();
        }

        protected virtual void UnRegisterUICallbacks()
        {
            UnRegisterTMProInputFieldCallbacks();
            UnRegisterInputFieldCallbacks();
            UnRegisterToggleCallbacks();
            UnRegisterSliderCallbacks();
            UnRegisterScrollbarCallbacks();
            UnRegisterDropdownCallbacks();
            UnRegisterTMProDropdownCallbacks();
        }

        protected virtual void RegisterTMProDropdownCallbacks()
        {
            var element = uiElement as TMPro.TMP_Dropdown;
            if (element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<int>(element.onValueChanged, SetValueFromDropdown);
                UnityEventTools.AddPersistentListener<int>(element.onValueChanged, SetValueFromDropdown);
#else
                element.onValueChanged.RemoveListener(SetValueFromDropdown);
                element.onValueChanged.AddListener(SetValueFromDropdown);
#endif
            }
        }

        protected virtual void RegisterDropdownCallbacks()
        {
            var element = uiElement as Dropdown;
            if(element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<int>(element.onValueChanged, SetValueFromDropdown);
                UnityEventTools.AddPersistentListener<int>(element.onValueChanged, SetValueFromDropdown);
#else
                element.onValueChanged.RemoveListener(SetValueFromDropdown);
                element.onValueChanged.AddListener(SetValueFromDropdown);
#endif
            }
        }

        protected virtual void RegisterScrollbarCallbacks()
        {
            var element = uiElement as Scrollbar;
            if(element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<float>(element.onValueChanged, SetValueFromSliderOrScrollbar);
                UnityEventTools.AddPersistentListener<float>(element.onValueChanged, SetValueFromSliderOrScrollbar);
#else
                element.onValueChanged.RemoveListener(SetValueFromSliderOrScrollbar);
                element.onValueChanged.AddListener(SetValueFromSliderOrScrollbar);
#endif
            }
        }

        protected virtual void RegisterSliderCallbacks()
        {
            var element = uiElement as Slider;
            if(element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<float>(element.onValueChanged, SetValueFromSliderOrScrollbar);
                UnityEventTools.AddPersistentListener<float>(element.onValueChanged, SetValueFromSliderOrScrollbar);
#else
                element.onValueChanged.RemoveListener(SetValueFromSliderOrScrollbar);
                element.onValueChanged.AddListener(SetValueFromSliderOrScrollbar);
#endif
            }
        }

        protected virtual void RegisterToggleCallbacks()
        {
            var element = uiElement as Toggle;
            if(element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<bool>(element.onValueChanged, SetValueFromToggle);
                UnityEventTools.AddPersistentListener<bool>(element.onValueChanged, SetValueFromToggle);
#else
                element.onValueChanged.RemoveListener(SetValueFromToggle);
                element.onValueChanged.AddListener(SetValueFromToggle);
#endif
            }
        }

        protected virtual void RegisterInputFieldCallbacks()
        {
            var element = uiElement as InputField;
            if(element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<string>(element.onEndEdit, SetValueFromInputField);
                UnityEventTools.AddPersistentListener<string>(element.onEndEdit, SetValueFromInputField);
#else
                element.onEndEdit.RemoveListener(SetValueFromInputField);
                element.onEndEdit.AddListener(SetValueFromInputField);
#endif
            }
        }

        protected virtual void RegisterTMProInputFieldCallbacks()
        {
            var element = uiElement as TMPro.TMP_InputField;
            if (element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<string>(element.onEndEdit, SetValueFromTMProInputField);
                UnityEventTools.AddPersistentListener<string>(element.onEndEdit, SetValueFromTMProInputField);
#else
                element.onEndEdit.RemoveListener(SetValueFromTMProInputField);
                element.onEndEdit.AddListener(SetValueFromTMProInputField);
#endif
            }
        }



        protected virtual void UnRegisterDropdownCallbacks()
        {
            var element = uiElement as Dropdown;
            if(element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<int>(element.onValueChanged, SetValueFromDropdown);
#else
                element.onValueChanged.RemoveListener(SetValueFromDropdown);
#endif
            }
        }

        protected virtual void UnRegisterTMProDropdownCallbacks()
        {
            var element = uiElement as TMPro.TMP_Dropdown;
            if (element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<int>(element.onValueChanged, SetValueFromDropdown);
#else
                element.onValueChanged.RemoveListener(SetValueFromDropdown);
#endif
            }
        }

        protected virtual void UnRegisterScrollbarCallbacks()
        {
            var element = uiElement as Scrollbar;
            if(element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<float>(element.onValueChanged, SetValueFromSliderOrScrollbar);
#else
                element.onValueChanged.RemoveListener(SetValueFromSliderOrScrollbar);
#endif
            }
        }

        protected virtual void UnRegisterSliderCallbacks()
        {
            var element = uiElement as Slider;
            if(element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<float>(element.onValueChanged, SetValueFromSliderOrScrollbar);
#else
                element.onValueChanged.RemoveListener(SetValueFromSliderOrScrollbar);
#endif
            }
        }

        protected virtual void UnRegisterToggleCallbacks()
        {
            var element = uiElement as Toggle;
            if(element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<bool>(element.onValueChanged, SetValueFromToggle);
#else
                element.onValueChanged.RemoveListener(SetValueFromToggle);
#endif
            }
        }

        protected virtual void UnRegisterInputFieldCallbacks()
        {
            var element = uiElement as InputField;
            if(element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<string>(element.onEndEdit, SetValueFromInputField);
#else
                element.onEndEdit.RemoveListener(SetValueFromInputField);
#endif
            }
        }

        protected virtual void UnRegisterTMProInputFieldCallbacks()
        {
            var element = uiElement as TMPro.TMP_InputField;
            if (element != null)
            {
#if UNITY_EDITOR
                UnityEventTools.RemovePersistentListener<string>(element.onEndEdit, SetValueFromTMProInputField);
#else
                element.onEndEdit.RemoveListener(SetValueFromTMProInputField);
#endif
            }
        }
    }
}