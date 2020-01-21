using UnityEngine;
using Mirror;

public class LobbyPlayer: NetworkRoomPlayer{
    [SyncVar(hook = "RenameGameObject")]
    public string playerName;

    public override void OnStartLocalPlayer(){
        GameLobbyManager.localLobbyPlayer = this;
    }

    public override void OnStartClient(){
        
    }

    private void SetupUI(){
        foreach(var kvp in GameLobbyManager.characterSelection.playerDictionary){
            if(kvp.Value != null){
                var btn = GameLobbyManager.characterSelection.getCharacterButton(kvp.Key);
                btn.Toggle(false);
            }
        }
    }

    [TargetRpc]
    void TargetLocalUI(NetworkConnection target, int character, bool select){
        var btn = GameLobbyManager.characterSelection.getCharacterButton(character);
        if(select){
            btn.Select();
            GameLobbyManager.characterSelection.ChangeSelectionBackground(character);
        }else{
            btn.Deselect();
            GameLobbyManager.characterSelection.ChangeSelectionBackground(-1);
        }
    }

    [TargetRpc]
    void TargetTeamUI(NetworkConnection target, int character, bool active){
        var btn = GameLobbyManager.characterSelection.getCharacterButton(character);
        if(active){
            btn.Deselect();
        }else{
            btn.Disable();
        }
    }

    [Command]
    public void CmdChangeName(string name){
        playerName = name;
    }

    void RenameGameObject(string name){
        gameObject.name = "Player " + name;
    }


    [Command]
    public void CmdSelectCharacter(int character){
        var query = GameLobbyManager.characterSelection.playerDictionary[(Character)character];
        print(query);

        if(query == this){ //jogador descelecionou seu personagem
            //HandleSelection(character, null, query);
            DeselectCharacter(character);
        }else if(query == null){  //jogador sselecionou posição vazia
            //HandleSelection(character, this, query);
            SelectCharacter(character);
        }
    }

    void DeselectCharacter(int character){
        print("aqui");
        UpdateDictionary(character, null);
        UpdateTeamUI(character, true, connectionToClient); //Libera botão para outros jogadores
        TargetLocalUI(connectionToClient, character, false); //Desceleciona botão para requerente
    }

    void SelectCharacter(int character){
        Character? cur = null;
        foreach(var kvp in GameLobbyManager.characterSelection.playerDictionary){
            if(kvp.Value == this){
                cur = kvp.Key;
            }
        }
        if(cur != null){ //Jogador já tem personagem selecionado e eatá escolhendo outro
            DeselectCharacter((int) cur);
        }

        UpdateDictionary(character, this);
        UpdateTeamUI(character, false, connectionToClient);
        
        TargetLocalUI(connectionToClient, character, true);
    }

    
    void UpdateTeamUI(int character, bool active, NetworkConnection sender){
        foreach(var kvp in NetworkServer.connections){
            if(kvp.Value == sender) continue;
            TargetTeamUI(kvp.Value, character, active);
        }

    }

    void UpdateDictionary(int charIndex, LobbyPlayer value){
        GameLobbyManager.characterSelection.playerDictionary[(Character)charIndex] = value; 
        var btn = GameLobbyManager.characterSelection.getCharacterButton(charIndex);
    }



}