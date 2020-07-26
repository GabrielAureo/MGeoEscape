using UnityEngine;
using Mirror;

public class LobbyPlayer: NetworkRoomPlayer{
    [SyncVar(hook = nameof(RenameGameObject))]
    public string playerName;
   

    public override void OnStartLocalPlayer(){
        GameManager.localLobbyPlayer = this;
        CmdSetupUI();
    }


    [Command]
    void CmdSetupUI(){
        foreach(var kvp in GameManager.characterSelection.playerDictionary){
            if(kvp.Value != null){
                TargetLocalSetup(connectionToServer, (int)kvp.Key);
                //btn.Toggle(false);
            }
        }
    }

    [TargetRpc]
    void TargetLocalSetup(NetworkConnection target, int character){
        var btn = GameManager.characterSelection.getCharacterButton(character);
        btn.Toggle(false);
    }

    [TargetRpc]
    void TargetLocalUI(NetworkConnection target, int character, bool select){
        var btn = GameManager.characterSelection.getCharacterButton(character);
        if(select){
            btn.Select();
            GameManager.characterSelection.ChangeSelectionBackground(character);
            CmdChangeReadyState(true);
        }else{
            btn.Deselect();
            GameManager.characterSelection.ChangeSelectionBackground(-1);
            CmdChangeReadyState(false);
        }
    }

    [TargetRpc]
    void TargetTeamUI(NetworkConnection target, int character, bool active){
        var btn = GameManager.characterSelection.getCharacterButton(character);
        btn.Toggle(active);
    }

    [Command]
    public void CmdChangeName(string name){
        playerName = name;
    }

    void RenameGameObject(string oldName, string newName){
        gameObject.name = "Lobby Player " + newName;
    }

    public void SelectCharacter(int character){
        CmdSelectCharacter(character);
    }
    [Command]
    public void CmdSelectCharacter(int character){
        var query = GameManager.characterSelection.playerDictionary[(Character)character];
        print(query);

        if(query == this){ //jogador descelecionou seu personagem
            //HandleSelection(character, null, query);
            Deselect(character);
        }else if(query == null){  //jogador sselecionou posição vazia
            //HandleSelection(character, this, query);
            Select(character);
        }
         
    }

    void Deselect(int character){
        print("aqui");
        UpdateDictionary(character, null);
        UpdateTeamUI(character, true, connectionToClient); //Libera botão para outros jogadores
        TargetLocalUI(connectionToClient, character, false); //Desceleciona botão para requerente
    }

    void Select(int character){
        Character? cur = null;
        foreach(var kvp in GameManager.characterSelection.playerDictionary){
            if(kvp.Value == this){
                cur = kvp.Key;
            }
        }
        if(cur != null){ //Jogador já tem personagem selecionado e eatá escolhendo outro
            Deselect((int) cur);
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
        GameManager.characterSelection.playerDictionary[(Character)charIndex] = value; 
        var btn = GameManager.characterSelection.getCharacterButton(charIndex);
    }



}