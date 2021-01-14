using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BaseSocket))]
public class SocketCallbackController : MonoBehaviour
{
    public SocketEvent OnServerTake;
    public SocketEvent OnServerReceive;
    public SocketEvent OnClientTake;
    public SocketEvent OnClientReceive;
    private void Reset()
    {
        transform.GetComponent<BaseSocket>().callbackController = this;
    }
}
