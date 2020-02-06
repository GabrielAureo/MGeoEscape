using UnityEngine;
using UnityEngine.Events;
public class Pressable : ARInteractable
{
    public UnityEvent pressEvent;
    public override void onTap(ARTouchData touchData)
    {
        pressEvent.Invoke();
    }

}