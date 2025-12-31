using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020004F8 RID: 1272
public class ItemValue : IEquatable<ItemValue>
{
	// Token: 0x17000446 RID: 1094
	// (get) Token: 0x0600295D RID: 10589 RVA: 0x0010DC78 File Offset: 0x0010BE78
	public int MaxUseTimes
	{
		get
		{
			return (int)EffectManager.GetValue(PassiveEffects.DegradationMax, this, 0f, null, null, (this.ItemClass != null) ? this.ItemClass.ItemTags : FastTags<TagGroup.Global>.none, true, true, true, true, true, 1, true, false);
		}
	}

	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x0600295E RID: 10590 RVA: 0x0010DCB8 File Offset: 0x0010BEB8
	public float PercentUsesLeft
	{
		get
		{
			int maxUseTimes = this.MaxUseTimes;
			if (maxUseTimes > 0)
			{
				return 1f - Mathf.Clamp01(this.UseTimes / (float)maxUseTimes);
			}
			return 1f;
		}
	}

	// Token: 0x0600295F RID: 10591 RVA: 0x0010DCEA File Offset: 0x0010BEEA
	public bool HasMetadata(string key, TypedMetadataValue.TypeTag typeTag = TypedMetadataValue.TypeTag.None)
	{
		if (this.Metadata == null)
		{
			return false;
		}
		if (typeTag == TypedMetadataValue.TypeTag.None)
		{
			return this.Metadata.ContainsKey(key);
		}
		TypedMetadataValue typedMetadataValue = this.Metadata[key];
		return typedMetadataValue.ValueMatchesTag(typedMetadataValue.GetValue(), typeTag);
	}

	// Token: 0x06002960 RID: 10592 RVA: 0x0010DD20 File Offset: 0x0010BF20
	public object GetMetadata(string key)
	{
		if (this.Metadata == null)
		{
			return false;
		}
		TypedMetadataValue typedMetadataValue;
		if (this.Metadata.TryGetValue(key, out typedMetadataValue))
		{
			return typedMetadataValue.GetValue();
		}
		return null;
	}

	// Token: 0x06002961 RID: 10593 RVA: 0x0010DD54 File Offset: 0x0010BF54
	public void SetMetadata(string key, object value, string typeTag)
	{
		TypedMetadataValue.TypeTag typeTag2 = TypedMetadataValue.StringToTag(typeTag);
		this.SetMetadata(key, value, typeTag2);
	}

	// Token: 0x06002962 RID: 10594 RVA: 0x0010DD74 File Offset: 0x0010BF74
	public void SetMetadata(string key, object value, TypedMetadataValue.TypeTag typeTag)
	{
		if (this.Metadata == null)
		{
			this.Metadata = new Dictionary<string, TypedMetadataValue>();
		}
		TypedMetadataValue typedMetadataValue;
		if (this.Metadata.TryGetValue(key, out typedMetadataValue))
		{
			typedMetadataValue.SetValue(value);
			return;
		}
		this.Metadata.Add(key, new TypedMetadataValue(value, typeTag));
	}

	// Token: 0x06002963 RID: 10595 RVA: 0x0010DDC0 File Offset: 0x0010BFC0
	public void SetMetadata(string key, TypedMetadataValue tmv)
	{
		if (this.Metadata == null)
		{
			this.Metadata = new Dictionary<string, TypedMetadataValue>();
		}
		TypedMetadataValue typedMetadataValue;
		if (this.Metadata.TryGetValue(key, out typedMetadataValue))
		{
			typedMetadataValue.SetValue(tmv.GetValue());
			return;
		}
		this.Metadata.Add(key, tmv);
	}

	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x06002964 RID: 10596 RVA: 0x0010DE0C File Offset: 0x0010C00C
	public bool HasQuality
	{
		get
		{
			ItemClass itemClass = this.ItemClass;
			return itemClass != null && (itemClass.HasQuality || itemClass is ItemClassModifier);
		}
	}

	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x06002965 RID: 10597 RVA: 0x0010DE38 File Offset: 0x0010C038
	public bool HasModSlots
	{
		get
		{
			return this.Modifications.Length != 0;
		}
	}

	// Token: 0x1700044A RID: 1098
	// (get) Token: 0x06002966 RID: 10598 RVA: 0x0010DE44 File Offset: 0x0010C044
	public bool IsMod
	{
		get
		{
			ItemClass itemClass = this.ItemClass;
			return itemClass != null && itemClass is ItemClassModifier;
		}
	}

	// Token: 0x1700044B RID: 1099
	// (get) Token: 0x06002967 RID: 10599 RVA: 0x0010DE68 File Offset: 0x0010C068
	public bool IsShapeHelperBlock
	{
		get
		{
			ItemClassBlock itemClassBlock = this.ItemClass as ItemClassBlock;
			return itemClassBlock != null && itemClassBlock.GetBlock().SelectAlternates;
		}
	}

	// Token: 0x1700044C RID: 1100
	// (get) Token: 0x06002968 RID: 10600 RVA: 0x0010DE94 File Offset: 0x0010C094
	public ItemClass ItemClass
	{
		get
		{
			if (this.type < 0 || ItemClass.list == null || this.type >= ItemClass.list.Length)
			{
				return null;
			}
			ItemClass itemClass = ItemClass.list[this.type];
			if (itemClass is ItemClassQuest)
			{
				return ItemClassQuest.GetItemQuestById(this.Seed);
			}
			return itemClass;
		}
	}

	// Token: 0x1700044D RID: 1101
	// (get) Token: 0x06002969 RID: 10601 RVA: 0x0010DEE4 File Offset: 0x0010C0E4
	public ItemClass ItemClassOrMissing
	{
		get
		{
			ItemClass itemClass = this.ItemClass;
			if (itemClass != null)
			{
				return itemClass;
			}
			return ItemClass.MissingItem;
		}
	}

	// Token: 0x0600296A RID: 10602 RVA: 0x0010DF02 File Offset: 0x0010C102
	public ItemValue()
	{
		this.Modifications = ItemValue.emptyItemValueArray;
		this.CosmeticMods = ItemValue.emptyItemValueArray;
	}

	// Token: 0x0600296B RID: 10603 RVA: 0x0010DF20 File Offset: 0x0010C120
	public ItemValue(int _type, bool _bCreateDefaultParts = false) : this(_type, 1, 6, _bCreateDefaultParts, null, 1f)
	{
	}

	// Token: 0x0600296C RID: 10604 RVA: 0x0010DF34 File Offset: 0x0010C134
	public ItemValue(int _type, int minQuality, int maxQuality, bool _bCreateDefaultModItems = false, string[] modsToInstall = null, float modInstallDescendingChance = 1f)
	{
		this.type = _type;
		this.Modifications = ItemValue.emptyItemValueArray;
		this.CosmeticMods = ItemValue.emptyItemValueArray;
		if (this.type == 0)
		{
			return;
		}
		DateTime utcNow = DateTime.UtcNow;
		this.Seed = (ushort)((utcNow - ItemValue.baseDate).Seconds + utcNow.Millisecond + this.type);
		if (!ThreadManager.IsMainThread())
		{
			return;
		}
		ItemClass itemClass = this.ItemClass;
		if (itemClass == null)
		{
			return;
		}
		GameRandom gameRandom = null;
		if (itemClass.HasQuality)
		{
			gameRandom = GameRandomManager.Instance.CreateGameRandom((int)this.Seed);
			this.Quality = (ushort)Math.Min(65535, gameRandom.RandomRange(minQuality, maxQuality + 1));
		}
		if (itemClass is ItemClassModifier)
		{
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
			return;
		}
		if (itemClass.Stacknumber.Value > 1)
		{
			GameRandomManager.Instance.FreeGameRandom(gameRandom);
			return;
		}
		this.Modifications = new ItemValue[Math.Min(255, (int)EffectManager.GetValue(PassiveEffects.ModSlots, this, (float)Utils.FastMax(0, (int)(this.Quality - 1)), null, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false))];
		this.CosmeticMods = new ItemValue[itemClass.HasAnyTags(ItemClassModifier.CosmeticItemTags) ? 1 : 0];
		if (_bCreateDefaultModItems)
		{
			if (gameRandom == null)
			{
				gameRandom = GameRandomManager.Instance.CreateGameRandom((int)this.Seed);
			}
			this.createDefaultModItems(itemClass, gameRandom, modsToInstall, modInstallDescendingChance);
		}
		GameRandomManager.Instance.FreeGameRandom(gameRandom);
	}

	// Token: 0x0600296D RID: 10605 RVA: 0x0010E0A0 File Offset: 0x0010C2A0
	public void Clear()
	{
		this.type = 0;
		this.UseTimes = 0f;
		this.Quality = 0;
		this.Meta = 0;
		this.Seed = 0;
		this.Modifications = new ItemValue[0];
		this.CosmeticMods = new ItemValue[0];
		this.Metadata = null;
		this.SelectedAmmoTypeIndex = 0;
	}

	// Token: 0x0600296E RID: 10606 RVA: 0x0010E0FC File Offset: 0x0010C2FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void createDefaultModItems(ItemClass ic, GameRandom random, string[] modsToInstall, float modInstallDescendingChance)
	{
		FastTags<TagGroup.Global> fastTags = FastTags<TagGroup.Global>.none;
		bool flag = false;
		bool flag2 = false;
		if (modsToInstall != null && modsToInstall.Length != 0)
		{
			float num = modInstallDescendingChance;
			if (!ic.ItemTags.IsEmpty)
			{
				int num2 = 0;
				for (int i = 0; i < modsToInstall.Length; i++)
				{
					ItemClassModifier itemClassModifier = ItemClass.GetItemClass(modsToInstall[i], true) as ItemClassModifier;
					if (itemClassModifier == null)
					{
						itemClassModifier = ItemClassModifier.GetDesiredItemModWithAnyTags(ic.ItemTags, fastTags, FastTags<TagGroup.Global>.Parse(modsToInstall[i]), random);
					}
					if (itemClassModifier != null)
					{
						if (itemClassModifier.HasAnyTags(ItemClassModifier.CosmeticModTypes))
						{
							flag = true;
							if (!flag2 && random.RandomFloat <= modInstallDescendingChance)
							{
								this.CosmeticMods[0] = new ItemValue(itemClassModifier.Id, false);
								fastTags |= itemClassModifier.ItemTags;
								flag2 = true;
								Log.Warning("ItemValue createDefaultModItems cosmetic {0}", new object[]
								{
									this.CosmeticMods[0]
								});
							}
						}
						else if (num2 < this.Modifications.Length && random.RandomFloat <= num)
						{
							this.Modifications[num2] = new ItemValue(itemClassModifier.Id, false);
							fastTags |= itemClassModifier.ItemTags;
							num2++;
							num *= 0.5f;
						}
					}
				}
				for (int j = num2; j < this.Modifications.Length; j++)
				{
					this.Modifications[j] = ItemValue.None.Clone();
				}
			}
		}
		if (!flag && !ic.HasAnyTags(ItemValue.noPreinstallCosmeticItemTags))
		{
			for (int k = 0; k < this.CosmeticMods.Length; k++)
			{
				ItemClassModifier cosmeticItemMod = ItemClassModifier.GetCosmeticItemMod(ic.ItemTags, fastTags, random);
				if (cosmeticItemMod != null)
				{
					this.CosmeticMods[k] = new ItemValue(cosmeticItemMod.Id, false);
					fastTags |= cosmeticItemMod.ItemTags;
				}
				else
				{
					this.CosmeticMods[k] = ItemValue.None.Clone();
				}
			}
		}
	}

	// Token: 0x0600296F RID: 10607 RVA: 0x0010E2CC File Offset: 0x0010C4CC
	public float GetValue(EntityAlive _entity, PassiveEffects _passiveEffect, FastTags<TagGroup.Global> _tags)
	{
		float num = 0f;
		float num2 = 1f;
		ItemClass itemClass = this.ItemClass;
		if (itemClass != null)
		{
			if (_entity != null)
			{
				MinEventParams.CopyTo(_entity.MinEventContext, MinEventParams.CachedEventParam);
			}
			if (itemClass.Actions != null && itemClass.Actions.Length != 0 && itemClass.Actions[0] is ItemActionRanged)
			{
				string[] magazineItemNames = (itemClass.Actions[0] as ItemActionRanged).MagazineItemNames;
				if (magazineItemNames != null)
				{
					ItemClass.GetItem(magazineItemNames[(int)this.SelectedAmmoTypeIndex], false).ModifyValue(_entity, this, _passiveEffect, ref num, ref num2, _tags, true, false);
				}
			}
			if (itemClass.Effects != null)
			{
				int seed = MinEventParams.CachedEventParam.Seed;
				if (_entity != null)
				{
					seed = _entity.MinEventContext.Seed;
				}
				MinEventParams.CachedEventParam.Seed = (int)((byte)this.Seed + ((this.Seed != 0) ? _passiveEffect : PassiveEffects.None));
				if (_entity != null)
				{
					_entity.MinEventContext.Seed = MinEventParams.CachedEventParam.Seed;
				}
				itemClass.Effects.ModifyValue(_entity, _passiveEffect, ref num, ref num2, (float)this.Quality, _tags, 1);
				MinEventParams.CachedEventParam.Seed = seed;
				if (_entity != null)
				{
					_entity.MinEventContext.Seed = seed;
				}
			}
		}
		return num * num2;
	}

	// Token: 0x06002970 RID: 10608 RVA: 0x0010E408 File Offset: 0x0010C608
	public void ModifyValue(EntityAlive _entity, ItemValue _originalItemValue, PassiveEffects _passiveEffect, ref float _originalValue, ref float _perc_value, FastTags<TagGroup.Global> _tags, bool _useMods = true, bool _useDurability = false)
	{
		if (_originalItemValue == null || !_originalItemValue.Equals(this))
		{
			int seed = MinEventParams.CachedEventParam.Seed;
			if (_entity != null)
			{
				seed = _entity.MinEventContext.Seed;
			}
			ItemClass itemClass = this.ItemClass;
			if (itemClass != null)
			{
				if (itemClass.Actions != null && itemClass.Actions.Length != 0 && itemClass.Actions[0] is ItemActionRanged)
				{
					string[] magazineItemNames = (itemClass.Actions[0] as ItemActionRanged).MagazineItemNames;
					if (magazineItemNames != null)
					{
						ItemClass itemClass2 = ItemClass.GetItemClass(magazineItemNames[(int)this.SelectedAmmoTypeIndex], false);
						if (itemClass2 != null && itemClass2.Effects != null)
						{
							itemClass2.Effects.ModifyValue(_entity, _passiveEffect, ref _originalValue, ref _perc_value, 0f, _tags, 1);
						}
					}
				}
				if (itemClass.Effects != null)
				{
					ItemValue itemValue = MinEventParams.CachedEventParam.ItemValue;
					ItemValue itemValue2 = (_entity != null) ? _entity.MinEventContext.ItemValue : null;
					MinEventParams.CachedEventParam.Seed = (int)((byte)this.Seed + ((this.Seed != 0) ? _passiveEffect : PassiveEffects.None));
					MinEventParams.CachedEventParam.ItemValue = this;
					if (_entity != null)
					{
						_entity.MinEventContext.Seed = MinEventParams.CachedEventParam.Seed;
						_entity.MinEventContext.ItemValue = this;
					}
					float num = _originalValue;
					itemClass.Effects.ModifyValue(_entity, _passiveEffect, ref _originalValue, ref _perc_value, (float)this.Quality, _tags, 1);
					if (_useDurability)
					{
						if (_passiveEffect != PassiveEffects.PhysicalDamageResist)
						{
							if (_passiveEffect != PassiveEffects.ElementalDamageResist)
							{
								if (_passiveEffect == PassiveEffects.BuffResistance)
								{
									if (this.PercentUsesLeft < 0.5f)
									{
										float num2 = _originalValue - num;
										_originalValue = num + num2 * this.PercentUsesLeft * 2f;
									}
								}
							}
							else if (this.PercentUsesLeft < 0.5f)
							{
								float num3 = _originalValue - num;
								_originalValue = num + num3 * this.PercentUsesLeft * 2f;
							}
						}
						else if (this.PercentUsesLeft < 0.5f)
						{
							float num4 = _originalValue - num;
							_originalValue = num + num4 * this.PercentUsesLeft * 2f;
						}
					}
					MinEventParams.CachedEventParam.ItemValue = itemValue;
					if (_entity != null)
					{
						_entity.MinEventContext.ItemValue = itemValue2;
					}
				}
			}
			if (_useMods)
			{
				for (int i = 0; i < this.CosmeticMods.Length; i++)
				{
					if (this.CosmeticMods[i] != null && this.CosmeticMods[i].ItemClass is ItemClassModifier)
					{
						this.CosmeticMods[i].ModifyValue(_entity, _originalItemValue, _passiveEffect, ref _originalValue, ref _perc_value, _tags, true, false);
					}
				}
				for (int j = 0; j < this.Modifications.Length; j++)
				{
					if (this.Modifications[j] != null && this.Modifications[j].ItemClass is ItemClassModifier)
					{
						this.Modifications[j].ModifyValue(_entity, _originalItemValue, _passiveEffect, ref _originalValue, ref _perc_value, _tags, true, false);
					}
				}
			}
			MinEventParams.CachedEventParam.Seed = seed;
			if (_entity != null)
			{
				_entity.MinEventContext.Seed = seed;
			}
		}
	}

	// Token: 0x06002971 RID: 10609 RVA: 0x0010E6E8 File Offset: 0x0010C8E8
	public void GetModifiedValueData(List<EffectManager.ModifierValuesAndSources> _modValueSources, EffectManager.ModifierValuesAndSources.ValueSourceType _sourceType, EntityAlive _entity, ItemValue _originalItemValue, PassiveEffects _passiveEffect, ref float _originalValue, ref float _perc_value, FastTags<TagGroup.Global> _tags)
	{
		if (_originalItemValue == null || !_originalItemValue.Equals(this))
		{
			ItemClass itemClass = this.ItemClass;
			if (itemClass != null)
			{
				if (itemClass.Actions != null && itemClass.Actions.Length != 0 && itemClass.Actions[0] is ItemActionRanged)
				{
					string[] magazineItemNames = (itemClass.Actions[0] as ItemActionRanged).MagazineItemNames;
					if (magazineItemNames != null)
					{
						ItemClass.GetItem(magazineItemNames[(int)this.SelectedAmmoTypeIndex], false).GetModifiedValueData(_modValueSources, EffectManager.ModifierValuesAndSources.ValueSourceType.Ammo, _entity, _originalItemValue, _passiveEffect, ref _originalValue, ref _perc_value, _tags);
					}
				}
				if (itemClass.Effects != null)
				{
					itemClass.Effects.GetModifiedValueData(_modValueSources, _sourceType, _entity, _passiveEffect, ref _originalValue, ref _perc_value, (float)this.Quality, _tags, 1);
				}
			}
			for (int i = 0; i < this.CosmeticMods.Length; i++)
			{
				if (this.CosmeticMods[i] != null && this.CosmeticMods[i].ItemClass is ItemClassModifier)
				{
					this.CosmeticMods[i].GetModifiedValueData(_modValueSources, EffectManager.ModifierValuesAndSources.ValueSourceType.CosmeticMod, _entity, _originalItemValue, _passiveEffect, ref _originalValue, ref _perc_value, _tags);
				}
			}
			for (int j = 0; j < this.Modifications.Length; j++)
			{
				if (this.Modifications[j] != null && this.Modifications[j].ItemClass is ItemClassModifier)
				{
					this.Modifications[j].GetModifiedValueData(_modValueSources, EffectManager.ModifierValuesAndSources.ValueSourceType.Mod, _entity, _originalItemValue, _passiveEffect, ref _originalValue, ref _perc_value, _tags);
				}
			}
		}
	}

	// Token: 0x06002972 RID: 10610 RVA: 0x0010E824 File Offset: 0x0010CA24
	public void FireEvent(MinEventTypes _eventType, MinEventParams _eventParms)
	{
		ItemClass itemClass = this.ItemClass;
		if (itemClass != null)
		{
			if (itemClass is ItemClassModifier && itemClass.Effects != null)
			{
				itemClass.Effects.FireEvent(_eventType, _eventParms);
				return;
			}
			if (itemClass.Actions != null && itemClass.Actions.Length != 0 && itemClass.Actions[0] is ItemActionRanged)
			{
				string[] magazineItemNames = (itemClass.Actions[0] as ItemActionRanged).MagazineItemNames;
				if (magazineItemNames != null)
				{
					ItemClass.GetItem(magazineItemNames[(int)this.SelectedAmmoTypeIndex], false).FireEvent(_eventType, _eventParms);
				}
			}
			itemClass.FireEvent(_eventType, _eventParms);
		}
		if (!this.HasQuality)
		{
			return;
		}
		for (int i = 0; i < this.Modifications.Length; i++)
		{
			if (this.Modifications[i] != null)
			{
				this.Modifications[i].FireEvent(_eventType, _eventParms);
			}
		}
		for (int j = 0; j < this.CosmeticMods.Length; j++)
		{
			if (this.CosmeticMods[j] != null)
			{
				this.CosmeticMods[j].FireEvent(_eventType, _eventParms);
			}
		}
	}

	// Token: 0x06002973 RID: 10611 RVA: 0x0010E910 File Offset: 0x0010CB10
	public ItemValue Clone()
	{
		ItemValue itemValue = new ItemValue(this.type, false);
		itemValue.Meta = this.Meta;
		itemValue.UseTimes = this.UseTimes;
		itemValue.Quality = this.Quality;
		itemValue.SelectedAmmoTypeIndex = this.SelectedAmmoTypeIndex;
		itemValue.Modifications = new ItemValue[this.Modifications.Length];
		for (int i = 0; i < this.Modifications.Length; i++)
		{
			itemValue.Modifications[i] = ((this.Modifications[i] != null) ? this.Modifications[i].Clone() : null);
		}
		if (this.Metadata != null)
		{
			itemValue.Metadata = new Dictionary<string, TypedMetadataValue>();
			foreach (KeyValuePair<string, TypedMetadataValue> keyValuePair in this.Metadata)
			{
				Dictionary<string, TypedMetadataValue> metadata = itemValue.Metadata;
				string key = keyValuePair.Key;
				TypedMetadataValue value = keyValuePair.Value;
				metadata.Add(key, (value != null) ? value.Clone() : null);
			}
		}
		itemValue.CosmeticMods = new ItemValue[this.CosmeticMods.Length];
		for (int j = 0; j < this.CosmeticMods.Length; j++)
		{
			itemValue.CosmeticMods[j] = ((this.CosmeticMods[j] != null) ? this.CosmeticMods[j].Clone() : null);
		}
		itemValue.Activated = this.Activated;
		if (itemValue.type == 0)
		{
			this.Seed = 0;
		}
		itemValue.Seed = this.Seed;
		itemValue.TextureFullArray = this.TextureFullArray;
		return itemValue;
	}

	// Token: 0x06002974 RID: 10612 RVA: 0x0010EA9C File Offset: 0x0010CC9C
	public bool IsEmpty()
	{
		return this.type == 0;
	}

	// Token: 0x06002975 RID: 10613 RVA: 0x0010EAA8 File Offset: 0x0010CCA8
	public BlockValue ToBlockValue()
	{
		if (this.type < Block.ItemsStartHere)
		{
			return new BlockValue
			{
				type = this.type
			};
		}
		return BlockValue.Air;
	}

	// Token: 0x06002976 RID: 10614 RVA: 0x00002914 File Offset: 0x00000B14
	public void ReadOld(BinaryReader _br)
	{
	}

	// Token: 0x06002977 RID: 10615 RVA: 0x0010EAE0 File Offset: 0x0010CCE0
	public static ItemValue ReadOrNull(BinaryReader _br)
	{
		byte b = _br.ReadByte();
		if (b == 0)
		{
			return null;
		}
		ItemValue itemValue = new ItemValue();
		itemValue.ReadData(_br, (int)b);
		return itemValue;
	}

	// Token: 0x06002978 RID: 10616 RVA: 0x0010EB08 File Offset: 0x0010CD08
	public void Read(BinaryReader _br)
	{
		byte b = _br.ReadByte();
		if (b == 0)
		{
			this.type = 0;
			return;
		}
		this.ReadData(_br, (int)b);
	}

	// Token: 0x06002979 RID: 10617 RVA: 0x0010EB30 File Offset: 0x0010CD30
	[PublicizedFrom(EAccessModifier.Private)]
	public void ReadData(BinaryReader _br, int version)
	{
		int num = 0;
		if (version >= 8)
		{
			num = (int)_br.ReadByte();
		}
		this.type = (int)_br.ReadUInt16();
		if ((num & 1) > 0)
		{
			this.type += Block.ItemsStartHere;
		}
		if (version < 8 && this.type >= 32768)
		{
			this.type += 32768;
		}
		if (version > 5)
		{
			this.UseTimes = _br.ReadSingle();
		}
		else
		{
			this.UseTimes = (float)_br.ReadUInt16();
		}
		this.Quality = _br.ReadUInt16();
		this.Meta = (int)_br.ReadUInt16();
		if (this.Meta >= 65535)
		{
			this.Meta = -1;
		}
		if (version > 6)
		{
			int num2 = (int)_br.ReadByte();
			for (int i = 0; i < num2; i++)
			{
				string key = _br.ReadString();
				TypedMetadataValue tmv = TypedMetadataValue.Read(_br);
				this.SetMetadata(key, tmv);
			}
		}
		if ((version > 4 || this.HasQuality) && !(this.ItemClass is ItemClassModifier))
		{
			byte b = _br.ReadByte();
			this.Modifications = new ItemValue[(int)b];
			if (b != 0)
			{
				for (int j = 0; j < (int)b; j++)
				{
					if (_br.ReadBoolean())
					{
						this.Modifications[j] = new ItemValue();
						this.Modifications[j].Read(_br);
					}
					else
					{
						this.Modifications[j] = ItemValue.None.Clone();
					}
				}
			}
			b = _br.ReadByte();
			this.CosmeticMods = new ItemValue[(int)b];
			if (b != 0)
			{
				for (int k = 0; k < (int)b; k++)
				{
					if (_br.ReadBoolean())
					{
						this.CosmeticMods[k] = new ItemValue();
						this.CosmeticMods[k].Read(_br);
					}
					else
					{
						this.CosmeticMods[k] = ItemValue.None.Clone();
					}
				}
			}
		}
		if (version > 1)
		{
			this.Activated = _br.ReadByte();
		}
		if (version > 2)
		{
			this.SelectedAmmoTypeIndex = _br.ReadByte();
		}
		if (version > 3)
		{
			this.Seed = _br.ReadUInt16();
			if (this.type == 0)
			{
				this.Seed = 0;
			}
		}
		if (version > 8)
		{
			if (_br.ReadBoolean())
			{
				this.TextureFullArray.Read(_br, 1);
				return;
			}
			this.TextureFullArray.Fill(0L);
		}
	}

	// Token: 0x0600297A RID: 10618 RVA: 0x0010ED58 File Offset: 0x0010CF58
	public static void Write(ItemValue _iv, BinaryWriter _bw)
	{
		if (_iv == null)
		{
			_bw.Write(0);
			return;
		}
		_iv.Write(_bw);
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x0010ED6C File Offset: 0x0010CF6C
	public void Write(BinaryWriter _bw)
	{
		if (this.IsEmpty())
		{
			_bw.Write(0);
			return;
		}
		_bw.Write(9);
		int num = this.type;
		byte value = 0;
		if (this.type >= Block.ItemsStartHere)
		{
			value = 1;
			num -= Block.ItemsStartHere;
		}
		_bw.Write(value);
		_bw.Write((ushort)num);
		_bw.Write(this.UseTimes);
		_bw.Write(this.Quality);
		_bw.Write((ushort)this.Meta);
		int num2 = (this.Metadata != null) ? this.Metadata.Count : 0;
		_bw.Write((byte)num2);
		if (this.Metadata != null)
		{
			foreach (string text in this.Metadata.Keys)
			{
				TypedMetadataValue typedMetadataValue = this.Metadata[text];
				if (((typedMetadataValue != null) ? typedMetadataValue.GetValue() : null) != null)
				{
					_bw.Write(text);
					TypedMetadataValue.Write(this.Metadata[text], _bw);
				}
			}
		}
		if (!(this.ItemClass is ItemClassModifier))
		{
			_bw.Write((byte)this.Modifications.Length);
			for (int i = 0; i < this.Modifications.Length; i++)
			{
				bool flag = this.Modifications[i] != null && !this.Modifications[i].IsEmpty();
				_bw.Write(flag);
				if (flag)
				{
					this.Modifications[i].Write(_bw);
				}
			}
			_bw.Write((byte)this.CosmeticMods.Length);
			for (int j = 0; j < this.CosmeticMods.Length; j++)
			{
				bool flag2 = this.CosmeticMods[j] != null && !this.CosmeticMods[j].IsEmpty();
				_bw.Write(flag2);
				if (flag2)
				{
					this.CosmeticMods[j].Write(_bw);
				}
			}
		}
		_bw.Write(this.Activated);
		_bw.Write(this.SelectedAmmoTypeIndex);
		if (this.type == 0)
		{
			this.Seed = 0;
		}
		_bw.Write(this.Seed);
		if (this.TextureFullArray.IsDefault)
		{
			_bw.Write(false);
		}
		else
		{
			_bw.Write(true);
			this.TextureFullArray.Write(_bw);
		}
		ItemClass itemClass = ItemClass.list[this.type];
		if (itemClass == null)
		{
			if (this.type != 0)
			{
				Log.Error("No ItemClass entry for type " + this.type.ToString());
				return;
			}
		}
		else
		{
			NameIdMapping nameIdMapping;
			if (itemClass.IsBlock())
			{
				nameIdMapping = Block.nameIdMapping;
			}
			else
			{
				nameIdMapping = ItemClass.nameIdMapping;
			}
			if (nameIdMapping != null)
			{
				nameIdMapping.AddMapping(this.type, itemClass.Name, false);
			}
		}
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x0010F01C File Offset: 0x0010D21C
	public bool Equals(ItemValue _other)
	{
		return _other != null && (_other.type == this.type && _other.UseTimes == this.UseTimes && _other.Meta == this.Meta && _other.Seed == this.Seed && _other.Quality == this.Quality && _other.SelectedAmmoTypeIndex == this.SelectedAmmoTypeIndex && _other.Activated == this.Activated && ItemValue.Equals(_other.Metadata, this.Metadata) && ItemValue.Equals(_other.CosmeticMods, this.CosmeticMods)) && ItemValue.Equals(_other.Modifications, this.Modifications);
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x0010F0CC File Offset: 0x0010D2CC
	public override bool Equals(object _other)
	{
		return _other is ItemValue && this.Equals((ItemValue)_other);
	}

	// Token: 0x0600297E RID: 10622 RVA: 0x0010F0E4 File Offset: 0x0010D2E4
	public bool EqualsExceptUseTimesAndAmmo(ItemValue _other)
	{
		if (_other == null)
		{
			return false;
		}
		bool flag = this.ItemClass != null && this.ItemClass.Actions != null && this.ItemClass.Actions.Length != 0 && this.ItemClass.Actions[0] is ItemActionRanged;
		return _other.type == this.type && (flag || _other.Meta == this.Meta) && _other.Seed == this.Seed && _other.Quality == this.Quality && _other.SelectedAmmoTypeIndex == this.SelectedAmmoTypeIndex && _other.Activated == this.Activated && ItemValue.Equals(_other.Metadata, this.Metadata) && ItemValue.Equals(_other.CosmeticMods, this.CosmeticMods) && ItemValue.Equals(_other.Modifications, this.Modifications);
	}

	// Token: 0x0600297F RID: 10623 RVA: 0x0010F1C8 File Offset: 0x0010D3C8
	public static bool Equals(ItemValue[] _a, ItemValue[] _b)
	{
		if (_a == null && _b == null)
		{
			return true;
		}
		if (_a == null || _b == null)
		{
			return false;
		}
		if (_a.Length != _b.Length)
		{
			return false;
		}
		if (_a.Length == 0)
		{
			return true;
		}
		for (int i = 0; i < _a.Length; i++)
		{
			if (_a[i] != null || _b[i] != null)
			{
				if (_a[i] == null || _b[i] == null)
				{
					return false;
				}
				if (!_a[i].Equals(_b[i]))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06002980 RID: 10624 RVA: 0x0010F229 File Offset: 0x0010D429
	public static bool Equals(object[] _a, object[] _b)
	{
		return _a is ItemValue[] && _b is ItemValue[] && ItemValue.Equals((ItemValue[])_a, (ItemValue[])_b);
	}

	// Token: 0x06002981 RID: 10625 RVA: 0x0010F250 File Offset: 0x0010D450
	public static bool Equals(Dictionary<string, TypedMetadataValue> _a, Dictionary<string, TypedMetadataValue> _b)
	{
		if (_a == null && _b == null)
		{
			return true;
		}
		if (_a == null || _b == null)
		{
			return false;
		}
		if (_a.Count != _b.Count)
		{
			return false;
		}
		if (_a.Count == 0)
		{
			return true;
		}
		foreach (string key in _a.Keys)
		{
			if (!_b.ContainsKey(key))
			{
				return false;
			}
			if (!_a[key].Equals(_b[key]))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002982 RID: 10626 RVA: 0x0010F2F0 File Offset: 0x0010D4F0
	public override int GetHashCode()
	{
		return this.type;
	}

	// Token: 0x06002983 RID: 10627 RVA: 0x0010F2F8 File Offset: 0x0010D4F8
	public int GetItemId()
	{
		return this.type - Block.ItemsStartHere;
	}

	// Token: 0x06002984 RID: 10628 RVA: 0x0010F306 File Offset: 0x0010D506
	public int GetItemOrBlockId()
	{
		if (this.type < Block.ItemsStartHere)
		{
			return this.type;
		}
		return this.type - Block.ItemsStartHere;
	}

	// Token: 0x06002985 RID: 10629 RVA: 0x0010F328 File Offset: 0x0010D528
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			(this.type >= Block.ItemsStartHere) ? ("item=" + (this.type - Block.ItemsStartHere).ToString()) : ("block=" + this.type.ToString()),
			" m=",
			this.Meta.ToString(),
			" ut=",
			this.UseTimes.ToString()
		});
	}

	// Token: 0x06002986 RID: 10630 RVA: 0x0010F3B4 File Offset: 0x0010D5B4
	public string GetPropertyOverride(string _propertyName, string _originalValue)
	{
		if (this.Modifications.Length == 0 && this.CosmeticMods.Length == 0)
		{
			return _originalValue;
		}
		string result = "";
		string itemName = this.ItemClass.GetItemName();
		for (int i = 0; i < this.Modifications.Length; i++)
		{
			ItemValue itemValue = this.Modifications[i];
			if (itemValue != null)
			{
				ItemClassModifier itemClassModifier = itemValue.ItemClass as ItemClassModifier;
				if (itemClassModifier != null && itemClassModifier.GetPropertyOverride(_propertyName, itemName, ref result))
				{
					return result;
				}
			}
		}
		result = "";
		for (int j = 0; j < this.CosmeticMods.Length; j++)
		{
			ItemValue itemValue2 = this.CosmeticMods[j];
			if (itemValue2 != null)
			{
				ItemClassModifier itemClassModifier2 = itemValue2.ItemClass as ItemClassModifier;
				if (itemClassModifier2 != null && itemClassModifier2.GetPropertyOverride(_propertyName, itemName, ref result))
				{
					return result;
				}
			}
		}
		return _originalValue;
	}

	// Token: 0x06002987 RID: 10631 RVA: 0x0010F474 File Offset: 0x0010D674
	public bool HasMods()
	{
		for (int i = 0; i < this.Modifications.Length; i++)
		{
			ItemValue itemValue = this.Modifications[i];
			if (itemValue != null && !itemValue.IsEmpty())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400204F RID: 8271
	[PublicizedFrom(EAccessModifier.Private)]
	public const int EmptySaveVersion = 0;

	// Token: 0x04002050 RID: 8272
	[PublicizedFrom(EAccessModifier.Private)]
	public const int CurrentSaveVersion = 9;

	// Token: 0x04002051 RID: 8273
	[PublicizedFrom(EAccessModifier.Private)]
	public const byte MaxModifications = 255;

	// Token: 0x04002052 RID: 8274
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ItemValue[] emptyItemValueArray = new ItemValue[0];

	// Token: 0x04002053 RID: 8275
	public static ItemValue None = new ItemValue(0, false);

	// Token: 0x04002054 RID: 8276
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> noPreinstallCosmeticItemTags = FastTags<TagGroup.Global>.Parse("weapon,tool,armor");

	// Token: 0x04002055 RID: 8277
	public int type;

	// Token: 0x04002056 RID: 8278
	public byte Activated;

	// Token: 0x04002057 RID: 8279
	public byte SelectedAmmoTypeIndex;

	// Token: 0x04002058 RID: 8280
	public float UseTimes;

	// Token: 0x04002059 RID: 8281
	public int Meta;

	// Token: 0x0400205A RID: 8282
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, TypedMetadataValue> Metadata;

	// Token: 0x0400205B RID: 8283
	public ushort Quality;

	// Token: 0x0400205C RID: 8284
	public ItemValue[] Modifications;

	// Token: 0x0400205D RID: 8285
	public ItemValue[] CosmeticMods;

	// Token: 0x0400205E RID: 8286
	public ushort Seed;

	// Token: 0x0400205F RID: 8287
	public TextureFullArray TextureFullArray;

	// Token: 0x04002060 RID: 8288
	[PublicizedFrom(EAccessModifier.Private)]
	public static DateTime baseDate = new DateTime(2013, 10, 1);

	// Token: 0x04002061 RID: 8289
	[PublicizedFrom(EAccessModifier.Private)]
	public const byte cFlagsItem = 1;
}
