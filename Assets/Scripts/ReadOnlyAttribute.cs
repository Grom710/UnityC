using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field |
					   System.AttributeTargets.Property |
					   System.AttributeTargets.Struct,
					   AllowMultiple = false, Inherited = true)]
public class ReadOnlyAttribute : PropertyAttribute
{

}