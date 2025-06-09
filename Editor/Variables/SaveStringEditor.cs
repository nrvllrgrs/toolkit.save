using ToolkitEngine.SaveManagement;
using UnityEditor;

namespace ToolkitEditor.SaveManagement
{
	[CustomPropertyDrawer(typeof(SaveString))]
	public class SaveStringEditor : SaveVariableDrawer
	{
		protected override bool IsValid(SaveDefinition definition)
		{
			return definition is StringDefinition;
		}
	}
}