using UnityEngine;

public class SerializeInterface : PropertyAttribute
{
	public System.Type RequiredType { get; private set; }
	public SerializeInterface(System.Type type)
	{
		this.RequiredType = type;
	}
}