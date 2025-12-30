using System;
using UnityEngine.Scripting;

// Token: 0x02000C5F RID: 3167
[Preserve]
public class XUiC_ContainerStandardControls : XUiController
{
	// Token: 0x17000A09 RID: 2569
	// (get) Token: 0x06006180 RID: 24960 RVA: 0x00279661 File Offset: 0x00277861
	// (set) Token: 0x06006181 RID: 24961 RVA: 0x00279674 File Offset: 0x00277874
	public PackedBoolArray LockedSlots
	{
		get
		{
			Func<PackedBoolArray> getLockedSlotsFromStorage = this.GetLockedSlotsFromStorage;
			if (getLockedSlotsFromStorage == null)
			{
				return null;
			}
			return getLockedSlotsFromStorage();
		}
		set
		{
			Action<PackedBoolArray> setLockedSlotsToStorage = this.SetLockedSlotsToStorage;
			if (setLockedSlotsToStorage == null)
			{
				return;
			}
			setLockedSlotsToStorage(value);
		}
	}

	// Token: 0x06006182 RID: 24962 RVA: 0x00279688 File Offset: 0x00277888
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("btnSort");
		if (childById != null)
		{
			childById.OnPress += delegate(XUiController _, int _)
			{
				this.Sort();
			};
		}
		childById = base.GetChildById("btnMoveAll");
		if (childById != null)
		{
			childById.OnPress += delegate(XUiController _, int _)
			{
				this.MoveAll();
			};
		}
		childById = base.GetChildById("btnMoveFillAndSmart");
		if (childById != null)
		{
			childById.OnPress += delegate(XUiController _, int _)
			{
				this.MoveFillAndSmart();
			};
		}
		childById = base.GetChildById("btnMoveFillStacks");
		if (childById != null)
		{
			childById.OnPress += delegate(XUiController _, int _)
			{
				this.MoveFillStacks();
			};
		}
		childById = base.GetChildById("btnMoveSmart");
		if (childById != null)
		{
			childById.OnPress += delegate(XUiController _, int _)
			{
				this.MoveSmart();
			};
		}
		childById = base.GetChildById("btnToggleLockMode");
		if (childById != null)
		{
			childById.OnPress += delegate(XUiController _, int _)
			{
				this.ToggleLockMode();
			};
			XUiV_Button xuiV_Button = childById.ViewComponent as XUiV_Button;
			if (xuiV_Button != null)
			{
				this.lockModeButtonColorTweener = xuiV_Button.UiTransform.gameObject.GetOrAddComponent<TweenColor>();
				this.lockModeButtonColorTweener.from = xuiV_Button.DefaultSpriteColor;
				this.lockModeButtonColorTweener.to = xuiV_Button.SelectedSpriteColor;
				this.lockModeButtonColorTweener.style = UITweener.Style.PingPong;
				this.lockModeButtonColorTweener.enabled = false;
				this.lockModeButtonColorTweener.duration = 0.4f;
			}
		}
	}

	// Token: 0x06006183 RID: 24963 RVA: 0x002797D3 File Offset: 0x002779D3
	public override void OnOpen()
	{
		base.OnOpen();
		Action<PackedBoolArray> applyLockedSlotStates = this.ApplyLockedSlotStates;
		if (applyLockedSlotStates == null)
		{
			return;
		}
		applyLockedSlotStates(this.LockedSlots);
	}

	// Token: 0x06006184 RID: 24964 RVA: 0x002797F1 File Offset: 0x002779F1
	public void Sort()
	{
		Action<XUiC_ContainerStandardControls> updateLockedSlotStates = this.UpdateLockedSlotStates;
		if (updateLockedSlotStates != null)
		{
			updateLockedSlotStates(this);
		}
		Action<PackedBoolArray> sortPressed = this.SortPressed;
		if (sortPressed == null)
		{
			return;
		}
		sortPressed(this.LockedSlots);
	}

	// Token: 0x06006185 RID: 24965 RVA: 0x0027981C File Offset: 0x00277A1C
	public void MoveAll()
	{
		Action<XUiC_ContainerStandardControls> updateLockedSlotStates = this.UpdateLockedSlotStates;
		if (updateLockedSlotStates != null)
		{
			updateLockedSlotStates(this);
		}
		XUiController srcWindow;
		XUiC_ItemStackGrid srcGrid;
		IInventory dstInventory;
		if (this.MoveAllowed(out srcWindow, out srcGrid, out dstInventory))
		{
			ValueTuple<bool, bool> valueTuple = XUiM_LootContainer.StashItems(srcWindow, srcGrid, dstInventory, 0, this.LockedSlots, XUiM_LootContainer.EItemMoveKind.All, this.MoveStartBottomRight);
			bool item = valueTuple.Item1;
			bool item2 = valueTuple.Item2;
			Action<bool, bool> moveAllDone = this.MoveAllDone;
			if (moveAllDone == null)
			{
				return;
			}
			moveAllDone(item, item2);
		}
	}

	// Token: 0x06006186 RID: 24966 RVA: 0x00279888 File Offset: 0x00277A88
	public void MoveFillAndSmart()
	{
		Action<XUiC_ContainerStandardControls> updateLockedSlotStates = this.UpdateLockedSlotStates;
		if (updateLockedSlotStates != null)
		{
			updateLockedSlotStates(this);
		}
		XUiController srcWindow;
		XUiC_ItemStackGrid srcGrid;
		IInventory dstInventory;
		if (this.MoveAllowed(out srcWindow, out srcGrid, out dstInventory))
		{
			XUiM_LootContainer.StashItems(srcWindow, srcGrid, dstInventory, 0, this.LockedSlots, XUiM_LootContainer.EItemMoveKind.FillOnlyFirstCreateSecond, this.MoveStartBottomRight);
		}
	}

	// Token: 0x06006187 RID: 24967 RVA: 0x002798D4 File Offset: 0x00277AD4
	public void MoveFillStacks()
	{
		Action<XUiC_ContainerStandardControls> updateLockedSlotStates = this.UpdateLockedSlotStates;
		if (updateLockedSlotStates != null)
		{
			updateLockedSlotStates(this);
		}
		XUiController srcWindow;
		XUiC_ItemStackGrid srcGrid;
		IInventory dstInventory;
		if (this.MoveAllowed(out srcWindow, out srcGrid, out dstInventory))
		{
			XUiM_LootContainer.StashItems(srcWindow, srcGrid, dstInventory, 0, this.LockedSlots, XUiM_LootContainer.EItemMoveKind.FillOnly, this.MoveStartBottomRight);
		}
	}

	// Token: 0x06006188 RID: 24968 RVA: 0x00279920 File Offset: 0x00277B20
	public void MoveSmart()
	{
		Action<XUiC_ContainerStandardControls> updateLockedSlotStates = this.UpdateLockedSlotStates;
		if (updateLockedSlotStates != null)
		{
			updateLockedSlotStates(this);
		}
		XUiController srcWindow;
		XUiC_ItemStackGrid srcGrid;
		IInventory dstInventory;
		if (this.MoveAllowed(out srcWindow, out srcGrid, out dstInventory))
		{
			XUiM_LootContainer.StashItems(srcWindow, srcGrid, dstInventory, 0, this.LockedSlots, XUiM_LootContainer.EItemMoveKind.FillAndCreate, this.MoveStartBottomRight);
		}
	}

	// Token: 0x06006189 RID: 24969 RVA: 0x00279969 File Offset: 0x00277B69
	public void ToggleLockMode()
	{
		Action<XUiC_ContainerStandardControls> updateLockedSlotStates = this.UpdateLockedSlotStates;
		if (updateLockedSlotStates != null)
		{
			updateLockedSlotStates(this);
		}
		Action lockModeToggled = this.LockModeToggled;
		if (lockModeToggled == null)
		{
			return;
		}
		lockModeToggled();
	}

	// Token: 0x0600618A RID: 24970 RVA: 0x0027998D File Offset: 0x00277B8D
	public void LockModeChanged(bool _state)
	{
		if (this.lockModeButtonColorTweener != null)
		{
			this.lockModeButtonColorTweener.enabled = _state;
		}
	}

	// Token: 0x0600618B RID: 24971 RVA: 0x002799A9 File Offset: 0x00277BA9
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		if (_name == "move_start_bottom_left")
		{
			this.MoveStartBottomRight = StringParsers.ParseBool(_value, 0, -1, true);
			return true;
		}
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x04004955 RID: 18773
	public Func<PackedBoolArray> GetLockedSlotsFromStorage;

	// Token: 0x04004956 RID: 18774
	public Action<PackedBoolArray> SetLockedSlotsToStorage;

	// Token: 0x04004957 RID: 18775
	public Action<PackedBoolArray> ApplyLockedSlotStates;

	// Token: 0x04004958 RID: 18776
	public Action<XUiC_ContainerStandardControls> UpdateLockedSlotStates;

	// Token: 0x04004959 RID: 18777
	public Action<PackedBoolArray> SortPressed;

	// Token: 0x0400495A RID: 18778
	public Action LockModeToggled;

	// Token: 0x0400495B RID: 18779
	public XUiC_ContainerStandardControls.MoveAllowedDelegate MoveAllowed;

	// Token: 0x0400495C RID: 18780
	public Action<bool, bool> MoveAllDone;

	// Token: 0x0400495D RID: 18781
	public bool MoveStartBottomRight;

	// Token: 0x0400495E RID: 18782
	[PublicizedFrom(EAccessModifier.Private)]
	public bool LockModeEnabled;

	// Token: 0x0400495F RID: 18783
	[PublicizedFrom(EAccessModifier.Private)]
	public TweenColor lockModeButtonColorTweener;

	// Token: 0x02000C60 RID: 3168
	// (Invoke) Token: 0x06006194 RID: 24980
	public delegate bool MoveAllowedDelegate(out XUiController _parentWindow, out XUiC_ItemStackGrid _sourceGrid, out IInventory _destinationInventory);
}
