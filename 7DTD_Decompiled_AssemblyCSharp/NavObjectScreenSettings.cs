using System;
using UnityEngine;

// Token: 0x020006AF RID: 1711
public class NavObjectScreenSettings : NavObjectSettings
{
	// Token: 0x0600326B RID: 12907 RVA: 0x00156250 File Offset: 0x00154450
	public override void Init()
	{
		base.Init();
		this.Properties.ParseEnum<NavObjectScreenSettings.ShowTextTypes>("text_type", ref this.ShowTextType);
		this.Properties.ParseFloat("fade_percent", ref this.FadePercent);
		this.Properties.ParseBool("show_offscreen", ref this.ShowOffScreen);
		this.Properties.ParseBool("use_head_offset", ref this.UseHeadOffset);
		this.Properties.ParseFloat("sprite_size", ref this.SpriteSize);
		this.Properties.ParseColor("sprite_fill_color", ref this.SpriteFillColor);
		this.Properties.ParseEnum<NavObjectScreenSettings.SpriteFillTypes>("sprite_fill_type", ref this.SpriteFillType);
		this.Properties.ParseString("sprite_fill_name", ref this.SpriteFillName);
		this.Properties.ParseString("sub_sprite_name", ref this.SubSpriteName);
		this.Properties.ParseVec("sub_sprite_offset", ref this.SubSpriteOffset);
		this.Properties.ParseFloat("sub_sprite_size", ref this.SubSpriteSize);
		this.Properties.ParseInt("font_size", ref this.FontSize);
		this.Properties.ParseColor("font_color", ref this.FontColor);
		this.FadeEndDistance = (this.MaxDistance - this.MinDistance) * this.FadePercent;
	}

	// Token: 0x0400296E RID: 10606
	public NavObjectScreenSettings.ShowTextTypes ShowTextType;

	// Token: 0x0400296F RID: 10607
	public int FontSize = 24;

	// Token: 0x04002970 RID: 10608
	public Color FontColor = Color.white;

	// Token: 0x04002971 RID: 10609
	public float SpriteSize = 32f;

	// Token: 0x04002972 RID: 10610
	public float FadePercent = 0.9f;

	// Token: 0x04002973 RID: 10611
	public float FadeEndDistance;

	// Token: 0x04002974 RID: 10612
	public bool ShowOffScreen;

	// Token: 0x04002975 RID: 10613
	public bool UseHeadOffset;

	// Token: 0x04002976 RID: 10614
	public NavObjectScreenSettings.SpriteFillTypes SpriteFillType;

	// Token: 0x04002977 RID: 10615
	public string SpriteFillName;

	// Token: 0x04002978 RID: 10616
	public Color SpriteFillColor = Color.white;

	// Token: 0x04002979 RID: 10617
	public string SubSpriteName;

	// Token: 0x0400297A RID: 10618
	public Vector2 SubSpriteOffset;

	// Token: 0x0400297B RID: 10619
	public float SubSpriteSize = 16f;

	// Token: 0x020006B0 RID: 1712
	public enum ShowTextTypes
	{
		// Token: 0x0400297D RID: 10621
		None,
		// Token: 0x0400297E RID: 10622
		Distance,
		// Token: 0x0400297F RID: 10623
		Name,
		// Token: 0x04002980 RID: 10624
		SpawnName
	}

	// Token: 0x020006B1 RID: 1713
	public enum SpriteFillTypes
	{
		// Token: 0x04002982 RID: 10626
		None,
		// Token: 0x04002983 RID: 10627
		Health
	}
}
