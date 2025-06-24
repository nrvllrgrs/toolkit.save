using Unity.VisualScripting;

namespace ToolkitEngine.SaveManagement.VisualScripting
{
	[UnitTitle("Get Save Bool")]
	public class GetSaveBool : GetSaveVariable<bool, SaveBool>
    {
		protected override void DefineValuePort()
		{
			value = ValueOutput(nameof(value), GetValue);
		}
	}
}
