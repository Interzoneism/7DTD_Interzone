using System;
using UnityEngine;

// Token: 0x02000597 RID: 1431
public class MapObjectLandClaim : MapObject
{
	// Token: 0x06002E1E RID: 11806 RVA: 0x00131BED File Offset: 0x0012FDED
	public MapObjectLandClaim(Vector3 _position, Entity _entity) : base(EnumMapObjectType.LandClaim, _position, (long)MapObjectLandClaim.MapObjectLandCLaimKeys++, _entity, false)
	{
	}

	// Token: 0x06002E1F RID: 11807 RVA: 0x00131C08 File Offset: 0x0012FE08
	public override string GetMapIcon()
	{
		return "ui_game_symbol_brick";
	}

	// Token: 0x06002E20 RID: 11808 RVA: 0x00131C08 File Offset: 0x0012FE08
	public override string GetCompassIcon()
	{
		return "ui_game_symbol_brick";
	}

	// Token: 0x06002E21 RID: 11809 RVA: 0x00131C0F File Offset: 0x0012FE0F
	public override bool IsOnCompass()
	{
		return this.IsMapIconEnabled();
	}

	// Token: 0x06002E22 RID: 11810 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLayerForMapIcon()
	{
		return 0;
	}

	// Token: 0x06002E23 RID: 11811 RVA: 0x00131C17 File Offset: 0x0012FE17
	public override bool IsMapIconEnabled()
	{
		return this.entity != null && this.entity is EntityPlayerLocal;
	}

	// Token: 0x06002E24 RID: 11812 RVA: 0x000B195B File Offset: 0x000AFB5B
	public override Color GetMapIconColor()
	{
		return Color.white;
	}

	// Token: 0x04002497 RID: 9367
	[PublicizedFrom(EAccessModifier.Private)]
	public static int MapObjectLandCLaimKeys;
}
