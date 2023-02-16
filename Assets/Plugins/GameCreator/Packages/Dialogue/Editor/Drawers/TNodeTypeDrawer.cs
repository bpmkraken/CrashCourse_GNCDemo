using GameCreator.Editor.Common;
using GameCreator.Runtime.Dialogue;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Dialogue
{
    public abstract class TNodeTypeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement head = new VisualElement();
            VisualElement body = new VisualElement();

            root.Add(head);
            root.Add(body);

            SerializedProperty options = property.FindPropertyRelative("m_Options");
            PropertyTool fieldOptions = new PropertyTool(options);

            head.Add(fieldOptions);
            this.SetupBody(property, body);

            fieldOptions.EventChange += changeEvent =>
            {
                int index = changeEvent.changedProperty.enumValueIndex;
                body.style.display = index == (int) NodeTypeData.FromNode
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            };
            
            body.style.display = options.enumValueIndex == (int) NodeTypeData.FromNode
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            return root;
        }
        
        protected abstract void SetupBody(SerializedProperty property, VisualElement body);
    }
}