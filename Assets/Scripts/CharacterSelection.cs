using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;
using DG.Tweening;

public class CharacterSelection : NetworkBehaviour{
    Character chosenCharacter;
    [Header("UI Components")]
    [SerializeField] CanvasRenderer backgroundRenderer;
    [SerializeField] CharacterButton detectiveButton;
    [SerializeField] CharacterButton geologistButton;
    [SerializeField] CharacterButton archeologistButton;
    
    [Header("Materials")]
    [SerializeField] Material detectiveMaterial;
    [SerializeField] Material geologistMaterial;
    [SerializeField] Material archeologistMaterial;
    [SerializeField] Material defaultMaterial;
    

    Tween fade;

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

    public void ChangeSelectionBackground(int characterIndex){
        Character character = (Character) characterIndex;
        var newMaterial = getCharacterMaterial(character);
        FadeToMaterial(newMaterial);
    }

    public void ResetSelectionBackground(){
        FadeToMaterial(defaultMaterial);
    }

    private void FadeToMaterial(Material newMaterial){
        var cur = backgroundRenderer.GetMaterial();
        if(fade != null && fade.IsActive()) fade.Kill();
        fade = cur.DOFloat(0f, "Vector1_91214B53", .1f);
        fade.onComplete+= ()=>{
            backgroundRenderer.SetMaterial(newMaterial,0);
            cur.SetFloat("Vector1_91214B53", 1f);
            newMaterial.SetFloat("Vector1_91214B53", 0f);
            cur = backgroundRenderer.GetMaterial();
            cur.DOFloat(1f, "Vector1_91214B53", .1f);
        };
    }

    private Material getCharacterMaterial(Character character){
        switch(character){
            case Character.Detective:
                return detectiveMaterial;
            case Character.Archeologist:
                return archeologistMaterial;
            case Character.Geologist:
                return geologistMaterial;
            default:
                return defaultMaterial;
        }
    }

    public CharacterButton getCharacterButton(int characterIndex){
        var character = (Character)characterIndex;
        return getCharacterButton(character);
    }
    public void CmdSelectCharacter(int character){
        GameLobbyManager.localLobbyPlayer.SelectCharacter(character);
    }
}
public enum Character {Detective = (1 << 0),  Geologist = (1 << 1), Archeologist = (1 << 2)}



