using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000E59 RID: 3673
[Preserve]
public class XUiC_SubtitlesDisplay : XUiController
{
	// Token: 0x17000BAF RID: 2991
	// (get) Token: 0x06007353 RID: 29523 RVA: 0x002F03AC File Offset: 0x002EE5AC
	// (set) Token: 0x06007354 RID: 29524 RVA: 0x002F03B3 File Offset: 0x002EE5B3
	public static bool IsDisplaying
	{
		get
		{
			return XUiC_SubtitlesDisplay._isDisplaying;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			XUiC_SubtitlesDisplay._isDisplaying = value;
			GameManager instance = GameManager.Instance;
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			instance.SetToolTipPause((primaryPlayer != null) ? primaryPlayer.PlayerUI.nguiWindowManager : null, value);
		}
	}

	// Token: 0x06007355 RID: 29525 RVA: 0x002F03E8 File Offset: 0x002EE5E8
	public override void Init()
	{
		base.Init();
		XUiC_SubtitlesDisplay.ID = this.windowGroup.ID;
		this.speakerLabel = (XUiV_Label)base.GetChildById("lblSpeaker").ViewComponent;
		this.subtitlesLabel = (XUiV_Label)base.GetChildById("lblSubtitle").ViewComponent;
		this.background = (XUiV_Panel)base.GetChildById("bgPanel").ViewComponent;
		this.bgSprite = this.background.UiTransform.Find("_background").GetComponentInChildren<UISprite>();
		this.subtitlesLabel.MaxLineCount = 2;
		this.subtitlesLabel.Overflow = UILabel.Overflow.ShrinkContent;
	}

	// Token: 0x06007356 RID: 29526 RVA: 0x002F0494 File Offset: 0x002EE694
	public static void DisplaySubtitle(LocalPlayerUI ui, string speakerText, string contentText, float duration = 3f, bool centerAlign = false)
	{
		XUiC_SubtitlesDisplay instance = XUiC_SubtitlesDisplay.GetInstance(ui.xui);
		if (instance != null)
		{
			ui.windowManager.OpenIfNotOpen("SubtitlesDisplay", false, false, false);
			instance.SetSubtitle(speakerText, contentText, Mathf.Max(duration, 3f) + 1f, centerAlign);
			if (GameManager.Instance.World != null)
			{
				EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
				NGUIWindowManager nguiwindowManager = (primaryPlayer != null) ? primaryPlayer.PlayerUI.nguiWindowManager : null;
				if (nguiwindowManager != null)
				{
					GameManager.Instance.ClearCurrentTooltip(nguiwindowManager);
				}
			}
		}
	}

	// Token: 0x06007357 RID: 29527 RVA: 0x002F051F File Offset: 0x002EE71F
	[PublicizedFrom(EAccessModifier.Private)]
	public static XUiC_SubtitlesDisplay GetInstance(XUi _xui)
	{
		XUiWindowGroup xuiWindowGroup = (XUiWindowGroup)_xui.playerUI.windowManager.GetWindow(XUiC_SubtitlesDisplay.ID);
		if (xuiWindowGroup == null)
		{
			return null;
		}
		XUiController controller = xuiWindowGroup.Controller;
		if (controller == null)
		{
			return null;
		}
		return controller.GetChildByType<XUiC_SubtitlesDisplay>();
	}

	// Token: 0x06007358 RID: 29528 RVA: 0x002F0554 File Offset: 0x002EE754
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetSubtitle(string speaker, string content, float duration, bool centerAlign)
	{
		this.speakerLabel.Text = speaker;
		this.subtitlesLabel.Text = content;
		this.alignment = (centerAlign ? NGUIText.Alignment.Center : NGUIText.Alignment.Left);
		this.subtitlesLabel.Label.alignment = this.alignment;
		this.subtitlesLabel.Label.ProcessText(false, true);
		this.openTime = Time.time;
		this.duration = duration;
		XUiC_SubtitlesDisplay.IsDisplaying = true;
	}

	// Token: 0x06007359 RID: 29529 RVA: 0x002F05C8 File Offset: 0x002EE7C8
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.IsOpen)
		{
			if (this.subtitlesLabel.Label.alignment != this.alignment)
			{
				this.subtitlesLabel.Label.alignment = this.alignment;
			}
			if (this.subtitlesLabel.Label.overflowMethod != UILabel.Overflow.ShrinkContent)
			{
				this.subtitlesLabel.Label.overflowMethod = UILabel.Overflow.ShrinkContent;
			}
			if (this.subtitlesLabel.Label.width != 610)
			{
				this.subtitlesLabel.Label.width = 610;
			}
			if (Time.time - this.openTime >= this.duration)
			{
				base.xui.playerUI.windowManager.CloseIfOpen("SubtitlesDisplay");
				XUiC_SubtitlesDisplay.IsDisplaying = false;
			}
		}
	}

	// Token: 0x0600735A RID: 29530 RVA: 0x002F0698 File Offset: 0x002EE898
	public override void OnClose()
	{
		base.OnClose();
		XUiC_SubtitlesDisplay.IsDisplaying = false;
	}

	// Token: 0x040057CA RID: 22474
	public static string ID = "";

	// Token: 0x040057CB RID: 22475
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label speakerLabel;

	// Token: 0x040057CC RID: 22476
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label subtitlesLabel;

	// Token: 0x040057CD RID: 22477
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Panel background;

	// Token: 0x040057CE RID: 22478
	[PublicizedFrom(EAccessModifier.Private)]
	public UISprite bgSprite;

	// Token: 0x040057CF RID: 22479
	[PublicizedFrom(EAccessModifier.Private)]
	public float openTime;

	// Token: 0x040057D0 RID: 22480
	[PublicizedFrom(EAccessModifier.Private)]
	public float duration;

	// Token: 0x040057D1 RID: 22481
	[PublicizedFrom(EAccessModifier.Private)]
	public const float minDuration = 3f;

	// Token: 0x040057D2 RID: 22482
	[PublicizedFrom(EAccessModifier.Private)]
	public const float durationAdd = 1f;

	// Token: 0x040057D3 RID: 22483
	[PublicizedFrom(EAccessModifier.Private)]
	public const int labelPadding = 28;

	// Token: 0x040057D4 RID: 22484
	[PublicizedFrom(EAccessModifier.Private)]
	public int targetHeight = 64;

	// Token: 0x040057D5 RID: 22485
	[PublicizedFrom(EAccessModifier.Private)]
	public bool pendingUpdate;

	// Token: 0x040057D6 RID: 22486
	[PublicizedFrom(EAccessModifier.Private)]
	public string pendingText = "";

	// Token: 0x040057D7 RID: 22487
	[PublicizedFrom(EAccessModifier.Private)]
	public NGUIText.Alignment alignment = NGUIText.Alignment.Left;

	// Token: 0x040057D8 RID: 22488
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool _isDisplaying;
}
