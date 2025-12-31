using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D3F RID: 3391
[Preserve]
public class XUiC_OnScreenIcons : XUiController
{
	// Token: 0x060069A5 RID: 27045 RVA: 0x002AEC75 File Offset: 0x002ACE75
	public override void Init()
	{
		base.Init();
		NavObjectManager.Instance.OnNavObjectAdded += this.Instance_OnNavObjectAdded;
		NavObjectManager.Instance.OnNavObjectRemoved += this.Instance_OnNavObjectRemoved;
	}

	// Token: 0x060069A6 RID: 27046 RVA: 0x002AECA9 File Offset: 0x002ACEA9
	public override void Cleanup()
	{
		base.Cleanup();
		this.screenIconList.Clear();
		this.disabledIcons.Clear();
	}

	// Token: 0x060069A7 RID: 27047 RVA: 0x002AECC7 File Offset: 0x002ACEC7
	[PublicizedFrom(EAccessModifier.Private)]
	public void Instance_OnNavObjectRemoved(NavObject newNavObject)
	{
		if (newNavObject.HasOnScreen)
		{
			this.UnRegisterIcon(newNavObject);
		}
	}

	// Token: 0x060069A8 RID: 27048 RVA: 0x002AECD8 File Offset: 0x002ACED8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Instance_OnNavObjectAdded(NavObject newNavObject)
	{
		if (newNavObject.HasOnScreen)
		{
			this.RegisterIcon(newNavObject);
		}
	}

	// Token: 0x060069A9 RID: 27049 RVA: 0x002AECEC File Offset: 0x002ACEEC
	public override void Update(float _dt)
	{
		if (this.screenIconList.Count == 0)
		{
			return;
		}
		Vector3 offset = new Vector3((float)(-(float)base.ViewComponent.Size.x) * 0.5f, (float)(-(float)base.ViewComponent.Size.y) * 0.5f, 0f);
		EntityPlayerLocal entityPlayer = base.xui.playerUI.entityPlayer;
		Vector3 forward = entityPlayer.cameraTransform.forward;
		Vector3 position = entityPlayer.GetPosition();
		int num = 300;
		for (int i = this.screenIconList.Count - 1; i >= 0; i--)
		{
			XUiC_OnScreenIcons.OnScreenIcon onScreenIcon = this.screenIconList[i];
			onScreenIcon.Update(offset, position, forward, base.xui, ref num);
			if (onScreenIcon.ReadyForUnload)
			{
				this.screenIconList.RemoveAt(i);
				this.disabledIcons.Add(onScreenIcon);
				onScreenIcon.Transform.gameObject.SetActive(false);
			}
		}
		base.Update(_dt);
	}

	// Token: 0x060069AA RID: 27050 RVA: 0x002AEDE4 File Offset: 0x002ACFE4
	public override void OnOpen()
	{
		base.OnOpen();
		this.Camera = base.xui.playerUI.entityPlayer.playerCamera;
	}

	// Token: 0x060069AB RID: 27051 RVA: 0x00282545 File Offset: 0x00280745
	public override void OnClose()
	{
		base.OnClose();
	}

	// Token: 0x060069AC RID: 27052 RVA: 0x002AEE08 File Offset: 0x002AD008
	[PublicizedFrom(EAccessModifier.Private)]
	public void RegisterIcon(NavObject newNavObject)
	{
		for (int i = 0; i < this.screenIconList.Count; i++)
		{
			if (this.screenIconList[i].NavObject == newNavObject)
			{
				return;
			}
		}
		XUiC_OnScreenIcons.OnScreenIcon onScreenIcon = this.CreateIcon();
		onScreenIcon.Owner = this;
		onScreenIcon.NavObject = newNavObject;
		onScreenIcon.Init();
	}

	// Token: 0x060069AD RID: 27053 RVA: 0x002AEE5C File Offset: 0x002AD05C
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_OnScreenIcons.OnScreenIcon CreateIcon()
	{
		XUiC_OnScreenIcons.OnScreenIcon onScreenIcon;
		if (this.disabledIcons.Count > 0)
		{
			onScreenIcon = this.disabledIcons[0];
			this.disabledIcons.RemoveAt(0);
			onScreenIcon.ReadyForUnload = false;
			if (onScreenIcon.Transform)
			{
				onScreenIcon.Sprite.color = Color.clear;
				onScreenIcon.Sprite.spriteName = "";
			}
		}
		else
		{
			onScreenIcon = new XUiC_OnScreenIcons.OnScreenIcon();
		}
		this.screenIconList.Add(onScreenIcon);
		return onScreenIcon;
	}

	// Token: 0x060069AE RID: 27054 RVA: 0x002AEEDC File Offset: 0x002AD0DC
	public void UnRegisterIcon(NavObject navObject)
	{
		for (int i = this.screenIconList.Count - 1; i >= 0; i--)
		{
			XUiC_OnScreenIcons.OnScreenIcon onScreenIcon = this.screenIconList[i];
			if (onScreenIcon.NavObject == navObject)
			{
				if (!this.disabledIcons.Contains(onScreenIcon))
				{
					this.disabledIcons.Add(onScreenIcon);
					if (onScreenIcon.Transform)
					{
						onScreenIcon.Transform.gameObject.SetActive(false);
					}
				}
				this.screenIconList.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x04004F9F RID: 20383
	public List<XUiC_OnScreenIcons.OnScreenIcon> screenIconList = new List<XUiC_OnScreenIcons.OnScreenIcon>();

	// Token: 0x04004FA0 RID: 20384
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiC_OnScreenIcons.OnScreenIcon> disabledIcons = new List<XUiC_OnScreenIcons.OnScreenIcon>();

	// Token: 0x04004FA1 RID: 20385
	public Camera Camera;

	// Token: 0x02000D40 RID: 3392
	public class OnScreenIcon
	{
		// Token: 0x060069B0 RID: 27056 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init()
		{
		}

		// Token: 0x060069B1 RID: 27057 RVA: 0x002AEF7C File Offset: 0x002AD17C
		public void Update(Vector3 offset, Vector3 playerPosition, Vector3 cameraForward, XUi xui, ref int depth)
		{
			if (!this.NavObject.IsValid())
			{
				return;
			}
			if (this.Sprite)
			{
				this.Sprite.enabled = (this.Label.enabled = !this.NavObject.hiddenOnCompass);
				depth = this.UpdateDepth(depth) + 1;
			}
			this.ScreenSettings = this.NavObject.CurrentScreenSettings;
			if (this.ScreenSettings == null)
			{
				this.HideObjects();
				return;
			}
			EntityPlayerLocal entityPlayer = xui.playerUI.entityPlayer;
			if (entityPlayer.IsDead())
			{
				this.HideObjects();
				return;
			}
			float num = Vector3.Distance(this.NavObject.GetPosition(), entityPlayer.position - Origin.position);
			float maxDistance = this.NavObject.GetMaxDistance(this.ScreenSettings, entityPlayer);
			if (maxDistance != -1f && num > maxDistance)
			{
				this.HideObjects();
				return;
			}
			if (num < this.ScreenSettings.MinDistance)
			{
				this.HideObjects();
				return;
			}
			this.CreateObjects();
			string spriteName = this.NavObject.GetSpriteName(this.ScreenSettings);
			if (this.Sprite.spriteName != spriteName)
			{
				this.Sprite.atlas = xui.GetAtlasByName("UIAtlas", spriteName);
				this.Sprite.spriteName = spriteName;
			}
			Color color = this.NavObject.UseOverrideColor ? this.NavObject.OverrideColor : this.ScreenSettings.Color;
			this.Sprite.color = color;
			Vector3 vector = this.NavObject.GetPosition() + this.ScreenSettings.Offset;
			if (this.ScreenSettings.UseHeadOffset && this.NavObject.TrackType == NavObject.TrackTypes.Entity)
			{
				vector.y += this.NavObject.TrackedEntity.GetEyeHeight();
				vector += this.NavObject.TrackedEntity.EntityClass.NavObjectHeadOffset;
			}
			Vector3 lhs = vector - this.Owner.Camera.transform.position;
			lhs.Normalize();
			Vector3 vector2 = entityPlayer.finalCamera.WorldToScreenPoint(vector);
			if (this.ScreenSettings.ShowOffScreen)
			{
				if (vector2.x < 30f)
				{
					vector2.x = 30f;
				}
				float num2 = (this.ScreenSettings.ShowTextType != NavObjectScreenSettings.ShowTextTypes.None) ? 30f : 0f;
				if (vector2.y < 30f + num2)
				{
					vector2.y = 30f + num2;
				}
				if (vector2.x > (float)(Screen.width - 30))
				{
					vector2.x = (float)(Screen.width - 30);
				}
				if (vector2.y > (float)(Screen.height - 30))
				{
					vector2.y = (float)(Screen.height - 30);
				}
				if (Vector3.Dot(lhs, cameraForward) < 0f)
				{
					if (vector2.y < (float)(Screen.height / 2))
					{
						vector2.y = (float)(Screen.height - 30);
					}
					else
					{
						vector2.y = 30f + num2;
					}
					vector2.x = (float)Screen.width - vector2.x - 30f;
				}
			}
			else if (Vector3.Dot(lhs, cameraForward) < 0f)
			{
				this.HideObjects();
				return;
			}
			Vector3 localPosition = xui.TranslateScreenVectorToXuiVector(vector2);
			localPosition.z = 0f;
			this.Transform.localPosition = localPosition;
			this.Transform.gameObject.SetActive(true);
			if (maxDistance != -1f)
			{
				float num3 = 1f;
				if (num >= maxDistance)
				{
					num3 = 0f;
				}
				else if (num >= this.ScreenSettings.FadeEndDistance)
				{
					num3 = 1f - Utils.FastClamp01((num - this.ScreenSettings.FadeEndDistance) / (maxDistance - this.ScreenSettings.FadeEndDistance));
					if (num3 < 1f)
					{
						this.Sprite.color = color;
					}
				}
				this.Sprite.alpha = num3;
			}
			else
			{
				this.Sprite.alpha = 1f;
			}
			float num4 = 1f;
			if (num <= maxDistance - this.ScreenSettings.FadeEndDistance && this.ScreenSettings.HasPulse)
			{
				float num5 = Mathf.PingPong(Time.time, 0.5f);
				this.Sprite.color = Color.Lerp(Color.grey, color, num5 * 4f);
				if (num5 > 0.25f)
				{
					num4 += num5 - 0.25f;
				}
			}
			int num6 = (int)(num4 * this.ScreenSettings.SpriteSize);
			this.Sprite.SetDimensions(num6, num6);
			if (this.ScreenSettings.SpriteFillType != NavObjectScreenSettings.SpriteFillTypes.None && this.NavObject.TrackedEntity != null)
			{
				if (this.FillSprite == null)
				{
					this.SetupFillSprite();
				}
				this.FillSprite.color = this.ScreenSettings.SpriteFillColor;
				this.FillSprite.alpha = this.Sprite.alpha;
				this.FillSprite.spriteName = this.ScreenSettings.SpriteFillName;
				this.FillSprite.SetDimensions(num6, num6);
				if (this.ScreenSettings.SpriteFillType == NavObjectScreenSettings.SpriteFillTypes.Health)
				{
					this.FillSprite.fillAmount = ((EntityAlive)this.NavObject.TrackedEntity).Stats.Health.ValuePercent;
				}
			}
			else
			{
				this.RemoveFillSprite();
			}
			if (this.ScreenSettings.SubSpriteName != null)
			{
				if (this.SubSprite == null)
				{
					this.SetupSubSprite();
				}
				int num7 = (int)this.ScreenSettings.SubSpriteSize;
				this.SubSprite.transform.localPosition = this.ScreenSettings.SubSpriteOffset;
				this.SubSprite.SetDimensions(num7, num7);
				this.SubSprite.spriteName = this.ScreenSettings.SubSpriteName;
			}
			else
			{
				this.RemoveSubSprite();
			}
			if (this.Label != null)
			{
				if (this.ScreenSettings.ShowTextType != NavObjectScreenSettings.ShowTextTypes.None)
				{
					this.Label.alpha = this.Sprite.alpha;
					this.Label.fontSize = this.ScreenSettings.FontSize;
					this.Label.color = (this.NavObject.UseOverrideFontColor ? this.NavObject.OverrideColor : this.ScreenSettings.FontColor);
					if (this.ScreenSettings.ShowTextType == NavObjectScreenSettings.ShowTextTypes.Distance)
					{
						string arg = "m";
						if (num >= 1000f)
						{
							num /= 1000f;
							arg = "km";
						}
						this.Label.text = string.Format("{0} {1}", num.ToCultureInvariantString("0.0"), arg);
						return;
					}
					if (this.ScreenSettings.ShowTextType == NavObjectScreenSettings.ShowTextTypes.Name)
					{
						this.Label.text = this.NavObject.DisplayName;
						return;
					}
					this.Label.text = ((this.NavObject.TrackedEntity != null) ? this.NavObject.TrackedEntity.spawnByName : "");
					return;
				}
				else
				{
					this.Label.text = "";
				}
			}
		}

		// Token: 0x060069B2 RID: 27058 RVA: 0x002AF67C File Offset: 0x002AD87C
		public void CreateObjects()
		{
			if (this.Sprite)
			{
				return;
			}
			GameObject gameObject = new GameObject("ScreenIcon");
			Transform transform = gameObject.transform;
			this.Transform = transform;
			transform.SetParent(this.Owner.ViewComponent.UiTransform, false);
			gameObject.layer = 12;
			GameObject gameObject2 = new GameObject("Sprite");
			gameObject2.transform.SetParent(transform, false);
			gameObject2.layer = 12;
			UISprite uisprite = gameObject2.AddComponent<UISprite>();
			this.Sprite = uisprite;
			uisprite.atlas = this.Owner.xui.GetAtlasByName("UIAtlas", "menu_empty");
			uisprite.spriteName = "menu_empty";
			uisprite.SetDimensions(50, 50);
			uisprite.color = Color.clear;
			uisprite.pivot = UIWidget.Pivot.Center;
			uisprite.depth = 300;
			GameObject gameObject3 = new GameObject("Label");
			gameObject3.transform.SetParent(transform, false);
			gameObject3.layer = 12;
			UILabel uilabel = gameObject3.AddComponent<UILabel>();
			this.Label = uilabel;
			uilabel.font = this.Owner.xui.GetUIFontByName("ReferenceFont", true);
			uilabel.fontSize = 24;
			uilabel.pivot = UIWidget.Pivot.Center;
			uilabel.overflowMethod = UILabel.Overflow.ResizeFreely;
			uilabel.alignment = NGUIText.Alignment.Center;
			uilabel.effectStyle = UILabel.Effect.Outline;
			uilabel.effectColor = new Color32(0, 0, 0, byte.MaxValue);
			uilabel.effectDistance = new Vector2(2f, 2f);
			uilabel.color = Color.white;
			uilabel.text = "";
			uilabel.depth = 300;
			uilabel.width = 200;
			this.ScreenSettings = this.NavObject.CurrentScreenSettings;
			uilabel.transform.localPosition = new Vector2(0f, -(this.ScreenSettings.SpriteSize * 0.5f + 8f));
		}

		// Token: 0x060069B3 RID: 27059 RVA: 0x002AF853 File Offset: 0x002ADA53
		[PublicizedFrom(EAccessModifier.Private)]
		public void HideObjects()
		{
			if (this.Transform)
			{
				this.Transform.localPosition = NavObject.InvalidPos;
			}
		}

		// Token: 0x060069B4 RID: 27060 RVA: 0x002AF874 File Offset: 0x002ADA74
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetupFillSprite()
		{
			GameObject gameObject = new GameObject("FilledSprite");
			gameObject.layer = 12;
			gameObject.transform.SetParent(this.Transform, false);
			UISprite uisprite = gameObject.AddComponent<UISprite>();
			this.FillSprite = uisprite;
			uisprite.atlas = this.Owner.xui.GetAtlasByName("UIAtlas", "menu_empty");
			uisprite.spriteName = "menu_empty";
			uisprite.SetDimensions(50, 50);
			uisprite.color = Color.clear;
			uisprite.pivot = UIWidget.Pivot.Center;
			uisprite.fillDirection = UIBasicSprite.FillDirection.Radial360;
			uisprite.type = UIBasicSprite.Type.Filled;
			uisprite.depth = 300;
		}

		// Token: 0x060069B5 RID: 27061 RVA: 0x002AF914 File Offset: 0x002ADB14
		[PublicizedFrom(EAccessModifier.Private)]
		public void SetupSubSprite()
		{
			GameObject gameObject = new GameObject("SubSprite");
			gameObject.layer = 12;
			Transform transform = gameObject.transform;
			transform.SetParent(this.Transform, false);
			transform.localPosition = this.ScreenSettings.SubSpriteOffset;
			UISprite uisprite = gameObject.AddComponent<UISprite>();
			this.SubSprite = uisprite;
			uisprite.atlas = this.Owner.xui.GetAtlasByName("UIAtlas", "menu_empty");
			uisprite.spriteName = "menu_empty";
			int num = (int)this.ScreenSettings.SubSpriteSize;
			uisprite.SetDimensions(num, num);
			uisprite.color = Color.clear;
			uisprite.pivot = UIWidget.Pivot.Center;
			uisprite.depth = 300;
		}

		// Token: 0x060069B6 RID: 27062 RVA: 0x002AF9C5 File Offset: 0x002ADBC5
		public void RemoveFillSprite()
		{
			if (this.FillSprite != null)
			{
				UnityEngine.Object.Destroy(this.FillSprite.gameObject);
				this.FillSprite = null;
			}
		}

		// Token: 0x060069B7 RID: 27063 RVA: 0x002AF9EC File Offset: 0x002ADBEC
		public void RemoveSubSprite()
		{
			if (this.SubSprite != null)
			{
				UnityEngine.Object.Destroy(this.SubSprite.gameObject);
				this.SubSprite = null;
			}
		}

		// Token: 0x060069B8 RID: 27064 RVA: 0x002AFA14 File Offset: 0x002ADC14
		public int UpdateDepth(int depth)
		{
			this.Sprite.depth = depth;
			this.Label.depth = depth;
			if (this.FillSprite != null)
			{
				depth = (this.FillSprite.depth = depth + 1);
			}
			if (this.SubSprite != null)
			{
				depth = (this.SubSprite.depth = depth + 1);
			}
			return depth;
		}

		// Token: 0x04004FA2 RID: 20386
		public XUiC_OnScreenIcons Owner;

		// Token: 0x04004FA3 RID: 20387
		public NavObject NavObject;

		// Token: 0x04004FA4 RID: 20388
		public Transform Transform;

		// Token: 0x04004FA5 RID: 20389
		public UISprite Sprite;

		// Token: 0x04004FA6 RID: 20390
		public UISprite FillSprite;

		// Token: 0x04004FA7 RID: 20391
		public UISprite SubSprite;

		// Token: 0x04004FA8 RID: 20392
		public UILabel Label;

		// Token: 0x04004FA9 RID: 20393
		public bool ReadyForUnload;

		// Token: 0x04004FAA RID: 20394
		public NavObjectScreenSettings ScreenSettings;
	}
}
