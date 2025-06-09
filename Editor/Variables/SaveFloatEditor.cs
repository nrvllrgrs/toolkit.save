using ToolkitEngine.SaveManagement;
using UnityEditor;

namespace ToolkitEditor.SaveManagement
{
	[CustomPropertyDrawer(typeof(SaveFloat))]
	public class SaveFloatEditor : SaveVariableDrawer
	{
		protected override bool IsValid(SaveDefinition definition)
		{
			return definition is FloatDefinition;
		}
	}
}