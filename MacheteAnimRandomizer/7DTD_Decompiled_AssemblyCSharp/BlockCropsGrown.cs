using System;
using System.Collections.Generic;
using System.Globalization;
using Audio;
using UnityEngine.Scripting;

// Token: 0x020000F7 RID: 247
[Preserve]
public class BlockCropsGrown : BlockPlant
{
	// Token: 0x06000660 RID: 1632 RVA: 0x0002D320 File Offset: 0x0002B520
	public BlockCropsGrown()
	{
		this.CanPickup = true;
		this.IsRandomlyTick = false;
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0002D37C File Offset: 0x0002B57C
	public override void LateInit()
	{
		base.LateInit();
		if (base.Properties.Values.ContainsKey(BlockCropsGrown.PropBlockBeforeHarvesting))
		{
			this.babyPlant = ItemClass.GetItem(base.Properties.Values[BlockCropsGrown.PropBlockBeforeHarvesting], false).ToBlockValue();
		}
		if (base.Properties.Values.ContainsKey(BlockCropsGrown.PropGrowingBonusHarvestDivisor))
		{
			this.bonusHarvestDivisor = StringParsers.ParseFloat(base.Properties.Values[BlockCropsGrown.PropGrowingBonusHarvestDivisor], 0, -1, NumberStyles.Any);
		}
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x0002D40A File Offset: 0x0002B60A
	[PublicizedFrom(EAccessModifier.Private)]
	public void setPlantBackToBaby(WorldBase _world, int _cIdx, Vector3i _myBlockPos, BlockValue _blockValue)
	{
		this.babyPlant.rotation = _blockValue.rotation;
		_world.SetBlockRPC(_cIdx, _myBlockPos, this.babyPlant);
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x0002D42C File Offset: 0x0002B62C
	public override string GetActivationText(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		List<Block.SItemDropProb> list = null;
		int num;
		if (this.itemsToDrop.TryGetValue(EnumDropEvent.Harvest, out list) && (num = Utils.FastMax(0, list[0].minCount)) > 0)
		{
			if (_blockPos.y > 1)
			{
				int num2 = (int)((float)_world.GetBlock(_blockPos - Vector3i.up).Block.blockMaterial.FertileLevel / this.bonusHarvestDivisor);
				num += num2;
			}
			return string.Format(Localization.Get("pickupCrops", false), num, Localization.Get(list[0].name, false));
		}
		return null;
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x0002D4CC File Offset: 0x0002B6CC
	public override bool OnBlockActivated(WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		List<Block.SItemDropProb> list = null;
		int num;
		if (this.itemsToDrop.TryGetValue(EnumDropEvent.Harvest, out list) && (num = Utils.FastMax(0, list[0].minCount)) > 0)
		{
			if (_blockPos.y > 1)
			{
				int num2 = (int)((float)_world.GetBlock(_blockPos - Vector3i.up).Block.blockMaterial.FertileLevel / this.bonusHarvestDivisor);
				num += num2;
			}
			ItemStack itemStack = new ItemStack(ItemClass.GetItem(list[0].name, false), num);
			ItemStack @is = itemStack.Clone();
			if ((_player.inventory.CanStackNoEmpty(itemStack) && _player.inventory.AddItem(itemStack)) || _player.bag.AddItem(itemStack) || _player.inventory.AddItem(itemStack))
			{
				_player.PlayOneShot("item_plant_pickup", false, false, false, null);
				this.setPlantBackToBaby(_world, _cIdx, _blockPos, _blockValue);
				QuestEventManager.Current.BlockPickedUp(_blockValue.Block.GetBlockName(), _blockPos);
				_player.AddUIHarvestingItem(@is, false);
				return true;
			}
			Manager.PlayInsidePlayerHead("ui_denied", -1, 0f, false, false);
		}
		return false;
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool HasBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return true;
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x0002D5F3 File Offset: 0x0002B7F3
	public override BlockActivationCommand[] GetBlockActivationCommands(WorldBase _world, BlockValue _blockValue, int _clrIdx, Vector3i _blockPos, EntityAlive _entityFocusing)
	{
		return this.cmds;
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x0002D5FB File Offset: 0x0002B7FB
	public override bool OnBlockActivated(string _commandName, WorldBase _world, int _cIdx, Vector3i _blockPos, BlockValue _blockValue, EntityPlayerLocal _player)
	{
		if (_commandName == "pickup")
		{
			this.OnBlockActivated(_world, _cIdx, _blockPos, _blockValue, _player);
			return true;
		}
		return false;
	}

	// Token: 0x04000784 RID: 1924
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropBlockBeforeHarvesting = "BlockBeforeHarvesting";

	// Token: 0x04000785 RID: 1925
	[PublicizedFrom(EAccessModifier.Protected)]
	public static string PropGrowingBonusHarvestDivisor = "CropsGrown.BonusHarvestDivisor";

	// Token: 0x04000786 RID: 1926
	[PublicizedFrom(EAccessModifier.Private)]
	public new BlockActivationCommand[] cmds = new BlockActivationCommand[]
	{
		new BlockActivationCommand("pickup", "hand", true, false, null)
	};

	// Token: 0x04000787 RID: 1927
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue babyPlant = BlockValue.Air;

	// Token: 0x04000788 RID: 1928
	[PublicizedFrom(EAccessModifier.Private)]
	public float bonusHarvestDivisor = float.MaxValue;
}
