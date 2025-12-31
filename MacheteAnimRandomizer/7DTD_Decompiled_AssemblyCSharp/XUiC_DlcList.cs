using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C93 RID: 3219
[Preserve]
public class XUiC_DlcList : XUiC_List<XUiC_DlcList.DlcEntry>
{
	// Token: 0x06006356 RID: 25430 RVA: 0x00284CC8 File Offset: 0x00282EC8
	public override void RebuildList(bool _resetFilter = false)
	{
		this.allEntries.Clear();
		foreach (XUiC_DlcList.DlcEntry dlcEntry in this.dlcEntries)
		{
			if (EntitlementManager.Instance.IsAvailableOnPlatform(dlcEntry.DlcSet) && EntitlementManager.Instance.IsEntitlementPurchasable(dlcEntry.DlcSet))
			{
				this.allEntries.Add(dlcEntry);
			}
		}
		this.allEntries.Sort();
		base.RebuildList(_resetFilter);
	}

	// Token: 0x06006357 RID: 25431 RVA: 0x00284D64 File Offset: 0x00282F64
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "dlc_count")
		{
			_value = this.allEntries.Count.ToString();
			return true;
		}
		if (!(_bindingName == "paging_active"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = (this.allEntries.Count > base.PageLength).ToString();
		return true;
	}

	// Token: 0x06006358 RID: 25432 RVA: 0x00284DCB File Offset: 0x00282FCB
	public override void OnOpen()
	{
		base.OnOpen();
		this.RebuildList(false);
	}

	// Token: 0x06006359 RID: 25433 RVA: 0x00284DDA File Offset: 0x00282FDA
	public void PageUp()
	{
		base.GetChildByType<XUiC_Paging>().PageUp();
	}

	// Token: 0x0600635A RID: 25434 RVA: 0x00284DE8 File Offset: 0x00282FE8
	public void PageDown()
	{
		base.GetChildByType<XUiC_Paging>().PageDown();
	}

	// Token: 0x04004AD0 RID: 19152
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_DlcList.DlcEntry> dlcEntries = new List<XUiC_DlcList.DlcEntry>
	{
		new XUiC_DlcList.DlcEntry("armorHoarderSet", "Data/DlcInfo/banner_hoarder_outfit", EntitlementSetEnum.HoarderCosmetic),
		new XUiC_DlcList.DlcEntry("armorMarauderSet", "Data/DlcInfo/banner_marauder_outfit", EntitlementSetEnum.MarauderCosmetic),
		new XUiC_DlcList.DlcEntry("armorDesertSet", "Data/DlcInfo/banner_desert_outfit", EntitlementSetEnum.DesertCosmetic)
	};

	// Token: 0x02000C94 RID: 3220
	[Preserve]
	public class DlcEntry : XUiListEntry<XUiC_DlcList.DlcEntry>
	{
		// Token: 0x0600635C RID: 25436 RVA: 0x00284E58 File Offset: 0x00283058
		public DlcEntry(string _nameKey, string _imageResourceUri, EntitlementSetEnum _dlcSet)
		{
			this.Name = Localization.Get(_nameKey, false);
			this.ImageResourceUri = _imageResourceUri;
			this.DlcSet = _dlcSet;
		}

		// Token: 0x0600635D RID: 25437 RVA: 0x00284E7B File Offset: 0x0028307B
		public override int CompareTo(XUiC_DlcList.DlcEntry _otherEntry)
		{
			if (_otherEntry == null)
			{
				return -1;
			}
			return string.Compare(this.Name, _otherEntry.Name, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x0600635E RID: 25438 RVA: 0x00284E94 File Offset: 0x00283094
		public override bool GetBindingValue(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = (this.Name ?? "");
				return true;
			}
			if (_bindingName == "texture_uri")
			{
				_value = (this.ImageResourceUri ?? "");
				return true;
			}
			if (!(_bindingName == "owned"))
			{
				return false;
			}
			_value = EntitlementManager.Instance.HasEntitlement(this.DlcSet).ToString();
			return true;
		}

		// Token: 0x0600635F RID: 25439 RVA: 0x000197A5 File Offset: 0x000179A5
		public override bool MatchesSearch(string _searchString)
		{
			return true;
		}

		// Token: 0x06006360 RID: 25440 RVA: 0x00284F10 File Offset: 0x00283110
		[Preserve]
		public static bool GetNullBindingValues(ref string _value, string _bindingName)
		{
			if (_bindingName == "name")
			{
				_value = string.Empty;
				return true;
			}
			if (_bindingName == "texture_uri")
			{
				_value = string.Empty;
				return true;
			}
			if (!(_bindingName == "owned"))
			{
				return false;
			}
			_value = "False";
			return true;
		}

		// Token: 0x04004AD1 RID: 19153
		public readonly string Name;

		// Token: 0x04004AD2 RID: 19154
		public readonly string ImageResourceUri;

		// Token: 0x04004AD3 RID: 19155
		public readonly EntitlementSetEnum DlcSet;
	}

	// Token: 0x02000C95 RID: 3221
	[Preserve]
	public class DlcListEntryController : XUiC_ListEntry<XUiC_DlcList.DlcEntry>
	{
		// Token: 0x06006361 RID: 25441 RVA: 0x00284F64 File Offset: 0x00283164
		public override void Init()
		{
			base.Init();
			XUiController childById = base.GetChildById("dlcImage");
			XUiV_Texture xuiV_Texture = ((childById != null) ? childById.ViewComponent : null) as XUiV_Texture;
			if (xuiV_Texture != null)
			{
				this.textureScaler = xuiV_Texture.UiTransform.gameObject.AddComponent<TweenScale>();
				this.textureScaler.enabled = false;
				this.textureScaler.from = Vector3.one;
				this.textureScaler.to = Vector3.one * 1.15f;
				this.textureScaler.duration = 0.25f;
				this.textureScaler.ResetToBeginning();
			}
		}

		// Token: 0x06006362 RID: 25442 RVA: 0x00284FFE File Offset: 0x002831FE
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnHovered(bool _isOver)
		{
			base.OnHovered(_isOver);
			if (this.textureScaler == null)
			{
				return;
			}
			if (_isOver)
			{
				this.textureScaler.PlayForward();
				return;
			}
			this.textureScaler.PlayReverse();
		}

		// Token: 0x06006363 RID: 25443 RVA: 0x00285030 File Offset: 0x00283230
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnPressed(int _mouseButton)
		{
			base.OnPressed(_mouseButton);
			XUiC_DlcList.DlcEntry entry = base.GetEntry();
			if (entry == null)
			{
				return;
			}
			EntitlementManager.Instance.OpenStore(entry.DlcSet, delegate(EntitlementSetEnum _)
			{
				this.List.RebuildList(false);
			});
		}

		// Token: 0x04004AD4 RID: 19156
		[PublicizedFrom(EAccessModifier.Private)]
		public const float hoverDuration = 0.25f;

		// Token: 0x04004AD5 RID: 19157
		[PublicizedFrom(EAccessModifier.Private)]
		public const float hoverScale = 1.15f;

		// Token: 0x04004AD6 RID: 19158
		[PublicizedFrom(EAccessModifier.Private)]
		public TweenScale textureScaler;
	}
}
