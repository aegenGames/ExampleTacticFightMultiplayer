using UnityEngine;
using TMPro;

public class PartBodyMarker : MonoBehaviour
{
	[SerializeField]
	private Character.Body _partBody;
	[SerializeField]
	private TextMeshProUGUI _displayPartBody;

	public Character.Body PartBody { get => _partBody; }

	private void Start()
	{
		_displayPartBody.text = _partBody.ToString();
	}
}