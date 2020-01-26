using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;


public class SafePuzzle: Puzzle{
    [SerializeField] Safe m_safe = null;
    [SerializeField] GameObject[] m_stickers = null;
    [SerializeField] List<PetrolItem> m_items = null;

    public override void Initialize()
    {
        GeneratePassword();
    }

    private void GeneratePassword(){
        string password = "";
        for(int i = 0; i < m_stickers.Length; i++){
            var item = m_items[Random.Range(0, m_items.Count)];
            password += item.value.ToString();
            m_stickers[i].GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", item.stickerTexture);
        }
        m_safe.password = password;
        Debug.Log("Safe Password: " + password);
    }


}