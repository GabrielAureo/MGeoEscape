using UnityEngine;
using Mirror;

public class LobbyPlayer: NetworkLobbyPlayer{
    [SyncVar(hook = "RenameGameObject")]
    public string playerName;
    public Character? cur_character;



    public override void OnStartLocalPlayer(){
        GameLobbyManager.localLobbyPlayer = this;
        //NetworkClient.RegisterHandler<LobbyUIMessage>((x,y) => HandleUI(x,y));
    }

    public override void OnStartClient(){
        
    }

    [TargetRpc]
    void TargetHandleUI(NetworkConnection target, int character, bool fill){
        Debug.Log("Rpc received by " + connectionToClient);
        var btn = GameLobbyManager.characterSelection.getCharacterButton(character);
        print(btn);
        print(fill);
        btn.interactable = fill;
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
        //if(characterSelection == null) characterSelection = charSelectObj.GetComponent<CharacterSelection>();
        var query = GameLobbyManager.characterSelection.playerDictionary[(Character)character];
        // if(characterSelection._buttons[character]) return; //jogador tentou selecionar posição já escolhida

        // characterSelection._buttons[character] = true;

        //print(this.netIdentity.name);
        

        if(query == this){ //jogador descelecionou seu personagem
            UpdateDictionary(character, null);
            cur_character = null;
            UpdateUI(character, true, connectionToClient);

        }else if(query == null){  //jogador sselecionou posição vazia
            if(cur_character != null){ //jogador já tem personagem selecionado, que é liberado
                UpdateDictionary((int)cur_character, null);
                UpdateUI((int)cur_character, true, connectionToClient);
            }
            UpdateDictionary(character, this);
            UpdateUI(character, false, connectionToClient);
            cur_character = (Character)character;
        }else{
            
        }


        // var btnID = characterSelection.buttons[character].GetComponent<NetworkIdentity>();
        // characterSelection.playerDictionary[(Character)character] = this; 
        // btnID.AssignClientAuthority(connectionToClient);
        //print(connectionToClient.playerController.name);
        // RpcFillCharacter(charSelectObj, character);
        // btnID.RemoveClientAuthority(connectionToClient);
        /*foreach(var kvp in GameLobbyManager.characterSelection.playerDictionary){
            Debug.Log(kvp.Key + ", " + kvp.Value);
        }*/
    }

    
    void UpdateUI(int character, bool fill, NetworkConnection sender){
        /*var msg = new LobbyUIMessage();
        msg.fill = fill;
        msg.character = character;
        NetworkServer.SendToAll<LobbyUIMessage>(msg);*/

        foreach(var kvp in NetworkServer.connections){
            if(kvp.Value == sender) continue;
            print(kvp.Value);
            TargetHandleUI(kvp.Value, character, fill);
        }

    }

    

    void UpdateDictionary(int charIndex, LobbyPlayer value){
        var btnID = GameLobbyManager.characterSelection.getCharacterButton(charIndex).GetComponent<NetworkIdentity>();
        GameLobbyManager.characterSelection.playerDictionary[(Character)charIndex] = value; 
        /*btnID.AssignClientAuthority(connectionToClient);
        RpcFillCharacter(charIndex);
        btnID.RemoveClientAuthority(connectionToClient);*/
    }

    [ClientRpc]
    public void RpcFillCharacter(int btnIndex){
        if(!isLocalPlayer) GameLobbyManager.characterSelection.getCharacterButton(btnIndex).interactable = false; // desativa botão da posição escolhida para todos exceto para o jogador
        
    }



}