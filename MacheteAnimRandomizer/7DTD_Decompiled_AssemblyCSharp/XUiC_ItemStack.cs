using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using Audio;
using InControl;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CDD RID: 3293
[UnityEngine.Scripting.Preserve]
public class XUiC_ItemStack : XUiC_SelectableEntry
{
	// Token: 0x17000A69 RID: 2665
	// (get) Token: 0x060065EF RID: 26095 RVA: 0x00295B57 File Offset: 0x00293D57
	// (set) Token: 0x060065F0 RID: 26096 RVA: 0x00295B5F File Offset: 0x00293D5F
	public bool isQuickSwap
	{
		get
		{
			return this._isQuickSwap;
		}
		set
		{
			this._isQuickSwap = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000A6A RID: 2666
	// (get) Token: 0x060065F1 RID: 26097 RVA: 0x00295B6F File Offset: 0x00293D6F
	// (set) Token: 0x060065F2 RID: 26098 RVA: 0x00295B77 File Offset: 0x00293D77
	public Color32 SelectionBorderColor
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.selectionBorderColor;
		}
		[PublicizedFrom(EAccessModifier.Protected)]
		set
		{
			if (!this.selectionBorderColor.ColorEquals(value))
			{
				this.selectionBorderColor = value;
				this.IsDirty = true;
			}
		}
	}

	// Token: 0x17000A6B RID: 2667
	// (get) Token: 0x060065F3 RID: 26099 RVA: 0x00295B95 File Offset: 0x00293D95
	// (set) Token: 0x060065F4 RID: 26100 RVA: 0x00295B9D File Offset: 0x00293D9D
	public int SlotNumber { get; set; }

	// Token: 0x17000A6C RID: 2668
	// (get) Token: 0x060065F5 RID: 26101 RVA: 0x00295BA6 File Offset: 0x00293DA6
	// (set) Token: 0x060065F6 RID: 26102 RVA: 0x00295BAE File Offset: 0x00293DAE
	public XUiC_ItemStack.StackLocationTypes StackLocation { get; set; }

	// Token: 0x140000A2 RID: 162
	// (add) Token: 0x060065F7 RID: 26103 RVA: 0x00295BB8 File Offset: 0x00293DB8
	// (remove) Token: 0x060065F8 RID: 26104 RVA: 0x00295BF0 File Offset: 0x00293DF0
	public event XUiEvent_SlotChangedEventHandler SlotChangedEvent;

	// Token: 0x140000A3 RID: 163
	// (add) Token: 0x060065F9 RID: 26105 RVA: 0x00295C28 File Offset: 0x00293E28
	// (remove) Token: 0x060065FA RID: 26106 RVA: 0x00295C60 File Offset: 0x00293E60
	public event XUiEvent_ToolLockChangeEventHandler ToolLockChangedEvent;

	// Token: 0x140000A4 RID: 164
	// (add) Token: 0x060065FB RID: 26107 RVA: 0x00295C98 File Offset: 0x00293E98
	// (remove) Token: 0x060065FC RID: 26108 RVA: 0x00295CD0 File Offset: 0x00293ED0
	public event XUiEvent_LockChangeEventHandler LockChangedEvent;

	// Token: 0x140000A5 RID: 165
	// (add) Token: 0x060065FD RID: 26109 RVA: 0x00295D08 File Offset: 0x00293F08
	// (remove) Token: 0x060065FE RID: 26110 RVA: 0x00295D40 File Offset: 0x00293F40
	public event XUiEvent_TimeIntervalElapsedEventHandler TimeIntervalElapsedEvent;

	// Token: 0x17000A6D RID: 2669
	// (get) Token: 0x060065FF RID: 26111 RVA: 0x00295D75 File Offset: 0x00293F75
	public ItemClass itemClass
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			ItemStack itemStack = this.itemStack;
			if (itemStack == null)
			{
				return null;
			}
			ItemValue itemValue = itemStack.itemValue;
			if (itemValue == null)
			{
				return null;
			}
			return itemValue.ItemClass;
		}
	}

	// Token: 0x17000A6E RID: 2670
	// (get) Token: 0x06006600 RID: 26112 RVA: 0x00295D93 File Offset: 0x00293F93
	public ItemClass itemClassOrMissing
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			ItemStack itemStack = this.itemStack;
			if (itemStack == null)
			{
				return null;
			}
			ItemValue itemValue = itemStack.itemValue;
			if (itemValue == null)
			{
				return null;
			}
			return itemValue.ItemClassOrMissing;
		}
	}

	// Token: 0x17000A6F RID: 2671
	// (get) Token: 0x06006601 RID: 26113 RVA: 0x00295DB1 File Offset: 0x00293FB1
	// (set) Token: 0x06006602 RID: 26114 RVA: 0x00295DB9 File Offset: 0x00293FB9
	public float HoverIconGrow { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000A70 RID: 2672
	// (get) Token: 0x06006603 RID: 26115 RVA: 0x00295DC2 File Offset: 0x00293FC2
	// (set) Token: 0x06006604 RID: 26116 RVA: 0x00295DCD File Offset: 0x00293FCD
	public bool AssembleLock
	{
		get
		{
			return this.stackLockType == XUiC_ItemStack.StackLockTypes.Assemble;
		}
		set
		{
			this.stackLockType = (value ? XUiC_ItemStack.StackLockTypes.Assemble : XUiC_ItemStack.StackLockTypes.None);
			base.RefreshBindings(false);
		}
	}

	// Token: 0x17000A71 RID: 2673
	// (get) Token: 0x06006605 RID: 26117 RVA: 0x00295DE3 File Offset: 0x00293FE3
	// (set) Token: 0x06006606 RID: 26118 RVA: 0x00295DEE File Offset: 0x00293FEE
	public bool QuestLock
	{
		get
		{
			return this.stackLockType == XUiC_ItemStack.StackLockTypes.Quest;
		}
		set
		{
			this.stackLockType = (value ? XUiC_ItemStack.StackLockTypes.Quest : XUiC_ItemStack.StackLockTypes.None);
			base.RefreshBindings(false);
		}
	}

	// Token: 0x17000A72 RID: 2674
	// (get) Token: 0x06006607 RID: 26119 RVA: 0x00295E04 File Offset: 0x00294004
	// (set) Token: 0x06006608 RID: 26120 RVA: 0x00295E0F File Offset: 0x0029400F
	public bool ToolLock
	{
		get
		{
			return this.stackLockType == XUiC_ItemStack.StackLockTypes.Tool;
		}
		set
		{
			this.stackLockType = (value ? XUiC_ItemStack.StackLockTypes.Tool : XUiC_ItemStack.StackLockTypes.None);
			XUiEvent_ToolLockChangeEventHandler toolLockChangedEvent = this.ToolLockChangedEvent;
			if (toolLockChangedEvent != null)
			{
				toolLockChangedEvent(this.SlotNumber, this.itemStack, value);
			}
			base.RefreshBindings(false);
		}
	}

	// Token: 0x17000A73 RID: 2675
	// (get) Token: 0x06006609 RID: 26121 RVA: 0x00295E43 File Offset: 0x00294043
	// (set) Token: 0x0600660A RID: 26122 RVA: 0x00295E4E File Offset: 0x0029404E
	public bool HiddenLock
	{
		get
		{
			return this.stackLockType == XUiC_ItemStack.StackLockTypes.Hidden;
		}
		set
		{
			this.stackLockType = (value ? XUiC_ItemStack.StackLockTypes.Hidden : XUiC_ItemStack.StackLockTypes.None);
			base.RefreshBindings(false);
		}
	}

	// Token: 0x17000A74 RID: 2676
	// (get) Token: 0x0600660B RID: 26123 RVA: 0x00295E64 File Offset: 0x00294064
	// (set) Token: 0x0600660C RID: 26124 RVA: 0x00295E6C File Offset: 0x0029406C
	public bool AttributeLock
	{
		get
		{
			return this.attributeLock;
		}
		set
		{
			this.attributeLock = value;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x17000A75 RID: 2677
	// (get) Token: 0x0600660D RID: 26125 RVA: 0x00295E7C File Offset: 0x0029407C
	public bool StackLock
	{
		get
		{
			return this.stackLockType > XUiC_ItemStack.StackLockTypes.None;
		}
	}

	// Token: 0x17000A76 RID: 2678
	// (get) Token: 0x0600660E RID: 26126 RVA: 0x00295E87 File Offset: 0x00294087
	// (set) Token: 0x0600660F RID: 26127 RVA: 0x00295E8F File Offset: 0x0029408F
	public bool IsDragAndDrop
	{
		get
		{
			return this.isDragAndDrop;
		}
		set
		{
			this.isDragAndDrop = value;
			if (!value)
			{
				return;
			}
			base.ViewComponent.EventOnPress = false;
			base.ViewComponent.EventOnHover = false;
		}
	}

	// Token: 0x17000A77 RID: 2679
	// (get) Token: 0x06006610 RID: 26128 RVA: 0x00295EB4 File Offset: 0x002940B4
	// (set) Token: 0x06006611 RID: 26129 RVA: 0x00295EBC File Offset: 0x002940BC
	public bool IsHolding { get; set; }

	// Token: 0x17000A78 RID: 2680
	// (get) Token: 0x06006612 RID: 26130 RVA: 0x00295EC5 File Offset: 0x002940C5
	// (set) Token: 0x06006613 RID: 26131 RVA: 0x00295ECD File Offset: 0x002940CD
	public bool IsLocked
	{
		get
		{
			return this.isLocked;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (value == this.isLocked)
			{
				return;
			}
			this.isLocked = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000A79 RID: 2681
	// (get) Token: 0x06006614 RID: 26132 RVA: 0x00295EE7 File Offset: 0x002940E7
	// (set) Token: 0x06006615 RID: 26133 RVA: 0x00295EEF File Offset: 0x002940EF
	public int RepairAmount { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000A7A RID: 2682
	// (get) Token: 0x06006616 RID: 26134 RVA: 0x00295EF8 File Offset: 0x002940F8
	public bool AllowIconGrow
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.itemClass != null;
		}
	}

	// Token: 0x17000A7B RID: 2683
	// (get) Token: 0x06006617 RID: 26135 RVA: 0x00295F03 File Offset: 0x00294103
	// (set) Token: 0x06006618 RID: 26136 RVA: 0x00295F0B File Offset: 0x0029410B
	public XUiC_ItemInfoWindow InfoWindow { get; set; }

	// Token: 0x17000A7C RID: 2684
	// (get) Token: 0x06006619 RID: 26137 RVA: 0x00295F14 File Offset: 0x00294114
	// (set) Token: 0x0600661A RID: 26138 RVA: 0x00295F1C File Offset: 0x0029411C
	public bool SimpleClick { get; [PublicizedFrom(EAccessModifier.Internal)] set; }

	// Token: 0x17000A7D RID: 2685
	// (get) Token: 0x0600661B RID: 26139 RVA: 0x00295F25 File Offset: 0x00294125
	// (set) Token: 0x0600661C RID: 26140 RVA: 0x00295F2D File Offset: 0x0029412D
	public bool AllowDropping { get; [PublicizedFrom(EAccessModifier.Protected)] set; } = true;

	// Token: 0x17000A7E RID: 2686
	// (get) Token: 0x0600661D RID: 26141 RVA: 0x00295F36 File Offset: 0x00294136
	// (set) Token: 0x0600661E RID: 26142 RVA: 0x00295F3E File Offset: 0x0029413E
	public bool PrefixId { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x17000A7F RID: 2687
	// (get) Token: 0x0600661F RID: 26143 RVA: 0x00295F47 File Offset: 0x00294147
	// (set) Token: 0x06006620 RID: 26144 RVA: 0x00295F4F File Offset: 0x0029414F
	public bool ShowFavorites { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000A80 RID: 2688
	// (get) Token: 0x06006621 RID: 26145 RVA: 0x00295F58 File Offset: 0x00294158
	// (set) Token: 0x06006622 RID: 26146 RVA: 0x00295F60 File Offset: 0x00294160
	public float LockTime
	{
		get
		{
			return this.lockTime;
		}
		set
		{
			this.lockTime = value;
			if (value == 0f)
			{
				this.timer.Text = "";
				this.timer.IsVisible = false;
				return;
			}
			this.timer.Text = string.Format("{0:00}:{1:00}", Mathf.Floor(this.lockTime / 60f), Mathf.Floor(this.lockTime % 60f));
			this.timer.IsVisible = true;
		}
	}

	// Token: 0x17000A81 RID: 2689
	// (get) Token: 0x06006623 RID: 26147 RVA: 0x00295FE6 File Offset: 0x002941E6
	// (set) Token: 0x06006624 RID: 26148 RVA: 0x00295FEE File Offset: 0x002941EE
	public bool UserLockedSlot
	{
		get
		{
			return this.userLockedSlot;
		}
		set
		{
			if (value == this.userLockedSlot)
			{
				return;
			}
			this.userLockedSlot = value;
			base.RefreshBindings(false);
		}
	}

	// Token: 0x06006625 RID: 26149 RVA: 0x00296008 File Offset: 0x00294208
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void setItemStack(ItemStack _stack)
	{
		this.itemStack = _stack.Clone();
	}

	// Token: 0x06006626 RID: 26150 RVA: 0x00296016 File Offset: 0x00294216
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateItemInfoWindow(XUiC_ItemStack _itemStack)
	{
		this.InfoWindow.SetItemStack(_itemStack, true);
	}

	// Token: 0x17000A82 RID: 2690
	// (get) Token: 0x06006627 RID: 26151 RVA: 0x00296025 File Offset: 0x00294225
	// (set) Token: 0x06006628 RID: 26152 RVA: 0x00296030 File Offset: 0x00294230
	public ItemStack ItemStack
	{
		get
		{
			return this.itemStack;
		}
		set
		{
			if (!this.itemStack.Equals(value))
			{
				this.setItemStack(value);
				if (this.itemStack.IsEmpty())
				{
					this.itemStack.Clear();
				}
				if (base.Selected)
				{
					this.updateItemInfoWindow(this);
				}
				this.HandleSlotChangeEvent();
				ItemClass itemClass = this.itemStack.itemValue.ItemClass;
				if (itemClass != null && (this.StackLocation == XUiC_ItemStack.StackLocationTypes.Backpack || this.StackLocation == XUiC_ItemStack.StackLocationTypes.ToolBelt))
				{
					this.QuestLock = itemClass.IsQuestItem;
				}
				if (value.IsEmpty())
				{
					this.stackLockType = XUiC_ItemStack.StackLockTypes.None;
				}
				this.updateBackgroundTexture();
				base.RefreshBindings(false);
			}
			else
			{
				if (this.ItemStack.IsEmpty() && this.backgroundTexture != null)
				{
					this.backgroundTexture.Texture = null;
				}
				if (base.Selected)
				{
					this.updateItemInfoWindow(this);
				}
				base.xui.playerUI.CursorController.HoverTarget = null;
			}
			this.viewComponent.IsSnappable = !this.itemStack.IsEmpty();
			this.IsDirty = true;
		}
	}

	// Token: 0x06006629 RID: 26153 RVA: 0x00296138 File Offset: 0x00294338
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateBackgroundTexture()
	{
		if (this.backgroundTexture == null)
		{
			return;
		}
		ItemClassBlock itemClassBlock = this.itemClass as ItemClassBlock;
		if (itemClassBlock != null)
		{
			Block block = itemClassBlock.GetBlock();
			if (block.GetAutoShapeType() == EAutoShapeType.None)
			{
				this.backgroundTexture.Texture = null;
				return;
			}
			int uiBackgroundTextureId = block.GetUiBackgroundTextureId(this.itemStack.itemValue.ToBlockValue(), BlockFace.Top, 0);
			this.<updateBackgroundTexture>g__setTextureForOpaqueMeshTextureId|131_0(uiBackgroundTextureId);
			return;
		}
		else
		{
			ItemClass itemClass = this.itemClass;
			ItemActionTextureBlock itemActionTextureBlock = ((itemClass != null) ? itemClass.Actions[0] : null) as ItemActionTextureBlock;
			if (itemActionTextureBlock != null)
			{
				if (this.itemStack.itemValue.Meta == 0)
				{
					this.itemStack.itemValue.Meta = itemActionTextureBlock.DefaultTextureID;
				}
				int textureID = (int)BlockTextureData.list[this.itemStack.itemValue.Meta].TextureID;
				this.<updateBackgroundTexture>g__setTextureForOpaqueMeshTextureId|131_0(textureID);
				return;
			}
			this.backgroundTexture.Texture = null;
			return;
		}
	}

	// Token: 0x0600662A RID: 26154 RVA: 0x00296214 File Offset: 0x00294414
	public void ResetTweenScale()
	{
		if (this.tweenScale != null && this.tweenScale.value != Vector3.one)
		{
			this.tweenScale.from = Vector3.one * 1.5f;
			this.tweenScale.to = Vector3.one;
			this.tweenScale.enabled = true;
			this.tweenScale.duration = 0.1f;
		}
	}

	// Token: 0x0600662B RID: 26155 RVA: 0x0029628C File Offset: 0x0029448C
	public void ForceSetItemStack(ItemStack _stack)
	{
		bool selected = base.Selected;
		this.itemStack = ItemStack.Empty.Clone();
		XUiEvent_SlotChangedEventHandler slotChangedEvent = this.SlotChangedEvent;
		if (slotChangedEvent != null)
		{
			slotChangedEvent(this.SlotNumber, this.itemStack);
		}
		if (!_stack.IsEmpty())
		{
			base.Selected = selected;
		}
		this.ItemStack = _stack;
		XUiEvent_SlotChangedEventHandler slotChangedEvent2 = this.SlotChangedEvent;
		if (slotChangedEvent2 == null)
		{
			return;
		}
		slotChangedEvent2(this.SlotNumber, this.itemStack);
	}

	// Token: 0x0600662C RID: 26156 RVA: 0x002962FF File Offset: 0x002944FF
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleSlotChangeEvent()
	{
		if (this.itemStack.IsEmpty() && base.Selected)
		{
			base.Selected = false;
		}
		XUiEvent_SlotChangedEventHandler slotChangedEvent = this.SlotChangedEvent;
		if (slotChangedEvent == null)
		{
			return;
		}
		slotChangedEvent(this.SlotNumber, this.itemStack);
	}

	// Token: 0x0600662D RID: 26157 RVA: 0x0029633C File Offset: 0x0029453C
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("timer");
		if (childById != null)
		{
			this.timer = (childById.ViewComponent as XUiV_Label);
		}
		XUiController childById2 = base.GetChildById("itemIcon");
		if (childById2 != null)
		{
			this.itemIconSprite = (childById2.ViewComponent as XUiV_Sprite);
		}
		XUiController childById3 = base.GetChildById("lockTypeIcon");
		if (childById3 != null)
		{
			this.lockTypeIcon = (childById3.ViewComponent as XUiV_Sprite);
		}
		XUiController childById4 = base.GetChildById("backgroundTexture");
		if (childById4 != null)
		{
			this.backgroundTexture = (childById4.ViewComponent as XUiV_Texture);
			if (this.backgroundTexture != null)
			{
				this.backgroundTexture.CreateMaterial();
			}
		}
		XUiController childById5 = base.GetChildById("rectSlotLock");
		if (childById5 != null)
		{
			childById5.OnHover += delegate(XUiController _, bool _over)
			{
			};
			childById5.OnPress += delegate(XUiController _, int _)
			{
				this.UserLockedSlot = !this.UserLockedSlot;
				base.RefreshBindings(false);
			};
		}
		this.tweenScale = this.itemIconSprite.UiTransform.gameObject.AddComponent<TweenScale>();
		base.ViewComponent.UseSelectionBox = false;
	}

	// Token: 0x0600662E RID: 26158 RVA: 0x00296454 File Offset: 0x00294654
	public void UpdateTimer(float _dt)
	{
		if (!this.IsLocked)
		{
			return;
		}
		if (this.lockType == XUiC_ItemStack.LockTypes.Shell || this.lockType == XUiC_ItemStack.LockTypes.Burning)
		{
			return;
		}
		float num = this.lockTime;
		if (this.lockTime > 0f)
		{
			this.lockTime -= _dt;
			if (this.currentInterval == -1)
			{
				this.currentInterval = (int)this.lockTime / this.TimeInterval;
			}
			if (this.TimeIntervalElapsedEvent != null && this.TimeInterval != 0)
			{
				int num2 = (int)this.lockTime / this.TimeInterval;
				if (num2 != this.currentInterval)
				{
					this.TimeIntervalElapsedEvent(this.lockTime, this);
					this.currentInterval = num2;
				}
			}
		}
		if (this.lockTime <= 0f && num != 0f)
		{
			XUiEvent_TimeIntervalElapsedEventHandler timeIntervalElapsedEvent = this.TimeIntervalElapsedEvent;
			if (timeIntervalElapsedEvent != null)
			{
				timeIntervalElapsedEvent(this.lockTime, this);
			}
			if (this.LockChangedEvent != null)
			{
				this.LockChangedEvent(this.lockType, this);
			}
			else
			{
				this.IsLocked = false;
			}
		}
		if (this.lockTime <= 0f)
		{
			this.timer.IsVisible = false;
			this.timer.Text = "";
			return;
		}
		this.timer.IsVisible = true;
		this.timer.Text = string.Format("{0:00}:{1:00}", Mathf.Floor(this.lockTime / 60f), Mathf.Floor(this.lockTime % 60f));
	}

	// Token: 0x0600662F RID: 26159 RVA: 0x002965C4 File Offset: 0x002947C4
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.WindowGroup.isShowing)
		{
			PlayerActionsGUI guiactions = base.xui.playerUI.playerInput.GUIActions;
			CursorControllerAbs cursorController = base.xui.playerUI.CursorController;
			Vector3 a = cursorController.GetScreenPosition();
			bool mouseButtonUp = cursorController.GetMouseButtonUp(UICamera.MouseButton.LeftButton);
			bool mouseButtonDown = cursorController.GetMouseButtonDown(UICamera.MouseButton.LeftButton);
			bool mouseButton = cursorController.GetMouseButton(UICamera.MouseButton.LeftButton);
			bool mouseButtonUp2 = cursorController.GetMouseButtonUp(UICamera.MouseButton.RightButton);
			bool mouseButtonDown2 = cursorController.GetMouseButtonDown(UICamera.MouseButton.RightButton);
			bool mouseButton2 = cursorController.GetMouseButton(UICamera.MouseButton.RightButton);
			if (!this.IsLocked && !this.isDragAndDrop)
			{
				if (this.isOver && UICamera.hoveredObject == base.ViewComponent.UiTransform.gameObject && base.ViewComponent.EventOnPress)
				{
					if (guiactions.LastInputType == BindingSourceType.DeviceBindingSource)
					{
						bool wasReleased = guiactions.Submit.WasReleased;
						bool wasReleased2 = guiactions.HalfStack.WasReleased;
						bool wasPressed = guiactions.Inspect.WasPressed;
						bool wasReleased3 = guiactions.RightStick.WasReleased;
						if (this.SimpleClick && !this.StackLock)
						{
							if (wasReleased)
							{
								this.HandleMoveToPreferredLocation();
							}
							else if (wasPressed)
							{
								this.HandleItemInspect();
							}
						}
						else if (base.xui.dragAndDrop.CurrentStack.IsEmpty() && !this.ItemStack.IsEmpty())
						{
							if (!this.StackLock)
							{
								if (wasReleased)
								{
									this.SwapItem();
								}
								else if (wasReleased2)
								{
									this.HandlePartialStackPickup();
								}
								else if (wasReleased3)
								{
									this.HandleMoveToPreferredLocation();
								}
								else if (wasPressed)
								{
									this.HandleItemInspect();
								}
							}
						}
						else if (!this.StackLock)
						{
							if (wasReleased)
							{
								this.HandleStackSwap();
							}
							else if (wasReleased2 && this.AllowDropping)
							{
								this.HandleDropOne();
							}
						}
					}
					else if (this.SimpleClick && !this.StackLock)
					{
						if (mouseButtonUp)
						{
							this.HandleMoveToPreferredLocation();
						}
					}
					else if (InputUtils.ShiftKeyPressed)
					{
						if (!this.StackLock && mouseButtonUp)
						{
							this.HandleMoveToPreferredLocation();
						}
					}
					else if (mouseButton || mouseButton2)
					{
						if (base.xui.dragAndDrop.CurrentStack.IsEmpty() && !this.ItemStack.IsEmpty())
						{
							if (!this.lastClicked)
							{
								this.startMousePos = a;
							}
							else if (Mathf.Abs((a - this.startMousePos).magnitude) > (float)this.PickupSnapDistance && !this.StackLock)
							{
								if (mouseButton)
								{
									this.SwapItem();
								}
								else
								{
									this.HandlePartialStackPickup();
								}
							}
						}
						if (mouseButtonDown || mouseButtonDown2)
						{
							this.lastClicked = true;
						}
					}
					else if (mouseButtonUp)
					{
						if (base.xui.dragAndDrop.CurrentStack.IsEmpty())
						{
							this.HandleItemInspect();
						}
						else if (this.lastClicked && !this.StackLock)
						{
							this.HandleStackSwap();
						}
					}
					else if (mouseButtonUp2)
					{
						if (this.lastClicked && !this.StackLock && this.AllowDropping)
						{
							this.HandleDropOne();
						}
					}
					else
					{
						this.lastClicked = false;
					}
				}
				else
				{
					this.lastClicked = false;
					if ((this.isOver || this.itemIconSprite.UiTransform.localScale != Vector3.one) && this.tweenScale.value != Vector3.one && !this.itemStack.IsEmpty())
					{
						this.tweenScale.from = Vector3.one * 1.5f;
						this.tweenScale.to = Vector3.one;
						this.tweenScale.enabled = true;
						this.tweenScale.duration = 0.5f;
					}
				}
			}
			else if (this.IsLocked && ((guiactions.LastInputType == BindingSourceType.DeviceBindingSource && guiactions.Submit.WasReleased) || (guiactions.LastInputType != BindingSourceType.DeviceBindingSource && guiactions.LeftClick.WasPressed)) && this.isOver)
			{
				XUiEvent_LockChangeEventHandler lockChangedEvent = this.LockChangedEvent;
				if (lockChangedEvent != null)
				{
					lockChangedEvent(XUiC_ItemStack.LockTypes.None, this);
				}
			}
		}
		this.updateBorderColor();
		if (this.flashLockTypeIcon != XUiC_ItemStack.flashLockTypes.None)
		{
			Color b = (this.flashLockTypeIcon == XUiC_ItemStack.flashLockTypes.Allowed) ? this.modAllowedColor : this.modAlreadyEquippedColor;
			float num = Mathf.PingPong(Time.time, 0.5f);
			this.setLockTypeIconColor(Color.Lerp(Color.grey, b, num * 4f));
		}
		if (this.IsDirty)
		{
			this.IsDirty = false;
			this.updateLockTypeIcon();
			base.RefreshBindings(false);
			this.ResetTweenScale();
		}
		if (this.IsLocked && this.lockType != XUiC_ItemStack.LockTypes.None)
		{
			this.UpdateTimer(_dt);
		}
	}

	// Token: 0x06006630 RID: 26160 RVA: 0x00282536 File Offset: 0x00280736
	public override void OnOpen()
	{
		base.OnOpen();
		this.IsDirty = true;
	}

	// Token: 0x06006631 RID: 26161 RVA: 0x00296A8C File Offset: 0x00294C8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateBorderColor()
	{
		if (this.IsDragAndDrop)
		{
			this.SelectionBorderColor = Color.clear;
			return;
		}
		if (base.Selected)
		{
			this.SelectionBorderColor = this.selectColor;
			return;
		}
		if (this.isOver)
		{
			this.SelectionBorderColor = this.highlightColor;
			return;
		}
		if (this.IsHolding)
		{
			this.SelectionBorderColor = this.holdingColor;
			return;
		}
		this.SelectionBorderColor = this.backgroundColor;
	}

	// Token: 0x06006632 RID: 26162 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual bool CanSwap(ItemStack _stack)
	{
		return true;
	}

	// Token: 0x06006633 RID: 26163 RVA: 0x00296AFD File Offset: 0x00294CFD
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleItemInspect()
	{
		if (!this.ItemStack.IsEmpty() && this.InfoWindow != null)
		{
			base.Selected = true;
			this.InfoWindow.SetMaxCountOnDirty = true;
			this.updateItemInfoWindow(this);
		}
		this.HandleClickComplete();
	}

	// Token: 0x06006634 RID: 26164 RVA: 0x00296B34 File Offset: 0x00294D34
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandleStackSwap()
	{
		base.xui.currentPopupMenu.ClearItems();
		if (!this.AllowDropping)
		{
			base.xui.dragAndDrop.CurrentStack = ItemStack.Empty.Clone();
			base.xui.dragAndDrop.PickUpType = this.StackLocation;
		}
		bool flag = false;
		ItemStack currentStack = base.xui.dragAndDrop.CurrentStack;
		ItemClass itemClassOrMissing = currentStack.itemValue.ItemClassOrMissing;
		int num = 0;
		if (itemClassOrMissing != null)
		{
			num = ((this.OverrideStackCount == -1) ? itemClassOrMissing.Stacknumber.Value : Mathf.Min(itemClassOrMissing.Stacknumber.Value, this.OverrideStackCount));
			if (!currentStack.IsEmpty() && this.itemStack.IsEmpty() && num < currentStack.count)
			{
				flag = true;
			}
		}
		if (!flag && (this.itemStack.IsEmpty() || currentStack.IsEmpty()))
		{
			this.SwapItem();
			base.Selected = false;
		}
		else if (!flag && (!this.itemStack.itemValue.ItemClassOrMissing.CanStack() || !itemClassOrMissing.CanStack()))
		{
			this.SwapItem();
			base.Selected = false;
		}
		else if (currentStack.itemValue.type == this.itemStack.itemValue.type && !currentStack.itemValue.HasQuality && !this.itemStack.itemValue.HasQuality)
		{
			if (currentStack.count + this.itemStack.count > num)
			{
				int count = currentStack.count + this.itemStack.count - num;
				ItemStack itemStack = this.itemStack.Clone();
				itemStack.count = num;
				currentStack.count = count;
				base.xui.dragAndDrop.CurrentStack = currentStack;
				base.xui.dragAndDrop.PickUpType = this.StackLocation;
				this.ItemStack = itemStack;
				this.PlayPickupSound(null);
			}
			else
			{
				ItemStack itemStack2 = this.itemStack.Clone();
				itemStack2.count += currentStack.count;
				this.ItemStack = itemStack2;
				base.xui.dragAndDrop.CurrentStack = ItemStack.Empty.Clone();
				this.PlayPlaceSound(null);
			}
			if (base.Selected)
			{
				this.updateItemInfoWindow(this);
			}
		}
		else if (flag)
		{
			int count2 = currentStack.count - num;
			ItemStack itemStack3 = currentStack.Clone();
			itemStack3.count = num;
			currentStack.count = count2;
			base.xui.dragAndDrop.CurrentStack = currentStack;
			base.xui.dragAndDrop.PickUpType = this.StackLocation;
			this.ItemStack = itemStack3;
			this.PlayPickupSound(null);
		}
		else
		{
			this.SwapItem();
			base.Selected = false;
		}
		this.HandleClickComplete();
		base.xui.calloutWindow.UpdateCalloutsForItemStack(base.ViewComponent.UiTransform.gameObject, this.ItemStack, this.isOver, true, true, true);
	}

	// Token: 0x06006635 RID: 26165 RVA: 0x00296E18 File Offset: 0x00295018
	[PublicizedFrom(EAccessModifier.Protected)]
	public void HandlePartialStackPickup()
	{
		ItemStack itemStack = base.xui.dragAndDrop.CurrentStack;
		if (itemStack.IsEmpty() && !this.itemStack.IsEmpty())
		{
			int num = this.itemStack.count / 2;
			if (num > 0)
			{
				itemStack = this.itemStack.Clone();
				itemStack.count = num;
				if (this.AllowDropping)
				{
					ItemStack itemStack2 = this.itemStack.Clone();
					itemStack2.count -= num;
					this.ItemStack = itemStack2;
				}
				base.xui.dragAndDrop.CurrentStack = itemStack;
				base.xui.dragAndDrop.PickUpType = this.StackLocation;
				this.PlayPickupSound(null);
			}
		}
		if (base.Selected)
		{
			base.Selected = false;
		}
		this.HandleClickComplete();
		base.xui.calloutWindow.UpdateCalloutsForItemStack(base.ViewComponent.UiTransform.gameObject, this.ItemStack, this.isOver, true, true, true);
	}

	// Token: 0x06006636 RID: 26166 RVA: 0x00296F10 File Offset: 0x00295110
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleDropOne()
	{
		ItemStack currentStack = base.xui.dragAndDrop.CurrentStack;
		if (!currentStack.IsEmpty())
		{
			int num = 1;
			if (this.itemStack.IsEmpty())
			{
				ItemStack itemStack = currentStack.Clone();
				itemStack.count = num;
				currentStack.count -= num;
				base.xui.dragAndDrop.CurrentStack = currentStack;
				this.ItemStack = itemStack;
				this.PlayPlaceSound(null);
			}
			else if (currentStack.itemValue.type == this.itemStack.itemValue.type)
			{
				ItemClass itemClassOrMissing = currentStack.itemValue.ItemClassOrMissing;
				int num2 = (this.OverrideStackCount == -1) ? itemClassOrMissing.Stacknumber.Value : Mathf.Min(itemClassOrMissing.Stacknumber.Value, this.OverrideStackCount);
				if (this.itemStack.count + 1 <= num2)
				{
					ItemStack itemStack2 = this.itemStack.Clone();
					itemStack2.count++;
					currentStack.count--;
					this.ItemStack = itemStack2.Clone();
					base.xui.dragAndDrop.CurrentStack = currentStack;
					XUiEvent_SlotChangedEventHandler slotChangedEvent = this.SlotChangedEvent;
					if (slotChangedEvent != null)
					{
						slotChangedEvent(this.SlotNumber, this.itemStack);
					}
					this.IsDirty = true;
				}
				this.PlayPlaceSound(null);
			}
			if (currentStack.count == 0)
			{
				base.xui.dragAndDrop.CurrentStack = ItemStack.Empty.Clone();
			}
		}
		base.Selected = false;
		this.HandleClickComplete();
		base.xui.calloutWindow.UpdateCalloutsForItemStack(base.ViewComponent.UiTransform.gameObject, this.ItemStack, this.isOver, true, true, true);
	}

	// Token: 0x06006637 RID: 26167 RVA: 0x002970C4 File Offset: 0x002952C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void HandleClickComplete()
	{
		this.lastClicked = false;
		if (this.itemIconSprite.UiTransform.localScale.x <= 1f)
		{
			return;
		}
		if (this.itemStack.IsEmpty())
		{
			return;
		}
		this.tweenScale.from = Vector3.one * 1.5f;
		this.tweenScale.to = Vector3.one;
		this.tweenScale.enabled = true;
		this.tweenScale.duration = 0.5f;
	}

	// Token: 0x06006638 RID: 26168 RVA: 0x0029714C File Offset: 0x0029534C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		this.isOver = _isOver;
		if (!this.IsLocked)
		{
			if (_isOver)
			{
				if (this.InfoWindow != null && this.InfoWindow.ViewComponent.IsVisible)
				{
					this.InfoWindow.HoverEntry = this;
				}
				if (this.AllowIconGrow)
				{
					this.tweenScale.from = Vector3.one;
					this.tweenScale.to = Vector3.one * 1.5f;
					this.tweenScale.enabled = true;
					this.tweenScale.duration = 0.5f;
				}
			}
			else
			{
				if (this.InfoWindow != null && this.InfoWindow.ViewComponent.IsVisible)
				{
					this.InfoWindow.HoverEntry = null;
				}
				if (this.AllowIconGrow)
				{
					this.tweenScale.from = Vector3.one * 1.5f;
					this.tweenScale.to = Vector3.one;
					this.tweenScale.enabled = true;
					this.tweenScale.duration = 0.5f;
				}
			}
		}
		base.xui.calloutWindow.UpdateCalloutsForItemStack(base.ViewComponent.UiTransform.gameObject, this.ItemStack, this.isOver, true, !this.StackLock, true);
		base.OnHovered(_isOver);
	}

	// Token: 0x06006639 RID: 26169 RVA: 0x0029729C File Offset: 0x0029549C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SwapItem()
	{
		base.xui.currentPopupMenu.ClearItems();
		ItemStack currentStack = base.xui.dragAndDrop.CurrentStack;
		if (!currentStack.IsEmpty() && !this.CanSwap(currentStack))
		{
			return;
		}
		if (this.StackLocation == XUiC_ItemStack.StackLocationTypes.LootContainer && base.xui.dragAndDrop.PickUpType != XUiC_ItemStack.StackLocationTypes.LootContainer && !currentStack.IsEmpty() && !currentStack.itemValue.ItemClassOrMissing.CanPlaceInContainer())
		{
			Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
			GameManager.ShowTooltip(base.xui.playerUI.entityPlayer, "Quest Items cannot be placed in containers.", false, false, 0f);
			return;
		}
		if (this.itemStack.IsEmpty())
		{
			this.PlayPlaceSound(currentStack);
		}
		else
		{
			this.PlayPickupSound(null);
		}
		base.xui.dragAndDrop.CurrentStack = this.itemStack.Clone();
		base.xui.dragAndDrop.PickUpType = this.StackLocation;
		if (this.StackLocation == XUiC_ItemStack.StackLocationTypes.ToolBelt)
		{
			base.xui.dragAndDrop.CurrentStack.Deactivate();
		}
		this.ForceSetItemStack(currentStack.Clone());
		base.xui.calloutWindow.UpdateCalloutsForItemStack(base.ViewComponent.UiTransform.gameObject, this.ItemStack, this.isOver, true, true, true);
	}

	// Token: 0x0600663A RID: 26170 RVA: 0x002973F0 File Offset: 0x002955F0
	public void HandleMoveToPreferredLocation()
	{
		base.xui.currentPopupMenu.ClearItems();
		if (this.ItemStack.IsEmpty())
		{
			return;
		}
		if (this.StackLock)
		{
			return;
		}
		if (this.StackLocation == XUiC_ItemStack.StackLocationTypes.ToolBelt)
		{
			this.ItemStack.Deactivate();
		}
		int count = this.ItemStack.count;
		switch (this.StackLocation)
		{
		case XUiC_ItemStack.StackLocationTypes.Backpack:
		case XUiC_ItemStack.StackLocationTypes.ToolBelt:
		{
			XUiM_AssembleItem assembleItem = base.xui.AssembleItem;
			bool flag = ((assembleItem != null) ? assembleItem.CurrentItem : null) != null;
			if (base.xui.vehicle != null && !flag)
			{
				string vehicleSlotType = this.ItemStack.itemValue.ItemClass.VehicleSlotType;
				ItemStack itemStack;
				if (vehicleSlotType != "" && base.xui.Vehicle.SetPart(base.xui, vehicleSlotType, this.ItemStack, out itemStack))
				{
					this.PlayPlaceSound(null);
					this.ItemStack = itemStack;
					this.HandleSlotChangeEvent();
					return;
				}
				if (base.xui.vehicle.GetVehicle().HasStorage())
				{
					XUiC_VehicleContainer childByType = base.xui.FindWindowGroupByName(XUiC_VehicleStorageWindowGroup.ID).GetChildByType<XUiC_VehicleContainer>();
					if (childByType != null)
					{
						if (childByType.AddItem(this.ItemStack))
						{
							this.PlayPlaceSound(null);
							this.ItemStack = ItemStack.Empty.Clone();
							this.HandleSlotChangeEvent();
							return;
						}
						if (count != this.ItemStack.count)
						{
							this.PlayPlaceSound(null);
							if (this.ItemStack.count == 0)
							{
								this.ItemStack = ItemStack.Empty.Clone();
							}
							this.HandleSlotChangeEvent();
							return;
						}
					}
				}
			}
			if (flag && this.ItemStack.itemValue.ItemClass is ItemClassModifier)
			{
				ItemStack itemStack2;
				if (base.xui.AssembleItem.AddPartToItem(this.ItemStack, out itemStack2))
				{
					this.PlayPlaceSound(null);
					this.ItemStack = itemStack2;
					this.HandleSlotChangeEvent();
					return;
				}
				Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
				return;
			}
			else
			{
				if (base.xui.PlayerEquipment != null && base.xui.PlayerEquipment.IsOpen && this.itemStack.itemValue.ItemClass.IsEquipment)
				{
					this.PlayPlaceSound(null);
					this.ItemStack = base.xui.PlayerEquipment.EquipItem(this.ItemStack);
					this.HandleSlotChangeEvent();
					return;
				}
				if (base.xui.lootContainer != null)
				{
					XUiC_LootContainer childByType2 = base.xui.FindWindowGroupByName(XUiC_LootWindowGroup.ID).GetChildByType<XUiC_LootContainer>();
					if (XUiM_LootContainer.AddItem(this.ItemStack, base.xui))
					{
						this.PlayPlaceSound(null);
						this.ItemStack = ItemStack.Empty.Clone();
						this.HandleSlotChangeEvent();
						if (childByType2 != null)
						{
							childByType2.SetSlots(base.xui.lootContainer, base.xui.lootContainer.items);
						}
						return;
					}
					if (count != this.ItemStack.count)
					{
						this.PlayPlaceSound(null);
						if (this.ItemStack.count == 0)
						{
							this.ItemStack = ItemStack.Empty.Clone();
						}
						this.HandleSlotChangeEvent();
						if (childByType2 != null)
						{
							childByType2.SetSlots(base.xui.lootContainer, base.xui.lootContainer.items);
						}
						return;
					}
				}
				if (base.xui.currentWorkstationToolGrid != null && base.xui.currentWorkstationToolGrid.TryAddTool(this.itemClass, this.ItemStack))
				{
					this.PlayPlaceSound(null);
					this.ItemStack = ItemStack.Empty.Clone();
					this.HandleSlotChangeEvent();
					return;
				}
				if (base.xui.currentWorkstationFuelGrid != null && this.itemClass.FuelValue != null && this.itemClass.FuelValue.Value > 0)
				{
					if (base.xui.currentWorkstationFuelGrid.AddItem(this.itemClass, this.ItemStack))
					{
						this.PlayPlaceSound(null);
						this.ItemStack = ItemStack.Empty.Clone();
						this.HandleSlotChangeEvent();
						return;
					}
					if (count != this.ItemStack.count)
					{
						this.PlayPlaceSound(null);
						if (this.ItemStack.count == 0)
						{
							this.ItemStack = ItemStack.Empty.Clone();
						}
						this.HandleSlotChangeEvent();
						return;
					}
				}
				if (base.xui.currentDewCollectorModGrid != null && base.xui.currentDewCollectorModGrid.TryAddMod(this.itemClass, this.ItemStack))
				{
					this.PlayPlaceSound(null);
					this.ItemStack = ItemStack.Empty.Clone();
					this.HandleSlotChangeEvent();
					return;
				}
				if (base.xui.currentCombineGrid != null && base.xui.currentCombineGrid.TryAddItemToSlot(this.itemClass, this.ItemStack))
				{
					this.PlayPlaceSound(null);
					this.ItemStack = ItemStack.Empty.Clone();
					this.HandleSlotChangeEvent();
					return;
				}
				if (base.xui.powerSourceSlots != null && base.xui.powerSourceSlots.TryAddItemToSlot(this.itemClass, this.ItemStack))
				{
					this.PlayPlaceSound(null);
					this.ItemStack = ItemStack.Empty.Clone();
					this.HandleSlotChangeEvent();
					return;
				}
				if (base.xui.powerAmmoSlots != null)
				{
					if (base.xui.powerAmmoSlots.TryAddItemToSlot(this.itemClass, this.ItemStack))
					{
						this.PlayPlaceSound(null);
						this.ItemStack = ItemStack.Empty.Clone();
						this.HandleSlotChangeEvent();
						return;
					}
					if (count != this.ItemStack.count)
					{
						this.PlayPlaceSound(null);
						if (this.ItemStack.count == 0)
						{
							this.ItemStack = ItemStack.Empty.Clone();
						}
						this.HandleSlotChangeEvent();
						return;
					}
				}
				if (base.xui.Trader.Trader != null && (this.StackLocation == XUiC_ItemStack.StackLocationTypes.Backpack || this.StackLocation == XUiC_ItemStack.StackLocationTypes.ToolBelt))
				{
					this.HandleItemInspect();
					this.InfoWindow.SetMaxCountOnDirty = true;
					return;
				}
				if (this.StackLocation == XUiC_ItemStack.StackLocationTypes.Backpack)
				{
					if (base.xui.PlayerInventory.AddItemToToolbelt(this.ItemStack))
					{
						this.PlayPlaceSound(null);
						this.ItemStack = ItemStack.Empty.Clone();
						this.HandleSlotChangeEvent();
						return;
					}
					if (count != this.ItemStack.count)
					{
						this.PlayPlaceSound(null);
						if (this.ItemStack.count == 0)
						{
							this.ItemStack = ItemStack.Empty.Clone();
						}
						this.HandleSlotChangeEvent();
						return;
					}
				}
				else if (this.StackLocation == XUiC_ItemStack.StackLocationTypes.ToolBelt)
				{
					if (base.xui.PlayerInventory.AddItemToBackpack(this.ItemStack))
					{
						this.PlayPlaceSound(null);
						this.ItemStack = ItemStack.Empty.Clone();
						this.HandleSlotChangeEvent();
						return;
					}
					if (count != this.ItemStack.count)
					{
						this.PlayPlaceSound(null);
						if (this.ItemStack.count == 0)
						{
							this.ItemStack = ItemStack.Empty.Clone();
						}
						this.HandleSlotChangeEvent();
						return;
					}
				}
			}
			break;
		}
		case XUiC_ItemStack.StackLocationTypes.LootContainer:
		case XUiC_ItemStack.StackLocationTypes.Workstation:
		case XUiC_ItemStack.StackLocationTypes.Merge:
			if (base.xui.PlayerInventory.AddItem(this.ItemStack))
			{
				this.PlayPlaceSound(null);
				this.ItemStack = ItemStack.Empty.Clone();
				this.HandleSlotChangeEvent();
			}
			else if (count != this.ItemStack.count)
			{
				this.PlayPlaceSound(null);
				if (this.ItemStack.count == 0)
				{
					this.ItemStack = ItemStack.Empty.Clone();
				}
				this.HandleSlotChangeEvent();
				return;
			}
			break;
		case XUiC_ItemStack.StackLocationTypes.Creative:
		{
			ItemStack itemStack3 = this.itemStack.Clone();
			if (!base.xui.PlayerInventory.AddItem(itemStack3))
			{
				return;
			}
			this.PlayPlaceSound(null);
			break;
		}
		}
		base.xui.calloutWindow.UpdateCalloutsForItemStack(base.ViewComponent.UiTransform.gameObject, this.ItemStack, this.isOver, true, true, true);
	}

	// Token: 0x0600663B RID: 26171 RVA: 0x00297B88 File Offset: 0x00295D88
	[PublicizedFrom(EAccessModifier.Protected)]
	public void PlayPlaceSound(ItemStack newStack = null)
	{
		string text;
		if (newStack != null)
		{
			text = ((newStack.itemValue.ItemClass == null) ? "" : newStack.itemValue.ItemClass.SoundPlace);
		}
		else
		{
			text = ((this.itemStack.itemValue.ItemClass == null) ? "" : this.itemStack.itemValue.ItemClass.SoundPlace);
		}
		if (text != "")
		{
			if (text != null)
			{
				Manager.PlayInsidePlayerHead(text, -1, 0f, false, false);
				return;
			}
		}
		else if (this.placeSound != null)
		{
			Manager.PlayXUiSound(this.placeSound, 0.75f);
		}
	}

	// Token: 0x0600663C RID: 26172 RVA: 0x00297C34 File Offset: 0x00295E34
	[PublicizedFrom(EAccessModifier.Protected)]
	public void PlayPickupSound(ItemStack newStack = null)
	{
		ItemStack itemStack = (newStack != null) ? newStack : this.itemStack;
		string text = (itemStack.IsEmpty() || itemStack.itemValue.ItemClass == null) ? "" : itemStack.itemValue.ItemClass.SoundPickup;
		if (text != "")
		{
			if (text != null)
			{
				Manager.PlayInsidePlayerHead(text, -1, 0f, false, false);
				return;
			}
		}
		else if (this.pickupSound != null)
		{
			Manager.PlayXUiSound(this.pickupSound, 0.75f);
		}
	}

	// Token: 0x0600663D RID: 26173 RVA: 0x00297CB8 File Offset: 0x00295EB8
	public void UnlockStack()
	{
		this.lockType = XUiC_ItemStack.LockTypes.None;
		this.IsLocked = false;
		this.lockTime = 0f;
		this.lockSprite = "";
		this.setLockTypeIconColor(Color.white);
		this.RepairAmount = 0;
		this.timer.IsVisible = false;
		this.IsDirty = true;
	}

	// Token: 0x0600663E RID: 26174 RVA: 0x00297D10 File Offset: 0x00295F10
	public void LockStack(XUiC_ItemStack.LockTypes _lockType, float _time, int _count, BaseItemActionEntry _itemActionEntry)
	{
		if (_lockType == XUiC_ItemStack.LockTypes.Crafting)
		{
			this.lockSprite = _itemActionEntry.IconName;
		}
		else if (_lockType == XUiC_ItemStack.LockTypes.Scrapping)
		{
			this.lockSprite = "ui_game_symbol_scrap";
		}
		else if (_lockType == XUiC_ItemStack.LockTypes.Burning)
		{
			this.lockSprite = "ui_game_symbol_campfire";
		}
		else if (_lockType == XUiC_ItemStack.LockTypes.Repairing)
		{
			this.lockSprite = "ui_game_symbol_hammer";
		}
		this.IsLocked = true;
		this.lockType = _lockType;
		if (_lockType == XUiC_ItemStack.LockTypes.Repairing)
		{
			this.RepairAmount = _count;
		}
		this.LockTime = _time;
		this.IsDirty = true;
	}

	// Token: 0x0600663F RID: 26175 RVA: 0x00297D88 File Offset: 0x00295F88
	[PublicizedFrom(EAccessModifier.Private)]
	public void updateLockTypeIcon()
	{
		if (this.IsLocked && this.lockType != XUiC_ItemStack.LockTypes.None)
		{
			return;
		}
		this.lockSprite = "";
		if (this.itemClass != null)
		{
			ItemClassBlock itemClassBlock = this.itemClass as ItemClassBlock;
			if (itemClassBlock != null)
			{
				itemClassBlock.GetBlock();
				if (!this.itemStack.itemValue.TextureFullArray.IsDefault)
				{
					this.lockSprite = "ui_game_symbol_paint_brush";
				}
			}
			ItemClassModifier itemClassModifier = this.itemClass as ItemClassModifier;
			if (itemClassModifier != null)
			{
				this.lockSprite = "ui_game_symbol_assemble";
				if (itemClassModifier.HasAnyTags(ItemClassModifier.CosmeticModTypes))
				{
					this.lockSprite = "ui_game_symbol_paint_bucket";
				}
				if (base.xui.AssembleItem.CurrentItem != null)
				{
					if ((itemClassModifier.InstallableTags.IsEmpty || base.xui.AssembleItem.CurrentItem.itemValue.ItemClass.HasAnyTags(itemClassModifier.InstallableTags)) && !base.xui.AssembleItem.CurrentItem.itemValue.ItemClass.HasAnyTags(itemClassModifier.DisallowedTags))
					{
						if (this.StackLocation != XUiC_ItemStack.StackLocationTypes.Part)
						{
							for (int i = 0; i < base.xui.AssembleItem.CurrentItem.itemValue.Modifications.Length; i++)
							{
								ItemValue itemValue = base.xui.AssembleItem.CurrentItem.itemValue.Modifications[i];
								if (!itemValue.IsEmpty() && itemValue.ItemClass.HasAnyTags(itemClassModifier.ItemTags))
								{
									this.flashLockTypeIcon = XUiC_ItemStack.flashLockTypes.AlreadyEquipped;
									return;
								}
							}
						}
						this.flashLockTypeIcon = XUiC_ItemStack.flashLockTypes.Allowed;
					}
					else
					{
						this.setLockTypeIconColor(Color.grey);
						this.flashLockTypeIcon = XUiC_ItemStack.flashLockTypes.None;
					}
				}
				else
				{
					this.setLockTypeIconColor(Color.white);
					this.flashLockTypeIcon = XUiC_ItemStack.flashLockTypes.None;
				}
			}
			if (this.itemStack.itemValue.HasMods())
			{
				this.lockSprite = "ui_game_symbol_modded";
				this.setLockTypeIconColor(Color.white);
				this.flashLockTypeIcon = XUiC_ItemStack.flashLockTypes.None;
			}
		}
		if (this.StackLocation == XUiC_ItemStack.StackLocationTypes.Part)
		{
			this.lockSprite = "";
		}
	}

	// Token: 0x06006640 RID: 26176 RVA: 0x00297F80 File Offset: 0x00296180
	[PublicizedFrom(EAccessModifier.Private)]
	public void setLockTypeIconColor(Color _color)
	{
		if (this.lockTypeIcon == null)
		{
			return;
		}
		this.lockTypeIcon.Color = _color;
	}

	// Token: 0x06006641 RID: 26177 RVA: 0x00297F97 File Offset: 0x00296197
	public void ForceRefreshItemStack()
	{
		XUiEvent_SlotChangedEventHandler slotChangedEvent = this.SlotChangedEvent;
		if (slotChangedEvent == null)
		{
			return;
		}
		slotChangedEvent(this.SlotNumber, this.itemStack);
	}

	// Token: 0x17000A83 RID: 2691
	// (get) Token: 0x06006642 RID: 26178 RVA: 0x00297FB5 File Offset: 0x002961B5
	public bool IsFavorite
	{
		get
		{
			return !this.itemStack.IsEmpty() && base.xui.playerUI.entityPlayer.favoriteCreativeStacks.Contains((ushort)this.itemStack.itemValue.type);
		}
	}

	// Token: 0x17000A84 RID: 2692
	// (get) Token: 0x06006643 RID: 26179 RVA: 0x00297FF4 File Offset: 0x002961F4
	public virtual string ItemIcon
	{
		get
		{
			if (this.itemStack.IsEmpty())
			{
				return "";
			}
			ItemClass itemClassOrMissing = this.itemClassOrMissing;
			ItemClassBlock itemClassBlock = itemClassOrMissing as ItemClassBlock;
			Block block = (itemClassBlock != null) ? itemClassBlock.GetBlock() : null;
			if (block == null)
			{
				return this.itemStack.itemValue.GetPropertyOverride(ItemClass.PropCustomIcon, itemClassOrMissing.GetIconName());
			}
			if (!block.SelectAlternates)
			{
				return this.itemStack.itemValue.GetPropertyOverride(ItemClass.PropCustomIcon, itemClassOrMissing.GetIconName());
			}
			return block.GetAltBlockValue(this.itemStack.itemValue.Meta).Block.GetIconName();
		}
	}

	// Token: 0x17000A85 RID: 2693
	// (get) Token: 0x06006644 RID: 26180 RVA: 0x00298094 File Offset: 0x00296294
	public virtual string ItemIconColor
	{
		get
		{
			ItemClass itemClassOrMissing = this.itemClassOrMissing;
			if (itemClassOrMissing == null)
			{
				return "255,255,255,0";
			}
			Color32 v = itemClassOrMissing.GetIconTint(this.itemStack.itemValue);
			return this.itemiconcolorFormatter.Format(v);
		}
	}

	// Token: 0x17000A86 RID: 2694
	// (get) Token: 0x06006645 RID: 26181 RVA: 0x002980D4 File Offset: 0x002962D4
	// (set) Token: 0x06006646 RID: 26182 RVA: 0x002980EB File Offset: 0x002962EB
	public bool GreyedOut
	{
		get
		{
			return this.itemIconSprite.UIAtlas == "ItemIconAtlasGreyscale";
		}
		set
		{
			if (this.itemIconSprite != null)
			{
				this.itemIconSprite.UIAtlas = (value ? "ItemIconAtlasGreyscale" : "ItemIconAtlas");
			}
		}
	}

	// Token: 0x17000A87 RID: 2695
	// (get) Token: 0x06006647 RID: 26183 RVA: 0x00298110 File Offset: 0x00296310
	public string ItemNameText
	{
		get
		{
			if (this.itemStack.IsEmpty())
			{
				return "";
			}
			ItemClass itemClassOrMissing = this.itemClassOrMissing;
			string text = itemClassOrMissing.GetLocalizedItemName();
			if (itemClassOrMissing.IsBlock())
			{
				text = Block.list[this.itemStack.itemValue.type].GetLocalizedBlockName(this.itemStack.itemValue);
			}
			if (!this.PrefixId)
			{
				return text;
			}
			int itemOrBlockId = this.itemStack.itemValue.GetItemOrBlockId();
			return string.Format("{0}\n({1}) {2}", text, itemOrBlockId, itemClassOrMissing.Name);
		}
	}

	// Token: 0x17000A88 RID: 2696
	// (get) Token: 0x06006648 RID: 26184 RVA: 0x0029819F File Offset: 0x0029639F
	public bool ShowDurability
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			if (!this.IsLocked || this.lockType == XUiC_ItemStack.LockTypes.None)
			{
				ItemClass itemClass = this.itemClass;
				return itemClass != null && itemClass.ShowQualityBar;
			}
			return false;
		}
	}

	// Token: 0x06006649 RID: 26185 RVA: 0x002981C4 File Offset: 0x002963C4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 1820482109U)
		{
			if (num <= 770575247U)
			{
				if (num <= 684314633U)
				{
					if (num != 402247192U)
					{
						if (num == 684314633U)
						{
							if (_bindingName == "stacklockicon")
							{
								if (this.stackLockType == XUiC_ItemStack.StackLockTypes.Quest)
								{
									_value = "ui_game_symbol_quest";
								}
								else if (this.attributeLock && this.itemStack.IsEmpty())
								{
									_value = "ui_game_symbol_pack_mule";
								}
								else
								{
									_value = "ui_game_symbol_lock";
								}
								return true;
							}
						}
					}
					else if (_bindingName == "userlockedslot")
					{
						_value = this.UserLockedSlot.ToString();
						return true;
					}
				}
				else if (num != 696352375U)
				{
					if (num != 727013168U)
					{
						if (num == 770575247U)
						{
							if (_bindingName == "stacklockcolor")
							{
								if (this.attributeLock && this.itemStack.IsEmpty())
								{
									_value = "200,200,200,64";
								}
								else
								{
									_value = "255,255,255,255";
								}
								return true;
							}
						}
					}
					else if (_bindingName == "backgroundcolor")
					{
						_value = this.backgroundcolorFormatter.Format(this.AttributeLock ? this.attributeLockColor : this.backgroundColor);
						return true;
					}
				}
				else if (_bindingName == "isQuickSwap")
				{
					_value = this.isQuickSwap.ToString();
					return true;
				}
			}
			else if (num <= 1129104269U)
			{
				if (num != 847165955U)
				{
					if (num != 1062608009U)
					{
						if (num == 1129104269U)
						{
							if (_bindingName == "showicon")
							{
								_value = (this.ItemIcon != "").ToString();
								return true;
							}
						}
					}
					else if (_bindingName == "durabilitycolor")
					{
						CachedStringFormatter<Color32> cachedStringFormatter = this.durabilitycolorFormatter;
						ItemStack itemStack = this.itemStack;
						_value = cachedStringFormatter.Format(QualityInfo.GetQualityColor((int)((itemStack != null) ? itemStack.itemValue.Quality : 0)));
						return true;
					}
				}
				else if (_bindingName == "itemtypeicon")
				{
					_value = "";
					if (!this.itemStack.IsEmpty())
					{
						ItemClass itemClassOrMissing = this.itemClassOrMissing;
						if (itemClassOrMissing != null)
						{
							if (itemClassOrMissing.IsBlock() && this.itemStack.itemValue.TextureFullArray.IsDefault)
							{
								_value = Block.list[this.itemStack.itemValue.type].ItemTypeIcon;
							}
							else
							{
								if (itemClassOrMissing.AltItemTypeIcon != null && itemClassOrMissing.Unlocks != "" && XUiM_ItemStack.CheckKnown(base.xui.playerUI.entityPlayer, itemClassOrMissing, this.itemStack.itemValue))
								{
									_value = itemClassOrMissing.AltItemTypeIcon;
									return true;
								}
								_value = itemClassOrMissing.ItemTypeIcon;
							}
						}
					}
					return true;
				}
			}
			else if (num != 1388578781U)
			{
				if (num != 1768227542U)
				{
					if (num == 1820482109U)
					{
						if (_bindingName == "isfavorite")
						{
							_value = (this.ShowFavorites && this.IsFavorite).ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "selectionbordercolor")
				{
					_value = this.selectionbordercolorFormatter.Format(this.SelectionBorderColor);
					return true;
				}
			}
			else if (_bindingName == "hasitemtypeicon")
			{
				if (this.itemStack.IsEmpty() || !string.IsNullOrEmpty(this.lockSprite))
				{
					_value = "false";
				}
				else
				{
					ItemClass itemClassOrMissing2 = this.itemClassOrMissing;
					if (itemClassOrMissing2 == null)
					{
						_value = "false";
					}
					else
					{
						_value = (itemClassOrMissing2.IsBlock() ? (Block.list[this.itemStack.itemValue.type].ItemTypeIcon != "").ToString() : (itemClassOrMissing2.ItemTypeIcon != "").ToString());
					}
				}
				return true;
			}
		}
		else if (num <= 2944858628U)
		{
			if (num <= 2648037987U)
			{
				if (num != 2412344255U)
				{
					if (num == 2648037987U)
					{
						if (_bindingName == "islocked")
						{
							_value = this.isLocked.ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "isassemblelocked")
				{
					_value = ((this.stackLockType != XUiC_ItemStack.StackLockTypes.None && this.stackLockType != XUiC_ItemStack.StackLockTypes.Hidden) || (this.attributeLock && this.itemStack.IsEmpty())).ToString();
					return true;
				}
			}
			else if (num != 2705680661U)
			{
				if (num != 2733860383U)
				{
					if (num == 2944858628U)
					{
						if (_bindingName == "hasdurability")
						{
							_value = this.ShowDurability.ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "locktypeicon")
				{
					_value = (this.lockSprite ?? "");
					return true;
				}
			}
			else if (_bindingName == "itemcount")
			{
				_value = "";
				if (!this.itemStack.IsEmpty())
				{
					if (this.ShowDurability)
					{
						_value = ((this.itemStack.itemValue.Quality > 0) ? this.itemcountFormatter.Format((int)this.itemStack.itemValue.Quality) : (this.itemStack.itemValue.IsMod ? "*" : ""));
					}
					else
					{
						_value = ((this.itemClassOrMissing.Stacknumber == 1) ? "" : this.itemcountFormatter.Format(this.itemStack.count));
					}
				}
				return true;
			}
		}
		else if (num <= 3741212336U)
		{
			if (num != 3106195591U)
			{
				if (num != 3708628627U)
				{
					if (num == 3741212336U)
					{
						if (_bindingName == "tooltip")
						{
							_value = this.ItemNameText;
							return true;
						}
					}
				}
				else if (_bindingName == "itemicon")
				{
					_value = this.ItemIcon;
					return true;
				}
			}
			else if (_bindingName == "iconcolor")
			{
				_value = this.ItemIconColor;
				return true;
			}
		}
		else if (num != 4046184996U)
		{
			if (num != 4049247086U)
			{
				if (num == 4172540779U)
				{
					if (_bindingName == "durabilityfill")
					{
						ItemStack itemStack2 = this.itemStack;
						_value = ((((itemStack2 != null) ? itemStack2.itemValue : null) == null) ? "0.0" : this.durabilityFillFormatter.Format(this.itemStack.itemValue.PercentUsesLeft));
						return true;
					}
				}
			}
			else if (_bindingName == "itemtypeicontint")
			{
				_value = "255,255,255,255";
				if (!this.itemStack.IsEmpty())
				{
					ItemClass itemClassOrMissing3 = this.itemClassOrMissing;
					if (itemClassOrMissing3 != null && itemClassOrMissing3.Unlocks != "" && XUiM_ItemStack.CheckKnown(base.xui.playerUI.entityPlayer, itemClassOrMissing3, this.itemStack.itemValue))
					{
						_value = this.altitemtypeiconcolorFormatter.Format(itemClassOrMissing3.AltItemTypeIconColor);
					}
				}
				return true;
			}
		}
		else if (_bindingName == "ishovered")
		{
			_value = this.isOver.ToString();
			return true;
		}
		return false;
	}

	// Token: 0x0600664A RID: 26186 RVA: 0x00298944 File Offset: 0x00296B44
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_name);
		if (num <= 783618599U)
		{
			if (num <= 551907657U)
			{
				if (num <= 448973864U)
				{
					if (num != 310027284U)
					{
						if (num == 448973864U)
						{
							if (_name == "mod_already_equipped_color")
							{
								this.modAlreadyEquippedColor = StringParsers.ParseColor32(_value);
								return true;
							}
						}
					}
					else if (_name == "press_color")
					{
						this.pressColor = StringParsers.ParseColor32(_value);
						return true;
					}
				}
				else if (num != 505492686U)
				{
					if (num == 551907657U)
					{
						if (_name == "background_color")
						{
							this.backgroundColor = StringParsers.ParseColor32(_value);
							return true;
						}
					}
				}
				else if (_name == "show_favorites")
				{
					this.ShowFavorites = StringParsers.ParseBool(_value, 0, -1, true);
					return true;
				}
			}
			else if (num <= 589168786U)
			{
				if (num != 587922239U)
				{
					if (num == 589168786U)
					{
						if (_name == "final_pressed_color")
						{
							this.finalPressedColor = StringParsers.ParseColor32(_value);
							return true;
						}
					}
				}
				else if (_name == "highlight_color")
				{
					this.highlightColor = StringParsers.ParseColor32(_value);
					return true;
				}
			}
			else if (num != 605893325U)
			{
				if (num == 783618599U)
				{
					if (_name == "hover_icon_grow")
					{
						this.HoverIconGrow = StringParsers.ParseFloat(_value, 0, -1, NumberStyles.Any);
						return true;
					}
				}
			}
			else if (_name == "pickup_sound")
			{
				base.xui.LoadData<AudioClip>(_value, delegate(AudioClip _o)
				{
					this.pickupSound = _o;
				});
				return true;
			}
		}
		else if (num <= 1808090607U)
		{
			if (num <= 933221862U)
			{
				if (num != 785383727U)
				{
					if (num == 933221862U)
					{
						if (_name == "allow_dropping")
						{
							this.AllowDropping = StringParsers.ParseBool(_value, 0, -1, true);
							return true;
						}
					}
				}
				else if (_name == "prefix_id")
				{
					this.PrefixId = StringParsers.ParseBool(_value, 0, -1, true);
					return true;
				}
			}
			else if (num != 1079211868U)
			{
				if (num == 1808090607U)
				{
					if (_name == "attribute_lock_color")
					{
						this.attributeLockColor = StringParsers.ParseColor32(_value);
						return true;
					}
				}
			}
			else if (_name == "mod_allowed_color")
			{
				this.modAllowedColor = StringParsers.ParseColor32(_value);
				return true;
			}
		}
		else if (num <= 3096841498U)
		{
			if (num != 2540527193U)
			{
				if (num == 3096841498U)
				{
					if (_name == "override_stack_count")
					{
						this.OverrideStackCount = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
						return true;
					}
				}
			}
			else if (_name == "select_color")
			{
				this.selectColor = StringParsers.ParseColor32(_value);
				return true;
			}
		}
		else if (num != 3396886192U)
		{
			if (num != 3919060864U)
			{
				if (num == 3936377800U)
				{
					if (_name == "pickup_snap_distance")
					{
						this.PickupSnapDistance = int.Parse(_value);
						return true;
					}
				}
			}
			else if (_name == "place_sound")
			{
				base.xui.LoadData<AudioClip>(_value, delegate(AudioClip _o)
				{
					this.placeSound = _o;
				});
				return true;
			}
		}
		else if (_name == "holding_color")
		{
			this.holdingColor = StringParsers.ParseColor32(_value);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x0600664B RID: 26187 RVA: 0x00298D28 File Offset: 0x00296F28
	public static bool IsStackLocationFromPlayer(XUiC_ItemStack.StackLocationTypes? location)
	{
		if (location != null)
		{
			XUiC_ItemStack.StackLocationTypes? stackLocationTypes = location;
			XUiC_ItemStack.StackLocationTypes stackLocationTypes2 = XUiC_ItemStack.StackLocationTypes.Backpack;
			if (!(stackLocationTypes.GetValueOrDefault() == stackLocationTypes2 & stackLocationTypes != null))
			{
				stackLocationTypes = location;
				stackLocationTypes2 = XUiC_ItemStack.StackLocationTypes.ToolBelt;
				if (!(stackLocationTypes.GetValueOrDefault() == stackLocationTypes2 & stackLocationTypes != null))
				{
					stackLocationTypes = location;
					stackLocationTypes2 = XUiC_ItemStack.StackLocationTypes.Equipment;
					return stackLocationTypes.GetValueOrDefault() == stackLocationTypes2 & stackLocationTypes != null;
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600664D RID: 26189 RVA: 0x00298F0C File Offset: 0x0029710C
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <updateBackgroundTexture>g__setTextureForOpaqueMeshTextureId|131_0(int _textureId)
	{
		if (_textureId == 0)
		{
			this.backgroundTexture.Texture = null;
			return;
		}
		MeshDescription meshDescription = MeshDescription.meshes[0];
		UVRectTiling uvrectTiling = meshDescription.textureAtlas.uvMapping[_textureId];
		this.backgroundTexture.Texture = meshDescription.textureAtlas.diffuseTexture;
		if (meshDescription.bTextureArray)
		{
			this.backgroundTexture.Material.SetTexture("_BumpMap", meshDescription.textureAtlas.normalTexture);
			this.backgroundTexture.Material.SetFloat("_Index", (float)uvrectTiling.index);
			this.backgroundTexture.Material.SetFloat("_Size", (float)uvrectTiling.blockW);
		}
		else
		{
			this.backgroundTexture.UVRect = uvrectTiling.uv;
		}
		this.backgroundTexture.SetTextureDirty();
	}

	// Token: 0x04004CEA RID: 19690
	[PublicizedFrom(EAccessModifier.Protected)]
	public ItemStack itemStack = ItemStack.Empty.Clone();

	// Token: 0x04004CEB RID: 19691
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOver;

	// Token: 0x04004CEC RID: 19692
	[PublicizedFrom(EAccessModifier.Protected)]
	public AudioClip pickupSound;

	// Token: 0x04004CED RID: 19693
	[PublicizedFrom(EAccessModifier.Protected)]
	public AudioClip placeSound;

	// Token: 0x04004CEE RID: 19694
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 selectColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004CEF RID: 19695
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 pressColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004CF0 RID: 19696
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool lastClicked;

	// Token: 0x04004CF1 RID: 19697
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemStack.LockTypes lockType;

	// Token: 0x04004CF2 RID: 19698
	[PublicizedFrom(EAccessModifier.Private)]
	public string lockSprite;

	// Token: 0x04004CF3 RID: 19699
	[PublicizedFrom(EAccessModifier.Protected)]
	public float lockTime;

	// Token: 0x04004CF4 RID: 19700
	public int TimeInterval = 5;

	// Token: 0x04004CF5 RID: 19701
	public int OverrideStackCount = -1;

	// Token: 0x04004CF6 RID: 19702
	public bool _isQuickSwap;

	// Token: 0x04004CF7 RID: 19703
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 selectionBorderColor;

	// Token: 0x04004CFA RID: 19706
	[PublicizedFrom(EAccessModifier.Protected)]
	public int PickupSnapDistance = 4;

	// Token: 0x04004CFB RID: 19707
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 finalPressedColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004CFC RID: 19708
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 backgroundColor = new Color32(96, 96, 96, byte.MaxValue);

	// Token: 0x04004CFD RID: 19709
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 highlightColor = new Color32(222, 206, 163, byte.MaxValue);

	// Token: 0x04004CFE RID: 19710
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 holdingColor = new Color32(byte.MaxValue, 128, 0, byte.MaxValue);

	// Token: 0x04004CFF RID: 19711
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 attributeLockColor = new Color32(48, 48, 48, byte.MaxValue);

	// Token: 0x04004D00 RID: 19712
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 modAllowedColor = Color.green;

	// Token: 0x04004D01 RID: 19713
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color32 modAlreadyEquippedColor = Color.yellow;

	// Token: 0x04004D02 RID: 19714
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Sprite lockTypeIcon;

	// Token: 0x04004D03 RID: 19715
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Sprite itemIconSprite;

	// Token: 0x04004D04 RID: 19716
	public XUiV_Label timer;

	// Token: 0x04004D05 RID: 19717
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Texture backgroundTexture;

	// Token: 0x04004D0B RID: 19723
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemStack.flashLockTypes flashLockTypeIcon;

	// Token: 0x04004D0C RID: 19724
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ItemStack.StackLockTypes stackLockType;

	// Token: 0x04004D0D RID: 19725
	[PublicizedFrom(EAccessModifier.Private)]
	public bool attributeLock;

	// Token: 0x04004D0E RID: 19726
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDragAndDrop;

	// Token: 0x04004D10 RID: 19728
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isLocked;

	// Token: 0x04004D17 RID: 19735
	[PublicizedFrom(EAccessModifier.Private)]
	public bool userLockedSlot;

	// Token: 0x04004D18 RID: 19736
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentInterval = -1;

	// Token: 0x04004D19 RID: 19737
	[PublicizedFrom(EAccessModifier.Protected)]
	public TweenScale tweenScale;

	// Token: 0x04004D1A RID: 19738
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 startMousePos = Vector3.zero;

	// Token: 0x04004D1B RID: 19739
	[PublicizedFrom(EAccessModifier.Protected)]
	public readonly CachedStringFormatterXuiRgbaColor itemiconcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004D1C RID: 19740
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterInt itemcountFormatter = new CachedStringFormatterInt();

	// Token: 0x04004D1D RID: 19741
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterFloat durabilityFillFormatter = new CachedStringFormatterFloat(null);

	// Token: 0x04004D1E RID: 19742
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor altitemtypeiconcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004D1F RID: 19743
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor durabilitycolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004D20 RID: 19744
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor backgroundcolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004D21 RID: 19745
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor selectionbordercolorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x02000CDE RID: 3294
	[PublicizedFrom(EAccessModifier.Private)]
	public enum flashLockTypes
	{
		// Token: 0x04004D23 RID: 19747
		None,
		// Token: 0x04004D24 RID: 19748
		Allowed,
		// Token: 0x04004D25 RID: 19749
		AlreadyEquipped
	}

	// Token: 0x02000CDF RID: 3295
	public enum LockTypes
	{
		// Token: 0x04004D27 RID: 19751
		None,
		// Token: 0x04004D28 RID: 19752
		Shell,
		// Token: 0x04004D29 RID: 19753
		Crafting,
		// Token: 0x04004D2A RID: 19754
		Repairing,
		// Token: 0x04004D2B RID: 19755
		Scrapping,
		// Token: 0x04004D2C RID: 19756
		Burning
	}

	// Token: 0x02000CE0 RID: 3296
	public enum StackLockTypes
	{
		// Token: 0x04004D2E RID: 19758
		None,
		// Token: 0x04004D2F RID: 19759
		Assemble,
		// Token: 0x04004D30 RID: 19760
		Quest,
		// Token: 0x04004D31 RID: 19761
		Tool,
		// Token: 0x04004D32 RID: 19762
		Hidden
	}

	// Token: 0x02000CE1 RID: 3297
	public enum StackLocationTypes
	{
		// Token: 0x04004D34 RID: 19764
		Backpack,
		// Token: 0x04004D35 RID: 19765
		ToolBelt,
		// Token: 0x04004D36 RID: 19766
		LootContainer,
		// Token: 0x04004D37 RID: 19767
		Equipment,
		// Token: 0x04004D38 RID: 19768
		Creative,
		// Token: 0x04004D39 RID: 19769
		Vehicle,
		// Token: 0x04004D3A RID: 19770
		Workstation,
		// Token: 0x04004D3B RID: 19771
		Merge,
		// Token: 0x04004D3C RID: 19772
		DewCollector,
		// Token: 0x04004D3D RID: 19773
		Cosmetics,
		// Token: 0x04004D3E RID: 19774
		Part
	}
}
