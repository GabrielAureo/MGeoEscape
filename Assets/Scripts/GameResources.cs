using UnityEngine;

public class GameResources: MonoBehaviour{
    [Header("Data")]
    //public CharacterData characterData;
    
    [Header("Prefabs")]
    public GameObject previewSocketPrefab;
    public GameObject busyPreviewPrefab;
    public GameObject emptySocketPrefab;
    public GameObject movableContainer;
    [Header("Puzzles Assets")]
    public PetrolCollection petrolCollection;
    
    private static GameResources _instance;

    public static GameResources Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(this);
        }
    }

}