namespace Pluton.Rust.Objects
{
	using System;
	using Core;

    [Serializable]
    public class PlayerStats : CountedInstance
    {

        public uint Kills;
        public uint Deaths;

        public uint PlayerKills;
        public uint PlayerDeaths;

        public uint NPCKills;
        public uint NPCDeaths;

        public double TotalDamageTaken;
        public double TotalDamageDone;

        public double DamageToPlayers;
        public double DamageFromPlayers;

        public double DamageToNPCs;
        public double DamageFromNPCs;

        public double DamageToEntities;

        public double FallDamage;

        public PlayerStats(string steamid)
        {
            if (!Server.GetInstance().serverData.ContainsKey("PlayerStats", steamid)) {
                Kills = 0;
                Deaths = 0;
                PlayerKills = 0;
                PlayerDeaths = 0;
                NPCKills = 0;
                NPCDeaths = 0;
                TotalDamageTaken = 0;
                TotalDamageDone = 0;
                DamageToPlayers = 0;
                DamageFromPlayers = 0;
                DamageToNPCs = 0;
                DamageFromNPCs = 0;
                DamageToEntities = 0;
                FallDamage = 0;
                Server.GetInstance().serverData.Add("PlayerStats", steamid, this);
            } else {
                var stats = Server.GetInstance().serverData.Get("PlayerStats", steamid) as PlayerStats;
                Kills = stats.Kills;
                Deaths = stats.Deaths;
                PlayerKills = stats.PlayerKills;
                PlayerDeaths = stats.PlayerDeaths;
                NPCKills = stats.NPCKills;
                NPCDeaths = stats.NPCDeaths;
                TotalDamageTaken = stats.TotalDamageTaken;
                TotalDamageDone = stats.TotalDamageDone;
                DamageToPlayers = stats.DamageToPlayers;
                DamageFromPlayers = stats.DamageFromPlayers;
                DamageToNPCs = stats.DamageToNPCs;
                DamageFromNPCs = stats.DamageFromNPCs;
                DamageToEntities = stats.DamageToEntities;
                FallDamage = stats.FallDamage;
            }
        }

        public void AddKill(bool player, bool npc)
        {
            Kills++;
            if (player)
                PlayerKills++;
            else if (npc)
                NPCKills++;
        }

        public void AddDeath(bool player, bool npc)
        {
            Deaths++;
            if (player)
                PlayerDeaths++;
            else if (npc)
                NPCDeaths++;
        }

        public void AddDamageFrom(float dmgAmount, bool player, bool npc, bool fall)
        {
            TotalDamageTaken += dmgAmount;
            if (player)
                DamageFromPlayers += dmgAmount;
            else if (npc)
                DamageFromNPCs += dmgAmount;
            else if (fall)
                FallDamage += dmgAmount;
        }

        public void AddDamageTo(float dmgAmount, bool player, bool npc, bool entity)
        {
            TotalDamageDone += dmgAmount;
            if (player)
                DamageToPlayers += dmgAmount;
            else if (npc)
                DamageToNPCs += dmgAmount;
            else if (entity)
                DamageToEntities += dmgAmount;
        }
    }
}

