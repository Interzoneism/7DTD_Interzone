using System;
using UnityEngine.Scripting;

// Token: 0x02000742 RID: 1858
[Preserve]
public class NetPackageEntitySpawnResponse : NetPackage
{
	// Token: 0x0600364F RID: 13903 RVA: 0x001668B7 File Offset: 0x00164AB7
	public NetPackageEntitySpawnResponse Setup(bool _success, ItemValue _itemValue)
	{
		this.success = _success;
		this.itemValue = _itemValue;
		return this;
	}

	// Token: 0x06003650 RID: 13904 RVA: 0x001668C8 File Offset: 0x00164AC8
	public override void read(PooledBinaryReader _reader)
	{
		this.success = _reader.ReadBoolean();
		this.itemValue = new ItemValue();
		this.itemValue.Read(_reader);
	}

	// Token: 0x06003651 RID: 13905 RVA: 0x001668ED File Offset: 0x00164AED
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.success);
		this.itemValue.Write(_writer);
	}

	// Token: 0x06003652 RID: 13906 RVA: 0x00166910 File Offset: 0x00164B10
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		EntityPlayerLocal primaryPlayer = _world.GetPrimaryPlayer();
		bool flag = this.itemValue.ItemClass.HasAnyTags(FastTags<TagGroup.Global>.Parse("vehicle"));
		bool flag2 = this.itemValue.ItemClass.HasAnyTags(FastTags<TagGroup.Global>.Parse("drone"));
		bool flag3 = this.itemValue.ItemClass.HasAnyTags(FastTags<TagGroup.Global>.Parse("turretRanged")) || this.itemValue.ItemClass.HasAnyTags(FastTags<TagGroup.Global>.Parse("turretMelee"));
		if (this.success)
		{
			if (flag)
			{
				if (primaryPlayer.inventory.holdingItem.Equals(this.itemValue.ItemClass))
				{
					ItemActionSpawnVehicle itemActionSpawnVehicle = primaryPlayer.inventory.holdingItem.Actions[1] as ItemActionSpawnVehicle;
					if (itemActionSpawnVehicle != null)
					{
						itemActionSpawnVehicle.ClearPreview(primaryPlayer.inventory.holdingItemData.actionData[1]);
					}
				}
				primaryPlayer.inventory.DecItem(this.itemValue, 1, false, null);
				primaryPlayer.PlayOneShot("placeblock", false, false, false, null);
				return;
			}
			if (primaryPlayer.inventory.holdingItem.Equals(this.itemValue.ItemClass))
			{
				ItemActionSpawnTurret itemActionSpawnTurret = primaryPlayer.inventory.holdingItem.Actions[1] as ItemActionSpawnTurret;
				if (itemActionSpawnTurret != null)
				{
					itemActionSpawnTurret.ClearPreview(primaryPlayer.inventory.holdingItemData.actionData[1]);
				}
			}
			primaryPlayer.inventory.DecItem(this.itemValue, 1, false, null);
			primaryPlayer.PlayOneShot("placeblock", false, false, false, null);
			return;
		}
		else
		{
			if (flag)
			{
				GameManager.ShowTooltip(primaryPlayer, "uiCannotAddVehicle", false, false, 0f);
				return;
			}
			if (flag2)
			{
				GameManager.ShowTooltip(primaryPlayer, "uiCannotAddDrone", false, false, 0f);
				return;
			}
			if (flag3)
			{
				GameManager.ShowTooltip(primaryPlayer, "uiCannotAddTurret", false, false, 0f);
			}
			return;
		}
	}

	// Token: 0x06003653 RID: 13907 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002C16 RID: 11286
	public bool success;

	// Token: 0x04002C17 RID: 11287
	public ItemValue itemValue;
}
