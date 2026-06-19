using UnityEngine;

// 1. Определяем атрибут. 
// Наследуемся от PropertyAttribute, чтобы Unity распознала его как атрибут для инспектора.
// [System.AttributeUsage] разрешает применять атрибут к полям, свойствам и структурам.

[System.AttributeUsage(System.AttributeTargets.Field |
					   System.AttributeTargets.Property |
					   System.AttributeTargets.Struct,
					   AllowMultiple = false, Inherited = true)]
public class ReadOnlyAttribute : PropertyAttribute
{
	// Класс пуст, так как его единственная задача — быть маркером-меткой.
}