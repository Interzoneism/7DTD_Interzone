using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000EB4 RID: 3764
[Preserve]
public class XUiC_WorkstationOutputWindow : XUiController
{
	// Token: 0x06007713 RID: 30483 RVA: 0x00307A20 File Offset: 0x00305C20
	public override void Init()
	{
		base.Init();
		this.outputGrid = base.GetChildByType<XUiC_WorkstationOutputGrid>();
		this.controls = base.GetChildByType<XUiC_ContainerStandardControls>();
		if (this.controls != null)
		{
			this.controls.SortPressed = delegate(PackedBoolArray _ignoredSlots)
			{
				ItemStack[] slots = StackSortUtil.CombineAndSortStacks(this.outputGrid.GetSlots(), 0, _ignoredSlots);
				this.outputGrid.SetSlots(slots);
			};
			this.controls.MoveAllowed = delegate(out XUiController _parentWindow, out XUiC_ItemStackGrid _grid, out IInventory _inventory)
			{
				_parentWindow = this;
				_grid = this.outputGrid;
				_inventory = base.xui.PlayerInventory;
				return true;
			};
			this.controls.MoveAllDone = delegate(bool _allMoved, bool _anyMoved)
			{
				if (_anyMoved)
				{
					Manager.BroadcastPlayByLocalPlayer(base.xui.playerUI.entityPlayer.position + Vector3.one * 0.5f, "UseActions/takeall1");
				}
			};
		}
	}

	// Token: 0x06007714 RID: 30484 RVA: 0x00307A98 File Offset: 0x00305C98
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (!base.xui.playerUI.windowManager.IsInputActive() && (base.xui.playerUI.playerInput.GUIActions.LeftStick.WasPressed || base.xui.playerUI.playerInput.PermanentActions.Reload.WasPressed))
		{
			this.controls.MoveAll();
		}
	}

	// Token: 0x04005AB2 RID: 23218
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WorkstationOutputGrid outputGrid;

	// Token: 0x04005AB3 RID: 23219
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ContainerStandardControls controls;
}
