using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;


public class SafePuzzle: Puzzle{
    [SerializeField] Safe m_safe = null;
    [SerializeField] GameObject[] m_stickers = null;
    [SerializeField] List<PetrolCollection.PetrolItem> m_items = null;
    public SyncListInt chosenItems  = new SyncListInt();
    [SyncVar] private string generatedPassword;

    void Awake(){
    }
    public override void Initialize()
    {
        //Inititaliza Safe open state when the game begins
        GeneratePassword();
    }

    private void GeneratePassword(){
        string password = "";
        for(int i = 0; i < m_stickers.Length; i++){
            var index = Random.Range(0, m_items.Count);
            var item = m_items[index];
            chosenItems.Add(index);
            password += item.value.ToString();
        }
        generatedPassword = password;
    }
    public override void OnStartClient(){
        Debug.Log("Safe Password: " + generatedPassword);
        m_safe.password = generatedPassword;
        m_safe.input = "******";
    }

    public void SetStickers(NetworkIdentity localPlayer){
        var player_char = (int)localPlayer.GetComponent<GamePlayer>().character;
        var player_idx = player_char >> 1;
        var item_idx = chosenItems[player_idx];

        m_stickers[player_idx].GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", m_items[item_idx].stickerTexture);
    }

    public override void OnLocalPlayerReady(NetworkIdentity player)
    {
        SetStickers(player);
    }
}