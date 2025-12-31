using System;
using Audio;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000EAD RID: 3757
[Preserve]
public class XUiC_WorkstationFuelGrid : XUiC_WorkstationGrid
{
	// Token: 0x140000D0 RID: 208
	// (add) Token: 0x060076D0 RID: 30416 RVA: 0x003064C0 File Offset: 0x003046C0
	// (remove) Token: 0x060076D1 RID: 30417 RVA: 0x003064F8 File Offset: 0x003046F8
	public event XuiEvent_WorkstationItemsChanged OnWorkstationFuelChanged;

	// Token: 0x060076D2 RID: 30418 RVA: 0x00306530 File Offset: 0x00304730
	public override void Init()
	{
		base.Init();
		this.flameIcon = this.windowGroup.Controller.GetChildById("flameIcon");
		this.button = this.windowGroup.Controller.GetChildById("button");
		this.button.OnPress += this.Button_OnPress;
		this.onOffLabel = this.windowGroup.Controller.GetChildById("onoff");
		this.items = new ItemStack[this.itemControllers.Length];
		this.turnOff = Localization.Get("xuiTurnOff", false);
		this.turnOn = Localization.Get("xuiTurnOn", false);
	}

	// Token: 0x060076D3 RID: 30419 RVA: 0x003065E0 File Offset: 0x003047E0
	public override void HandleSlotChangedEvent(int slotNumber, ItemStack stack)
	{
		base.HandleSlotChangedEvent(slotNumber, stack);
		((XUiV_Button)this.button.ViewComponent).Enabled = (this.workstationData.GetBurnTimeLeft() > 0f || this.hasAnyFuel());
		this.onFuelItemsChanged();
	}

	// Token: 0x060076D4 RID: 30420 RVA: 0x00306620 File Offset: 0x00304820
	[PublicizedFrom(EAccessModifier.Private)]
	public void onFuelItemsChanged()
	{
		if (this.OnWorkstationFuelChanged != null)
		{
			this.OnWorkstationFuelChanged();
		}
	}

	// Token: 0x060076D5 RID: 30421 RVA: 0x00306638 File Offset: 0x00304838
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.workstationData == null)
		{
			return;
		}
		XUiC_ItemStack xuiC_ItemStack = this.itemControllers[0];
		if (xuiC_ItemStack == null)
		{
			return;
		}
		if (!this.hasFuelStack() && this.hasAnyFuel())
		{
			for (int i = 0; i < this.itemControllers.Length - 1; i++)
			{
				this.CycleStacks();
				if (this.hasFuelStack())
				{
					this.UpdateBackend(this.getUISlots());
					break;
				}
			}
		}
		if (this.isOn && (!this.HasRequirement(null) || this.workstationData.GetIsBesideWater()))
		{
			this.TurnOff();
			this.onFuelItemsChanged();
			return;
		}
		if (this.isOn && this.workstationData != null && xuiC_ItemStack.ItemStack != null)
		{
			if (!xuiC_ItemStack.ItemStack.IsEmpty())
			{
				if (xuiC_ItemStack.IsLocked)
				{
					xuiC_ItemStack.LockTime = this.workstationData.GetBurnTimeLeft();
				}
				else
				{
					xuiC_ItemStack.LockStack(XUiC_ItemStack.LockTypes.Burning, this.workstationData.GetBurnTimeLeft(), 0, null);
				}
			}
			else
			{
				xuiC_ItemStack.UnlockStack();
			}
		}
		if (xuiC_ItemStack != null && (this.workstationData == null || xuiC_ItemStack.ItemStack == null || xuiC_ItemStack.ItemStack.IsEmpty() || !this.isOn))
		{
			xuiC_ItemStack.UnlockStack();
		}
		if (base.xui.playerUI.playerInput.GUIActions.WindowPagingRight.WasPressed && PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			this.Button_OnPress(null, 0);
		}
	}

	// Token: 0x060076D6 RID: 30422 RVA: 0x00306794 File Offset: 0x00304994
	public override void OnOpen()
	{
		base.OnOpen();
		this.isOn = this.workstationData.GetIsBurning();
		((XUiV_Label)this.onOffLabel.ViewComponent).Text = (this.isOn ? this.turnOff : this.turnOn);
		if (this.flameIcon != null)
		{
			((XUiV_Sprite)this.flameIcon.ViewComponent).Color = (this.isOn ? this.flameOnColor : this.flameOffColor);
		}
		base.xui.currentWorkstationFuelGrid = this;
	}

	// Token: 0x060076D7 RID: 30423 RVA: 0x00306827 File Offset: 0x00304A27
	public override void OnClose()
	{
		base.OnClose();
		base.xui.currentWorkstationFuelGrid = null;
	}

	// Token: 0x060076D8 RID: 30424 RVA: 0x0030683C File Offset: 0x00304A3C
	public void TurnOn()
	{
		if (!this.isOn)
		{
			Manager.PlayInsidePlayerHead("forge_burn_fuel", -1, 0f, false, false);
		}
		this.isOn = true;
		this.workstationData.SetIsBurning(this.isOn);
		((XUiV_Label)this.onOffLabel.ViewComponent).Text = this.turnOff;
		if (this.flameIcon != null)
		{
			((XUiV_Sprite)this.flameIcon.ViewComponent).Color = this.flameOnColor;
		}
	}

	// Token: 0x060076D9 RID: 30425 RVA: 0x003068C0 File Offset: 0x00304AC0
	public void TurnOff()
	{
		if (this.isOn)
		{
			Manager.PlayInsidePlayerHead("forge_fire_die", -1, 0f, false, false);
		}
		this.isOn = false;
		this.workstationData.SetIsBurning(this.isOn);
		((XUiV_Label)this.onOffLabel.ViewComponent).Text = this.turnOn;
		XUiC_ItemStack xuiC_ItemStack = this.itemControllers[0];
		if (xuiC_ItemStack != null)
		{
			xuiC_ItemStack.UnlockStack();
		}
		if (this.flameIcon != null)
		{
			((XUiV_Sprite)this.flameIcon.ViewComponent).Color = this.flameOffColor;
		}
	}

	// Token: 0x060076DA RID: 30426 RVA: 0x00306958 File Offset: 0x00304B58
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAnyFuel()
	{
		int num = 0;
		if (!XUi.IsGameRunning())
		{
			return false;
		}
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[i];
			if (xuiC_ItemStack != null && !xuiC_ItemStack.ItemStack.IsEmpty())
			{
				ItemClass itemClass = ItemClass.list[xuiC_ItemStack.ItemStack.itemValue.type];
				if (itemClass != null)
				{
					if (!itemClass.IsBlock())
					{
						if (itemClass != null && itemClass.FuelValue != null)
						{
							num += itemClass.FuelValue.Value;
						}
					}
					else
					{
						Block block = Block.list[itemClass.Id];
						if (block != null)
						{
							num += block.FuelValue;
						}
					}
				}
			}
		}
		return num > 0;
	}

	// Token: 0x060076DB RID: 30427 RVA: 0x003069FC File Offset: 0x00304BFC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasFuelStack()
	{
		XUiC_ItemStack xuiC_ItemStack = this.itemControllers[0];
		if (!XUi.IsGameRunning() || xuiC_ItemStack == null || xuiC_ItemStack.ItemStack.IsEmpty())
		{
			return false;
		}
		int num = 0;
		ItemClass itemClass = ItemClass.list[xuiC_ItemStack.ItemStack.itemValue.type];
		if (itemClass == null)
		{
			return false;
		}
		if (!itemClass.IsBlock())
		{
			if (itemClass != null && itemClass.FuelValue != null)
			{
				num = itemClass.FuelValue.Value;
			}
		}
		else
		{
			Block block = Block.list[itemClass.Id];
			if (block == null)
			{
				return false;
			}
			num = block.FuelValue;
		}
		return num > 0;
	}

	// Token: 0x060076DC RID: 30428 RVA: 0x00306A8C File Offset: 0x00304C8C
	public override bool HasRequirement(Recipe recipe)
	{
		XUiC_ItemStack xuiC_ItemStack = this.itemControllers[0];
		if (!XUi.IsGameRunning() || xuiC_ItemStack == null || xuiC_ItemStack.ItemStack.IsEmpty())
		{
			return this.workstationData.GetBurnTimeLeft() > 0f;
		}
		int num = 0;
		ItemClass itemClass = ItemClass.list[xuiC_ItemStack.ItemStack.itemValue.type];
		if (itemClass == null)
		{
			return this.workstationData.GetBurnTimeLeft() > 0f;
		}
		if (!itemClass.IsBlock())
		{
			if (itemClass != null && itemClass.FuelValue != null)
			{
				num = itemClass.FuelValue.Value;
			}
		}
		else
		{
			Block block = Block.list[itemClass.Id];
			if (block == null)
			{
				return this.workstationData.GetBurnTimeLeft() > 0f;
			}
			num = block.FuelValue;
		}
		return num > 0;
	}

	// Token: 0x060076DD RID: 30429 RVA: 0x00306B4C File Offset: 0x00304D4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void CycleStacks()
	{
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[i];
			if (xuiC_ItemStack != null && xuiC_ItemStack.ItemStack.count <= 0 && i + 1 < this.itemControllers.Length)
			{
				XUiC_ItemStack xuiC_ItemStack2 = this.itemControllers[i + 1];
				if (xuiC_ItemStack2 != null)
				{
					xuiC_ItemStack.ItemStack = xuiC_ItemStack2.ItemStack.Clone();
					xuiC_ItemStack2.ItemStack = ItemStack.Empty.Clone();
				}
			}
		}
	}

	// Token: 0x060076DE RID: 30430 RVA: 0x00306BC2 File Offset: 0x00304DC2
	[PublicizedFrom(EAccessModifier.Private)]
	public void Button_OnPress(XUiController _sender, int _mouseButton)
	{
		if (!this.isOn && (this.hasAnyFuel() || this.workstationData.GetBurnTimeLeft() > 0f) && !this.workstationData.GetIsBesideWater())
		{
			this.TurnOn();
			return;
		}
		this.TurnOff();
	}

	// Token: 0x060076DF RID: 30431 RVA: 0x00306C00 File Offset: 0x00304E00
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateBackend(ItemStack[] stackList)
	{
		base.UpdateBackend(stackList);
		this.workstationData.SetFuelStacks(stackList);
		this.windowGroup.Controller.SetAllChildrenDirty(false);
	}

	// Token: 0x060076E0 RID: 30432 RVA: 0x00306C28 File Offset: 0x00304E28
	public bool AddItem(ItemClass _itemClass, ItemStack _itemStack)
	{
		int startIndex = this.isOn ? 1 : 0;
		this.TryStackItem(startIndex, _itemStack);
		return _itemStack.count > 0 && this.AddItem(_itemStack);
	}

	// Token: 0x060076E1 RID: 30433 RVA: 0x00306C60 File Offset: 0x00304E60
	public bool TryStackItem(int startIndex, ItemStack _itemStack)
	{
		int num = 0;
		for (int i = startIndex; i < this.itemControllers.Length; i++)
		{
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[i];
			ItemStack itemStack = xuiC_ItemStack.ItemStack;
			num = _itemStack.count;
			if (itemStack != null && _itemStack.itemValue.type == itemStack.itemValue.type && itemStack.CanStackPartly(ref num))
			{
				xuiC_ItemStack.ItemStack.count += num;
				xuiC_ItemStack.ItemStack = xuiC_ItemStack.ItemStack;
				xuiC_ItemStack.ForceRefreshItemStack();
				_itemStack.count -= num;
				if (_itemStack.count == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060076E2 RID: 30434 RVA: 0x00306D0C File Offset: 0x00304F0C
	public bool AddItem(ItemStack _item)
	{
		for (int i = 0; i < this.itemControllers.Length; i++)
		{
			XUiC_ItemStack xuiC_ItemStack = this.itemControllers[i];
			ItemStack itemStack = xuiC_ItemStack.ItemStack;
			if (itemStack == null || itemStack.IsEmpty())
			{
				xuiC_ItemStack.ItemStack = _item;
				return true;
			}
		}
		return false;
	}

	// Token: 0x060076E3 RID: 30435 RVA: 0x00306D54 File Offset: 0x00304F54
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (!(name == "flameoncolor"))
			{
				if (!(name == "flameoffcolor"))
				{
					return false;
				}
				this.flameOffColor = StringParsers.ParseColor32(value);
			}
			else
			{
				this.flameOnColor = StringParsers.ParseColor32(value);
			}
			return true;
		}
		return flag;
	}

	// Token: 0x04005A9C RID: 23196
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOn;

	// Token: 0x04005A9D RID: 23197
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController button;

	// Token: 0x04005A9E RID: 23198
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController onOffLabel;

	// Token: 0x04005A9F RID: 23199
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController flameIcon;

	// Token: 0x04005AA0 RID: 23200
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 flameOnColor = new Color32(250, byte.MaxValue, 163, byte.MaxValue);

	// Token: 0x04005AA1 RID: 23201
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 flameOffColor = Color.white;

	// Token: 0x04005AA2 RID: 23202
	[PublicizedFrom(EAccessModifier.Private)]
	public string turnOff;

	// Token: 0x04005AA3 RID: 23203
	[PublicizedFrom(EAccessModifier.Private)]
	public string turnOn;

	// Token: 0x04005AA5 RID: 23205
	[PublicizedFrom(EAccessModifier.Private)]
	public float normalizedDt;
}
