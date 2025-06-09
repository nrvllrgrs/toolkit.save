using ToolkitEngine.SaveManagement;
using UnityEditor;

namespace ToolkitEditor.SaveManagement
{
	[CustomPropertyDrawer(typeof(SaveVector3Int))]
	public class SaveVector3IntEditor : SaveVariableEditor
	{
		protected override bool IsValid(SaveDefinition definition)
		{
			return definition is Vector3IntDefinition;
		}
	}
}