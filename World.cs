namespace Pluton.Rust
{
	using System.Linq;
	using System.Timers;
    using System.Collections;
    using System.Collections.Generic;
	using Core;
	using UnityEngine;
	using Objects;

	public class World : Singleton<World>, ISingleton
	{
		public float ResourceGatherMultiplier = 1.0f;
		public Timer freezeTimeTimer;

        private float frozenTime = -1;

		public BaseEntity AttachParachute(Player p) => AttachParachute(p.basePlayer);

		public BaseEntity AttachParachute(BaseEntity e)
		{
			BaseEntity parachute = GameManager.server.CreateEntity("assets/prefabs/misc/parachute/parachute.prefab", default(Vector3), default(Quaternion));

			if (parachute) {
				parachute.SetParent(e, "parachute_attach");
				parachute.Spawn();
			}

			return parachute;
		}

		public void AirDrop()
		{
			float speed = Random.Range(30f, 55f);
			float height = Random.Range(900f, 1000f);

			AirDrop(speed, height);
		}

		public void AirDrop(float speed, float height = 400f)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", default(Vector3), default(Quaternion));

			if (baseEntity)
                baseEntity.Spawn();

            CargoPlane cp = baseEntity.GetComponent<CargoPlane>();

            Vector3 start = (Vector3) cp.GetFieldValue("startPos");
            Vector3 end = (Vector3) cp.GetFieldValue("endPos");

			start.y = height;
			end.y = height;

			cp.SetFieldValue("secondsToTake", Vector3.Distance(start, end) / speed);
			cp.SetFieldValue("startPos", start);
			cp.SetFieldValue("endPos", end);
		}

		public void AirDropAt(Vector3 position, float speed = 50f, float height = 400f)
		{
			float worldSize = (global::World.Size - (global::World.Size / 7));
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab", default(Vector3), default(Quaternion));

			if (baseEntity)
                baseEntity.Spawn();

            CargoPlane cp = baseEntity.GetComponent<CargoPlane>();
			Vector3 startPos = Vector3.zero, endPos = Vector3.zero;
		    float rand = (worldSize * Random.Range(0.4f, 1.2f));

			while (startPos.x == 0 || startPos.z == 0)
				startPos = Vector3Ex.Range(-rand, rand);

			startPos.y = height;
			endPos = position + (position - startPos);
			endPos.y = height;

			float secsToTake = Vector3.Distance(startPos, endPos) / speed;

			cp.SetFieldValue("startPos", startPos);
			cp.SetFieldValue("endPos", endPos);
			cp.SetFieldValue("secondsToTake", secsToTake);
			cp.transform.rotation = Quaternion.LookRotation(endPos - startPos);

			baseEntity.Spawn();
		}

		public void AirDropAt(float x, float y, float z, float speed = 50f, float height = 400f) => AirDropAt(new Vector3(x, y, z), speed, height);

		public void AirDropAtPlayer(Player player, float speed = 50f, float height = 400f) => AirDropAt(player.Location, speed, height);

		public Entity PatrolHelicopter(float height = 10f)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", default(Vector3), default(Quaternion), true);

			if (baseEntity) {
				baseEntity.Spawn();

				return new Entity(baseEntity);
			}

			return null;
		}

		public Entity PatrolHelicopterAt(Vector3 position, float height = 10f)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", default(Vector3), default(Quaternion), true);

			if (baseEntity) {
				PatrolHelicopterAI component = baseEntity.GetComponent<PatrolHelicopterAI>();
				component.SetInitialDestination(position + new Vector3(0, height, 0), 0.25f);
				baseEntity.Spawn();

				return new Entity(baseEntity);
			}

			return null;
		}

		public Entity PatrolHelicopterAt(float x, float y, float z, float height = 10f) => PatrolHelicopterAt(new Vector3(x, y, z), height);

		public Entity PatrolHelicopterAtPlayer(Player player, float height = 10f) => PatrolHelicopterAt(player.Location, height);

		public float GetGround(float x, float z)
		{
			RaycastHit hit;
			Vector3 origin = new Vector3(x, 1000f, z);
			float ground = 0f;

			if (Physics.Raycast(origin, Vector3.down, out hit, Vector3.Distance(origin, new Vector3(origin.x, -100f, origin.z)), 1 << 23)) {
				ground = hit.point.y;
			}

			return ground;
		}

		public float GetGround(Vector3 v3) => GetGround(v3.x, v3.z);

		public List<string> GetPrefabNames()
		{
			var pool = (Dictionary<uint, string>) typeof(StringPool).GetStaticFieldValue("toString");
			return (from keyvaluepair in pool
			        orderby keyvaluepair.Value ascending
			        select keyvaluepair.Value).ToList();
		}

		public BaseEntity SpawnMapEntity(string name, float x, float z) => SpawnMapEntity(name, x, GetGround(x, z), z);

		public BaseEntity SpawnMapEntity(string name, Vector3 loc) => SpawnMapEntity(name, loc.x, loc.y, loc.z);

		public BaseEntity SpawnMapEntity(string name, Vector3 loc, Quaternion q) => SpawnMapEntity(name, loc.x, loc.y, loc.z, q);

		public BaseEntity SpawnMapEntity(string name, float x, float y, float z) => SpawnMapEntity(name, x, y, z, Quaternion.identity);

		public BaseEntity SpawnAnimal(string name, float x, float z) => SpawnAnimal(name, x, GetGround(x, z), z);

		public BaseEntity SpawnAnimal(string name, Vector3 loc) => SpawnAnimal(name, loc.x, loc.x, loc.z);

		public BaseEntity SpawnEvent(string evt, float x, float z) => SpawnEffect(evt, x, GetGround(x, z), z);

		public BaseEntity SpawnEvent(string evt, Vector3 loc) => SpawnEffect(evt, loc.x, loc.x, loc.z);    

		// Like sounds, smoke, fire
		public BaseEntity SpawnEffect(string evt, float x, float y, float z)
		{
			BaseEntity ent = GameManager.server.CreateEntity("assets/bundled/prefabs/fx/" + evt + ".prefab", new Vector3(x, y, z), new Quaternion());

			ent.Spawn();

			return ent;
		}

		// Animals: boar, bear, stag, wolf, horse, chicken
		public BaseEntity SpawnAnimal(string name, float x, float y, float z)
		{
			BaseEntity ent = GameManager.server.CreateEntity("assets/bundled/prefabs/autospawn/animals/" + name + ".prefab", new Vector3(x, y, z), new Quaternion());

			ent.Spawn();

			return ent;
		}

		// Map entities, like a resource node, a tree of even a structure
		public BaseEntity SpawnMapEntity(string name, float x, float y, float z, Quaternion q)
		{
			BaseEntity ent = GameManager.server.CreateEntity(name, new Vector3(x, y, z), q);

			ent.SpawnAsMapEntity();

			return ent;
		}

		public float Time {
			get {
				return TOD_Sky.Instance.Cycle.Hour;
			}
			set {
				TOD_Sky.Instance.Cycle.Hour = value;
			}
		}

		public float Timescale {
			get {
                TOD_Components comp = TOD_Sky.Instance.GetComponent<TOD_Components>();
                TOD_Time time = comp.GetComponent<TOD_Time>();

				return time.DayLengthInMinutes;
			}
			set {
                TOD_Components comp = TOD_Sky.Instance.GetComponent<TOD_Components>();
                TOD_Time time = comp.GetComponent<TOD_Time>();

				time.DayLengthInMinutes = value;
			}
		}

		public void FreezeTime()
		{
			if (freezeTimeTimer == null) {
				frozenTime = Time;
				freezeTimeTimer = new Timer(10000);
				freezeTimeTimer.Elapsed += new ElapsedEventHandler(this.Freeze);
			}

			freezeTimeTimer.Start();
		}

		void Freeze(object sender, ElapsedEventArgs e)
		{         
			if (frozenTime != -1)
				Time = frozenTime;
			else
				freezeTimeTimer.Stop();
		}

		public void UnFreezeTime() => frozenTime = -1;

		public void Initialize() { }

		private ArrayList list = new ArrayList();

		public void PrintPrefabs()
		{
			GameManifest.PrefabProperties[] prefabProperties = GameManifest.Get().prefabProperties;

			foreach (var prefabProperty in prefabProperties)
				if (!list.Contains(prefabProperty.name))
					list.Add(prefabProperty.name);

			foreach (object o in list)
				Debug.Log(o);
		}
	}
}
