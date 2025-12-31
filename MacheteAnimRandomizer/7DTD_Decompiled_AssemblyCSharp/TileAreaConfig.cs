using System;

// Token: 0x02000A58 RID: 2648
public struct TileAreaConfig
{
	// Token: 0x060050B5 RID: 20661 RVA: 0x0020150C File Offset: 0x001FF70C
	public void checkCoordinates(ref int _tileX, ref int _tileZ)
	{
		Vector2i vector2i = this.tileEnd - this.tileStart + new Vector2i(1, 1);
		if (_tileX < this.tileStart.x)
		{
			if (this.bWrapAroundX)
			{
				_tileX += vector2i.x;
			}
			else
			{
				_tileX = this.tileStart.x;
			}
		}
		else if (_tileX > this.tileEnd.x)
		{
			if (this.bWrapAroundX)
			{
				_tileX -= vector2i.x;
			}
			else
			{
				_tileX = this.tileEnd.x;
			}
		}
		if (_tileZ >= this.tileStart.y)
		{
			if (_tileZ > this.tileEnd.y)
			{
				if (this.bWrapAroundZ)
				{
					_tileZ -= vector2i.y;
					return;
				}
				_tileZ = this.tileEnd.y;
			}
			return;
		}
		if (this.bWrapAroundZ)
		{
			_tileZ += vector2i.y;
			return;
		}
		_tileZ = this.tileStart.y;
	}

	// Token: 0x04003DD5 RID: 15829
	public Vector2i tileStart;

	// Token: 0x04003DD6 RID: 15830
	public Vector2i tileEnd;

	// Token: 0x04003DD7 RID: 15831
	public int tileSizeInWorldUnits;

	// Token: 0x04003DD8 RID: 15832
	public bool bWrapAroundX;

	// Token: 0x04003DD9 RID: 15833
	public bool bWrapAroundZ;
}
