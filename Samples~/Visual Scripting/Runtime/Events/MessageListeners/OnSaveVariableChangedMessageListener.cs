using Unity.VisualScripting;
using UnityEngine;

namespace ToolkitEngine.SaveManagement.VisualScripting
{
	[AddComponentMenu("")]
	public class OnSaveVariableChangedMessageListener : MessageListener
    {
		private void Start() => GetComponent<IObservableVariable>()?.onVariableChanged.AddListener((value) =>
		{
			EventBus.Trigger(nameof(OnSaveVariableChanged), gameObject, value);
		});
	}
}