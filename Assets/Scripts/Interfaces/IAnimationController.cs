public interface IAnimationController
{
	public bool IsActive { get; }
	public void CallAttackAnimation(Character.Body partBody);
	public void SetBlockAnimation(bool value);
	public void OnDeactivate();
}