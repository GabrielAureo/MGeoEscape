using UnityEngine;
using Mirror;

public class LobbyUIMessage : MessageBase{
    public int character;
    public bool fill;

    public override string ToString(){
        return character + ", " + fill;
    }

}