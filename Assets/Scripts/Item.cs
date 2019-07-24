using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName="Item", menuName="Item")]
public class Item: ScriptableObject{
    public string itemName;
    public GameObject obj;

}