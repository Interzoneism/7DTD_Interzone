using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000579 RID: 1401
public class LocalPlayer : MonoBehaviour
{
	// Token: 0x17000482 RID: 1154
	// (get) Token: 0x06002D40 RID: 11584 RVA: 0x0012E226 File Offset: 0x0012C426
	// (set) Token: 0x06002D41 RID: 11585 RVA: 0x0012E22E File Offset: 0x0012C42E
	public EntityPlayerLocal entityPlayerLocal { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000483 RID: 1155
	// (get) Token: 0x06002D42 RID: 11586 RVA: 0x0012E237 File Offset: 0x0012C437
	// (set) Token: 0x06002D43 RID: 11587 RVA: 0x0012E23F File Offset: 0x0012C43F
	public LocalPlayerUI playerUI { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06002D44 RID: 11588 RVA: 0x0012E248 File Offset: 0x0012C448
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.avatarController = base.GetComponentInChildren<AvatarLocalPlayerController>();
		this.entityPlayerLocal = base.GetComponent<EntityPlayerLocal>();
		this.playerUI = LocalPlayerUI.GetUIForPlayer(this.entityPlayerLocal);
		LocalPlayerCamera.CameraType camType = LocalPlayerCamera.CameraType.Main;
		Camera[] componentsInChildren = base.GetComponentsInChildren<Camera>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Camera camera = componentsInChildren[i];
			if (i > 0)
			{
				camType = LocalPlayerCamera.CameraType.Weapon;
			}
			if (camera.name != "FinalCamera")
			{
				LocalPlayerCamera.AddToCamera(camera, camType).SetUI(this.playerUI);
			}
		}
		Transform transform = this.entityPlayerLocal.playerCamera.transform;
		Transform transform2 = transform.Find("ScreenEffectsWithDepth");
		if (transform2 != null)
		{
			this.SetupLocalPlayerVisual(transform2.Find("UnderwaterHaze"));
		}
		Transform transform3 = transform.Find("effect_refract_plane");
		if (transform3 != null)
		{
			transform3.GetComponent<MeshRenderer>().material.SetInt("_ZTest", 8);
			this.SetupLocalPlayerVisual(transform3);
		}
		this.SetupLocalPlayerVisual(transform.Find("effect_underwater_debris"));
		this.SetupLocalPlayerVisual(transform.Find("effect_dropletsParticle"));
		this.SetupLocalPlayerVisual(transform.Find("effect_water_fade"));
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupLocalPlayerVisual(Transform _transform)
	{
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x0012E36C File Offset: 0x0012C56C
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator Start()
	{
		this.DispatchLocalPlayersChanged();
		while (this.avatarController == null)
		{
			yield return null;
			this.avatarController = base.GetComponentInChildren<AvatarLocalPlayerController>();
		}
		yield break;
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x0012E37B File Offset: 0x0012C57B
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		LocalPlayerManager.OnLocalPlayersChanged += this.HandleLocalPlayersChanged;
	}

	// Token: 0x06002D48 RID: 11592 RVA: 0x0012E38E File Offset: 0x0012C58E
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		LocalPlayerManager.OnLocalPlayersChanged -= this.HandleLocalPlayersChanged;
	}

	// Token: 0x06002D49 RID: 11593 RVA: 0x0012E3A1 File Offset: 0x0012C5A1
	[PublicizedFrom(EAccessModifier.Private)]
	public void DispatchLocalPlayersChanged()
	{
		LocalPlayerManager.LocalPlayersChanged();
	}

	// Token: 0x06002D4A RID: 11594 RVA: 0x0012E3A8 File Offset: 0x0012C5A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleLocalPlayersChanged()
	{
		int num = 0;
		Camera[] componentsInChildren = base.GetComponentsInChildren<Camera>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].depth = -1f + (float)(this.playerUI.playerIndex * 2 + num++) * 0.01f;
		}
	}

	// Token: 0x06002D4B RID: 11595 RVA: 0x0012E3F4 File Offset: 0x0012C5F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		this.DispatchLocalPlayersChanged();
	}

	// Token: 0x040023E2 RID: 9186
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AvatarLocalPlayerController avatarController;
}
