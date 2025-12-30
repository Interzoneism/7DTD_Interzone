using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001696 RID: 5782
	[Preserve]
	public class ActionResetPlayerData : ActionBaseClientAction
	{
		// Token: 0x0600B01E RID: 45086 RVA: 0x0044CBE8 File Offset: 0x0044ADE8
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				GameManager instance = GameManager.Instance;
				if (this.removeQuests)
				{
					entityPlayerLocal.QuestJournal.Clear();
				}
				if (this.removeBackpack)
				{
					entityPlayerLocal.SetDroppedBackpackPositions(null);
					if (entityPlayerLocal.persistentPlayerData != null)
					{
						entityPlayerLocal.persistentPlayerData.ClearDroppedBackpacks();
					}
				}
				entityPlayerLocal.Progression.ResetProgression(this.resetLevels || this.resetSkills, this.removeBooks, this.removeCrafting);
				if (this.resetLevels)
				{
					entityPlayerLocal.Progression.Level = 1;
					entityPlayerLocal.Progression.ExpToNextLevel = entityPlayerLocal.Progression.GetExpForNextLevel();
					entityPlayerLocal.Progression.SkillPoints = entityPlayerLocal.QuestJournal.GetRewardedSkillPoints();
					entityPlayerLocal.Progression.ExpDeficit = 0;
					entityPlayerLocal.Buffs.SetCustomVar("$PlayerLevelBonus", 0f, true, CVarOperation.set);
					entityPlayerLocal.Buffs.SetCustomVar("$LastPlayerLevel", 1f, true, CVarOperation.set);
				}
				if (this.resetStats)
				{
					entityPlayerLocal.KilledZombies = 0;
					entityPlayerLocal.KilledPlayers = 0;
					entityPlayerLocal.Died = 0;
					entityPlayerLocal.distanceWalked = 0f;
					entityPlayerLocal.totalItemsCrafted = 0U;
					entityPlayerLocal.longestLife = 0f;
					entityPlayerLocal.currentLife = 0f;
				}
				if (this.removeCrafting)
				{
					List<Recipe> recipes = CraftingManager.GetRecipes();
					for (int i = 0; i < recipes.Count; i++)
					{
						if (recipes[i].IsLearnable)
						{
							entityPlayerLocal.Buffs.RemoveCustomVar(recipes[i].GetName());
						}
					}
					List<string> list = null;
					foreach (string text in entityPlayerLocal.Buffs.CVars.Keys)
					{
						if (text.StartsWith("_craftCount_"))
						{
							if (list == null)
							{
								list = new List<string>();
							}
							list.Add(text);
						}
					}
					if (list != null)
					{
						for (int j = 0; j < list.Count; j++)
						{
							entityPlayerLocal.Buffs.RemoveCustomVar(list[j]);
						}
					}
				}
				if (this.removeLandclaims)
				{
					PersistentPlayerData playerDataFromEntityID = instance.persistentPlayers.GetPlayerDataFromEntityID(target.entityId);
					if (playerDataFromEntityID.LPBlocks != null)
					{
						for (int k = 0; k < playerDataFromEntityID.LPBlocks.Count; k++)
						{
							instance.persistentPlayers.m_lpBlockMap.Remove(playerDataFromEntityID.LPBlocks[k]);
						}
						playerDataFromEntityID.LPBlocks.Clear();
					}
					NavObjectManager.Instance.UnRegisterNavObjectByOwnerEntity(entityPlayerLocal, "land_claim");
				}
				if (this.removeSleepingBag)
				{
					PersistentPlayerData playerDataFromEntityID2 = instance.persistentPlayers.GetPlayerDataFromEntityID(target.entityId);
					entityPlayerLocal.RemoveSpawnPoints(false);
					playerDataFromEntityID2.ClearBedroll();
				}
				if (this.removeChallenges)
				{
					entityPlayerLocal.challengeJournal.ResetChallenges();
				}
			}
		}

		// Token: 0x0600B01F RID: 45087 RVA: 0x0044CEBC File Offset: 0x0044B0BC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnServerPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				GameManager instance = GameManager.Instance;
				PersistentPlayerData playerDataFromEntityID = instance.persistentPlayers.GetPlayerDataFromEntityID(target.entityId);
				if (this.removeBackpack)
				{
					List<Entity> list = instance.World.Entities.list;
					for (int i = 0; i < list.Count; i++)
					{
						EntityBackpack entityBackpack = list[i] as EntityBackpack;
						if (entityBackpack != null && entityBackpack.RefPlayerId == entityPlayer.entityId)
						{
							entityBackpack.RefPlayerId = -1;
						}
					}
					entityPlayer.ClearDroppedBackpackPositions();
					if (playerDataFromEntityID != null)
					{
						playerDataFromEntityID.ClearDroppedBackpacks();
					}
				}
				entityPlayer.Progression.ResetProgression(this.resetLevels || this.resetSkills, this.removeBooks, this.removeCrafting);
				if (this.resetLevels)
				{
					entityPlayer.Progression.Level = 1;
					entityPlayer.Progression.ExpToNextLevel = entityPlayer.Progression.GetExpForNextLevel();
					entityPlayer.Progression.SkillPoints = entityPlayer.QuestJournal.GetRewardedSkillPoints();
					entityPlayer.Progression.ExpDeficit = 0;
					entityPlayer.Buffs.SetCustomVar("$PlayerLevelBonus", 0f, true, CVarOperation.set);
					entityPlayer.Buffs.SetCustomVar("$LastPlayerLevel", 1f, true, CVarOperation.set);
				}
				if (this.resetStats)
				{
					entityPlayer.KilledZombies = 0;
					entityPlayer.KilledPlayers = 0;
					entityPlayer.Died = 0;
					entityPlayer.distanceWalked = 0f;
					entityPlayer.totalItemsCrafted = 0U;
					entityPlayer.longestLife = 0f;
					entityPlayer.currentLife = 0f;
				}
				if (this.removeCrafting)
				{
					List<Recipe> recipes = CraftingManager.GetRecipes();
					for (int j = 0; j < recipes.Count; j++)
					{
						if (recipes[j].IsLearnable)
						{
							entityPlayer.Buffs.RemoveCustomVar(recipes[j].GetName());
						}
					}
					List<string> list2 = null;
					foreach (string text in entityPlayer.Buffs.CVars.Keys)
					{
						if (text.StartsWith("_craftCount_"))
						{
							if (list2 == null)
							{
								list2 = new List<string>();
							}
							list2.Add(text);
						}
					}
					if (list2 != null)
					{
						for (int k = 0; k < list2.Count; k++)
						{
							entityPlayer.Buffs.RemoveCustomVar(list2[k]);
						}
					}
				}
				if (this.removeLandclaims && playerDataFromEntityID.LPBlocks != null)
				{
					for (int l = 0; l < playerDataFromEntityID.LPBlocks.Count; l++)
					{
						instance.persistentPlayers.m_lpBlockMap.Remove(playerDataFromEntityID.LPBlocks[l]);
					}
					playerDataFromEntityID.LPBlocks.Clear();
				}
				if (this.removeSleepingBag)
				{
					playerDataFromEntityID.ClearBedroll();
				}
				if (this.removeChallenges && entityPlayer is EntityPlayerLocal)
				{
					entityPlayer.challengeJournal.ResetChallenges();
				}
			}
		}

		// Token: 0x0600B020 RID: 45088 RVA: 0x0044D1A8 File Offset: 0x0044B3A8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseBool(ActionResetPlayerData.PropResetLevels, ref this.resetLevels);
			properties.ParseBool(ActionResetPlayerData.PropResetSkills, ref this.resetSkills);
			properties.ParseBool(ActionResetPlayerData.PropRemoveLandClaims, ref this.removeLandclaims);
			properties.ParseBool(ActionResetPlayerData.PropRemoveSleepingBag, ref this.removeSleepingBag);
			properties.ParseBool(ActionResetPlayerData.PropRemoveBooks, ref this.removeBooks);
			properties.ParseBool(ActionResetPlayerData.PropRemoveCrafting, ref this.removeCrafting);
			properties.ParseBool(ActionResetPlayerData.PropRemoveQuests, ref this.removeQuests);
			properties.ParseBool(ActionResetPlayerData.PropRemoveChallenges, ref this.removeChallenges);
			properties.ParseBool(ActionResetPlayerData.PropRemoveBackpack, ref this.removeBackpack);
			properties.ParseBool(ActionResetPlayerData.PropResetStats, ref this.resetStats);
		}

		// Token: 0x0600B021 RID: 45089 RVA: 0x0044D268 File Offset: 0x0044B468
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionResetPlayerData
			{
				resetLevels = this.resetLevels,
				resetSkills = this.resetSkills,
				removeLandclaims = this.removeLandclaims,
				targetGroup = this.targetGroup,
				removeSleepingBag = this.removeSleepingBag,
				removeBooks = this.removeBooks,
				removeCrafting = this.removeCrafting,
				removeQuests = this.removeQuests,
				removeChallenges = this.removeChallenges,
				removeBackpack = this.removeBackpack,
				resetStats = this.resetStats
			};
		}

		// Token: 0x0400898C RID: 35212
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool resetLevels;

		// Token: 0x0400898D RID: 35213
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool removeLandclaims;

		// Token: 0x0400898E RID: 35214
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool removeSleepingBag;

		// Token: 0x0400898F RID: 35215
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool resetSkills;

		// Token: 0x04008990 RID: 35216
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool removeBooks;

		// Token: 0x04008991 RID: 35217
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool removeCrafting;

		// Token: 0x04008992 RID: 35218
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool removeQuests;

		// Token: 0x04008993 RID: 35219
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool removeChallenges;

		// Token: 0x04008994 RID: 35220
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool removeBackpack;

		// Token: 0x04008995 RID: 35221
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool resetStats;

		// Token: 0x04008996 RID: 35222
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropResetLevels = "reset_levels";

		// Token: 0x04008997 RID: 35223
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropResetSkills = "reset_skills";

		// Token: 0x04008998 RID: 35224
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRemoveLandClaims = "remove_landclaims";

		// Token: 0x04008999 RID: 35225
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRemoveSleepingBag = "remove_bedroll";

		// Token: 0x0400899A RID: 35226
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRemoveBooks = "reset_books";

		// Token: 0x0400899B RID: 35227
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRemoveCrafting = "reset_crafting";

		// Token: 0x0400899C RID: 35228
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRemoveQuests = "remove_quests";

		// Token: 0x0400899D RID: 35229
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRemoveChallenges = "remove_challenges";

		// Token: 0x0400899E RID: 35230
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRemoveBackpack = "remove_backpack";

		// Token: 0x0400899F RID: 35231
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropResetStats = "reset_stats";
	}
}
