using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RotaryHeart.Lib
{
    public class LabelWindow
    {
        public delegate void SelectCallback(string label, bool added);
        public SelectCallback OnSelect;

        object m_AssetLabels;
        bool allowMultipleSelection = true;
        bool allowCustom;
        Dictionary<string, float> allLabels = new Dictionary<string, float>();

        #region Reflection

        #region Fields

        Assembly m_assembly;

        Type m_inputDataType;
        Type m_listElementType;
        Type m_popupListType;

        FieldInfo m_CloseOnSelection;
        FieldInfo m_AllowCustom;
        FieldInfo m_OnSelectCallback;
        FieldInfo m_MaxCount;
        FieldInfo m_SortAlphabetically;
        FieldInfo m_EnableAutoCompletion;

        PropertyInfo m_filterScore;
        PropertyInfo m_selected;
        PropertyInfo m_text;

        MethodInfo m_OnInternalSelectCallback;
        MethodInfo m_NewOrMatchingElement;

        #endregion

        #region Properties

        Assembly EditorAssembly
        {
            get
            {
                if (m_assembly == null)
                {
                    m_assembly = typeof(Editor).Assembly;
                }

                return m_assembly;
            }
        }

        Type InputDataType
        {
            get
            {
                if (m_inputDataType == null)
                {
                    m_inputDataType = EditorAssembly.GetType("UnityEditor.PopupList+InputData");
                }

                return m_inputDataType;
            }
        }
        Type ListElementType
        {
            get
            {
                if (m_listElementType == null)
                {
                    m_listElementType = EditorAssembly.GetType("UnityEditor.PopupList+ListElement");
                }

                return m_listElementType;
            }
        }
        Type PopupList
        {
            get
            {
                if (m_popupListType == null)
                {
                    m_popupListType = EditorAssembly.GetType("UnityEditor.PopupList");
                }

                return m_popupListType;
            }
        }

        FieldInfo CloseOnSelection
        {
            get
            {
                if (m_CloseOnSelection == null)
                {
                    m_CloseOnSelection = InputDataType.GetField("m_CloseOnSelection");
                }

                return m_CloseOnSelection;
            }
        }
        FieldInfo AllowCustom
        {
            get
            {
                if (m_AllowCustom == null)
                {
                    m_AllowCustom = InputDataType.GetField("m_AllowCustom");
                }

                return m_AllowCustom;
            }
        }
        FieldInfo OnSelectCallback
        {
            get
            {
                if (m_OnSelectCallback == null)
                {
                    m_OnSelectCallback = InputDataType.GetField("m_OnSelectCallback");
                }

                return m_OnSelectCallback;
            }
        }
        FieldInfo MaxCount
        {
            get
            {
                if (m_MaxCount == null)
                {
                    m_MaxCount = InputDataType.GetField("m_MaxCount");
                }

                return m_MaxCount;
            }
        }
        FieldInfo SortAlphabetically
        {
            get
            {
                if (m_SortAlphabetically == null)
                {
                    m_SortAlphabetically = InputDataType.GetField("m_SortAlphabetically");
                }

                return m_SortAlphabetically;
            }
        }
        FieldInfo EnableAutoCompletion
        {
            get
            {
                if (m_EnableAutoCompletion == null)
                {
                    m_EnableAutoCompletion = InputDataType.GetField("m_EnableAutoCompletion");
                }

                return m_EnableAutoCompletion;
            }
        }

        PropertyInfo FilterScore
        {
            get
            {
                if (m_filterScore == null)
                {
                    m_filterScore = ListElementType.GetProperty("filterScore");
                }

                return m_filterScore;
            }
        }
        PropertyInfo Selected
        {
            get
            {
                if (m_selected == null)
                {
                    m_selected = ListElementType.GetProperty("selected");
                }

                return m_selected;
            }
        }
        PropertyInfo Text
        {
            get
            {
                if (m_text == null)
                {
                    m_text = ListElementType.GetProperty("text");
                }

                return m_text;
            }
        }

        MethodInfo OnInternalSelectCallback
        {
            get
            {
                if (m_OnInternalSelectCallback == null)
                {
                    m_OnInternalSelectCallback = typeof(LabelWindow).GetMethod("_OnInternalSelectCallback", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(ListElementType);
                }

                return m_OnInternalSelectCallback;
            }
        }
        MethodInfo NewOrMatchingElement
        {
            get
            {
                if (m_NewOrMatchingElement == null)
                {
                    m_NewOrMatchingElement = InputDataType.GetMethod("NewOrMatchingElement");
                }

                return m_NewOrMatchingElement;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Opens the default label window
        /// </summary>
        /// <param name="position">The position where the window is going to be drawn</param>
        /// <param name="labels">Label array used to populate the list</param>
        /// <param name="selectedLabels">Selected labels</param>
        /// <param name="populateDefaultLabels">Should the default labels be included</param>
        /// <param name="allowMultipleSelection">Should the system allow more than 1 label selected</param>
        /// <param name="closeOnSelection">Should the window close on selection</param>
        /// <param name="allowCustom">Allow custom labels</param>
        /// <param name="maxCount">Max label count</param>
        /// <param name="sortAlphabetically">Should the labels be sorted</param>
        /// <param name="enableAutoCompletition">Should the window show auto completetion</param>
        public void OpenLabelWindow(Rect position, string[] labels, string[] selectedLabels, bool populateDefaultLabels = true, bool allowMultipleSelection = true, bool closeOnSelection = false, bool allowCustom = true, int maxCount = 15, bool sortAlphabetically = true, bool enableAutoCompletition = true)
        {
            if (labels == null)
                labels = new string[] { };
            if (selectedLabels == null)
                selectedLabels = new string[] { };

            this.allowMultipleSelection = allowMultipleSelection;
            this.allowCustom = allowCustom;

            //Cache the data required
            CacheData(labels, selectedLabels, populateDefaultLabels, closeOnSelection, maxCount, sortAlphabetically, enableAutoCompletition);

            //Create a reference of the window
            var popupListReference = Activator.CreateInstance(PopupList, new object[] { m_AssetLabels });

            //Get the correct show method
            var showMethod = typeof(PopupWindow).GetMethods(BindingFlags.Static | BindingFlags.NonPublic).Where(x =>
            x.Name.Equals("Show") && x.GetParameters().Length == 4).Single();

            //Invoke the method with the correct arguments
            showMethod.Invoke(null, new object[] { position, popupListReference, null, 6 });
        }

        void CacheData(string[] labels, string[] selectedLabels, bool populateDefaultLabels, bool closeOnSelection, int maxCount, bool sortAlphabetically, bool enableAutoCompletition)
        {
            //Create instance to the delegate
            Delegate action = Delegate.CreateDelegate(OnSelectCallback.FieldType, this, OnInternalSelectCallback);

            //Create instance of data to send to the popup window
            m_AssetLabels = Activator.CreateInstance(InputDataType);

            //Assign all the respective values, including the delegate callback
            CloseOnSelection.SetValue(m_AssetLabels, closeOnSelection);
            AllowCustom.SetValue(m_AssetLabels, true);
            OnSelectCallback.SetValue(m_AssetLabels, action);
            MaxCount.SetValue(m_AssetLabels, maxCount);
            SortAlphabetically.SetValue(m_AssetLabels, sortAlphabetically);
            EnableAutoCompletion.SetValue(m_AssetLabels, enableAutoCompletition);

            //Get all the labels available
            if (populateDefaultLabels)
            {
                allLabels = (Dictionary<string, float>)typeof(AssetDatabase).InvokeMember("GetAllLabels", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, null);
            }

            //Include any custom one sent on the array
            foreach (var tag in labels)
            {
                if (string.IsNullOrEmpty(tag))
                    continue;

                if (!allLabels.ContainsKey(tag))
                    allLabels.Add(tag, 0);
            }

            //Asing all the selected values
            foreach (var pair in allLabels)
            {
                var element = NewOrMatchingElement.Invoke(m_AssetLabels, new object[] { pair.Key });

                if ((float)(FilterScore.GetValue(element, null)) < pair.Value)
                {
                    FilterScore.SetValue(element, pair.Value, null);
                }
                Selected.SetValue(element, selectedLabels.Any(label => string.Equals(label, pair.Key, StringComparison.OrdinalIgnoreCase)), null);
            }
        }

        /// <summary>
        /// Function called by Unity when an element is selected
        /// </summary>
        /// <param name="data">Element data</param>
        void _OnInternalSelectCallback<T>(T data)
        {
            string selectedLabel = Text.GetValue(data, null).ToString();

            if (!allowCustom && !allLabels.Keys.Any(x => x.ToLower().Equals(selectedLabel.ToLower())))
                return;

            if (!allowMultipleSelection)
            {
                foreach (var pair in allLabels)
                {
                    if (pair.Key.ToLower().Equals(selectedLabel.ToLower()))
                    {
                        continue;
                    }

                    var element = NewOrMatchingElement.Invoke(m_AssetLabels, new object[] { pair.Key });

                    Selected.SetValue(element, false, null);
                }
            }

            bool currentValue = (bool)(Selected.GetValue(data, null));

            Selected.SetValue(data, !currentValue, null);

            if (OnSelect != null)
                OnSelect.Invoke(selectedLabel, !currentValue);
        }
    }
}
