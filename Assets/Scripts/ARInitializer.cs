using UnityEngine;
using Vuforia;
using Mirror;
public class ARInitializer : NetworkBehaviour{
    public override void OnStartLocalPlayer(){
        
        Camera.main.GetComponent<VuforiaBehaviour>().enabled = true;
        VuforiaRuntime.Instance.InitVuforia();

        var vuforia = VuforiaARController.Instance;    
        vuforia.RegisterVuforiaStartedCallback(OnVuforiaStarted);    
        vuforia.RegisterOnPauseCallback(OnPaused);
    }


    private void OnVuforiaStarted() {    
        CameraDevice.Instance.SetFocusMode(
        CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }

    private void OnPaused(bool paused) {    
    if (!paused) // resumed
    {
        // Set again autofocus mode when app is resumed
        CameraDevice.Instance.SetFocusMode(
            CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);    
    }
    }
}