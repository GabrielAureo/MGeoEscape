using UnityEngine;
using Mirror;

public class LobbyPlayer: NetworkLobbyPlayer{
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
    void TargetHandleUI(NetworkConnection target, int character, bool fill){
        Debug.Log("Rpc received by " + connectionToClient);
        var btn = GameLobbyManager.characterSelection.getCharacterButton(character);
        print(fill);
        btn.Toggle(fill);
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
        UpdateOtherPlayersUI(character, true, connectionToClient); //Libera botão para outros jogadores
        TargetHandleUI(connectionToClient, character, false); //Desceleciona botão para requerente
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
        UpdateOtherPlayersUI(character, false, connectionToClient);
        TargetHandleUI(connectionToClient, character, true);
    }
    /*void HandleSelection(int character, LobbyPlayer player, LobbyPlayer query){
        Character? cur = null;
        bool fill = true;
        foreach(var kvp in GameLobbyManager.characterSelection.playerDictionary){
            if(kvp.Value == this){
                cur = kvp.Key;
            }
        }
        if(cur != null && query == null){ //jogador já tem personagem selecionado, que é liberado
            UpdateDictionary((int) cur, null);
            UpdateUI((int) cur, true, connectionToClient);
        }
        if(query == null) fill = false;
        if(player == null){
            this.readyToBegin = false;
        }else{
            this.readyToBegin = true;
        }
        UpdateDictionary(character, player);
        UpdateUI(character, fill, connectionToClient);
    }*/

    
    void UpdateOtherPlayersUI(int character, bool fill, NetworkConnection sender){
        foreach(var kvp in NetworkServer.connections){
            if(kvp.Value == sender) continue;
            TargetHandleUI(kvp.Value, character, fill);
        }

    }

    void UpdateDictionary(int charIndex, LobbyPlayer value){
        GameLobbyManager.characterSelection.playerDictionary[(Character)charIndex] = value; 
        var btn = GameLobbyManager.characterSelection.getCharacterButton(charIndex);
        //btn.Toggle(value != null);
    }



}