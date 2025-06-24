using Unity.VisualScripting;

namespace ToolkitEngine.SaveManagement.VisualScripting
{
	[UnitTitle("Set Save Float")]
	public class SetSaveFloat : SetSaveVariable<float, SaveFloat>
	{
		protected override void DefineValuePort()
		{
			value = ValueInput(nameof(value), false);
		}
	}
}