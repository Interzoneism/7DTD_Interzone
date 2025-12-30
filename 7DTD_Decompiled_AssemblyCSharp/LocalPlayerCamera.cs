using System;
using UnityEngine;

// Token: 0x0200057B RID: 1403
[RequireComponent(typeof(Camera))]
public class LocalPlayerCamera : MonoBehaviour
{
	// Token: 0x14000039 RID: 57
	// (add) Token: 0x06002D53 RID: 11603 RVA: 0x0012E47C File Offset: 0x0012C67C
	// (remove) Token: 0x06002D54 RID: 11604 RVA: 0x0012E4B4 File Offset: 0x0012C6B4
	public event Action<LocalPlayerCamera> PreCull;

	// Token: 0x1400003A RID: 58
	// (add) Token: 0x06002D55 RID: 11605 RVA: 0x0012E4EC File Offset: 0x0012C6EC
	// (remove) Token: 0x06002D56 RID: 11606 RVA: 0x0012E524 File Offset: 0x0012C724
	public event Action<LocalPlayerCamera> PreRender;

	// Token: 0x06002D57 RID: 11607 RVA: 0x0012E55C File Offset: 0x0012C75C
	public static LocalPlayerCamera AddToCamera(Camera camera, LocalPlayerCamera.CameraType camType)
	{
		LocalPlayerCamera localPlayerCamera = camera.gameObject.AddMissingComponent<LocalPlayerCamera>();
		if (camType != LocalPlayerCamera.CameraType.UI)
		{
			localPlayerCamera.Init(camType);
		}
		return localPlayerCamera;
	}

	// Token: 0x06002D58 RID: 11608 RVA: 0x0012E581 File Offset: 0x0012C781
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init(LocalPlayerCamera.CameraType camType)
	{
		this.camera = base.GetComponent<Camera>();
		this.cameraType = camType;
		if (camType != LocalPlayerCamera.CameraType.UI)
		{
			this.camera.allowDynamicResolution = true;
		}
		this.entityPlayerLocal = base.GetComponentInParent<EntityPlayerLocal>();
		this.localPlayer = base.GetComponentInParent<LocalPlayer>();
	}

	// Token: 0x06002D59 RID: 11609 RVA: 0x0012E5BE File Offset: 0x0012C7BE
	public void SetUI(LocalPlayerUI ui)
	{
		this.playerUI = ui;
	}

	// Token: 0x06002D5A RID: 11610 RVA: 0x0012E5C7 File Offset: 0x0012C7C7
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		this.playerUI = base.GetComponentInChildren<LocalPlayerUI>();
		if (this.playerUI)
		{
			this.Init(LocalPlayerCamera.CameraType.UI);
			this.playerUI.UpdateChildCameraIndices();
		}
		LocalPlayerManager.OnLocalPlayersChanged += this.HandleLocalPlayersChanged;
	}

	// Token: 0x06002D5B RID: 11611 RVA: 0x0012E605 File Offset: 0x0012C805
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		LocalPlayerManager.OnLocalPlayersChanged -= this.HandleLocalPlayersChanged;
	}

	// Token: 0x06002D5C RID: 11612 RVA: 0x0012E618 File Offset: 0x0012C818
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsAttachedToLocalPlayer()
	{
		return this.localPlayer != null;
	}

	// Token: 0x06002D5D RID: 11613 RVA: 0x0012E626 File Offset: 0x0012C826
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleLocalPlayersChanged()
	{
		this.ModifyCameraProperties();
	}

	// Token: 0x06002D5E RID: 11614 RVA: 0x0012E630 File Offset: 0x0012C830
	[PublicizedFrom(EAccessModifier.Private)]
	public void ModifyCameraProperties()
	{
		this.camera.enabled = true;
		if (this.IsAttachedToLocalPlayer())
		{
			this.camera.fieldOfView = (float)Constants.cDefaultCameraFieldOfView * LocalPlayerCamera.splitScreenFOVFactors;
			return;
		}
		UIRect[] componentsInChildren = base.GetComponentsInChildren<UIRect>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].UpdateAnchors();
		}
		this.SetCameraDepth();
	}

	// Token: 0x06002D5F RID: 11615 RVA: 0x0012E68C File Offset: 0x0012C88C
	public void SetCameraDepth()
	{
		this.camera.depth = 1.01f + (float)this.playerUI.playerIndex * 0.01f + (float)this.uiChildIndex * 0.001f;
	}

	// Token: 0x06002D60 RID: 11616 RVA: 0x0012E6C0 File Offset: 0x0012C8C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPreCull()
	{
		if (this.cameraType == LocalPlayerCamera.CameraType.Main)
		{
			OcclusionManager.Instance.LocalPlayerOnPreCull();
		}
		if (this.PreCull != null)
		{
			this.PreCull(this);
		}
		if (GameRenderManager.dynamicIsEnabled && (this.cameraType == LocalPlayerCamera.CameraType.Main || this.cameraType == LocalPlayerCamera.CameraType.Weapon))
		{
			this.camera.targetTexture = this.entityPlayerLocal.renderManager.GetDynamicRenderTexture();
		}
		if (this.cameraType == LocalPlayerCamera.CameraType.Main)
		{
			this.entityPlayerLocal.renderManager.UpscalingPreCull();
		}
	}

	// Token: 0x06002D61 RID: 11617 RVA: 0x0012E741 File Offset: 0x0012C941
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPreRender()
	{
		if (this.PreRender != null)
		{
			this.PreRender(this);
		}
	}

	// Token: 0x040023EA RID: 9194
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly float splitScreenFOVFactors = 1f;

	// Token: 0x040023EB RID: 9195
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Camera camera;

	// Token: 0x040023EC RID: 9196
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LocalPlayerCamera.CameraType cameraType;

	// Token: 0x040023ED RID: 9197
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LocalPlayerUI playerUI;

	// Token: 0x040023EE RID: 9198
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityPlayerLocal entityPlayerLocal;

	// Token: 0x040023EF RID: 9199
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public LocalPlayer localPlayer;

	// Token: 0x040023F0 RID: 9200
	public int uiChildIndex;

	// Token: 0x0200057C RID: 1404
	public enum CameraType
	{
		// Token: 0x040023F2 RID: 9202
		None,
		// Token: 0x040023F3 RID: 9203
		Main,
		// Token: 0x040023F4 RID: 9204
		Weapon,
		// Token: 0x040023F5 RID: 9205
		UI
	}
}
