using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001692 RID: 5778
	[Preserve]
	public class ActionReplaceEntities : ActionBaseTargetAction
	{
		// Token: 0x0600B001 RID: 45057 RVA: 0x0044C374 File Offset: 0x0044A574
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
			base.OnInit();
			string[] array = this.entityNames.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
				{
					if (keyValuePair.Value.entityClassName == array[i])
					{
						this.entityIDs.Add(keyValuePair.Key);
						if (this.entityIDs.Count == array.Length)
						{
							break;
						}
					}
				}
			}
			if (this.singleChoice && this.selectedEntityIndex == -1)
			{
				this.selectedEntityIndex = UnityEngine.Random.Range(0, this.entityIDs.Count);
			}
		}

		// Token: 0x0600B002 RID: 45058 RVA: 0x0044C44C File Offset: 0x0044A64C
		public override void StartTargetAction()
		{
			this.newList = new List<Entity>();
		}

		// Token: 0x0600B003 RID: 45059 RVA: 0x0044C459 File Offset: 0x0044A659
		public override void EndTargetAction()
		{
			if (this.targetGroup != "")
			{
				base.Owner.AddEntitiesToGroup(this.targetGroup, this.newList, false);
			}
		}

		// Token: 0x0600B004 RID: 45060 RVA: 0x0044C488 File Offset: 0x0044A688
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			World world = GameManager.Instance.World;
			if (target != null && !(target is EntityPlayer))
			{
				int index = (this.selectedEntityIndex == -1) ? UnityEngine.Random.Range(0, this.entityIDs.Count) : this.selectedEntityIndex;
				Entity entity = EntityFactory.CreateEntity(this.entityIDs[index], target.position, target.rotation, (base.Owner.Target != null) ? base.Owner.Target.entityId : -1, base.Owner.ExtraData);
				entity.SetSpawnerSource(EnumSpawnerSource.Dynamic);
				world.SpawnEntityInWorld(entity);
				this.newList.Add(entity);
				if (this.attackTarget)
				{
					EntityAlive entityAlive = entity as EntityAlive;
					if (entityAlive != null)
					{
						EntityAlive entityAlive2 = base.Owner.Target as EntityAlive;
						if (entityAlive2 != null)
						{
							GameEventManager.Current.RegisterSpawnedEntity(entityAlive, entityAlive2, base.Owner.Requester, base.Owner, true);
							entityAlive.SetAttackTarget(entityAlive2, 12000);
							entityAlive.aiManager.SetTargetOnlyPlayers(100f);
							if (base.Owner.Requester != null)
							{
								GameEventActionSequence gameEventActionSequence = (base.Owner.OwnerSequence == null) ? base.Owner : base.Owner.OwnerSequence;
								if (base.Owner.Requester is EntityPlayerLocal)
								{
									GameEventManager.Current.HandleGameEntitySpawned(gameEventActionSequence.Name, entity.entityId, gameEventActionSequence.Tag);
								}
								else
								{
									SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageGameEventResponse>().Setup(gameEventActionSequence.Name, gameEventActionSequence.Target.entityId, gameEventActionSequence.ExtraData, gameEventActionSequence.Tag, NetPackageGameEventResponse.ResponseTypes.EntitySpawned, entity.entityId, -1, false), false, gameEventActionSequence.Requester.entityId, -1, -1, null, 192, false);
								}
							}
						}
					}
				}
				this.HandleRemoveData(target);
				GameManager.Instance.StartCoroutine(this.removeLater(target));
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600B005 RID: 45061 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void HandleRemoveData(Entity ent)
		{
		}

		// Token: 0x0600B006 RID: 45062 RVA: 0x0044C691 File Offset: 0x0044A891
		[PublicizedFrom(EAccessModifier.Protected)]
		public IEnumerator removeLater(Entity e)
		{
			yield return new WaitForSeconds(0.25f);
			if (e is EntityVehicle)
			{
				(e as EntityVehicle).Kill();
			}
			GameManager.Instance.World.RemoveEntity(e.entityId, EnumRemoveEntityReason.Killed);
			yield break;
		}

		// Token: 0x0600B007 RID: 45063 RVA: 0x0044C6A0 File Offset: 0x0044A8A0
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionReplaceEntities.PropEntityNames))
			{
				this.entityNames = properties.Values[ActionReplaceEntities.PropEntityNames];
			}
			if (properties.Values.ContainsKey(ActionReplaceEntities.PropSingleChoice))
			{
				this.singleChoice = StringParsers.ParseBool(properties.Values[ActionReplaceEntities.PropSingleChoice], 0, -1, true);
			}
			properties.ParseBool(ActionReplaceEntities.PropAttackTarget, ref this.attackTarget);
		}

		// Token: 0x0600B008 RID: 45064 RVA: 0x0044C720 File Offset: 0x0044A920
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionReplaceEntities
			{
				entityNames = this.entityNames,
				entityIDs = this.entityIDs,
				singleChoice = this.singleChoice,
				targetGroup = this.targetGroup,
				selectedEntityIndex = this.selectedEntityIndex,
				attackTarget = this.attackTarget
			};
		}

		// Token: 0x04008979 RID: 35193
		[PublicizedFrom(EAccessModifier.Protected)]
		public string entityNames = "";

		// Token: 0x0400897A RID: 35194
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool singleChoice;

		// Token: 0x0400897B RID: 35195
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropEntityNames = "entity_names";

		// Token: 0x0400897C RID: 35196
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropSingleChoice = "single_choice";

		// Token: 0x0400897D RID: 35197
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropAttackTarget = "attack_target";

		// Token: 0x0400897E RID: 35198
		[PublicizedFrom(EAccessModifier.Protected)]
		public List<int> entityIDs = new List<int>();

		// Token: 0x0400897F RID: 35199
		[PublicizedFrom(EAccessModifier.Protected)]
		public int selectedEntityIndex = -1;

		// Token: 0x04008980 RID: 35200
		[PublicizedFrom(EAccessModifier.Protected)]
		public List<Entity> newList;

		// Token: 0x04008981 RID: 35201
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool attackTarget = true;
	}
}
