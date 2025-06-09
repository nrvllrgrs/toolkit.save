using ToolkitEngine.SaveManagement;
using UnityEditor;

namespace ToolkitEditor.SaveManagement
{
	[CustomPropertyDrawer(typeof(SaveVector3))]
	public class SaveVector3Editor : SaveVariableDrawer
	{
		protected override bool IsValid(SaveDefinition definition)
		{
			return definition is Vector3Definition;
		}
	}
}