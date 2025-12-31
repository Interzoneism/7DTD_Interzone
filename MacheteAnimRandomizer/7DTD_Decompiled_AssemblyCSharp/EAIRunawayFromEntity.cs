using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003EB RID: 1003
[Preserve]
public class EAIRunawayFromEntity : EAIRunAway
{
	// Token: 0x06001E5D RID: 7773 RVA: 0x000BAB17 File Offset: 0x000B8D17
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 1;
	}

	// Token: 0x06001E5E RID: 7774 RVA: 0x000BD804 File Offset: 0x000BBA04
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
		this.targetClasses = new List<Type>();
		string text;
		if (data.TryGetValue("class", out text))
		{
			string[] array = text.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i += 2)
			{
				Type entityType = EntityFactory.GetEntityType(array[i]);
				this.targetClasses.Add(entityType);
			}
		}
		base.GetData(data, "safeDistance", ref this.safeDistance);
		base.GetData(data, "minSneakDistance", ref this.minSneakDistance);
	}

	// Token: 0x06001E5F RID: 7775 RVA: 0x000BD883 File Offset: 0x000BBA83
	public override bool CanExecute()
	{
		this.FindEnemy();
		return !(this.avoidEntity == null) && base.CanExecute();
	}

	// Token: 0x06001E60 RID: 7776 RVA: 0x000BD8A4 File Offset: 0x000BBAA4
	public void FindEnemy()
	{
		this.avoidEntity = null;
		if (this.theEntity.noisePlayer && this.theEntity.noisePlayerVolume >= 8f)
		{
			this.avoidEntity = this.theEntity.noisePlayer;
			return;
		}
		float seeDistance = this.theEntity.GetSeeDistance();
		Bounds bb = BoundsUtils.ExpandBounds(this.theEntity.boundingBox, seeDistance, seeDistance, seeDistance);
		for (int i = 0; i < this.targetClasses.Count; i++)
		{
			Type type = this.targetClasses[i];
			this.theEntity.world.GetEntitiesInBounds(type, bb, EAIRunawayFromEntity.list);
			if (type == typeof(EntityPlayer))
			{
				float num = float.MaxValue;
				for (int j = 0; j < EAIRunawayFromEntity.list.Count; j++)
				{
					EntityPlayer entityPlayer = EAIRunawayFromEntity.list[j] as EntityPlayer;
					float seeDistance2 = this.manager.GetSeeDistance(entityPlayer);
					if (seeDistance2 < num && this.theEntity.CanSee(entityPlayer) && this.theEntity.CanSeeStealth(seeDistance2, entityPlayer.Stealth.lightLevel) && !entityPlayer.IsIgnoredByAI())
					{
						num = seeDistance2;
						this.avoidEntity = entityPlayer;
					}
				}
			}
			else
			{
				float num2 = float.MaxValue;
				for (int k = 0; k < EAIRunawayFromEntity.list.Count; k++)
				{
					EntityAlive entityAlive = EAIRunawayFromEntity.list[k] as EntityAlive;
					float distanceSq = this.theEntity.GetDistanceSq(entityAlive);
					if (distanceSq <= this.minSneakDistance * this.minSneakDistance)
					{
						this.avoidEntity = entityAlive;
						break;
					}
					if (distanceSq < num2 && this.theEntity.CanSee(entityAlive) && !entityAlive.IsIgnoredByAI())
					{
						num2 = distanceSq;
						this.avoidEntity = entityAlive;
					}
				}
			}
			EAIRunawayFromEntity.list.Clear();
			if (this.avoidEntity)
			{
				break;
			}
		}
	}

	// Token: 0x06001E61 RID: 7777 RVA: 0x000BDA8E File Offset: 0x000BBC8E
	public override bool Continue()
	{
		return this.theEntity.GetDistanceSq(this.avoidEntity) < this.safeDistance * this.safeDistance && base.Continue();
	}

	// Token: 0x06001E62 RID: 7778 RVA: 0x000BDAB8 File Offset: 0x000BBCB8
	public override void Reset()
	{
		this.avoidEntity = null;
	}

	// Token: 0x06001E63 RID: 7779 RVA: 0x000BDAC1 File Offset: 0x000BBCC1
	public override void Update()
	{
		base.Update();
		this.theEntity.navigator.setMoveSpeed(this.theEntity.IsSwimming() ? this.theEntity.GetMoveSpeed() : this.theEntity.GetMoveSpeedPanic());
	}

	// Token: 0x06001E64 RID: 7780 RVA: 0x000BDAFE File Offset: 0x000BBCFE
	[PublicizedFrom(EAccessModifier.Protected)]
	public override Vector3 GetFleeFromPos()
	{
		return this.avoidEntity.position;
	}

	// Token: 0x06001E65 RID: 7781 RVA: 0x000BDB0B File Offset: 0x000BBD0B
	public override string ToString()
	{
		return string.Format("{0}, {1}", base.ToString(), (this.avoidEntity != null) ? this.avoidEntity.GetDebugName() : "");
	}

	// Token: 0x040014F6 RID: 5366
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Type> targetClasses;

	// Token: 0x040014F7 RID: 5367
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive avoidEntity;

	// Token: 0x040014F8 RID: 5368
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRunNoiseVolume = 8;

	// Token: 0x040014F9 RID: 5369
	[PublicizedFrom(EAccessModifier.Private)]
	public float safeDistance = 38f;

	// Token: 0x040014FA RID: 5370
	[PublicizedFrom(EAccessModifier.Private)]
	public float minSneakDistance = 3.5f;

	// Token: 0x040014FB RID: 5371
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Entity> list = new List<Entity>();
}
