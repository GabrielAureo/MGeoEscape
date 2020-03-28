using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MovablePlacementPose))]
public class MovablePlacementPoseDrawer: PropertyDrawer{

    private bool m_transf_btn = false;
    private bool foldout;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
        float height = 0;
        var labelRect = GetVerticalRect(position, ref height);
        foldout = EditorGUI.Foldout(labelRect,foldout,"Placement Pose",true);

        if(!foldout) return;

        EditorGUI.BeginProperty(position, label, property);
        

        SerializedProperty transformProperty = property.FindPropertyRelative("position");
        SerializedProperty rotationProperty = property.FindPropertyRelative("rotation");
        SerializedProperty scaleProperty = property.FindPropertyRelative("scale");

        var transformRect = GetVerticalRect(position, ref height);
        var positionRect = GetVerticalRect(position, ref height);
        var rotationRect = GetVerticalRect(position, ref height);
        var scaleRect = GetVerticalRect(position, ref height);
        Transform t = null;
        
        if(!m_transf_btn){
            m_transf_btn = GUI.Button(transformRect,"Record Transform");
        }else{
            t = (Transform)EditorGUI.ObjectField(transformRect, t, typeof(Transform),true);
        }
        if(t != null){
            transformProperty.vector3Value = t.position;
            rotationProperty.quaternionValue = t.rotation;
            scaleProperty.vector3Value = t.localScale;
            m_transf_btn = false;
        }
        

        EditorGUI.PropertyField(positionRect,transformProperty, new GUIContent("Position"));
        EditorGUI.PropertyField(rotationRect, rotationProperty, new GUIContent("Rotation"));
        EditorGUI.PropertyField(scaleRect, scaleProperty, new GUIContent("Scale"));


        EditorGUI.EndProperty();
    }

    private Rect GetVerticalRect(Rect position, ref float height){
        var rect = new Rect(position.x, position.y + height, position.width, EditorGUIUtility.singleLineHeight);
        height += EditorGUIUtility.singleLineHeight;
        return rect;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
        int height = (foldout)?5:1;
        return EditorGUIUtility.singleLineHeight * height;
    }
}