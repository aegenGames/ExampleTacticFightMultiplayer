using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class Healthpoints : NetworkBehaviour, IHealthpoints
{
	[SerializeField]
	private int _maxHP;
	[SerializeField]
	private Image _healthbar;
	[SerializeField]
	private TextMeshProUGUI _displayHP;

	[SyncVar(hook = nameof(DisplayHP))]
	private int _hp;

	public int HP
	{
		get => _hp;
		set
		{
			_hp = value > 0 ? value : 0;
		}
	}

	private void DisplayHP(int _, int newHP)
	{
		if(isServer)
			RcpDisplayHP(newHP);
	}

	[ClientRpc]
	private void RcpDisplayHP(int hp)
	{
		if (_healthbar)
			_healthbar.fillAmount = (float)_hp / _maxHP;
		if (_displayHP)
			_displayHP.text = _hp.ToString();
	}

	public void ResetHP()
	{
		_hp = _maxHP;
	}
}