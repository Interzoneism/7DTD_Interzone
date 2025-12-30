using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016AF RID: 5807
	[Preserve]
	public class ActionSpawnContainer : ActionBaseSpawn
	{
		// Token: 0x0600B08E RID: 45198 RVA: 0x0044EED8 File Offset: 0x0044D0D8
		public override void AddPropertiesToSpawnedEntity(Entity entity)
		{
			base.AddPropertiesToSpawnedEntity(entity);
			entity.spawnByAllowShare = base.Owner.CrateShare;
			EntityLootContainer entityLootContainer = entity as EntityLootContainer;
			if (entityLootContainer != null)
			{
				if (this.overrideLootList != "")
				{
					string[] array = this.overrideLootList.Split(',', StringSplitOptions.None);
					entityLootContainer.OverrideLootList = array[entity.rand.RandomRange(array.Length)];
				}
				entityLootContainer.OverrideName = this.overrideName;
			}
		}

		// Token: 0x0600B08F RID: 45199 RVA: 0x0044EF4A File Offset: 0x0044D14A
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionSpawnContainer.PropOverrideLootList, ref this.overrideLootList);
			properties.ParseString(ActionSpawnContainer.PropOverrideName, ref this.overrideName);
		}

		// Token: 0x0600B090 RID: 45200 RVA: 0x0044EF78 File Offset: 0x0044D178
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSpawnContainer
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
				spawnType = this.spawnType,
				clearPositionOnComplete = this.clearPositionOnComplete,
				yOffset = this.yOffset,
				useEntityGroup = this.useEntityGroup,
				ignoreMultiplier = this.ignoreMultiplier,
				raycastOffset = this.raycastOffset,
				isAggressive = false,
				spawnSound = this.spawnSound,
				overrideLootList = this.overrideLootList,
				overrideName = this.overrideName
			};
		}

		// Token: 0x04008A0E RID: 35342
		[PublicizedFrom(EAccessModifier.Protected)]
		public string overrideLootList = "";

		// Token: 0x04008A0F RID: 35343
		[PublicizedFrom(EAccessModifier.Protected)]
		public string overrideName = "";

		// Token: 0x04008A10 RID: 35344
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropOverrideLootList = "override_loot_list";

		// Token: 0x04008A11 RID: 35345
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropOverrideName = "override_name";
	}
}
