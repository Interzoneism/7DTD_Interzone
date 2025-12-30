using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200169C RID: 5788
	[Preserve]
	public class ActionRespawnEntity : BaseAction
	{
		// Token: 0x0600B03B RID: 45115 RVA: 0x0044DAFA File Offset: 0x0044BCFA
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
			base.OnInit();
			this.AddToGroups = this.addToGroup.Split(',', StringSplitOptions.None);
		}

		// Token: 0x0600B03C RID: 45116 RVA: 0x0044DB18 File Offset: 0x0044BD18
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (this.oldEntityClass == -1 && base.Owner.Target != null && !(base.Owner.Target is EntityPlayer))
			{
				Entity target = base.Owner.Target;
				this.oldEntityClass = target.entityClass;
				this.oldEntityID = target.entityId;
				this.oldPosition = target.position;
				this.oldRotation = target.rotation;
			}
			if (this.delay > 0f)
			{
				this.delay -= Time.deltaTime;
				return BaseAction.ActionCompleteStates.InComplete;
			}
			World world = GameManager.Instance.World;
			GameEventActionSequence gameEventActionSequence = (base.Owner.OwnerSequence == null) ? base.Owner : base.Owner.OwnerSequence;
			Entity entity = EntityFactory.CreateEntity(this.oldEntityClass, this.oldPosition, this.oldRotation, gameEventActionSequence.Target.entityId, gameEventActionSequence.ExtraData);
			if (entity == null)
			{
				return BaseAction.ActionCompleteStates.Complete;
			}
			entity.SetSpawnerSource(EnumSpawnerSource.Dynamic);
			world.SpawnEntityInWorld(entity);
			world.RemoveEntity(this.oldEntityID, EnumRemoveEntityReason.Killed);
			base.Owner.Target = entity;
			EntityAlive entityAlive = entity as EntityAlive;
			EntityAlive entityAlive2 = gameEventActionSequence.Target as EntityAlive;
			if (entityAlive2 != null)
			{
				GameEventManager.Current.RegisterSpawnedEntity(entityAlive, entityAlive2, gameEventActionSequence.Requester, gameEventActionSequence, true);
				entityAlive.SetAttackTarget(entityAlive2, 12000);
			}
			if (base.Owner.Requester != null)
			{
				if (gameEventActionSequence.Requester is EntityPlayerLocal)
				{
					GameEventManager.Current.HandleGameEntitySpawned(gameEventActionSequence.Name, entityAlive.entityId, gameEventActionSequence.Tag);
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(gameEventActionSequence.Name, gameEventActionSequence.Target.entityId, gameEventActionSequence.ExtraData, gameEventActionSequence.Tag, NetPackageGameEventResponse.ResponseTypes.TwitchSetOwner, entityAlive.entityId, -1, false), false, gameEventActionSequence.Requester.entityId, -1, -1, null, 192, false);
				}
			}
			if (entity != null && this.AddToGroups != null)
			{
				for (int i = 0; i < this.AddToGroups.Length; i++)
				{
					if (this.AddToGroups[i] != "")
					{
						base.Owner.AddEntityToGroup(this.AddToGroups[i], entity);
					}
				}
			}
			if (this.respawnSound != "")
			{
				Manager.BroadcastPlayByLocalPlayer(this.oldPosition, this.respawnSound);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B03D RID: 45117 RVA: 0x0044DD8C File Offset: 0x0044BF8C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionRespawnEntity.PropTargetGroup, ref this.targetGroup);
			properties.ParseString(ActionRespawnEntity.PropRespawnSound, ref this.respawnSound);
			properties.ParseString(ActionRespawnEntity.PropAddToGroup, ref this.addToGroup);
			properties.ParseFloat(ActionRespawnEntity.PropDelay, ref this.delay);
		}

		// Token: 0x0600B03E RID: 45118 RVA: 0x0044DDE4 File Offset: 0x0044BFE4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRespawnEntity
			{
				targetGroup = this.targetGroup,
				addToGroup = this.addToGroup,
				AddToGroups = this.AddToGroups,
				respawnSound = this.respawnSound,
				delay = this.delay
			};
		}

		// Token: 0x040089B9 RID: 35257
		[PublicizedFrom(EAccessModifier.Protected)]
		public string targetGroup = "";

		// Token: 0x040089BA RID: 35258
		[PublicizedFrom(EAccessModifier.Protected)]
		public string addToGroup = "";

		// Token: 0x040089BB RID: 35259
		[PublicizedFrom(EAccessModifier.Protected)]
		public string[] AddToGroups;

		// Token: 0x040089BC RID: 35260
		[PublicizedFrom(EAccessModifier.Protected)]
		public string respawnSound = "";

		// Token: 0x040089BD RID: 35261
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetGroup = "target_group";

		// Token: 0x040089BE RID: 35262
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAddToGroup = "add_to_group";

		// Token: 0x040089BF RID: 35263
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropRespawnSound = "respawn_sound";

		// Token: 0x040089C0 RID: 35264
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropDelay = "delay";

		// Token: 0x040089C1 RID: 35265
		[PublicizedFrom(EAccessModifier.Private)]
		public int oldEntityClass = -1;

		// Token: 0x040089C2 RID: 35266
		[PublicizedFrom(EAccessModifier.Private)]
		public int oldEntityID = -1;

		// Token: 0x040089C3 RID: 35267
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 oldPosition;

		// Token: 0x040089C4 RID: 35268
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 oldRotation;

		// Token: 0x040089C5 RID: 35269
		[PublicizedFrom(EAccessModifier.Private)]
		public float delay = 3f;
	}
}
