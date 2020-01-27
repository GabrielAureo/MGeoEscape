using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PuzzlesInitializer : NetworkBehaviour
{
    [SerializeField] List<Puzzle> m_puzzles = null;
    public override void OnStartServer(){
        foreach(var puzzle in m_puzzles){
            puzzle.Initialize();
        }
    }
    [ClientRpc]
    public void RpcSceneSetup(NetworkIdentity localPlayer){
        foreach(var puzzle in m_puzzles){
            puzzle.OnLocalPlayerReady(localPlayer);
        }
    }
}
