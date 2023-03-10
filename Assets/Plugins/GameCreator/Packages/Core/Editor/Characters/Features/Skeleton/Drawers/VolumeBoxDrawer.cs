using GameCreator.Editor.Common;
using GameCreator.Runtime.Characters;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(VolumeBox))]
    public class VolumeBoxDrawer : VolumeDrawer
    {
        protected override void CreateGUI(SerializedProperty property, VisualElement root)
        {
            SerializedProperty center = property.FindPropertyRelative("m_Center");
            SerializedProperty size = property.FindPropertyRelative("m_Size");

            PropertyTool fieldCenter = new PropertyTool(center);
            PropertyTool fieldSize = new PropertyTool(size);

            root.Add(fieldCenter);
            root.Add(fieldSize);
        }
    }
}