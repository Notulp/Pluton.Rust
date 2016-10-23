namespace Pluton.Rust.Events {
	using Core;
	using Rust.Objects;
	using UnityEngine;

	public class PlayerTakeRadsEvent : CountedInstance {
		public readonly Player Victim;
		public readonly float Current;
		public readonly float RadAmount;

		public float Next;

		public PlayerTakeRadsEvent(BasePlayer basePlayer, float current, float amount) {
			Victim = Server.GetPlayer(basePlayer);
			Current = current;
			RadAmount = amount;

			float next = Mathf.Clamp(amount,
			                         basePlayer.metabolism.radiation_level.min,
			                         basePlayer.metabolism.radiation_level.max);
			Next = Mathf.Max(current, next);
		}
	}
}
