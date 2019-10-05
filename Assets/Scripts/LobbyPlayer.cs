using UnityEngine;
using Mirror;

public class LobbyPlayer: NetworkLobbyPlayer{
    public string m_Name;
    public Character character;

    public override void OnStartLocalPlayer(){
        GameLobbyManager.localLobbyPlayer = this;
    }

    [Command]
    public void CmdSelectCharacter(GameObject charSelectObj, int character){
        CharacterSelection characterSelection = charSelectObj.GetComponent<CharacterSelection>();
        if(characterSelection._buttons[character]) return;
        print(characterSelection._buttons.Count);
        characterSelection._buttons[character] = true;
        var btnID = characterSelection.buttons[character].GetComponent<NetworkIdentity>();
        btnID.AssignClientAuthority(connectionToClient);
        characterSelection.RpcFillCharacter(character);
        btnID.RemoveClientAuthority(connectionToClient);
    }

    [ClientRpc]
    public void RpcFillCharacter(GameObject charSelectObj, int character){
        CharacterSelection characterSelection = charSelectObj.GetComponent<CharacterSelection>();
        characterSelection.buttons[character].interactable = false;
    }

}