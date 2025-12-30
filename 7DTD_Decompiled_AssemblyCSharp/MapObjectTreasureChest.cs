using System;
using UnityEngine;

// Token: 0x020005A0 RID: 1440
public class MapObjectTreasureChest : MapObject
{
	// Token: 0x06002E70 RID: 11888 RVA: 0x0013218F File Offset: 0x0013038F
	public MapObjectTreasureChest(Vector3 _position, int _questCode, int _defaultRadius) : base(EnumMapObjectType.TreasureChest, _position, (long)_questCode, null, false)
	{
		this.DefaultRadius = _defaultRadius;
	}

	// Token: 0x06002E71 RID: 11889 RVA: 0x001321AB File Offset: 0x001303AB
	public override string GetMapIcon()
	{
		return "ui_game_symbol_treasure";
	}

	// Token: 0x06002E72 RID: 11890 RVA: 0x001321AB File Offset: 0x001303AB
	public override string GetCompassIcon()
	{
		return "ui_game_symbol_treasure";
	}

	// Token: 0x06002E73 RID: 11891 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsOnCompass()
	{
		return true;
	}

	// Token: 0x06002E74 RID: 11892 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsCompassIconClamped()
	{
		return true;
	}

	// Token: 0x06002E75 RID: 11893 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E76 RID: 11894 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsMapIconEnabled()
	{
		return true;
	}

	// Token: 0x06002E77 RID: 11895 RVA: 0x001321B2 File Offset: 0x001303B2
	public override Color GetMapIconColor()
	{
		if (this.IsSelected)
		{
			return new Color32(222, 206, 163, byte.MaxValue);
		}
		return Color.white;
	}

	// Token: 0x040024A2 RID: 9378
	public bool IsSelected;

	// Token: 0x040024A3 RID: 9379
	public int DefaultRadius = 1;
}
