using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
	[Header("Match GUI")]
	[SerializeField]
	private GameObject _connection;
	[SerializeField]
	private Lobby _lobby;

	public string NetworkAdress { set => this.networkAddress = value; }
	public string PlayerName { get; set; }

	public override void OnStartClient()
	{
		_connection.SetActive(false);
	}

	public override void OnServerAddPlayer(NetworkConnectionToClient conn)
	{
		GameObject playerGO = Instantiate(playerPrefab);
		playerGO.name = $"{PlayerName} [connId={conn.connectionId}]";
		NetworkServer.AddPlayerForConnection(conn, playerGO);

		Player player = playerGO.GetComponent<Player>();
		_lobby.AddPlayer(player);
	}

	public override void OnClientDisconnect()
	{
		_connection.SetActive(true);
		_lobby.gameObject.SetActive(true);
	}
	
	public override void OnServerDisconnect(NetworkConnectionToClient conn)
	{
		_lobby.RemovePlayer(conn.identity.GetComponent<Player>());
		_lobby.gameObject.SetActive(true);
		base.OnServerDisconnect(conn);
	}
}