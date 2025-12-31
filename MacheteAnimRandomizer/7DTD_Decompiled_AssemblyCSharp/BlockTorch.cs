using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000147 RID: 327
[Preserve]
public class BlockTorch : BlockParticle
{
	// Token: 0x06000921 RID: 2337 RVA: 0x0003F540 File Offset: 0x0003D740
	[PublicizedFrom(EAccessModifier.Protected)]
	public override Vector3 getParticleOffset(BlockValue _blockValue)
	{
		switch (_blockValue.rotation)
		{
		case 0:
			return new Vector3(0.5f, 0.7f, 0.1f);
		case 1:
			return new Vector3(0.1f, 0.7f, 0.5f);
		case 2:
			return new Vector3(0.5f, 0.7f, 0.9f);
		case 3:
			return new Vector3(0.9f, 0.7f, 0.5f);
		case 4:
			return new Vector3(0.2f, 0.7f, 0.2f);
		default:
			return Vector3.zero;
		}
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x0003F5E0 File Offset: 0x0003D7E0
	public override ItemStack OnBlockPickedUp(WorldBase _world, int _clrIdx, Vector3i _blockPos, BlockValue _blockValue, int _entityId)
	{
		ItemStack itemStack = new ItemStack((this.PickedUpItemValue == null) ? _blockValue.ToItemValue() : ItemClass.GetItem(this.PickedUpItemValue, false), 1);
		itemStack = ((this.PickupTarget == null) ? itemStack : new ItemStack(Block.GetBlockValue(this.PickupTarget, false).ToItemValue(), 1));
		itemStack.itemValue.UseTimes = (float)((int)_blockValue.meta | (int)_blockValue.meta2 << 8);
		return itemStack;
	}
}
