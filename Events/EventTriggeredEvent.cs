namespace Pluton.Rust.Events
{
	using Core;

	public class EventTriggeredEvent : Event
	{
		public GameObjectRef PrefabRef;
		public bool Stop;

		public EventTriggeredEvent(TriggeredEventPrefab triggeredEventPrefab)
		{
			PrefabRef = triggeredEventPrefab.targetPrefab;
		}

		public string Prefab {
			get { return PrefabRef.resourcePath; }
		}
	}
}
