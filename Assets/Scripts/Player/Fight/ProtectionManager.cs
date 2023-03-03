using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProtectionManager : MonoBehaviour
{
	[SerializeField]
	private int _maxPoints;
	[SerializeField]
	private TextMeshProUGUI _displayPoints;
	[SerializeField]
	private List<Toggle> _togglesProtectSelect;

	private int _points;

	public int Points
	{
		get => _points;
		set
		{
			_points = value;
			_displayPoints.text = _points.ToString();
		}
	}

	private void Start()
	{
		Points = _maxPoints;
	}

	public HashSet<Character.Body> GetDefList()
	{
		HashSet<Character.Body> defList = new HashSet<Character.Body>();

		foreach(Toggle toggle in _togglesProtectSelect)
		{
			if (toggle.isOn)
			{
				defList.Add(toggle.GetComponent<PartBodyMarker>().PartBody);
			}
		}

		return defList;
	}

	public void UpdatePoints(Toggle toggle)
	{
		if(toggle.isOn)
		{
			if (Points == 0)
				toggle.SetIsOnWithoutNotify(false);
			else
				--Points;
		}
		else
		{
			++Points;
		}
	}

	public void ResetPoints()
	{
		foreach (Toggle toggle in _togglesProtectSelect)
		{
			toggle.SetIsOnWithoutNotify(false);
		}
		Points = _maxPoints;
	}
}