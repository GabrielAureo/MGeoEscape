
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;

[System.Serializable]
public class EdgesDictionary : SerializableDictionaryBase<SocketNode, SocketNodeNeighbors>
{
    public Dictionary<SocketNode, List<SocketNode>> Unpack()
    {
        var unpackedDict = new Dictionary<SocketNode, List<SocketNode>>();
        foreach (var key in Keys)
        {
            unpackedDict.Add(key, this[key].data);
        }

        return unpackedDict;
    }
}
[System.Serializable]
public class SocketNodeNeighbors
{
    public List<SocketNode> data;

    public SocketNodeNeighbors()
    {
        data = new List<SocketNode>();
    }
}

[System.Serializable]
public class ViewConfigurationDictionary : SerializableDictionaryBase<string, SocketGraphContainer>{}