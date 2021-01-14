using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;


public class SafePuzzle: Puzzle{
    [SerializeField] RotarySafe m_safe = null;
    [SerializeField] List<GameObject> m_trueRocksPrefabs = null;
    [SerializeField] List<GameObject> m_fakeRocksPrefabs = null;
    [SerializeField] Vuforia.ImageTargetBehaviour m_target = null;
    public SyncList<float> generatedPassword = new SyncList<float>();
    public SyncList<int> chosenItems = new SyncList<int>();

    void Awake(){
    }
    public override void OnServerInitialize()
    {
        //Inititaliza Safe open state when the game begins
        GeneratePassword();
    }

    private void SetupPuzzleAcrossClients(NetworkIdentity localPlayer){
        if(localPlayer == null) return;
        var player = localPlayer.GetComponent<GamePlayer>();
        if(player != null){
            var player_idx = (int)player.character >> 1;
            if(player.character != Character.Detective){
                if(m_safe.isActiveAndEnabled) GameObject.Destroy(m_safe.gameObject);
            }
            

            CmdSpawnRock(player_idx, localPlayer);
        }
    }
    [Command(ignoreAuthority=true)]
    void CmdSpawnRock(int player_idx, NetworkIdentity playerIdentity){
        var rock = GameObject.Instantiate(m_trueRocksPrefabs[player_idx]);
        var item = GameResources.Instance.petrolCollection.items[chosenItems[player_idx]];
        rock.GetComponent<MeshRenderer>().materials[1].SetTexture("_BaseMap", item.stickerTexture);

        var obj = GameObject.Instantiate(GameResources.Instance.emptySocketPrefab);
        var socket = obj.GetComponent<Socket>();
        //socket.TryPlaceObject(movable);

        NetworkServer.Spawn(obj, playerIdentity.connectionToClient);
    }

    private void GeneratePassword(){
        Debug.Log(GameResources.Instance);
        List<int> items = new List<int>(GameResources.Instance.petrolCollection.items.Keys);
        
        for(int i = items.Count; i > 3; i--){
            var rand = Random.Range(0, items.Count);
            items.RemoveAt(rand);
        }
        foreach(var item in items){
            chosenItems.Add(item);
            generatedPassword.Add(GameResources.Instance.petrolCollection.items[item].value);
        }
    }
    public override void OnStartClient(){
        #if UNITY_ANDROID && !UNITY_EDITOR
        m_safe.transform.parent =  m_target.transform;
        #endif
        m_safe.password = new List<float>(generatedPassword);
        //Reset safe input
        m_safe.ClearInput();
    }

    public void SetStickers(NetworkIdentity localPlayer){
        var player = localPlayer.GetComponent<GamePlayer>();
        if(player != null){
            var player_char = (int) player.character;
            var player_idx = player_char >> 1;
            //var item_idx = chosenItems[player_idx];

            // m_stickers[player_idx].GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", m_items[item_idx].stickerTexture);
        }

    }

    public override void OnLocalPlayerReady(NetworkIdentity player)
    {
        //SetStickers(player);
        //LocalPlayerAnnouncer.RunOnLocalPlayer(SetupPuzzleAcrossClients);
        SetupPuzzleAcrossClients(player);

    }
    
}