using System;
using UnityEngine;

// Token: 0x02000593 RID: 1427
public class MapObjectAnimal : MapObject
{
	// Token: 0x06002DF0 RID: 11760 RVA: 0x001318FE File Offset: 0x0012FAFE
	public MapObjectAnimal(Entity _entity) : base(EnumMapObjectType.Entity, Vector3.zero, (long)_entity.entityId, _entity, false)
	{
	}

	// Token: 0x06002DF1 RID: 11761 RVA: 0x00131915 File Offset: 0x0012FB15
	public MapObjectAnimal(MapObjectAnimal _other) : base(EnumMapObjectType.Entity, _other.position, (long)_other.entity.entityId, _other.entity, false)
	{
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x00131938 File Offset: 0x0012FB38
	public override void RefreshData()
	{
		if (this.type == EnumMapObjectType.Entity && this.entity != null)
		{
			if (!((EntityAlive)this.entity).IsAlive())
			{
				this.isTracked = false;
				return;
			}
			EntityPlayerLocal primaryPlayer = this.entity.world.GetPrimaryPlayer();
			if (primaryPlayer != null && EffectManager.GetValue(PassiveEffects.Tracking, null, 0f, primaryPlayer, null, this.entity.EntityTags, true, true, true, true, true, 1, true, false) > 0f)
			{
				this.isTracked = true;
				return;
			}
		}
		this.isTracked = false;
	}

	// Token: 0x06002DF3 RID: 11763 RVA: 0x001319C7 File Offset: 0x0012FBC7
	public override bool IsOnCompass()
	{
		return this.isTracked;
	}

	// Token: 0x06002DF4 RID: 11764 RVA: 0x001319CF File Offset: 0x0012FBCF
	public override string GetMapIcon()
	{
		if (!this.isTracked)
		{
			return this.entity.GetMapIcon();
		}
		return this.entity.GetTrackerIcon();
	}

	// Token: 0x06002DF5 RID: 11765 RVA: 0x001319F0 File Offset: 0x0012FBF0
	public override string GetCompassIcon()
	{
		if (!this.isTracked)
		{
			return this.entity.GetCompassIcon();
		}
		return this.entity.GetTrackerIcon();
	}

	// Token: 0x06002DF6 RID: 11766 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool NearbyCompassBlink()
	{
		return true;
	}

	// Token: 0x06002DF7 RID: 11767 RVA: 0x001319C7 File Offset: 0x0012FBC7
	public override bool IsMapIconEnabled()
	{
		return this.isTracked;
	}

	// Token: 0x06002DF8 RID: 11768 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public override float GetMaxCompassIconScale()
	{
		return 1f;
	}

	// Token: 0x06002DF9 RID: 11769 RVA: 0x00131A11 File Offset: 0x0012FC11
	public override float GetMinCompassIconScale()
	{
		return 0.6f;
	}

	// Token: 0x06002DFA RID: 11770 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool UseUpDownCompassIcons()
	{
		return false;
	}

	// Token: 0x06002DFB RID: 11771 RVA: 0x00131A18 File Offset: 0x0012FC18
	public override float GetMaxCompassDistance()
	{
		return 500f;
	}

	// Token: 0x06002DFC RID: 11772 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsShowName()
	{
		return false;
	}

	// Token: 0x0400248F RID: 9359
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isTracked;
}
