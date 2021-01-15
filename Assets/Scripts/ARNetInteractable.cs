using UnityEngine;
using Mirror;
/// <summary>
/// Networked version of ArInteractable, so it properly syncs over the network. If the interactable
/// objects doesn't need to sync over network, use the IARInteractable interface instead.
/// </summary>
public abstract class ARNetInteractable : NetworkBehaviour, IARInteractable
{
    public abstract void onHold();

    public abstract void onRelease();

    public abstract void onTap();

}