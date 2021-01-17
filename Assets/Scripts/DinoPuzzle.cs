using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

public class DinoPuzzle : Puzzle {

    [SerializeField] private List<SupplierSocket> suppliers;
    [SerializeField] private SocketGraph graph;
    private SyncList<int> _fillNodes = new SyncList<int>();

    private class SupplierWrapper
    {   
        public int spawnIndex;
        public List<uint> movableDistribution;
    }
    private SyncDictionary<int, List<uint>> suppliersDistribution =
        new SyncDictionary<int, List<uint>>();
    public override void OnServerInitialize()
    {
        StartCoroutine(InitializePuzzle());
    }

    IEnumerator InitializePuzzle()
    {
        
        var pool = new List<Movable>(graph.acceptedMovables);
        yield return new WaitUntil(()=>pool.All(movable => movable.isServer));
        
        var rand = Random.Range(0, graph.connections.Count);
        graph.StartGraph(rand);
        _fillNodes.Add(rand);
        

        
        Shuffle(pool);
        yield return new WaitUntil(() => pool.All(bone => bone.isServer));
        SetDistributionDictionary(pool);

    }

    private void SetDistributionDictionary(List<Movable> pool)
    {
         var n = Mathf.CeilToInt(pool.Count/3f);
                
        var start = 0;
        for (int i = 1; i <= 3; i++)
        {
            var bound = Mathf.Clamp(n * i, 0, pool.Count);
            var l = pool.GetRange(start, bound-start).Select(item => item.netIdentity.netId).ToList();
            // var sw = new SupplierWrapper()
            // {
            //     //supplier = suppliers[i - 1].netIdentity,
            //     movableDistribution = l
            // };

            var sw = l;
            
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
    

    public override void OnLocalPlayerReady(NetworkIdentity player)
    {
        if (player == null) return;
        foreach (var index in _fillNodes)
        {
            graph.TriggerNeighbors(index, true);
        }
        
        FillSuppliers(player.GetComponent<GamePlayer>().character);
        
    }

    IEnumerator FillSuppliers(Character character)
    {
        yield return new WaitUntil(() => suppliersDistribution.Count > 0);
        Debug.LogError(suppliersDistribution.Count);
        var wp = suppliersDistribution[CharacterSelection.CharacterToIndex(character)];
        var supplier = suppliers[CharacterSelection.CharacterToIndex(character)];
        //supplier.transform.position = spawnPoints[wp.spawnIndex].position;

        supplier.products = wp.Select(netId =>
        {
            var identity = NetworkIdentity.spawned[netId];
            var movable = identity.transform.GetComponent<Movable>();
            movable.transform.position = supplier.transform.position;
            return movable;
        }).ToList();
        Debug.LogError(supplier.products.Count);
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
