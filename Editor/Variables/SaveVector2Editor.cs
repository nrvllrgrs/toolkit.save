using ToolkitEngine.SaveManagement;
using UnityEditor;

namespace ToolkitEditor.SaveManagement
{
	[CustomPropertyDrawer(typeof(SaveVector2))]
	public class SaveVector2Editor : SaveVariableDrawer
	{
		protected override bool IsValid(SaveDefinition definition)
		{
			return definition is Vector2Definition;
		}
	}
}