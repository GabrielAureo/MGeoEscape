using UnityEngine;
using Mirror;

public class GameLobbyManager: NetworkLobbyManager{

    public static LobbyPlayer localLobbyPlayer;
    public static CharacterSelection characterSelection;
    [SerializeField] CharacterSelection m_characterSelection;

    public override void OnStartServer(){
        characterSelection = m_characterSelection;
    }    

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        PlayerController player = gamePlayer.GetComponent<PlayerController>();
        player.character = lobbyPlayer.GetComponent<LobbyPlayer>().character;
        return true;
    }

    public override void OnLobbyServerPlayersReady()
    {
        base.OnLobbyServerPlayersReady();
    }

}