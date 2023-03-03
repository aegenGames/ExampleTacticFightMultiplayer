using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializeInterface))]
public class RequireInterfaceDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType == SerializedPropertyType.ObjectReference)
		{
			var requiredAttribute = this.attribute as SerializeInterface; 
			EditorGUI.BeginProperty(position, label, property);

			UnityEngine.Object obj = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(UnityEngine.Object), true);
			if (obj is GameObject g) property.objectReferenceValue = g.GetComponent(requiredAttribute.RequiredType);
			property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, requiredAttribute.RequiredType, true);

			EditorGUI.EndProperty();
		}
		else
		{
			var previousColor = GUI.color;
			GUI.color = Color.red;
			EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));
			GUI.color = previousColor;
		}
	}
}
