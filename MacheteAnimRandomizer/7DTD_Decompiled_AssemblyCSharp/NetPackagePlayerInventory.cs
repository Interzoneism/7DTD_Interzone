using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000772 RID: 1906
[Preserve]
public class NetPackagePlayerInventory : NetPackage
{
	// Token: 0x0600376F RID: 14191 RVA: 0x0016ACA8 File Offset: 0x00168EA8
	public NetPackagePlayerInventory Setup(EntityPlayerLocal _player, bool _changedToolbelt, bool _changedBag, bool _changedEquipment, bool _changedDragAndDropItem)
	{
		if (_changedToolbelt)
		{
			this.toolbelt = ((_player.AttachedToEntity != null && _player.saveInventory != null) ? _player.saveInventory.CloneItemStack() : _player.inventory.CloneItemStack());
		}
		if (_changedBag)
		{
			this.bag = _player.bag.GetSlots();
		}
		if (_changedEquipment)
		{
			this.equipment = _player.equipment.Clone();
		}
		if (_changedDragAndDropItem)
		{
			this.dragAndDropItem = _player.DragAndDropItem.Clone();
		}
		return this;
	}

	// Token: 0x170005A2 RID: 1442
	// (get) Token: 0x06003770 RID: 14192 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x06003771 RID: 14193 RVA: 0x0016AD2C File Offset: 0x00168F2C
	public override void read(PooledBinaryReader _reader)
	{
		if (_reader.ReadBoolean())
		{
			this.toolbelt = GameUtils.ReadItemStack(_reader);
		}
		if (_reader.ReadBoolean())
		{
			this.bag = GameUtils.ReadItemStack(_reader);
		}
		if (_reader.ReadBoolean())
		{
			ItemValue[] array = GameUtils.ReadItemValueArray(_reader);
			this.equipment = new Equipment();
			int num = Utils.FastMin(array.Length, this.equipment.GetSlotCount());
			for (int i = 0; i < num; i++)
			{
				this.equipment.SetSlotItemRaw(i, array[i]);
			}
			for (int j = 0; j < num; j++)
			{
				this.equipment.SetCosmeticSlot(j, _reader.ReadInt32());
			}
			int num2 = _reader.ReadInt32();
			for (int k = 0; k < num2; k++)
			{
				this.equipment.m_unlockedCosmetics.Add(_reader.ReadInt32());
			}
		}
		if (_reader.ReadBoolean())
		{
			ItemStack[] array2 = GameUtils.ReadItemStack(_reader);
			if (array2 != null && array2.Length != 0)
			{
				this.dragAndDropItem = array2[0];
			}
		}
	}

	// Token: 0x06003772 RID: 14194 RVA: 0x0016AE20 File Offset: 0x00169020
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.toolbelt != null);
		if (this.toolbelt != null)
		{
			GameUtils.WriteItemStack(_writer, this.toolbelt);
		}
		_writer.Write(this.bag != null);
		if (this.bag != null)
		{
			GameUtils.WriteItemStack(_writer, this.bag);
		}
		_writer.Write(this.equipment != null);
		if (this.equipment != null)
		{
			GameUtils.WriteItemValueArray(_writer, this.equipment.GetItems());
			int[] cosmeticIDs = this.equipment.GetCosmeticIDs();
			for (int i = 0; i < cosmeticIDs.Length; i++)
			{
				_writer.Write(cosmeticIDs[i]);
			}
			List<int> unlockedCosmetics = this.equipment.m_unlockedCosmetics;
			_writer.Write(unlockedCosmetics.Count);
			for (int j = 0; j < unlockedCosmetics.Count; j++)
			{
				_writer.Write(unlockedCosmetics[j]);
			}
		}
		_writer.Write(this.dragAndDropItem != null);
		if (this.dragAndDropItem != null)
		{
			GameUtils.WriteItemStack(_writer, new ItemStack[]
			{
				this.dragAndDropItem
			});
		}
	}

	// Token: 0x06003773 RID: 14195 RVA: 0x0016AF28 File Offset: 0x00169128
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		PlayerDataFile latestPlayerData = base.Sender.latestPlayerData;
		if (this.toolbelt != null)
		{
			latestPlayerData.inventory = this.toolbelt;
		}
		if (this.bag != null)
		{
			latestPlayerData.bag = this.bag;
		}
		if (this.equipment != null)
		{
			latestPlayerData.equipment = this.equipment;
		}
		if (this.dragAndDropItem != null)
		{
			latestPlayerData.dragAndDropItem = this.dragAndDropItem;
		}
		latestPlayerData.bModifiedSinceLastSave = true;
	}

	// Token: 0x06003774 RID: 14196 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04002CEF RID: 11503
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] toolbelt;

	// Token: 0x04002CF0 RID: 11504
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack[] bag;

	// Token: 0x04002CF1 RID: 11505
	[PublicizedFrom(EAccessModifier.Private)]
	public Equipment equipment;

	// Token: 0x04002CF2 RID: 11506
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack dragAndDropItem;
}
