using System;
using UnityEngine.Scripting;

// Token: 0x02000CA3 RID: 3235
[Preserve]
public class XUiC_ItemDronePartStack : XUiC_ItemPartStack
{
	// Token: 0x060063D5 RID: 25557 RVA: 0x00287534 File Offset: 0x00285734
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CanSwap(ItemStack stack)
	{
		bool result = base.CanSwap(stack);
		if (base.ItemClass != null && this.itemClass.HasAnyTags(EntityDrone.cStorageModifierTags))
		{
			EntityDrone currentVehicleEntity = ((XUiC_DroneWindowGroup)this.windowGroup.Controller).CurrentVehicleEntity;
			if (!currentVehicleEntity.CanRemoveExtraStorage())
			{
				currentVehicleEntity.NotifyToManyStoredItems();
				return false;
			}
		}
		return result;
	}

	// Token: 0x060063D6 RID: 25558 RVA: 0x0028758C File Offset: 0x0028578C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool CanRemove()
	{
		bool result = base.CanRemove();
		if (this.itemClass != null && this.itemClass.HasAnyTags(EntityDrone.cStorageModifierTags))
		{
			EntityDrone currentVehicleEntity = ((XUiC_DroneWindowGroup)this.windowGroup.Controller).CurrentVehicleEntity;
			if (!currentVehicleEntity.CanRemoveExtraStorage())
			{
				if (base.xui.playerUI.CursorController.GetMouseButtonUp(UICamera.MouseButton.LeftButton))
				{
					currentVehicleEntity.NotifyToManyStoredItems();
				}
				return false;
			}
		}
		return result;
	}
}
