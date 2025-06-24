using Unity.VisualScripting;

namespace ToolkitEngine.SaveManagement.VisualScripting
{
	[UnitTitle("Set Save Bool")]
	public class SetSaveBool : SetSaveVariable<bool, SaveBool>
    {
		protected override void DefineValuePort()
		{
			value = ValueInput(nameof(value), false);
		}
	}
}