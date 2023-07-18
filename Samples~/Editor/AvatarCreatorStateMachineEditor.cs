using System;
using ReadyPlayerMe.Core;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    [CustomEditor(typeof(AvatarCreatorStateMachine)), CanEditMultipleObjects]
    public class AvatarCreatorStateMachineEditor : Editor
    {
        private AvatarCreatorStateMachine avatarCreatorStateMachine;
        private string[] genderOptions;
        private string[] bodyTypeOptions;

        public override void OnInspectorGUI()
        {
            avatarCreatorStateMachine = (AvatarCreatorStateMachine) target;
            DrawDefaultInspector();
            if (avatarCreatorStateMachine.avatarCreatorData != null)
            {
                DrawDefaultBodyTypeField();
                DrawDefaultGenderField();
            }
        }

        private void DrawDefaultGenderField()
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel(new GUIContent("Default Gender"));

                EditorGUI.BeginChangeCheck();
                var choiceIndex = EditorGUILayout.Popup((int) avatarCreatorStateMachine.avatarCreatorData.AvatarProperties.Gender, genderOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    avatarCreatorStateMachine.avatarCreatorData.AvatarProperties.Gender =
                        (OutfitGender) Enum.Parse(typeof(OutfitGender), genderOptions[choiceIndex]);
                    EditorUtility.SetDirty(avatarCreatorStateMachine.avatarCreatorData);
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawDefaultBodyTypeField()
        {
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel(new GUIContent("Default BodyType"));

                EditorGUI.BeginChangeCheck();
                var choiceIndex = EditorGUILayout.Popup((int) avatarCreatorStateMachine.avatarCreatorData.AvatarProperties.BodyType, bodyTypeOptions);
                if (EditorGUI.EndChangeCheck())
                {
                    avatarCreatorStateMachine.avatarCreatorData.AvatarProperties.BodyType =
                        (BodyType) Enum.Parse(typeof(BodyType), bodyTypeOptions[choiceIndex]);
                    EditorUtility.SetDirty(avatarCreatorStateMachine.avatarCreatorData);
                }
            }
            GUILayout.EndHorizontal();
        }


        private void OnEnable()
        {
            genderOptions = Enum.GetNames(typeof(OutfitGender));
            bodyTypeOptions = Enum.GetNames(typeof(BodyType));
        }

    }
}
