using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChangingCG))]
public class ChangingCGEditor : Editor
{
    //private SerializedProperty cgs;

    //private void OnEnable()
    //{
    //    cgs = serializedObject.FindProperty("cgs");
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