using Invector.IK;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Invector.vShooter
{
    using PlayerController;
    public class vShooterIKAdjustWindow : EditorWindow
    {
        public static vShooterIKAdjustWindow curWindow;
        public vIShooterIKController ikController;
        public SerializedObject ikList;
        public SerializedObject ik;
        public IKSolverEditorHelper leftIK, rightIK;
        public Transform selected, referenceSelected;

        GUISkin skin;
        public bool applicationStarted;
        public Vector2 scroll;
        Rect selectorRect;
        public bool useLockCamera
        {
            get
            {
                return (ikController is vILockCamera);
            }
        }

        public bool LockCamera
        {
            get
            {
                if (useLockCamera)
                    return (ikController as vILockCamera).LockCamera;
                return false;
            }
            set
            {
                if (useLockCamera)
                    (ikController as vILockCamera).LockCamera = value;
            }
        }

        public static void InitEditorWindow()
        {
            if (!curWindow)
            {
                curWindow = (vShooterIKAdjustWindow)EditorWindow.GetWindow<vShooterIKAdjustWindow>("IK Adjust Window");
                curWindow.titleContent.image = Resources.Load("icon_v2") as Texture2D;
            }
        }

        [System.Obsolete]
        protected virtual void OnEnable()
        {
            skin = Resources.Load("welcomeWindowSkin") as GUISkin;
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.duringSceneGui -= this.OnSceneGUI;
            // Add (or re-add) the delegate.
            if (SceneView.onSceneGUIDelegate != this.OnSceneGUI)
            {
                SceneView.duringSceneGui += this.OnSceneGUI;
            }
        }

        void DrawSceneGizmos()
        {
            if (!Application.isPlaying) return;
            if (ikController == null)
            {
                return;
            }

            if (!ikController.CurrentActiveWeapon) return;
            applicationStarted = true;
            if (ikController.WeaponIKAdjustList && ikController.CurrentWeaponIK && ikController.LeftIK != null && ikController.RightIK != null)
            {
                DrawIKHandles(ikController.CurrentWeaponIK);
            }
        }

        void OnGUI()
        {
            GUILayout.BeginVertical(skin.GetStyle("WindowBG"));

            if (!Application.isPlaying)
            {
                DrawMessageInfo("Go to <color=green>PlayMode</color> to edit the IK Adjust List", "PlayModeIcon");
                ikController = null;
                return;
            }
            if (ikController == null)
            {
                DrawMessageInfo("Select a <color=green>Shooter Controller</color>", "ShooterControllerIcon");
                return;
            }

            if (!ikController.CurrentActiveWeapon)
            {
                DrawMessageInfo("Shooter Controller Doesn't have a <color=green>Active Weapon</color>", "WeaponIcon");
                return;
            }

            if (skin == null) skin = Resources.Load("vSkin") as GUISkin;

            scroll = EditorGUILayout.BeginScrollView(scroll);
            {
                {
                    if (ikController.WeaponIKAdjustList)
                    {
                        if (ikList == null || ikList.targetObject != ikController.WeaponIKAdjustList) ikList = new SerializedObject(ikController.WeaponIKAdjustList);
                        if (ikList != null) ikList.UpdateIfRequiredOrScript();


                        var weaponIKAdjustList = ikController.WeaponIKAdjustList;
                        EditorGUI.BeginChangeCheck();
                        weaponIKAdjustList = (vWeaponIKAdjustList)EditorGUILayout.ObjectField(weaponIKAdjustList, typeof(vWeaponIKAdjustList), false);
                        if (EditorGUI.EndChangeCheck())
                        {
                            ikController.WeaponIKAdjustList = weaponIKAdjustList;
                            ikController.SetIKAdjust(weaponIKAdjustList.GetWeaponIK(ikController.CurrentActiveWeapon.weaponCategory));
                            return;
                        }
                        EditorGUI.BeginChangeCheck();
                        string stateTag = ikController.IsCrouching ? "Crouching " : "Standing ";
                        if (ikController.IsAiming) stateTag += "Aiming";
                        GUILayout.Box("State : " + stateTag + " / " + ikController.CurrentActiveWeapon.weaponCategory + " Category", skin.box, GUILayout.ExpandWidth(true));

                        if (GUILayout.Button(ikController.LockAiming ? "Unlock Aim" : "Lock Aim", EditorStyles.miniButton))
                        {
                            ikController.LockAiming = !ikController.LockAiming;
                        }

                        if (GUILayout.Button(ikController.IsCrouching ? "Unlock Crouch" : "Lock Crouch", EditorStyles.miniButton))
                        {
                            ikController.IsCrouching = !ikController.IsCrouching;
                        }

                        if (useLockCamera && GUILayout.Button(LockCamera ? "Unlock Camera" : "Lock Camera", EditorStyles.miniButton))
                        {
                            LockCamera = !LockCamera;
                        }

                        if (ikController.CurrentWeaponIK)
                        {
                            if (ik == null || ik.targetObject != ikController.CurrentWeaponIK) ik = new SerializedObject(ikController.CurrentWeaponIK);
                            ik.Update();
                            try
                            {
                                GUILayout.Space(20);
                                //GUILayout.BeginVertical(); 
                                DrawWeaponIKSettings(ikController.CurrentWeaponIK);
                                GUILayout.Space(20);
                                DrawLeftIKOffsets();
                                GUILayout.Space(20);
                                //DrawGizmosAlert();
                                //GUILayout.EndVertical();
                            }
                            catch { }
                        }
                        else
                        {
                            EditorStyles.helpBox.richText = true;
                            EditorGUILayout.HelpBox("This weapon doesn't have a IKAdjust for the '" + ikController.CurrentActiveWeapon.weaponCategory + "' category,  click in the button below to create one.", MessageType.Info);
                            if (GUILayout.Button("Create New IK Adjust", skin.button))
                            {
                                vWeaponIKAdjust ikAdjust = ScriptableObject.CreateInstance<vWeaponIKAdjust>();
                                AssetDatabase.CreateAsset(ikAdjust, "Assets/" + ikController.gameObject.name + "@" + ikController.CurrentActiveWeapon.weaponCategory + ".asset");
                                ikAdjust.weaponCategories = new List<string>() { ikController.CurrentActiveWeapon.weaponCategory };

                                ikController.WeaponIKAdjustList.weaponIKAdjusts.Add(ikAdjust);
                                ikController.SetIKAdjust(ikAdjust);

                                SerializedObject scriptableIK = new SerializedObject(ikAdjust);
                                scriptableIK.ApplyModifiedProperties();
                                ikList.ApplyModifiedProperties();
                                EditorUtility.SetDirty(ikList.targetObject);
                                AssetDatabase.SaveAssets();
                                EditorUtility.SetDirty(scriptableIK.targetObject);
                            }
                            if (GUILayout.Button("Choose IK Adjust", skin.button))
                            {
                                var action = new UnityEngine.Events.UnityAction<vWeaponIKAdjust>((ikAdjust) =>
                                 {
                                     ikController.WeaponIKAdjustList.weaponIKAdjusts.Add(ikAdjust);
                                     ikController.SetIKAdjust(ikAdjust);

                                     ikList.ApplyModifiedProperties();
                                     EditorUtility.SetDirty(ikList.targetObject);
                                 });

                                PopupWindow.Show(selectorRect, new vIKAjustSelector(ikController.CurrentActiveWeapon.weaponCategory, action, skin));
                            }
                            if (Event.current.type == EventType.Repaint) selectorRect = GUILayoutUtility.GetLastRect();
                            ikController.LoadIKAdjust(ikController.CurrentActiveWeapon.weaponCategory);
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (ikList != null)
                            {
                                ikList.ApplyModifiedProperties();
                                EditorUtility.SetDirty(ikList.targetObject);
                            }
                        }
                    }
                    else
                    {
                        GUILayout.FlexibleSpace();
                        GUILayout.Label("You need to assign a <color=green>IK Adjust List</color> in the ShooterManager First!", skin.GetStyle("SuperTitle"));
                    }
                }
                GUILayout.Space(20);
            }

            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace(); GUILayout.EndVertical();
        }

        private void DrawMessageInfo(string message, string icon)
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label(message, skin.GetStyle("SuperTitle"));
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ", skin.GetStyle(icon));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private void DrawGizmosAlert()
        {
            GUILayout.Label("Make sure to <color=green>Enable the Gizmos</color> button of your SceneView Window", skin.GetStyle("SuperTitle"));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(" ", skin.GetStyle("GizmosIcon"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }

        public void CreateNewIKAdjustList(vIShooterIKController targetShooterManager)
        {
            vWeaponIKAdjustList ikAdjust = ScriptableObject.CreateInstance<vWeaponIKAdjustList>();
            AssetDatabase.CreateAsset(ikAdjust, "Assets/" + ikController.gameObject.name + "@IKAdjustList.asset");

            targetShooterManager.WeaponIKAdjustList = ikAdjust;
            ikList = new SerializedObject(ikAdjust);
            ikList.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(ikList.targetObject);
        }

        void DrawIKHandles(vWeaponIKAdjust currentWeaponIK)
        {
            if (leftIK == null || leftIK.iKSolver == null || !leftIK.iKSolver.isValidBones) leftIK = new IKSolverEditorHelper(ikController.LeftIK);
            if (rightIK == null || rightIK.iKSolver == null || !rightIK.iKSolver.isValidBones) rightIK = new IKSolverEditorHelper(ikController.RightIK);
            if (leftIK == null || rightIK == null) return;
            leftIK.DrawIKHandles(ref selected, ref referenceSelected, Color.blue);
            rightIK.DrawIKHandles(ref selected, ref referenceSelected, Color.green);

            if (selected != null)
            {
                if (DrawTransformHandles(selected, referenceSelected))
                {
                    Undo.RecordObject(currentWeaponIK, "Change IK Bone Transform");
                    ApplyOffsets((ikController.IsAiming ? (ikController.IsCrouching ? (ikController.CurrentWeaponIK.crouchingAiming) : ikController.CurrentWeaponIK.standingAiming) :
                                                   (ikController.IsCrouching ? ikController.CurrentWeaponIK.crouching : ikController.CurrentWeaponIK.standing)),
                                                   ikController.IsLeftWeapon ? ikController.LeftIK : ikController.RightIK,
                                                   ikController.IsLeftWeapon ? ikController.RightIK : ikController.LeftIK);
                }
            }
        }

        void ApplyOffsets(IKAdjust currentIKAdjust, vIKSolver weaponArm, vIKSolver supportWeaponArm)
        {
            currentIKAdjust.supportHandOffset.position = supportWeaponArm.endBoneOffset.localPosition;
            currentIKAdjust.supportHandOffset.eulerAngles = supportWeaponArm.endBoneOffset.localEulerAngles;
            currentIKAdjust.supportHintOffset.position = supportWeaponArm.middleBoneOffset.localPosition;
            currentIKAdjust.supportHintOffset.eulerAngles = supportWeaponArm.middleBoneOffset.localEulerAngles;

            currentIKAdjust.weaponHandOffset.position = weaponArm.endBoneOffset.localPosition;
            currentIKAdjust.weaponHandOffset.eulerAngles = weaponArm.endBoneOffset.localEulerAngles;
            currentIKAdjust.weaponHintOffset.position = weaponArm.middleBoneOffset.localPosition;
            currentIKAdjust.weaponHintOffset.eulerAngles = weaponArm.middleBoneOffset.localEulerAngles;
            ik.ApplyModifiedProperties();
            EditorUtility.SetDirty(ik.targetObject);
        }

        void DrawWeaponIKSettings(vWeaponIKAdjust currentWeaponIK)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(" ", skin.GetStyle("PopUpArrow"));

            if (GUILayout.Button(currentWeaponIK.name, skin.GetStyle("PopUp")))
            {
                var index = ikController.WeaponIKAdjustList.IndexOfIK(currentWeaponIK);
                var action = new UnityEngine.Events.UnityAction<vWeaponIKAdjust>((ikAdjust) =>
                {
                    ikController.WeaponIKAdjustList.weaponIKAdjusts[index] = (ikAdjust);
                    ikController.SetIKAdjust(ikAdjust);

                    ikList.ApplyModifiedProperties();
                    EditorUtility.SetDirty(ikList.targetObject);
                });

                PopupWindow.Show(selectorRect, new vIKAjustSelector(ikController.CurrentActiveWeapon.weaponCategory, action, skin, currentWeaponIK));
            }

            if (Event.current.type == EventType.Repaint) selectorRect = GUILayoutUtility.GetLastRect();
            if (GUILayout.Button(new GUIContent("Create Copy", "Create and Apply a copy of this IK Adjust to the current Weapon category"), skin.button))
            {
                if (EditorUtility.DisplayDialog("Create Copy", "Create a copy of this IK Adjust? ", "Confirm"))
                {
                    string assetPath = AssetDatabase.GetAssetPath(currentWeaponIK);

                    var newAssetPath = assetPath.Replace(".asset", "_Copy.asset");
                    if (AssetDatabase.CopyAsset(assetPath, newAssetPath))
                    {
                        vWeaponIKAdjust newIK = (vWeaponIKAdjust)AssetDatabase.LoadAssetAtPath(newAssetPath, typeof(vWeaponIKAdjust));
                        ikController.WeaponIKAdjustList.ReplaceWeaponIKAdjust(currentWeaponIK, newIK);
                        ikController.SetIKAdjust(newIK);
                        ProjectWindowUtil.ShowCreatedAsset(newIK);

                        return;
                    }
                }
            }

            GUILayout.EndHorizontal();
            if (ikController.LeftIK == null || ikController.RightIK == null || !ikController.LeftIK.isValidBones || !ikController.RightIK.isValidBones) return;
            GUILayout.BeginHorizontal();

            GUI.enabled = selected != ikController.LeftIK.endBoneOffset;
            if (GUILayout.Button("Left Hand", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(true)))
            {
                referenceSelected = ikController.LeftIK.endBone;
                selected = ikController.LeftIK.endBoneOffset;
            }
            GUI.enabled = selected != ikController.LeftIK.middleBoneOffset;
            if (GUILayout.Button("Left Hint", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(true)))
            {
                referenceSelected = ikController.LeftIK.middleBone;
                selected = ikController.LeftIK.middleBoneOffset;

            }
            GUILayout.Space(20);
            GUI.enabled = selected != ikController.RightIK.endBoneOffset;
            if (GUILayout.Button("Right Hand", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(true)))
            {
                referenceSelected = ikController.RightIK.endBone;
                selected = ikController.RightIK.endBoneOffset;

            }
            GUI.enabled = selected != ikController.RightIK.middleBoneOffset;
            if (GUILayout.Button("Right Hint", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(true)))
            {
                referenceSelected = ikController.RightIK.middleBone;
                selected = ikController.RightIK.middleBoneOffset;

            }
            GUI.enabled = true;


            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (selected != null)
            {
                GUILayout.Label(selected.name, EditorStyles.whiteBoldLabel);
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Reset Position", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(true)))
                {
                    Undo.RecordObject(selected, "Reset Position");
                    selected.localPosition = Vector3.zero;
                    ApplyOffsets((ikController.IsAiming ? (ikController.IsCrouching ? (ikController.CurrentWeaponIK.crouchingAiming) : ikController.CurrentWeaponIK.standingAiming) :
                                              (ikController.IsCrouching ? ikController.CurrentWeaponIK.crouching : ikController.CurrentWeaponIK.standing)),
                                              ikController.IsLeftWeapon ? ikController.LeftIK : ikController.RightIK,
                                              ikController.IsLeftWeapon ? ikController.RightIK : ikController.LeftIK);
                }
                if (GUILayout.Button("Reset", EditorStyles.miniButtonMid, GUILayout.ExpandWidth(true)))
                {
                    Undo.RecordObject(selected, "Reset ALL");
                    selected.localPosition = Vector3.zero;
                    selected.localEulerAngles = Vector3.zero;
                    ApplyOffsets((ikController.IsAiming ? (ikController.IsCrouching ? (ikController.CurrentWeaponIK.crouchingAiming) : ikController.CurrentWeaponIK.standingAiming) :
                                              (ikController.IsCrouching ? ikController.CurrentWeaponIK.crouching : ikController.CurrentWeaponIK.standing)),
                                              ikController.IsLeftWeapon ? ikController.LeftIK : ikController.RightIK,
                                              ikController.IsLeftWeapon ? ikController.RightIK : ikController.LeftIK);
                }
                if (GUILayout.Button("Reset Rotation", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(true)))
                {
                    Undo.RecordObject(selected, "Reset Rotation");
                    selected.localEulerAngles = Vector3.zero;
                    ApplyOffsets((ikController.IsAiming ? (ikController.IsCrouching ? (ikController.CurrentWeaponIK.crouchingAiming) : ikController.CurrentWeaponIK.standingAiming) :
                                              (ikController.IsCrouching ? ikController.CurrentWeaponIK.crouching : ikController.CurrentWeaponIK.standing)),
                                              ikController.IsLeftWeapon ? ikController.LeftIK : ikController.RightIK,
                                              ikController.IsLeftWeapon ? ikController.RightIK : ikController.LeftIK);
                }
                GUILayout.EndHorizontal();
            }
            if (ikController.IsAiming)
            {
                DrawHeadTrackSliders(ikController.IsCrouching ? currentWeaponIK.crouchingAiming.spineOffset : currentWeaponIK.standingAiming.spineOffset);
            }
            else
            {
                DrawHeadTrackSliders(ikController.IsCrouching ? currentWeaponIK.crouching.spineOffset : currentWeaponIK.standing.spineOffset);
            }

        }

        void DrawHeadTrackSliders(IKOffsetSpine offsetSpine)
        {
            Vector2 _spine = offsetSpine.spine;
            Vector2 _head = offsetSpine.head;
            EditorGUI.BeginChangeCheck();
            {
                GUILayout.BeginVertical(skin.box);
                {
                    GUILayout.BeginHorizontal(skin.box);
                    GUILayout.Label("- <b>Spine and Head Offsets</b> are applied for <color=green>each weapon and state</color>", skin.GetStyle("Description"));
                    GUILayout.EndHorizontal();

                    GUILayout.Label("Spine Offset", EditorStyles.whiteBoldLabel);

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _spine.x, "X");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _spine.y, "Y");
                    GUILayout.EndHorizontal();

                    GUILayout.Label("Head Offset", EditorStyles.whiteBoldLabel);

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _head.x, "X");
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _head.y, "Y");
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(ikController.CurrentWeaponIK, "Change Offset Spine");
                offsetSpine.spine = _spine;
                offsetSpine.head = _head;

                ik.ApplyModifiedProperties();
                EditorUtility.SetDirty(ik.targetObject);
            }
        }

        void DrawLeftIKOffsets()
        {
            if (!ikController.WeaponIKAdjustList) return;

            Vector3 _ikPosL = ikController.WeaponIKAdjustList.ikPositionOffsetL;
            Vector3 _ikPosR = ikController.WeaponIKAdjustList.ikPositionOffsetR;
            Vector3 _ikRotL = ikController.WeaponIKAdjustList.ikRotationOffsetL;
            Vector3 _ikRotR = ikController.WeaponIKAdjustList.ikRotationOffsetR;

            EditorGUI.BeginChangeCheck();
            {
                GUILayout.BeginVertical(skin.box);

                GUILayout.BeginHorizontal(skin.box);
                GUILayout.Label("- Start by aligning the <b>Left Hand IK </b>, this will have affect on <color=green>all weapons and states</color>", skin.GetStyle("Description"));
                GUILayout.EndHorizontal();

                if (!ikController.CurrentActiveWeapon.isLeftWeapon)
                {
                    GUILayout.Label("Left IK Position Offset", EditorStyles.whiteBoldLabel);

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosL.x, "X", -1, 1);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosL.y, "Y", -1, 1);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosL.z, "Z", -1, 1);
                    GUILayout.EndHorizontal();
                }

                if (ikController.CurrentActiveWeapon.isLeftWeapon)
                {
                    GUILayout.Label("Right IK Position Offset", EditorStyles.whiteBoldLabel);

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosR.x, "X", -1, 1);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosR.y, "Y", -1, 1);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikPosR.z, "Z", -1, 1);
                    GUILayout.EndHorizontal();
                }

                if (!ikController.CurrentActiveWeapon.isLeftWeapon)
                {
                    GUILayout.Label("Left IK Rotation Offset", EditorStyles.whiteBoldLabel);

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotL.x, "X");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotL.y, "Y");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotL.z, "Z");
                    GUILayout.EndHorizontal();
                }

                if (ikController.CurrentActiveWeapon.isLeftWeapon)
                {
                    GUILayout.Label("Right IK Rotation Offset", EditorStyles.whiteBoldLabel);

                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotR.x, "X");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotR.y, "Y");
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    DrawSlider(ref _ikRotR.z, "Z");
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(ikController.WeaponIKAdjustList, "IkOffsets");

                ikController.WeaponIKAdjustList.ikPositionOffsetL.x = _ikPosL.x;
                ikController.WeaponIKAdjustList.ikPositionOffsetL.y = _ikPosL.y;
                ikController.WeaponIKAdjustList.ikPositionOffsetL.z = _ikPosL.z;
                ikController.WeaponIKAdjustList.ikPositionOffsetR.x = _ikPosR.x;
                ikController.WeaponIKAdjustList.ikPositionOffsetR.y = _ikPosR.y;
                ikController.WeaponIKAdjustList.ikPositionOffsetR.z = _ikPosR.z;
                ikController.WeaponIKAdjustList.ikRotationOffsetL.x = _ikRotL.x;
                ikController.WeaponIKAdjustList.ikRotationOffsetL.y = _ikRotL.y;
                ikController.WeaponIKAdjustList.ikRotationOffsetL.z = _ikRotL.z;
                ikController.WeaponIKAdjustList.ikRotationOffsetR.x = _ikRotR.x;
                ikController.WeaponIKAdjustList.ikRotationOffsetR.y = _ikRotR.y;
                ikController.WeaponIKAdjustList.ikRotationOffsetR.z = _ikRotR.z;

                ikList.ApplyModifiedProperties();
                EditorUtility.SetDirty(ikList.targetObject);
            }
        }

        void DrawSlider(ref float value, string name, float min = -180, float max = 180)
        {
            GUILayout.Label(name);
            value = EditorGUILayout.Slider(value, min, max);
        }

        bool DrawTransformHandles(Transform target, Transform reference)
        {
            if (!target) return false;
            Vector3 position = target.position;
            Quaternion rotation = target.rotation;
            Handles.DrawLine(target.position, reference.position);
            if (Tools.current != Tool.Rotate)
            {
                position = Handles.PositionHandle(position, Tools.pivotRotation == PivotRotation.Local ? rotation : Quaternion.identity);
            }

            if (Tools.current == Tool.Rotate)
                rotation = Handles.RotationHandle(rotation, position);
            if (position != target.position)
            {
                Undo.RecordObject(target, "Change IK Bone Transform");
                target.position = position;

                return true;
            }
            else if (rotation != target.rotation)
            {
                Undo.RecordObject(target, "Change IK Bone Transform");
                target.rotation = rotation;
                return true;
            }
            return false;
        }

        public void OnSceneGUI()
        {
            DrawSceneGizmos();
        }

        void Update()
        {
            this.minSize = new Vector2(300, 300);
            if (EditorApplication.isPlaying && !EditorApplication.isPaused)
            {
                if (curWindow == null) curWindow = this;

                if (Selection.activeTransform && (ikController == null || (Selection.activeGameObject != ikController.gameObject && Selection.activeTransform.GetComponent<vIShooterIKController>() != null)))
                {
                    ikController = Selection.activeGameObject.GetComponent<vIShooterIKController>();
                }
                Repaint();
            }


        }

        [System.Obsolete]
        void OnFocus()
        {
            // Remove delegate listener if it has previously
            // been assigned.
            SceneView.duringSceneGui -= this.OnSceneGUI;
            // Add (or re-add) the delegate.
            if (SceneView.onSceneGUIDelegate != this.OnSceneGUI)
            {
                SceneView.duringSceneGui += this.OnSceneGUI;
            }
        }

        void OnDestroy()
        {
            // When the window is destroyed, remove the delegate
            // so that it will no longer do any drawing.
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        void OnSceneGUI(SceneView sceneView)
        {
            DrawSceneGizmos();
        }

        [System.Serializable]
        public class IKSolverEditorHelper
        {
            public vIKSolver iKSolver;
            public IKSolverEditorHelper(vIKSolver iKSolver)
            {
                this.iKSolver = iKSolver;
            }
            public void DrawIKHandles(ref Transform selected, ref Transform referenceSelected, Color color)
            {
                if (iKSolver == null || !iKSolver.isValidBones) return;
                DrawArmLine(color);
                if (selected != iKSolver.endBoneOffset)
                    if (DrawTransformButton(iKSolver.endBone, Handles.SphereHandleCap))
                    {
                        referenceSelected = iKSolver.endBone;
                        selected = iKSolver.endBoneOffset;
                        Handles.color = Color.white;
                    }
                if (selected != iKSolver.middleBoneOffset)
                    if (DrawTransformButton(iKSolver.middleBone, Handles.CubeHandleCap))
                    {
                        referenceSelected = iKSolver.middleBone;
                        selected = iKSolver.middleBoneOffset;
                        Handles.color = Color.white;
                    }

                Handles.color = Color.white;
            }
            public bool DrawTransformButton(Transform target, Handles.CapFunction cap)
            {

                if (!target) return false;
                if (Handles.Button(target.position, target.rotation, 0.02f, 0.02f, cap))
                {
                    return true;
                }
                return false;
            }
            void DrawArmLine(Color color)
            {
                Handles.color = color;
                if (iKSolver == null || !iKSolver.isValidBones) return;

                Handles.DrawAAPolyLine(iKSolver.endBone.position, iKSolver.middleBone.position, iKSolver.rootBone.position);
            }
        }
    }

    public class vIKAjustSelector : PopupWindowContent
    {
        Vector2 scroll;
        public string weaponCategory;
        public List<vWeaponIKAdjust> weaponIKAdjusts;
        vWeaponIKAdjust selected;
        UnityEngine.Events.UnityAction<vWeaponIKAdjust> onSelect;
        GUISkin skin;
        GUIStyle selectorStyle;
        bool canSelectNull;
        public vIKAjustSelector(string weaponCategory, UnityEngine.Events.UnityAction<vWeaponIKAdjust> onSelect, GUISkin skin, vWeaponIKAdjust selected = null)
        {
            this.weaponCategory = weaponCategory;
            this.onSelect = onSelect;
            this.skin = skin;
            selectorStyle = new GUIStyle(skin.button);
            selectorStyle.border = new RectOffset(1, 1, 1, 1);
            selectorStyle.margin = new RectOffset(0, 0, 0, 0);
            selectorStyle.padding = new RectOffset(0, 0, 0, 0);
            selectorStyle.overflow = new RectOffset(0, 0, 0, 0);
            selectorStyle.alignment = TextAnchor.UpperCenter;
            selectorStyle.fontSize = 12;
            selectorStyle.clipping = TextClipping.Clip;
            selectorStyle.wordWrap = true;
            if (selected != null)
            {
                this.selected = selected;
                canSelectNull = false;
            }
            else canSelectNull = true;
        }
        public override Vector2 GetWindowSize()
        {
            Vector2 windowSize = base.GetWindowSize();
            windowSize.y = 32 + (Mathf.Clamp((weaponIKAdjusts.Count + (canSelectNull ? 1 : 0)) * 25, 57, 300));
            return windowSize;
        }
        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);
            GUILayout.Box(weaponCategory.ToUpper() + " IK Adjust Selector", skin.GetStyle("WindowBG"), GUILayout.Width(rect.width), GUILayout.Height(30));
            scroll = GUILayout.BeginScrollView(scroll, false, false, GUILayout.Width(rect.width));
            GUI.color = selected == null ? Color.green : Color.white;
            if (canSelectNull && GUILayout.Button("None", selectorStyle, GUILayout.Height(25), GUILayout.Width(rect.width)))
            {
                selected = null;
                editorWindow.Close();
            }
            for (int i = 0; i < weaponIKAdjusts.Count; i++)
            {
                GUI.color = selected == weaponIKAdjusts[i] ? Color.green : Color.white;
                if (GUILayout.Button(weaponIKAdjusts[i].name, selectorStyle, GUILayout.Height(25), GUILayout.Width(rect.width)))
                {
                    selected = weaponIKAdjusts[i];
                    EditorGUIUtility.PingObject(weaponIKAdjusts[i]);
                    editorWindow.Close();
                }
            }
            GUI.color = Color.white;
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        public override void OnOpen()
        {
            weaponIKAdjusts = FindAssetsByType<vWeaponIKAdjust>();
            if (weaponIKAdjusts.Count > 0)
            {
                weaponIKAdjusts = weaponIKAdjusts.FindAll(w => w.weaponCategories.Contains(weaponCategory));
            }
        }
        public override void OnClose()
        {
            if (selected && onSelect != null) onSelect(selected);
        }
        public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }
    }
}