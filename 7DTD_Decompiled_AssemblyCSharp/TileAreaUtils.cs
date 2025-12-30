using System;
using System.Runtime.CompilerServices;

// Token: 0x02000A59 RID: 2649
public static class TileAreaUtils
{
	// Token: 0x060050B6 RID: 20662 RVA: 0x002015FC File Offset: 0x001FF7FC
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static uint MakeKey(int _tileX, int _tileZ)
	{
		return (uint)(_tileX << 16 | (_tileZ & 65535));
	}

	// Token: 0x060050B7 RID: 20663 RVA: 0x0020160A File Offset: 0x001FF80A
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetTileXPos(uint _key)
	{
		return (int)_key >> 16;
	}

	// Token: 0x060050B8 RID: 20664 RVA: 0x00201610 File Offset: 0x001FF810
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetTileZPos(uint _key)
	{
		return (int)((short)_key);
	}
}
