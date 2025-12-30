using System;

// Token: 0x02000176 RID: 374
public abstract class BlockShapeBillboardRotatedAbstract : BlockShapeRotatedAbstract
{
	// Token: 0x06000B19 RID: 2841 RVA: 0x00048A12 File Offset: 0x00046C12
	public BlockShapeBillboardRotatedAbstract()
	{
		this.IsSolidCube = false;
		this.IsSolidSpace = false;
		this.IsRotatable = true;
		this.LightOpacity = 0;
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x00048A36 File Offset: 0x00046C36
	public override void Init(Block _block)
	{
		base.Init(_block);
		_block.IsDecoration = true;
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsRenderDecoration()
	{
		return true;
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool isRenderFace(BlockValue _blockValue, BlockFace _face, BlockValue _adjBlockValue)
	{
		return false;
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int getFacesDrawnFullBitfield(BlockValue _blockValue)
	{
		return 0;
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x00048A48 File Offset: 0x00046C48
	public override BlockValue RotateY(bool _bLeft, BlockValue _blockValue, int _rotCount)
	{
		for (int i = 0; i < _rotCount; i++)
		{
			byte b = _blockValue.rotation;
			if (b <= 3)
			{
				if (_bLeft)
				{
					b = ((b > 0) ? (b - 1) : 3);
				}
				else
				{
					b = ((b < 3) ? (b + 1) : 0);
				}
			}
			else if (b <= 7)
			{
				if (_bLeft)
				{
					b = ((b > 4) ? (b - 1) : 7);
				}
				else
				{
					b = ((b < 7) ? (b + 1) : 4);
				}
			}
			else if (b <= 11)
			{
				if (_bLeft)
				{
					b = ((b > 8) ? (b - 1) : 11);
				}
				else
				{
					b = ((b < 11) ? (b + 1) : 8);
				}
			}
			_blockValue.rotation = b;
		}
		return _blockValue;
	}
}
