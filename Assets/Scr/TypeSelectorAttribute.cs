using System;
using UnityEngine;

public class TypeSelectorAttribute : PropertyAttribute
{
    public readonly Type BaseType;

    public TypeSelectorAttribute(Type baseType)
    {
        BaseType = baseType;
    }
}