using SurvivorsEscape;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(SMBEvent))]
public class EditorSMBEvent : Editor
{
    private SerializedProperty _totalFrames;
    private SerializedProperty _currentFrame;
    private SerializedProperty _normalizedTime;
    private SerializedProperty _normalizedTimeUncapped;
    private SerializedProperty _motionTime;
    private SerializedProperty _Events;

    private ReorderableList _EventList;

    private void OnEnable()
    {
        _totalFrames = serializedObject.FindProperty("_totalFrames");
        _currentFrame = serializedObject.FindProperty("_currentFrame");
        _normalizedTime = serializedObject.FindProperty("_normalizedTime");
        _normalizedTimeUncapped = serializedObject.FindProperty("_normalizedTimeUncapped");
        _motionTime = serializedObject.FindProperty("_motionTime");
        _Events = serializedObject.FindProperty("_Events");

        _EventList = new ReorderableList(serializedObject, _Events, true, true, true, true);

        _EventList.drawHeaderCallback = DrawHeaderCallback;
        _EventList.drawElementCallback = DrawElementCallback;
        _EventList.elementHeightCallback = ElementHeightCallback;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        
        using (new EditorGUI.DisabledGroupScope(true))
        {
            EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((SMBEvent)target), typeof(SMBEvent), false);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_totalFrames);
            EditorGUILayout.PropertyField(_currentFrame);
            EditorGUILayout.PropertyField(_normalizedTime);
            EditorGUILayout.PropertyField(_normalizedTimeUncapped);
        }

        EditorGUILayout.PropertyField(_motionTime);
        
        _EventList.DoLayoutList();
        

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawHeaderCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "Events");
    }

    private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = _EventList.serializedProperty.GetArrayElementAtIndex(index);
        SerializedProperty eventName = element.FindPropertyRelative("_name");
        SerializedProperty timing = element.FindPropertyRelative("_timing");

        string elementTitle;
        int timingIndex = timing.enumValueIndex;
        elementTitle = string.IsNullOrEmpty(eventName.stringValue) ?
            $"Event *Name* ({timing.enumDisplayNames[timingIndex]})" :
            $"Event {eventName.stringValue} ({timing.enumDisplayNames[timingIndex]})";

        using (new EditorGUI.IndentLevelScope(1))
        {
            EditorGUI.PropertyField(rect, element, new GUIContent(elementTitle), true);
        }
            
    }

    private float ElementHeightCallback(int index)
    {
        SerializedProperty element = _EventList.serializedProperty.GetArrayElementAtIndex(index);
        float propertyHeight = EditorGUI.GetPropertyHeight(element, true);

        return propertyHeight;
    }
}
