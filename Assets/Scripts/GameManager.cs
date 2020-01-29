using UnityEngine;
using Mirror;

public class GameManager: NetworkRoomManager{

    public static LobbyPlayer localLobbyPlayer;
    public static CharacterSelection characterSelection;
    [SerializeField] CharacterSelection m_characterSelection = null;

    public override void OnStartServer(){
        characterSelection = m_characterSelection;
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

    public override void OnRoomServerPlayersReady()
    {
        base.OnRoomServerPlayersReady();
    }

}