using UnityEngine;
public class Resources: MonoBehaviour{
    [Header("Prefabs")]
    public GameObject previewSocketPrefab;
    private static Resources _instance;

    public static Resources Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

}