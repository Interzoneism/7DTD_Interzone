using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016B0 RID: 5808
	[Preserve]
	public class ActionSpawnEntity : ActionBaseSpawn
	{
		// Token: 0x0600B093 RID: 45203 RVA: 0x0044F0C4 File Offset: 0x0044D2C4
		public override void AddPropertiesToSpawnedEntity(Entity entity)
		{
			if (this.AddBuffs != null)
			{
				EntityAlive entityAlive = entity as EntityAlive;
				if (entityAlive == null)
				{
					return;
				}
				for (int i = 0; i < this.AddBuffs.Length; i++)
				{
					entityAlive.Buffs.AddBuff(this.AddBuffs[i], -1, true, false, -1f);
				}
			}
		}

		// Token: 0x0600B094 RID: 45204 RVA: 0x0044F11C File Offset: 0x0044D31C
		public override void HandleTargeting(EntityAlive attacker, EntityAlive targetAlive)
		{
			base.HandleTargeting(attacker, targetAlive);
			attacker.SetMaxViewAngle(360f);
			attacker.sightRangeBase = 100f;
			attacker.SetSightLightThreshold(new Vector2(-2f, -2f));
			attacker.SetAttackTarget(targetAlive, 12000);
			if (this.onlyTargetPlayers)
			{
				attacker.aiManager.SetTargetOnlyPlayers(100f);
			}
		}

		// Token: 0x0600B095 RID: 45205 RVA: 0x0044F180 File Offset: 0x0044D380
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			string text = "";
			properties.ParseString(ActionSpawnEntity.PropAddBuffs, ref text);
			if (text != "")
			{
				this.AddBuffs = text.Split(',', StringSplitOptions.None);
			}
			properties.ParseBool(ActionBaseSpawn.PropIsAggressive, ref this.isAggressive);
		}

		// Token: 0x0600B096 RID: 45206 RVA: 0x0044F1D4 File Offset: 0x0044D3D4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSpawnEntity
			{
				count = this.count,
				currentCount = this.currentCount,
				entityNames = this.entityNames,
				maxDistance = this.maxDistance,
				minDistance = this.minDistance,
				safeSpawn = this.safeSpawn,
				airSpawn = this.airSpawn,
				singleChoice = this.singleChoice,
				targetGroup = this.targetGroup,
				partyAdditionText = this.partyAdditionText,
				AddToGroup = this.AddToGroup,
				AddToGroups = this.AddToGroups,
				AddBuffs = this.AddBuffs,
				spawnType = this.spawnType,
				clearPositionOnComplete = this.clearPositionOnComplete,
				yOffset = this.yOffset,
				attackTarget = this.attackTarget,
				useEntityGroup = this.useEntityGroup,
				ignoreMultiplier = this.ignoreMultiplier,
				onlyTargetPlayers = this.onlyTargetPlayers,
				raycastOffset = this.raycastOffset,
				isAggressive = this.isAggressive,
				spawnSound = this.spawnSound
			};
		}

		// Token: 0x04008A12 RID: 35346
		[PublicizedFrom(EAccessModifier.Protected)]
		public string[] AddBuffs;

		// Token: 0x04008A13 RID: 35347
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool onlyTargetPlayers = true;

		// Token: 0x04008A14 RID: 35348
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAddBuffs = "add_buffs";

		// Token: 0x04008A15 RID: 35349
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropOnlyTargetPlayers = "only_target_players";
	}
}
