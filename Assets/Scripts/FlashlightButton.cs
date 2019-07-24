using UnityEngine;
using UnityEngine.UI;
using Vuforia;

[RequireComponent(typeof(Button))]
public class FlashlightButton : MonoBehaviour{
    bool trigger = false;
    void Start(){
        GetComponent<Button>().onClick.AddListener(()=>{
            trigger = !trigger;
            CameraDevice.Instance.SetFlashTorchMode(trigger);
        });
    }
    
}