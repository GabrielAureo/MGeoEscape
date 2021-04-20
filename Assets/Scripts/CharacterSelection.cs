using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;
using DG.Tweening;
using Telepathy;

public class CharacterSelection : NetworkBehaviour{
    Character chosenCharacter;
    [Header("UI Components")]
    [SerializeField] CanvasRenderer backgroundRenderer = null;
    [SerializeField] CharacterButton detectiveButton = null;
    [SerializeField] CharacterButton geologistButton = null;
    [SerializeField] CharacterButton archeologistButton = null;
    
    [Header("Materials")]
    [SerializeField] Material detectiveMaterial = null;
    [SerializeField] Material geologistMaterial = null;
    [SerializeField] Material archeologistMaterial = null;
    [SerializeField] Material defaultMaterial = null;
    

    Tween fade;
    
    public SyncDictionary<int, uint> playerDictionary = new SyncDictionary<int, uint>();
    
    public override void OnStartServer(){
        base.OnStartServer();

        playerDictionary.Add((int)Character.Archeologist, 0);
        playerDictionary.Add((int)Character.Detective, 0);
        playerDictionary.Add((int)Character.Geologist, 0);
        
        
    }
    
    

    public override void OnStartClient()
    {
        base.OnStartClient();
        detectiveButton.selection = this;
        archeologistButton.selection = this;
        geologistButton.selection = this;

        playerDictionary.Callback += UpdateUI;
        UpdateUI();
    }

    private void UpdateUI(SyncDictionary<int, uint>.Operation op, int key, uint value)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (var kvp in playerDictionary)
        {
            var button = getCharacterButton(kvp.Key);
            if (ClientScene.localPlayer != null && kvp.Value == ClientScene.localPlayer.netId)
            {
                button.Select();
                continue;
            }
            if (kvp.Value != 0)
            {
                button.Disable();
            }
            else
            {
                button.Enable();
            }
        }

    }


    public void Awake(){
        // GameManager.characterSelection = this;
        // GetComponent<CanvasGroup>().interactable = true;
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
    [Command(ignoreAuthority = true)]
    public void CmdSelectCharacter(int character, NetworkConnectionToClient sender = null){
        var conn = playerDictionary[character];
        if (conn == sender.identity.netId)
        {
            playerDictionary[character] = 0;
            return;
        }
        playerDictionary[character] = sender.identity.netId;
        
    }

    public static Character IndexToCharacter(int index)
    {
        return (Character) (1 << index);
    }

    public static int CharacterToIndex(Character character)
    {
        return ((int)character) >> 1; 
    }
}
public enum Character {Detective = (1 << 0),  Geologist = (1 << 1), Archeologist = (1 << 2)}



