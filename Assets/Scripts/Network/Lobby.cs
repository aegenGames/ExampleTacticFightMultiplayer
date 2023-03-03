using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Lobby : NetworkBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _displayPlayers;
	[SerializeField]
	private Button _startButton;
	[SerializeField]
	private MatchManager _matchManager;

	private HashSet<Player> _players = new HashSet<Player>();
	public string LocalName { get; set; }

	public void AddPlayer(Player player)
	{
		player.OnUpdated += OnUpdatePlayerList;
		_players.Add(player);
		RpcAddPlayer(player);
	}

	[ClientRpc]
	private void RpcAddPlayer(Player player)
	{
		player.Name = LocalName;
	}

	public void RemovePlayer(Player player)
	{
		_players.Remove(player);
		OnUpdatePlayerList();
	}

	private void OnEnable()
	{
		_matchManager.StopAllCoroutines();
	}

	private void OnUpdatePlayerList()
	{
		string list = "";
		foreach (Player player in _players)
		{
			list += string.Format("{0}\n", player.Name);
		}
		RpcDisplayPlayerList(list);
		_startButton.interactable = _players.Count > 1;
	}

	[ClientRpc]
	private void RpcDisplayPlayerList(string list)
	{
		_displayPlayers.text = list;
	}

	[ClientRpc]
	public void RpcStartGame()
	{
		_matchManager.Players = _players.ToList<Player>();
		_matchManager.StartMatchManager();
		this.gameObject.SetActive(false);
	}
}