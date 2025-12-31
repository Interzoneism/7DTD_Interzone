using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001682 RID: 5762
	[Preserve]
	public class ActionPushEntity : ActionBaseTargetAction
	{
		// Token: 0x0600AFB8 RID: 44984 RVA: 0x0044B0A8 File Offset: 0x004492A8
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null)
			{
				DamageResponse dmResponse = DamageResponse.New(false);
				dmResponse.StunDuration = 1f;
				dmResponse.Strength = (int)(this.force * 100f);
				Vector3 vector = Vector3.zero;
				switch (this.direction)
				{
				case ActionPushEntity.Direction.Random:
				{
					GameRandom random = GameEventManager.Current.Random;
					vector = new Vector3(random.RandomFloat, random.RandomFloat, random.RandomFloat);
					vector.Normalize();
					break;
				}
				case ActionPushEntity.Direction.Forward:
					vector = entityAlive.transform.forward;
					break;
				case ActionPushEntity.Direction.Backward:
					vector = entityAlive.transform.forward * -1f;
					break;
				case ActionPushEntity.Direction.Left:
					vector = entityAlive.transform.right * -1f;
					break;
				case ActionPushEntity.Direction.Right:
					vector = entityAlive.transform.right;
					break;
				}
				dmResponse.Source = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Bashing, vector);
				entityAlive.DoRagdoll(dmResponse);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AFB9 RID: 44985 RVA: 0x0044B1A8 File Offset: 0x004493A8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckValidPosition(ref Vector3 newPoint, EntityAlive target)
		{
			World world = GameManager.Instance.World;
			Ray ray = new Ray(target.position, (newPoint - target.position).normalized);
			if (Voxel.Raycast(world, ray, this.force, -538750981, 67, 0f))
			{
				newPoint = Voxel.voxelRayHitInfo.hit.pos - ray.direction * 0.1f;
			}
			BlockValue block = world.GetBlock(new Vector3i(newPoint - ray.direction * 0.5f));
			if (block.Block.IsCollideMovement || block.Block.IsCollideArrows)
			{
				newPoint = Voxel.voxelRayHitInfo.hit.pos - ray.direction;
				block = world.GetBlock(new Vector3i(newPoint - ray.direction * 0.5f));
				if (block.Block.IsCollideMovement || block.Block.IsCollideArrows)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600AFBA RID: 44986 RVA: 0x0044B2D8 File Offset: 0x004494D8
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseEnum<ActionPushEntity.Direction>(ActionPushEntity.PropDirection, ref this.direction);
			properties.ParseFloat(ActionPushEntity.PropForce, ref this.force);
		}

		// Token: 0x0600AFBB RID: 44987 RVA: 0x0044B303 File Offset: 0x00449503
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionPushEntity
			{
				targetGroup = this.targetGroup,
				force = this.force,
				direction = this.direction
			};
		}

		// Token: 0x04008946 RID: 35142
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionPushEntity.Direction direction;

		// Token: 0x04008947 RID: 35143
		[PublicizedFrom(EAccessModifier.Protected)]
		public float force = 2f;

		// Token: 0x04008948 RID: 35144
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropDirection = "direction";

		// Token: 0x04008949 RID: 35145
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropForce = "distance";

		// Token: 0x02001683 RID: 5763
		public enum Direction
		{
			// Token: 0x0400894B RID: 35147
			Random,
			// Token: 0x0400894C RID: 35148
			Forward,
			// Token: 0x0400894D RID: 35149
			Backward,
			// Token: 0x0400894E RID: 35150
			Left,
			// Token: 0x0400894F RID: 35151
			Right
		}
	}
}
