using UnityEngine;
using Mirror;

public class PlayerVisibility: MonoBehaviour{
    [EnumFlag]
    [SerializeField] Character character = 0;
    
    public void Awake(){
        CheckFlag(ClientScene.localPlayer);

        LocalPlayerAnnouncer.OnLocalPlayerUpdated += (CheckFlag);
        
        //CheckFlag(ClientScene.localPlayer);
    }

    private void CheckFlag(NetworkIdentity localPlayer){
        if(localPlayer != null){
            var player = localPlayer.GetComponent<GamePlayer>();
            Debug.Log("Setting Visibility of " + gameObject.name + "for player:" + player);
            if(player!= null && !character.HasFlag(player.character)){
                gameObject.SetActive(false);
            }
        }
        
    }

    private void OnDestroy(){
        LocalPlayerAnnouncer.OnLocalPlayerUpdated -= (CheckFlag);
    }
}