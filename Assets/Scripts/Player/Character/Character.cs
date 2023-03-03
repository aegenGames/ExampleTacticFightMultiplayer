using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.Events;

public class Character : NetworkBehaviour
{
	public enum Body
	{
		LH,
		RH,
		LUB,
		RUB,
		LMB,
		RMB,
		LLB,
		RLB
	}

	[SerializeField]
	[SerializeInterface(typeof(IAnimationController))]
	private Object animController;
	private IAnimationController _animController => animController as IAnimationController;

	[SerializeField]
	[SerializeInterface(typeof(IHealthpoints))]
	private Object healthPoints;
	private IHealthpoints _healthPoints => healthPoints as IHealthpoints;

	[SerializeField]
	private TextMeshProUGUI _displayName;
	[SerializeField]
	private SkinnedMeshRenderer _mesh;
	[SerializeField]
	private int _dmg = 1;

	[SyncVar]
	private string _name;
	private Character _target;
	private Character.Body _curAttack;

	public string Name
	{
		get => _name;
		set 
		{
			_name = value;
			DisplayName();
		}
	}
	public bool IsDied { get; private set; } = false;
	public HashSet<Character.Body> ProtectList { get; set; }

	public UnityEvent OnEndAttack { get; } = new UnityEvent();

    public void OnDisable()
    {
		SetMeshColor(Color.red);
	}

    [Command(requiresAuthority = false)]
	private void CmdTakeDmg(int dmg, Character.Body target)
	{
		TargetTakeDmg(dmg, target);
	}

	[TargetRpc]
	private void TargetTakeDmg(int dmg, Character.Body target)
	{
		if (!ProtectList.Contains(target))
			CmdDecreaseHP(dmg);
	}

	[Command]
	private void CmdDecreaseHP(int dmg)
	{
		_healthPoints.HP -= dmg;
		IsDied = _healthPoints.HP == 0;
	}

	public IEnumerator Attack(Character target, Dictionary<Character.Body, int> attackList)
	{
		if (attackList == null)
			yield break;

		_target = target;
		foreach (var attack in attackList)
		{
			for(int i = 0; i < attack.Value; ++i)
			{
				_curAttack = attack.Key;
				_animController.CallAttackAnimation(attack.Key);
				do
				{
					yield return new WaitForSeconds(0.1f);
				} while (_animController.IsActive);
			}
		}
		OnEndAttack.Invoke();
	}

	public void SetMeshColor(Color32 color)
	{
		_mesh.material.SetColor("_TintColor", color);
	}

	public void ClearCharacter()
	{
		CmdRemoveAuthority();
	}

	[Command]
	private void CmdRemoveAuthority()
	{
		RemoveAuthority();
	}

	[Server]
	private void RemoveAuthority()
	{
		OnEndAttack.RemoveAllListeners();
		this.netIdentity.RemoveClientAuthority();
	}

	[Server]
	public void ResetCharacter()
	{
		_healthPoints.ResetHP();
		IsDied = false;
	}

	[ClientRpc]
	public void DisplayName()
	{
		_displayName.text = _name;
	}

	/// <summary>
	/// Invoke in animation
	/// </summary>
	[ClientCallback]
	public void MakeDmg()
	{
		if (!isOwned)
			return;

		_target.CmdTakeDmg(_dmg, _curAttack);
	}
}