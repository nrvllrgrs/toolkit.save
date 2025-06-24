using Unity.VisualScripting;

namespace ToolkitEngine.SaveManagement.VisualScripting
{
	[UnitTitle("Set Save String")]
	public class SetSaveString : SetSaveVariable<string, SaveString>
	{
		protected override void DefineValuePort()
		{
			value = ValueInput(nameof(value), false);
		}
	}
}