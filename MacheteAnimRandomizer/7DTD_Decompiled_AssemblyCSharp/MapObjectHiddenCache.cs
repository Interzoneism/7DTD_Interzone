using System;
using UnityEngine;

// Token: 0x02000596 RID: 1430
public class MapObjectHiddenCache : MapObject
{
	// Token: 0x06002E13 RID: 11795 RVA: 0x00131BA8 File Offset: 0x0012FDA8
	public MapObjectHiddenCache(Vector3 _position) : base(EnumMapObjectType.HiddenCache, _position, (long)(++MapObjectHiddenCache.newID), null, false)
	{
	}

	// Token: 0x06002E14 RID: 11796 RVA: 0x00131B69 File Offset: 0x0012FD69
	public override string GetMapIcon()
	{
		return "ui_game_symbol_fetch_loot";
	}

	// Token: 0x06002E15 RID: 11797 RVA: 0x00131B69 File Offset: 0x0012FD69
	public override string GetCompassIcon()
	{
		return "ui_game_symbol_fetch_loot";
	}

	// Token: 0x06002E16 RID: 11798 RVA: 0x00131B70 File Offset: 0x0012FD70
	public override string GetCompassDownIcon()
	{
		return "ui_game_symbol_fetch_loot_down";
	}

	// Token: 0x06002E17 RID: 11799 RVA: 0x00131B77 File Offset: 0x0012FD77
	public override string GetCompassUpIcon()
	{
		return "ui_game_symbol_fetch_loot_up";
	}

	// Token: 0x06002E18 RID: 11800 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool UseUpDownCompassIcons()
	{
		return false;
	}

	// Token: 0x06002E19 RID: 11801 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsOnCompass()
	{
		return true;
	}

	// Token: 0x06002E1A RID: 11802 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsCompassIconClamped()
	{
		return true;
	}

	// Token: 0x06002E1B RID: 11803 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E1C RID: 11804 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsMapIconEnabled()
	{
		return false;
	}

	// Token: 0x06002E1D RID: 11805 RVA: 0x00131BC3 File Offset: 0x0012FDC3
	public override Color GetMapIconColor()
	{
		if (this.IsSelected)
		{
			return new Color32(byte.MaxValue, 180, 0, byte.MaxValue);
		}
		return Color.white;
	}

	// Token: 0x04002495 RID: 9365
	public bool IsSelected;

	// Token: 0x04002496 RID: 9366
	[PublicizedFrom(EAccessModifier.Private)]
	public static int newID;
}
