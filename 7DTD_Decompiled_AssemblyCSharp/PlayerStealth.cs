using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000482 RID: 1154
public struct PlayerStealth
{
	// Token: 0x0600258F RID: 9615 RVA: 0x000F2DFE File Offset: 0x000F0FFE
	public void Init(EntityPlayer _player)
	{
		this.player = _player;
		this.noises = new List<PlayerStealth.NoiseData>();
		this.barColorUI = new Color32(0, 0, 0, byte.MaxValue);
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x000F2E28 File Offset: 0x000F1028
	public void Tick()
	{
		float num = this.player.speedForward * this.player.speedForward + this.player.speedStrafe * this.player.speedStrafe;
		if (num > 0.01f)
		{
			this.speedAverage = Utils.FastLerpUnclamped(this.speedAverage, (float)Math.Sqrt((double)num), 0.2f);
		}
		else
		{
			this.speedAverage *= 0.5f;
		}
		float num3;
		float num2 = LightManager.GetStealthLightLevel(this.player, out num3);
		float num4 = num3 / (num2 + 0.05f);
		num4 = Utils.FastClamp(num4, 0.5f, 3.2f);
		num2 += num3 * num4;
		if (this.player.IsCrouching)
		{
			num2 *= 0.6f;
		}
		this.player.Buffs.SetCustomVar("_lightlevel", num2 * 100f, true, CVarOperation.set);
		num2 *= 1f + this.speedAverage * 0.15f;
		float num5 = EffectManager.GetValue(PassiveEffects.LightMultiplier, null, 1f, this.player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.lightAttackPercent = ((num3 < 0.1f) ? num5 : 1f);
		num5 = 0.32f + 0.68f * num5;
		float v = num2 * num5 * 100f;
		this.lightLevel = Utils.FastClamp(v, 0f, 200f);
		this.ProcNoiseCleanup();
		float num6 = this.CalcVolume();
		this.player.Buffs.SetCustomVar("_noiselevel", this.noiseVolume, true, CVarOperation.set);
		int num7 = this.sleeperNoiseWaitTicks - 1;
		this.sleeperNoiseWaitTicks = num7;
		if (num7 <= 0)
		{
			this.sleeperNoiseVolume -= 2.5f;
			if (this.sleeperNoiseVolume < 0f)
			{
				this.sleeperNoiseVolume = 0f;
			}
		}
		if (num6 > 0f)
		{
			float num8 = num6 * 0.6f;
			float num9 = EAIManager.CalcSenseScale();
			num8 *= 1f + num9 * 1.6f;
			num8 = Utils.FastMin(num8, 40f + 15f * num9);
			this.player.world.GetEntitiesAround(EntityFlags.AIHearing, this.player.position, num8, PlayerStealth.entityTempList);
			for (int i = 0; i < PlayerStealth.entityTempList.Count; i++)
			{
				EntityAlive entityAlive = (EntityAlive)PlayerStealth.entityTempList[i];
				float distance = this.player.GetDistance(entityAlive);
				float num10 = this.noiseVolume * (1f + num9 * entityAlive.aiManager.feralSense);
				EntityHuman entityHuman = entityAlive as EntityHuman;
				if (entityHuman != null && entityHuman.IsStormEffected())
				{
					num10 *= 2f;
				}
				num10 /= distance * 0.6f + 0.4f;
				num10 *= this.player.DetectUsScale(entityAlive);
				if (num10 >= 1f)
				{
					bool flag = true;
					if (entityAlive.noisePlayer)
					{
						flag = (num10 > entityAlive.noisePlayerVolume);
					}
					if (flag)
					{
						entityAlive.noisePlayer = this.player;
						entityAlive.noisePlayerDistance = distance;
						entityAlive.noisePlayerVolume = num10;
					}
				}
			}
			PlayerStealth.entityTempList.Clear();
		}
		num7 = this.alertEnemiesTicks - 1;
		this.alertEnemiesTicks = num7;
		if (num7 <= 0)
		{
			this.alertEnemiesTicks = 20;
			this.alertEnemy = false;
			this.player.world.GetEntitiesAround(EntityFlags.AIHearing, this.player.position, 12f, PlayerStealth.entityTempList);
			for (int j = 0; j < PlayerStealth.entityTempList.Count; j++)
			{
				if (((EntityAlive)PlayerStealth.entityTempList[j]).IsAlert)
				{
					this.alertEnemy = true;
					break;
				}
			}
			PlayerStealth.entityTempList.Clear();
			this.SetBarColor(this.alertEnemy);
		}
		if (this.player.isEntityRemote)
		{
			if (this.sendTickDelay > 0)
			{
				this.sendTickDelay--;
			}
			if ((this.player.IsCrouching && this.sendTickDelay == 0 && (this.lightLevelSent != (int)this.lightLevel || this.noiseVolumeSent != (int)this.noiseVolume)) || this.alertEnemySent != this.alertEnemy)
			{
				this.sendTickDelay = 16;
				this.lightLevelSent = (int)this.lightLevel;
				this.noiseVolumeSent = (int)this.noiseVolume;
				this.alertEnemySent = this.alertEnemy;
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageEntityStealth>().Setup(this.player, this.lightLevelSent, this.noiseVolumeSent, this.alertEnemySent), false, this.player.entityId, -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x000F32D8 File Offset: 0x000F14D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetBarColor(bool _isAlert)
	{
		this.barColorUI.r = 50;
		this.barColorUI.g = 135;
		if (_isAlert)
		{
			this.barColorUI.r = 180;
			this.barColorUI.g = 180;
		}
	}

	// Token: 0x06002592 RID: 9618 RVA: 0x000F3328 File Offset: 0x000F1528
	public void ProcNoiseCleanup()
	{
		for (int i = 0; i < this.noises.Count; i++)
		{
			PlayerStealth.NoiseData noiseData = this.noises[i];
			if (noiseData.ticks > 1)
			{
				noiseData.ticks--;
				this.noises[i] = noiseData;
			}
			else
			{
				this.noises.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x06002593 RID: 9619 RVA: 0x000F338C File Offset: 0x000F158C
	[PublicizedFrom(EAccessModifier.Private)]
	public float CalcVolume()
	{
		float num = 0f;
		float num2 = 1f;
		for (int i = 0; i < this.noises.Count; i++)
		{
			num += this.noises[i].volume * num2;
			num2 *= 0.6f;
		}
		this.noiseVolume = Mathf.Pow(num * 2.35f, 0.86f);
		this.noiseVolume *= 1.5f;
		this.noiseVolume *= EffectManager.GetValue(PassiveEffects.NoiseMultiplier, null, 1f, this.player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		return num;
	}

	// Token: 0x06002594 RID: 9620 RVA: 0x000F3438 File Offset: 0x000F1638
	public bool CanSleeperAttackDetect(EntityAlive _e)
	{
		if (this.player.IsCrouching)
		{
			float num = Utils.FastLerp(3f, 15f, this.lightAttackPercent);
			if (_e.GetDistance(this.player) > num)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002595 RID: 9621 RVA: 0x000F347A File Offset: 0x000F167A
	public void SetClientLevels(float _lightLevel, float _noiseVolume, bool _isAlert)
	{
		this.lightLevel = _lightLevel;
		this.noiseVolume = _noiseVolume;
		this.alertEnemy = _isAlert;
		this.SetBarColor(_isAlert);
	}

	// Token: 0x06002596 RID: 9622 RVA: 0x000F3498 File Offset: 0x000F1698
	public bool NotifyNoise(float volume, float duration)
	{
		if (volume <= 0f)
		{
			return false;
		}
		this.AddNoise(this.noises, volume, (int)(duration * 20f));
		if (volume >= 11f)
		{
			this.sleeperNoiseWaitTicks = 20;
		}
		float num = volume;
		if (volume > 60f)
		{
			num = 60f + Mathf.Pow(volume - 60f, 1.4f);
		}
		num *= EffectManager.GetValue(PassiveEffects.NoiseMultiplier, null, 1f, this.player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		this.sleeperNoiseVolume += num;
		if (this.sleeperNoiseVolume >= 360f)
		{
			this.sleeperNoiseVolume = 360f;
			return true;
		}
		return false;
	}

	// Token: 0x06002597 RID: 9623 RVA: 0x000F3548 File Offset: 0x000F1748
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddNoise(List<PlayerStealth.NoiseData> list, float volume, int ticks)
	{
		PlayerStealth.NoiseData item = new PlayerStealth.NoiseData(volume, ticks);
		for (int i = 0; i < list.Count; i++)
		{
			PlayerStealth.NoiseData noiseData = this.noises[i];
			if (volume >= noiseData.volume)
			{
				list.Insert(i, item);
				return;
			}
		}
		list.Insert(list.Count, item);
	}

	// Token: 0x06002598 RID: 9624 RVA: 0x000F359C File Offset: 0x000F179C
	public static PlayerStealth Read(EntityPlayer _player, BinaryReader br)
	{
		int num = br.ReadInt32();
		PlayerStealth playerStealth = default(PlayerStealth);
		playerStealth.Init(_player);
		playerStealth.lightLevel = (float)br.ReadInt32();
		int num2 = br.ReadInt32();
		if (num2 > 0)
		{
			if (num >= 3)
			{
				for (int i = 0; i < num2; i++)
				{
					br.ReadSingle();
					float volume = br.ReadSingle();
					int ticks = br.ReadInt32();
					playerStealth.AddNoise(playerStealth.noises, volume, ticks);
				}
			}
			else if (num >= 2)
			{
				for (int j = 0; j < num2; j++)
				{
					br.ReadSingle();
					br.ReadSingle();
					br.ReadInt32();
				}
			}
			else
			{
				for (int k = 0; k < num2; k++)
				{
					br.ReadInt32();
					br.ReadInt32();
				}
			}
		}
		return playerStealth;
	}

	// Token: 0x06002599 RID: 9625 RVA: 0x000F3660 File Offset: 0x000F1860
	public void Write(BinaryWriter bw)
	{
		bw.Write(3);
		bw.Write(this.lightLevel);
		bw.Write((this.noises != null) ? this.noises.Count : 0);
		if (this.noises != null)
		{
			for (int i = 0; i < this.noises.Count; i++)
			{
				PlayerStealth.NoiseData noiseData = this.noises[i];
				bw.Write(0f);
				bw.Write(noiseData.volume);
				bw.Write(noiseData.ticks);
			}
		}
	}

	// Token: 0x170003FB RID: 1019
	// (get) Token: 0x0600259A RID: 9626 RVA: 0x000F36EA File Offset: 0x000F18EA
	public Color32 ValueColorUI
	{
		get
		{
			return this.barColorUI;
		}
	}

	// Token: 0x170003FC RID: 1020
	// (get) Token: 0x0600259B RID: 9627 RVA: 0x000F36F2 File Offset: 0x000F18F2
	public float ValuePercentUI
	{
		get
		{
			return Utils.FastClamp01((this.lightLevel + this.noiseVolume * 0.5f + (float)(this.alertEnemy ? 5 : 0)) * 0.01f + 0.005f);
		}
	}

	// Token: 0x04001C8B RID: 7307
	public const float cLightLevelMax = 200f;

	// Token: 0x04001C8C RID: 7308
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cLightMpyBase = 0.32f;

	// Token: 0x04001C8D RID: 7309
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cVersion = 3;

	// Token: 0x04001C8E RID: 7310
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cNextSoundPercent = 0.6f;

	// Token: 0x04001C8F RID: 7311
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSleeperNoiseDecay = 50f;

	// Token: 0x04001C90 RID: 7312
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cSleeperNoiseHear = 360f;

	// Token: 0x04001C91 RID: 7313
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSleeperNoiseWaitTicks = 20;

	// Token: 0x04001C92 RID: 7314
	public float lightLevel;

	// Token: 0x04001C93 RID: 7315
	[PublicizedFrom(EAccessModifier.Private)]
	public float lightAttackPercent;

	// Token: 0x04001C94 RID: 7316
	public float noiseVolume;

	// Token: 0x04001C95 RID: 7317
	public int smell;

	// Token: 0x04001C96 RID: 7318
	[PublicizedFrom(EAccessModifier.Private)]
	public float speedAverage;

	// Token: 0x04001C97 RID: 7319
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer player;

	// Token: 0x04001C98 RID: 7320
	[PublicizedFrom(EAccessModifier.Private)]
	public int sendTickDelay;

	// Token: 0x04001C99 RID: 7321
	[PublicizedFrom(EAccessModifier.Private)]
	public int lightLevelSent;

	// Token: 0x04001C9A RID: 7322
	[PublicizedFrom(EAccessModifier.Private)]
	public int noiseVolumeSent;

	// Token: 0x04001C9B RID: 7323
	[PublicizedFrom(EAccessModifier.Private)]
	public bool alertEnemySent;

	// Token: 0x04001C9C RID: 7324
	[PublicizedFrom(EAccessModifier.Private)]
	public int sleeperNoiseWaitTicks;

	// Token: 0x04001C9D RID: 7325
	[PublicizedFrom(EAccessModifier.Private)]
	public float sleeperNoiseVolume;

	// Token: 0x04001C9E RID: 7326
	[PublicizedFrom(EAccessModifier.Private)]
	public List<PlayerStealth.NoiseData> noises;

	// Token: 0x04001C9F RID: 7327
	[PublicizedFrom(EAccessModifier.Private)]
	public int alertEnemiesTicks;

	// Token: 0x04001CA0 RID: 7328
	[PublicizedFrom(EAccessModifier.Private)]
	public bool alertEnemy;

	// Token: 0x04001CA1 RID: 7329
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 barColorUI;

	// Token: 0x04001CA2 RID: 7330
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Entity> entityTempList = new List<Entity>();

	// Token: 0x02000483 RID: 1155
	[PublicizedFrom(EAccessModifier.Private)]
	public struct NoiseData
	{
		// Token: 0x0600259D RID: 9629 RVA: 0x000F3732 File Offset: 0x000F1932
		public NoiseData(float _volume, int _ticks)
		{
			this.volume = _volume;
			this.ticks = _ticks;
		}

		// Token: 0x04001CA3 RID: 7331
		public float volume;

		// Token: 0x04001CA4 RID: 7332
		public int ticks;
	}
}
