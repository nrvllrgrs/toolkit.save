using System;
using ToolkitEngine.VisualScripting;
using Unity.VisualScripting;

namespace ToolkitEngine.SaveManagement.VisualScripting
{
	public enum VariableType
	{
		Bool,
		Float,
		String,
	};

	[UnitCategory("Events/Variables")]
	[UnitTitle("On Save Variable Changed")]
	public class OnSaveVariableChanged : TargetEventUnit<VariableEventArgs>
	{
		#region Fields

		[UnitHeaderInspectable("Variable")]
		public VariableType variableType;

		private VariableEventArgs m_eventArgs = null;

		#endregion

		#region Ports

		[DoNotSerialize]
		[PortLabelHidden]
		public ValueOutput boolValue { get; private set; }

		[DoNotSerialize]
		[PortLabelHidden]
		public ValueOutput floatValue { get; private set; }

		[DoNotSerialize]
		[PortLabelHidden]
		public ValueOutput stringValue { get; private set; }

		#endregion

		#region Properties

		protected override bool showEventArgs => false;

		public override Type MessageListenerType => typeof(OnSaveVariableChangedMessageListener);

		#endregion

		#region Methods

		protected override void Definition()
		{
			base.Definition();

			switch (variableType)
			{
				case VariableType.Bool:
					boolValue = ValueOutput(nameof(boolValue), GetValue<bool>);
					break;

				case VariableType.Float:
					floatValue = ValueOutput(nameof(floatValue), GetValue<float>);
					break;

				case VariableType.String:
					stringValue = ValueOutput(nameof(stringValue), GetValue<string>);
					break;
			}
		}

		private T GetValue<T>(Flow flow)
		{
			return m_eventArgs != null && m_eventArgs.value is T value
				? value
				: default;
		}

		protected override void AssignArguments(Flow flow, VariableEventArgs args)
		{
			m_eventArgs = args;
		}

		protected override void StartListeningToManager()
		{
			SaveManager.CastInstance.VariableChanged += InvokeTrigger;
		}

		protected override void StopListeningToManager()
		{
			SaveManager.CastInstance.VariableChanged -= InvokeTrigger;
		}

		#endregion
	}
}