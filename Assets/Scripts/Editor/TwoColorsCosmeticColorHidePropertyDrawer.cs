using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(TwoColorsCosmeticColorHideAttribute))]
[Obsolete("Not used any more", true)]
public class TwoColorsCosmeticColorHidePropertyDrawer : PropertyDrawer
{
    /*
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        TwoColorsCosmeticColorHideAttribute condHAtt = (TwoColorsCosmeticColorHideAttribute)attribute;
        bool enabled = GetTwoColorsCosmeticColorHideAttributeResult(condHAtt, property);

        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (enabled)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        GUI.enabled = wasEnabled;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        TwoColorsCosmeticColorHideAttribute condHAtt = (TwoColorsCosmeticColorHideAttribute)attribute;
        bool enabled = GetTwoColorsCosmeticColorHideAttributeResult(condHAtt, property);

        if (enabled)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }

    private bool GetTwoColorsCosmeticColorHideAttributeResult(TwoColorsCosmeticColorHideAttribute attr, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath;
        string conditionPath = propertyPath.Replace(property.name, attr.cosmeticItemInformationSourceField);
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        if (sourcePropertyValue != null)
        {
            CharacterCosmeticItemInformation item = (CharacterCosmeticItemInformation)(sourcePropertyValue.objectReferenceValue);

            if (item == null)
            {
                enabled = false;
            }
            else
            {
                if (attr.primary)
                {
                    enabled = item.HasPrimaryColor;
                }
                else
                {
                    enabled = item.HasSecondaryColor;
                }
            }
        }
        else
        {
            Debug.LogWarning("Attempting to use a TwoColorsCosmeticColorHideAttribute but no matching SourcePropertyValue found in object: " + attr.cosmeticItemInformationSourceField);
        }

        return enabled;
    }
    */
}
