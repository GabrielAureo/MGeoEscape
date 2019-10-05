using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;

public class CharacterSelection : NetworkBehaviour{
    Character chosenCharacter;
    public List<bool> _buttons;
    [SerializeField] public List<Button> buttons;
    
    public override void OnStartServer(){
        base.OnStartServer();
        _buttons = new List<bool>();
        for(int i = 0; i < 3; i++){
            _buttons.Add(false);
        }
    }
    
    //[Command]
    public void CmdSelectCharacter(int character){
        /*if(_buttons[character]) return;
        print(_buttons.Count);
        _buttons[character] = true;
        var btnID = buttons[character].GetComponent<NetworkIdentity>();
        btnID.AssignClientAuthority(connectionToClient);
        RpcFillCharacter(character);
        btnID.RemoveClientAuthority(connectionToClient);*/
        GameLobbyManager.localLobbyPlayer.CmdSelectCharacter(gameObject, character);
    }

    [ClientRpc]
    public void RpcFillCharacter(int character){
        print("vrau");
        buttons[character].interactable = false;
    }
}
public enum Character { Detective, Archeologist, Geologist}



