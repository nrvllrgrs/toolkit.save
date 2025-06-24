using Unity.VisualScripting;

namespace ToolkitEngine.SaveManagement.VisualScripting
{
	[UnitTitle("Get Save Float")]
	public class GetSaveFloat : GetSaveVariable<float, SaveFloat>
	{
		protected override void DefineValuePort()
		{
			value = ValueOutput(nameof(value), GetValue);
		}
	}
}
