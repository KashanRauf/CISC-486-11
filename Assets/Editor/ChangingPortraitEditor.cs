using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChangingPortrait))]
public class ChangingPortraitEditor : Editor
{
    //private SerializedProperty portraits;

    //private void OnEnable()
    //{
    //    portraits = serializedObject.FindProperty("portraits");
    //}

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();

    //    // Draw the normal Image inspector.
    //    DrawPropertiesExcluding(
    //        serializedObject,
    //        "m_Script"
    //    );

    //    //EditorGUILayout.Space(10);

    //    //EditorGUILayout.LabelField("Portraits", EditorStyles.boldLabel);

    //    //EditorGUILayout.PropertyField(
    //    //    portraits,
    //    //    new GUIContent("Portraits"),
    //    //    true
    //    //);

    //    serializedObject.ApplyModifiedProperties();
    //}
}