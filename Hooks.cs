namespace Pluton.Rust
{
    using System;
    using System.Linq;
    using System.Globalization;
    using System.Collections.Generic;
    using Network;
    using UnityEngine;
    using ProjectileShoot = ProtoBuf.ProjectileShoot;

    using Events;
    using Core;
    using Core.PluginLoaders;
    using Objects;
    using Logger = Core.Logger;
    using Facepunch.Steamworks;

    public class Hooks : Core.Hooks
    {
        internal static List<string> hookNames = new List<string>() {
            "On_BeingHammered",
            "On_BuildingComplete",
            "On_BuildingPartDemolished",
            "On_BuildingPartDestroyed",
            "Pre_BuildingUpgrade",
            "On_BuildingUpgrade",
            "On_Chat",
            "On_ClientAuth",
            "Pre_ClientAuth",
            "On_ClientConsole",
            "On_CombatEntityHurt",
            "On_Command",
            "On_CommandPermission",
            "On_ConsumeFuel",
            "On_CorpseHurt",
            "Pre_DoorCodeEntered",
            "On_DoorCodeEntered",
            "Pre_DoorUse",
            "On_DoorUse",
            "On_EventTriggered",
            "On_ItemAdded",
            "On_ItemLoseCondition",
            "On_ItemPickup",
            "On_ItemRemoved",
            "On_ItemRepaired",
            "On_ItemUsed",
            "On_LandmineArmed",
            "On_LandmineExploded",
            "Pre_LandmineTriggered",
            "On_LandmineTriggered",
            "On_LootingEntity",
            "On_LootingItem",
            "On_LootingPlayer",
            "On_NetworkableKill",
            "On_NPCHurt",
            "On_NPCKilled",
            "On_Placement",
            "On_PlayerAssisted",
            "On_PlayerClothingChanged",
            "On_PlayerConnected",
            "On_PlayerDied",
            "On_PlayerDisconnected",
            "On_PlayerGathering",
            "On_PlayerHurt",
            "On_PlayerLoaded",
            "On_PlayerRespawn",
            "On_PlayerShoot",
            "On_PlayerSleep",
            "On_PlayerStartCrafting",
            "On_PlayerSyringeOther",
            "Pre_PlayerSyringeOther",
            "On_PlayerSyringeSelf",
            "Pre_PlayerSyringeSelf",
            "On_PlayerHealthChange",
            "On_PlayerTakeRadiation",
            "On_PlayerThrowCharge",
            "On_PlayerThrowGrenade",
            "On_PlayerThrowSignal",
            "Pre_PlayerThrowWeapon",
            "On_PlayerThrowWeapon",
            "On_PlayerWakeUp",
            "On_PlayerWounded",
            "On_QuarryMining",
            "On_PlayerShootRocket",
            "On_ServerConsole",
            "On_ServerInit",
            "On_ServerSaved",
            "On_ServerShutdown",
        };

        new public void Initialize()
        {
            SetInstance<Hooks>();
            HookNames.AddRange(hookNames);
            CreateOrUpdateSubjects();
            Loaded = true;
        }

        #region General Hooks

        /// <summary>
        /// Called from <c>Hammer.DoAttackShared(HitInfo)</c> .
        /// </summary>
        public static void On_BeingHammered(HitInfo info, BasePlayer ownerPlayer) => OnNext("On_BeingHammered", new HammerEvent(info, ownerPlayer));

        /// <summary>
        /// Called from <c>BaseCombatEntity.Hurt(HitInfo)</c> .
        /// </summary>
        public static void On_CombatEntityHurt(BaseCombatEntity combatEntity, HitInfo info)
        {
            try {
                Assert.Test(combatEntity.isServer, "This should be called serverside only");

                if (combatEntity.IsDead())
                    return;

                using (TimeWarning.New("Hurt", 50)) {
                    BaseNPC baseNPC = combatEntity.GetComponent<BaseNPC>();
                    BaseCorpse baseCorpse = combatEntity.GetComponent<BaseCorpse>();
                    BasePlayer basePlayer = combatEntity.GetComponent<BasePlayer>();

                    combatEntity.ScaleDamage(info);

                    if (basePlayer != null) {
                        Player player = Server.GetPlayer(basePlayer);

                        if (player.Teleporting) {
                            for (int i = 0; i < info.damageTypes.types.Length; i++) {
                                info.damageTypes.types[i] = 0f;
                            }
                        }

                        HurtEvent he = new PlayerHurtEvent(player, info);
                        OnNext("On_PlayerHurt", he);
                    } else if (baseNPC != null) {
                            HurtEvent he = new NPCHurtEvent(new NPC(baseNPC), info);
                            OnNext("On_NPCHurt", he);
                        } else if (baseCorpse != null) {
                                HurtEvent he = new CorpseHurtEvent(baseCorpse, info);
                                OnNext("On_CorpseHurt", he);
                            } else {
                                HurtEvent he = new CombatEntityHurtEvent(combatEntity, info);
                                OnNext("On_CombatEntityHurt", he);
                            }

                    if (info.PointStart != Vector3.zero) {
                        DirectionProperties[] directionProperties = (DirectionProperties[])combatEntity.GetFieldValue("propDirection");

                        for (int i = 0; i < directionProperties.Length; i++) {
                            if (!(directionProperties[i].extraProtection == null)) {
                                if (directionProperties[i].IsWeakspot(combatEntity.transform, info)) {
                                    directionProperties[i].extraProtection.Scale(info.damageTypes);
                                }
                            }
                        }
                    }

                    // the DebugHurt() method
                    if (ConVar.Vis.attack) {
                        if (info.PointStart != info.PointEnd) {
                            ConsoleNetwork.BroadcastToAllClients("ddraw.arrow", new object[] {
                                60, UnityEngine.Color.cyan, info.PointStart, info.PointEnd, 0.1
                            });

                            ConsoleNetwork.BroadcastToAllClients("ddraw.sphere", new object[] {
                                60, UnityEngine.Color.cyan, info.HitPositionWorld, 0.05
                            });
                        }

                        string text = String.Empty;

                        for (int i = 0; i < info.damageTypes.types.Length; i++) {
                            float num = info.damageTypes.types[i];

                            if (num != 0) {
                                string text2 = text;
                                text = String.Concat(new string[] {
                                    text2, " ", ((global::Rust.DamageType)i).ToString().PadRight(10), num.ToString("0.00"), "\r\n"
                                });
                            }
                        }

                        string text3 = String.Concat(new object[] {
                            "<color=lightblue>Damage:</color>".PadRight(10),
                            info.damageTypes.Total().ToString("0.00"),
                            "\r\n<color=lightblue>Health:</color>".PadRight(10),
                            combatEntity.health.ToString("0.00"), " / ",
                            (combatEntity.health - info.damageTypes.Total() > 0) ? "<color=green>" : "<color=red>",
                            (combatEntity.health - info.damageTypes.Total()).ToString("0.00"), "</color>",
                            "\r\n<color=lightblue>Hit Ent:</color>".PadRight(10), combatEntity,
                            "\r\n<color=lightblue>Attacker:</color>".PadRight(10), info.Initiator,
                            "\r\n<color=lightblue>Weapon:</color>".PadRight(10), info.Weapon,
                            "\r\n<color=lightblue>Damages:</color>\r\n", text
                        });

                        ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[] {
                            60, UnityEngine.Color.white, info.HitPositionWorld, text3
                        });
                    }

                    combatEntity.health -= info.damageTypes.Total();
                    combatEntity.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);

                    if (ConVar.Global.developer > 1) {
                        Debug.Log(string.Concat(new object[] {
                            "[Combat]".PadRight(10),
                            combatEntity.gameObject.name,
                            " hurt ",
                            info.damageTypes.GetMajorityDamageType(),
                            "/",
                            info.damageTypes.Total(),
                            " - ",
                            combatEntity.health.ToString("0"),
                            " health left"
                        }));
                    }

                    combatEntity.lastDamage = info.damageTypes.GetMajorityDamageType();
                    combatEntity.lastAttacker = info.Initiator;

                    BaseCombatEntity baseCombatEntity = combatEntity.lastAttacker as BaseCombatEntity;

                    if (baseCombatEntity != null)
                        baseCombatEntity.MarkHostileTime();

                    combatEntity.SetFieldValue("_lastAttackedTime", Time.time);

                    if (combatEntity.health <= 0f) {
                        combatEntity.Die(info);

                        BuildingBlock bb = combatEntity.GetComponent<BuildingBlock>();

                        if (bb != null) {
                            OnNext("On_BuildingPartDestroyed", new BuildingPartDestroyedEvent(bb, info));
                        }
                    }
                }
            } catch (Exception ex) {
                Logger.LogError("[Hooks] Error in CombatEntityHurt hook.");
                Logger.LogException(ex);
            }
        }

        /// <summary>
        /// Called from <c>BaseOven.ConsumeFuel(Item, ItemModBurnable)</c> .
        /// </summary>
        public static void On_ConsumeFuel(BaseOven bo, Item fuel, ItemModBurnable burn) => OnNext("On_ConsumeFuel", new ConsumeFuelEvent(bo, fuel, burn));

        /// <summary>
        /// Called from <c>nothing</c> .
        /// </summary>
        public static void On_EventTriggered(TriggeredEventPrefab tep)
        {
            var ete = new EventTriggeredEvent(tep);

            OnNext("On_EventTriggered", ete);

            if (ete.Stop)
                return;

            Debug.Log("[event] " + ete.Prefab);

            BaseEntity baseEntity = GameManager.server.CreateEntity(ete.Prefab);

            if (baseEntity)
                baseEntity.Spawn();
        }

        /// <summary>
        /// Called from <c>BaseNetworkable.Kill(BaseNetworkable.DestroyMode)</c> .
        /// </summary>
        public static void On_NetworkableKill(BaseNetworkable bn) => OnNext("On_NetworkableKill", bn);

        /// <summary>
        /// Called from <c>MiningQuarry.ProcessResources()</c> .
        /// </summary>
        public static void On_QuarryMining(MiningQuarry mq) => OnNext("On_QuarryMining", mq);

        #endregion

        #region Chat Hooks

        /// <summary>
        /// Called from <c>ConVar.Chat.say(ConsoleSystem.Arg)</c> .
        /// </summary>
        public static void On_Chat(ConsoleSystem.Arg arg)
        {
            if (arg.ArgsStr.StartsWith("\"/") && !arg.ArgsStr.StartsWith("\"/ ")) {
                On_Command(arg);
                return;
            }

            if (!ConVar.Chat.enabled) {
                arg.ReplyWith("Chat is disabled.");
            } else {
                if (arg.ArgsStr == "\"\"")
                    return;

                BasePlayer basePlayer = arg.Player();

                if (!basePlayer || basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
                    return;

                string str = arg.GetString(0, "text").Trim();

                if (str.Length > 128)
                    str = str.Substring(0, 128);

                if (str.Length <= 0)
                    return;

                if (basePlayer.NextChatTime < Single.Epsilon) {
                    basePlayer.NextChatTime = Time.realtimeSinceStartup - 30f;
                }

                if (basePlayer.NextChatTime > Time.realtimeSinceStartup) {
                    basePlayer.NextChatTime += 2f;
                    float num = basePlayer.NextChatTime - Time.realtimeSinceStartup;

                    ConsoleNetwork.SendClientCommand(basePlayer.net.connection, "chat.add", new object[] {
                        0,
                        "You're chatting too fast - try again in " + (num + 0.5f).ToString("0") + " seconds"
                    });

                    if (num > 120f) {
                        basePlayer.Kick("Chatting too fast");
                    }

                    return;
                }

                var ce = new ChatEvent(Server.GetPlayer(basePlayer), arg);

                OnNext("On_Chat", ce);

                if (!ce.AllowFormatting && ce.FinalText.Contains("<")) {
                    if (ce.FinalText.Contains("<size", CompareOptions.IgnoreCase))
                        return;

                    if (ce.FinalText.Contains("<color", CompareOptions.IgnoreCase))
                        return;

                    if (ce.FinalText.Contains("<material", CompareOptions.IgnoreCase))
                        return;

                    if (ce.FinalText.Contains("<quad", CompareOptions.IgnoreCase))
                        return;

                    if (ce.FinalText.Contains("<b>", CompareOptions.IgnoreCase))
                        return;

                    if (ce.FinalText.Contains("<i>", CompareOptions.IgnoreCase))
                        return;
                }

                if (ConVar.Chat.serverlog) {
                    ServerConsole.PrintColoured(new object[] {
                        ConsoleColor.DarkYellow,
                        basePlayer.displayName + ": ",
                        ConsoleColor.DarkGreen,
                        str
                    });

                    ConVar.Server.Log("Log.Chat.txt",
                                      string.Format("{0}: {1}\n",
                                                    basePlayer.userID,
                                                    basePlayer.displayName,
                                                    str));

                    Debug.Log(String.Format("[CHAT] {0}: {1}", basePlayer.displayName, str));
                }

                string colour = "#5af";

                if (basePlayer.IsAdmin()) {
                    colour = "#af5";
                }

                if (DeveloperList.IsDeveloper(basePlayer)) {
                    colour = "#fa5";
                }

                basePlayer.NextChatTime = Time.realtimeSinceStartup + 1.5f;

                var chatEntry = new ConVar.Chat.ChatEntry {
                    Message = ce.FinalText,
                    UserId = basePlayer.userID,
                    Username = basePlayer.displayName,
                    Color = colour,
                    Time = Facepunch.Math.Epoch.Current
                };

                (typeof(ConVar.Chat).GetStaticFieldValue("History") as List<ConVar.Chat.ChatEntry>).Add(chatEntry);

                Facepunch.RCon.Broadcast(Facepunch.RCon.LogType.Chat, chatEntry);

                if (ce.FinalText != "") {
                    Logger.ChatLog(ce.BroadcastName, ce.OriginalText);
                    arg.ReplyWith(ce.Reply);

                    if (ConVar.Server.globalchat) {
                        ConsoleNetwork.BroadcastToAllClients("chat.add2",
                                                             basePlayer.userID,
                                                             ce.FinalText,
                                                             ce.BroadcastName,
                                                             colour,
                                                             1);
                    } else {
                        float num = 2500;

                        foreach (BasePlayer current in BasePlayer.activePlayerList) {
                            float sqrMagnitude = (current.transform.position - basePlayer.transform.position).sqrMagnitude;

                            if (sqrMagnitude <= num) {
                                ConsoleNetwork.SendClientCommand(current.net.connection,
                                                                 "chat.add2",
                                                                 basePlayer.userID,
                                                                 ce.FinalText,
                                                                 ce.BroadcastName,
                                                                 colour,
                                                                 Mathf.Clamp01(num - sqrMagnitude + 0.2f));
                            }
                        }
                    }
                }
            }
        }

        public static void On_Command(ConsoleSystem.Arg arg)
        {
            Player player = Server.GetPlayer(arg.Player());
            string[] args = arg.ArgsStr.Substring(2, arg.ArgsStr.Length - 3).Replace("\\", "").Split(new string[] { " " },
                                                                                                     StringSplitOptions.None);

            var cmd = new CommandEvent(player, args);

            // TODO: do this part in a different function to be documented
            if (cmd.Cmd == "")
                return;

            foreach (KeyValuePair<string, BasePlugin> pl in PluginLoader.GetInstance().Plugins) {
                ChatCommand[] commands = ((ChatCommands)pl.Value.GetGlobalObject("Commands")).getChatCommands(cmd.Cmd);

                foreach (ChatCommand chatCmd in commands) {
                    if (chatCmd.callback != null) {
                        CommandPermissionEvent permission = new CommandPermissionEvent(player, args, chatCmd);

                        OnNext("On_CommandPermission", permission);

                        if (permission.Blocked) {
                            player.Message(permission.Reply);
                            continue;
                        }

                        try {
                            chatCmd.callback(cmd.Args, player);
                        } catch (Exception ex) {
                            Logger.LogError(chatCmd.plugin.FormatException(ex));
                        }
                    }
                }
            }

            OnNext("On_Command", cmd);

            if (cmd.Reply != "")
                arg.ReplyWith(cmd.Reply);
        }

        #endregion

        #region Item Hooks

        /// <summary>
        /// Called from <c>ItemContainer.Insert(Item)</c> .
        /// </summary>
        public static void On_ItemAdded(ItemContainer ic, Item i) => OnNext("On_ItemAdded", new InventoryModEvent(ic, i));

        /// <summary>
        /// Called from <c>Item.LoseCondition(float)</c> .
        /// </summary>
        public static void On_ItemLoseCondition(Item i, float f) => OnNext("On_ItemLoseCondition", new ItemConditionEvent(i, f));

        /// <summary>
        /// Called from <c>CollectibleEntity.Pickup(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_ItemPickup(CollectibleEntity entity, BaseEntity.RPCMessage msg)
        {
            if (!msg.player.IsAlive() || !msg.player.CanInteract() || entity.itemList == null)
                return;

            foreach (ItemAmount itemAmount in entity.itemList) {
                Item item = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0);

                OnNext("On_ItemPickup", new ItemPickupEvent(entity, msg, item));

                msg.player.GiveItem(item, BaseEntity.GiveItemReason.PickedUp);
            }

            entity.itemList = null;

            if (entity.pickupEffect.isValid) {
                Effect.server.Run(entity.pickupEffect.resourcePath,
                                  entity.transform.position,
                                  entity.transform.up,
                                  null,
                                  false);
            }

            entity.Kill(BaseNetworkable.DestroyMode.None);
        }

        /// <summary>
        /// Called from <c>ItemContainer.Remove(Item)</c> .
        /// </summary>
        public static void On_ItemRemoved(ItemContainer ic, Item i) => OnNext("On_ItemRemoved", new InventoryModEvent(ic, i));

        /// <summary>
        /// Called from <c>RepairBench.RepairItem(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_ItemRepaired(RepairBench rb, BaseEntity.RPCMessage msg) => OnNext("On_ItemRepaired", new ItemRepairEvent(rb, msg));

        /// <summary>
        /// Called from <c>Item.UseItem(int)</c> .
        /// </summary>
        public static void On_ItemUsed(Item item, int amountToConsume) => OnNext("On_ItemUsed", new ItemUsedEvent(item, amountToConsume));

        #endregion

        #region Construction Hooks

        /// <summary>
        /// Called from <c>BuildingBlock.DoUpgradeToGrade(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_BuildingUpgrade(BuildingBlock block, BaseEntity.RPCMessage msg)
        {
            BasePlayer messagePlayer = msg.player;
            BuildingGrade.Enum buildingGrade = (BuildingGrade.Enum) msg.read.Int32();
            ConstructionGrade constructionGrade = (ConstructionGrade) block.CallMethod("GetGrade", buildingGrade);

            Pre<BuildingUpgradeEvent> preBuildingUpgradeEvent = new Pre<BuildingUpgradeEvent>(block, buildingGrade, messagePlayer);

            OnNext("Pre_BuildingUpgrade", preBuildingUpgradeEvent);

            if (preBuildingUpgradeEvent.IsCanceled)
                return;

            if (constructionGrade == null)
                return;

            if ((bool) block.CallMethod("CanChangeToGrade", buildingGrade, messagePlayer) == false)
                return;

            if ((bool)block.CallMethod("CanAffordUpgrade", buildingGrade, messagePlayer) == false)
                return;

            block.CallMethod("PayForUpgrade", constructionGrade, messagePlayer);
            block.SetGrade(buildingGrade);
            block.SetHealthToMax();
            block.CallMethod("StartBeingRotatable");
            block.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
            block.UpdateSkin(false);

            Effect.server.Run("assets/bundled/prefabs/fx/build/promote_" + buildingGrade.ToString().ToLower() + ".prefab", block, 0u, Vector3.zero, Vector3.zero);

            OnNext("On_BuildingUpgrade", preBuildingUpgradeEvent.Event);
        }

        /// <summary>
        /// Called from <c>BuildingBlock.DoDemolish(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_BuildingPartDemolished(BuildingBlock bb, BaseEntity.RPCMessage msg) => OnNext("On_BuildingPartDemolished", new BuildingPartDemolishedEvent(bb, msg.player));

        /// <summary>
        /// Called from <c>Construction.CreateConstruction(Construction.Target, bool)</c> .
        /// </summary>
        public static BaseEntity On_Placement(Construction construction, Construction.Target target, bool needsValidPlacement)
        {
            try {
                GameObject gameObject = GameManager.server.CreatePrefab(construction.fullName,
                                                                        default(Vector3),
                                                                        default(Quaternion),
                                                                        true);
                BuildingBlock component = gameObject.GetComponent<BuildingBlock>();
                bool flag = construction.UpdatePlacement(gameObject.transform, construction, target);

                if (needsValidPlacement && !flag) {
                    UnityEngine.Object.Destroy(gameObject);
                    return null;
                }

                BuildingEvent be = null;

                if (component != null) {
                    be = new BuildingEvent(construction, target, component, needsValidPlacement);

                    OnNext("On_Placement", be);
                }

                if (be != null && be.DoDestroy) {
                    be.Builder.Message(be.DestroyReason);
                    UnityEngine.Object.Destroy(gameObject);
                    return null;
                }

                return gameObject.GetComponent<BaseEntity>();
            } catch (Exception ex) {
                Logger.LogException(ex);
                return null;
            }
        }

        #endregion

        #region Trap Hooks

        /// <summary>
        /// Called from <c>Landmine.Arm()</c>
        /// </summary>
        public static void On_LandmineArmed(Landmine l) => OnNext("On_LandmineArmed", l);

        /// <summary>
        /// Called from <c>Landmine.Explode()</c>
        /// </summary>
        public static void On_LandmineExploded(Landmine l) => OnNext("On_LandmineExploded", l);

        /// <summary>
        /// Called from <c>Landmine.Trigger(BasePlayer)</c>
        /// </summary>
        public static void On_LandmineTriggered(Landmine landmine, BasePlayer basePlayer)
        {
            Pre<LandmineTriggerEvent> preLandmineTriggerEvent = new Pre<LandmineTriggerEvent>(landmine, basePlayer);

            OnNext("Pre_LandmineTriggered", preLandmineTriggerEvent);

            if (preLandmineTriggerEvent.IsCanceled == false) {
                if (basePlayer != null) {
                    landmine.SetFieldValue("triggerPlayerID", basePlayer.userID);
                }

                landmine.SetFlag(BaseEntity.Flags.Open, true);
                landmine.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);

                OnNext("On_LandmineTriggered", preLandmineTriggerEvent.Event);
            }
        }

        #endregion

        #region Door Hooks

        /// <summary>
        /// Called from <c>CodeLock.UnlockWithCode(BaseEntity.RPCMessage)</c>
        /// </summary>
        public static void On_DoorCodeEntered(CodeLock codeLock, BaseEntity.RPCMessage rpc)
        {
            if (!codeLock.IsLocked())
                return;

            string code = rpc.read.String();
            Pre<DoorCodeEvent> preDoorCodeEvent = new Pre<DoorCodeEvent>(codeLock, rpc.player, code);

            OnNext("Pre_DoorCodeEntered", preDoorCodeEvent);

            if (preDoorCodeEvent.IsCanceled || (!preDoorCodeEvent.Event.IsCorrect() && !preDoorCodeEvent.Event.ForceAllow)) {
                Effect.server.Run(codeLock.effectDenied.resourcePath, codeLock, 0u, Vector3.zero, Vector3.forward);
                rpc.player.Hurt(1f, global::Rust.DamageType.ElectricShock, codeLock, true);
                return;
            }

            Effect.server.Run(codeLock.effectUnlocked.resourcePath, codeLock, 0u, Vector3.zero, Vector3.forward);

            codeLock.SetFlag(BaseEntity.Flags.Locked, false);
            codeLock.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);

            List<ulong> whitelist = new List<ulong>();

            whitelist = (List<ulong>) codeLock.GetFieldValue("whitelistPlayers");

            if (!whitelist.Contains(rpc.player.userID)) {
                whitelist.Add(rpc.player.userID);
                codeLock.SetFieldValue("whitelistPlayers", whitelist);
            }

            OnNext("On_DoorCodeEntered", preDoorCodeEvent.Event);
        }

        /// <summary>
        /// Called from <c>Door.RPC_OpenDoor(BaseEntity.RPCMessage)</c> and <c>Door.RPC_CloseDoor(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_DoorUse(Door door, BaseEntity.RPCMessage msg, bool open)
        {
            if ((open && door.IsOpen()) || (!open && !door.IsOpen()))
                return;

            var preDoorUseEvent = new Pre<DoorUseEvent>(door, msg, open);

            OnNext("Pre_DoorUse", preDoorUseEvent);

            if (preDoorUseEvent.IsCanceled) {
                if (preDoorUseEvent.Event.DenyReason != "")
                    msg.player.SendConsoleCommand("chat.add",
                                                  0,
                                                  String.Format("{0}: {1}",
                                                  Server.server_message_name.ColorText("fa5"),
                                                  preDoorUseEvent.Event.DenyReason));

                return;
            }

            bool doAction = true;
            BaseLock baseLock = door.GetSlot(BaseEntity.Slot.Lock) as BaseLock;

            if (baseLock != null && preDoorUseEvent.Event.IgnoreLock == false) {
                doAction = open ? baseLock.OnTryToOpen(msg.player) : baseLock.OnTryToClose(msg.player);

                if (doAction && open && (baseLock.IsLocked() && Time.realtimeSinceStartup - (float)door.GetFieldValue("decayResetTimeLast") > 60)) {
                    Decay.RadialDecayTouch(door.transform.position, 40, 270532608);
                    door.SetFieldValue("decayResetTimeLast", Time.realtimeSinceStartup);
                }
            }

            if (doAction) {
                door.SetFlag(BaseEntity.Flags.Open, open);
                door.SendNetworkUpdateImmediate(false);
                door.CallMethod("UpdateDoorAnimationParameters", false);

                OnNext("On_DoorUse", preDoorUseEvent.Event);
            }
        }

        #endregion

        #region NPC Hooks

        /// <summary>
        /// Called from <c>BaseNPC.OnKilled(HitInfo)</c> .
        /// </summary>
        public static void On_NPCKilled(BaseNPC baseNPC, HitInfo info)
        {
            if (info.Initiator != null && info.Initiator.ToPlayer() != null) {
                Server.GetPlayer(info.Initiator as BasePlayer).Stats.AddKill(false, true);
            }

            NPC npc = new NPC(baseNPC);

            OnNext("On_NPCKilled", new NPCDeathEvent(npc, info));
        }

        #endregion

        #region Player Hooks

        /// <summary>
        /// Called from <c>BasePlayer.RPC_Assist(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_PlayerAssisted(BasePlayer bp) => OnNext("On_PlayerAssisted", Server.GetPlayer(bp));

        /// <summary>
        /// Called from <c>ResourceDispenser.GiveResourceFromItem(BaseEntity, ItemAmount, float, float, AttackEntity)</c> .
        /// </summary>
        public static void On_PlayerClothingChanged(PlayerInventory pi, Item i) => OnNext("On_PlayerClothingChanged", new PlayerClothingEvent(pi, i));
        
        /// <summary>
        /// Called from <c>BasePlayer.PlayerInit(Connection)</c> .
        /// </summary>
        public static void On_PlayerConnected(BasePlayer basePlayer)
        {
            Player player = new Player(basePlayer);

            if (Server.GetInstance().OfflinePlayers.ContainsKey(basePlayer.userID))
                Server.GetInstance().OfflinePlayers.Remove(basePlayer.userID);

            if (!Server.GetInstance().Players.ContainsKey(basePlayer.userID))
                Server.GetInstance().Players.Add(basePlayer.userID, player);

            OnNext("On_PlayerConnected", player);
        }

        /// <summary>
        /// Called from <c>BasePlayer.OnDisconnected()</c> .
        /// </summary>
        public static void On_PlayerDisconnected(BasePlayer basePlayer)
        {
            Player player = Server.GetPlayer(basePlayer);

            if (Server.GetInstance().serverData.ContainsKey("OfflinePlayers", player.SteamID)) {
                OfflinePlayer offlinePlayer = (Server.GetInstance().serverData.Get("OfflinePlayers", player.SteamID) as OfflinePlayer);

                offlinePlayer.Update(player);

                Server.GetInstance().OfflinePlayers[basePlayer.userID] = offlinePlayer;
            } else {
                OfflinePlayer offlinePlayer = new OfflinePlayer(player);

                Server.GetInstance().OfflinePlayers.Add(basePlayer.userID, offlinePlayer);
            }

            if (Server.GetInstance().Players.ContainsKey(basePlayer.userID))
                Server.GetInstance().Players.Remove(basePlayer.userID);

            OnNext("On_PlayerDisconnected", player);
        }

        /// <summary>
        /// Called from <c>BasePlayer.Die(HitInfo)</c> .
        /// </summary>
        public static void On_PlayerDied(BasePlayer basePlayer, HitInfo info)
        {
            using (TimeWarning.New("Player.Die", 0.1f)) {
                if (!basePlayer.IsDead()) {
                    if (info == null) {
                        info = new HitInfo();
                        info.damageTypes.Add(basePlayer.lastDamage, Single.MaxValue);
                        info.Initiator = (BaseEntity)basePlayer;
                    }

                    Player victim = Server.GetPlayer(basePlayer);

                    if (!((bool)basePlayer.CallMethod("WoundInsteadOfDying", info))) {
                        var pde = new PlayerDeathEvent(victim, info);

                        OnNext("On_PlayerDied", pde);

                        if (pde.Die) {
                            if (info.Initiator != null) {
                                // PlayerStats statsV = victim.Stats;

                                if (info.Initiator is BasePlayer) {
                                    Server.GetPlayer(info.Initiator as BasePlayer).Stats.AddKill(true, false);
                                    victim.Stats.AddDeath(true, false);
                                } else if (info.Initiator is BaseNPC) {
                                        victim.Stats.AddDeath(false, true);
                                    }
                            }

                            if (!pde.dropLoot) {
                                if (basePlayer.belt != null) {
                                    Vector3 vector = new Vector3(UnityEngine.Random.Range(-2f, 2f),
                                                                 0.2f,
                                                                 UnityEngine.Random.Range(-2f, 2f));

                                    basePlayer.belt.DropActive(vector.normalized * 3f);
                                }

                                basePlayer.inventory.Strip();
                            }

                            basePlayer.CallMethodOnBase(typeof(BaseCombatEntity), "Die", info);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Called from <c>ResourceDispenser.GiveResourceFromItem(BaseEntity, ItemAmount, float, float, AttackEntity)</c> .
        /// </summary>
        public static void On_PlayerGathering(ResourceDispenser dispenser,
                                              BaseEntity to,
                                              ItemAmount itemAmount,
                                              float gatherDamage,
                                              float destroyFraction)
        {
            BaseEntity from = (BaseEntity)dispenser.GetFieldValue("baseEntity");

            if (itemAmount.amount == 0)
                return;

            float num = gatherDamage / from.MaxHealth();
            float num2 = itemAmount.startAmount / (float)dispenser.GetFieldValue("startingItemCounts");
            float value = itemAmount.startAmount * num / num2;
            float num3 = Mathf.Clamp(value, 0, itemAmount.amount);
            float num4 = num3 * destroyFraction * 2;

            if (itemAmount.amount < num3 + num4) {
                itemAmount.amount -= destroyFraction * num3;
                num3 = itemAmount.amount;
                num4 = 0;
            }

            float amount = itemAmount.amount;

            itemAmount.amount -= num3;

            if (itemAmount.amount < 0) {
                itemAmount.amount = 0;
            }

            int num5 = Mathf.Clamp(Mathf.RoundToInt(num3), 0, Mathf.CeilToInt(amount));

            itemAmount.amount -= num4;

            if (itemAmount.amount < 0) {
                itemAmount.amount = 0;
            }

            GatherEvent ge = new GatherEvent(dispenser, from, to, itemAmount, num5);

            OnNext("On_PlayerGathering", ge);

            if (ge.Amount <= 0)
                return;

            Item item = ItemManager.CreateByItemID(itemAmount.itemid, ge.Amount);

            if (item == null)
                return;

            to.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
        }

        /// <summary>
        /// Called from <c>BasePlayer.OnHealthChanged(float, float)</c> .
        /// </summary>
        public static void On_PlayerHealthChange(BasePlayer p, float f, float f2) => OnNext("On_PlayerHealthChange", new PlayerHealthChangeEvent(p, f, f2));

        /// <summary>
        /// Called from <c>BasePlayer.EnterGame()</c> .
        /// </summary>
        public static void On_PlayerLoaded(BasePlayer bp) => OnNext("On_PlayerLoaded", Server.GetPlayer(bp));
        
        /// <summary>
        /// Called from <c>BasePlayer.RespawnAt(Vector3, Quaternion)</c> .
        /// </summary>
        public static void On_PlayerRespawn(BasePlayer basePlayer, Vector3 pos, Quaternion quat)
        {
            Player player = Server.GetPlayer(basePlayer);
            var re = new RespawnEvent(player, pos, quat);

            OnNext("On_PlayerRespawn", re);

            basePlayer.SetPlayerFlag(BasePlayer.PlayerFlags.Wounded, false);
            basePlayer.SetPlayerFlag(BasePlayer.PlayerFlags.HasBuildingPrivilege, false);
            basePlayer.SetPlayerFlag(BasePlayer.PlayerFlags.InBuildingPrivilege, false);
            basePlayer.SetPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot, true);
            ++ServerPerformance.spawns;
            basePlayer.transform.position = re.SpawnPos;
            basePlayer.transform.rotation = re.SpawnRot;
            (basePlayer.GetFieldValue("tickInterpolator") as TickInterpolator).Reset(pos);
            basePlayer.SetFieldValue("lastTickTime", 0f);
            basePlayer.CancelInvoke("WoundingEnd");
            basePlayer.StopSpectating();
            basePlayer.UpdateNetworkGroup();
            basePlayer.UpdatePlayerCollider(true, false);
            basePlayer.StartSleeping();
            basePlayer.Invoke("LifeStoryStart", 0f);
            basePlayer.metabolism.Reset();

            if (re.StartHealth < Single.Epsilon)
            {
                basePlayer.InitializeHealth(basePlayer.StartHealth(), basePlayer.StartMaxHealth());
            }
            else
            {
                basePlayer.InitializeHealth(re.StartHealth, basePlayer.StartMaxHealth());
            }

            if (re.GiveDefault)
            {
                basePlayer.inventory.GiveDefaultItems();
            }

            if (re.WakeUp)
            {
                basePlayer.EndSleeping();
            }

            basePlayer.SendNetworkUpdateImmediate(false);
            basePlayer.ClearEntityQueue();
            basePlayer.ClientRPCPlayer(null, basePlayer, "StartLoading");

            if (basePlayer.IsConnected())
                basePlayer.SendFullSnapshot();

            // player.SetPlayerFlag (BasePlayer.PlayerFlags.ReceivingSnapshot, false);
            // player.ClientRPCPlayer(null, player, "FinishLoading");
        }
        
        /// <summary>
        /// Called from <c>BaseProjectile.CLProject(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_PlayerShoot(BaseProjectile baseProjectile, BaseEntity.RPCMessage msg)
        {
            OnNext("On_PlayerShoot", new ShootEvent(baseProjectile, msg));
        }

        /// <summary>
        /// Called from <c>BaseLauncher.SV_Launch(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_PlayerShootRocket(BaseLauncher baseLauncher, BaseEntity.RPCMessage msg, BaseEntity baseEntity)
        {
            OnNext("On_PlayerShootRocket", new ShootRocketEvent(baseLauncher, msg, baseEntity));
        }

        /// <summary>
        /// Called from <c>BasePlayer.StartSleeping()</c> .
        /// </summary>
        public static void On_PlayerSleep(BasePlayer bp) => OnNext("On_PlayerSleep", Server.GetPlayer(bp));

        /// <summary>
        /// Called from <c>ItemCrafter.CraftItem(ItemBlueprint, BasePlayer, ProtoBuf.Item.InstanceData, int, int, Item)</c> .
        /// </summary>
        public static bool On_PlayerStartCrafting(ItemCrafter self,
                                                  ItemBlueprint bp,
                                                  BasePlayer owner,
                                                  ProtoBuf.Item.InstanceData instanceData = null,
                                                  int amount = 1,
                                                  int skinID = 0,
                                                  Item fromTempBlueprint = null)
        {
            var ce = new CraftEvent(self, bp, owner, instanceData, amount, skinID);

            OnNext("On_PlayerStartCrafting", ce);

            if (!self.CanCraft(bp, ce.Amount))
                return false;

            if (ce.Cancel) {
                if (ce.cancelReason != String.Empty)
                    ce.Crafter.Message(ce.cancelReason);
                return false;
            }

            self.taskUID++;

            ItemCraftTask itemCraftTask = Facepunch.Pool.Get<ItemCraftTask>();
            itemCraftTask.blueprint = bp;
            self.CallMethod("CollectIngredients", bp, itemCraftTask, ce.Amount, owner);
            itemCraftTask.endTime = 0;
            itemCraftTask.taskUID = self.taskUID;
            itemCraftTask.owner = owner;
            itemCraftTask.instanceData = instanceData;

            if (itemCraftTask.instanceData != null) {
                itemCraftTask.instanceData.ShouldPool = false;
            }

            itemCraftTask.amount = ce.Amount;
            itemCraftTask.skinID = ce.SkinID;

            if (fromTempBlueprint != null) {
                fromTempBlueprint.RemoveFromContainer();
                itemCraftTask.takenItems.Add(fromTempBlueprint);
                itemCraftTask.conditionScale = 0.5f;
            }

            self.queue.Enqueue(itemCraftTask);

            if (itemCraftTask.owner != null) {
                itemCraftTask.owner.Command("note.craft_add", new object[] {
                    itemCraftTask.taskUID,
                    itemCraftTask.blueprint.targetItem.itemid,
                    amount
                });
            }

            return true;
        }

        /// <summary>
        /// Called from <c>MedicalTool.UseSelf(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_PlayerSyringeSelf(MedicalTool medicalTool, BaseEntity.RPCMessage msg)
        {
            BasePlayer messagePlayer = msg.player;

            if (messagePlayer.CanInteract() == false)
                return;

            if ((bool)medicalTool.CallMethod("HasItemAmount") == false)
                return;

            BasePlayer owner = medicalTool.GetOwnerPlayer();

            var preSyringeUseEvent = new Pre<SyringeUseEvent>(medicalTool, owner, owner);

            OnNext("Pre_PlayerSyringeSelf", preSyringeUseEvent);

            if (preSyringeUseEvent.IsCanceled == false) {
                medicalTool.ClientRPCPlayer(null, messagePlayer, "Reset");
                medicalTool.CallMethod("GiveEffectsTo", owner);
                medicalTool.CallMethod("UseItemAmount", 1);

                OnNext("On_PlayerSyringeSelf", preSyringeUseEvent.Event);
            }
        }

        /// <summary>
        /// Called from <c>MedicalTool.UseOther(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_PlayerSyringeOther(MedicalTool medicalTool, BaseEntity.RPCMessage msg)
        {
            BasePlayer messagePlayer = msg.player;

            if (messagePlayer.CanInteract() == false)
                return;

            if ((bool)medicalTool.CallMethod("HasItemAmount") == false || medicalTool.canUseOnOther == false)
                return;

            BasePlayer owner = medicalTool.GetOwnerPlayer();

            if (owner == null)
                return;

            BasePlayer target = BaseNetworkable.serverEntities.Find(msg.read.UInt32()) as BasePlayer;

            if (target != null && Vector3.Distance(target.transform.position, owner.transform.position) < 4f) {
                var preSyringeUseEvent = new Pre<SyringeUseEvent>(medicalTool, owner, target);
                
                OnNext("Pre_PlayerSyringeOther", preSyringeUseEvent);

                if (preSyringeUseEvent.IsCanceled == false) {
                    medicalTool.ClientRPCPlayer(null, messagePlayer, "Reset");
                    medicalTool.CallMethod("GiveEffectsTo", target);
                    medicalTool.CallMethod("UseItemAmount", 1);

                    OnNext("On_PlayerSyringeOther", preSyringeUseEvent.Event);
                }
            }
        }

        /// <summary>
        /// Called from <c>BasePlayer.UpdateRadiation(float)</c> .
        /// </summary>
        public static void On_PlayerTakeRadiation(BasePlayer basePlayer, float radAmount)
        {
            var ptr = new PlayerTakeRadsEvent(basePlayer, basePlayer.metabolism.radiation_level.value, radAmount);

            OnNext("On_PlayerTakeRadiation", ptr);

            basePlayer.metabolism.radiation_level.value = ptr.Next;
        }

        /// <summary>
        /// Called from <c>BaseMelee.CLProject(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_PlayerThrowWeapon(BaseMelee baseMelee, BaseEntity.RPCMessage msg)
        {
            BasePlayer messagePlayer = msg.player;

            if ((bool) baseMelee.CallMethod("VerifyClientAttack", messagePlayer) == false) {
                baseMelee.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
                return;
            }

            if (!baseMelee.canThrowAsProjectile){
                Debug.LogWarning(messagePlayer + " fired invalid projectile: Not throwable");
                return;
            }

            Item item = baseMelee.GetItem();

            if (item == null) {
                Debug.LogWarning(messagePlayer + " fired invalid projectile: Item not found");
                return;
            }

            ItemModProjectile component = item.info.GetComponent<ItemModProjectile>();

            if (component == null) {
                Debug.LogWarning(messagePlayer + " fired invalid projectile: Item mod not found");
                return;
            }

            ProjectileShoot projectileShoot = ProjectileShoot.Deserialize(msg.read);

            if (projectileShoot.projectiles.Count != 1){
                Debug.LogWarning(messagePlayer + " fired invalid projectile: Projectile count mismatch");
                return;
            }

            messagePlayer.CleanupExpiredProjectiles();

            foreach (ProjectileShoot.Projectile current in projectileShoot.projectiles) {
                if (messagePlayer.HasFiredProjectile(current.projectileID)) {
                    Debug.LogWarning(messagePlayer + " fired invalid projectile: Duplicate ID ->" + current.projectileID);
                }
                else {
                    Pre<WeaponThrowEvent> preWeaponThrowEvent = new Pre<WeaponThrowEvent>(baseMelee, messagePlayer, projectileShoot, current);

                    OnNext("Pre_PlayerThrowWeapon", preWeaponThrowEvent);

                    if (preWeaponThrowEvent.IsCanceled == false)
                    {
                        messagePlayer.NoteFiredProjectile(current.projectileID, current.startPos, current.startVel, baseMelee, item.info, item);

                        Effect effect = new Effect();
                        effect.Init(Effect.Type.Projectile, current.startPos, current.startVel.normalized, msg.connection);
                        effect.scale = preWeaponThrowEvent.Event.Magnitude;
                        effect.pooledString = component.projectileObject.resourcePath;
                        effect.number = current.seed;

                        EffectNetwork.Send(effect);

                        OnNext("On_PlayerThrowWeapon", preWeaponThrowEvent.Event);
                    }
                }
            }

            item.SetParent(null);
        }

        /// <summary>
        /// Called from <c>ThrownWeapon.DoThrow(BaseEntity.RPCMessage)</c> .
        /// </summary>
        public static void On_PlayerThrowExplosive(ThrownWeapon thrownWeapon, BaseEntity.RPCMessage msg)
        {
            ThrowEvent evt = new ThrowEvent(thrownWeapon, msg);

            switch (evt.ProjectileName)
            {
                case "F1 Grenade":
                case "Beancan Grenade":
                    OnNext("On_PlayerThrowGrenade", evt);
                    break;

                case "Timed Explosive Charge":
                case "Survey Charge":
                case "Satchel Charge":
                    OnNext("On_PlayerThrowCharge", evt);
                    break;

                case "Supply Signal":
                    OnNext("On_PlayerThrowSignal", evt);
                    break;
            }
        }

        /// <summary>
        /// Called from <c>BasePlayer.EndSleeping()</c> .
        /// </summary>
        public static void On_PlayerWakeUp(BasePlayer bp) => OnNext("On_PlayerWakeUp", Server.GetPlayer(bp));

        /// <summary>
        /// Called from <c>BasePlayer.StartWounded()</c> .
        /// </summary>
        public static void On_PlayerWounded(BasePlayer bp) => OnNext("On_PlayerWounded", Server.GetPlayer(bp));

        #endregion

        #region Looting Hooks

        /// <summary>
        /// Called from <c>PlayerLoot.StartLootingEntity(BaseEntity, bool)</c> .
        /// </summary>
        public static void On_LootingEntity(PlayerLoot playerLoot)
        {
            BasePlayer looter = playerLoot.GetComponent<BasePlayer>();
            var ele = new EntityLootEvent(playerLoot, Server.GetPlayer(looter), new Entity(playerLoot.entitySource));

            OnNext("On_LootingEntity", ele);

            if (ele.Cancel) {
                playerLoot.Clear();
                looter.SendConsoleCommand("chat.add",
                                          0,
                                          String.Format("{0}: {1}",
                                                        Server.server_message_name.ColorText("fa5"),
                                                        ele.cancelReason));
            }
        }

        /// <summary>
        /// Called from <c>PlayerLoot.StartLootingItem(Item)</c> .
        /// </summary>
        public static void On_LootingItem(PlayerLoot playerLoot)
        {
            BasePlayer looter = playerLoot.GetComponent<BasePlayer>();
            var ile = new ItemLootEvent(playerLoot, Server.GetPlayer(looter), playerLoot.itemSource);

            OnNext("On_LootingItem", ile);

            if (ile.Cancel) {
                playerLoot.Clear();
                looter.SendConsoleCommand("chat.add",
                                          0,
                                          String.Format("{0}: {1}",
                                                        Server.server_message_name.ColorText("fa5"),
                                                        ile.cancelReason));
            }
        }

        /// <summary>
        /// Called from <c>PlayerLoot.StartLootingItem(BasePlayer)</c> .
        /// </summary>
        public static void On_LootingPlayer(PlayerLoot playerLoot)
        {
            BasePlayer looter = playerLoot.GetComponent<BasePlayer>();
            var ple = new PlayerLootEvent(playerLoot,
                                          Server.GetPlayer(looter),
                                          Server.GetPlayer(playerLoot.entitySource as BasePlayer));

            OnNext("On_LootingPlayer", ple);

            if (ple.Cancel) {
                playerLoot.Clear();
                looter.SendConsoleCommand("chat.add",
                                          0,
                                          String.Format("{0}: {1}",
                                                        Server.server_message_name.ColorText("fa5"),
                                                        ple.cancelReason));
            }
        }

        #endregion

        #region Server Hooks

        /// <summary>
        /// Called from <c>ConsoleSystem.SystemRealm.Normal(RunOptions, string, out string, params object[])</c> .
        /// </summary>
        public static string On_ServerConsole(ConsoleSystem.SystemRealm realm, ConsoleSystem.RunOptions options, string cmd, out string error, params object[] args)
        {
            error = null;
            string text = ConsoleSystem.BuildCommand(cmd, args);
            var arg = new ConsoleSystem.Arg(text);

            var sce = new ServerConsoleEvent(arg, cmd);

            foreach (KeyValuePair<string, BasePlugin> pl in PluginLoader.GetInstance().Plugins) {
                object globalObj = pl.Value.GetGlobalObject("ServerConsoleCommands");

                if (globalObj is ConsoleCommands) {
                    ConsoleCommand[] commands = (globalObj as ConsoleCommands).getConsoleCommands(sce.Cmd);

                    foreach (ConsoleCommand cc in commands) {
                        if (cc.callback == null)
                            continue;

                        try {
                            cc.callback(arg.ArgsStr.Split(' '));
                        } catch (Exception ex) {
                            Logger.LogError(cc.plugin.FormatException(ex));
                        }
                    }
                }
            }

            OnNext("On_ServerConsole", sce);

            arg.FromClient = !realm.isServer;
            if (!arg.Invalid && arg.CheckPermissions()) {
                bool flag = ConsoleSystem.Run.Internal(arg, options.giveFeedback);
                if ((options.giveFeedback & flag) && arg.Reply != null && arg.Reply.Length > 0) {
                    Debug.Log(arg.Reply);
                }
                return arg.Reply;
            }
            error = "Command not found";
            if (!realm.isServer && (!options.forwardToServer || !((bool)typeof(ConsoleSystem).CallStaticMethod("SendToServer", text)))) {
                if (options.giveFeedback) {
                    Debug.Log(error);
                }
                return null;
            }
            if (realm.isServer && options.giveFeedback) {
                Debug.Log(error);
            }
            return null;
        }

        public static void On_ServerInit()
        {
            Server.GetInstance().SendCommand("plugins.loaded");

            if (Server.GetInstance().Loaded)
                return;

            Server.GetInstance().Loaded = true;

            OnNext("On_ServerInit");
        }

        public static void On_ServerSaved() => OnNext("On_ServerSaved");

        public static void On_ServerShutdown()
        {
            Core.Bootstrap.timers.Dispose();

            OnNext("On_ServerShutdown");

            PluginLoader.GetInstance().UnloadPlugins();

            Core.Bootstrap.SaveAll();
        }

        #endregion

        #region Client Hooks

        /// <summary>
        /// Called from <c>ConnectionAuth.Approve(Connection)</c> .
        /// </summary>
        public static void On_ClientAuth(ConnectionAuth ca, Connection connection)
        {
            var ae = new Pre<AuthEvent>(connection);

            OnNext("Pre_ClientAuth", ae);
            if (!ae.IsCanceled)
                OnNext("On_ClientAuth", ae.Event);

            ConnectionAuth.m_AuthConnection.Remove(connection);

            if (!ae.Event.Approved)
            {
                ConnectionAuth.Reject(connection, ae.Event.Reason);
                return;
            }

            SingletonComponent<ServerMgr>.Instance.ConnectionApproved(connection);
        }

        /// <summary>
        /// Called from <c>ConsoleNetwork.OnClientCommand(Message)</c> .
        /// </summary>
        public static void On_ClientConsole(ConsoleSystem.Arg arg, string rconCmd)
        {
            var ce = new ClientConsoleEvent(arg, rconCmd);

            if (arg.connection != null) {
                OnNext("On_ClientConsole", ce);

                if (arg.Invalid) {
                    if (!Net.sv.IsConnected())
                        return;

                    Net.sv.write.Start();
                    Net.sv.write.PacketID(Message.Type.ConsoleMessage);
                    Net.sv.write.String(ce.Reply);
                    Net.sv.write.Send(new SendInfo(arg.connection));
                } else {
                    arg.ReplyWith(ce.Reply);
                }
            }
        }

        #endregion

        public static void SetModded()
        {
            try {
                if (global::Rust.Global.SteamServer == null)
                    return;
                
                using (TimeWarning.New("UpdateServerInformation", 0.1f)) {
                    var steamServer = global::Rust.Global.SteamServer;

                    System.Reflection.Assembly assembly = typeof(ServerMgr).Assembly;
                    byte[] byteArray = System.IO.File.ReadAllBytes(assembly.Location);
                    Ionic.Crc.CRC32 cRC = new Ionic.Crc.CRC32();

                    cRC.SlurpBlock(byteArray, 0, byteArray.Length);

                    string _AssemblyHash = cRC.Crc32Result.ToString("x");

                    steamServer.ServerName = ConVar.Server.hostname;
                    steamServer.MaxPlayers = ConVar.Server.maxplayers;
                    steamServer.Passworded = false;
                    steamServer.MapName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

                    string gameTags = string.Format("mp{0},cp{1},qp{5},v{2}{3},h{4}", new object[] {
                        ConVar.Server.maxplayers,
                        BasePlayer.activePlayerList.Count,
                        global::Rust.Protocol.network,
                        ConVar.Server.pve ? ",pve" : string.Empty,
                        pluton.enabled ? ",modded,pluton" : string.Empty,
                        _AssemblyHash,
                        ServerMgr.Instance.connectionQueue.Queued
                    });

                    steamServer.GameTags = gameTags;

                    string[] array = ConVar.Server.description.SplitToChunks(100).ToArray();

                    for (int i = 0; i < 16; i++) {
                        if (i < array.Length) {
                            steamServer.SetKey(string.Format("description_{0:00}", i), array[i]);
                        } else {
                            steamServer.SetKey(string.Format("description_{0:00}", i), string.Empty);
                        }
                    }

                    steamServer.SetKey("hash", _AssemblyHash);
                    steamServer.SetKey("world.seed", global::World.Seed.ToString());
                    steamServer.SetKey("world.size", global::World.Size.ToString());
                    steamServer.SetKey("pve", ConVar.Server.pve.ToString());
                    steamServer.SetKey("headerimage", ConVar.Server.headerimage);
                    steamServer.SetKey("url", ConVar.Server.url);
                    steamServer.SetKey("uptime", ((int)Time.realtimeSinceStartup).ToString());
                    steamServer.SetKey("mem_ws", Performance.report.usedMemoryWorkingSetMB.ToString());
                    steamServer.SetKey("mem_pv", Performance.report.usedMemoryPrivateMB.ToString());
                    steamServer.SetKey("gc_mb", Performance.report.memoryAllocations.ToString());
                    steamServer.SetKey("gc_cl", Performance.report.memoryCollections.ToString());
                    steamServer.SetKey("fps", Performance.report.frameRate.ToString());
                    steamServer.SetKey("fps_avg", Performance.report.frameRateAverage.ToString("0.00"));
                    steamServer.SetKey("ent_cnt", BaseNetworkable.serverEntities.Count.ToString());
                    steamServer.SetKey("build", BuildInformation.VersionStampDays.ToString());
                }
            } catch (Exception ex) {
                Logger.LogError("[Hooks] Error while setting the server modded.");
                Logger.LogException(ex);
            }
        }
    }
}
