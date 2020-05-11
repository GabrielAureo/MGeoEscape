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
}