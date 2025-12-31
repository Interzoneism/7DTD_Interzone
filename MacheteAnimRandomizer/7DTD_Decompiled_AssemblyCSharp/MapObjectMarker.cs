using System;
using UnityEngine;

// Token: 0x0200059A RID: 1434
public class MapObjectMarker : MapObject
{
	// Token: 0x06002E35 RID: 11829 RVA: 0x00131F8E File Offset: 0x0013018E
	public MapObjectMarker(Vector3 _position, long _key) : base(EnumMapObjectType.MapQuickMarker, _position, _key, null, false)
	{
	}

	// Token: 0x06002E36 RID: 11830 RVA: 0x00131F9B File Offset: 0x0013019B
	public override string GetMapIcon()
	{
		return "ui_game_symbol_map_waypoint_set";
	}

	// Token: 0x06002E37 RID: 11831 RVA: 0x00131F9B File Offset: 0x0013019B
	public override string GetCompassIcon()
	{
		return "ui_game_symbol_map_waypoint_set";
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x00131FA2 File Offset: 0x001301A2
	public override Color GetMapIconColor()
	{
		return Color.red;
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsOnCompass()
	{
		return true;
	}

	// Token: 0x06002E3A RID: 11834 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E3B RID: 11835 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsMapIconEnabled()
	{
		return true;
	}

	// Token: 0x06002E3C RID: 11836 RVA: 0x00131A42 File Offset: 0x0012FC42
	public override void SetPosition(Vector3 _pos)
	{
		this.position = _pos;
	}

	// Token: 0x06002E3D RID: 11837 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsCenterOnLeftBottomCorner()
	{
		return true;
	}
}
