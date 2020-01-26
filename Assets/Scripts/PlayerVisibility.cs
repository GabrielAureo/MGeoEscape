using UnityEngine;
using Mirror;

public class PlayerVisibility: NetworkBehaviour{
    [EnumFlag]
    [SerializeField] Character character;

    public override void OnStartClient(){
        if(!character.HasFlag(NetworkServer.localConnection.identity.GetComponent<OnlinePlayer>().character)){
            gameObject.SetActive(false);
        }
    }
}