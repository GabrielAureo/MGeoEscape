using UnityEngine;
using Mirror;

public class DebugNetworkManager : NetworkManager{
    public Character character;

    public override void Awake(){
        
        if(!NetworkServer.active){
            base.Awake();
            StartHost();
        }
            
    }

    public override void OnServerAddPlayer(NetworkConnection conn){
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        player.GetComponent<GamePlayer>().character = character;
        NetworkServer.AddPlayerForConnection(conn, player);
    }

}