namespace Pluton.Rust.Events
{
	using Core;

	public class EventTriggeredEvent : CountedInstance
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
