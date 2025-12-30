using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001642 RID: 5698
	[Preserve]
	public class ActionAddClosestEntityToGroup : BaseAction
	{
		// Token: 0x0600AEA8 RID: 44712 RVA: 0x00443174 File Offset: 0x00441374
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			FastTags<TagGroup.Global> tags = FastTags<TagGroup.Global>.Parse(this.tag);
			List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(this.excludeTarget ? base.Owner.Target : null, new Bounds(base.Owner.Target.position, Vector3.one * 2f * this.maxDistance));
			List<Entity> list = new List<Entity>();
			Entity entity = null;
			float num = float.MaxValue;
			if (this.targetIsOwner)
			{
				PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerDataFromEntityID(base.Owner.Target.entityId);
				for (int i = 0; i < entitiesInBounds.Count; i++)
				{
					if (entitiesInBounds[i].HasAnyTags(tags))
					{
						EntityVehicle entityVehicle = entitiesInBounds[i] as EntityVehicle;
						if (entityVehicle != null)
						{
							if (this.targetIsOwner && !entityVehicle.IsOwner(playerDataFromEntityID.PrimaryId))
							{
								goto IL_15D;
							}
						}
						else
						{
							EntityTurret entityTurret = entitiesInBounds[i] as EntityTurret;
							if (entityTurret == null || entityTurret.OwnerID == null || (this.targetIsOwner && entityTurret.OwnerID != null && !entityTurret.OwnerID.Equals(playerDataFromEntityID.PrimaryId)))
							{
								goto IL_15D;
							}
						}
						float num2 = Vector3.Distance(base.Owner.Target.position, entitiesInBounds[i].position);
						if (num2 < num)
						{
							num = num2;
							entity = entitiesInBounds[i];
						}
					}
					IL_15D:;
				}
			}
			else
			{
				for (int j = 0; j < entitiesInBounds.Count; j++)
				{
					if (entitiesInBounds[j].HasAnyTags(tags))
					{
						float num3 = Vector3.Distance(base.Owner.Target.position, entitiesInBounds[j].position);
						if (num3 < num)
						{
							num = num3;
							entity = entitiesInBounds[j];
						}
					}
				}
			}
			if (entity == null)
			{
				return BaseAction.ActionCompleteStates.InCompleteRefund;
			}
			list.Add(entity);
			base.Owner.AddEntitiesToGroup(this.groupName, list, this.twitchNegative);
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AEA9 RID: 44713 RVA: 0x0044337C File Offset: 0x0044157C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionAddClosestEntityToGroup.PropGroupName, ref this.groupName);
			properties.ParseString(ActionAddClosestEntityToGroup.PropTag, ref this.tag);
			properties.ParseFloat(ActionAddClosestEntityToGroup.PropMaxDistance, ref this.maxDistance);
			properties.ParseBool(ActionAddClosestEntityToGroup.PropTwitchNegative, ref this.twitchNegative);
			properties.ParseBool(ActionAddClosestEntityToGroup.PropTargetIsOwner, ref this.targetIsOwner);
			properties.ParseBool(ActionAddClosestEntityToGroup.PropExcludeTarget, ref this.excludeTarget);
		}

		// Token: 0x0600AEAA RID: 44714 RVA: 0x004433F8 File Offset: 0x004415F8
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddClosestEntityToGroup
			{
				maxDistance = this.maxDistance,
				tag = this.tag,
				groupName = this.groupName,
				twitchNegative = this.twitchNegative,
				targetIsOwner = this.targetIsOwner,
				excludeTarget = this.excludeTarget
			};
		}

		// Token: 0x04008791 RID: 34705
		[PublicizedFrom(EAccessModifier.Protected)]
		public string groupName = "";

		// Token: 0x04008792 RID: 34706
		[PublicizedFrom(EAccessModifier.Protected)]
		public string tag;

		// Token: 0x04008793 RID: 34707
		[PublicizedFrom(EAccessModifier.Protected)]
		public float maxDistance = 10f;

		// Token: 0x04008794 RID: 34708
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool twitchNegative = true;

		// Token: 0x04008795 RID: 34709
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool targetIsOwner;

		// Token: 0x04008796 RID: 34710
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool excludeTarget = true;

		// Token: 0x04008797 RID: 34711
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGroupName = "group_name";

		// Token: 0x04008798 RID: 34712
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTag = "entity_tags";

		// Token: 0x04008799 RID: 34713
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxDistance = "max_distance";

		// Token: 0x0400879A RID: 34714
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTwitchNegative = "twitch_negative";

		// Token: 0x0400879B RID: 34715
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetIsOwner = "target_is_owner";

		// Token: 0x0400879C RID: 34716
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropExcludeTarget = "exclude_target";
	}
}
