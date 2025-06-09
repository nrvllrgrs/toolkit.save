using ToolkitEngine.SaveManagement;
using UnityEditor;

namespace ToolkitEditor.SaveManagement
{
	[CustomPropertyDrawer(typeof(SaveVector2Int))]
	public class SaveVector2IntEditor : SaveVariableEditor
	{
		protected override bool IsValid(SaveDefinition definition)
		{
			return definition is Vector2IntDefinition;
		}
	}
}