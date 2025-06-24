using UnityEngine.Events;

namespace ToolkitEngine.SaveManagement
{
	public interface IObservableVariable
    {
		UnityEvent<VariableEventArgs> onVariableChanged { get; }

	}
}