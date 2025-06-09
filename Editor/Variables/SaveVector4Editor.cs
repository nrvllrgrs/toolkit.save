using ToolkitEngine.SaveManagement;
using UnityEditor;

namespace ToolkitEditor.SaveManagement
{
	[CustomPropertyDrawer(typeof(SaveVector4))]
	public class SaveVector4Editor : SaveVariableEditor
	{
		protected override bool IsValid(SaveDefinition definition)
		{
			return definition is Vector4Definition;
		}
	}
}