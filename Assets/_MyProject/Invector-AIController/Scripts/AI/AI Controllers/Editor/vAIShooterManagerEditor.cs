using Invector;
using Invector.vCharacterController.AI;
using Invector.vShooter;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(vAIShooterManager), true)]
public class vShooterManagerEditor : vEditorBase
{
    vAIShooterManager manager;
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void AdditionalGUI()
    {
        if (!manager)
            manager = (vAIShooterManager)this.target;

      
        if (toolbars[selectedToolBar].title.Equals("IK Adjust"))
        {
            if (!Application.isPlaying && GUILayout.Button("Create New IK Adjust List"))
            {
                CreateNewIKAdjustList(manager);
            }

            if (manager.weaponIKAdjustList != null && GUILayout.Button("Edit IK Adjust List"))
            {
                vShooterIKAdjustWindow.InitEditorWindow();
            }
        }


      
    }
    public void CreateNewIKAdjustList(vAIShooterManager targetShooterManager)
    {
        vWeaponIKAdjustList ikAdjust = ScriptableObject.CreateInstance<vWeaponIKAdjustList>();
        AssetDatabase.CreateAsset(ikAdjust, "Assets/" + manager.gameObject.name + "@IKAdjustList.asset");
        targetShooterManager.weaponIKAdjustList = ikAdjust;
        AssetDatabase.SaveAssets();

    }
}