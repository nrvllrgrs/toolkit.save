using ToolkitEngine.SaveManagement;
using UnityEditor;

namespace ToolkitEditor.SaveManagement
{
	[CustomPropertyDrawer(typeof(SaveInt))]
	public class SaveIntEditor : SaveVariableEditor
	{
		protected override bool IsValid(SaveDefinition definition)
		{
			return definition is IntDefinition;
		}
	}
}