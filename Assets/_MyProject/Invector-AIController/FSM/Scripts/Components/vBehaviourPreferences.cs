#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace Invector.vCharacterController.AI.FSMBehaviour
{
    public class vFSMBehaviourPreferences
    {
        public static Dictionary<string, object> keyValues = new Dictionary<string, object>();
        public static Color gridLinesColor { get { return GetValue<Color>("gridLinesColor"); } set { SetValue("gridLinesColor", value); } }
        public static Color gridBackgroundColor { get { return GetValue<Color>("gridBackgroundColor"); } set { SetValue("gridBackgroundColor", value); } }

        public static Color transitionTrueColor { get { return GetValue<Color>("transitionTrueColor"); } set { SetValue("transitionTrueColor", value); } }
        public static Color transitionFalseColor { get { return GetValue<Color>("transitionFalseColor"); } set { SetValue("transitionFalseColor", value); } }
        public static Color transitionDefaultColor { get { return GetValue<Color>("transitionDefaultColor"); } set { SetValue("transitionDefaultColor", value); } }
        public static Color transitionMuteColor { get { return GetValue<Color>("transitionMuteColor"); } set { SetValue("transitionMuteColor", value); } }

        public static Color entryNormalColor { get { return GetValue<Color>("entryNormalColor"); } set { SetValue("entryNormalColor", value); } }
        public static Color anyNormalColor { get { return GetValue<Color>("anyNormalColor"); } set { SetValue("anyNormalColor", value); } }

        public static Color entrySelectedColor { get { return GetValue<Color>("entrySelectedColor"); } set { SetValue("entrySelectedColor", value); } }
        public static Color anySelectedColor { get { return GetValue<Color>("anySelectedColor"); } set { SetValue("anySelectedColor", value); } }

        public static Color defaultStateFontColor { get { return GetValue<Color>("defaultStateFontColor"); } set { SetValue("defaultStateFontColor", value); } }
        public static Color selectedStateFontColor { get { return GetValue<Color>("selectedStateFontColor"); } set { SetValue("selectedStateFontColor", value); } }

        public static Color defaultStateColor { get { return GetValue<Color>("defaultStateColor"); } set { SetValue("defaultStateColor", value); } }
        public static Color selectedStateColor { get { return GetValue<Color>("selectedStateColor"); } set { SetValue("selectedStateColor", value); } }

        public static Color entryNormalFontColor { get { return GetValue<Color>("entryNormalFontColor"); } set { SetValue("entryNormalFontColor", value); } }
        public static Color anyNormalFontColor { get { return GetValue<Color>("anyNormalFontColor"); } set { SetValue("anyNormalFontColor", value); } }

        public static Color entrySelectedFontColor { get { return GetValue<Color>("entrySelectedFontColor"); } set { SetValue("entrySelectedFontColor", value); } }
        public static Color anySelectedFontColor { get { return GetValue<Color>("anySelectedFontColor"); } set { SetValue("anySelectedFontColor", value); } }

        public static float borderAlpha { get { return GetValue<float>("borderAlpha"); } set { SetValue("borderAlpha", value); } }


        // Add preferences section named "My Preferences" to the Preferences Window
        [PreferenceItem("vFSM Behaviour")]

        public static void PreferencesGUI()
        {
            var style = new GUIStyle(EditorStyles.boldLabel);
            style.fontSize = 12;
            GUILayout.Label("FSM View Colors", style);
            gridLinesColor = EditorGUILayout.ColorField("Grid Lines Color", gridLinesColor);
            gridBackgroundColor = EditorGUILayout.ColorField("Grid Background Color", gridBackgroundColor);
            GUILayout.Label("Special State Colors", style);
            entryNormalColor = EditorGUILayout.ColorField("Entry Normal Background", entryNormalColor);
            entryNormalFontColor = EditorGUILayout.ColorField("Entry Normal Font", entryNormalFontColor);
            entrySelectedColor = EditorGUILayout.ColorField("Entry Selected Background", entrySelectedColor);
            entrySelectedFontColor = EditorGUILayout.ColorField("Entry Selected Font", entrySelectedFontColor);
            anyNormalColor = EditorGUILayout.ColorField("Any Normal Background", anyNormalColor);
            anyNormalFontColor = EditorGUILayout.ColorField("Any Normal Font", anyNormalFontColor);
            anySelectedColor = EditorGUILayout.ColorField("Any Selected Background", anySelectedColor);
            anySelectedFontColor = EditorGUILayout.ColorField("Any Selected Font", anySelectedFontColor);

            GUILayout.Label("Default State Colors", style);
            defaultStateColor = EditorGUILayout.ColorField("Normal Background", defaultStateColor);
            defaultStateFontColor = EditorGUILayout.ColorField("Normal Font", defaultStateFontColor);
            selectedStateColor = EditorGUILayout.ColorField("Selected Background", selectedStateColor);
            selectedStateFontColor = EditorGUILayout.ColorField("Selected Font", selectedStateFontColor);
            borderAlpha = EditorGUILayout.Slider("Border Color Alpha", borderAlpha, 0, 1f);

            GUILayout.Label("Transition Colors", style);
            transitionDefaultColor = EditorGUILayout.ColorField("Default", transitionDefaultColor);
            transitionTrueColor = EditorGUILayout.ColorField("True", transitionTrueColor);
            transitionFalseColor = EditorGUILayout.ColorField("False", transitionFalseColor);
            transitionMuteColor = EditorGUILayout.ColorField("Mute", transitionMuteColor);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Use Defaults", GUILayout.ExpandWidth(false)))
            {
                DefaultPreferences();
                Save();
            }
            if (GUI.changed) Save();
        }

        static T GetValue<T>(string key)
        {
            if (keyValues != null && keyValues.ContainsKey(key))
            {
                return (T)keyValues[key];
            }
            else if (keyValues == null || !keyValues.ContainsKey(key))
            {
                Load();
                if (keyValues != null && keyValues.ContainsKey(key))
                {
                    return (T)keyValues[key];
                }
            }
            return default(T);
        }

        static void SetValue<T>(string key, T value)
        {
            if (keyValues.ContainsKey(key))
            {
                keyValues[key] = value;
            }
            else if (!keyValues.ContainsKey(key))
            {
                Load();
                if (keyValues.ContainsKey(key))
                {
                    keyValues[key] = value;
                }
            }
        }

        static void Save()
        {
            System.Text.StringBuilder text = new System.Text.StringBuilder();
            var count = 0;
            if (keyValues == null) Load();
            foreach (var key in keyValues.Keys)
            {
                var obj = keyValues[key];
                if (obj.GetType() == typeof(Color)) text.Append(key + "*" + ((Color)obj).ToString());
                else if (obj.GetType() == typeof(float)) text.Append(key + "*" + ((float)obj).ToString());
                if (count < keyValues.Count - 1)
                    text.Append("_");
                count++;
            }
            EditorPrefs.SetString("vFSMPreferences", text.ToString());

        }

        static void Load()
        {
            var stringValue = EditorPrefs.GetString("vFSMPreferences", "");
          
            if (stringValue == "")
            {
                DefaultPreferences();               
            }
            else
                StringToPreferences(stringValue);

        }

        static void DefaultPreferences()
        {          
            keyValues = new Dictionary<string, object>();
            keyValues.Add("gridLinesColor", new Color(Color.white.r * 0.3f, Color.white.g * 0.3f, Color.white.b * 0.3f, 1f));
            keyValues.Add("gridBackgroundColor", new Color(Color.white.r * 0.2f, Color.white.g * 0.2f, Color.white.b * 0.2f, 1f));

            keyValues.Add("transitionFalseColor", new Color(Color.red.r * 0.5f, Color.red.g * 0.5f, Color.red.b * 0.5f, 1f));
            keyValues.Add("transitionTrueColor", new Color(Color.green.r * 0.5f, Color.green.g * 0.5f, Color.green.b * 0.5f, 1f));           
            keyValues.Add("transitionDefaultColor", new Color(.5f, .5f, .5f, 1f));
            keyValues.Add("transitionMuteColor", new Color(.1f, .1f, .1f, 1f));

            keyValues.Add("entryNormalColor", new Color(Color.green.r * 0.4f, Color.green.g * 0.4f, Color.green.b * 0.4f,1f));
            keyValues.Add("anyNormalColor", new Color(Color.cyan.r * 0.4f, Color.cyan.g * 0.4f, Color.cyan.b * 0.4f, 1f));

            keyValues.Add("entrySelectedColor", new Color(Color.green.r * 0.6f, Color.green.g * 0.6f, Color.green.b * 0.6f, 1f));
            keyValues.Add("anySelectedColor", new Color(Color.cyan.r * 0.6f, Color.cyan.g * 0.6f, Color.cyan.b * 0.6f, 1f));

            keyValues.Add("entryNormalFontColor", Color.black);
            keyValues.Add("anyNormalFontColor", Color.black);

            keyValues.Add("entrySelectedFontColor", Color.white);
            keyValues.Add("anySelectedFontColor", Color.white);

            keyValues.Add("defaultStateColor", new Color(.2f, .2f, .2f, 1f));
            keyValues.Add("selectedStateColor", new Color(.3f, .3f, .3f, 1f));

            keyValues.Add("defaultStateFontColor", new Color(.5f, .5f, .5f, 1f));
            keyValues.Add("selectedStateFontColor", new Color(1f, 1f, 1f, 1f));
          
            keyValues.Add("borderAlpha", 1f);

            Save();
        }
        static bool inLoad;
        static void StringToPreferences(string stringValue)
        {
            if (inLoad) return;
            keyValues = new Dictionary<string, object>();
            var prefs = stringValue.Split('_');
            inLoad = true;
            if (prefs.Length == 19)
            {
                for (int i = 0; i < prefs.Length; i++)
                {
                    var values = prefs[i].Split('*');
                    string key = values[0];
                    try
                    {

                        object value = null;
                        if (values[1].Contains("RGBA")) value = StringToColor(values[1]);
                        else value = float.Parse(values[1]);

                        if (keyValues.ContainsKey(key)) keyValues[key] = value;
                        else keyValues.Add(key, value);
                    }
                    catch
                    {

                    }
                }
            }
            else
            {
                DefaultPreferences();
               
            }
            inLoad = false;
        }

        static Color StringToColor(string value)
        {
            value = value.Replace("RGBA(", "");
            value = value.Replace(")", "");

            //Get the individual values (red green blue and alpha)
            var strings = value.Split(","[0]);

            var outputcolor = Color.white;
            for (var i = 0; i < 4; i++)
            {
                outputcolor[i] = System.Single.Parse(strings[i], System.Globalization.NumberFormatInfo.InvariantInfo);
            }
            return outputcolor;
        }
    }
}
#endif