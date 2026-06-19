#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// 2. Регистрируем наш "рисовальщик" для атрибута ReadOnlyAttribute.
// [CustomPropertyDrawer(typeof(ReadOnlyAttribute))] указывает Unity использовать этот класс
// для отрисовки всех полей с атрибутом [ReadOnly].

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    // 3. Переопределяем метод OnGUI, который отвечает за отрисовку поля.
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Сохраняем текущий режим GUI, чтобы вернуть его после.
        var previousGUIState = GUI.enabled;
        
        // Отключаем взаимодействие с полем (делаем его серым и неактивным).
        GUI.enabled = false;
        
        // Рисуем поле свойства в его стандартном виде, но в отключенном состоянии.
        // EditorGUI.PropertyField — это универсальный метод для отрисовки любого типа поля.
        EditorGUI.PropertyField(position, property, label, true);
        
        // Возвращаем режим GUI в исходное состояние.
        // Это важно, чтобы другие поля на этом же объекте не стали неактивными.
        GUI.enabled = previousGUIState;
    }
}
#endif