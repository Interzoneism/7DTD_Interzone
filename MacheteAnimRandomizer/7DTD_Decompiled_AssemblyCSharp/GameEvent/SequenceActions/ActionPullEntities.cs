using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001681 RID: 5761
	[Preserve]
	public class ActionPullEntities : BaseAction
	{
		// Token: 0x0600AFB2 RID: 44978 RVA: 0x0044AE39 File Offset: 0x00449039
		public override bool CanPerform(Entity player)
		{
			return GameManager.Instance.World.CanPlaceBlockAt(new Vector3i(player.position), null, false);
		}

		// Token: 0x0600AFB3 RID: 44979 RVA: 0x0044AE5C File Offset: 0x0044905C
		public override BaseAction.ActionCompleteStates OnPerformAction()
		{
			if (this.targetGroup != "")
			{
				if (this.entityPullList == null)
				{
					this.entityPullList = new List<Entity>();
					List<Entity> entityGroup = base.Owner.GetEntityGroup(this.targetGroup);
					if (entityGroup != null)
					{
						this.entityPullList.AddRange(entityGroup);
						this.index = 0;
					}
					if (this.entityPullList.Count == 0)
					{
						return BaseAction.ActionCompleteStates.InCompleteRefund;
					}
				}
				else
				{
					Entity entity = this.entityPullList[this.index];
					if (entity.IsDead() || entity.IsDespawned)
					{
						this.index++;
						if (this.index >= this.entityPullList.Count)
						{
							return BaseAction.ActionCompleteStates.Complete;
						}
					}
					Vector3 zero = Vector3.zero;
					if (ActionBaseSpawn.FindValidPosition(out zero, base.Owner.Target, this.minDistance, this.maxDistance, false, 0f, false))
					{
						entity.SetPosition(zero, true);
						EntityAlive entityAlive = entity as EntityAlive;
						if (entityAlive != null)
						{
							EntityAlive entityAlive2 = base.Owner.Target as EntityAlive;
							if (entityAlive2 != null)
							{
								entityAlive.SetAttackTarget(entityAlive2, 12000);
							}
						}
						if (this.pullSound != "")
						{
							Manager.BroadcastPlayByLocalPlayer(zero, this.pullSound);
						}
						this.index++;
					}
					if (this.index >= this.entityPullList.Count)
					{
						return BaseAction.ActionCompleteStates.Complete;
					}
				}
				return BaseAction.ActionCompleteStates.InComplete;
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AFB4 RID: 44980 RVA: 0x0044AFB8 File Offset: 0x004491B8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionPullEntities.PropTargetGroup, ref this.targetGroup);
			properties.ParseString(ActionPullEntities.PropPullSound, ref this.pullSound);
			properties.ParseFloat(ActionPullEntities.PropMinDistance, ref this.minDistance);
			properties.ParseFloat(ActionPullEntities.PropMaxDistance, ref this.maxDistance);
		}

		// Token: 0x0600AFB5 RID: 44981 RVA: 0x0044B010 File Offset: 0x00449210
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionPullEntities
			{
				targetGroup = this.targetGroup,
				pullSound = this.pullSound,
				minDistance = this.minDistance,
				maxDistance = this.maxDistance
			};
		}

		// Token: 0x0400893C RID: 35132
		[PublicizedFrom(EAccessModifier.Protected)]
		public string targetGroup = "";

		// Token: 0x0400893D RID: 35133
		[PublicizedFrom(EAccessModifier.Protected)]
		public float minDistance = 7f;

		// Token: 0x0400893E RID: 35134
		[PublicizedFrom(EAccessModifier.Protected)]
		public float maxDistance = 9f;

		// Token: 0x0400893F RID: 35135
		[PublicizedFrom(EAccessModifier.Protected)]
		public string pullSound = "";

		// Token: 0x04008940 RID: 35136
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetGroup = "target_group";

		// Token: 0x04008941 RID: 35137
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMinDistance = "min_distance";

		// Token: 0x04008942 RID: 35138
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropMaxDistance = "max_distance";

		// Token: 0x04008943 RID: 35139
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPullSound = "pull_sound";

		// Token: 0x04008944 RID: 35140
		[PublicizedFrom(EAccessModifier.Private)]
		public List<Entity> entityPullList;

		// Token: 0x04008945 RID: 35141
		[PublicizedFrom(EAccessModifier.Private)]
		public int index;
	}
}
