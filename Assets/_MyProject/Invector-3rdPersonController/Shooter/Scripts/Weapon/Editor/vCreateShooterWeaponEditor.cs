using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
namespace Invector.vShooter
{
    public class vCreateShooterWeaponEditor : EditorWindow
    {
        GUISkin skin;
        GameObject weaponObj;
        Vector2 rect = new Vector2(400, 100);
        Texture2D m_Logo;

        [MenuItem("Invector/Shooter/Create Shooter Weapon")]
        public static void CreateNewCharacter()
        {
            GetWindow<vCreateShooterWeaponEditor>();
        }

        void OnGUI()
        {
            if (!skin) skin = Resources.Load("vSkin") as GUISkin;
            GUI.skin = skin;

            this.minSize = rect;
            this.titleContent = new GUIContent("ShooterWeapon", null, "Window Creator");
            m_Logo = Resources.Load("icon_v2") as Texture2D;
            GUILayout.BeginVertical("Shooter Weapon Creator Window", "window");
            GUILayout.Label(m_Logo, GUILayout.MaxHeight(25));
            GUILayout.Space(5);

            GUILayout.BeginVertical("box");
            weaponObj = EditorGUILayout.ObjectField("FBX Model", weaponObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;

            if (weaponObj != null)
            {
                if (GUILayout.Button("Create"))
                    Create();
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private void Create()
        {
            var template = Resources.Load("ShooterWeaponTemplate") as GameObject;
            GameObject weapon;

            if (template)
                weapon = Instantiate(template);
            else
                weapon = new GameObject(" ", typeof(vShooterWeapon));

            var _weaponObj = Instantiate(weaponObj);
            _weaponObj.transform.SetParent(weapon.transform);
            _weaponObj.transform.localPosition = Vector3.zero;
            _weaponObj.transform.localEulerAngles = Vector3.zero;
            Selection.activeGameObject = weapon;
            SceneView.lastActiveSceneView.FrameSelected();

            this.Close();
        }
    }
}

