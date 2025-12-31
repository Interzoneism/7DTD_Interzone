using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000966 RID: 2406
public class Progression
{
	// Token: 0x060048BF RID: 18623 RVA: 0x001CC864 File Offset: 0x001CAA64
	public Progression()
	{
		this.ExpToNextLevel = this.getExpForLevel((float)(this.Level + 1));
	}

	// Token: 0x060048C0 RID: 18624 RVA: 0x001CC8D0 File Offset: 0x001CAAD0
	public Progression(EntityAlive _parent)
	{
		this.parent = _parent;
		this.ExpToNextLevel = this.getExpForLevel((float)(this.Level + 1));
		this.SetupData();
	}

	// Token: 0x060048C1 RID: 18625 RVA: 0x001CC948 File Offset: 0x001CAB48
	public Dictionary<int, ProgressionValue> GetDict()
	{
		return this.ProgressionValues.Dict;
	}

	// Token: 0x060048C2 RID: 18626 RVA: 0x001CC955 File Offset: 0x001CAB55
	public static int CalcId(string _name)
	{
		return Progression.ProgressionNameIds.Add(_name);
	}

	// Token: 0x060048C3 RID: 18627 RVA: 0x001CC962 File Offset: 0x001CAB62
	public ProgressionValue GetProgressionValue(int _id)
	{
		return this.ProgressionValues.Get(_id);
	}

	// Token: 0x060048C4 RID: 18628 RVA: 0x001CC970 File Offset: 0x001CAB70
	[PublicizedFrom(EAccessModifier.Protected)]
	public float getLevelFloat()
	{
		return (float)this.Level + (1f - (float)this.ExpToNextLevel / (float)this.GetExpForNextLevel());
	}

	// Token: 0x060048C5 RID: 18629 RVA: 0x001CC98F File Offset: 0x001CAB8F
	[PublicizedFrom(EAccessModifier.Protected)]
	public int getExpForLevel(float _level)
	{
		return (int)Math.Min((float)Progression.BaseExpToLevel * Mathf.Pow(Progression.ExpMultiplier, _level), 2.1474836E+09f);
	}

	// Token: 0x060048C6 RID: 18630 RVA: 0x001CC9AE File Offset: 0x001CABAE
	public int GetLevel()
	{
		return this.Level;
	}

	// Token: 0x060048C7 RID: 18631 RVA: 0x001CC9B6 File Offset: 0x001CABB6
	public int GetExpForNextLevel()
	{
		return this.getExpForLevel((float)Mathf.Clamp(this.Level + 1, 0, Progression.ClampExpCostAtLevel));
	}

	// Token: 0x060048C8 RID: 18632 RVA: 0x001CC9D2 File Offset: 0x001CABD2
	public float GetLevelProgressPercentage()
	{
		return this.getLevelFloat() - (float)this.Level;
	}

	// Token: 0x060048C9 RID: 18633 RVA: 0x001CC9E4 File Offset: 0x001CABE4
	public void ModifyValue(PassiveEffects _effect, ref float _baseVal, ref float _percVal, FastTags<TagGroup.Global> _tags)
	{
		if (_effect == PassiveEffects.AttributeLevel)
		{
			return;
		}
		if (_effect == PassiveEffects.SkillLevel)
		{
			return;
		}
		if (_effect == PassiveEffects.PerkLevel)
		{
			return;
		}
		List<ProgressionValue> list;
		if (this.passiveEffects.TryGetValue(_effect, out list))
		{
			for (int i = 0; i < list.Count; i++)
			{
				ProgressionValue progressionValue = list[i];
				ProgressionClass progressionClass = progressionValue.ProgressionClass;
				if (progressionClass != null)
				{
					progressionClass.ModifyValue(this.parent, progressionValue, _effect, ref _baseVal, ref _percVal, _tags);
				}
			}
		}
	}

	// Token: 0x060048CA RID: 18634 RVA: 0x001CCA48 File Offset: 0x001CAC48
	public void GetModifiedValueData(List<EffectManager.ModifierValuesAndSources> _modValueSources, EffectManager.ModifierValuesAndSources.ValueSourceType _sourceType, PassiveEffects _effect, ref float _base_val, ref float _perc_val, FastTags<TagGroup.Global> _tags)
	{
		if (_effect == PassiveEffects.AttributeLevel)
		{
			return;
		}
		if (_effect == PassiveEffects.SkillLevel)
		{
			return;
		}
		if (_effect == PassiveEffects.PerkLevel)
		{
			return;
		}
		for (int i = 0; i < this.ProgressionValueQuickList.Count; i++)
		{
			ProgressionValue progressionValue = this.ProgressionValueQuickList[i];
			if (progressionValue != null)
			{
				ProgressionClass progressionClass = progressionValue.ProgressionClass;
				if (progressionClass != null && progressionClass.Effects != null && progressionClass.Effects.PassivesIndex != null && progressionClass.Effects.PassivesIndex.Contains(_effect))
				{
					progressionClass.GetModifiedValueData(_modValueSources, _sourceType, this.parent, progressionValue, _effect, ref _base_val, ref _perc_val, _tags);
				}
			}
		}
	}

	// Token: 0x060048CB RID: 18635 RVA: 0x001CCAD8 File Offset: 0x001CACD8
	public void Update()
	{
		if (this.timer <= 0f)
		{
			this.FireEvent(MinEventTypes.onSelfProgressionUpdate, this.parent.MinEventContext);
			this.timer = 1f;
		}
		else
		{
			this.timer -= Time.deltaTime;
		}
		this.parent.Buffs.SetCustomVar("_expdeficit", (float)this.ExpDeficit, true, CVarOperation.set);
	}

	// Token: 0x060048CC RID: 18636 RVA: 0x001CCB44 File Offset: 0x001CAD44
	public void FireEvent(MinEventTypes _eventType, MinEventParams _params)
	{
		if (this.eventList == null)
		{
			return;
		}
		for (int i = 0; i < this.eventList.Count; i++)
		{
			ProgressionValue progressionValue = this.eventList[i];
			ProgressionClass progressionClass = progressionValue.ProgressionClass;
			_params.ProgressionValue = progressionValue;
			progressionClass.FireEvent(_eventType, _params);
		}
	}

	// Token: 0x060048CD RID: 18637 RVA: 0x001CCB94 File Offset: 0x001CAD94
	public int AddLevelExp(int _exp, string _cvarXPName = "_xpOther", Progression.XPTypes _xpType = Progression.XPTypes.Other, bool useBonus = true, bool notifyUI = true)
	{
		if (this.parent as EntityPlayer == null)
		{
			return _exp;
		}
		float num = (float)_exp;
		if (useBonus)
		{
			if (this.xpFastTags == null)
			{
				this.xpFastTags = new FastTags<TagGroup.Global>[11];
				for (int i = 0; i < 11; i++)
				{
					this.xpFastTags[i] = FastTags<TagGroup.Global>.Parse(((Progression.XPTypes)i).ToStringCached<Progression.XPTypes>());
				}
			}
			num = num * (float)GameStats.GetInt(EnumGameStats.XPMultiplier) / 100f;
			num = EffectManager.GetValue(PassiveEffects.PlayerExpGain, null, num, this.parent, null, this.xpFastTags[(int)_xpType], true, true, true, true, true, 1, true, false);
		}
		if (num > 214748370f)
		{
			num = 214748370f;
		}
		if (_xpType != Progression.XPTypes.Debug)
		{
			GameSparksCollector.IncrementCounter(GameSparksCollector.GSDataKey.XpEarnedBy, _xpType.ToStringCached<Progression.XPTypes>(), num, true, GameSparksCollector.GSDataCollection.SessionUpdates);
		}
		int level = this.Level;
		EntityPlayerLocal entityPlayerLocal = this.parent as EntityPlayerLocal;
		if (entityPlayerLocal)
		{
			entityPlayerLocal.PlayerUI.xui.CollectedItemList.AddIconNotification("ui_game_symbol_xp", (int)num, false);
		}
		this.AddLevelExpRecursive((int)num, _cvarXPName, notifyUI);
		if (this.Level != level)
		{
			Log.Out("{0} made level {1} (was {2}), exp for next level {3}", new object[]
			{
				this.parent.EntityName,
				this.Level,
				level,
				this.ExpToNextLevel
			});
		}
		return (int)num;
	}

	// Token: 0x060048CE RID: 18638 RVA: 0x00002914 File Offset: 0x00000B14
	public void OnDeath()
	{
	}

	// Token: 0x060048CF RID: 18639 RVA: 0x001CCCE0 File Offset: 0x001CAEE0
	public void AddXPDeficit()
	{
		this.ExpDeficit += (int)((float)this.GetExpForNextLevel() * EffectManager.GetValue(PassiveEffects.ExpDeficitPerDeathPercentage, null, 0.1f, this.parent, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false));
		this.ExpDeficit = Mathf.Clamp(this.ExpDeficit, 0, (int)((float)this.GetExpForNextLevel() * EffectManager.GetValue(PassiveEffects.ExpDeficitMaxPercentage, null, 0.5f, this.parent, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)));
		this.ExpDeficitGained = true;
	}

	// Token: 0x060048D0 RID: 18640 RVA: 0x001CCD70 File Offset: 0x001CAF70
	public void OnRespawnFromDeath()
	{
		if (!this.ExpDeficitGained)
		{
			return;
		}
		EntityPlayerLocal player = this.parent as EntityPlayerLocal;
		if (this.ExpDeficit == (int)((float)this.GetExpForNextLevel() * EffectManager.GetValue(PassiveEffects.ExpDeficitMaxPercentage, null, 0.5f, this.parent, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)))
		{
			GameManager.ShowTooltip(player, Localization.Get("ttResurrectMaxXPLost", false), false, false, 0f);
		}
		else
		{
			GameManager.ShowTooltip(player, string.Format(Localization.Get("ttResurrectXPLost", false), EffectManager.GetValue(PassiveEffects.ExpDeficitPerDeathPercentage, null, 0.1f, this.parent, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) * 100f), false, false, 0f);
		}
		this.ExpDeficitGained = false;
	}

	// Token: 0x060048D1 RID: 18641 RVA: 0x001CCE38 File Offset: 0x001CB038
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddLevelExpRecursive(int exp, string _cvarXPName, bool notifyUI = true)
	{
		if (this.Level >= Progression.MaxLevel)
		{
			this.Level = Progression.MaxLevel;
			return;
		}
		this.parent.Buffs.IncrementCustomVar(_cvarXPName, (float)exp);
		int num;
		if (this.ExpDeficit > 0)
		{
			num = exp - this.ExpDeficit;
			this.ExpDeficit -= exp;
			this.ExpDeficit = Mathf.Clamp(this.ExpDeficit, 0, (int)((float)this.GetExpForNextLevel() * EffectManager.GetValue(PassiveEffects.ExpDeficitMaxPercentage, null, 0.5f, this.parent, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false)));
		}
		else
		{
			num = exp - this.ExpToNextLevel;
			this.ExpToNextLevel -= exp;
		}
		EntityPlayerLocal entityPlayerLocal = this.parent as EntityPlayerLocal;
		if (this.ExpDeficit <= 0)
		{
			int level = this.Level;
			if (this.ExpToNextLevel <= 0)
			{
				this.Level++;
				if (Progression.SkillPointMultiplier == 0f)
				{
					this.SkillPoints += Progression.SkillPointsPerLevel;
				}
				else
				{
					this.SkillPoints += (int)Math.Min((float)Progression.SkillPointsPerLevel * Mathf.Pow(Progression.SkillPointMultiplier, (float)this.Level), 2.1474836E+09f);
				}
				if (entityPlayerLocal)
				{
					GameSparksCollector.PlayerLevelUp(entityPlayerLocal, this.Level);
				}
				this.ExpToNextLevel = this.GetExpForNextLevel();
			}
			if ((this.ExpToNextLevel > num || this.Level == Progression.MaxLevel) && level != this.Level && entityPlayerLocal && notifyUI)
			{
				GameManager.ShowTooltip(entityPlayerLocal, string.Format(Localization.Get("ttLevelUp", false), this.Level.ToString(), this.SkillPoints), string.Empty, "levelupplayer", null, false, false, 0f);
			}
		}
		if (num > 0)
		{
			this.AddLevelExpRecursive(num, _cvarXPName, true);
		}
	}

	// Token: 0x060048D2 RID: 18642 RVA: 0x001CD008 File Offset: 0x001CB208
	public void SpendSkillPoints(int _points, string _progressionName)
	{
		ProgressionValue progressionValue = this.GetProgressionValue(_progressionName);
		if (progressionValue != null && progressionValue.ProgressionClass.CurrencyType == ProgressionCurrencyType.SP)
		{
			this.addProgressionCurrency(_points, progressionValue);
		}
	}

	// Token: 0x060048D3 RID: 18643 RVA: 0x001CD036 File Offset: 0x001CB236
	public ProgressionValue GetProgressionValue(string _progressionName)
	{
		return this.ProgressionValues.Get(_progressionName);
	}

	// Token: 0x060048D4 RID: 18644 RVA: 0x001CD044 File Offset: 0x001CB244
	public void GetPerkList(List<ProgressionValue> perkList, string _skillName)
	{
		perkList.Clear();
		for (int i = 0; i < this.ProgressionValueQuickList.Count; i++)
		{
			ProgressionValue progressionValue = this.ProgressionValueQuickList[i];
			if ((progressionValue.ProgressionClass.Type == ProgressionType.Perk || progressionValue.ProgressionClass.Type == ProgressionType.Book) && progressionValue.ProgressionClass.Parent.Name == _skillName)
			{
				perkList.Add(progressionValue);
			}
		}
	}

	// Token: 0x060048D5 RID: 18645 RVA: 0x001CD0B8 File Offset: 0x001CB2B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void addProgressionCurrency(int _currencyAmount, ProgressionValue _pv)
	{
		if (_pv == null)
		{
			return;
		}
		ProgressionClass progressionClass = _pv.ProgressionClass;
		if (_pv.Level >= progressionClass.MaxLevel)
		{
			if (_pv.Level > progressionClass.MaxLevel)
			{
				_pv.Level = progressionClass.MaxLevel;
			}
			return;
		}
		if (progressionClass.Type == ProgressionType.Skill)
		{
			_currencyAmount = (int)EffectManager.GetValue(PassiveEffects.SkillExpGain, null, (float)_currencyAmount, this.parent, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
		}
		int num = _currencyAmount - _pv.CostForNextLevel;
		_pv.CostForNextLevel -= _currencyAmount;
		if (_pv.CostForNextLevel <= 0)
		{
			int level = _pv.Level;
			_pv.Level = level + 1;
			_pv.CostForNextLevel = progressionClass.CalculatedCostForLevel(_pv.Level + 1);
		}
		if (num > 0)
		{
			this.addProgressionCurrency(num, _pv);
		}
	}

	// Token: 0x060048D6 RID: 18646 RVA: 0x001CD178 File Offset: 0x001CB378
	public byte[] ToBytes(bool _IsNetwork = false)
	{
		byte[] result = null;
		using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(true))
		{
			MemoryStream memoryStream = new MemoryStream();
			pooledBinaryWriter.SetBaseStream(memoryStream);
			this.Write(pooledBinaryWriter, _IsNetwork);
			result = memoryStream.ToArray();
			memoryStream.Dispose();
		}
		return result;
	}

	// Token: 0x060048D7 RID: 18647 RVA: 0x001CD1D4 File Offset: 0x001CB3D4
	public void Write(BinaryWriter _bw, bool _IsNetwork = false)
	{
		_bw.Write(3);
		_bw.Write((ushort)this.Level);
		_bw.Write(this.ExpToNextLevel);
		_bw.Write((ushort)this.SkillPoints);
		int count = this.ProgressionValues.Count;
		_bw.Write(count);
		foreach (KeyValuePair<int, ProgressionValue> keyValuePair in this.ProgressionValues.Dict)
		{
			keyValuePair.Value.Write(_bw, _IsNetwork);
		}
		_bw.Write(this.ExpDeficit);
	}

	// Token: 0x060048D8 RID: 18648 RVA: 0x001CD280 File Offset: 0x001CB480
	public static Progression FromBytes(byte[] data, EntityAlive _parent)
	{
		Progression result = null;
		try
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
			{
				MemoryStream memoryStream = new MemoryStream(data);
				pooledBinaryReader.SetBaseStream(memoryStream);
				result = Progression.Read(pooledBinaryReader, _parent);
				memoryStream.Dispose();
			}
		}
		catch
		{
			result = null;
		}
		return result;
	}

	// Token: 0x060048D9 RID: 18649 RVA: 0x001CD2E8 File Offset: 0x001CB4E8
	public static Progression Read(BinaryReader _br, EntityAlive _parent)
	{
		Progression progression = _parent.Progression;
		if (progression == null)
		{
			Log.Warning("Progression Read {0}, new", new object[]
			{
				_parent
			});
			progression = new Progression(_parent);
			_parent.Progression = progression;
		}
		byte b = _br.ReadByte();
		progression.Level = (int)_br.ReadUInt16();
		progression.ExpToNextLevel = _br.ReadInt32();
		progression.SkillPoints = (int)_br.ReadUInt16();
		int num = _br.ReadInt32();
		ProgressionValue progressionValue = new ProgressionValue();
		for (int i = 0; i < num; i++)
		{
			progressionValue.Read(_br);
			if (Progression.ProgressionClasses.ContainsKey(progressionValue.Name))
			{
				ProgressionValue progressionValue2 = progression.ProgressionValues.Get(progressionValue.Name);
				if (progressionValue2 != null)
				{
					progressionValue2.CopyFrom(progressionValue);
				}
				else
				{
					Log.Error("ProgressionValues missing {0}", new object[]
					{
						progressionValue.Name
					});
					progressionValue2 = new ProgressionValue();
					progressionValue2.CopyFrom(progressionValue);
					progression.ProgressionValues.Add(progressionValue.Name, progressionValue2);
				}
			}
		}
		if (b > 2)
		{
			progression.ExpDeficit = _br.ReadInt32();
		}
		progression.SetupData();
		return progression;
	}

	// Token: 0x060048DA RID: 18650 RVA: 0x001CD3F8 File Offset: 0x001CB5F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupData()
	{
		foreach (KeyValuePair<string, ProgressionClass> keyValuePair in Progression.ProgressionClasses)
		{
			string name = keyValuePair.Value.Name;
			if (!this.ProgressionValues.Contains(name))
			{
				ProgressionValue value = new ProgressionValue(name)
				{
					Level = keyValuePair.Value.MinLevel,
					CostForNextLevel = keyValuePair.Value.CalculatedCostForLevel(this.Level + 1)
				};
				this.ProgressionValues.Add(name, value);
			}
		}
		this.ProgressionValueQuickList.Clear();
		foreach (KeyValuePair<int, ProgressionValue> keyValuePair2 in this.ProgressionValues.Dict)
		{
			this.ProgressionValueQuickList.Add(keyValuePair2.Value);
		}
		this.eventList.Clear();
		this.passiveEffects.Clear();
		for (int i = 0; i < this.ProgressionValueQuickList.Count; i++)
		{
			ProgressionValue progressionValue = this.ProgressionValueQuickList[i];
			ProgressionClass progressionClass = progressionValue.ProgressionClass;
			if (progressionClass.HasEvents())
			{
				this.eventList.Add(progressionValue);
			}
			MinEffectController effects = progressionClass.Effects;
			if (effects != null)
			{
				HashSet<PassiveEffects> passivesIndex = effects.PassivesIndex;
				if (passivesIndex != null)
				{
					foreach (PassiveEffects key in passivesIndex)
					{
						List<ProgressionValue> list;
						if (!this.passiveEffects.TryGetValue(key, out list))
						{
							list = new List<ProgressionValue>();
							this.passiveEffects.Add(key, list);
						}
						list.Add(progressionValue);
					}
				}
			}
		}
	}

	// Token: 0x060048DB RID: 18651 RVA: 0x001CD5E0 File Offset: 0x001CB7E0
	public void ClearProgressionClassLinks()
	{
		if (this.ProgressionValueQuickList == null)
		{
			return;
		}
		foreach (ProgressionValue progressionValue in this.ProgressionValueQuickList)
		{
			if (progressionValue != null)
			{
				progressionValue.ClearProgressionClassLink();
			}
		}
		this.SetupData();
	}

	// Token: 0x060048DC RID: 18652 RVA: 0x001CD648 File Offset: 0x001CB848
	public static void Cleanup()
	{
		if (Progression.ProgressionClasses != null)
		{
			Progression.ProgressionClasses.Clear();
		}
	}

	// Token: 0x060048DD RID: 18653 RVA: 0x001CD65C File Offset: 0x001CB85C
	public void ResetProgression(bool _resetSkills = true, bool _resetBooks = false, bool _resetCrafting = false)
	{
		int num = 0;
		int i = 0;
		while (i < this.ProgressionValueQuickList.Count)
		{
			ProgressionValue progressionValue = this.ProgressionValueQuickList[i];
			ProgressionClass progressionClass = progressionValue.ProgressionClass;
			if (!progressionClass.IsBook)
			{
				goto IL_34;
			}
			if (_resetBooks)
			{
				progressionValue.Level = 0;
				goto IL_34;
			}
			IL_C3:
			i++;
			continue;
			IL_34:
			if (progressionClass.IsCrafting)
			{
				if (!_resetCrafting)
				{
					goto IL_C3;
				}
				progressionValue.Level = 1;
			}
			if (!_resetSkills)
			{
				goto IL_C3;
			}
			if (progressionClass.IsAttribute)
			{
				if (progressionValue.Level > 1)
				{
					for (int j = 2; j <= progressionValue.Level; j++)
					{
						num += progressionClass.CalculatedCostForLevel(j);
					}
					progressionValue.Level = 1;
					goto IL_C3;
				}
				goto IL_C3;
			}
			else
			{
				if (progressionClass.IsPerk && progressionValue.Level > 0)
				{
					for (int k = 1; k <= progressionValue.Level; k++)
					{
						num += progressionClass.CalculatedCostForLevel(k);
					}
					progressionValue.Level = 0;
					goto IL_C3;
				}
				goto IL_C3;
			}
		}
		EntityPlayerLocal entityPlayerLocal = this.parent as EntityPlayerLocal;
		if (entityPlayerLocal != null)
		{
			entityPlayerLocal.PlayerUI.xui.Recipes.RefreshTrackedRecipe();
		}
		this.SkillPoints += num;
	}

	// Token: 0x060048DE RID: 18654 RVA: 0x001CD774 File Offset: 0x001CB974
	public void RefreshPerks(string attribute)
	{
		for (int i = 0; i < this.ProgressionValueQuickList.Count; i++)
		{
			ProgressionValue progressionValue = this.ProgressionValueQuickList[i];
			ProgressionClass progressionClass = progressionValue.ProgressionClass;
			if (progressionClass.IsPerk && (attribute == "" || attribute.EqualsCaseInsensitive(progressionClass.ParentName)))
			{
				progressionValue.CalculatedLevel(this.parent);
			}
		}
	}

	// Token: 0x040037F4 RID: 14324
	public const byte cVersion = 3;

	// Token: 0x040037F5 RID: 14325
	public static int BaseExpToLevel;

	// Token: 0x040037F6 RID: 14326
	public static int ClampExpCostAtLevel;

	// Token: 0x040037F7 RID: 14327
	public static float ExpMultiplier;

	// Token: 0x040037F8 RID: 14328
	public static int MaxLevel;

	// Token: 0x040037F9 RID: 14329
	public static int SkillPointsPerLevel;

	// Token: 0x040037FA RID: 14330
	public static float SkillPointMultiplier;

	// Token: 0x040037FB RID: 14331
	public static Dictionary<string, ProgressionClass> ProgressionClasses;

	// Token: 0x040037FC RID: 14332
	[PublicizedFrom(EAccessModifier.Private)]
	public static DictionaryNameIdMapping ProgressionNameIds = new DictionaryNameIdMapping();

	// Token: 0x040037FD RID: 14333
	public bool bProgressionStatsChanged;

	// Token: 0x040037FE RID: 14334
	public int ExpToNextLevel;

	// Token: 0x040037FF RID: 14335
	public int ExpDeficit;

	// Token: 0x04003800 RID: 14336
	public int Level = 1;

	// Token: 0x04003801 RID: 14337
	public int SkillPoints;

	// Token: 0x04003802 RID: 14338
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ExpDeficitGained;

	// Token: 0x04003803 RID: 14339
	[PublicizedFrom(EAccessModifier.Private)]
	public DictionaryNameId<ProgressionValue> ProgressionValues = new DictionaryNameId<ProgressionValue>(Progression.ProgressionNameIds);

	// Token: 0x04003804 RID: 14340
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ProgressionValue> ProgressionValueQuickList = new List<ProgressionValue>();

	// Token: 0x04003805 RID: 14341
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ProgressionValue> eventList = new List<ProgressionValue>();

	// Token: 0x04003806 RID: 14342
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<PassiveEffects, List<ProgressionValue>> passiveEffects = new Dictionary<PassiveEffects, List<ProgressionValue>>();

	// Token: 0x04003807 RID: 14343
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityAlive parent;

	// Token: 0x04003808 RID: 14344
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Global>[] xpFastTags;

	// Token: 0x04003809 RID: 14345
	[PublicizedFrom(EAccessModifier.Private)]
	public float timer = 1f;

	// Token: 0x02000967 RID: 2407
	public enum XPTypes
	{
		// Token: 0x0400380B RID: 14347
		Kill,
		// Token: 0x0400380C RID: 14348
		Harvesting,
		// Token: 0x0400380D RID: 14349
		Upgrading,
		// Token: 0x0400380E RID: 14350
		Crafting,
		// Token: 0x0400380F RID: 14351
		Selling,
		// Token: 0x04003810 RID: 14352
		Quest,
		// Token: 0x04003811 RID: 14353
		Looting,
		// Token: 0x04003812 RID: 14354
		Party,
		// Token: 0x04003813 RID: 14355
		Other,
		// Token: 0x04003814 RID: 14356
		Repairing,
		// Token: 0x04003815 RID: 14357
		Debug,
		// Token: 0x04003816 RID: 14358
		Max
	}
}
