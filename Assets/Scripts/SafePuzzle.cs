using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;


public class SafePuzzle: Puzzle{
    [SerializeField] Safe m_safe = null;
    [SerializeField] GameObject[] m_stickers = null;
    [SerializeField] List<PetrolItem> m_items = null;
    public SyncListInt chosenItems  = new SyncListInt();

    void Awake(){
    }
    public override void Initialize()
    {

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
        m_safe.password = password;
        Debug.Log("Safe Password: " + password);
    }

    public override void OnStartClient(){
        // var conn_idx = NetworkClient.connection.connectionId;
        // var item_idx = chosenItems[conn_idx];
        // Debug.Log(conn_idx);

        // m_stickers[conn_idx].GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", m_items[item_idx].stickerTexture);
        // SetStickers(GameLobbyManager.localGamePlayer.netIdentity);
        Debug.Log("Print this fucking shit");
    }

    public void SetStickers(NetworkIdentity localPlayer){
        print(localPlayer.connectionToServer);
        var conn_idx = localPlayer.connectionToServer.connectionId;
        var item_idx = chosenItems[conn_idx];

        m_stickers[conn_idx].GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", m_items[item_idx].stickerTexture);
    }

    public override void OnLocalPlayerReady(NetworkIdentity player)
    {
        SetStickers(player);
    }
}