using System;
using UnityEngine;
using System.Collections.Generic;
using Mirror;
using RotaryHeart.Lib.SerializableDictionary;

[CreateAssetMenu(menuName ="Puzzles/PetrolCollection")]
public class PetrolCollection : ScriptableObject{
    [Serializable]
    public class PetrolItem{
        public string itemName;
        public int value;
        public Texture2D stickerTexture;

    }
    [Serializable]
    public class PetrolDictionary : SerializableDictionaryBase<int, PetrolItem> {}
    public PetrolDictionary items;
}


