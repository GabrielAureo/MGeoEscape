// using UnityEngine;
// using UnityEditor;

// public class CharacterData : ScriptableObject{
//     public CharacterInfo geologist;
//     public CharacterInfo detective;
//     public CharacterInfo archeologist;

//     [MenuItem("Project/Create Character data")]
//     public static void CreateCharacterData(){
//         var dict = AssetDatabase.LoadAssetAtPath<CharacterData>("Assets/MGeOEscape/Character Dictionary.asset");
//         if(dict){
//             Debug.LogError("Character Dictionary already exists");
//         }else{
//             var asset = ScriptableObject.CreateInstance<CharacterData>();
//             AssetDatabase.CreateAsset(asset, "Assets/MGeoEscape/Character Dictionary.asset");
//         }
//     }
// }