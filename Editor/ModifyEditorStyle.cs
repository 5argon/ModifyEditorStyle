using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Reflection;
using RotaryHeart.Lib;

public class ModifyEditorStyle
{
    private const string defaultFont =
#if UNITY_EDITOR_WIN
    "Segoe UI";
#else
    "Lucida Grande";
#endif

    private static bool enable
    {
        get
        {
            return EditorPrefs.GetBool("ModifyEditorStyle_Enable", true);
        }
        set
        {
            EditorPrefs.SetBool("ModifyEditorStyle_Enable", value);
        }
    }

    private static int fontSize
    {
        get
        {
            return EditorPrefs.GetInt("ModifyEditorStyle_FontSize", 11);
        }
        set
        {
            EditorPrefs.SetInt("ModifyEditorStyle_FontSize", value);
        }
    }

    private static int smallFontSize
    {
        get
        {
            return EditorPrefs.GetInt("ModifyEditorStyle_SmallFontSize", 9);
        }
        set
        {
            EditorPrefs.SetInt("ModifyEditorStyle_SmallFontSize", value);
        }
    }

    private static int bigFontSize
    {
        get
        {
            return EditorPrefs.GetInt("ModifyEditorStyle_BigFontSize", 12);
        }
        set
        {
            EditorPrefs.SetInt("ModifyEditorStyle_BigFontSize", value);
        }
    }

    private static int paddingTop
    {
        get
        {
            return EditorPrefs.GetInt("ModifyEditorStyle_PaddingTop", 1);
        }
        set
        {
            EditorPrefs.SetInt("ModifyEditorStyle_PaddingTop", value);
        }
    }

    private static int paddingBottom
    {
        get
        {
            return EditorPrefs.GetInt("ModifyEditorStyle_PaddingBottom", 2);
        }
        set
        {
            EditorPrefs.SetInt("ModifyEditorStyle_PaddingBottom", value);
        }
    }

    private static string selected
    {
        get
        {
            return EditorPrefs.GetString("ModifyEditorStyle_Selected", defaultFont);
        }
        set
        {
            EditorPrefs.SetString("ModifyEditorStyle_Selected", string.IsNullOrEmpty(value) ? defaultFont : value);
        }
    }

    private static string selectedBold
    {
        get
        {
            return EditorPrefs.GetString("ModifyEditorStyle_SelectedBold", defaultFont);
        }
        set
        {
            EditorPrefs.SetString("ModifyEditorStyle_SelectedBold", string.IsNullOrEmpty(value) ? defaultFont : value);
        }
    }

    private static string[] _fonts;
    private static string[] fonts
    {
        get
        {
            if (_fonts == null)
            {
                _fonts = Font.GetOSInstalledFontNames();
            }
            return _fonts;
        }
    }

    private static IEnumerable<GUIStyle> GUISkinStyles
    {
        get
        {
            GUISkin skin = GUI.skin;
            yield return skin.label;
            yield return skin.button;
            yield return skin.textArea;
            yield return skin.textField;
        }
    }

    /// You can comment out what you don't wanna change
    private static IEnumerable<GUIStyle> EditorStylesGUIStyles
    {
        get
        {
            yield return EditorStyles.colorField;
            yield return EditorStyles.foldout;
            yield return EditorStyles.foldoutPreDrop;
            yield return EditorStyles.label;
            yield return EditorStyles.numberField; //textField
            yield return EditorStyles.objectField;
            yield return EditorStyles.objectFieldMiniThumb;
            yield return EditorStyles.radioButton;
            yield return EditorStyles.textArea; //textField
            yield return EditorStyles.textField; //textField
            yield return EditorStyles.toggle;
            yield return EditorStyles.whiteLabel;
            yield return EditorStyles.wordWrappedLabel;
        }
    }

    private static IEnumerable<GUIStyle> NeedPadding
    {
        get
        {
            GUISkin skin = GUI.skin;
            yield return skin.label;
            yield return skin.textArea;
            yield return skin.textField;
            yield return EditorStyles.foldout;
            yield return EditorStyles.foldoutPreDrop;
            yield return EditorStyles.label;
            yield return EditorStyles.textArea; //textField
            yield return EditorStyles.textField; //textField
            yield return EditorStyles.numberField; //textField

            yield return GUI.skin.FindStyle("TV Line");
            yield return GUI.skin.FindStyle("TV Insertion");
            yield return GUI.skin.FindStyle("TV Ping");
            yield return GUI.skin.FindStyle("TV Selection");

            //Styles in older version
            yield return GUI.skin.FindStyle("IN Foldout");
            yield return GUI.skin.FindStyle("PR Insertion");
            yield return GUI.skin.FindStyle("PR Label");
        }
    }

    private static IEnumerable<GUIStyle> EditorStylesBold
    {
        get
        {
            yield return EditorStyles.boldLabel;
            yield return EditorStyles.toggleGroup; //BoldToggle
            yield return EditorStyles.whiteBoldLabel;

            //Internal style
            yield return GUI.skin.FindStyle("TV LineBold");
        }

    }

    private static IEnumerable<GUIStyle> EditorStylesBig
    {
        get
        {
            yield return EditorStyles.largeLabel;
            yield return EditorStyles.whiteLargeLabel;
        }
    }

    private static IEnumerable<GUIStyle> EditorStylesSmall
    {
        get
        {
            yield return EditorStyles.centeredGreyMiniLabel; //Same as miniLabel
            yield return EditorStyles.helpBox;
            yield return EditorStyles.layerMaskField; //MiniPopup
            yield return EditorStyles.miniBoldLabel;
            yield return EditorStyles.miniButton;
            yield return EditorStyles.miniButtonLeft;
            yield return EditorStyles.miniButtonMid;
            yield return EditorStyles.miniButtonRight;
            yield return EditorStyles.miniLabel;
            yield return EditorStyles.miniTextField;
            yield return EditorStyles.objectFieldThumb;
            yield return EditorStyles.popup; //MiniPopup
            yield return EditorStyles.toolbar;
            yield return EditorStyles.toolbarButton;
            yield return EditorStyles.toolbarDropDown;
            yield return EditorStyles.toolbarPopup;
            yield return EditorStyles.toolbarTextField;
            yield return EditorStyles.whiteMiniLabel;
            yield return EditorStyles.wordWrappedMiniLabel;

            //Not available in 2017.1.5f1 but available in 2018.3, not sure when was it added.
#if UNITY_2018_3_OR_NEWER
            yield return EditorStyles.miniPullDown;
#endif

            //Internal styles
            yield return GUI.skin.FindStyle("GV Gizmo DropDown");
        }
    }

    /// You can comment out what you don't wanna change
    private static IEnumerable<GUIStyle> InternalStyles
    {
        get
        {
            yield return GUI.skin.FindStyle("TV Line");
            yield return GUI.skin.FindStyle("TV Insertion");
            yield return GUI.skin.FindStyle("TV Ping");
            yield return GUI.skin.FindStyle("TV Selection");

            //Styles in older version
            yield return GUI.skin.FindStyle("IN Foldout");
            yield return GUI.skin.FindStyle("PR Insertion");
            yield return GUI.skin.FindStyle("PR Label");

        }
    }

#if UNITY_2018_3_OR_NEWER
    private class ModifyEditorStyleProvider : SettingsProvider
    {
        public ModifyEditorStyleProvider(string path, SettingsScopes scopes = SettingsScopes.Any)
        : base(path, scopes)
        { }

        public override void OnGUI(string searchContext)
        {
            ModifyEditorStylePreference();
        }
    }

    [SettingsProvider]
    static SettingsProvider ModifyEditorStyleSettingsProvider()
    {
        return new ModifyEditorStyleProvider("Preferences/Modify Editor Style");
    }
#else
    [PreferenceItem("Modify Editor Style")]
#endif
    static void ModifyEditorStylePreference()
    {
        EditorGUILayout.HelpBox("Changing the font size works but unfortunately the line height used in various drawers was baked as a const 16, we could not change it as a const was baked throughout the compiled Unity source code. (The enlarged characters with hanging part like 'g' will clip.)\n\nAlso, some parts seems to not change immediately until you recompile something.", MessageType.Info);

        enable = EditorGUILayout.BeginToggleGroup("Enable", enable);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Font");
        if (EditorGUILayout.DropdownButton(new GUIContent(selected), FocusType.Keyboard))
        {
            LabelWindow window = new LabelWindow();
            window.OnSelect = (string value, bool enabled) =>
            {
                if (enabled)
                    selected = value;
                else
                    selected = null;
            };
            window.OpenLabelWindow(new Rect(Event.current.mousePosition, Vector2.one), fonts, new string[] { selected }, false, false, true, false);
        }
        EditorGUILayout.EndHorizontal();


        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Bold Font");
        if (EditorGUILayout.DropdownButton(new GUIContent(selectedBold), FocusType.Keyboard))
        {
            LabelWindow window = new LabelWindow();
            window.OnSelect = (string value, bool enabled) =>
            {
                if (enabled)
                    selectedBold = value;
                else
                    selectedBold = null;
            };
            window.OpenLabelWindow(new Rect(Event.current.mousePosition, Vector2.one), fonts, new string[] { selectedBold }, false, false, true, false);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        fontSize = EditorGUILayout.IntField("Font Size", fontSize);
        smallFontSize = EditorGUILayout.IntField("Small Font Size", smallFontSize);
        bigFontSize = EditorGUILayout.IntField("Big Font Size", bigFontSize);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Applies custom paddings to certain UI", MessageType.Info);
        paddingTop = EditorGUILayout.IntField("Padding Top", paddingTop);
        paddingBottom = EditorGUILayout.IntField("Padding Bottom", paddingBottom);

        if (GUILayout.Button("Modify"))
        {
            Modify();
        }

        EditorGUILayout.EndToggleGroup();
    }

    //These statics are cleared out so often including just on loading a new scene.. it makes the linked font disappear
    private static Font normalFont;
    private static Font bigFont;
    private static Font smallFont;
    private static Font boldFont;
    private static Font smallBoldFont;

    static void Modify()
    {
        if (!enable) return;


        normalFont = Font.CreateDynamicFontFromOSFont(selected, fontSize);
        //bigFont = Font.CreateDynamicFontFromOSFont(selected, bigFontSize);
        smallFont = Font.CreateDynamicFontFromOSFont(selected, smallFontSize);
        boldFont = Font.CreateDynamicFontFromOSFont(selectedBold, fontSize);
        smallBoldFont = Font.CreateDynamicFontFromOSFont(selectedBold, smallFontSize);

        GUISkin skin = GUI.skin;
        //Debug.Log($"- : {skin.font?.name} {skin.font?.fontSize}");
        skin.font = normalFont;
        GUI.skin = skin; //SetDefaultFont activated on this setter

        //EditorStyles static was pulled from s_Current which was populated from `EditorGUIUtility.GetBuiltinSkin` which we cannot interfere.
        //s_Current is internal and therefore we need to reflect to change the font. All other styles are accessible except the fonts.
        var eType = typeof(EditorStyles);
        var es = (EditorStyles)(eType.GetField("s_Current", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
        eType.GetField("m_StandardFont", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(es, normalFont);
        eType.GetField("m_BoldFont", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(es, boldFont);
        eType.GetField("m_MiniFont", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(es, smallFont);
        eType.GetField("m_MiniBoldFont", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(es, smallBoldFont);

        //We should not override font where there's no font in the first place, because that will make the fallback switch
        //to bold on override not working.

        // foreach (var z in GUISkinStyles)
        // {
        //     Debug.Log($"{z.name} : {z.font?.name} {z.font?.fontSize} {z.fontSize} {z.padding}");
        // }

        // foreach (var x in EditorStylesGUIStyles)
        // {
        //     if (x != null)
        //     {
        //         if(x.font != null)
        //         {
        //             Debug.Log($"{x.name} : {x.font.name} {x.font.fontSize} {x.fontSize} {x.padding}");
        //         }
        //         else
        //         {
        //             Debug.Log($"{x.name} : NO FONT {x.fontSize} {x.padding}");
        //         }
        //     }
        // }

        foreach (var x in NeedPadding)
        {
            if (x != null)
            {
                //Debug.Log($"{x.name} -> {x.padding}");
                var p = x.padding;
                p.top = paddingTop;
                p.bottom = paddingBottom;
                x.padding = p;
            }
        }

        foreach (var x in EditorStylesBig)
        {
            if (x != null)
            {
                // if(x.font != null)
                // {
                //     Debug.Log($"{x.name} : {x.font.name} {x.font.fontSize} {x.fontSize} {x.padding}");
                // }
                // else
                // {
                //     Debug.Log($"{x.name} : NO FONT {x.fontSize} {x.padding}");
                // }

                x.fontSize = bigFontSize;
            }
        }

        foreach (var x in EditorStylesSmall)
        {
            if (x != null)
            {
                // if(x.font != null)
                // {
                //     Debug.Log($"SMALL {x.name} : {x.font.name} {x.font.fontSize} {x.fontSize} {x.padding}");
                // }
                // else
                // {
                //     Debug.Log($"SMALL {x.name} : NO FONT {x.fontSize} {x.padding}");
                // }

                x.fontSize = smallFontSize;
            }
        }

        // foreach (var x in EditorStylesBold)
        // {
        //     if (x != null)
        //     {
        //         if(x.font != null)
        //         {
        //             Debug.Log($"{x.name} : {x.font.name} {x.font.fontSize} {x.fontSize} {x.padding}");
        //         }
        //         else
        //         {
        //             Debug.Log($"{x.name} : NO FONT {x.fontSize} {x.padding}");
        //         }
        //     }
        // }

        // foreach (var x in InternalStyles)
        // {
        //     if (x != null)
        //     {
        //         if(x.font != null)
        //         {
        //             Debug.Log($"{x.name} : {x.font.name} {x.font.fontSize} {x.fontSize} {x.padding}");
        //         }
        //         else
        //         {
        //             Debug.Log($"{x.name} : NO FONT {x.fontSize} {x.padding}");
        //         }
        //     }
        // }

        // Debug.Log($"Modified");
    }

    static void ModifyStartUp(int instanceID, Rect selectionRect)
    {
        Modify();
        EditorApplication.hierarchyWindowItemOnGUI -= ModifyStartUp;
    }

    static void ModifySceneChange(Scene scene, OpenSceneMode mode)
    {
        EditorApplication.hierarchyWindowItemOnGUI -= ModifyStartUp;
        EditorApplication.hierarchyWindowItemOnGUI += ModifyStartUp;
    }

    [InitializeOnLoad]
    public class Startup
    {
        static Startup()
        {
            //Debug.Log($"STARTUP!!!");
            EditorApplication.hierarchyWindowItemOnGUI -= ModifyStartUp;
            EditorApplication.hierarchyWindowItemOnGUI += ModifyStartUp;

            //Somehow loading a new scene clears the static variable that we stored the font?
            EditorSceneManager.sceneOpened -= ModifySceneChange;
            EditorSceneManager.sceneOpened += ModifySceneChange;
        }
    }
}
