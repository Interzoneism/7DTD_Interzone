using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016B1 RID: 5809
	[Preserve]
	public class ActionSpawnEntitySpawner : ActionSpawnEntity
	{
		// Token: 0x1700139A RID: 5018
		// (get) Token: 0x0600B099 RID: 45209 RVA: 0x000197A5 File Offset: 0x000179A5
		public override bool UseRepeating
		{
			[PublicizedFrom(EAccessModifier.Protected)]
			get
			{
				return true;
			}
		}

		// Token: 0x0600B09A RID: 45210 RVA: 0x0044F31F File Offset: 0x0044D51F
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
			this.AddToGroup = this.AddToGroup + "," + this.internalGroupName;
			base.OnInit();
		}

		// Token: 0x0600B09B RID: 45211 RVA: 0x0044F344 File Offset: 0x0044D544
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void HandleExtraAction()
		{
			if (this.spawnOnHit && base.Owner.EventVariables.EventVariables.ContainsKey("Damaged"))
			{
				this.newZombieNeeded += (int)base.Owner.EventVariables.EventVariables["Damaged"];
				base.Owner.EventVariables.EventVariables.Remove("Damaged");
			}
		}

		// Token: 0x0600B09C RID: 45212 RVA: 0x0044F3BC File Offset: 0x0044D5BC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool HandleRepeat()
		{
			EntityPlayer entityPlayer = base.Owner.Target as EntityPlayer;
			if (entityPlayer == null)
			{
				return false;
			}
			if (base.Owner.GetEntityGroupLiveCount(this.internalGroupName) < this.spawnerMin + base.GetPartyAdditionCount(entityPlayer))
			{
				return true;
			}
			if (this.newZombieNeeded > 0)
			{
				this.newZombieNeeded--;
				return true;
			}
			return false;
		}

		// Token: 0x0600B09D RID: 45213 RVA: 0x0044F41C File Offset: 0x0044D61C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseInt(ActionSpawnEntitySpawner.PropSpawnerMin, ref this.spawnerMin);
			properties.ParseBool(ActionSpawnEntitySpawner.PropSpawnOnHit, ref this.spawnOnHit);
		}

		// Token: 0x0600B09E RID: 45214 RVA: 0x0044F448 File Offset: 0x0044D648
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionSpawnEntitySpawner
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
				spawnerMin = this.spawnerMin,
				spawnOnHit = this.spawnOnHit,
				spawnSound = this.spawnSound
			};
		}

		// Token: 0x04008A16 RID: 35350
		public int spawnerMin = 5;

		// Token: 0x04008A17 RID: 35351
		public bool spawnOnHit = true;

		// Token: 0x04008A18 RID: 35352
		[PublicizedFrom(EAccessModifier.Private)]
		public string internalGroupName = "_spawner";

		// Token: 0x04008A19 RID: 35353
		[PublicizedFrom(EAccessModifier.Private)]
		public int newZombieNeeded;

		// Token: 0x04008A1A RID: 35354
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSpawnerMin = "spawner_min";

		// Token: 0x04008A1B RID: 35355
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSpawnOnHit = "spawn_on_hit";
	}
}
