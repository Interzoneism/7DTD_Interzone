using System;
using UnityEngine;

// Token: 0x0200059C RID: 1436
public class MapObjectRestorePower : MapObject
{
	// Token: 0x06002E47 RID: 11847 RVA: 0x00131FFD File Offset: 0x001301FD
	public MapObjectRestorePower(Vector3 _position) : base(EnumMapObjectType.RestorePower, _position, (long)(++MapObjectRestorePower.newID), null, false)
	{
	}

	// Token: 0x06002E48 RID: 11848 RVA: 0x00131B69 File Offset: 0x0012FD69
	public override string GetMapIcon()
	{
		return "ui_game_symbol_fetch_loot";
	}

	// Token: 0x06002E49 RID: 11849 RVA: 0x00131B69 File Offset: 0x0012FD69
	public override string GetCompassIcon()
	{
		return "ui_game_symbol_fetch_loot";
	}

	// Token: 0x06002E4A RID: 11850 RVA: 0x00131B70 File Offset: 0x0012FD70
	public override string GetCompassDownIcon()
	{
		return "ui_game_symbol_fetch_loot_down";
	}

	// Token: 0x06002E4B RID: 11851 RVA: 0x00131B77 File Offset: 0x0012FD77
	public override string GetCompassUpIcon()
	{
		return "ui_game_symbol_fetch_loot_up";
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool UseUpDownCompassIcons()
	{
		return true;
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsOnCompass()
	{
		return true;
	}

	// Token: 0x06002E4E RID: 11854 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsCompassIconClamped()
	{
		return true;
	}

	// Token: 0x06002E4F RID: 11855 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool NearbyCompassBlink()
	{
		return true;
	}

	// Token: 0x06002E50 RID: 11856 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E51 RID: 11857 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsMapIconEnabled()
	{
		return false;
	}

	// Token: 0x06002E52 RID: 11858 RVA: 0x00132018 File Offset: 0x00130218
	public override Color GetMapIconColor()
	{
		if (this.IsSelected)
		{
			return new Color32(byte.MaxValue, 180, 0, byte.MaxValue);
		}
		return Color.yellow;
	}

	// Token: 0x0400249E RID: 9374
	public bool IsSelected;

	// Token: 0x0400249F RID: 9375
	[PublicizedFrom(EAccessModifier.Private)]
	public static int newID;
}
