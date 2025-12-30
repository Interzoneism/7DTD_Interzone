using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020012EB RID: 4843
public class vp_DemoManager
{
	// Token: 0x060096E4 RID: 38628 RVA: 0x003BE998 File Offset: 0x003BCB98
	public vp_DemoManager()
	{
		this.DesktopResolution = Screen.currentResolution;
		this.LastInputTime = Time.time;
		this.m_FullScreenFadeTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
	}

	// Token: 0x060096E5 RID: 38629 RVA: 0x003BEA4C File Offset: 0x003BCC4C
	public virtual void Update()
	{
		if (double.IsNaN((double)Camera.main.fieldOfView))
		{
			Camera.main.fieldOfView = 60f;
		}
		if (Input.GetKeyDown(KeyCode.U))
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
		if (Screen.fullScreen && Screen.currentResolution.width != this.DesktopResolution.width)
		{
			Screen.SetResolution(this.DesktopResolution.width, this.DesktopResolution.height, true);
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			this.m_SimulateLowFPS = !this.m_SimulateLowFPS;
		}
		if (this.m_SimulateLowFPS)
		{
			for (int i = 0; i < 20000000; i++)
			{
			}
		}
	}

	// Token: 0x060096E6 RID: 38630 RVA: 0x003BEB00 File Offset: 0x003BCD00
	public bool ButtonToggle(Rect rect, string label, bool state, bool arrow, Texture imageUpPointer)
	{
		if (!this.ShowGUI)
		{
			return false;
		}
		GUIStyle style = this.UpStyle;
		GUIStyle style2 = this.DownStyle;
		float num = 0f;
		if (state)
		{
			style = this.DownStyle;
			style2 = this.UpStyle;
			num = rect.width * 0.5f + 2f;
		}
		GUI.Label(new Rect(rect.x, rect.y - 30f, rect.width, rect.height), label, this.CenterStyle);
		if (GUI.Button(new Rect(rect.x, rect.y, rect.width * 0.5f - 2f, rect.height), "OFF", style2))
		{
			state = false;
		}
		if (GUI.Button(new Rect(rect.x + rect.width * 0.5f + 2f, rect.y, rect.width * 0.5f, rect.height), "ON", style))
		{
			state = true;
		}
		if (arrow)
		{
			GUI.Label(new Rect(rect.x + rect.width * 0.5f * 0.5f - 14f + num, rect.y + rect.height, 32f, 32f), imageUpPointer);
		}
		return state;
	}

	// Token: 0x060096E7 RID: 38631 RVA: 0x003BEC58 File Offset: 0x003BCE58
	public void DrawBoxes(string caption, string description, Texture imageLeftArrow, Texture imageRightArrow, vp_DemoManager.LoadLevelCallback nextLevelCallback = null, vp_DemoManager.LoadLevelCallback prevLevelCallback = null, bool drawBox = true)
	{
		if (!this.ShowGUI)
		{
			return;
		}
		GUI.color = new Color(1f, 1f, 1f, 1f * this.GlobalAlpha);
		GUILayout.BeginArea(new Rect((float)(Screen.width / 2) - 400f, 30f, 800f, 100f));
		if (imageLeftArrow != null)
		{
			GUI.Box(new Rect(30f, 10f, 80f, 80f), "");
		}
		if (drawBox)
		{
			GUI.Box(new Rect(120f, 0f, 560f, 100f), "");
		}
		GUI.color = new Color(1f, 1f, 1f, this.m_TextAlpha * this.GlobalAlpha);
		for (int i = 0; i < 3; i++)
		{
			GUILayout.BeginArea(new Rect(130f, 10f, 540f, 80f));
			GUILayout.Label("--- " + caption.ToUpper() + " ---\n" + description, this.LabelStyle, Array.Empty<GUILayoutOption>());
			GUILayout.EndArea();
		}
		GUI.color = new Color(1f, 1f, 1f, 1f * this.GlobalAlpha);
		if (imageRightArrow != null)
		{
			GUI.Box(new Rect(690f, 10f, 80f, 80f), "");
		}
		if (imageLeftArrow != null && GUI.Button(new Rect(35f, 15f, 80f, 80f), imageLeftArrow, "Label"))
		{
			if (prevLevelCallback == null)
			{
				this.m_FadeToScreen = Mathf.Max(this.CurrentScreen - 1, 1);
				this.m_FadeState = vp_DemoManager.FadeState.FadeOut;
			}
			else
			{
				prevLevelCallback();
			}
		}
		if (Time.time < this.LastInputTime + 30f)
		{
			this.m_BigArrowFadeAlpha = 1f;
		}
		else
		{
			this.m_BigArrowFadeAlpha = 0.5f - Mathf.Sin((Time.time - 0.5f) * 6f) * 1f;
		}
		GUI.color = new Color(1f, 1f, 1f, this.m_BigArrowFadeAlpha * this.GlobalAlpha);
		if (imageRightArrow != null && GUI.Button(new Rect(700f, 15f, 80f, 80f), imageRightArrow, "Label"))
		{
			if (nextLevelCallback == null)
			{
				this.m_FadeToScreen = this.CurrentScreen + 1;
				this.m_FadeState = vp_DemoManager.FadeState.FadeOut;
			}
			else
			{
				nextLevelCallback();
			}
		}
		GUI.color = new Color(1f, 1f, 1f, 1f * this.GlobalAlpha);
		GUILayout.EndArea();
		GUI.color = new Color(1f, 1f, 1f, this.m_TextAlpha * this.GlobalAlpha);
	}

	// Token: 0x060096E8 RID: 38632 RVA: 0x003BEF4C File Offset: 0x003BD14C
	public int ToggleColumn(int width, int y, int sel, string[] strings, bool center, bool arrow, Texture imageRightPointer, Texture imageLeftPointer)
	{
		if (!this.ShowGUI)
		{
			return 0;
		}
		float num = (float)(strings.Length * 30);
		Vector2 vector = new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2));
		Rect position;
		if (center)
		{
			position = new Rect(vector.x - (float)width, (float)y, (float)width, 30f);
		}
		else
		{
			position = new Rect((float)(Screen.width - width - 10), vector.y - num / 2f, (float)width, 30f);
		}
		int num2 = 0;
		foreach (string text in strings)
		{
			if (center)
			{
				position.x = vector.x - (float)(width / 2);
			}
			else
			{
				position.x = 10f;
			}
			position.width = (float)width;
			GUIStyle style = this.UpStyle;
			if (num2 == sel)
			{
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, 1f * this.GlobalAlpha);
				style = this.DownStyle;
				if (center)
				{
					position.x = vector.x - (float)(width / 2) + 10f;
				}
				else
				{
					position.x = 20f;
				}
				position.width = (float)(width - 20);
				if (arrow && !this.ClosingDown)
				{
					if (center)
					{
						GUI.Label(new Rect(position.x - 27f, position.y, 32f, 32f), imageRightPointer);
					}
					else
					{
						GUI.Label(new Rect(position.x + position.width + 5f, position.y, 32f, 32f), imageLeftPointer);
					}
				}
				GUI.color = color;
			}
			if (GUI.Button(position, text, style))
			{
				sel = num2;
			}
			position.y += 35f;
			num2++;
		}
		return sel;
	}

	// Token: 0x060096E9 RID: 38633 RVA: 0x003BF130 File Offset: 0x003BD330
	public int ButtonColumn(int y, int sel, string[] strings, Texture imagePointer)
	{
		if (!this.ShowGUI)
		{
			return 0;
		}
		float num = (float)(Screen.width / 2);
		Rect position = new Rect(num - 100f, (float)y, 200f, 30f);
		int num2 = 0;
		foreach (string text in strings)
		{
			position.x = num - 100f;
			position.width = 200f;
			if (GUI.Button(position, text))
			{
				sel = num2;
				this.ButtonColumnClickTime = Time.time;
				this.ButtonColumnArrowY = position.y;
			}
			position.y += 35f;
			num2++;
		}
		if (Time.time < this.ButtonColumnArrowFadeoutTime)
		{
			this.ButtonColumnClickTime = Time.time;
		}
		GUI.color = new Color(1f, 1f, 1f, Mathf.Max(0f, 1f - (Time.time - this.ButtonColumnClickTime) * 1f * this.GlobalAlpha));
		GUI.Label(new Rect(position.x - 27f, this.ButtonColumnArrowY, 32f, 32f), imagePointer);
		GUI.color = new Color(1f, 1f, 1f, 1f * this.GlobalAlpha);
		return sel;
	}

	// Token: 0x060096EA RID: 38634 RVA: 0x003BF285 File Offset: 0x003BD485
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Reset()
	{
		this.ButtonSelection = 0;
		this.FirstFrame = true;
		this.LastInputTime = Time.time;
	}

	// Token: 0x060096EB RID: 38635 RVA: 0x003BF2A0 File Offset: 0x003BD4A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitGUIStyles()
	{
		this.LabelStyle = new GUIStyle("Label");
		this.LabelStyle.alignment = TextAnchor.LowerCenter;
		this.UpStyle = new GUIStyle("Button");
		this.DownStyle = new GUIStyle("Button");
		this.DownStyle.normal = this.DownStyle.active;
		this.CenterStyle = new GUIStyle("Label");
		this.CenterStyle.alignment = TextAnchor.MiddleCenter;
		this.m_StylesInitialized = true;
	}

	// Token: 0x060096EC RID: 38636 RVA: 0x003BF338 File Offset: 0x003BD538
	public void DrawImage(Texture image, float xOffset, float yOffset)
	{
		if (!this.ShowGUI)
		{
			return;
		}
		if (image == null)
		{
			return;
		}
		float num = (float)(Screen.width / 2);
		float num2 = (float)Mathf.Min(image.width, Screen.width);
		float num3 = (float)image.height / (float)image.width;
		GUI.DrawTexture(new Rect(num - num2 / 2f + xOffset, 140f + yOffset, num2, num2 * num3), image);
	}

	// Token: 0x060096ED RID: 38637 RVA: 0x003BF3A2 File Offset: 0x003BD5A2
	public void DrawImage(Texture image)
	{
		this.DrawImage(image, 0f, 0f);
	}

	// Token: 0x060096EE RID: 38638 RVA: 0x003BF3B8 File Offset: 0x003BD5B8
	public void DrawEditorPreview(Texture section, Texture imageEditorPreview, Texture imageEditorScreenshot)
	{
		if (!this.ShowGUI)
		{
			return;
		}
		Color color = GUI.color;
		Vector2 vector = new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		float num = 0f;
		if (this.EditorPreviewSectionExpanded)
		{
			float num2 = (float)(Screen.height - section.height - imageEditorPreview.height);
			float num3 = (float)(Screen.height - section.height);
			GUI.DrawTexture(new Rect(num, num2, (float)imageEditorPreview.width, (float)imageEditorPreview.height), imageEditorPreview);
			GUI.DrawTexture(new Rect(num, num3, (float)section.width, (float)section.height), section);
			if (vector.x > num && vector.x < num + (float)section.width && vector.y > num2 && vector.y < (float)(Screen.height - imageEditorPreview.height))
			{
				this.m_EditorPreviewScreenshotTextAlpha = Mathf.Min(1f, this.m_EditorPreviewScreenshotTextAlpha + 0.01f);
				if (Input.GetMouseButtonDown(0))
				{
					this.EditorPreviewSectionExpanded = false;
				}
			}
			else
			{
				this.m_EditorPreviewScreenshotTextAlpha = Mathf.Max(0f, this.m_EditorPreviewScreenshotTextAlpha - 0.03f);
			}
			GUI.color = new Color(1f, 1f, 1f, color.a * 0.5f * this.m_EditorPreviewScreenshotTextAlpha * this.GlobalAlpha);
			GUI.DrawTexture(new Rect(num + 48f, num3 + (float)(section.height / 2) - (float)(imageEditorScreenshot.height / 2), (float)imageEditorScreenshot.width, (float)imageEditorScreenshot.height), imageEditorScreenshot);
		}
		else
		{
			float num4 = (float)(Screen.height - imageEditorPreview.height);
			GUI.DrawTexture(new Rect(num, num4, (float)imageEditorPreview.width, (float)imageEditorPreview.height), imageEditorPreview);
			if (vector.x > num && vector.x < num + (float)section.width && vector.y > num4 && Input.GetMouseButtonUp(0))
			{
				this.EditorPreviewSectionExpanded = true;
			}
		}
		GUI.color = color;
	}

	// Token: 0x060096EF RID: 38639 RVA: 0x003BF5BC File Offset: 0x003BD7BC
	public void DrawFullScreenText(Texture imageFullScreen)
	{
		if (!this.ShowGUI)
		{
			return;
		}
		if (Time.realtimeSinceStartup > 5f)
		{
			return;
		}
		if (Time.realtimeSinceStartup > 3f)
		{
			this.m_FullScreenTextAlpha -= this.m_FadeSpeed * Time.deltaTime * 15f;
		}
		GUI.color = new Color(1f, 1f, 1f, this.m_FullScreenTextAlpha * this.GlobalAlpha);
		GUI.DrawTexture(new Rect((float)(Screen.width / 2 - 120), (float)(Screen.height / 2 - 16), 240f, 32f), imageFullScreen);
		GUI.color = new Color(1f, 1f, 1f, 1f * this.GlobalAlpha);
	}

	// Token: 0x060096F0 RID: 38640 RVA: 0x003BF680 File Offset: 0x003BD880
	public void DoScreenTransition()
	{
		if (!this.ShowGUI)
		{
			return;
		}
		if (this.m_FadeState == vp_DemoManager.FadeState.FadeOut)
		{
			this.m_TextAlpha -= this.m_FadeSpeed;
			if (this.m_TextAlpha <= 0f)
			{
				this.m_TextAlpha = 0f;
				this.Reset();
				this.CurrentScreen = this.m_FadeToScreen;
				this.m_FadeState = vp_DemoManager.FadeState.FadeIn;
				return;
			}
		}
		else if (this.m_FadeState == vp_DemoManager.FadeState.FadeIn && !this.ClosingDown)
		{
			this.m_TextAlpha += this.m_FadeSpeed;
			if (this.m_TextAlpha >= 1f)
			{
				this.m_TextAlpha = 1f;
				this.m_FadeState = vp_DemoManager.FadeState.None;
			}
		}
	}

	// Token: 0x060096F1 RID: 38641 RVA: 0x003BF727 File Offset: 0x003BD927
	public void SetScreen(int screen)
	{
		this.m_FadeToScreen = screen;
		this.m_FadeState = vp_DemoManager.FadeState.FadeOut;
	}

	// Token: 0x060096F2 RID: 38642 RVA: 0x003BF738 File Offset: 0x003BD938
	public void OnGUI()
	{
		if (!this.m_StylesInitialized)
		{
			this.InitGUIStyles();
		}
		this.DoScreenTransition();
		if (vp_Utility.LockCursor && this.FadeGUIOnCursorLock)
		{
			this.GlobalAlpha = 0.35f;
		}
		else if (!this.ClosingDown)
		{
			this.GlobalAlpha = 1f;
		}
		if (Time.time - this.CurrentFullScreenFadeTime < this.m_FullScreenFadeInDuration)
		{
			this.GlobalAlpha = Time.time - this.CurrentFullScreenFadeTime;
			GUI.color = new Color(0f, 0f, 0f, (this.m_FullScreenFadeInDuration - this.GlobalAlpha) / this.m_FullScreenFadeInDuration);
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.m_FullScreenFadeTexture);
			GUI.color = Color.white;
		}
		if (this.CurrentFullScreenFadeTime > Time.time)
		{
			this.GlobalAlpha = this.CurrentFullScreenFadeTime - Time.time;
			GUI.color = new Color(0f, 0f, 0f, (this.m_FullScreenFadeOutDuration - this.GlobalAlpha) / this.m_FullScreenFadeOutDuration);
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.m_FullScreenFadeTexture);
			GUI.color = Color.white;
		}
	}

	// Token: 0x060096F3 RID: 38643 RVA: 0x003BF888 File Offset: 0x003BDA88
	public void LoadLevel(int level)
	{
		this.ClosingDown = true;
		vp_Timer.CancelAll();
		vp_TimeUtility.TimeScale = 1f;
		this.m_FadeState = vp_DemoManager.FadeState.FadeOut;
		this.CurrentFullScreenFadeTime = Time.time + this.m_FullScreenFadeOutDuration;
		vp_Timer.In(this.m_FullScreenFadeOutDuration, delegate()
		{
			SceneManager.LoadScene(level);
		}, null);
	}

	// Token: 0x040072AE RID: 29358
	public GUIStyle UpStyle;

	// Token: 0x040072AF RID: 29359
	public GUIStyle LabelStyle;

	// Token: 0x040072B0 RID: 29360
	public GUIStyle DownStyle;

	// Token: 0x040072B1 RID: 29361
	public GUIStyle CenterStyle;

	// Token: 0x040072B2 RID: 29362
	public int CurrentScreen = 1;

	// Token: 0x040072B3 RID: 29363
	public Resolution DesktopResolution;

	// Token: 0x040072B4 RID: 29364
	public bool FirstFrame = true;

	// Token: 0x040072B5 RID: 29365
	public bool EditorPreviewSectionExpanded = true;

	// Token: 0x040072B6 RID: 29366
	public bool ShowGUI = true;

	// Token: 0x040072B7 RID: 29367
	public float ButtonColumnClickTime;

	// Token: 0x040072B8 RID: 29368
	public float ButtonColumnArrowY = -100f;

	// Token: 0x040072B9 RID: 29369
	public float ButtonColumnArrowFadeoutTime;

	// Token: 0x040072BA RID: 29370
	public int ButtonSelection;

	// Token: 0x040072BB RID: 29371
	public float LastInputTime;

	// Token: 0x040072BC RID: 29372
	public bool FadeGUIOnCursorLock = true;

	// Token: 0x040072BD RID: 29373
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_FadeSpeed = 0.03f;

	// Token: 0x040072BE RID: 29374
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_FadeToScreen;

	// Token: 0x040072BF RID: 29375
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_StylesInitialized;

	// Token: 0x040072C0 RID: 29376
	[PublicizedFrom(EAccessModifier.Private)]
	public vp_DemoManager.FadeState m_FadeState;

	// Token: 0x040072C1 RID: 29377
	[PublicizedFrom(EAccessModifier.Private)]
	public Texture2D m_FullScreenFadeTexture;

	// Token: 0x040072C2 RID: 29378
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_FullScreenFadeOutDuration = 0.5f;

	// Token: 0x040072C3 RID: 29379
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_FullScreenFadeInDuration = 0.75f;

	// Token: 0x040072C4 RID: 29380
	public float CurrentFullScreenFadeTime;

	// Token: 0x040072C5 RID: 29381
	public bool ClosingDown;

	// Token: 0x040072C6 RID: 29382
	public float GlobalAlpha = 0.35f;

	// Token: 0x040072C7 RID: 29383
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_TextAlpha = 1f;

	// Token: 0x040072C8 RID: 29384
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_EditorPreviewScreenshotTextAlpha;

	// Token: 0x040072C9 RID: 29385
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_FullScreenTextAlpha = 1f;

	// Token: 0x040072CA RID: 29386
	[PublicizedFrom(EAccessModifier.Private)]
	public float m_BigArrowFadeAlpha = 1f;

	// Token: 0x040072CB RID: 29387
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_SimulateLowFPS;

	// Token: 0x020012EC RID: 4844
	[PublicizedFrom(EAccessModifier.Private)]
	public enum FadeState
	{
		// Token: 0x040072CD RID: 29389
		None,
		// Token: 0x040072CE RID: 29390
		FadeOut,
		// Token: 0x040072CF RID: 29391
		FadeIn
	}

	// Token: 0x020012ED RID: 4845
	// (Invoke) Token: 0x060096F5 RID: 38645
	public delegate void LoadLevelCallback();
}
