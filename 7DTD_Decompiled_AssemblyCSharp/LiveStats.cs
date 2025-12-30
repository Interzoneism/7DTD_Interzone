using System;
using System.IO;

// Token: 0x02000FFB RID: 4091
public class LiveStats
{
	// Token: 0x060081CC RID: 33228 RVA: 0x00349A5D File Offset: 0x00347C5D
	public LiveStats(int _maxLiveLevel, int _oversaturationLevel)
	{
		this.maxLiveLevel = _maxLiveLevel;
		this.oversaturationLevel = _oversaturationLevel;
		this.Reset();
	}

	// Token: 0x060081CD RID: 33229 RVA: 0x00349A7C File Offset: 0x00347C7C
	public void Copy(LiveStats other)
	{
		this.liveLevel = other.liveLevel;
		this.maxLiveLevel = other.maxLiveLevel;
		this.oversaturationLevel = other.oversaturationLevel;
		this.saturationLevel = other.saturationLevel;
		this.exhaustionLevel = other.exhaustionLevel;
		this.timer = other.timer;
	}

	// Token: 0x060081CE RID: 33230 RVA: 0x00349AD1 File Offset: 0x00347CD1
	public void Reset()
	{
		this.timer = 0;
		this.liveLevel = this.maxLiveLevel;
		this.saturationLevel = 0f;
		this.exhaustionLevel = 0f;
	}

	// Token: 0x060081CF RID: 33231 RVA: 0x00349AFC File Offset: 0x00347CFC
	public void AddStats(int _addLifeValue)
	{
		this.liveLevel += _addLifeValue;
		if (this.liveLevel > this.maxLiveLevel)
		{
			this.saturationLevel += (float)(this.liveLevel - this.maxLiveLevel);
			this.liveLevel = this.maxLiveLevel;
			this.saturationLevel = Utils.FastMin(this.saturationLevel, (float)this.oversaturationLevel);
		}
		if (this.liveLevel < 0)
		{
			this.liveLevel = 0;
		}
	}

	// Token: 0x060081D0 RID: 33232 RVA: 0x00349B74 File Offset: 0x00347D74
	public void OnUpdate(EntityPlayer _entityPlayer)
	{
		while (this.exhaustionLevel > 1f)
		{
			this.exhaustionLevel -= 1f;
			if (this.saturationLevel > 0f)
			{
				this.saturationLevel = Math.Max(this.saturationLevel - 1f, 0f);
			}
			else
			{
				this.liveLevel = Math.Max(this.liveLevel - 1, 0);
			}
		}
	}

	// Token: 0x060081D1 RID: 33233 RVA: 0x00349BE1 File Offset: 0x00347DE1
	public void Read(BinaryReader _br)
	{
		this.liveLevel = (int)_br.ReadInt16();
		this.timer = (int)_br.ReadInt16();
		this.saturationLevel = _br.ReadSingle();
		this.exhaustionLevel = _br.ReadSingle();
	}

	// Token: 0x060081D2 RID: 33234 RVA: 0x00349C13 File Offset: 0x00347E13
	public void Write(BinaryWriter _bw)
	{
		_bw.Write((ushort)this.liveLevel);
		_bw.Write((ushort)this.timer);
		_bw.Write(this.saturationLevel);
		_bw.Write(this.exhaustionLevel);
	}

	// Token: 0x060081D3 RID: 33235 RVA: 0x00349C47 File Offset: 0x00347E47
	public int GetLifeLevel()
	{
		return this.liveLevel;
	}

	// Token: 0x060081D4 RID: 33236 RVA: 0x00349C4F File Offset: 0x00347E4F
	public int GetMaxLifeLevel()
	{
		return this.maxLiveLevel;
	}

	// Token: 0x060081D5 RID: 33237 RVA: 0x00349C57 File Offset: 0x00347E57
	public void SetLifeLevel(int _value)
	{
		this.liveLevel = _value;
	}

	// Token: 0x060081D6 RID: 33238 RVA: 0x00349C60 File Offset: 0x00347E60
	public bool IsFilledUp()
	{
		return this.liveLevel >= this.maxLiveLevel;
	}

	// Token: 0x060081D7 RID: 33239 RVA: 0x00349C73 File Offset: 0x00347E73
	public void AddExhaustion(float _v)
	{
		this.exhaustionLevel = Math.Min(this.exhaustionLevel + _v, 40f);
	}

	// Token: 0x060081D8 RID: 33240 RVA: 0x00349C8D File Offset: 0x00347E8D
	public float GetSaturationLevel()
	{
		return this.saturationLevel;
	}

	// Token: 0x060081D9 RID: 33241 RVA: 0x00349C95 File Offset: 0x00347E95
	public void SetSaturationLevel(float _level)
	{
		this.saturationLevel = _level;
	}

	// Token: 0x060081DA RID: 33242 RVA: 0x00349C9E File Offset: 0x00347E9E
	public float GetExhaustionLevel()
	{
		return this.exhaustionLevel;
	}

	// Token: 0x060081DB RID: 33243 RVA: 0x00349CA6 File Offset: 0x00347EA6
	public void SetExhaustionLevel(float _level)
	{
		this.exhaustionLevel = _level;
	}

	// Token: 0x060081DC RID: 33244 RVA: 0x00349CAF File Offset: 0x00347EAF
	public float GetLifeLevelFraction()
	{
		return (float)this.liveLevel / (float)this.maxLiveLevel;
	}

	// Token: 0x060081DD RID: 33245 RVA: 0x00349CC0 File Offset: 0x00347EC0
	public LiveStats Clone()
	{
		LiveStats liveStats = new LiveStats(this.maxLiveLevel, this.oversaturationLevel);
		liveStats.SetSaturationLevel(this.saturationLevel);
		liveStats.SetExhaustionLevel(this.exhaustionLevel);
		liveStats.SetLifeLevel(this.liveLevel);
		return liveStats;
	}

	// Token: 0x04006470 RID: 25712
	[PublicizedFrom(EAccessModifier.Private)]
	public int liveLevel;

	// Token: 0x04006471 RID: 25713
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxLiveLevel;

	// Token: 0x04006472 RID: 25714
	[PublicizedFrom(EAccessModifier.Private)]
	public int oversaturationLevel;

	// Token: 0x04006473 RID: 25715
	[PublicizedFrom(EAccessModifier.Private)]
	public float saturationLevel;

	// Token: 0x04006474 RID: 25716
	[PublicizedFrom(EAccessModifier.Private)]
	public float exhaustionLevel;

	// Token: 0x04006475 RID: 25717
	[PublicizedFrom(EAccessModifier.Private)]
	public int timer;
}
