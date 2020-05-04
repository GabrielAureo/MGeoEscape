using UnityEngine;
using Mirror;

public class MainManager: MonoBehaviour{
    private static MainManager _instance;

    public static MainManager Instance { get { return _instance; } }

    public PuzzlesManager puzzlesManager;

    
    
    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }



    void Update(){
        // var id = puzzlesManager.netId;
        // print(id);
        // var sp = (SafePuzzle)NetworkIdentity.spawned[id].GetComponent<PuzzlesManager>().puzzles[0];
        // Debug.Log(sp.generatedPassword);
        // string s = "";
        // foreach(var p in sp.generatedPassword){
        //     s += p.ToString() + " ";
        // }
        // Debug.Log(s);
        
        print(GameObject.FindObjectOfType<PuzzlesManager>());
    }
}