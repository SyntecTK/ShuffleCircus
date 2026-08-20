using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArtifactData))]
public class ArtifactDataEditor : Editor
{
    private SerializedProperty effectProperty;
    private List<Type> availableEffectTypes;
    private string[] effectTypeNames;

    private void OnEnable()
    {
        effectProperty = serializedObject.FindProperty("effect");
        CacheEffectTypes();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "m_Script", "effect");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Effect", EditorStyles.boldLabel);

        if (effectProperty == null)
        {
            EditorGUILayout.HelpBox("Could not find SerializeReference field 'effect'.", MessageType.Error);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        if (availableEffectTypes.Count == 0)
        {
            EditorGUILayout.HelpBox("No ArtifactEffect subclasses found.", MessageType.Warning);
            serializedObject.ApplyModifiedProperties();
            return;
        }

        int selectedIndex = GetCurrentTypeIndex();
        int newIndex = EditorGUILayout.Popup("Effect Type", selectedIndex, effectTypeNames);

        if (newIndex != selectedIndex)
        {
            if (newIndex == 0)
            {
                effectProperty.managedReferenceValue = null;
            }
            else
            {
                Type selectedType = availableEffectTypes[newIndex - 1];
                effectProperty.managedReferenceValue = Activator.CreateInstance(selectedType);
            }
        }

        if (effectProperty.managedReferenceValue != null)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(effectProperty, true);
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void CacheEffectTypes()
    {
        availableEffectTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a =>
            {
                try
                {
                    return a.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    return e.Types.Where(t => t != null);
                }
            })
            .Where(t => typeof(ArtifactEffect).IsAssignableFrom(t) && !t.IsAbstract && !t.IsGenericType)
            .OrderBy(t => t.Name)
            .ToList();

        effectTypeNames = new string[availableEffectTypes.Count + 1];
        effectTypeNames[0] = "None";

        for (int i = 0; i < availableEffectTypes.Count; i++)
        {
            effectTypeNames[i + 1] = availableEffectTypes[i].Name;
        }
    }

    private int GetCurrentTypeIndex()
    {
        if (effectProperty.managedReferenceValue == null)
        {
            return 0;
        }

        Type currentType = effectProperty.managedReferenceValue.GetType();
        int index = availableEffectTypes.IndexOf(currentType);
        return index >= 0 ? index + 1 : 0;
    }
}
