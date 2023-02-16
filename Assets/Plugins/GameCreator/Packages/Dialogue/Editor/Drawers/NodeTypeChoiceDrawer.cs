using GameCreator.Editor.Common;
using GameCreator.Runtime.Dialogue;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Dialogue
{
    [CustomPropertyDrawer(typeof(NodeTypeChoice))]
    public class NodeTypeChoiceDrawer : TNodeTypeDrawer
    {
        protected override void SetupBody(SerializedProperty property, VisualElement body)
        {
            SerializedProperty hideUnavailable = property.FindPropertyRelative("m_HideUnavailable");
            SerializedProperty hideVisited = property.FindPropertyRelative("m_HideVisited");
            SerializedProperty skipChoice = property.FindPropertyRelative("m_SkipChoice");
            SerializedProperty shuffleChoices = property.FindPropertyRelative("m_ShuffleChoices");
            SerializedProperty timedChoice = property.FindPropertyRelative("m_TimedChoice");
            
            body.Add(new PropertyTool(hideUnavailable));
            body.Add(new PropertyTool(hideVisited));
            body.Add(new PropertyTool(skipChoice));
            body.Add(new PropertyTool(shuffleChoices));
            body.Add(new PropertyTool(timedChoice));
        }
    }
}