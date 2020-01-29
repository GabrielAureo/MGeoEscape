using UnityEngine;
using Mirror;
public class GamePlayer: NetworkBehaviour{
    [SyncVar(hook="RenameGameObject")]
    [HideInInspector] public string playerName;
    [HideInInspector] [SyncVar] public Character character;

    [SerializeField] ARTouchController touchController;

    [Command]
    public void CmdPlayerReady(){
        GameObject.FindObjectOfType<PuzzlesInitializer>().RpcSceneSetup(netIdentity);
    }

    public override void OnStartLocalPlayer(){
        CmdPlayerReady();
    }
    void RenameGameObject(string name){
        gameObject.name = "Game Player " + name;
    }

    void Update(){
        if(!isLocalPlayer) return;
        touchController.HandleInput();

    }
    
}