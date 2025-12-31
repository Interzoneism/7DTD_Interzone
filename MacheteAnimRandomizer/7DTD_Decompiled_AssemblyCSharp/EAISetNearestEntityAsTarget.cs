using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003F0 RID: 1008
[Preserve]
public class EAISetNearestEntityAsTarget : EAITarget
{
	// Token: 0x06001E7F RID: 7807 RVA: 0x000BE25B File Offset: 0x000BC45B
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity, 25f, true);
		this.MutexBits = 1;
		this.sorter = new EAISetNearestEntityAsTargetSorter(_theEntity);
	}

	// Token: 0x06001E80 RID: 7808 RVA: 0x000BE280 File Offset: 0x000BC480
	public override void SetData(DictionarySave<string, string> data)
	{
		base.SetData(data);
		this.targetClasses = new List<EAISetNearestEntityAsTarget.TargetClass>();
		string text;
		if (data.TryGetValue("class", out text))
		{
			string[] array = text.Split(',', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i += 3)
			{
				EAISetNearestEntityAsTarget.TargetClass targetClass;
				targetClass.type = EntityFactory.GetEntityType(array[i]);
				targetClass.hearDistMax = 0f;
				if (i + 1 < array.Length)
				{
					targetClass.hearDistMax = StringParsers.ParseFloat(array[i + 1], 0, -1, NumberStyles.Any);
				}
				if (targetClass.hearDistMax == 0f)
				{
					targetClass.hearDistMax = 50f;
				}
				targetClass.seeDistMax = 0f;
				if (i + 2 < array.Length)
				{
					targetClass.seeDistMax = StringParsers.ParseFloat(array[i + 2], 0, -1, NumberStyles.Any);
				}
				if (targetClass.type == typeof(EntityPlayer))
				{
					this.playerTargetClassIndex = this.targetClasses.Count;
				}
				this.targetClasses.Add(targetClass);
			}
		}
	}

	// Token: 0x06001E81 RID: 7809 RVA: 0x000BE384 File Offset: 0x000BC584
	public void SetTargetOnlyPlayers(float _distance)
	{
		this.targetClasses.Clear();
		EAISetNearestEntityAsTarget.TargetClass item = default(EAISetNearestEntityAsTarget.TargetClass);
		item.type = typeof(EntityPlayer);
		item.hearDistMax = _distance;
		item.seeDistMax = -_distance;
		this.targetClasses.Add(item);
		this.playerTargetClassIndex = 0;
	}

	// Token: 0x06001E82 RID: 7810 RVA: 0x000BE3DC File Offset: 0x000BC5DC
	public override bool CanExecute()
	{
		if (this.theEntity.distraction != null)
		{
			return false;
		}
		this.FindTarget();
		if (!this.closeTargetEntity)
		{
			return false;
		}
		this.targetEntity = this.closeTargetEntity;
		this.targetPlayer = (this.closeTargetEntity as EntityPlayer);
		return true;
	}

	// Token: 0x06001E83 RID: 7811 RVA: 0x000BE434 File Offset: 0x000BC634
	[PublicizedFrom(EAccessModifier.Private)]
	public void FindTarget()
	{
		this.closeTargetDist = float.MaxValue;
		this.closeTargetEntity = null;
		float seeDistance = this.theEntity.GetSeeDistance();
		for (int i = 0; i < this.targetClasses.Count; i++)
		{
			EAISetNearestEntityAsTarget.TargetClass targetClass = this.targetClasses[i];
			float num = seeDistance;
			if (targetClass.seeDistMax != 0f)
			{
				float v = (targetClass.seeDistMax < 0f) ? (-targetClass.seeDistMax) : (targetClass.seeDistMax * this.theEntity.senseScale);
				num = Utils.FastMin(num, v);
			}
			if (targetClass.type == typeof(EntityPlayer))
			{
				this.FindTargetPlayer(num);
				if (this.theEntity.noisePlayer && this.theEntity.noisePlayer != this.closeTargetEntity)
				{
					if (this.closeTargetEntity)
					{
						if (this.theEntity.noisePlayerVolume >= this.theEntity.sleeperNoiseToWake)
						{
							Vector3 position = this.theEntity.noisePlayer.position;
							float magnitude = (this.theEntity.position - position).magnitude;
							if (magnitude < this.closeTargetDist)
							{
								this.closeTargetDist = magnitude;
								this.closeTargetEntity = this.theEntity.noisePlayer;
							}
						}
					}
					else if (!this.theEntity.IsSleeping)
					{
						this.SeekNoise(this.theEntity.noisePlayer);
					}
				}
				if (this.closeTargetEntity)
				{
					EntityPlayer entityPlayer = (EntityPlayer)this.closeTargetEntity;
					if (entityPlayer.IsBloodMoonDead && entityPlayer.currentLife >= 0.5f)
					{
						Log.Out("Player {0}, living {1}, lost BM immunity", new object[]
						{
							entityPlayer.GetDebugName(),
							entityPlayer.currentLife * 60f
						});
						entityPlayer.IsBloodMoonDead = false;
					}
				}
			}
			else if (!this.theEntity.IsSleeping && !this.theEntity.HasInvestigatePosition)
			{
				this.theEntity.world.GetEntitiesInBounds(targetClass.type, BoundsUtils.ExpandBounds(this.theEntity.boundingBox, num, 4f, num), EAISetNearestEntityAsTarget.list);
				EAISetNearestEntityAsTarget.list.Sort(this.sorter);
				int j = 0;
				while (j < EAISetNearestEntityAsTarget.list.Count)
				{
					EntityAlive entityAlive = (EntityAlive)EAISetNearestEntityAsTarget.list[j];
					if (!(entityAlive is EntityDrone) && base.check(entityAlive))
					{
						float distance = this.theEntity.GetDistance(entityAlive);
						if (distance < this.closeTargetDist)
						{
							this.closeTargetDist = distance;
							this.closeTargetEntity = entityAlive;
							this.lastSeenPos = entityAlive.position;
							break;
						}
						break;
					}
					else
					{
						j++;
					}
				}
				EAISetNearestEntityAsTarget.list.Clear();
			}
		}
	}

	// Token: 0x06001E84 RID: 7812 RVA: 0x000BE70C File Offset: 0x000BC90C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SeekNoise(EntityPlayer player)
	{
		float num = (player.position - this.theEntity.position).magnitude;
		if (this.playerTargetClassIndex >= 0)
		{
			float num2 = this.targetClasses[this.playerTargetClassIndex].hearDistMax;
			num2 *= this.theEntity.senseScale;
			num2 *= player.DetectUsScale(this.theEntity);
			if (num > num2)
			{
				return;
			}
		}
		num *= 0.9f;
		if (num > this.manager.noiseSeekDist)
		{
			num = this.manager.noiseSeekDist;
		}
		if (this.theEntity.IsBloodMoon)
		{
			num = this.manager.noiseSeekDist * 0.25f;
		}
		Vector3 breadcrumbPos = player.GetBreadcrumbPos(num * base.RandomFloat);
		int ticks = this.theEntity.CalcInvestigateTicks((int)(30f + base.RandomFloat * 30f) * 20, player);
		this.theEntity.SetInvestigatePosition(breadcrumbPos, ticks, true);
		this.PlaySoundSenseNoise();
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x000BE808 File Offset: 0x000BCA08
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlaySoundSenseNoise()
	{
		float time = Time.time;
		if (this.senseSoundTime - time < 0f)
		{
			this.senseSoundTime = time + 10f + base.RandomFloat * 10f;
			this.theEntity.PlayOneShot(this.theEntity.soundSense, false, false, false, null);
		}
	}

	// Token: 0x06001E86 RID: 7814 RVA: 0x000BE860 File Offset: 0x000BCA60
	[PublicizedFrom(EAccessModifier.Private)]
	public void FindTargetPlayer(float seeDist)
	{
		if (this.theEntity.IsSleeperPassive)
		{
			return;
		}
		this.theEntity.world.GetEntitiesInBounds(typeof(EntityPlayer), BoundsUtils.ExpandBounds(this.theEntity.boundingBox, seeDist, seeDist, seeDist), EAISetNearestEntityAsTarget.list);
		if (!this.theEntity.IsSleeping)
		{
			for (int i = 0; i < EAISetNearestEntityAsTarget.list.Count; i++)
			{
				EntityPlayer entityPlayer = (EntityPlayer)EAISetNearestEntityAsTarget.list[i];
				if (entityPlayer.IsAlive() && !entityPlayer.IsIgnoredByAI())
				{
					float seeDistance = this.manager.GetSeeDistance(entityPlayer);
					if (seeDistance < this.closeTargetDist && this.theEntity.CanSee(entityPlayer) && this.theEntity.CanSeeStealth(seeDistance, entityPlayer.Stealth.lightLevel))
					{
						this.closeTargetDist = seeDistance;
						this.closeTargetEntity = entityPlayer;
					}
				}
			}
			EAISetNearestEntityAsTarget.list.Clear();
			return;
		}
		EAISetNearestEntityAsTarget.list.Sort(this.sorter);
		EntityPlayer x = null;
		float num = float.MaxValue;
		bool flag = false;
		if (this.theEntity.noisePlayer != null)
		{
			if (this.theEntity.noisePlayerVolume >= this.theEntity.sleeperNoiseToWake)
			{
				x = this.theEntity.noisePlayer;
				num = this.theEntity.noisePlayerDistance;
			}
			else if (this.theEntity.noisePlayerVolume >= this.theEntity.sleeperNoiseToSense)
			{
				flag = true;
			}
		}
		for (int j = 0; j < EAISetNearestEntityAsTarget.list.Count; j++)
		{
			EntityPlayer entityPlayer2 = (EntityPlayer)EAISetNearestEntityAsTarget.list[j];
			if (this.theEntity.CanSee(entityPlayer2) && !entityPlayer2.IsIgnoredByAI())
			{
				float distance = this.theEntity.GetDistance(entityPlayer2);
				int sleeperDisturbedLevel = this.theEntity.GetSleeperDisturbedLevel(distance, entityPlayer2.Stealth.lightLevel);
				if (sleeperDisturbedLevel >= 2)
				{
					if (distance < num)
					{
						x = entityPlayer2;
						num = distance;
					}
				}
				else if (sleeperDisturbedLevel >= 1)
				{
					flag = true;
				}
			}
		}
		EAISetNearestEntityAsTarget.list.Clear();
		if (x != null)
		{
			this.closeTargetDist = num;
			this.closeTargetEntity = x;
			return;
		}
		if (flag)
		{
			this.theEntity.Groan();
			return;
		}
		this.theEntity.Snore();
	}

	// Token: 0x06001E87 RID: 7815 RVA: 0x000BEA94 File Offset: 0x000BCC94
	public override void Start()
	{
		this.theEntity.SetAttackTarget(this.targetEntity, 200);
		this.theEntity.ConditionalTriggerSleeperWakeUp();
		this.PlaySoundSenseNoise();
		base.Start();
	}

	// Token: 0x06001E88 RID: 7816 RVA: 0x000BEAC4 File Offset: 0x000BCCC4
	public override bool Continue()
	{
		if (this.targetEntity.IsDead() || this.theEntity.distraction != null)
		{
			if (this.theEntity.GetAttackTarget() == this.targetEntity)
			{
				this.theEntity.SetAttackTarget(null, 0);
			}
			return false;
		}
		this.findTime += 0.05f;
		if (this.findTime > 2f)
		{
			this.findTime = 0f;
			this.FindTarget();
			if (this.closeTargetEntity && this.closeTargetEntity != this.targetEntity)
			{
				return false;
			}
		}
		if (this.theEntity.GetAttackTarget() != this.targetEntity)
		{
			return false;
		}
		if (base.check(this.targetEntity) && (this.targetPlayer == null || this.theEntity.CanSeeStealth(this.manager.GetSeeDistance(this.targetEntity), this.targetPlayer.Stealth.lightLevel)))
		{
			this.theEntity.SetAttackTarget(this.targetEntity, 600);
			this.lastSeenPos = this.targetEntity.position;
			return true;
		}
		if (this.theEntity.GetDistanceSq(this.lastSeenPos) < 2.25f)
		{
			this.lastSeenPos = Vector3.zero;
		}
		this.theEntity.SetAttackTarget(null, 0);
		int ticks = this.theEntity.CalcInvestigateTicks(Constants.cEnemySenseMemory * 20, this.targetEntity);
		if (this.lastSeenPos != Vector3.zero)
		{
			this.theEntity.SetInvestigatePosition(this.lastSeenPos, ticks, true);
		}
		return false;
	}

	// Token: 0x06001E89 RID: 7817 RVA: 0x000BEC67 File Offset: 0x000BCE67
	public override void Reset()
	{
		this.targetEntity = null;
		this.targetPlayer = null;
	}

	// Token: 0x06001E8A RID: 7818 RVA: 0x000BEC77 File Offset: 0x000BCE77
	public override string ToString()
	{
		return string.Format("{0}, {1}", base.ToString(), this.targetEntity ? this.targetEntity.EntityName : "");
	}

	// Token: 0x04001507 RID: 5383
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cHearDistMax = 50f;

	// Token: 0x04001508 RID: 5384
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EAISetNearestEntityAsTarget.TargetClass> targetClasses;

	// Token: 0x04001509 RID: 5385
	[PublicizedFrom(EAccessModifier.Private)]
	public int playerTargetClassIndex = -1;

	// Token: 0x0400150A RID: 5386
	[PublicizedFrom(EAccessModifier.Private)]
	public float closeTargetDist;

	// Token: 0x0400150B RID: 5387
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive closeTargetEntity;

	// Token: 0x0400150C RID: 5388
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive targetEntity;

	// Token: 0x0400150D RID: 5389
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer targetPlayer;

	// Token: 0x0400150E RID: 5390
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 lastSeenPos;

	// Token: 0x0400150F RID: 5391
	[PublicizedFrom(EAccessModifier.Private)]
	public float findTime;

	// Token: 0x04001510 RID: 5392
	[PublicizedFrom(EAccessModifier.Private)]
	public float senseSoundTime;

	// Token: 0x04001511 RID: 5393
	[PublicizedFrom(EAccessModifier.Private)]
	public EAISetNearestEntityAsTargetSorter sorter;

	// Token: 0x04001512 RID: 5394
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Entity> list = new List<Entity>();

	// Token: 0x020003F1 RID: 1009
	[PublicizedFrom(EAccessModifier.Private)]
	public struct TargetClass
	{
		// Token: 0x04001513 RID: 5395
		public Type type;

		// Token: 0x04001514 RID: 5396
		public float hearDistMax;

		// Token: 0x04001515 RID: 5397
		public float seeDistMax;
	}
}
