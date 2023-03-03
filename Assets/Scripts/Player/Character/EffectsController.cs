using UnityEngine;
using Mirror;

public class EffectsController : MonoBehaviour
{
	[SerializeField]
	private GameObject _punchPrefab;
	[SerializeField]
	private GameObject _kickPrefab;

	[ClientCallback]
	public void UsePunchEffect()
	{
		SpawnEffect(_punchPrefab);
	}

	[ClientCallback]
	public void UseKickEffect()
	{
		SpawnEffect(_kickPrefab);
	}

	private void SpawnEffect(GameObject effect)
	{
		GameObject spawnedEffect = Instantiate(effect, effect.transform.position, Quaternion.identity);
		spawnedEffect.transform.LookAt(Camera.main.transform);
		spawnedEffect.SetActive(true);
	}
}