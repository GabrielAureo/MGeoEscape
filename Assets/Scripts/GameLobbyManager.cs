using UnityEngine;
using Mirror;

public class GameLobbyManager: NetworkLobbyManager{
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        PlayerController player = gamePlayer.GetComponent<PlayerController>();
        print(player);
        player.character = lobbyPlayer.GetComponent<LobbyPlayer>().character;
        print(player.character);
        return true;
    }

    public override void OnLobbyServerPlayersReady()
    {
        base.OnLobbyServerPlayersReady();
    }

}