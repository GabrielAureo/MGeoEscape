using UnityEngine;
using Mirror;

/// <summary>
/// Networked container class for movable objects
/// </summary>
public class NetMovable : NetworkBehaviour{
    [SyncVar]
    private GameObject movable;

    public GameObject GetMovable(){
        return movable;
    }

    public void SetMovable(GameObject movableObj){
        if(!movableObj.GetComponent<Movable>()){
            Debug.LogError("The GameObject needs a Movable component");
            return;
        }
        movable.transform.parent = transform;

    }
}