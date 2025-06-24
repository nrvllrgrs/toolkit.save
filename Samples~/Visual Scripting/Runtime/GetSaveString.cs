using Unity.VisualScripting;

namespace ToolkitEngine.SaveManagement.VisualScripting
{
	[UnitTitle("Get Save String")]
	public class GetSaveString : GetSaveVariable<string, SaveString>
	{
		protected override void DefineValuePort()
		{
			value = ValueOutput(nameof(value), GetValue);
		}
	}
}
