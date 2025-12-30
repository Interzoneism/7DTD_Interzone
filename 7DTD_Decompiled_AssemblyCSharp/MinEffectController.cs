using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

// Token: 0x02000616 RID: 1558
public class MinEffectController
{
	// Token: 0x06003052 RID: 12370 RVA: 0x001491BC File Offset: 0x001473BC
	public bool IsOwnerTiered()
	{
		byte b = 0;
		while ((int)b < this.EffectGroups.Count)
		{
			if (this.EffectGroups[(int)b].OwnerTiered)
			{
				return true;
			}
			b += 1;
		}
		return false;
	}

	// Token: 0x06003053 RID: 12371 RVA: 0x001491F8 File Offset: 0x001473F8
	public void ModifyValue(EntityAlive _self, PassiveEffects _effect, ref float _base_value, ref float _perc_value, float _level = 0f, FastTags<TagGroup.Global> _tags = default(FastTags<TagGroup.Global>), int multiplier = 1)
	{
		if (!this.PassivesIndex.Contains(_effect))
		{
			return;
		}
		MinEventParams minEventParams;
		if (_self == null)
		{
			minEventParams = MinEventParams.CachedEventParam;
			minEventParams.Self = null;
		}
		else
		{
			minEventParams = _self.MinEventContext;
		}
		for (int i = 0; i < this.EffectGroups.Count; i++)
		{
			if (!minEventParams.Tags.Equals(_tags))
			{
				minEventParams.Tags = new FastTags<TagGroup.Global>(_tags);
			}
			this.EffectGroups[i].ModifyValue(minEventParams, _self, _effect, ref _base_value, ref _perc_value, _level, _tags, multiplier);
		}
	}

	// Token: 0x06003054 RID: 12372 RVA: 0x00149284 File Offset: 0x00147484
	public void GetModifiedValueData(List<EffectManager.ModifierValuesAndSources> _modValueSources, EffectManager.ModifierValuesAndSources.ValueSourceType _sourceType, EntityAlive _self, PassiveEffects _effect, ref float _base_value, ref float _perc_value, float _level = 0f, FastTags<TagGroup.Global> _tags = default(FastTags<TagGroup.Global>), int multiplier = 1)
	{
		if (!this.PassivesIndex.Contains(_effect))
		{
			return;
		}
		byte b = 0;
		while ((int)b < this.EffectGroups.Count)
		{
			this.EffectGroups[(int)b].GetModifiedValueData(_modValueSources, _sourceType, this.ParentType, _self, _effect, ref _base_value, ref _perc_value, _level, _tags, multiplier, this.ParentPointer);
			b += 1;
		}
	}

	// Token: 0x06003055 RID: 12373 RVA: 0x001492E4 File Offset: 0x001474E4
	public void FireEvent(MinEventTypes _eventType, MinEventParams _eventParms)
	{
		_eventParms.ParentType = this.ParentType;
		byte b = 0;
		while ((int)b < this.EffectGroups.Count)
		{
			this.EffectGroups[(int)b].FireEvent(_eventType, _eventParms);
			b += 1;
		}
	}

	// Token: 0x06003056 RID: 12374 RVA: 0x00149328 File Offset: 0x00147528
	public bool HasEvents()
	{
		byte b = 0;
		while ((int)b < this.EffectGroups.Count)
		{
			if (this.EffectGroups[(int)b].HasEvents())
			{
				return true;
			}
			b += 1;
		}
		return false;
	}

	// Token: 0x06003057 RID: 12375 RVA: 0x00149364 File Offset: 0x00147564
	public bool HasTrigger(MinEventTypes _eventType)
	{
		byte b = 0;
		while ((int)b < this.EffectGroups.Count)
		{
			if (this.EffectGroups[(int)b].HasTrigger(_eventType))
			{
				return true;
			}
			b += 1;
		}
		return false;
	}

	// Token: 0x06003058 RID: 12376 RVA: 0x001493A0 File Offset: 0x001475A0
	public void AddEffectGroup(MinEffectGroup item, int _order = 0, bool _extends = false)
	{
		if (this.EffectGroups == null)
		{
			this.EffectGroups = new List<MinEffectGroup>();
		}
		if (this.PassivesIndex == null)
		{
			this.PassivesIndex = new HashSet<PassiveEffects>(EffectManager.PassiveEffectsComparer);
		}
		if (_extends)
		{
			for (int i = 0; i < this.EffectGroups.Count; i++)
			{
				MinEffectGroup minEffectGroup = this.EffectGroups[i];
				if (minEffectGroup.Requirements == null)
				{
					for (int j = 0; j < minEffectGroup.PassiveEffects.Count; j++)
					{
						PassiveEffect passiveEffect = minEffectGroup.PassiveEffects[j];
						if (passiveEffect.Tags.IsEmpty && (passiveEffect.Modifier == PassiveEffect.ValueModifierTypes.base_set || passiveEffect.Modifier == PassiveEffect.ValueModifierTypes.perc_set))
						{
							for (int k = item.PassiveEffects.Count - 1; k >= 0; k--)
							{
								PassiveEffect passiveEffect2 = item.PassiveEffects[k];
								if (passiveEffect2.Type == passiveEffect.Type && passiveEffect2.Modifier == passiveEffect.Modifier)
								{
									item.PassiveEffects.RemoveAt(k);
								}
							}
						}
					}
				}
			}
		}
		this.EffectGroups.Insert(_order, item);
		this.PassivesIndex.UnionWith(item.PassivesIndices);
	}

	// Token: 0x06003059 RID: 12377 RVA: 0x001494D0 File Offset: 0x001476D0
	public static MinEffectController ParseXml(XElement _element, XElement _elementToExtend = null, MinEffectController.SourceParentType _type = MinEffectController.SourceParentType.None, object _parentPointer = null)
	{
		bool flag = false;
		MinEffectController minEffectController = new MinEffectController();
		minEffectController.EffectGroups = new List<MinEffectGroup>();
		minEffectController.PassivesIndex = new HashSet<PassiveEffects>(EffectManager.PassiveEffectsComparer);
		minEffectController.ParentType = _type;
		minEffectController.ParentPointer = _parentPointer;
		int num = 0;
		foreach (XElement element in _element.Elements("effect_group"))
		{
			flag = true;
			minEffectController.AddEffectGroup(MinEffectGroup.ParseXml(element), num++, false);
		}
		if (_elementToExtend != null)
		{
			flag = true;
			XElement xelement = _elementToExtend;
			while (xelement != null)
			{
				num = 0;
				foreach (XElement element2 in xelement.Elements("effect_group"))
				{
					minEffectController.AddEffectGroup(MinEffectGroup.ParseXml(element2), num++, true);
				}
				XAttribute xattribute = xelement.Attribute("extends");
				if (xattribute != null)
				{
					string extendName = xattribute.Value;
					xelement = _element.Document.Descendants(xelement.Name).FirstOrDefault((XElement e) => (string)e.Attribute("name") == extendName);
					if (xelement == null)
					{
						Log.Warning("Unable to find element to extend '" + extendName + "'");
					}
				}
				else
				{
					xelement = null;
				}
			}
		}
		if (!flag)
		{
			return null;
		}
		return minEffectController;
	}

	// Token: 0x040026D0 RID: 9936
	public List<MinEffectGroup> EffectGroups;

	// Token: 0x040026D1 RID: 9937
	public HashSet<PassiveEffects> PassivesIndex;

	// Token: 0x040026D2 RID: 9938
	public MinEffectController.SourceParentType ParentType;

	// Token: 0x040026D3 RID: 9939
	public object ParentPointer = -1;

	// Token: 0x02000617 RID: 1559
	public enum SourceParentType
	{
		// Token: 0x040026D5 RID: 9941
		None,
		// Token: 0x040026D6 RID: 9942
		ItemClass,
		// Token: 0x040026D7 RID: 9943
		ItemModifierClass,
		// Token: 0x040026D8 RID: 9944
		EntityClass,
		// Token: 0x040026D9 RID: 9945
		ProgressionClass,
		// Token: 0x040026DA RID: 9946
		BuffClass,
		// Token: 0x040026DB RID: 9947
		ChallengeClass,
		// Token: 0x040026DC RID: 9948
		ChallengeGroup
	}
}
