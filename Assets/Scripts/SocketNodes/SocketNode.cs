using System.Collections.Generic;
using UnityEngine;

public class SocketNode: MonoBehaviour{
    Socket socket;
    List<SocketNode> children;
    private bool active;

    public void Activate(){
        active = true;
    }
    public void ActivateChildren(){
        foreach(var child in children){
            child.Activate();
        }
    }
}