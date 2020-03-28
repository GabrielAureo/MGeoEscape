using UnityEngine;
public class DebugManager: MonoBehaviour{
    [SerializeField] GameObject m_standaloneCamera;
    [SerializeField] GameObject m_ARCamera;

    void Awake(){
        if(Camera.main == null){
            #if UNITY_EDITOR || UNITY_STANDALONE
            var cam = GameObject.Instantiate(m_standaloneCamera);
            cam.tag = "MainCamera";
            #elif UNITY_ANDROID && !UNITY_EDITOR
            var cam = GameObject.Instantiate(m_ARCamera);
            cam.tag = "MainCamera";
            #endif
        }else{
            if(Camera.main == m_standaloneCamera){
                #if UNITY_ANDROID && !UNITY_EDITOR
                Camera.main.enabled = false;
                var cam = GameObject.Instantiate(m_ARCamera);
                cam.tag = "MainCamera";
                #endif
            }else if(Camera.main == m_ARCamera){
                Camera.main.enabled = false;
                #if UNITY_EDITOR || UNITY_STANDALONE
                var cam = GameObject.Instantiate(m_standaloneCamera);
                cam.tag = "MainCamera";
                #endif

            }
        }
    }
    
}