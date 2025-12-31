using System;
using UnityEngine;

// Token: 0x0200059D RID: 1437
public class MapObjectSleeperVolume : MapObject
{
	// Token: 0x06002E53 RID: 11859 RVA: 0x00132042 File Offset: 0x00130242
	public MapObjectSleeperVolume(Vector3 _position) : base(EnumMapObjectType.SleeperVolume, _position, (long)(++MapObjectSleeperVolume.newID), null, false)
	{
	}

	// Token: 0x06002E54 RID: 11860 RVA: 0x00132064 File Offset: 0x00130264
	public override string GetMapIcon()
	{
		return "ui_game_symbol_enemy_dot";
	}

	// Token: 0x06002E55 RID: 11861 RVA: 0x00132064 File Offset: 0x00130264
	public override string GetCompassIcon()
	{
		return "ui_game_symbol_enemy_dot";
	}

	// Token: 0x06002E56 RID: 11862 RVA: 0x0013206B File Offset: 0x0013026B
	public override string GetCompassDownIcon()
	{
		return "ui_game_symbol_enemy_dot_down";
	}

	// Token: 0x06002E57 RID: 11863 RVA: 0x00132072 File Offset: 0x00130272
	public override string GetCompassUpIcon()
	{
		return "ui_game_symbol_enemy_dot_up";
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool UseUpDownCompassIcons()
	{
		return true;
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x00132079 File Offset: 0x00130279
	public override bool IsOnCompass()
	{
		return this.IsShowing;
	}

	// Token: 0x06002E5A RID: 11866 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsCompassIconClamped()
	{
		return true;
	}

	// Token: 0x06002E5B RID: 11867 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsMapIconEnabled()
	{
		return false;
	}

	// Token: 0x06002E5C RID: 11868 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public override float GetMaxCompassIconScale()
	{
		return 1f;
	}

	// Token: 0x06002E5D RID: 11869 RVA: 0x00131A11 File Offset: 0x0012FC11
	public override float GetMinCompassIconScale()
	{
		return 0.6f;
	}

	// Token: 0x06002E5E RID: 11870 RVA: 0x00132081 File Offset: 0x00130281
	public override float GetMaxCompassDistance()
	{
		return 32f;
	}

	// Token: 0x06002E5F RID: 11871 RVA: 0x00132088 File Offset: 0x00130288
	public override Color GetMapIconColor()
	{
		return new Color32(byte.MaxValue, 180, 0, byte.MaxValue);
	}

	// Token: 0x040024A0 RID: 9376
	public bool IsShowing = true;

	// Token: 0x040024A1 RID: 9377
	[PublicizedFrom(EAccessModifier.Private)]
	public static int newID;
}
