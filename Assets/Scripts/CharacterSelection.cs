using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CharacterSelection : MonoBehaviour{
    [SerializeField] NetworkLobbyManager lobbyManager;

    public void SelectCharacter(int character){
        print((Character)character);
    }
}
public enum Character { Detective, Archeologist, Geologist}



