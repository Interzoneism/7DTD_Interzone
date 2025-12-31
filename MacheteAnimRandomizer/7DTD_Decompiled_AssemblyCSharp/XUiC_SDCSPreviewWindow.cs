using System;
using HorizonBasedAmbientOcclusion;
using PI.NGSS;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000DF3 RID: 3571
[Preserve]
[PublicizedFrom(EAccessModifier.Internal)]
public class XUiC_SDCSPreviewWindow : XUiController
{
	// Token: 0x06006FEE RID: 28654 RVA: 0x002DA978 File Offset: 0x002D8B78
	public override void Init()
	{
		base.Init();
		this.textPreview = (XUiV_Texture)base.GetChildById("playerPreview").ViewComponent;
		this.textPreview.UpdateData();
		this.RenderTextureSystem = new RenderTextureSystem();
		this.zoomButton = base.GetChildById("zoomButton");
		if (this.zoomButton != null)
		{
			this.zoomButton.OnPress += this.ZoomButton_OnPress;
		}
		base.RegisterForInputStyleChanges();
	}

	// Token: 0x06006FEF RID: 28655 RVA: 0x002DA9F2 File Offset: 0x002D8BF2
	[PublicizedFrom(EAccessModifier.Private)]
	public void ZoomButton_OnPress(XUiController _sender, int _mouseButton)
	{
		this.toggleHeadZoom();
	}

	// Token: 0x06006FF0 RID: 28656 RVA: 0x002DA9FC File Offset: 0x002D8BFC
	public override void OnOpen()
	{
		base.OnOpen();
		this.originalPixelLightCount = QualitySettings.pixelLightCount;
		QualitySettings.pixelLightCount = 4;
		if (this.zoomButton != null)
		{
			this.zoomButton.ViewComponent.IsVisible = (PlatformManager.NativePlatform.Input.CurrentInputStyle == PlayerInputManager.InputStyle.Keyboard);
		}
	}

	// Token: 0x06006FF1 RID: 28657 RVA: 0x002DAA4A File Offset: 0x002D8C4A
	public override void OnClose()
	{
		base.OnClose();
		SDCSUtils.DestroyViz(this.previewTransform, false);
		this.RenderTextureSystem.Cleanup();
		this.lastProfile = "";
		this.lastFieldOfView = 54f;
		QualitySettings.pixelLightCount = this.originalPixelLightCount;
	}

	// Token: 0x06006FF2 RID: 28658 RVA: 0x002DAA8C File Offset: 0x002D8C8C
	public override void Update(float _dt)
	{
		base.Update(_dt);
		string text = ProfileSDF.CurrentProfileName();
		if (text != this.lastProfile)
		{
			this.Archetype = Archetype.GetArchetype(text);
			if (this.Archetype == null)
			{
				this.Archetype = ProfileSDF.CreateTempArchetype(text);
			}
			this.MakePreview();
			this.lastProfile = text;
			if (this.canZoom)
			{
				this.state = XUiC_SDCSPreviewWindow.ZoomStates.Head;
				this.SetToHeadZoom();
			}
		}
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard)
		{
			this.UpdateController();
		}
	}

	// Token: 0x06006FF3 RID: 28659 RVA: 0x002DAB0E File Offset: 0x002D8D0E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		base.InputStyleChanged(_oldStyle, _newStyle);
		if (this.zoomButton != null)
		{
			this.zoomButton.ViewComponent.IsVisible = (_newStyle == PlayerInputManager.InputStyle.Keyboard);
		}
	}

	// Token: 0x06006FF4 RID: 28660 RVA: 0x002DAB34 File Offset: 0x002D8D34
	[PublicizedFrom(EAccessModifier.Protected)]
	public void UpdateController()
	{
		float value = base.xui.playerUI.playerInput.GUIActions.TriggerAxis.Value;
		if (value != 0f)
		{
			this.CameraRotate(-value);
		}
		if (base.xui.playerUI.playerInput.GUIActions.HalfStack.WasPressed)
		{
			this.toggleHeadZoom();
		}
	}

	// Token: 0x06006FF5 RID: 28661 RVA: 0x002DAB98 File Offset: 0x002D8D98
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnDragged(EDragType _dragType, Vector2 _mousePositionDelta)
	{
		base.OnDragged(_dragType, _mousePositionDelta);
		float x = _mousePositionDelta.x;
		if (base.xui.playerUI.CursorController.GetMouseButton(UICamera.MouseButton.RightButton))
		{
			this.CameraRotate(x);
		}
	}

	// Token: 0x06006FF6 RID: 28662 RVA: 0x002DABD4 File Offset: 0x002D8DD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void CameraVerticalPan(float _value)
	{
		this.RenderCamera.transform.localPosition -= new Vector3(0f, _value, 0f);
		if (this.RenderCamera.transform.localPosition.y < -1.5f)
		{
			this.RenderCamera.transform.localPosition = new Vector3(this.RenderCamera.transform.localPosition.x, -1.5f, this.RenderCamera.transform.localPosition.z);
			return;
		}
		if (this.RenderCamera.transform.localPosition.y > 0f)
		{
			this.RenderCamera.transform.localPosition = new Vector3(this.RenderCamera.transform.localPosition.x, 0f, this.RenderCamera.transform.localPosition.z);
		}
	}

	// Token: 0x06006FF7 RID: 28663 RVA: 0x002DACD0 File Offset: 0x002D8ED0
	[PublicizedFrom(EAccessModifier.Private)]
	public void CameraRotate(float _value)
	{
		this.RenderCamera.transform.RotateAround(this.previewTransform.transform.position, Vector3.up, _value);
		this.RenderTextureSystem.LightGO.transform.RotateAround(this.previewTransform.transform.position, Vector3.up, _value);
	}

	// Token: 0x06006FF8 RID: 28664 RVA: 0x002DAD2E File Offset: 0x002D8F2E
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnScrolled(float _delta)
	{
		base.OnScrolled(_delta);
	}

	// Token: 0x06006FF9 RID: 28665 RVA: 0x002DAD38 File Offset: 0x002D8F38
	public void MakePreview()
	{
		SDCSUtils.CreateVizUI(this.Archetype, ref this.previewTransform, ref this.uiBoneCatalog);
		this.previewTransform.GetComponentInChildren<Animator>().Update(0f);
		this.init();
		this.previewTransform.transform.parent = this.RenderTextureSystem.TargetGO.transform;
		this.previewTransform.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
		this.previewTransform.transform.localPosition = new Vector3(0f, -0.9f, 0f);
		this.characterGazeController = this.previewTransform.GetComponentInChildren<CharacterGazeController>();
		Utils.SetLayerRecursively(this.RenderTextureSystem.TargetGO, 11);
		this.textPreview.Texture = this.RenderTexture;
	}

	// Token: 0x06006FFA RID: 28666 RVA: 0x002DAE14 File Offset: 0x002D9014
	[PublicizedFrom(EAccessModifier.Private)]
	public void toggleHeadZoom()
	{
		switch (this.state)
		{
		case XUiC_SDCSPreviewWindow.ZoomStates.Eyes:
			this.state = XUiC_SDCSPreviewWindow.ZoomStates.Head;
			break;
		case XUiC_SDCSPreviewWindow.ZoomStates.Head:
			this.state = XUiC_SDCSPreviewWindow.ZoomStates.Chest;
			break;
		case XUiC_SDCSPreviewWindow.ZoomStates.Chest:
			this.state = XUiC_SDCSPreviewWindow.ZoomStates.FullBody;
			break;
		case XUiC_SDCSPreviewWindow.ZoomStates.FullBody:
			this.state = XUiC_SDCSPreviewWindow.ZoomStates.Eyes;
			break;
		}
		switch (this.state)
		{
		case XUiC_SDCSPreviewWindow.ZoomStates.Eyes:
			this.SetToEyeZoom();
			return;
		case XUiC_SDCSPreviewWindow.ZoomStates.Head:
			this.SetToHeadZoom();
			return;
		case XUiC_SDCSPreviewWindow.ZoomStates.Chest:
			this.SetToChestZoom();
			return;
		case XUiC_SDCSPreviewWindow.ZoomStates.FullBody:
			this.SetToFullBodyZoom();
			return;
		default:
			return;
		}
	}

	// Token: 0x06006FFB RID: 28667 RVA: 0x002DAE9C File Offset: 0x002D909C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetToFullBodyZoom()
	{
		this.RenderCamera.transform.SetLocalPositionAndRotation(this.originalCamPosition, Quaternion.AngleAxis(19f, new Vector3(1f, 0f, 0f)) * this.originalCamRotation);
		this.RenderTextureSystem.LightGO.transform.SetLocalPositionAndRotation(this.originalLightPosition, this.originalLightRotation);
		this.RenderCamera.fieldOfView = 54f;
		this.SetCameraInitialRotationOffset();
	}

	// Token: 0x06006FFC RID: 28668 RVA: 0x002DAF20 File Offset: 0x002D9120
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetToHeadZoom()
	{
		this.RenderCamera.transform.SetLocalPositionAndRotation(this.originalCamPosition, Quaternion.AngleAxis(1.5f, new Vector3(1f, 0f, 0f)) * this.originalCamRotation);
		this.RenderTextureSystem.LightGO.transform.SetLocalPositionAndRotation(this.originalLightPosition, this.originalLightRotation);
		this.RenderCamera.fieldOfView = 12f;
		this.RenderTextureSystem.TargetGO.transform.localPosition = new Vector3(0.015f, -0.78f, 2.14f);
		this.SetCameraInitialRotationOffset();
	}

	// Token: 0x06006FFD RID: 28669 RVA: 0x002DAFCC File Offset: 0x002D91CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetToChestZoom()
	{
		this.RenderCamera.transform.SetLocalPositionAndRotation(this.originalCamPosition, Quaternion.AngleAxis(5f, new Vector3(1f, 0f, 0f)) * this.originalCamRotation);
		this.RenderTextureSystem.LightGO.transform.SetLocalPositionAndRotation(this.originalLightPosition, this.originalLightRotation);
		this.RenderCamera.fieldOfView = 20f;
		this.RenderTextureSystem.TargetGO.transform.localPosition = new Vector3(0.02f, -0.78f, 2.14f);
		this.SetCameraInitialRotationOffset();
	}

	// Token: 0x06006FFE RID: 28670 RVA: 0x002DB078 File Offset: 0x002D9278
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetToEyeZoom()
	{
		this.RenderCamera.transform.SetLocalPositionAndRotation(this.originalCamPosition, this.originalCamRotation);
		this.RenderTextureSystem.LightGO.transform.SetLocalPositionAndRotation(this.originalLightPosition, this.originalLightRotation);
		this.RenderCamera.fieldOfView = 6f;
		this.RenderTextureSystem.TargetGO.transform.localPosition = new Vector3(0.015f, -0.78f, 2.14f);
		this.SetCameraInitialRotationOffset();
	}

	// Token: 0x06006FFF RID: 28671 RVA: 0x002DB104 File Offset: 0x002D9304
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetCameraInitialRotationOffset()
	{
		this.RenderCamera.transform.RotateAround(this.previewTransform.transform.position, Vector3.up, -30f);
		this.RenderTextureSystem.LightGO.transform.RotateAround(this.previewTransform.transform.position, Vector3.up, -30f);
		if (this.characterGazeController != null)
		{
			this.characterGazeController.SnapNextUpdate();
		}
	}

	// Token: 0x06007000 RID: 28672 RVA: 0x002DB184 File Offset: 0x002D9384
	public void ZoomToHead()
	{
		if (this.state == XUiC_SDCSPreviewWindow.ZoomStates.Head)
		{
			return;
		}
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard && base.xui.playerUI.playerInput.GUIActions.TriggerAxis.Value != 0f)
		{
			return;
		}
		this.state = XUiC_SDCSPreviewWindow.ZoomStates.Head;
		this.SetToHeadZoom();
	}

	// Token: 0x06007001 RID: 28673 RVA: 0x002DB1E4 File Offset: 0x002D93E4
	public void ZoomToEye()
	{
		if (PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard && base.xui.playerUI.playerInput.GUIActions.TriggerAxis.Value != 0f)
		{
			return;
		}
		this.state = XUiC_SDCSPreviewWindow.ZoomStates.Eyes;
		this.SetToEyeZoom();
	}

	// Token: 0x06007002 RID: 28674 RVA: 0x002DB238 File Offset: 0x002D9438
	[PublicizedFrom(EAccessModifier.Private)]
	public void init()
	{
		if (this.RenderTextureSystem.ParentGO == null)
		{
			this.RenderTextureSystem.Create("characterpreview", new GameObject(), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), this.textPreview.Size, true, true, 1f);
			this.RenderTextureSystem.TargetGO.transform.localPosition = new Vector3(0f, -0.75f, 2.15f);
			this.RenderTexture = this.RenderTextureSystem.RenderTex;
			this.RenderCamera = this.RenderTextureSystem.CameraGO.GetComponent<Camera>();
			this.RenderCamera.orthographic = false;
			this.RenderCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
			this.RenderCamera.fieldOfView = 54f;
			this.RenderCamera.renderingPath = RenderingPath.DeferredShading;
			this.RenderCamera.tag = "MainCamera";
			this.RenderCamera.gameObject.AddComponent<StreamingController>();
			this.RenderTextureSystem.CameraGO.AddComponent<NGSS_Local>().NGSS_PCSS_SOFTNESS_NEAR = 0.05f;
			HBAO hbao = this.RenderTextureSystem.CameraGO.AddComponent<HBAO>();
			hbao.SetAoPerPixelNormals(HBAO.PerPixelNormals.Reconstruct);
			hbao.SetAoIntensity(0.5f);
			this.RenderTextureSystem.LightGO.GetComponent<Light>().enabled = false;
			GameObject gameObject = new GameObject("Key Light", new Type[]
			{
				typeof(Light)
			});
			gameObject.transform.SetParent(this.RenderTextureSystem.LightGO.transform, false);
			gameObject.transform.SetPositionAndRotation(new Vector3(0.25f, 0.475f, 0.62f), Quaternion.Euler(33f, -8f, 0f));
			gameObject.AddComponent<NGSS_Directional>().NGSS_PCSS_ENABLED = true;
			Light component = gameObject.GetComponent<Light>();
			component.color = new Color(0.9f, 0.8f, 0.7f, 1f);
			component.type = LightType.Spot;
			component.range = 20f;
			component.spotAngle = 60f;
			component.intensity = 1.5f;
			component.shadows = LightShadows.Hard;
			component.shadowStrength = 0.2f;
			component.shadowBias = 0.005f;
			NGSS_FrustumShadows ngss_FrustumShadows = this.RenderTextureSystem.CameraGO.AddComponent<NGSS_FrustumShadows>();
			ngss_FrustumShadows.mainShadowsLight = component;
			ngss_FrustumShadows.m_fastBlur = false;
			ngss_FrustumShadows.m_shadowsBlur = 1f;
			ngss_FrustumShadows.m_shadowsBlurIterations = 4;
			ngss_FrustumShadows.m_rayThickness = 0.025f;
			GameObject gameObject2 = new GameObject("Fill Light", new Type[]
			{
				typeof(Light)
			});
			gameObject2.transform.SetParent(this.RenderTextureSystem.LightGO.transform, false);
			gameObject2.transform.SetPositionAndRotation(new Vector3(-1.15f, 1.4f, 1f), Quaternion.Euler(50f, 45f, 0f));
			Light component2 = gameObject2.GetComponent<Light>();
			component2.color = new Color(1f, 1f, 1f, 1f);
			component2.type = LightType.Spot;
			component2.range = 20f;
			component2.spotAngle = 60f;
			component2.intensity = 0.5f;
			component2.shadows = LightShadows.Hard;
			component2.shadowStrength = 0.2f;
			component2.shadowBias = 0.005f;
			GameObject gameObject3 = new GameObject("Fill 2 Light", new Type[]
			{
				typeof(Light)
			});
			gameObject3.transform.SetParent(this.RenderTextureSystem.LightGO.transform, false);
			gameObject3.transform.SetPositionAndRotation(new Vector3(0f, -1.5f, -0.5f), Quaternion.Euler(-15f, 0f, 0f));
			Light component3 = gameObject3.GetComponent<Light>();
			component3.color = new Color(1f, 1f, 1f, 1f);
			component3.type = LightType.Spot;
			component3.range = 20f;
			component3.spotAngle = 60f;
			component3.intensity = 0.5f;
			component3.shadows = LightShadows.Hard;
			component3.shadowStrength = 0.2f;
			component3.shadowBias = 0.005f;
			GameObject gameObject4 = new GameObject("Back Light", new Type[]
			{
				typeof(Light)
			});
			gameObject4.transform.SetParent(this.RenderTextureSystem.LightGO.transform, false);
			gameObject4.transform.SetPositionAndRotation(new Vector3(-0.6f, 0.75f, 2.6f), Quaternion.Euler(55f, 133f, 0f));
			Light component4 = gameObject4.GetComponent<Light>();
			component4.color = new Color(0.4f, 0.75f, 1f, 1f);
			component4.type = LightType.Spot;
			component4.spotAngle = 60f;
			component4.range = 20f;
			component4.intensity = 1.5f;
			component4.shadows = LightShadows.Hard;
			component4.shadowStrength = 0.2f;
			component4.shadowBias = 0.005f;
			this.originalCamPosition = this.RenderCamera.transform.localPosition;
			this.originalCamRotation = this.RenderCamera.transform.localRotation;
			this.originalLightPosition = this.RenderTextureSystem.LightGO.transform.localPosition;
			this.originalLightRotation = this.RenderTextureSystem.LightGO.transform.localRotation;
			this.RenderCamera.transform.localPosition = new Vector3(0f, -0.75f, 0f);
		}
	}

	// Token: 0x06007003 RID: 28675 RVA: 0x002DB7D8 File Offset: 0x002D99D8
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (flag)
		{
			return flag;
		}
		if (name == "can_zoom")
		{
			this.canZoom = StringParsers.ParseBool(value, 0, -1, true);
			return true;
		}
		return false;
	}

	// Token: 0x04005508 RID: 21768
	public RenderTextureSystem RenderTextureSystem;

	// Token: 0x04005509 RID: 21769
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Texture textPreview;

	// Token: 0x0400550A RID: 21770
	public RenderTexture RenderTexture;

	// Token: 0x0400550B RID: 21771
	public Camera RenderCamera;

	// Token: 0x0400550C RID: 21772
	public Transform TargetTransform;

	// Token: 0x0400550D RID: 21773
	public GameObject RotateTable;

	// Token: 0x0400550E RID: 21774
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject previewTransform;

	// Token: 0x0400550F RID: 21775
	[PublicizedFrom(EAccessModifier.Private)]
	public string lastProfile;

	// Token: 0x04005510 RID: 21776
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastFieldOfView;

	// Token: 0x04005511 RID: 21777
	public Archetype Archetype;

	// Token: 0x04005512 RID: 21778
	[PublicizedFrom(EAccessModifier.Private)]
	public CharacterGazeController characterGazeController;

	// Token: 0x04005513 RID: 21779
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController zoomButton;

	// Token: 0x04005514 RID: 21780
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 originalCamPosition;

	// Token: 0x04005515 RID: 21781
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 originalLightPosition;

	// Token: 0x04005516 RID: 21782
	[PublicizedFrom(EAccessModifier.Private)]
	public Quaternion originalCamRotation;

	// Token: 0x04005517 RID: 21783
	[PublicizedFrom(EAccessModifier.Private)]
	public Quaternion originalLightRotation;

	// Token: 0x04005518 RID: 21784
	[PublicizedFrom(EAccessModifier.Private)]
	public bool canZoom = true;

	// Token: 0x04005519 RID: 21785
	[PublicizedFrom(EAccessModifier.Private)]
	public int originalPixelLightCount;

	// Token: 0x0400551A RID: 21786
	[PublicizedFrom(EAccessModifier.Private)]
	public SDCSUtils.TransformCatalog uiBoneCatalog;

	// Token: 0x0400551B RID: 21787
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SDCSPreviewWindow.ZoomStates state = XUiC_SDCSPreviewWindow.ZoomStates.FullBody;

	// Token: 0x0400551C RID: 21788
	[PublicizedFrom(EAccessModifier.Private)]
	public float baseOrtho = 1f;

	// Token: 0x02000DF4 RID: 3572
	[PublicizedFrom(EAccessModifier.Private)]
	public enum ZoomStates
	{
		// Token: 0x0400551E RID: 21790
		Eyes,
		// Token: 0x0400551F RID: 21791
		Head,
		// Token: 0x04005520 RID: 21792
		Chest,
		// Token: 0x04005521 RID: 21793
		FullBody
	}
}
