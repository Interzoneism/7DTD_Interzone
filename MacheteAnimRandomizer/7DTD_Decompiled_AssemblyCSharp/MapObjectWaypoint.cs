using System;
using UnityEngine;

// Token: 0x020005A3 RID: 1443
public class MapObjectWaypoint : MapObject
{
	// Token: 0x06002E84 RID: 11908 RVA: 0x00132238 File Offset: 0x00130438
	public MapObjectWaypoint(Waypoint _w) : base(EnumMapObjectType.MapMarker, _w.pos.ToVector3(), (long)MapObjectWaypoint.MapObjectWaypointKeys, null, false)
	{
		this.waypoint = _w;
		_w.MapObjectKey = (long)MapObjectWaypoint.MapObjectWaypointKeys++;
		this.name = _w.name.Text;
		this.iconName = _w.icon;
	}

	// Token: 0x06002E85 RID: 11909 RVA: 0x00132297 File Offset: 0x00130497
	public override string GetMapIcon()
	{
		return this.iconName;
	}

	// Token: 0x06002E86 RID: 11910 RVA: 0x00132297 File Offset: 0x00130497
	public override string GetCompassIcon()
	{
		return this.iconName;
	}

	// Token: 0x06002E87 RID: 11911 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsOnCompass()
	{
		return true;
	}

	// Token: 0x06002E88 RID: 11912 RVA: 0x0013229F File Offset: 0x0013049F
	public override bool IsTracked()
	{
		return this.waypoint.bTracked;
	}

	// Token: 0x06002E89 RID: 11913 RVA: 0x001322AC File Offset: 0x001304AC
	public override float GetMaxCompassDistance()
	{
		return (float)(this.waypoint.bTracked ? 1000 : 1000);
	}

	// Token: 0x06002E8A RID: 11914 RVA: 0x001322C8 File Offset: 0x001304C8
	public override float GetMinCompassDistance()
	{
		return (float)(this.waypoint.bTracked ? 250 : 0);
	}

	// Token: 0x06002E8B RID: 11915 RVA: 0x001322E0 File Offset: 0x001304E0
	public override float GetMaxCompassIconScale()
	{
		return base.GetMaxCompassIconScale();
	}

	// Token: 0x06002E8C RID: 11916 RVA: 0x001322E8 File Offset: 0x001304E8
	public override float GetMinCompassIconScale()
	{
		return base.GetMinCompassIconScale();
	}

	// Token: 0x06002E8D RID: 11917 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E8E RID: 11918 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsMapIconEnabled()
	{
		return true;
	}

	// Token: 0x06002E8F RID: 11919 RVA: 0x00131A42 File Offset: 0x0012FC42
	public override void SetPosition(Vector3 _pos)
	{
		this.position = _pos;
	}

	// Token: 0x06002E90 RID: 11920 RVA: 0x001322F0 File Offset: 0x001304F0
	public override string GetName()
	{
		return this.waypoint.name.Text;
	}

	// Token: 0x06002E91 RID: 11921 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsShowName()
	{
		return false;
	}

	// Token: 0x040024A4 RID: 9380
	[PublicizedFrom(EAccessModifier.Private)]
	public string name;

	// Token: 0x040024A5 RID: 9381
	[PublicizedFrom(EAccessModifier.Private)]
	public string iconName;

	// Token: 0x040024A6 RID: 9382
	public Waypoint waypoint;

	// Token: 0x040024A7 RID: 9383
	[PublicizedFrom(EAccessModifier.Private)]
	public static int MapObjectWaypointKeys;
}
