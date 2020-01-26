using System;
using UnityEngine;
[Serializable]
[CreateAssetMenu(menuName = "Puzzles/Safe Item")]
public class PetrolItem : ScriptableObject{
    public string itemName;
    public int value;
    public Texture2D stickerTexture;
}