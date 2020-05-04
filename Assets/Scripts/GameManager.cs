using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager: NetworkRoomManager{

    public static LobbyPlayer localLobbyPlayer;
    public static CharacterSelection characterSelection;
    [SerializeField] CharacterSelection m_characterSelection = null;

    private bool gameStarted;

    public Dictionary<byte[], GamePlayer> clientDictionary;

    public override void OnStartServer(){
        base.OnStartServer();
        characterSelection = m_characterSelection;
        clientDictionary = new Dictionary<byte[], GamePlayer>();
        NetworkServer.RegisterHandler<GUIDMessage>(LookupDevice, false);
        gameStarted = false;
    }

    private void LookupDevice(NetworkConnection conn, GUIDMessage msg){
        var guid = new System.Guid(msg.guid);
        GamePlayer player;
        clientDictionary.TryGetValue(msg.guid, out player);
        print(guid.ToString());
    }
    /*TBD aparentemente, o cliente se conecta automaticamente pra cena que o servidor está. Posso aproveitar isso e verificar se a cena atual não é o Lobby e
    e já começar a rotina de reconexão*/
    /*public override void OnClientConnect(NetworkConnection conn){
        OnRoomClientConnect(conn);
        base.CallOnClientEnterRoom();

        

        var msg = new GUIDMessage();
        
        msg.guid = GetDeviceGUID();
        NetworkClient.Send<GUIDMessage>(msg);
    }*/

    public override void OnStartClient(){
        base.OnStartClient();
        InitializeDeviceGUID();
    }

   /* public override void OnServerConnect(NetworkConnection conn){
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }
        string scene = SceneManager.GetActiveScene().name;
        if(scene != RoomScene){

        }

    }*/

    public override void OnRoomServerDisconnect(NetworkConnection conn){
        base.OnRoomServerDisconnect(conn);
        //conn.
    }

    public override void OnRoomServerPlayersReady(){
        base.OnRoomServerPlayersReady();
        ServerChangeScene(GameplayScene);
    }

    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject lobbyPlayer, GameObject gamePlayer)
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
        gameStarted = true;
        return true;
    }

   
    public override void OnServerDisconnect(NetworkConnection conn){
        base.OnServerDisconnect(conn);
    }

    //Generate unique indentifier on the first run of the game
    private void InitializeDeviceGUID(){
        print(Application.persistentDataPath);
        if(!System.IO.File.Exists(Application.persistentDataPath + "/PlayerGUID")){
            var guid = System.Guid.NewGuid();
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/PlayerGUID", guid.ToByteArray());
        }
    }

    private byte[] GetDeviceGUID(){
#if UNITY_STANDALONE
        var guidString = System.Environment.GetCommandLineArgs()[2];
        var guid = new System.Guid(guidString);
        return guid.ToByteArray();
#elif UNITY_ANDROID && !UNITY_EDITOR
        return System.IO.File.ReadAllBytes(Application.persistentDataPath + "/PlayerGUID");
#endif
    }

}