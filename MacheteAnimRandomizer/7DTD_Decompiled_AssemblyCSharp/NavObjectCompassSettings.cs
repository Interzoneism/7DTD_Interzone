using System;
using System.Globalization;
using UnityEngine;

// Token: 0x020006AC RID: 1708
public class NavObjectCompassSettings : NavObjectSettings
{
	// Token: 0x170004D8 RID: 1240
	// (get) Token: 0x06003267 RID: 12903 RVA: 0x00155F4A File Offset: 0x0015414A
	public bool ShowVerticalCompassIcons
	{
		get
		{
			return this.UpSpriteName != "" || this.DownSpriteName != "";
		}
	}

	// Token: 0x06003268 RID: 12904 RVA: 0x00155F70 File Offset: 0x00154170
	public override void Init()
	{
		base.Init();
		this.Properties.ParseBool("icon_clamped", ref this.IconClamped);
		this.Properties.ParseFloat("min_icon_scale", ref this.MinCompassIconScale);
		this.Properties.ParseFloat("max_icon_scale", ref this.MaxCompassIconScale);
		this.Properties.ParseFloat("min_fade_percent", ref this.MinFadePercent);
		this.MaxScaleDistance = this.MaxDistance;
		this.Properties.ParseFloat("max_scale_distance", ref this.MaxScaleDistance);
		this.Properties.ParseString("up_sprite_name", ref this.UpSpriteName);
		this.Properties.ParseString("down_sprite_name", ref this.DownSpriteName);
		this.Properties.ParseFloat("show_up_offset", ref this.ShowUpOffset);
		this.Properties.ParseFloat("show_down_offset", ref this.ShowDownOffset);
		if (this.Properties.Values.ContainsKey("hot_zone_type"))
		{
			NavObjectCompassSettings.HotZoneSettings.HotZoneTypes hotZoneTypes = NavObjectCompassSettings.HotZoneSettings.HotZoneTypes.None;
			if (!Enum.TryParse<NavObjectCompassSettings.HotZoneSettings.HotZoneTypes>(this.Properties.Values["hot_zone_type"], out hotZoneTypes))
			{
				hotZoneTypes = NavObjectCompassSettings.HotZoneSettings.HotZoneTypes.None;
			}
			if (hotZoneTypes != NavObjectCompassSettings.HotZoneSettings.HotZoneTypes.None)
			{
				this.HotZone = new NavObjectCompassSettings.HotZoneSettings();
				this.HotZone.HotZoneType = hotZoneTypes;
				this.Properties.ParseString("hot_zone_sprite", ref this.HotZone.SpriteName);
				if (this.Properties.Values.ContainsKey("hot_zone_sprite"))
				{
					this.HotZone.SpriteName = this.Properties.Values["hot_zone_sprite"];
				}
				if (this.Properties.Values.ContainsKey("hot_zone_color"))
				{
					this.HotZone.Color = StringParsers.ParseColor32(this.Properties.Values["hot_zone_color"]);
				}
				if (this.Properties.Values.ContainsKey("hot_zone_distance"))
				{
					this.HotZone.CustomDistance = StringParsers.ParseFloat(this.Properties.Values["hot_zone_distance"], 0, -1, NumberStyles.Any);
				}
			}
		}
		if (this.Properties.Values.ContainsKey("depth_offset"))
		{
			this.DepthOffset = StringParsers.ParseSInt32(this.Properties.Values["depth_offset"], 0, -1, NumberStyles.Integer);
		}
	}

	// Token: 0x0400295B RID: 10587
	public bool IconClamped;

	// Token: 0x0400295C RID: 10588
	public float MinCompassIconScale = 0.5f;

	// Token: 0x0400295D RID: 10589
	public float MaxCompassIconScale = 1.25f;

	// Token: 0x0400295E RID: 10590
	public float MaxScaleDistance = -1f;

	// Token: 0x0400295F RID: 10591
	public float MinFadePercent = -1f;

	// Token: 0x04002960 RID: 10592
	public string UpSpriteName = "";

	// Token: 0x04002961 RID: 10593
	public string DownSpriteName = "";

	// Token: 0x04002962 RID: 10594
	public float ShowUpOffset = 3f;

	// Token: 0x04002963 RID: 10595
	public float ShowDownOffset = -2f;

	// Token: 0x04002964 RID: 10596
	public int DepthOffset;

	// Token: 0x04002965 RID: 10597
	public NavObjectCompassSettings.HotZoneSettings HotZone;

	// Token: 0x020006AD RID: 1709
	public class HotZoneSettings
	{
		// Token: 0x04002966 RID: 10598
		public NavObjectCompassSettings.HotZoneSettings.HotZoneTypes HotZoneType;

		// Token: 0x04002967 RID: 10599
		public string SpriteName = "";

		// Token: 0x04002968 RID: 10600
		public Color Color = Color.white;

		// Token: 0x04002969 RID: 10601
		public float CustomDistance = -1f;

		// Token: 0x020006AE RID: 1710
		public enum HotZoneTypes
		{
			// Token: 0x0400296B RID: 10603
			None,
			// Token: 0x0400296C RID: 10604
			Treasure,
			// Token: 0x0400296D RID: 10605
			Custom
		}
	}
}
