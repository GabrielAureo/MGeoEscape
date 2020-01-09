using UnityEditor;
using UnityEngine.UI;
using UnityEngine;

//[CustomEditor(typeof(CharacterButton))]
[CanEditMultipleObjects]
public class CharacterButtonEditor: Editor{
    SerializedProperty m_baseColor;
    SerializedProperty m_selectedColor;
    SerializedProperty m_unselectedColor;
    SerializedProperty m_inactiveColor;
    SerializedProperty m_pressedColor;
    SerializedProperty m_graphic;
    SerializedProperty m_char;
    CharacterButton script;
    void OnEnable(){
        script = target as CharacterButton;
        m_char = serializedObject.FindProperty("character");
        m_graphic = serializedObject.FindProperty("graphic");
        m_baseColor = serializedObject.FindProperty("baseColor");
        m_selectedColor = serializedObject.FindProperty("selectedColor");
        m_unselectedColor = serializedObject.FindProperty("unselectedColor");
        m_inactiveColor = serializedObject.FindProperty("inactiveColor");
        m_pressedColor = serializedObject.FindProperty("pressedColor");
    }

    public override void OnInspectorGUI(){
        EditorGUILayout.PropertyField(m_char, new GUIContent("Character"));
        EditorGUI.indentLevel++;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(m_baseColor, new GUIContent("Base Color"));
        /*if(EditorGUI.EndChangeCheck()){
            var graphic = m_graphic.objectReferenceValue as Graphic;
            if(graphic == null) 
                graphic = script.GetComponent<Graphic>();

            var baseColor = m_baseColor.colorValue;
            graphic.canvasRenderer.SetColor(baseColor);
        }*/
        EditorGUILayout.PropertyField(m_selectedColor, new GUIContent("Selected Color"));
        EditorGUILayout.PropertyField(m_unselectedColor, new GUIContent("Unselected Color"));
        EditorGUILayout.PropertyField(m_inactiveColor, new GUIContent("Unactive Color"));
        EditorGUILayout.PropertyField(m_pressedColor, new GUIContent("Pressed Color"));
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();

    }

    
}