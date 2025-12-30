using System;
using System.Collections.Generic;
using Audio;
using GameEvent.GameEventHelpers;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000446 RID: 1094
[Preserve]
public class EntityHomerunGoal : Entity
{
	// Token: 0x0600225B RID: 8795 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanCollideWithBlocks()
	{
		return false;
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isEntityStatic()
	{
		return true;
	}

	// Token: 0x0600225D RID: 8797 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanBePushed()
	{
		return false;
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x000D88C4 File Offset: 0x000D6AC4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		World world = GameManager.Instance.World;
		if (world == null)
		{
			this.ReadyForDelete = true;
			return;
		}
		if (this.Owner == null)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				world.RemoveEntity(this.entityId, EnumRemoveEntityReason.Killed);
			}
			return;
		}
		this.TimeRemaining -= Time.deltaTime;
		if (this.TimeRemaining <= 0f)
		{
			this.ReadyForDelete = true;
			return;
		}
		if (this.IsMoving)
		{
			switch (this.direction)
			{
			case EntityHomerunGoal.Direction.YPositive:
				this.SetPosition(this.StartPosition + Vector3.up * Mathf.PingPong(Time.time, 2f) * 2f, true);
				break;
			case EntityHomerunGoal.Direction.XPositive:
				this.SetPosition(this.StartPosition + Vector3.right * Mathf.PingPong(Time.time, 2f) * 2f, true);
				break;
			case EntityHomerunGoal.Direction.XNegative:
				this.SetPosition(this.StartPosition + Vector3.left * Mathf.PingPong(Time.time, 2f) * 2f, true);
				break;
			case EntityHomerunGoal.Direction.ZPositive:
				this.SetPosition(this.StartPosition + Vector3.forward * Mathf.PingPong(Time.time, 2f) * 2f, true);
				break;
			case EntityHomerunGoal.Direction.ZNegative:
				this.SetPosition(this.StartPosition + Vector3.back * Mathf.PingPong(Time.time, 2f) * 2f, true);
				break;
			}
		}
		if (Vector3.Distance(this.position, this.Owner.Player.position) > 50f)
		{
			this.ReadyForDelete = true;
			return;
		}
		List<Entity> entitiesInBounds = GameManager.Instance.World.GetEntitiesInBounds(null, new Bounds(this.position, Vector3.one * this.Size));
		for (int i = 0; i < entitiesInBounds.Count; i++)
		{
			EntityAlive entityAlive = entitiesInBounds[i] as EntityAlive;
			if (entityAlive != null && entityAlive != null && entityAlive.IsAlive() && !(entityAlive is EntityPlayer) && entityAlive.emodel != null && entityAlive.emodel.transform != null && entityAlive.emodel.IsRagdollActive)
			{
				float lightBrightness = world.GetLightBrightness(entityAlive.GetBlockPosition());
				world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect("twitch_fireworks", entityAlive.position, lightBrightness, Color.white, null, null, false), entityAlive.entityId, false, true);
				Manager.BroadcastPlayByLocalPlayer(entityAlive.position, "twitch_baseball_balloon_pop");
				entityAlive.DamageEntity(new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.Suicide), 99999, false, 1f);
				world.RemoveEntity(entityAlive.entityId, EnumRemoveEntityReason.Killed);
				if (!this.ReadyForDelete)
				{
					this.Owner.Score += this.ScoreAdded;
					this.ReadyForDelete = true;
				}
				this.Owner.AddScoreDisplay(this.position);
				return;
			}
		}
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x000D8C14 File Offset: 0x000D6E14
	public override void CopyPropertiesFromEntityClass()
	{
		base.CopyPropertiesFromEntityClass();
		EntityClass entityClass = EntityClass.list[this.entityClass];
		entityClass.Properties.ParseInt("ScoreAdded", ref this.ScoreAdded);
		entityClass.Properties.ParseFloat("Size", ref this.Size);
		entityClass.Properties.ParseBool("IsMoving", ref this.IsMoving);
	}

	// Token: 0x040019A5 RID: 6565
	public HomerunData Owner;

	// Token: 0x040019A6 RID: 6566
	public bool ReadyForDelete;

	// Token: 0x040019A7 RID: 6567
	public int ScoreAdded = 1;

	// Token: 0x040019A8 RID: 6568
	public float Size = 2f;

	// Token: 0x040019A9 RID: 6569
	public Vector3 StartPosition;

	// Token: 0x040019AA RID: 6570
	public float TimeRemaining = 20f;

	// Token: 0x040019AB RID: 6571
	public bool IsMoving = true;

	// Token: 0x040019AC RID: 6572
	public EntityHomerunGoal.Direction direction;

	// Token: 0x02000447 RID: 1095
	public enum Direction
	{
		// Token: 0x040019AE RID: 6574
		YPositive,
		// Token: 0x040019AF RID: 6575
		XPositive,
		// Token: 0x040019B0 RID: 6576
		XNegative,
		// Token: 0x040019B1 RID: 6577
		ZPositive,
		// Token: 0x040019B2 RID: 6578
		ZNegative,
		// Token: 0x040019B3 RID: 6579
		Max
	}
}
