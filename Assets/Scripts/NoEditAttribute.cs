using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Атрибут, для блокировки модификации сериализуемых полей через инспектор
/// </summary>
public class NoEditAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(NoEditAttribute))]
public class NoEditPropertyDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        UnityEditor.EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }

    public override VisualElement CreatePropertyGUI(UnityEditor.SerializedProperty property)
    {
        var element = base.CreatePropertyGUI(property) ??
            new UnityEditor.UIElements.PropertyField(property);
        element.SetEnabled(false);
        return element;
    }
}
#endif