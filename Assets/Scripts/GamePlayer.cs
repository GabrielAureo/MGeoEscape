using UnityEngine;
using Mirror;
using UnityEngine.EventSystems;

public class GamePlayer: NetworkBehaviour{
    [SyncVar(hook=nameof(RenameGameObject))]
    [HideInInspector] public string playerName;
    [HideInInspector] [SyncVar] public Character character;

    [SerializeField] ARTouchController touchController = null;

    // [Command]
    // public void CmdPlayerReady(){
    //     GameObject.FindObjectOfType<PuzzlesManager>().RpcSceneSetup(netIdentity);
    // }

    void RenameGameObject(string oldName, string newName){
        gameObject.name = "Game Player " + name;
    }

    // void Update(){
    //     if(!isLocalPlayer) return;
    //     //touchController.HandleInput();
    //     transform.position = Camera.main.transform.position;
    //     transform.rotation = Camera.main.transform.rotation;

    // }
    
}