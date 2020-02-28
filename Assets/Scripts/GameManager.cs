using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager: NetworkRoomManager{

    public static LobbyPlayer localLobbyPlayer;
    public static CharacterSelection characterSelection;
    [SerializeField] CharacterSelection m_characterSelection = null;

    public Dictionary<string, GamePlayer> clientDictionary;


    public override void OnStartServer(){
        characterSelection = m_characterSelection;
    }

    public override void OnServerConnect(NetworkConnection conn){
         if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
        string scene = SceneManager.GetActiveScene().name;
        if(scene != RoomScene){

        }      
    }

    public override void OnRoomServerDisconnect(NetworkConnection conn){
        base.OnRoomServerDisconnect(conn);
        //conn.
    }

    public override void OnRoomServerPlayersReady(){
        base.OnRoomServerPlayersReady();
        ServerChangeScene(GameplayScene);
    }


    public override bool OnRoomServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        GamePlayer player = gamePlayer.GetComponent<GamePlayer>();
        LobbyPlayer _lobbyPlayer = lobbyPlayer.GetComponent<LobbyPlayer>();
        // var character = lobbyPlayer.GetComponent<LobbyPlayer>().cur_character;
        // if(character != null){
        //     player.character = character.GetValueOrDefault();
        // }   
        player.playerName = _lobbyPlayer.playerName;
        foreach(var kvp in characterSelection.playerDictionary){
            if(kvp.Value?.GetComponent<LobbyPlayer>() == _lobbyPlayer){
                player.character = (Character)kvp.Key;
                break;
            }
        }
        return true;
    }

   
    public override void OnServerDisconnect(NetworkConnection conn){
        base.OnServerDisconnect(conn);
    }

}