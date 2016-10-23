﻿namespace Pluton.Rust.Events {
	using Rust.Objects;

	public class PlayerDeathEvent : DeathEvent {
		public readonly Player Victim;

		public PlayerDeathEvent(Player player, HitInfo info)
			: base(info) {
			Victim = player;
		}
	}
}
