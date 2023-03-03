using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class AttackMananger : MonoBehaviour
{
	[SerializeField]
	private int _maxPoints;
	[SerializeField]
	private TextMeshProUGUI _displayPoints;

	private int _points;
	public Dictionary<Character.Body, int> AttackList { get; private set; } = new Dictionary<Character.Body, int>();

	public int Points
	{
		get => _points;
		set
		{
			_points = value;
			_displayPoints.text = _points.ToString();
		}
	}

	public UnityAction OnReset;

	private void Start()
	{
		Points = _maxPoints;
	}

	public void ResetPoints()
	{
		Points = _maxPoints;
		AttackList.Clear();
		OnReset?.Invoke();
	}

	public bool AddAttack(Character.Body bode)
	{
		if (Points == 0)
			return false;

		if(AttackList.ContainsKey(bode))
		{
			++AttackList[bode];
		}
		else
		{
			AttackList.Add(bode, 1);
		}

		--Points;
		return true;
	}

	public bool RemoveAttack(Character.Body bode)
	{
		if (Points == _maxPoints || !AttackList.ContainsKey(bode))
			return false;

		if (AttackList[bode] > 1)
		{
			--AttackList[bode];
		}
		else
		{
			AttackList.Remove(bode);
		}

		++Points;
		return true;
	}
}