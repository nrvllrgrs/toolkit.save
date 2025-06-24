using Unity.VisualScripting;

namespace ToolkitEngine.SaveManagement.VisualScripting
{
	[UnitCategory("Variables")]
	[UnitTitle("Set Save Variable")]
	public abstract class SetSaveVariable<T, K> : Unit
		where K : SaveVariable<T>
	{
		#region Fields

		[UnitHeaderInspectable]
		public K variable;

		#endregion

		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter;

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit;

		[DoNotSerialize]
		[PortLabelHidden]
		public ValueInput value { get; protected set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Trigger);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			DefineValuePort();
			Requirement(value, enter);
		}

		protected abstract void DefineValuePort();

		private ControlOutput Trigger(Flow flow)
		{
			variable.value = flow.GetValue<T>(value);
			return exit;
		}

		#endregion
	}
}