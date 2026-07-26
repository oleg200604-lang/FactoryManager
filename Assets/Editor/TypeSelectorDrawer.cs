using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TypeSelectorAttribute))]
public class TypeSelectorDrawer : PropertyDrawer
{
    private static readonly Dictionary<Type, Type[]> typeCache = new Dictionary<Type, Type[]>();
    private static readonly Dictionary<Type, string[]> nameCache = new Dictionary<Type, string[]>();

    private static Type[] GetAvailableTypes(Type baseType)
    {
        if (typeCache.TryGetValue(baseType, out Type[] cached))
            return cached;

        Type[] result = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(SafeGetTypes)
            .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .OrderBy(t => t.Name)
            .ToArray();

        typeCache[baseType] = result;
        return result;
    }

    private static Type[] SafeGetTypes(Assembly assembly)
    {
        try { return assembly.GetTypes(); }
        catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null).ToArray(); }
    }

    private static string[] GetDisplayNames(Type baseType, Type[] availableTypes)
    {
        if (nameCache.TryGetValue(baseType, out string[] cached))
            return cached;

        string[] names = new string[availableTypes.Length + 1];
        names[0] = "(нічого)";

        for (int i = 0; i < availableTypes.Length; i++)
            names[i + 1] = availableTypes[i].Name;

        nameCache[baseType] = names;
        return names;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ManagedReference)
        {
            EditorGUI.LabelField(position, label.text, "Потрібен [SerializeReference] для цього поля.");
            return;
        }

        TypeSelectorAttribute selector = (TypeSelectorAttribute)attribute;

        Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        Type[] availableTypes = GetAvailableTypes(selector.BaseType);
        string[] displayNames = GetDisplayNames(selector.BaseType, availableTypes);

        string currentTypeName = property.managedReferenceFullTypename;
        int currentIndex = 0;

        if (!string.IsNullOrEmpty(currentTypeName))
        {
            for (int i = 0; i < availableTypes.Length; i++)
            {
                if (currentTypeName.EndsWith(availableTypes[i].FullName))
                {
                    currentIndex = i + 1;
                    break;
                }
            }
        }

        EditorGUI.BeginChangeCheck();
        int selectedIndex = EditorGUI.Popup(dropdownRect, label.text, currentIndex, displayNames);

        if (EditorGUI.EndChangeCheck() && selectedIndex != currentIndex)
        {
            property.managedReferenceValue = selectedIndex == 0 ? null : Activator.CreateInstance(availableTypes[selectedIndex - 1]);
        }

        if (property.managedReferenceValue != null)
        {
            Rect fieldsRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width,
                EditorGUI.GetPropertyHeight(property, true) - EditorGUIUtility.singleLineHeight - 2);

            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(fieldsRect, property, GUIContent.none, true);
            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + 2;

        if (property.propertyType == SerializedPropertyType.ManagedReference && property.managedReferenceValue != null)
        {
            height += EditorGUI.GetPropertyHeight(property, true);
        }

        return height;
    }
}