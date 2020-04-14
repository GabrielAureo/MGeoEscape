using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName ="Puzzles/PetrolCollection")]
public class PetrolCollection : ScriptableObject{
    [Serializable]
    public class PetrolItem{
        public string itemName;
        public int value;
        public Texture2D stickerTexture;
    }
    public List<PetrolItem> items;
}
