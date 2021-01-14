using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PuzzlesManager : NetworkBehaviour
{
    [SerializeField] public List<Puzzle> puzzles = null;
    public override void OnStartServer(){
        foreach(var puzzle in puzzles){
            puzzle.OnServerInitialize();
        }
    }
    /*[ClientRpc]
    public void RpcSceneSetup(NetworkIdentity localPlayer){
        foreach(var puzzle in puzzles){
            puzzle.OnLocalPlayerReady(localPlayer);
        }
    }*/
    public override void OnStartClient(){
        foreach(var puzzle in puzzles){
            LocalPlayerAnnouncer.RunOnLocalPlayer(puzzle.OnLocalPlayerReady);
        }
    }


}
