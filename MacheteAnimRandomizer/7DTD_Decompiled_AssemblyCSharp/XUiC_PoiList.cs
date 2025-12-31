using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D77 RID: 3447
[Preserve]
public class XUiC_PoiList : XUiC_List<XUiC_PoiList.PoiListEntry>
{
	// Token: 0x17000AD7 RID: 2775
	// (get) Token: 0x06006BC3 RID: 27587 RVA: 0x002C2160 File Offset: 0x002C0360
	// (set) Token: 0x06006BC4 RID: 27588 RVA: 0x002C2168 File Offset: 0x002C0368
	public bool FilterSmallPois
	{
		get
		{
			return this.filterSmallPois;
		}
		set
		{
			if (this.filterSmallPois != value)
			{
				this.filterSmallPois = value;
				this.RebuildList(false);
			}
		}
	}

	// Token: 0x17000AD8 RID: 2776
	// (get) Token: 0x06006BC5 RID: 27589 RVA: 0x002C2181 File Offset: 0x002C0381
	// (set) Token: 0x06006BC6 RID: 27590 RVA: 0x002C2189 File Offset: 0x002C0389
	public int FilterTier
	{
		get
		{
			return this.filterTier;
		}
		set
		{
			if (this.filterTier != value)
			{
				this.filterTier = value;
				this.RebuildList(false);
			}
		}
	}

	// Token: 0x17000AD9 RID: 2777
	// (get) Token: 0x06006BC7 RID: 27591 RVA: 0x002C21A2 File Offset: 0x002C03A2
	// (set) Token: 0x06006BC8 RID: 27592 RVA: 0x002C21AA File Offset: 0x002C03AA
	public int MinTier { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000ADA RID: 2778
	// (get) Token: 0x06006BC9 RID: 27593 RVA: 0x002C21B3 File Offset: 0x002C03B3
	// (set) Token: 0x06006BCA RID: 27594 RVA: 0x002C21BB File Offset: 0x002C03BB
	public int MaxTier { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06006BCB RID: 27595 RVA: 0x002C21C4 File Offset: 0x002C03C4
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		if (GameManager.Instance != null)
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
			List<PrefabInstance> list = (dynamicPrefabDecorator != null) ? dynamicPrefabDecorator.GetDynamicPrefabs() : null;
			if (list != null)
			{
				foreach (PrefabInstance prefabInstance in list)
				{
					if (!this.openedBefore)
					{
						this.MinTier = Mathf.Min((int)prefabInstance.prefab.DifficultyTier, this.MinTier);
						this.MaxTier = Mathf.Max((int)prefabInstance.prefab.DifficultyTier, this.MaxTier);
					}
					if ((!this.filterSmallPois || prefabInstance.boundingBoxSize.Volume() >= 100) && (this.filterTier < 0 || (int)prefabInstance.prefab.DifficultyTier == this.filterTier))
					{
						this.allEntries.Add(new XUiC_PoiList.PoiListEntry(prefabInstance));
					}
				}
			}
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x06006BCC RID: 27596 RVA: 0x002C22E0 File Offset: 0x002C04E0
	public override void OnOpen()
	{
		base.OnOpen();
		if (!this.openedBefore || this.allEntries.Count == 0)
		{
			this.RebuildList(false);
		}
		this.openedBefore = true;
	}

	// Token: 0x0400520F RID: 21007
	[PublicizedFrom(EAccessModifier.Private)]
	public const int SmallPoiVolumeLimit = 100;

	// Token: 0x04005210 RID: 21008
	[PublicizedFrom(EAccessModifier.Private)]
	public bool filterSmallPois = true;

	// Token: 0x04005211 RID: 21009
	[PublicizedFrom(EAccessModifier.Private)]
	public int filterTier = -1;

	// Token: 0x04005212 RID: 21010
	[PublicizedFrom(EAccessModifier.Private)]
	public bool openedBefore;

	// Token: 0x02000D78 RID: 3448
	[Preserve]
	public class PoiListEntry : XUiListEntry<XUiC_PoiList.PoiListEntry>
	{
		// Token: 0x06006BCE RID: 27598 RVA: 0x002C2321 File Offset: 0x002C0521
		public PoiListEntry(PrefabInstance _prefabInstance)
		{
			this.prefabInstance = _prefabInstance;
		}

		// Token: 0x06006BCF RID: 27599 RVA: 0x002C2330 File Offset: 0x002C0530
		public override int CompareTo(XUiC_PoiList.PoiListEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return 1;
			}
			int num = string.Compare(this.prefabInstance.name, _otherEntry.prefabInstance.name, StringComparison.OrdinalIgnoreCase);
			if (num != 0)
			{
				return num;
			}
			num = this.prefabInstance.boundingBoxPosition.x - _otherEntry.prefabInstance.boundingBoxPosition.x;
			if (num != 0)
			{
				return num;
			}
			return this.prefabInstance.boundingBoxPosition.z - _otherEntry.prefabInstance.boundingBoxPosition.z;
		}

		// Token: 0x06006BD0 RID: 27600 RVA: 0x002C23AC File Offset: 0x002C05AC
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = this.prefabInstance.prefab.PrefabName;
				return true;
			}
			if (_bindingName == "localizedname")
			{
				_value = this.prefabInstance.prefab.LocalizedName;
				return true;
			}
			if (!(_bindingName == "coords"))
			{
				return false;
			}
			Vector3i boundingBoxPosition = this.prefabInstance.boundingBoxPosition;
			BiomeDefinition biomeInWorld = GameManager.Instance.World.GetBiomeInWorld(boundingBoxPosition.x, boundingBoxPosition.z);
			_value = "(" + ValueDisplayFormatters.WorldPos(boundingBoxPosition.ToVector3(), " ", false) + ")\n" + ((biomeInWorld != null) ? biomeInWorld.LocalizedName : null);
			return true;
		}

		// Token: 0x06006BD1 RID: 27601 RVA: 0x002C2468 File Offset: 0x002C0668
		public override bool MatchesSearch(string _searchString)
		{
			Prefab prefab = this.prefabInstance.prefab;
			if (prefab.PrefabName.ContainsCaseInsensitive(_searchString) || prefab.LocalizedName.ContainsCaseInsensitive(_searchString))
			{
				return true;
			}
			Vector3i boundingBoxPosition = this.prefabInstance.boundingBoxPosition;
			BiomeDefinition biomeInWorld = GameManager.Instance.World.GetBiomeInWorld(boundingBoxPosition.x, boundingBoxPosition.z);
			return biomeInWorld != null && biomeInWorld.LocalizedName.ContainsCaseInsensitive(_searchString);
		}

		// Token: 0x06006BD2 RID: 27602 RVA: 0x002C24D8 File Offset: 0x002C06D8
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = string.Empty;
				return true;
			}
			if (_bindingName == "localizedname")
			{
				_value = string.Empty;
				return true;
			}
			if (!(_bindingName == "coords"))
			{
				return false;
			}
			_value = string.Empty;
			return true;
		}

		// Token: 0x04005215 RID: 21013
		public readonly PrefabInstance prefabInstance;
	}
}
