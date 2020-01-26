using UnityEngine;
using Mirror;

public class GameLobbyManager: NetworkRoomManager{

    public static LobbyPlayer localLobbyPlayer;
    public static CharacterSelection characterSelection;
    [SerializeField] CharacterSelection m_characterSelection;

    public override void OnStartServer(){
        characterSelection = m_characterSelection;
    }    

    public override bool OnRoomServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        OnlinePlayer player = gamePlayer.GetComponent<OnlinePlayer>();
        LobbyPlayer _lobbyPlayer = lobbyPlayer.GetComponent<LobbyPlayer>();
        // var character = lobbyPlayer.GetComponent<LobbyPlayer>().cur_character;
        // if(character != null){
        //     player.character = character.GetValueOrDefault();
        // }   

        foreach(var kvp in characterSelection.playerDictionary){
            if(kvp.Value == _lobbyPlayer){
                player.character = kvp.Key;
                break;
            }
        }
        return true;
    }

    public override void OnRoomServerPlayersReady()
    {
        base.OnRoomServerPlayersReady();
    }

}