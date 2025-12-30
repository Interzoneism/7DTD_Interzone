using System;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C41 RID: 3137
[Preserve]
public class XUiC_CollectedItemList : XUiController
{
	// Token: 0x06006060 RID: 24672 RVA: 0x002723B4 File Offset: 0x002705B4
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("item");
		this.PrefabItems = childById.ViewComponent.UiTransform;
		this.height = (float)(childById.ViewComponent.Size.y + 2);
		childById.xui.CollectedItemList = this;
	}

	// Token: 0x06006061 RID: 24673 RVA: 0x00272409 File Offset: 0x00270609
	public void AddRemoveItemQueueEntry(ItemStack stack)
	{
		this.removeItemQueue.Add(stack);
	}

	// Token: 0x06006062 RID: 24674 RVA: 0x00272418 File Offset: 0x00270618
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.localPlayer == null)
		{
			this.localPlayer = base.xui.playerUI.entityPlayer;
		}
		base.ViewComponent.IsVisible = (!this.localPlayer.IsDead() && base.xui.playerUI.windowManager.IsHUDEnabled());
		if (this.removeItemQueue.Count > 0)
		{
			for (int i = 0; i < this.removeItemQueue.Count; i++)
			{
				this.RemoveItemStack(this.removeItemQueue[i]);
			}
			this.removeItemQueue.Clear();
		}
		if (this.items.Count > 0)
		{
			float time = Time.time;
			for (int j = 0; j < this.items.Count; j++)
			{
				if (time - this.items[j].TimeAdded > 5f)
				{
					this.removeLastEntry(j);
					j--;
				}
			}
		}
	}

	// Token: 0x06006063 RID: 24675 RVA: 0x00272510 File Offset: 0x00270710
	public override void OnOpen()
	{
		base.OnOpen();
		this.PrefabItems.gameObject.SetActive(false);
	}

	// Token: 0x06006064 RID: 24676 RVA: 0x0027252C File Offset: 0x0027072C
	public void AddIconNotification(string iconNotifier, int count = 1, bool _bAddOnlyIfNotExisting = false)
	{
		if (iconNotifier == string.Empty)
		{
			return;
		}
		if (this.items == null)
		{
			return;
		}
		if (count == 0)
		{
			return;
		}
		if (_bAddOnlyIfNotExisting && this.items.Count > 0 && this.items[this.items.Count - 1].uiAtlasIcon.EqualsCaseInsensitive(iconNotifier))
		{
			return;
		}
		Transform transform;
		UILabel uilabel;
		for (int i = this.items.Count - 1; i >= 0; i--)
		{
			if (this.items[i].uiAtlasIcon != string.Empty && this.items[i].uiAtlasIcon.EqualsCaseInsensitive(iconNotifier))
			{
				this.items[i].count += count;
				transform = this.items[i].Item.transform;
				if (transform != null)
				{
					uilabel = transform.GetComponentInChildren<UILabel>();
					if (uilabel != null)
					{
						uilabel.text = ((this.items[i].count > 1) ? ("+" + this.items[i].count.ToString()) : "");
					}
				}
				this.items[i].TimeAdded = Time.time;
				return;
			}
		}
		GameObject gameObject = base.ViewComponent.UiTransform.gameObject.AddChild(this.PrefabItems.gameObject);
		if (gameObject == null)
		{
			return;
		}
		gameObject.SetActive(true);
		transform = gameObject.transform.Find("Negative");
		if (transform != null)
		{
			transform.gameObject.SetActive(false);
		}
		uilabel = gameObject.transform.GetComponentInChildren<UILabel>();
		if (uilabel == null)
		{
			uilabel = gameObject.transform.GetComponent<UILabel>();
		}
		if (uilabel != null)
		{
			uilabel.text = ((count > 0) ? ("+" + count.ToString()) : (count.ToString() ?? ""));
		}
		UISprite component = gameObject.transform.Find("Icon").GetComponent<UISprite>();
		if (component != null)
		{
			component.atlas = base.xui.GetAtlasByName("UIAtlas", iconNotifier);
			component.spriteName = iconNotifier;
			component.color = Color.white;
		}
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, (float)this.items.Count * this.height + (float)this.yOffset, gameObject.transform.localPosition.z);
		XUiC_CollectedItemList.Data data = new XUiC_CollectedItemList.Data();
		data.Item = gameObject;
		data.TimeAdded = Time.time;
		data.ItemStack = null;
		data.count = count;
		data.uiAtlasIcon = iconNotifier;
		this.items.Add(data);
		if (this.items.Count > 12)
		{
			this.removeLastEntry(0);
		}
	}

	// Token: 0x06006065 RID: 24677 RVA: 0x0027282C File Offset: 0x00270A2C
	public void AddCraftingSkillNotification(ProgressionValue craftingSkill, bool _bAddOnlyIfNotExisting = false)
	{
		if (craftingSkill == null)
		{
			return;
		}
		if (this.items == null)
		{
			return;
		}
		if (_bAddOnlyIfNotExisting && this.items.Count > 0 && this.items[this.items.Count - 1].CraftingSkill == craftingSkill)
		{
			return;
		}
		Transform transform;
		UILabel uilabel;
		for (int i = this.items.Count - 1; i >= 0; i--)
		{
			if (this.items[i].CraftingSkill != null && this.items[i].CraftingSkill == craftingSkill)
			{
				transform = this.items[i].Item.transform;
				if (transform != null)
				{
					uilabel = transform.GetComponentInChildren<UILabel>();
					if (uilabel != null)
					{
						uilabel.text = string.Format("{0}/{1}", this.items[i].CraftingSkill.Level, this.items[i].CraftingSkill.ProgressionClass.MaxLevel);
					}
				}
				this.items[i].TimeAdded = Time.time;
				return;
			}
		}
		GameObject gameObject = base.ViewComponent.UiTransform.gameObject.AddChild(this.PrefabItems.gameObject);
		if (gameObject == null)
		{
			return;
		}
		gameObject.SetActive(true);
		transform = gameObject.transform.Find("Negative");
		if (transform != null)
		{
			transform.gameObject.SetActive(false);
		}
		uilabel = gameObject.transform.GetComponentInChildren<UILabel>();
		if (uilabel == null)
		{
			uilabel = gameObject.transform.GetComponent<UILabel>();
		}
		if (uilabel != null)
		{
			uilabel.text = string.Format("{0}/{1}", craftingSkill.Level, craftingSkill.ProgressionClass.MaxLevel);
		}
		UISprite component = gameObject.transform.Find("Icon").GetComponent<UISprite>();
		if (component != null)
		{
			component.atlas = base.xui.GetAtlasByName("UIAtlas", craftingSkill.ProgressionClass.Icon);
			component.spriteName = craftingSkill.ProgressionClass.Icon;
			component.color = Color.white;
		}
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, (float)this.items.Count * this.height + (float)this.yOffset, gameObject.transform.localPosition.z);
		XUiC_CollectedItemList.Data data = new XUiC_CollectedItemList.Data();
		data.Item = gameObject;
		data.TimeAdded = Time.time;
		data.ItemStack = null;
		data.count = -1;
		data.CraftingSkill = craftingSkill;
		data.uiAtlasIcon = craftingSkill.ProgressionClass.Icon;
		this.items.Add(data);
		if (this.items.Count > 12)
		{
			this.removeLastEntry(0);
		}
	}

	// Token: 0x06006066 RID: 24678 RVA: 0x00272B1C File Offset: 0x00270D1C
	public void AddItemStack(ItemStack _is, bool _bAddOnlyIfNotExisting = false)
	{
		if (_is == null)
		{
			return;
		}
		if (_is.itemValue == null)
		{
			return;
		}
		if (this.items == null)
		{
			return;
		}
		if (_is.itemValue.type == 0)
		{
			return;
		}
		if (_is.count == 0)
		{
			Manager.PlayInsidePlayerHead("missingitemtorepair", -1, 0f, false, false);
		}
		if (_bAddOnlyIfNotExisting && this.items.Count > 0 && this.items[this.items.Count - 1].ItemStack != null && this.items[this.items.Count - 1].ItemStack.itemValue.type == _is.itemValue.type && this.items[this.items.Count - 1].ItemStack.count == _is.count)
		{
			return;
		}
		bool flag = _is.count < 0;
		for (int i = this.items.Count - 1; i >= 0; i--)
		{
			if (this.items[i].ItemStack != null && this.items[i].ItemStack.itemValue.type == _is.itemValue.type)
			{
				if (this.items[i].isNegative == flag)
				{
					if (_is.count != 0 && !this.items[i].isMissingNotifier)
					{
						this.items[i].ItemStack.count += _is.count;
						Transform transform = this.items[i].Item.transform;
						if (transform != null)
						{
							UILabel componentInChildren = transform.GetComponentInChildren<UILabel>();
							if (componentInChildren != null)
							{
								componentInChildren.text = string.Format("{0} ({1})", (this.items[i].ItemStack.count >= 1) ? ("+" + this.items[i].ItemStack.count.ToString()) : this.items[i].ItemStack.count.ToString(), base.xui.PlayerInventory.GetItemCountWithMods(this.items[i].ItemStack.itemValue));
							}
						}
						this.items[i].TimeAdded = Time.time;
						return;
					}
					if (_is.count == 0 && this.items[i].isMissingNotifier)
					{
						this.items[i].TimeAdded = Time.time;
						return;
					}
				}
				else
				{
					this.items[i].TimeAdded = 0f;
				}
			}
		}
		GameObject gameObject = base.ViewComponent.UiTransform.gameObject.AddChild(this.PrefabItems.gameObject);
		if (gameObject == null)
		{
			return;
		}
		gameObject.SetActive(true);
		if (_is.count == 0)
		{
			Transform transform2 = gameObject.transform.Find("Negative");
			if (transform2 != null)
			{
				transform2.gameObject.SetActive(true);
			}
			UILabel uilabel = gameObject.transform.GetComponentInChildren<UILabel>();
			if (uilabel == null)
			{
				uilabel = gameObject.transform.GetComponent<UILabel>();
			}
			if (uilabel != null)
			{
				uilabel.text = "";
			}
		}
		else
		{
			Transform transform3 = gameObject.transform.Find("Negative");
			if (transform3 != null)
			{
				transform3.gameObject.SetActive(false);
			}
			UILabel uilabel2 = gameObject.transform.GetComponentInChildren<UILabel>();
			if (uilabel2 == null)
			{
				uilabel2 = gameObject.transform.GetComponent<UILabel>();
			}
			if (uilabel2 != null)
			{
				uilabel2.text = string.Format("{0} ({1})", (_is.count >= 1) ? ("+" + _is.count.ToString()) : _is.count.ToString(), base.xui.PlayerInventory.GetItemCountWithMods(_is.itemValue));
			}
		}
		UISprite component = gameObject.transform.Find("Icon").GetComponent<UISprite>();
		ItemClass itemClass = _is.itemValue.ItemClass;
		if (component != null && itemClass != null)
		{
			string spriteName = _is.itemValue.GetPropertyOverride("CustomIcon", itemClass.GetIconName());
			spriteName = itemClass.GetIconName();
			component.atlas = base.xui.GetAtlasByName("itemIconAtlas", spriteName);
			component.spriteName = spriteName;
			component.color = itemClass.GetIconTint(_is.itemValue);
		}
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, (float)this.items.Count * this.height + (float)this.yOffset, gameObject.transform.localPosition.z);
		XUiC_CollectedItemList.Data data = new XUiC_CollectedItemList.Data();
		data.Item = gameObject;
		data.TimeAdded = Time.time;
		data.ItemStack = _is;
		data.uiAtlasIcon = string.Empty;
		data.isMissingNotifier = (_is.count == 0);
		data.isNegative = flag;
		this.items.Add(data);
		if (this.items.Count > 12)
		{
			this.removeLastEntry(0);
		}
	}

	// Token: 0x06006067 RID: 24679 RVA: 0x00273088 File Offset: 0x00271288
	public void RemoveItemStack(ItemStack _is)
	{
		if (_is == null || _is.IsEmpty())
		{
			return;
		}
		_is = _is.Clone();
		if (_is.count > 0)
		{
			_is.count *= -1;
		}
		for (int i = this.items.Count - 1; i >= 0; i--)
		{
			if (this.items[i].ItemStack != null && this.items[i].ItemStack.itemValue.type == _is.itemValue.type)
			{
				if (this.items[i].isNegative)
				{
					if (_is.count != 0 && !this.items[i].isMissingNotifier)
					{
						this.items[i].ItemStack.count += _is.count;
						Transform transform = this.items[i].Item.transform;
						if (transform != null)
						{
							UILabel componentInChildren = transform.GetComponentInChildren<UILabel>();
							if (componentInChildren != null)
							{
								componentInChildren.text = string.Format("{0} ({1})", (this.items[i].ItemStack.count >= 1) ? ("+" + this.items[i].ItemStack.count.ToString()) : this.items[i].ItemStack.count.ToString(), base.xui.PlayerInventory.GetItemCountWithMods(this.items[i].ItemStack.itemValue));
							}
						}
						this.items[i].TimeAdded = Time.time;
						return;
					}
					if (_is.count == 0 && this.items[i].isMissingNotifier)
					{
						this.items[i].TimeAdded = Time.time;
						return;
					}
				}
				else
				{
					this.items[i].TimeAdded = 0f;
				}
			}
		}
		GameObject gameObject = base.ViewComponent.UiTransform.gameObject.AddChild(this.PrefabItems.gameObject);
		gameObject.SetActive(true);
		UILabel uilabel = gameObject.transform.GetComponentInChildren<UILabel>();
		if (uilabel == null)
		{
			uilabel = gameObject.transform.GetComponent<UILabel>();
		}
		uilabel.text = string.Format("{0} ({1})", (_is.count >= 0) ? ("-" + _is.count.ToString()) : _is.count.ToString(), base.xui.PlayerInventory.GetItemCountWithMods(_is.itemValue));
		gameObject.transform.Find("Negative").gameObject.SetActive(false);
		UISprite component = gameObject.transform.Find("Icon").GetComponent<UISprite>();
		ItemClass itemClassOrMissing = _is.itemValue.ItemClassOrMissing;
		string propertyOverride = _is.itemValue.GetPropertyOverride("CustomIcon", (itemClassOrMissing.CustomIcon == null) ? itemClassOrMissing.GetIconName() : itemClassOrMissing.CustomIcon.Value);
		component.atlas = base.xui.GetAtlasByName(((UnityEngine.Object)component.atlas).name, propertyOverride);
		component.spriteName = propertyOverride;
		component.color = itemClassOrMissing.GetIconTint(_is.itemValue);
		gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, (float)this.items.Count * this.height + (float)this.yOffset, gameObject.transform.localPosition.z);
		XUiC_CollectedItemList.Data data = new XUiC_CollectedItemList.Data();
		data.Item = gameObject;
		data.ItemStack = _is;
		data.TimeAdded = Time.time;
		data.uiAtlasIcon = string.Empty;
		data.isNegative = true;
		this.items.Add(data);
		if (this.items.Count > 12)
		{
			this.removeLastEntry(0);
		}
	}

	// Token: 0x06006068 RID: 24680 RVA: 0x002734B0 File Offset: 0x002716B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void removeLastEntry(int index = 0)
	{
		GameObject item = this.items[index].Item;
		if (item)
		{
			item.GetOrAddComponent<TemporaryObject>().enabled = true;
			TweenColor orAddComponent = item.GetOrAddComponent<TweenColor>();
			orAddComponent.from = Color.white;
			orAddComponent.to = new Color(1f, 1f, 1f, 0f);
			orAddComponent.enabled = true;
			orAddComponent.duration = 0.4f;
			TweenPosition tweenPosition = item.GetComponent<TweenPosition>();
			if (tweenPosition)
			{
				UnityEngine.Object.Destroy(tweenPosition);
			}
			tweenPosition = item.AddComponent<TweenPosition>();
			tweenPosition.from = item.transform.localPosition;
			tweenPosition.to = new Vector3(tweenPosition.from.x + 300f, tweenPosition.from.y, tweenPosition.from.z);
			tweenPosition.enabled = true;
			tweenPosition.duration = 0.4f;
		}
		this.items.RemoveAt(index);
		this.updateEntries();
	}

	// Token: 0x06006069 RID: 24681 RVA: 0x002735AC File Offset: 0x002717AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateEntries()
	{
		float time = Time.time;
		for (int i = 0; i < this.items.Count; i++)
		{
			XUiC_CollectedItemList.Data data = this.items[i];
			if (time - data.TimeAdded <= 5f)
			{
				GameObject item = data.Item;
				TweenPosition tweenPosition = item.GetComponent<TweenPosition>();
				if (tweenPosition)
				{
					UnityEngine.Object.Destroy(tweenPosition);
				}
				tweenPosition = item.AddComponent<TweenPosition>();
				tweenPosition.from = item.transform.localPosition;
				Vector3 from = tweenPosition.from;
				from.y = (float)i * this.height + (float)this.yOffset;
				tweenPosition.to = from;
				tweenPosition.enabled = true;
			}
		}
	}

	// Token: 0x0600606A RID: 24682 RVA: 0x00273662 File Offset: 0x00271862
	public void SetYOffset(int _yOffset)
	{
		if (_yOffset != this.yOffset)
		{
			this.yOffset = _yOffset;
			this.updateEntries();
		}
	}

	// Token: 0x040048C7 RID: 18631
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer localPlayer;

	// Token: 0x040048C8 RID: 18632
	[PublicizedFrom(EAccessModifier.Private)]
	public float height;

	// Token: 0x040048C9 RID: 18633
	[PublicizedFrom(EAccessModifier.Private)]
	public int yOffset;

	// Token: 0x040048CA RID: 18634
	public Transform PrefabItems;

	// Token: 0x040048CB RID: 18635
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cItemMax = 12;

	// Token: 0x040048CC RID: 18636
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_CollectedItemList.Data> items = new List<XUiC_CollectedItemList.Data>();

	// Token: 0x040048CD RID: 18637
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ItemStack> removeItemQueue = new List<ItemStack>();

	// Token: 0x02000C42 RID: 3138
	[PublicizedFrom(EAccessModifier.Private)]
	public class Data
	{
		// Token: 0x040048CE RID: 18638
		public GameObject Item;

		// Token: 0x040048CF RID: 18639
		public float TimeAdded;

		// Token: 0x040048D0 RID: 18640
		public ItemStack ItemStack;

		// Token: 0x040048D1 RID: 18641
		public ProgressionValue CraftingSkill;

		// Token: 0x040048D2 RID: 18642
		public string uiAtlasIcon;

		// Token: 0x040048D3 RID: 18643
		public int count;

		// Token: 0x040048D4 RID: 18644
		public bool isNegative;

		// Token: 0x040048D5 RID: 18645
		public bool isMissingNotifier;
	}
}
