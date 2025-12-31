using System;
using UnityEngine;

// Token: 0x0200059B RID: 1435
public class MapObjectQuest : MapObject
{
	// Token: 0x06002E3E RID: 11838 RVA: 0x00131FA9 File Offset: 0x001301A9
	public MapObjectQuest(Vector3 _position, string newIcon = "ui_game_symbol_quest") : base(EnumMapObjectType.Quest, _position, (long)(++MapObjectQuest.newID), null, false)
	{
		this.icon = newIcon;
	}

	// Token: 0x06002E3F RID: 11839 RVA: 0x00131FCB File Offset: 0x001301CB
	public override string GetMapIcon()
	{
		return this.icon;
	}

	// Token: 0x06002E40 RID: 11840 RVA: 0x00131FCB File Offset: 0x001301CB
	public override string GetCompassIcon()
	{
		return this.icon;
	}

	// Token: 0x06002E41 RID: 11841 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsOnCompass()
	{
		return true;
	}

	// Token: 0x06002E42 RID: 11842 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsCompassIconClamped()
	{
		return true;
	}

	// Token: 0x06002E43 RID: 11843 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool NearbyCompassBlink()
	{
		return true;
	}

	// Token: 0x06002E44 RID: 11844 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E45 RID: 11845 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsMapIconEnabled()
	{
		return true;
	}

	// Token: 0x06002E46 RID: 11846 RVA: 0x00131FD3 File Offset: 0x001301D3
	public override Color GetMapIconColor()
	{
		if (this.IsSelected)
		{
			return new Color32(byte.MaxValue, 180, 0, byte.MaxValue);
		}
		return Color.white;
	}

	// Token: 0x0400249B RID: 9371
	public bool IsSelected;

	// Token: 0x0400249C RID: 9372
	[PublicizedFrom(EAccessModifier.Private)]
	public string icon;

	// Token: 0x0400249D RID: 9373
	[PublicizedFrom(EAccessModifier.Private)]
	public static int newID;
}
