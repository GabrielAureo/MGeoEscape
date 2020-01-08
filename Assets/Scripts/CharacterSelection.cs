using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;

public class CharacterSelection : NetworkBehaviour{
    Character chosenCharacter;
    [SerializeField] CharacterButton detectiveButton;
    [SerializeField] CharacterButton geologistButton;
    [SerializeField] CharacterButton archeologistButton;

    public Dictionary<Character, LobbyPlayer> playerDictionary;
    
    public override void OnStartServer(){
        base.OnStartServer();
        
        playerDictionary = new Dictionary<Character, LobbyPlayer>(){
            {Character.Archeologist, null},
            {Character.Detective, null},
            {Character.Geologist, null}
        };
    }

    public override void OnStartClient(){
        base.OnStartLocalPlayer();
        Debug.Log("set Charselection to manager");
        GameLobbyManager.characterSelection = this;
    }
    public CharacterButton getCharacterButton(Character character){
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

    public CharacterButton getCharacterButton(int characterIndex){
        var character = (Character)characterIndex;
        return getCharacterButton(character);
    }
    public void CmdSelectCharacter(int character){
        GameLobbyManager.localLobbyPlayer.CmdSelectCharacter(character);
    }
}
public enum Character {Detective = 0,  Geologist = 1, Archeologist = 2}



