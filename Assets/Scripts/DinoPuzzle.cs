using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class DinoPuzzle : Puzzle {

    [SerializeField] private SupplierSocket supplier;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private SocketGraph graph;
    private SyncList<int> _fillNodes = new SyncList<int>();

    private class SupplierWrapper
    {   
        public int spawnIndex;
        public List<NetworkIdentity> movableDistribution;
    }
    private SyncDictionary<int, SupplierWrapper> suppliersDistribution =
        new SyncDictionary<int, SupplierWrapper>();
    public override void OnServerInitialize()
    {

        var rand = Random.Range(0, graph.connections.Count);

        graph.StartGraph(rand);
        _fillNodes.Add(rand);
        var pool = new List<Movable>(graph.acceptedMovables);
        Shuffle(pool);

        StartCoroutine(WaitBonesSpawn(pool));

    }

    private void SetDistributionDictionary(List<Movable> pool)
    {
         var n = Mathf.CeilToInt(pool.Count/3f);
                
        var start = 0;
        for (int i = 1; i <= 3; i++)
        {
            var bound = Mathf.Clamp(n * i, 0, pool.Count);
            var l = pool.GetRange(start, bound-start).Select(item => item.netIdentity).ToList();
            var sw = new SupplierWrapper()
            {
                //supplier = suppliers[i - 1].netIdentity,
                movableDistribution = l
            };
            
            suppliersDistribution.Add((int)CharacterSelection.IndexToCharacter(i-1), sw);
            
            start = bound;
        }
        // Debug.Log(suppliersDistribution.Count);
        // suppliersDistribution.Values.ToList().ForEach(wrapper =>
        // {
        //     string s = "";
        //     var t = from item in wrapper.movableDistribution select item.netId.ToString();
        //     t.ToList().ForEach(s1 => s+= s1 + ", ");
        //     Debug.Log($"[{s}]");
        // });
        
        
    }

    IEnumerator WaitBonesSpawn(List<Movable> pool)
    {
        yield return new WaitWhile(() => pool.All(bone => bone.isServer));
        SetDistributionDictionary(pool);
    }

    public override void OnLocalPlayerReady(NetworkIdentity player)
    {
        foreach (var index in _fillNodes)
        {
            graph.TriggerNeighbors(index, true);
        }
        
        FillSuppliers(player.GetComponent<GamePlayer>().character);
        
    }

    void Update()
    {
        Debug.LogError(suppliersDistribution.Count);
    }

    private void FillSuppliers(Character character)
    {
        Debug.LogError(suppliersDistribution.Count);
        var wp = suppliersDistribution[(int)character];
        supplier.transform.position = spawnPoints[wp.spawnIndex].position;

        supplier.products = wp.movableDistribution.Select(netId =>
        {
            var movable = netId.GetComponent<SocketNode>().exclusiveMovable;
            movable.transform.position = supplier.transform.position;
            return movable;
        }).ToList();

    }
   

    private static void Shuffle<T>(IList<T> list)  
    {  
        var n = list.Count;  
        while (n > 1) {  
            n--;
            var k = Random.Range(0, n + 1); 
            var value = list[k];  
            list[k] = list[n];  
            list[n] = value;  
        }  
    }
}
