using UnityEngine;
using UnityEngine.Events;
public class Pressable : ARInteractable
{
    public UnityEvent pressEvent;
    public override void onTap(ARTouchController controller)
    {
        pressEvent.Invoke();
    }

}