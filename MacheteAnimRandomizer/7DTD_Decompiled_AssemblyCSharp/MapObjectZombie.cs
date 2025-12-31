using System;
using UnityEngine;

// Token: 0x020005A4 RID: 1444
public class MapObjectZombie : MapObject
{
	// Token: 0x06002E92 RID: 11922 RVA: 0x001318FE File Offset: 0x0012FAFE
	public MapObjectZombie(Entity _entity) : base(EnumMapObjectType.Entity, Vector3.zero, (long)_entity.entityId, _entity, false)
	{
	}

	// Token: 0x06002E93 RID: 11923 RVA: 0x00131915 File Offset: 0x0012FB15
	public MapObjectZombie(MapObjectZombie _other) : base(EnumMapObjectType.Entity, _other.position, (long)_other.entity.entityId, _other.entity, false)
	{
	}

	// Token: 0x06002E94 RID: 11924 RVA: 0x00132302 File Offset: 0x00130502
	public override void RefreshData()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity != null)
		{
			EntityAlive entityAlive = (EntityAlive)this.entity;
			this.entity.world.GetPrimaryPlayer();
		}
		this.trackingType = MapObjectZombie.TrackingTypes.None;
	}

	// Token: 0x06002E95 RID: 11925 RVA: 0x0013233E File Offset: 0x0013053E
	public override bool IsOnCompass()
	{
		return this.trackingType > MapObjectZombie.TrackingTypes.None;
	}

	// Token: 0x06002E96 RID: 11926 RVA: 0x00132349 File Offset: 0x00130549
	public override string GetMapIcon()
	{
		if (this.trackingType != MapObjectZombie.TrackingTypes.Tracking)
		{
			return this.entity.GetMapIcon();
		}
		return this.entity.GetTrackerIcon();
	}

	// Token: 0x06002E97 RID: 11927 RVA: 0x0013236B File Offset: 0x0013056B
	public override string GetCompassIcon()
	{
		if (this.trackingType != MapObjectZombie.TrackingTypes.Tracking)
		{
			return this.entity.GetCompassIcon();
		}
		return this.entity.GetTrackerIcon();
	}

	// Token: 0x06002E98 RID: 11928 RVA: 0x0013238D File Offset: 0x0013058D
	public override bool UseUpDownCompassIcons()
	{
		return this.trackingType == MapObjectZombie.TrackingTypes.Quest;
	}

	// Token: 0x06002E99 RID: 11929 RVA: 0x0013238D File Offset: 0x0013058D
	public override bool IsCompassIconClamped()
	{
		return this.trackingType == MapObjectZombie.TrackingTypes.Quest;
	}

	// Token: 0x06002E9A RID: 11930 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool NearbyCompassBlink()
	{
		return true;
	}

	// Token: 0x06002E9B RID: 11931 RVA: 0x00132398 File Offset: 0x00130598
	public override bool IsMapIconEnabled()
	{
		return this.trackingType == MapObjectZombie.TrackingTypes.Tracking;
	}

	// Token: 0x06002E9C RID: 11932 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public override float GetMaxCompassIconScale()
	{
		return 1f;
	}

	// Token: 0x06002E9D RID: 11933 RVA: 0x00131A11 File Offset: 0x0012FC11
	public override float GetMinCompassIconScale()
	{
		return 0.6f;
	}

	// Token: 0x06002E9E RID: 11934 RVA: 0x001323A3 File Offset: 0x001305A3
	public override Color GetMapIconColor()
	{
		if (this.trackingType != MapObjectZombie.TrackingTypes.Quest)
		{
			return this.entity.GetMapIconColor();
		}
		return Color.red;
	}

	// Token: 0x06002E9F RID: 11935 RVA: 0x001323BF File Offset: 0x001305BF
	public override float GetMaxCompassDistance()
	{
		return (float)((this.trackingType == MapObjectZombie.TrackingTypes.Quest) ? 32 : 100);
	}

	// Token: 0x06002EA0 RID: 11936 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsShowName()
	{
		return false;
	}

	// Token: 0x040024A8 RID: 9384
	[PublicizedFrom(EAccessModifier.Private)]
	public MapObjectZombie.TrackingTypes trackingType;

	// Token: 0x020005A5 RID: 1445
	[PublicizedFrom(EAccessModifier.Private)]
	public enum TrackingTypes
	{
		// Token: 0x040024AA RID: 9386
		None,
		// Token: 0x040024AB RID: 9387
		Tracking,
		// Token: 0x040024AC RID: 9388
		Quest
	}
}
