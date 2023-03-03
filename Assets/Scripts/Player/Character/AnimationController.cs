using UnityEngine;
using Mirror;

[RequireComponent(typeof(Animator))]
public class AnimationController : NetworkBehaviour, IAnimationController
{
	[SerializeField]
	private NetworkAnimator _animator;

	private bool _isActive = false;
	public bool IsActive { get => _isActive; }

	public void CallAttackAnimation(Character.Body partBody)
	{
		_isActive = true;
		if (partBody == Character.Body.LH || partBody == Character.Body.LUB ||
		   partBody == Character.Body.RH || partBody == Character.Body.RUB)
		{
			UsePunch();
		}
		else
		{
			UseKick();
		}
	}

	public void SetBlockAnimation(bool value)
	{
		if (value)
			SetBlock();
		else
			RemoveBlock();
	}

	/// <summary>
	/// Invoke in animation
	/// </summary>
	public void OnDeactivate()
	{
		_isActive = false;
	}

	void Start()
	{
		_animator = this.GetComponent<NetworkAnimator>();
	}

	private void UseKick()
	{
		_animator.SetTrigger("KickTrigger");
	}

	private void UsePunch()
	{
		_animator.SetTrigger("PunchTrigger");
	}

	private void SetBlock()
	{
		_animator.animator.SetBool("IsBlocked", true);
	}

	private void RemoveBlock()
	{
		_animator.animator.SetBool("IsBlocked", false);
	}
}