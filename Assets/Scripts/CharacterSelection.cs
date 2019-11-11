using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;

public class CharacterSelection : NetworkBehaviour{
    Character chosenCharacter;
    [SerializeField] Button detectiveButton;
    [SerializeField] Button geologistButton;
    [SerializeField] Button archeologistButton;
    [HideInInspector] public List<bool> _buttons;
    //[HideInInspector] public List<Button> buttons;
    public Dictionary<Character, LobbyPlayer> playerDictionary;
    
    public override void OnStartServer(){
        base.OnStartServer();
        
        playerDictionary = new Dictionary<Character, LobbyPlayer>(){
            {Character.Archeologist, null},
            {Character.Detective, null},
            {Character.Geologist, null}
        };
        _buttons = new List<bool>();
        for(int i = 0; i < 3; i++){
            _buttons.Add(false);
        }
    }

    public override void OnStartClient(){
        base.OnStartLocalPlayer();
        Debug.Log("set Charselection to manager");
        GameLobbyManager.characterSelection = this;
    }
    public Button getCharacterButton(Character character){
        switch(character){
            case Character.Geologist:
                return geologistButton;
            case Character.Archeologist:
                return archeologistButton;
            case Character.Detective:
                return detectiveButton;
            default:
                Debug.LogError("Personagem inválido");
                return null;
        }
    }

    public Button getCharacterButton(int characterIndex){
        var character = (Character)characterIndex;
        return getCharacterButton(character);
    }
    //[Command]
    public void CmdSelectCharacter(int character){
        GameLobbyManager.localLobbyPlayer.CmdSelectCharacter(gameObject, character);
    }

    /*[ClientRpc]
    public void RpcFillCharacter(int character){
        print("vrau");
        buttons[character].interactable = false;
    }*/
}
public enum Character {Detective = 0,  Geologist = 1, Archeologist = 2}



