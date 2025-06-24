using Unity.VisualScripting;

namespace ToolkitEngine.SaveManagement.VisualScripting
{
	[UnitCategory("Variables")]
    public abstract class GetSaveVariable<T, K> : Unit
		where K : SaveVariable<T>
    {
		#region Fields

		[UnitHeaderInspectable]
		public K variable;

		private VariableEventArgs m_eventArgs;

		#endregion

		#region Ports

		[DoNotSerialize, PortLabelHidden]
		public ControlInput enter;

		[DoNotSerialize, PortLabelHidden]
		public ControlOutput exit;

		[DoNotSerialize]
		[PortLabelHidden]
		public ValueOutput value { get; protected set; }

		#endregion

		#region Methods

		protected override void Definition()
		{
			enter = ControlInput(nameof(enter), Trigger);
			exit = ControlOutput(nameof(exit));
			Succession(enter, exit);

			DefineValuePort();
		}

		protected abstract void DefineValuePort();

		private ControlOutput Trigger(Flow flow)
		{
			return exit;
		}

		protected T GetValue(Flow flow)
		{
			return variable.isDefined
				? variable.value
				: default;
		}

		#endregion
	}
}