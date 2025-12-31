using System;
using UnityEngine;

// Token: 0x0200059E RID: 1438
public class MapObjectSleepingBag : MapObject
{
	// Token: 0x06002E60 RID: 11872 RVA: 0x001320A4 File Offset: 0x001302A4
	public MapObjectSleepingBag(Vector3 _position, Entity _entity) : base(EnumMapObjectType.SleepingBag, _position, (long)_entity.entityId, _entity, false)
	{
	}

	// Token: 0x06002E61 RID: 11873 RVA: 0x001320B7 File Offset: 0x001302B7
	public override string GetMapIcon()
	{
		return "ui_game_symbol_map_bed";
	}

	// Token: 0x06002E62 RID: 11874 RVA: 0x001320B7 File Offset: 0x001302B7
	public override string GetCompassIcon()
	{
		return "ui_game_symbol_map_bed";
	}

	// Token: 0x06002E63 RID: 11875 RVA: 0x001320BE File Offset: 0x001302BE
	public override bool IsOnCompass()
	{
		return this.IsMapIconEnabled() && this.entity is EntityPlayerLocal;
	}

	// Token: 0x06002E64 RID: 11876 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E65 RID: 11877 RVA: 0x001320D8 File Offset: 0x001302D8
	public override bool IsMapIconEnabled()
	{
		return !(this.entity != null) || !(this.entity is EntityPlayer) || this.entity is EntityPlayerLocal || ((EntityPlayer)this.entity).IsFriendOfLocalPlayer;
	}

	// Token: 0x06002E66 RID: 11878 RVA: 0x00132118 File Offset: 0x00130318
	public override Color GetMapIconColor()
	{
		if (this.entity != null && this.entity is EntityPlayer && !(this.entity is EntityPlayerLocal))
		{
			bool isFriendOfLocalPlayer = ((EntityPlayer)this.entity).IsFriendOfLocalPlayer;
			return Color.green * 0.75f;
		}
		return Color.white;
	}
}
