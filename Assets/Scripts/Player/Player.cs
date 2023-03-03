using UnityEngine;
using Mirror;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
	[SerializeField]
	private GameObject _plannigPhaseObjects;
	[SerializeField]
	private AttackMananger _attackManager;
	[SerializeField]
	private ProtectionManager _protectManager;

	[SyncVar]
	private Character _character;
	[SyncVar]
	private bool _isAttacking = false;
	[SyncVar]
	private bool _isPlanning = false;
	[SyncVar]
	private string _name;

	public string Name
	{
		get => _name;
		set
		{
			if (!isLocalPlayer)
				return;

			_name = value;
			if (isServer)
				SetName(_name);
			else
				CmdSetName(_name);
		}
	}

	public Character Char
	{
		get => _character;
		set
		{
			_character = value;
			_character.netIdentity.AssignClientAuthority(connectionToClient);
			_character.Name = Name;
			TargetLocalSettingOfChar();
		}
	}

	public bool IsPlanning{ get => _isPlanning; set => _isPlanning = value; }
	public bool IsAttacking { get => _isAttacking; }

	public UnityAction OnUpdated;

	[Command]
	private void CmdSetName(string name)
	{
		SetName(name);
	}

	[Server]
	private void SetName(string name)
	{
		_name = name;
		OnUpdated();
	}

	[TargetRpc]
	public void TargetAttack(Player targetPlayer)
	{
		Attack(targetPlayer);
	}

	[Client]
	private void Attack(Player target)
	{
		CmdSetAttacing(true);
		StartCoroutine(this._character.Attack(target.Char, _attackManager.AttackList));
	}

	[TargetRpc]
	private void TargetLocalSettingOfChar()
	{
		_character.OnEndAttack.AddListener(EndAttack);
		_character.SetMeshColor(Color.blue);
	}

	[Command]
	private void CmdSetAttacing(bool value)
	{
		_isAttacking = value;
	}

	[Client]
	private void EndAttack()
	{
		CmdSetAttacing(false);
	}

	public void SetPlanningPhase(bool value)
	{
		if(isServer)
		{
			ServerSetPlanningPhase(value);
		}
		else
		{
			CmdSetPlanningPhase(value);
		}
	}

	[Command]
	private void CmdSetPlanningPhase(bool value)
	{
		ServerSetPlanningPhase(value);
	}

	[Server]
	private void ServerSetPlanningPhase(bool value)
	{
		IsPlanning = value;
		TargetSetPlanningPhase(value);
	}

	[TargetRpc]
	private void TargetSetPlanningPhase(bool value)
	{
		_plannigPhaseObjects.SetActive(value);
		if(value)
		{
			_attackManager.ResetPoints();
			_protectManager.ResetPoints();
		}
	}

	[TargetRpc]
	public void UpdateDef()
	{
		_character.ProtectList = _protectManager.GetDefList();
	}
}