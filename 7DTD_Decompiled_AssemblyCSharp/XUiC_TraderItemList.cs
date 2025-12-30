using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E78 RID: 3704
[Preserve]
public class XUiC_TraderItemList : XUiController
{
	// Token: 0x17000BD7 RID: 3031
	// (get) Token: 0x06007462 RID: 29794 RVA: 0x002F4F5A File Offset: 0x002F315A
	// (set) Token: 0x06007463 RID: 29795 RVA: 0x002F4F62 File Offset: 0x002F3162
	public ItemStack CurrentItem { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000BD8 RID: 3032
	// (get) Token: 0x06007464 RID: 29796 RVA: 0x002F4F6B File Offset: 0x002F316B
	// (set) Token: 0x06007465 RID: 29797 RVA: 0x002F4F73 File Offset: 0x002F3173
	public int Page
	{
		get
		{
			return this.page;
		}
		set
		{
			this.page = value;
		}
	}

	// Token: 0x17000BD9 RID: 3033
	// (get) Token: 0x06007466 RID: 29798 RVA: 0x002F4F7C File Offset: 0x002F317C
	// (set) Token: 0x06007467 RID: 29799 RVA: 0x002F4F84 File Offset: 0x002F3184
	public XUiC_ItemInfoWindow InfoWindow { get; set; }

	// Token: 0x17000BDA RID: 3034
	// (get) Token: 0x06007468 RID: 29800 RVA: 0x002F4F8D File Offset: 0x002F318D
	// (set) Token: 0x06007469 RID: 29801 RVA: 0x002F4F98 File Offset: 0x002F3198
	public XUiC_TraderItemEntry SelectedEntry
	{
		get
		{
			return this.selectedEntry;
		}
		set
		{
			if (this.selectedEntry != null)
			{
				this.selectedEntry.Selected = false;
			}
			this.selectedEntry = value;
			if (this.selectedEntry != null)
			{
				this.selectedEntry.Selected = true;
				for (int i = 0; i < this.indexList.Count; i++)
				{
					if (this.selectedEntry.SlotIndex == this.indexList[i])
					{
						this.selectedIndex = i % this.entryList.Count;
						break;
					}
				}
				this.InfoWindow.ViewComponent.IsVisible = true;
				this.InfoWindow.SetItemStack(this.selectedEntry, false);
				this.CurrentItem = this.selectedEntry.Item;
				return;
			}
			if (this.selectedIndex >= 0)
			{
				if (this.entryList[this.selectedIndex].Item != null)
				{
					this.SelectedEntry = this.entryList[this.selectedIndex];
				}
				else if (this.entryList[0].Item != null)
				{
					this.SelectedEntry = this.entryList[0];
				}
			}
			if (this.SelectedEntry == null)
			{
				this.selectedIndex = -1;
				this.InfoWindow.SetItemStack(null, false);
				this.CurrentItem = null;
			}
		}
	}

	// Token: 0x0600746A RID: 29802 RVA: 0x002F50D4 File Offset: 0x002F32D4
	public override void Init()
	{
		base.Init();
		for (int i = 0; i < this.children.Count; i++)
		{
			XUiController xuiController = this.children[i];
			if (xuiController is XUiC_TraderItemEntry)
			{
				this.entryList.Add((XUiC_TraderItemEntry)xuiController);
			}
		}
		XUiV_Grid xuiV_Grid = (XUiV_Grid)base.ViewComponent;
		if (xuiV_Grid != null)
		{
			this.Length = xuiV_Grid.Columns * xuiV_Grid.Rows;
		}
		this.InfoWindow = base.xui.GetChildByType<XUiC_ItemInfoWindow>();
	}

	// Token: 0x0600746B RID: 29803 RVA: 0x002F5158 File Offset: 0x002F3358
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPressEntry(XUiController _sender, int _mouseButton)
	{
		XUiC_TraderItemEntry xuiC_TraderItemEntry = _sender as XUiC_TraderItemEntry;
		if (xuiC_TraderItemEntry != null)
		{
			this.SelectedEntry = xuiC_TraderItemEntry;
			if (InputUtils.ShiftKeyPressed)
			{
				xuiC_TraderItemEntry.InfoWindow.BuySellCounter.SetToMaxCount();
				return;
			}
			xuiC_TraderItemEntry.InfoWindow.BuySellCounter.Count = xuiC_TraderItemEntry.Item.itemValue.ItemClass.EconomicBundleSize;
		}
	}

	// Token: 0x0600746C RID: 29804 RVA: 0x002F51B3 File Offset: 0x002F33B3
	public override void OnOpen()
	{
		if (base.ViewComponent != null && !base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = true;
		}
		this.ClearSelection();
		this.IsDirty = true;
	}

	// Token: 0x0600746D RID: 29805 RVA: 0x002F51E3 File Offset: 0x002F33E3
	public override void OnClose()
	{
		if (base.ViewComponent != null && base.ViewComponent.IsVisible)
		{
			base.ViewComponent.IsVisible = false;
		}
		this.ClearSelection();
	}

	// Token: 0x0600746E RID: 29806 RVA: 0x002F520C File Offset: 0x002F340C
	public void ClearSelection()
	{
		this.CurrentItem = null;
		this.selectedIndex = -1;
		this.SelectedEntry = null;
		this.InfoWindow.SetItemStack(null, false);
	}

	// Token: 0x0600746F RID: 29807 RVA: 0x002F5230 File Offset: 0x002F3430
	public void SetItems(ItemStack[] _stackList, List<int> _indexList)
	{
		if (_stackList == null)
		{
			return;
		}
		this.items.Clear();
		this.items.AddRange(_stackList);
		this.indexList.Clear();
		this.indexList.AddRange(_indexList);
		XUiC_ItemInfoWindow childByType = base.xui.GetChildByType<XUiC_ItemInfoWindow>();
		for (int i = 0; i < this.Length; i++)
		{
			int num = i + this.Length * this.page;
			this.entryList[i].OnPress -= this.OnPressEntry;
			this.entryList[i].InfoWindow = childByType;
			if (num < this.items.Count)
			{
				this.entryList[i].SlotIndex = _indexList[num];
				this.entryList[i].Item = _stackList[num];
				this.entryList[i].OnPress += this.OnPressEntry;
				this.entryList[i].ViewComponent.SoundPlayOnClick = true;
			}
			else
			{
				this.entryList[i].Item = null;
				this.entryList[i].ViewComponent.SoundPlayOnClick = false;
			}
		}
		if (this.CurrentItem != null)
		{
			ItemValue itemValue = this.CurrentItem.itemValue;
			XUiC_TraderItemEntry xuiC_TraderItemEntry = this.SelectedEntry;
			ItemValue other;
			if (xuiC_TraderItemEntry == null)
			{
				other = null;
			}
			else
			{
				ItemStack item = xuiC_TraderItemEntry.Item;
				other = ((item != null) ? item.itemValue : null);
			}
			if (itemValue.Equals(other))
			{
				return;
			}
		}
		this.ClearSelection();
	}

	// Token: 0x04005885 RID: 22661
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TraderItemEntry selectedEntry;

	// Token: 0x04005886 RID: 22662
	[PublicizedFrom(EAccessModifier.Private)]
	public int selectedIndex = -1;

	// Token: 0x04005887 RID: 22663
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ItemStack> items = new List<ItemStack>();

	// Token: 0x04005888 RID: 22664
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> indexList = new List<int>();

	// Token: 0x04005889 RID: 22665
	[PublicizedFrom(EAccessModifier.Private)]
	public int page;

	// Token: 0x0400588A RID: 22666
	public int Length;

	// Token: 0x0400588B RID: 22667
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showFavorites;

	// Token: 0x0400588C RID: 22668
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 selectedColor = new Color32(222, 206, 163, byte.MaxValue);

	// Token: 0x0400588D RID: 22669
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_TextInput txtInput;

	// Token: 0x0400588E RID: 22670
	[PublicizedFrom(EAccessModifier.Private)]
	public string category;

	// Token: 0x0400588F RID: 22671
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_TraderItemEntry> entryList = new List<XUiC_TraderItemEntry>();
}
