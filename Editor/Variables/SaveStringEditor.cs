using ToolkitEngine.SaveManagement;
using UnityEditor;

namespace ToolkitEditor.SaveManagement
{
	[CustomPropertyDrawer(typeof(SaveString))]
	public class SaveStringEditor : SaveVariableEditor
	{
		protected override bool IsValid(SaveDefinition definition)
		{
			return definition is StringDefinition;
		}
	}
}