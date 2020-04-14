using UnityEngine;

public class ARTouchData{
    public enum Status {HOLDING, WAITING, NO_TOUCH}
    public Ray ray;
    public IARInteractable selectedInteractable;
    public Status currentStatus;
    public Status lastStatus;
    public RaycastHit hit;
}