using UnityEditor;
 
[CustomEditor(typeof(CharacterButton))]
public class CharacterButtonEditor : UnityEditor.UI.ButtonEditor
{
    public override void OnInspectorGUI()
    {
        CharacterButton targetCharButton = (CharacterButton)target;
        targetCharButton.character = (Character)EditorGUILayout.EnumPopup("Character", targetCharButton.character);
        EditorUtility.SetDirty(target);

        // Show default inspector property editor
        base.OnInspectorGUI();
    }
}