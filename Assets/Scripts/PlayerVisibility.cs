using UnityEngine;
using Mirror;

public class PlayerVisibility: MonoBehaviour{
    [EnumFlag]
    [SerializeField] Character character;
    

    public void Awake(){
        CheckFlag(ClientScene.localPlayer);

        LocalPlayerAnnouncer.OnLocalPlayerUpdated += (CheckFlag);
    }

    private void CheckFlag(NetworkIdentity localPlayer){
        if(localPlayer != null){
            var player = localPlayer.GetComponent<GamePlayer>();
            if(player!= null && !character.HasFlag(player.character)){
                gameObject.SetActive(false);
            }
        }
        
    }

    private void OnDestroy(){
        LocalPlayerAnnouncer.OnLocalPlayerUpdated -= (CheckFlag);
    }
}