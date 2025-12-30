using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020010C4 RID: 4292
public class CameraMatrixOverride : MonoBehaviour
{
	// Token: 0x06008716 RID: 34582 RVA: 0x0036AFD8 File Offset: 0x003691D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		if (this.referenceCamera == null && !base.TryGetComponent<Camera>(out this.referenceCamera))
		{
			Debug.LogError("Failed to get Camera. The CameraMatrixOverride script must be attached to a GameObject with a Camera component.");
			base.enabled = false;
			return;
		}
		this.originalNearClip = this.referenceCamera.nearClipPlane;
	}

	// Token: 0x06008717 RID: 34583 RVA: 0x0036B024 File Offset: 0x00369224
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		if (this.referenceCamera == null)
		{
			return;
		}
		this.referenceCamera.nearClipPlane = this.originalNearClip;
		this.RestoreChildSettings();
	}

	// Token: 0x06008718 RID: 34584 RVA: 0x0036B04C File Offset: 0x0036924C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateRendererList()
	{
		bool flag = this.advancedSettings.enableBoundsPadding && this.referenceCamera.fieldOfView < this.fov;
		this.renderersToRestore.Clear();
		foreach (Renderer renderer in this.overriddenRenderers)
		{
			if (renderer != null)
			{
				this.renderersToRestore.Add(renderer);
			}
		}
		this.overriddenRenderers.Clear();
		base.GetComponentsInChildren<Renderer>(this.overriddenRenderers);
		Vector3 b = new Vector3(this.advancedSettings.boundsPadding, this.advancedSettings.boundsPadding, this.advancedSettings.boundsPadding);
		foreach (Renderer renderer2 in this.overriddenRenderers)
		{
			this.renderersToRestore.Remove(renderer2);
			CameraMatrixOverride.RendererSettings rendererSettings;
			if (!this.rendererSettingsMap.TryGetValue(renderer2, out rendererSettings))
			{
				rendererSettings = new CameraMatrixOverride.RendererSettings();
				rendererSettings.originalShadowCastingMode = renderer2.shadowCastingMode;
				rendererSettings.originalProperties = new MaterialPropertyBlock();
				rendererSettings.overriddenProperties = new MaterialPropertyBlock();
				this.rendererSettingsMap[renderer2] = rendererSettings;
			}
			if (this.advancedSettings.enableChildShadows && rendererSettings.shadowModeDirty)
			{
				renderer2.shadowCastingMode = rendererSettings.originalShadowCastingMode;
				rendererSettings.shadowModeDirty = false;
			}
			else if (!this.advancedSettings.enableChildShadows && !rendererSettings.shadowModeDirty)
			{
				renderer2.shadowCastingMode = ShadowCastingMode.Off;
				rendererSettings.shadowModeDirty = true;
			}
			if (flag && !(renderer2 is ParticleSystemRenderer))
			{
				renderer2.ResetBounds();
				Bounds bounds = renderer2.bounds;
				bounds.extents += b;
				renderer2.bounds = bounds;
				rendererSettings.boundsDirty = true;
			}
			else if (rendererSettings.boundsDirty)
			{
				renderer2.ResetBounds();
				rendererSettings.boundsDirty = false;
			}
		}
		foreach (Renderer renderer3 in this.renderersToRestore)
		{
			CameraMatrixOverride.RendererSettings rendererSettings2;
			if (this.rendererSettingsMap.TryGetValue(renderer3, out rendererSettings2))
			{
				if (rendererSettings2.shadowModeDirty)
				{
					renderer3.shadowCastingMode = rendererSettings2.originalShadowCastingMode;
					rendererSettings2.shadowModeDirty = false;
				}
				if (rendererSettings2.boundsDirty)
				{
					renderer3.ResetBounds();
					rendererSettings2.boundsDirty = false;
				}
			}
		}
		this.renderersToRestore.Clear();
	}

	// Token: 0x06008719 RID: 34585 RVA: 0x0036B31C File Offset: 0x0036951C
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.advancedSettings.updateTiming == CameraMatrixOverride.UpdateTiming.LateUpdate)
		{
			this.UpdateRendererList();
		}
	}

	// Token: 0x0600871A RID: 34586 RVA: 0x0036B331 File Offset: 0x00369531
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPreCull()
	{
		if (this.advancedSettings.updateTiming == CameraMatrixOverride.UpdateTiming.OnPreCull)
		{
			this.UpdateRendererList();
		}
	}

	// Token: 0x0600871B RID: 34587 RVA: 0x0036B348 File Offset: 0x00369548
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPreRender()
	{
		if (this.advancedSettings.updateTiming == CameraMatrixOverride.UpdateTiming.OnPreRender)
		{
			this.UpdateRendererList();
		}
		this.referenceCamera.nearClipPlane = (this.advancedSettings.enableNearClipOverride ? this.nearClipOverride : this.originalNearClip);
		Matrix4x4 projectionMatrix = this.referenceCamera.projectionMatrix;
		Matrix4x4 matrix4x = Matrix4x4.Perspective(this.fov, this.referenceCamera.aspect, this.referenceCamera.nearClipPlane * this.nearClipFactor, this.referenceCamera.farClipPlane * this.advancedSettings.farClipFactor);
		ref Matrix4x4 ptr = ref matrix4x;
		ptr[0, 2] = ptr[0, 2] + this.advancedSettings.jitterFactor * (projectionMatrix[0, 2] - matrix4x[0, 2]);
		ptr = ref matrix4x;
		ptr[1, 2] = ptr[1, 2] + this.advancedSettings.jitterFactor * (projectionMatrix[1, 2] - matrix4x[1, 2]);
		Matrix4x4 matrix4x2;
		switch (this.advancedSettings.projectionMode)
		{
		case CameraMatrixOverride.ProjectionMode.Custom:
			matrix4x2 = matrix4x;
			break;
		case CameraMatrixOverride.ProjectionMode.Reference:
			matrix4x2 = projectionMatrix;
			break;
		case CameraMatrixOverride.ProjectionMode.ReferenceNonJittered:
			matrix4x2 = this.referenceCamera.nonJitteredProjectionMatrix;
			break;
		default:
			matrix4x2 = projectionMatrix;
			break;
		}
		Matrix4x4 matrix4x3 = matrix4x2;
		if (this.advancedSettings.depthScaleFactor != 1f)
		{
			Matrix4x4 identity = Matrix4x4.identity;
			identity.m22 = this.advancedSettings.depthScaleFactor;
			matrix4x3 = identity * matrix4x3;
		}
		matrix4x3 = GL.GetGPUProjectionMatrix(matrix4x3, true);
		Matrix4x4 worldToCameraMatrix = this.referenceCamera.worldToCameraMatrix;
		Matrix4x4 value = matrix4x3 * worldToCameraMatrix;
		foreach (Renderer renderer in this.overriddenRenderers)
		{
			CameraMatrixOverride.RendererSettings rendererSettings;
			if (!this.rendererSettingsMap.TryGetValue(renderer, out rendererSettings))
			{
				Debug.LogError("[CMO] Failed to retrieve RendererSettings for overridden renderer");
			}
			else
			{
				renderer.GetPropertyBlock(rendererSettings.originalProperties);
				renderer.GetPropertyBlock(rendererSettings.overriddenProperties);
				MaterialPropertyBlock overriddenProperties = rendererSettings.overriddenProperties;
				overriddenProperties.SetMatrix("unity_MatrixVP", value);
				renderer.SetPropertyBlock(overriddenProperties);
			}
		}
	}

	// Token: 0x0600871C RID: 34588 RVA: 0x0036B574 File Offset: 0x00369774
	[PublicizedFrom(EAccessModifier.Private)]
	public void RestoreChildSettings()
	{
		foreach (Renderer renderer in this.overriddenRenderers)
		{
			CameraMatrixOverride.RendererSettings rendererSettings;
			if (renderer != null && this.rendererSettingsMap.TryGetValue(renderer, out rendererSettings))
			{
				if (rendererSettings.shadowModeDirty)
				{
					renderer.shadowCastingMode = rendererSettings.originalShadowCastingMode;
					rendererSettings.shadowModeDirty = false;
				}
				if (rendererSettings.boundsDirty)
				{
					renderer.ResetBounds();
					rendererSettings.boundsDirty = false;
				}
			}
		}
	}

	// Token: 0x0600871D RID: 34589 RVA: 0x0036B60C File Offset: 0x0036980C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPostRender()
	{
		foreach (Renderer renderer in this.overriddenRenderers)
		{
			CameraMatrixOverride.RendererSettings rendererSettings;
			if (!(renderer == null) && this.rendererSettingsMap.TryGetValue(renderer, out rendererSettings))
			{
				renderer.SetPropertyBlock(rendererSettings.originalProperties);
			}
		}
	}

	// Token: 0x040068E3 RID: 26851
	[Tooltip("The overridden FoV to use when rendering any child Renderers in the hierarchy beneath the Camera this script is attached to.")]
	public float fov = 45f;

	// Token: 0x040068E4 RID: 26852
	[Range(0.01f, 1f)]
	[Tooltip("The overridden near-clip distance to use when this script is enabled. Note this applies to the Camera as a whole, rather than specifically targeting child Renderers.")]
	public float nearClipOverride = 0.01f;

	// Token: 0x040068E5 RID: 26853
	[Range(1E-45f, 8f)]
	[Tooltip("A value of 1 results in normal rendering behaviour. Higher values effectively squash the depth of child Renderers towards the camera; this reduces the likelihood of clipping into environment geometry, but can distort certain screen effects such as reflections. A value of 2 seems to provide a good balance between reducing clipping and minimising distortion of screen effects.")]
	public float nearClipFactor = 2f;

	// Token: 0x040068E6 RID: 26854
	[Tooltip("An assortment of parameters left over from earlier prototyping. They remain exposed for debug purposes if ever required; otherwise it is not recommended to change them away from their default values.")]
	public CameraMatrixOverride.AdvancedSettings advancedSettings = new CameraMatrixOverride.AdvancedSettings();

	// Token: 0x040068E7 RID: 26855
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Camera referenceCamera;

	// Token: 0x040068E8 RID: 26856
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<Renderer, CameraMatrixOverride.RendererSettings> rendererSettingsMap = new Dictionary<Renderer, CameraMatrixOverride.RendererSettings>();

	// Token: 0x040068E9 RID: 26857
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Renderer> overriddenRenderers = new List<Renderer>();

	// Token: 0x040068EA RID: 26858
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public HashSet<Renderer> renderersToRestore = new HashSet<Renderer>();

	// Token: 0x040068EB RID: 26859
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float originalNearClip = 0.0751f;

	// Token: 0x020010C5 RID: 4293
	public enum ProjectionMode
	{
		// Token: 0x040068ED RID: 26861
		Custom,
		// Token: 0x040068EE RID: 26862
		Reference,
		// Token: 0x040068EF RID: 26863
		ReferenceNonJittered
	}

	// Token: 0x020010C6 RID: 4294
	public enum UpdateTiming
	{
		// Token: 0x040068F1 RID: 26865
		LateUpdate,
		// Token: 0x040068F2 RID: 26866
		OnPreCull,
		// Token: 0x040068F3 RID: 26867
		OnPreRender,
		// Token: 0x040068F4 RID: 26868
		None
	}

	// Token: 0x020010C7 RID: 4295
	[Serializable]
	public class AdvancedSettings
	{
		// Token: 0x040068F5 RID: 26869
		public bool enableNearClipOverride = true;

		// Token: 0x040068F6 RID: 26870
		public bool enableChildShadows;

		// Token: 0x040068F7 RID: 26871
		public bool enableBoundsPadding = true;

		// Token: 0x040068F8 RID: 26872
		public CameraMatrixOverride.ProjectionMode projectionMode;

		// Token: 0x040068F9 RID: 26873
		public CameraMatrixOverride.UpdateTiming updateTiming = CameraMatrixOverride.UpdateTiming.OnPreCull;

		// Token: 0x040068FA RID: 26874
		[Range(1E-45f, 2f)]
		public float depthScaleFactor = 1f;

		// Token: 0x040068FB RID: 26875
		public float farClipFactor = 1f;

		// Token: 0x040068FC RID: 26876
		public float jitterFactor = 1f;

		// Token: 0x040068FD RID: 26877
		public float boundsPadding = 1f;
	}

	// Token: 0x020010C8 RID: 4296
	[PublicizedFrom(EAccessModifier.Private)]
	public class RendererSettings
	{
		// Token: 0x040068FE RID: 26878
		public ShadowCastingMode originalShadowCastingMode;

		// Token: 0x040068FF RID: 26879
		public MaterialPropertyBlock originalProperties;

		// Token: 0x04006900 RID: 26880
		public MaterialPropertyBlock overriddenProperties;

		// Token: 0x04006901 RID: 26881
		public bool boundsDirty;

		// Token: 0x04006902 RID: 26882
		public bool shadowModeDirty;
	}
}
