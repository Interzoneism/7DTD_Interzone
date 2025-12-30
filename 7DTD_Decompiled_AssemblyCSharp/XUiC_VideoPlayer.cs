using System;
using Audio;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Video;

// Token: 0x02000EA4 RID: 3748
[Preserve]
public class XUiC_VideoPlayer : XUiController
{
	// Token: 0x0600765A RID: 30298 RVA: 0x00302F8C File Offset: 0x0030118C
	public override void Init()
	{
		base.Init();
		XUiC_VideoPlayer.ID = this.windowGroup.ID;
		this.videoTexture = base.GetChildById("videoTexture").ViewComponent.UiTransform.GetComponent<UITexture>();
		this.backgroundSprite = base.GetChildById("videoBackground").ViewComponent.UiTransform.GetComponent<UISprite>();
		this.skipPrompt = base.GetChildById("skipPrompt").ViewComponent.UiTransform.gameObject;
		this.skipLabel = (XUiV_Label)base.GetChildById("lblSkip").ViewComponent;
		this.videoPlayer = this.videoTexture.gameObject.AddComponent<VideoPlayer>();
		this.videoPlayer.playOnAwake = false;
		this.videoPlayer.isLooping = false;
		this.videoPlayer.renderMode = VideoRenderMode.RenderTexture;
		this.videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
		this.videoPlayer.prepareCompleted += this.OnVideoPrepared;
		this.videoPlayer.loopPointReached += this.OnVideoFinished;
		this.videoPlayer.errorReceived += this.OnVideoErrorReceived;
		this.skipPrompt.SetActive(false);
	}

	// Token: 0x0600765B RID: 30299 RVA: 0x003030C1 File Offset: 0x003012C1
	public static void PlayVideo(XUi _xui, VideoData _videoData, bool _skippable, XUiC_VideoPlayer.DelegateOnVideoFinished _videoFinishedCallback = null)
	{
		XUiC_VideoPlayer instance = XUiC_VideoPlayer.GetInstance(_xui);
		_xui.playerUI.windowManager.OpenIfNotOpen("VideoPlayer", true, false, false);
		instance.PlayVideo(_videoData, _skippable, _videoFinishedCallback);
	}

	// Token: 0x0600765C RID: 30300 RVA: 0x003030E9 File Offset: 0x003012E9
	public static void EndVideo(XUiC_VideoPlayer _videoPlayer)
	{
		_videoPlayer.FinishAndClose(true);
	}

	// Token: 0x0600765D RID: 30301 RVA: 0x003030F2 File Offset: 0x003012F2
	public static XUiC_VideoPlayer GetInstance(XUi _xui)
	{
		XUiWindowGroup xuiWindowGroup = (XUiWindowGroup)_xui.playerUI.windowManager.GetWindow(XUiC_VideoPlayer.ID);
		if (xuiWindowGroup == null)
		{
			return null;
		}
		XUiController controller = xuiWindowGroup.Controller;
		if (controller == null)
		{
			return null;
		}
		return controller.GetChildByType<XUiC_VideoPlayer>();
	}

	// Token: 0x0600765E RID: 30302 RVA: 0x00303124 File Offset: 0x00301324
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayVideo(VideoData _videoData, bool _skippable, XUiC_VideoPlayer.DelegateOnVideoFinished _videoFinishedCallback = null)
	{
		this.currentVideo = _videoData;
		this.subtitlesEnabled = GamePrefs.GetBool(EnumGamePrefs.OptionsSubtitlesEnabled);
		this.wasVideoSkipped = false;
		this.skippable = _skippable;
		this.skipPrompt.SetActive(false);
		if (_videoFinishedCallback != null)
		{
			this.onVideoFinished = (XUiC_VideoPlayer.DelegateOnVideoFinished)Delegate.Combine(this.onVideoFinished, _videoFinishedCallback);
		}
		if (this.rt == null || (this.rt != null && (Screen.width != this.rt.width || Screen.height != this.rt.height)))
		{
			Log.Out("Creating video render texture {0} / {1}", new object[]
			{
				Screen.width,
				Screen.height
			});
			this.rt = new RenderTexture(Screen.width, Screen.height, 16);
			this.rt.Create();
		}
		this.videoPlayer.targetTexture = this.rt;
		this.videoTexture.mainTexture = this.videoPlayer.targetTexture;
		string bindingXuiMarkupString = base.xui.playerUI.playerInput.GUIActions.Cancel.GetBindingXuiMarkupString(XUiUtils.EmptyBindingStyle.EmptyString, XUiUtils.DisplayStyle.Plain, null);
		this.skipLabel.Text = string.Format(Localization.Get("ui_video_skip", false), bindingXuiMarkupString);
		this.previousTimestamp = 0.0;
		this.videoPlayer.url = Application.streamingAssetsPath + _videoData.url;
		this.videoPlayer.Prepare();
		XUiC_VideoPlayer.IsVideoPlaying = true;
	}

	// Token: 0x0600765F RID: 30303 RVA: 0x003032AC File Offset: 0x003014AC
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.IsOpen)
		{
			if (this.subtitlesEnabled && this.videoPlayer.isPlaying)
			{
				double time = this.videoPlayer.time;
				foreach (VideoSubtitle videoSubtitle in this.currentVideo.subtitles)
				{
					if (videoSubtitle.timestamp >= this.previousTimestamp && videoSubtitle.timestamp <= time)
					{
						GameManager.ShowSubtitle(LocalPlayerUI.primaryUI.xui, Manager.GetFormattedSubtitleSpeaker(videoSubtitle.subtitleId), Manager.GetFormattedSubtitle(videoSubtitle.subtitleId), videoSubtitle.duration, true);
						break;
					}
				}
				this.previousTimestamp = time;
			}
			if (this.videoTexture.mainTexture != this.rt)
			{
				this.videoTexture.mainTexture = this.rt;
			}
			this.backgroundSprite.color = Color.black;
			if (this.skippable && base.xui.playerUI.playerInput != null)
			{
				if (!this.skipPrompt.activeSelf && base.xui.playerUI.playerInput.AnyGUIActionPressed())
				{
					this.skipPrompt.SetActive(true);
					this.skipVisibleTime = Time.time;
				}
				else if (this.skipPrompt.activeSelf && base.xui.playerUI.playerInput.GUIActions.Cancel.WasPressed)
				{
					this.FinishAndClose(true);
				}
				if (this.skipPrompt.activeSelf && Time.time - this.skipVisibleTime >= 3f)
				{
					this.skipPrompt.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06007660 RID: 30304 RVA: 0x0030347C File Offset: 0x0030167C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnVideoPrepared(VideoPlayer _source)
	{
		this.videoPlayer.Play();
	}

	// Token: 0x06007661 RID: 30305 RVA: 0x00303489 File Offset: 0x00301689
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnVideoErrorReceived(VideoPlayer _source, string _message)
	{
		Log.Error("Video player encountered an error. Skipping video. Message: {0}", new object[]
		{
			_message
		});
		this.FinishAndClose(true);
	}

	// Token: 0x06007662 RID: 30306 RVA: 0x003034A6 File Offset: 0x003016A6
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnVideoFinished(VideoPlayer _source)
	{
		this.FinishAndClose(false);
	}

	// Token: 0x06007663 RID: 30307 RVA: 0x003034AF File Offset: 0x003016AF
	[PublicizedFrom(EAccessModifier.Private)]
	public void FinishAndClose(bool _skipped)
	{
		this.wasVideoSkipped = _skipped;
		base.xui.playerUI.windowManager.Close(XUiC_SubtitlesDisplay.ID);
		base.xui.playerUI.windowManager.Close("VideoPlayer");
	}

	// Token: 0x06007664 RID: 30308 RVA: 0x003034EC File Offset: 0x003016EC
	public override void OnClose()
	{
		base.OnClose();
		if (this.rt != null)
		{
			this.videoTexture.mainTexture = (this.videoPlayer.targetTexture = null);
			this.rt.Release();
			UnityEngine.Object.Destroy(this.rt);
		}
		XUiC_VideoPlayer.IsVideoPlaying = false;
		if (this.onVideoFinished != null)
		{
			this.onVideoFinished(this.wasVideoSkipped);
		}
	}

	// Token: 0x04005A45 RID: 23109
	public static string ID = "";

	// Token: 0x04005A46 RID: 23110
	[PublicizedFrom(EAccessModifier.Protected)]
	public VideoPlayer videoPlayer;

	// Token: 0x04005A47 RID: 23111
	[PublicizedFrom(EAccessModifier.Protected)]
	public UITexture videoTexture;

	// Token: 0x04005A48 RID: 23112
	[PublicizedFrom(EAccessModifier.Protected)]
	public Camera videoCamera;

	// Token: 0x04005A49 RID: 23113
	[PublicizedFrom(EAccessModifier.Protected)]
	public UISprite backgroundSprite;

	// Token: 0x04005A4A RID: 23114
	[PublicizedFrom(EAccessModifier.Protected)]
	public GameObject skipPrompt;

	// Token: 0x04005A4B RID: 23115
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiV_Label skipLabel;

	// Token: 0x04005A4C RID: 23116
	public XUiC_VideoPlayer.DelegateOnVideoFinished onVideoFinished;

	// Token: 0x04005A4D RID: 23117
	[PublicizedFrom(EAccessModifier.Private)]
	public VideoData currentVideo;

	// Token: 0x04005A4E RID: 23118
	[PublicizedFrom(EAccessModifier.Private)]
	public RenderTexture rt;

	// Token: 0x04005A4F RID: 23119
	[PublicizedFrom(EAccessModifier.Private)]
	public double previousTimestamp;

	// Token: 0x04005A50 RID: 23120
	[PublicizedFrom(EAccessModifier.Private)]
	public bool skippable;

	// Token: 0x04005A51 RID: 23121
	[PublicizedFrom(EAccessModifier.Private)]
	public const float skipVisibleDuration = 3f;

	// Token: 0x04005A52 RID: 23122
	[PublicizedFrom(EAccessModifier.Private)]
	public float skipVisibleTime;

	// Token: 0x04005A53 RID: 23123
	public static bool IsVideoPlaying = false;

	// Token: 0x04005A54 RID: 23124
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasVideoSkipped;

	// Token: 0x04005A55 RID: 23125
	[PublicizedFrom(EAccessModifier.Private)]
	public bool subtitlesEnabled;

	// Token: 0x02000EA5 RID: 3749
	// (Invoke) Token: 0x06007668 RID: 30312
	public delegate void DelegateOnVideoFinished(bool skipped);
}
