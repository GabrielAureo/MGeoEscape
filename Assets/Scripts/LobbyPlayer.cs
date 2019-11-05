using UnityEngine;
using Mirror;

public class LobbyPlayer: NetworkLobbyPlayer{
    [SyncVar(hook = "RenameGameObject")]
    public string playerName;
    public Character character;


    public override void OnStartLocalPlayer(){
        GameLobbyManager.localLobbyPlayer = this;
    }

    [Command]
    public void CmdChangeName(string name){
        playerName = name;
    }

    void RenameGameObject(string name){
        gameObject.name = "Player " + name;
    }


    [Command]
    public void CmdSelectCharacter(GameObject charSelectObj, int character){
        CharacterSelection characterSelection = charSelectObj.GetComponent<CharacterSelection>();
        if(characterSelection._buttons[character]) return; //jogador tentou selecionar posição já escolhida

        characterSelection._buttons[character] = true;


        var btnID = characterSelection.buttons[character].GetComponent<NetworkIdentity>();
        characterSelection.playerDictionary[(Character)character] = this; 
        btnID.AssignClientAuthority(connectionToClient);
        print(connectionToClient.playerController.name);

        if(characterSelection.playerDictionary[(Character)character] == this){

        }


        RpcFillCharacter(charSelectObj, character);
        btnID.RemoveClientAuthority(connectionToClient);
        foreach(var kvp in characterSelection.playerDictionary){
            Debug.Log(kvp.Key + ", " + kvp.Value);
        }
    }

    [Command]
    public void CmdUnselectCharacter(){
        

    }

    [ClientRpc]
    public void RpcFillCharacter(GameObject charSelectObj, int character){
        CharacterSelection characterSelection = charSelectObj.GetComponent<CharacterSelection>();
        if(!isLocalPlayer) characterSelection.buttons[character].interactable = false;
        
    }

}