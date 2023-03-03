using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class AttackPointSelector : MonoBehaviour, IPointerDownHandler
{
	[SerializeField]
	private AttackMananger _attacks;
	[SerializeField]
	private TextMeshProUGUI _displayPoints;
	[SerializeField]
	private TextMeshProUGUI _displayPartBody;
	[SerializeField]
	private Character.Body _partBody;

	private int _points = 0;

	public int Points
	{
		get => _points;
		set 
		{
			_points = value;
			_displayPoints.text = Points.ToString();
		}
	}
	public Character.Body PartBody { get => _partBody; set => _partBody = value; }

	private void Start()
	{
		_attacks.OnReset += ResetSelector;
		_displayPartBody.text = PartBody.ToString();
	}

	public void ResetSelector()
	{
		Points = 0;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (Input.GetMouseButton(0))
		{
			if (_attacks.AddAttack(PartBody))
			{
				++Points;
			}
		} 
		else if (Input.GetMouseButton(1) && Points > 0)
		{
			if (_attacks.RemoveAttack(PartBody))
			{
				--Points;
			}
		}
		else return;
	}
}