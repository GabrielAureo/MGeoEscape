using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;
using UnityEngine.Networking.Types;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEditor;

public class SocketGraph: MonoBehaviour
{
    public List<Movable> acceptedMovables = new List<Movable>();
    //public EdgesDictionary connections;
    [HideInInspector]
    public string GUID = System.Guid.NewGuid().ToString();

    private static readonly int Opacity = Shader.PropertyToID("Opacity");

#if UNITY_EDITOR
    private void OnValidate()
    {
        var nodes = GetComponentsInChildren<SocketNode>().ToList();
        acceptedMovables = nodes.Select(node =>
        {
            node.graph = this;
            return node.exclusiveMovable;
        }).ToList();
        
    }
#endif

    // private void InitializeNodes()
    // {
    //     var nodes = GetNodesInScene();
    //     foreach (var node in nodes)
    //     {
    //         node.MovableAuth = CompatibleMovable;
    //     }
    // }

    public void StartGraph()
    {
        
    }
    

    public bool CompatibleMovable(Movable movable)
    {
        return acceptedMovables.Contains(movable);
    }

    private SocketNode[] GetNodesInScene()
    {
        return transform.GetComponentsInChildren<SocketNode>();
    }

//     private void OnDrawGizmos()
//     {
// #if UNITY_EDITOR
//         // if (Selection.activeGameObject != gameObject &&
//         //     Selection.activeGameObject.transform.parent != transform) return;
//
//         GetComponentsInChildren<SocketNode>().ToList().ForEach(node =>
//         {
//             var movable = node.exclusiveMovable;
//             var meshRenderer = movable.GetComponent<MeshRenderer>();
//             var filter = movable.GetComponent<MeshFilter>();
//
//             var material = new Material(meshRenderer.sharedMaterial)
//             {
//                 enableInstancing = true
//             };
//             material.SetFloat(Opacity, .5f);
//             //Graphics.DrawMesh(filter.sharedMesh, node.transform.localToWorldMatrix, material, 1);
//             Graphics.DrawMeshInstanced(filter.sharedMesh,0,material,new Matrix4x4[]{node.transform.localToWorldMatrix},1);
//         });
// #endif
//     }

}