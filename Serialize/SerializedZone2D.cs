namespace Pluton.Rust.Serialize
{
	using Objects;
	using Core.Serialize;
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using UnityEngine;
	using Logger = Core.Logger;

	public class SerializedZone2D : ISerializable
	{
		public string Name;
		public int[] Tris;
		public int TrisCount;
		public List<SerializedVector3> Verts;

		public Zone2D ToZone2D()
		{
			try {
				GameObject obj = new GameObject(Name);
				GameObject gobj = UnityEngine.Object.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
				Zone2D zone = gobj.AddComponent<Zone2D>();

				zone.Name = Name;
				zone.Tris = Tris;
				zone.TrisCount = TrisCount;
				zone.Verts = Verts.Select(x => x.ToVector3()).ToList();

				Core.Singleton<Util>.GetInstance().zones.Add(Name, zone);

				return zone;
			} catch (Exception ex) {
				Logger.LogException(ex);
				return null;
			}
		}

		public object Deserialize()
		{
			return null;
		}
	}
}
