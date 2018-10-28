using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ModifyEditorStyle
{
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
    private static int fontSize = 11;
    private static int selected = 0;
    private static string[] fonts;

    /// You can comment out what you don't wanna change
    private static IEnumerable<GUIStyle> EditorStylesGUIStyles
    {
        get
        {
            yield return EditorStyles.boldLabel;
            yield return EditorStyles.centeredGreyMiniLabel;
            yield return EditorStyles.colorField;
            yield return EditorStyles.foldout;
            yield return EditorStyles.foldoutPreDrop;
            yield return EditorStyles.helpBox;
            yield return EditorStyles.label;
            yield return EditorStyles.largeLabel;
            yield return EditorStyles.layerMaskField;
            yield return EditorStyles.miniBoldLabel;
            yield return EditorStyles.miniButton;
            yield return EditorStyles.miniButtonLeft;
            yield return EditorStyles.miniButtonMid;
            yield return EditorStyles.miniButtonRight;
            yield return EditorStyles.miniLabel;
            yield return EditorStyles.miniPullDown;
            yield return EditorStyles.miniTextField;
            yield return EditorStyles.numberField;
            yield return EditorStyles.objectField;
            yield return EditorStyles.objectFieldMiniThumb;
            yield return EditorStyles.objectFieldThumb;
            yield return EditorStyles.popup;
            yield return EditorStyles.radioButton;
            yield return EditorStyles.textArea;
            yield return EditorStyles.textField;
            yield return EditorStyles.toggle;
            yield return EditorStyles.toggleGroup;
            yield return EditorStyles.toolbar;
            yield return EditorStyles.toolbarButton;
            yield return EditorStyles.toolbarDropDown;
            yield return EditorStyles.toolbarPopup;
            yield return EditorStyles.toolbarTextField;
            yield return EditorStyles.whiteBoldLabel;
            yield return EditorStyles.whiteLabel;
            yield return EditorStyles.whiteLargeLabel;
            yield return EditorStyles.whiteMiniLabel;
            yield return EditorStyles.wordWrappedLabel;
            yield return EditorStyles.wordWrappedMiniLabel;
        }
    }

    /// You can comment out what you don't wanna change
    private static IEnumerable<GUIStyle> InternalStyles
    {
        get
        {
            yield return GUI.skin.FindStyle("TV Line");
            yield return GUI.skin.FindStyle("TV Line");
            yield return GUI.skin.FindStyle("TV Insertion");
            yield return GUI.skin.FindStyle("TV Ping");
            yield return GUI.skin.FindStyle("ToolbarButton");
            yield return GUI.skin.FindStyle("TV Line");
            yield return GUI.skin.FindStyle("TV LineBold");
            yield return GUI.skin.FindStyle("TV Selection");
        }
    }

    static void ModifyEditorStylePreference()
    {
        if (fonts == null)
        {
            fonts = Font.GetOSInstalledFontNames();
        }

        EditorGUILayout.HelpBox("Changing the font size works but unfortunately the line height used in various drawers is baked as a const 16, we could not change it as a const was baked throughout the compiled Unity source code, the enlarged font will clip. The default seems to be 11, I found that going to 13 is still readable if that helps with your eye condition for the time being. (It clips characters with hanging part like 'g') Some part seems to not change immediately until you recompile something.", MessageType.Info);

        selected = EditorGUILayout.Popup("Font", selected, fonts);
        fontSize = EditorGUILayout.IntField("Font Size", fontSize);
        if (GUILayout.Button("Modify"))
        {
            GUISkin skin = GUI.skin;
            Font changeToFont = Font.CreateDynamicFontFromOSFont(fonts[selected], fontSize);

            foreach (var x in EditorStylesGUIStyles)
            {
                x.font = changeToFont;
            }
            foreach (var x in InternalStyles)
            {
                x.font = changeToFont;
            }

            skin.font = changeToFont;
            skin.label.font = changeToFont;
            skin.button.font = changeToFont;
            skin.textArea.font = changeToFont;
            skin.textField.font = changeToFont;
            GUI.skin = skin;
        }
    }
}
