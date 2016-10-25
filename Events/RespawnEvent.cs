namespace Pluton.Rust.Events
{
	using Core;
	using Rust.Objects;
	using UnityEngine;

	public class RespawnEvent : Event
	{
		public readonly Player Player;
		public readonly Vector3 SpawnPos;
		public readonly Quaternion SpawnRot;

		public bool GiveDefault = true;
		public bool ChangePos = false;
		public bool WakeUp = false;
		public float StartHealth = -1;

		public RespawnEvent(Player player, Vector3 position, Quaternion quaternion)
		{
			Player = player;
			SpawnPos = position;
			SpawnRot = quaternion;
		}
	}
}
