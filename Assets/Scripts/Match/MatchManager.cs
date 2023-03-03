using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class MatchManager : NetworkBehaviour
{
	[SerializeField]
	private Character[] _characters;
	[SerializeField]
	private int _plainnigTime = 15;
	[SerializeField]
	private GameObject _endRoundUI;
	[SerializeField]
	private TextMeshProUGUI _displayPlainningTimer;
	[SerializeField]
	private TextMeshProUGUI _displayMatchResult;
	[SerializeField]
	private GameObject _exitGameButton;

	public List<Player> Players { get; set; }

	private void Update()
	{
		if(Input.GetKeyUp(KeyCode.Escape))
		{
			_exitGameButton.SetActive(!_exitGameButton.activeSelf);
		}
	}

	public void ExitGame()
	{
		foreach (Character character in _characters)
		{
			character.ClearCharacter();
		}

		Application.Quit();
	}

	public void StartMatchManager()
	{
		StartGame();
	}

	[ServerCallback]
	private void StartGame()
	{
		for (int i = 0; i < Players.Count; ++i)
		{
			if(Players[i].Char == null)
				Players[i].Char = _characters[i];
			_characters[i].DisplayName();
		}
		StartMatch();
	}

	[ServerCallback]
	public void StartMatch()
	{
		SetResultActive(false);
		foreach(Character character in _characters)
		{
			character.ResetCharacter();
		}
		StartCoroutine(PlanningPhase());
	}

	[Server]
	private IEnumerator PlanningPhase()
	{
		SetPlanningPhase(true);
		yield return StartCoroutine(Timer(_plainnigTime));
		SetPlanningPhase(false);

		StartCoroutine(BattlePhase());
	}

	[Server]
	private void SetPlanningPhase(bool value)
	{
		foreach (Player player in Players)
		{
			player.SetPlanningPhase(value);
		}
	}

	[Server]
	private IEnumerator BattlePhase()
	{
		Players[0].UpdateDef();
		Players[1].UpdateDef();
		yield return StartCoroutine(StartAttack(Players[0], Players[1]));
		yield return StartCoroutine(StartAttack(Players[1], Players[0]));
		if (_characters[0].IsDied || _characters[1].IsDied)
		{
			DisplayResult();
		}
		else
		{
			StartCoroutine(PlanningPhase());
		}
	}

	[Server]
	private IEnumerator StartAttack(Player attackingPlayer, Player defendingPlayer)
	{
		attackingPlayer.TargetAttack(defendingPlayer);
		do
		{
			yield return new WaitForSeconds(0.1f);
		}
		while (attackingPlayer.IsAttacking);
	}

	[Server]
	private void DisplayResult()
	{
		string result;

		if(_characters[0].IsDied && _characters[1].IsDied)
		{
			result = "Draw";
		}
		else if(_characters[0].IsDied)
		{
			result = _characters[1].Name + "\nWin";
		}
		else
		{
			result = _characters[0].Name + "\nWin";
		}

		RpcSetMatchResult(result);
		SetResultActive(true);
	}

	[ClientRpc]
	private void RpcSetMatchResult(string result)
	{
		_displayMatchResult.text = result;
	}

	[ClientRpc]
	private void SetResultActive(bool value)
	{
		_endRoundUI.SetActive(value);
	}

	[Server]
	private IEnumerator Timer(int time)
	{
		RpcTimerSetEnable(true);
		for (int i = time; i > 0; --i)
		{
			RpcChangeTimer(i);
			yield return new WaitForSeconds(1);
			if (!Players[0].IsPlanning && !Players[1].IsPlanning)
				break;
		}
		RpcTimerSetEnable(false);
	}

	[ClientRpc]
	private void RpcTimerSetEnable(bool value)
	{
		_displayPlainningTimer.gameObject.SetActive(value);
	}

	[ClientRpc]
	private void RpcChangeTimer(int value)
	{
		_displayPlainningTimer.text = value.ToString();
	}
}