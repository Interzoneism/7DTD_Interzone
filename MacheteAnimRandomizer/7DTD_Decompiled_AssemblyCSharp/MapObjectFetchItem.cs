using System;
using UnityEngine;

// Token: 0x02000595 RID: 1429
public class MapObjectFetchItem : MapObject
{
	// Token: 0x06002E07 RID: 11783 RVA: 0x00131B4E File Offset: 0x0012FD4E
	public MapObjectFetchItem(Vector3 _position) : base(EnumMapObjectType.FetchItem, _position, (long)(++MapObjectFetchItem.newID), null, false)
	{
	}

	// Token: 0x06002E08 RID: 11784 RVA: 0x00131B69 File Offset: 0x0012FD69
	public override string GetMapIcon()
	{
		return "ui_game_symbol_fetch_loot";
	}

	// Token: 0x06002E09 RID: 11785 RVA: 0x00131B69 File Offset: 0x0012FD69
	public override string GetCompassIcon()
	{
		return "ui_game_symbol_fetch_loot";
	}

	// Token: 0x06002E0A RID: 11786 RVA: 0x00131B70 File Offset: 0x0012FD70
	public override string GetCompassDownIcon()
	{
		return "ui_game_symbol_fetch_loot_down";
	}

	// Token: 0x06002E0B RID: 11787 RVA: 0x00131B77 File Offset: 0x0012FD77
	public override string GetCompassUpIcon()
	{
		return "ui_game_symbol_fetch_loot_up";
	}

	// Token: 0x06002E0C RID: 11788 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool UseUpDownCompassIcons()
	{
		return true;
	}

	// Token: 0x06002E0D RID: 11789 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsOnCompass()
	{
		return true;
	}

	// Token: 0x06002E0E RID: 11790 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsCompassIconClamped()
	{
		return true;
	}

	// Token: 0x06002E0F RID: 11791 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool NearbyCompassBlink()
	{
		return true;
	}

	// Token: 0x06002E10 RID: 11792 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E11 RID: 11793 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsMapIconEnabled()
	{
		return false;
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x00131B7E File Offset: 0x0012FD7E
	public override Color GetMapIconColor()
	{
		if (this.IsSelected)
		{
			return new Color32(byte.MaxValue, 180, 0, byte.MaxValue);
		}
		return Color.white;
	}

	// Token: 0x04002493 RID: 9363
	public bool IsSelected;

	// Token: 0x04002494 RID: 9364
	[PublicizedFrom(EAccessModifier.Private)]
	public static int newID;
}
