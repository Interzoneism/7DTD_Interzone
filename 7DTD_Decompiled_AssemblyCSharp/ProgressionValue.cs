using System;
using System.IO;
using UnityEngine;

// Token: 0x02000971 RID: 2417
public class ProgressionValue
{
	// Token: 0x06004917 RID: 18711 RVA: 0x001CE33C File Offset: 0x001CC53C
	public float GetCalculatedLevel(EntityAlive _ea)
	{
		if (this.calculatedFrame == Time.frameCount)
		{
			return this.calculatedLevel;
		}
		ProgressionClass progressionClass = this.ProgressionClass;
		if (progressionClass == null)
		{
			return 0f;
		}
		float num = (float)this.Level;
		PassiveEffects passiveEffects;
		switch (progressionClass.Type)
		{
		case ProgressionType.Attribute:
			passiveEffects = PassiveEffects.AttributeLevel;
			break;
		case ProgressionType.Skill:
			passiveEffects = PassiveEffects.SkillLevel;
			break;
		case ProgressionType.Perk:
			passiveEffects = PassiveEffects.PerkLevel;
			break;
		default:
			passiveEffects = PassiveEffects.None;
			break;
		}
		PassiveEffects passiveEffects2 = passiveEffects;
		if (passiveEffects2 != PassiveEffects.None)
		{
			num = EffectManager.GetValue(passiveEffects2, null, num, _ea, null, progressionClass.NameTag, true, true, true, true, true, 1, true, false);
		}
		num = Mathf.Min(num, (float)ProgressionClass.GetCalculatedMaxLevel(_ea, this));
		num = Mathf.Max(num, (float)progressionClass.MinLevel);
		this.calculatedFrame = Time.frameCount;
		this.calculatedLevel = num;
		return num;
	}

	// Token: 0x06004918 RID: 18712 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public ProgressionValue()
	{
	}

	// Token: 0x06004919 RID: 18713 RVA: 0x001CE3F3 File Offset: 0x001CC5F3
	public ProgressionValue(string _name)
	{
		this.name = _name;
	}

	// Token: 0x170007B3 RID: 1971
	// (get) Token: 0x0600491A RID: 18714 RVA: 0x001CE402 File Offset: 0x001CC602
	public string Name
	{
		get
		{
			return this.name;
		}
	}

	// Token: 0x170007B4 RID: 1972
	// (get) Token: 0x0600491B RID: 18715 RVA: 0x001CE40C File Offset: 0x001CC60C
	public ProgressionClass ProgressionClass
	{
		get
		{
			if (this.cachedProgressionClass == null && !Progression.ProgressionClasses.TryGetValue(this.name, out this.cachedProgressionClass))
			{
				Log.Error("ProgressionValue ProgressionClasses missing {0}", new object[]
				{
					this.name
				});
			}
			return this.cachedProgressionClass;
		}
	}

	// Token: 0x170007B5 RID: 1973
	// (get) Token: 0x0600491C RID: 18716 RVA: 0x001CE458 File Offset: 0x001CC658
	// (set) Token: 0x0600491D RID: 18717 RVA: 0x001CE482 File Offset: 0x001CC682
	public int CostForNextLevel
	{
		get
		{
			if (this.ProgressionClass.CurrencyType == ProgressionCurrencyType.SP)
			{
				return this.ProgressionClass.CalculatedCostForLevel(this.Level + 1);
			}
			return this.costForNextLevel;
		}
		set
		{
			if (this.ProgressionClass.CurrencyType != ProgressionCurrencyType.SP)
			{
				this.costForNextLevel = value;
			}
		}
	}

	// Token: 0x170007B6 RID: 1974
	// (get) Token: 0x0600491E RID: 18718 RVA: 0x001CE49C File Offset: 0x001CC69C
	// (set) Token: 0x0600491F RID: 18719 RVA: 0x001CE4CF File Offset: 0x001CC6CF
	public int Level
	{
		get
		{
			ProgressionClass progressionClass = this.ProgressionClass;
			if (progressionClass == null)
			{
				return this.level;
			}
			if (progressionClass.IsSkill)
			{
				return progressionClass.MaxLevel;
			}
			return this.level;
		}
		set
		{
			this.calculatedFrame = -1;
			if (this.ProgressionClass == null)
			{
				this.level = value;
				return;
			}
			if (this.ProgressionClass.IsSkill)
			{
				this.level = this.ProgressionClass.MaxLevel;
				return;
			}
			this.level = value;
		}
	}

	// Token: 0x06004920 RID: 18720 RVA: 0x001CE50E File Offset: 0x001CC70E
	public int CalculatedLevel(EntityAlive _ea)
	{
		return (int)this.GetCalculatedLevel(_ea);
	}

	// Token: 0x06004921 RID: 18721 RVA: 0x001CE518 File Offset: 0x001CC718
	public int CalculatedMaxLevel(EntityAlive _ea)
	{
		return ProgressionClass.GetCalculatedMaxLevel(_ea, this);
	}

	// Token: 0x06004922 RID: 18722 RVA: 0x001CE521 File Offset: 0x001CC721
	public bool IsLocked(EntityAlive _ea)
	{
		return ProgressionClass.GetCalculatedMaxLevel(_ea, this) == 0;
	}

	// Token: 0x170007B7 RID: 1975
	// (get) Token: 0x06004923 RID: 18723 RVA: 0x001CE52D File Offset: 0x001CC72D
	public float PercToNextLevel
	{
		get
		{
			return 1f - (float)this.CostForNextLevel / (float)this.ProgressionClass.CalculatedCostForLevel(this.level + 1);
		}
	}

	// Token: 0x06004924 RID: 18724 RVA: 0x001CE551 File Offset: 0x001CC751
	public void ClearProgressionClassLink()
	{
		this.cachedProgressionClass = null;
	}

	// Token: 0x06004925 RID: 18725 RVA: 0x001CE55A File Offset: 0x001CC75A
	public bool CanPurchase(EntityAlive _ea, int _level)
	{
		return _level <= this.ProgressionClass.MaxLevel;
	}

	// Token: 0x06004926 RID: 18726 RVA: 0x001CE56D File Offset: 0x001CC76D
	public void CopyFrom(ProgressionValue _pv)
	{
		this.name = _pv.name;
		this.level = _pv.level;
		this.costForNextLevel = _pv.costForNextLevel;
	}

	// Token: 0x06004927 RID: 18727 RVA: 0x001CE593 File Offset: 0x001CC793
	public void Read(BinaryReader _reader)
	{
		_reader.ReadByte();
		this.name = _reader.ReadString();
		this.level = (int)_reader.ReadByte();
		this.costForNextLevel = _reader.ReadInt32();
	}

	// Token: 0x06004928 RID: 18728 RVA: 0x001CE5C0 File Offset: 0x001CC7C0
	public void Write(BinaryWriter _writer, bool _IsNetwork)
	{
		_writer.Write(1);
		_writer.Write(this.name);
		_writer.Write((byte)this.level);
		_writer.Write(this.costForNextLevel);
	}

	// Token: 0x04003850 RID: 14416
	[PublicizedFrom(EAccessModifier.Private)]
	public const byte Version = 1;

	// Token: 0x04003851 RID: 14417
	[PublicizedFrom(EAccessModifier.Private)]
	public string name;

	// Token: 0x04003852 RID: 14418
	[PublicizedFrom(EAccessModifier.Private)]
	public int level;

	// Token: 0x04003853 RID: 14419
	[PublicizedFrom(EAccessModifier.Private)]
	public int costForNextLevel;

	// Token: 0x04003854 RID: 14420
	[PublicizedFrom(EAccessModifier.Private)]
	public int calculatedFrame;

	// Token: 0x04003855 RID: 14421
	[PublicizedFrom(EAccessModifier.Private)]
	public float calculatedLevel;

	// Token: 0x04003856 RID: 14422
	[PublicizedFrom(EAccessModifier.Private)]
	public ProgressionClass cachedProgressionClass;
}
