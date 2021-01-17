using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System.Linq;
using UnityEngine.Events;

/*
	Documentation: https://mirror-networking.com/docs/Components/NetworkManager.html
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkManager.html
*/

public class MainNetworkManager : NetworkManager
{
	static readonly ILogger logger = LogFactory.GetLogger(typeof(MainNetworkManager));
	[SerializeField] private NetworkBehaviour roomPlayerPrefab;
	[SerializeField] private CharacterSelection characterSelection;


	private List<NetworkConnectionToClient> pendingPlayers;

	public UnityAction OnServerPlayersSpawn;
	public override void OnStartClient()
	{
		if (roomPlayerPrefab == null || roomPlayerPrefab.gameObject == null)
			logger.LogError("NetworkRoomManager no RoomPlayer prefab is registered. Please add a RoomPlayer prefab.");
		else
			ClientScene.RegisterPrefab(roomPlayerPrefab.gameObject);

		if (playerPrefab == null)
			logger.LogError("NetworkRoomManager no GamePlayer prefab is registered. Please add a GamePlayer prefab.");
		
		NetworkClient.RegisterHandler<PlayerSpawnMessage>(OnPlayerSpawn);
		
	}

	public override void OnStartServer()
	{
		base.OnStartServer();
		characterSelection.playerDictionary.Callback += (op, key, item) => CheckReady();
	}

	private void CheckReady()
	{
		var count = characterSelection.playerDictionary.Count(kvp => kvp.Value != 0);
		if (count != 3) return;
		
		foreach (var kvp in characterSelection.playerDictionary)
		{
			var conn  = NetworkIdentity.spawned[kvp.Value].connectionToClient;
			SpawnGamePlayer(conn);
		}

		OnServerPlayersSpawn();
	}
	
	public struct PlayerSpawnMessage : NetworkMessage{}

	private void OnPlayerSpawn(NetworkConnection conn, PlayerSpawnMessage msg)
	{
		characterSelection.gameObject.SetActive(false);
		#if UNITY_EDITOR || UNITY_STANDALONE
		Camera.main.gameObject.AddComponent<ExtendedFlycam>();
		#endif
	}

	private void SpawnGamePlayer(NetworkConnection conn)
	{
		var character = characterSelection.playerDictionary.Single(pair => pair.Value == conn.identity.netId).Key;
		var playerObj = Instantiate(playerPrefab);
		var player = playerObj.GetComponent<GamePlayer>();
		player.character = (Character)character;
		NetworkServer.ReplacePlayerForConnection(conn, playerObj, true);
		var msg = new PlayerSpawnMessage();
		NetworkServer.SendToAll(msg);
		
	}

	public override void OnServerAddPlayer(NetworkConnection conn)
	{
		var player = Instantiate(roomPlayerPrefab.gameObject);
		NetworkServer.AddPlayerForConnection(conn, player);
	}

	public override void OnServerDisconnect(NetworkConnection conn)
	{
		foreach (var kvp in characterSelection.playerDictionary)
		{
			if (kvp.Value == conn.identity.netId)
			{
				characterSelection.playerDictionary[kvp.Key] = 0;
				break;
			}
		}

		base.OnServerDisconnect(conn);
	}
}
