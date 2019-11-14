using UnityEngine;
using Mirror;

public class LobbyUIMessage : MessageBase{
    public int character;
    public bool fill;
    public int sender;

    public override string ToString(){
        return "Connection: " + sender + "\nCharacter: " + character + "\nFill: " + fill;
    }

}