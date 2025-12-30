using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200006D RID: 109
public sealed class SunShaftsEffectRenderer : PostProcessEffectRenderer<SunShaftsEffect>
{
	// Token: 0x1700003A RID: 58
	// (get) Token: 0x060001E7 RID: 487 RVA: 0x000109CC File Offset: 0x0000EBCC
	public Mesh fullscreenTriangle
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (this.m_FullscreenTriangle != null)
			{
				return this.m_FullscreenTriangle;
			}
			this.m_FullscreenTriangle = new Mesh
			{
				name = "Fullscreen Triangle"
			};
			this.m_FullscreenTriangle.SetVertices(new List<Vector3>
			{
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f, 3f, 0f),
				new Vector3(3f, -1f, 0f)
			});
			this.m_FullscreenTriangle.SetIndices(new int[]
			{
				0,
				1,
				2
			}, MeshTopology.Triangles, 0, false);
			this.m_FullscreenTriangle.UploadMeshData(false);
			return this.m_FullscreenTriangle;
		}
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x00010A90 File Offset: 0x0000EC90
	public override void Init()
	{
		base.Init();
		this.sunShaftsMaterial = new Material(base.settings.sunShaftsShader);
		this.sunShaftsMaterial.hideFlags = HideFlags.DontSave;
		this.simpleClearMaterial = new Material(base.settings.simpleClearShader);
		this.simpleClearMaterial.hideFlags = HideFlags.DontSave;
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x00010AF3 File Offset: 0x0000ECF3
	public override void Release()
	{
		base.Release();
		UnityEngine.Object.Destroy(this.sunShaftsMaterial);
		UnityEngine.Object.Destroy(this.simpleClearMaterial);
	}

	// Token: 0x060001EA RID: 490 RVA: 0x00010B14 File Offset: 0x0000ED14
	public void DrawBorder(CommandBuffer cmd, RenderTargetIdentifier dest, int width, int height, Material material, int borderWidth = 1)
	{
		cmd.SetRenderTarget(dest);
		Matrix4x4 proj = Matrix4x4.Ortho(-1f, 1f, -1f, 1f, 0f, 1f);
		Matrix4x4 identity = Matrix4x4.identity;
		cmd.SetViewProjectionMatrices(identity, proj);
		cmd.SetViewport(new Rect(0f, 0f, (float)borderWidth, (float)height));
		cmd.DrawMesh(this.fullscreenTriangle, Matrix4x4.identity, material);
		cmd.SetViewport(new Rect((float)(width - borderWidth), 0f, (float)borderWidth, (float)height));
		cmd.DrawMesh(this.fullscreenTriangle, Matrix4x4.identity, material);
		cmd.SetViewport(new Rect(0f, 0f, (float)width, (float)borderWidth));
		cmd.DrawMesh(this.fullscreenTriangle, Matrix4x4.identity, material);
		cmd.SetViewport(new Rect(0f, (float)(height - borderWidth), (float)width, (float)borderWidth));
		cmd.DrawMesh(this.fullscreenTriangle, Matrix4x4.identity, material);
	}

	// Token: 0x060001EB RID: 491 RVA: 0x00010C14 File Offset: 0x0000EE14
	public override void Render(PostProcessRenderContext context)
	{
		SunShaftsEffect.SunSettings sunSettings = base.settings.autoUpdateSun ? SkyManager.GetSunShaftSettings() : base.settings.GetSunSettings();
		int num = 4;
		if (base.settings.resolution == SunShaftsEffect.SunShaftsResolution.Normal)
		{
			num = 2;
		}
		else if (base.settings.resolution == SunShaftsEffect.SunShaftsResolution.High)
		{
			num = 1;
		}
		Vector3 vector = context.camera.WorldToViewportPoint(sunSettings.sunPosition);
		int width = context.width;
		int height = context.height;
		int width2 = width / num;
		int height2 = height / num;
		int nameID = Shader.PropertyToID("_Temp1");
		context.command.GetTemporaryRT(nameID, width2, height2, 0);
		context.command.SetGlobalVector("_BlurRadius4", new Vector4(1f, 1f, 0f, 0f) * base.settings.sunShaftBlurRadius);
		context.command.SetGlobalVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, base.settings.maxRadius));
		context.command.SetGlobalVector("_SunThreshold", sunSettings.sunThreshold);
		context.command.Blit(context.source, nameID, this.sunShaftsMaterial, 2);
		this.DrawBorder(context.command, nameID, width2, height2, this.simpleClearMaterial, 1);
		int num2 = Mathf.Clamp(base.settings.radialBlurIterations, 1, 4);
		float num3 = base.settings.sunShaftBlurRadius * 0.0013020834f;
		context.command.SetGlobalVector("_BlurRadius4", new Vector4(num3, num3, 0f, 0f));
		context.command.SetGlobalVector("_SunPosition", new Vector4(vector.x, vector.y, vector.z, base.settings.maxRadius));
		for (int i = 0; i < num2; i++)
		{
			int nameID2 = Shader.PropertyToID("_Temp3");
			context.command.GetTemporaryRT(nameID2, width2, height2, 0);
			context.command.Blit(nameID, nameID2, this.sunShaftsMaterial, 1);
			context.command.ReleaseTemporaryRT(nameID);
			num3 = base.settings.sunShaftBlurRadius * (((float)i * 2f + 1f) * 6f) / 768f;
			context.command.SetGlobalVector("_BlurRadius4", new Vector4(num3, num3, 0f, 0f));
			nameID = Shader.PropertyToID("_Temp4");
			context.command.GetTemporaryRT(nameID, width2, height2, 0);
			context.command.Blit(nameID2, nameID, this.sunShaftsMaterial, 1);
			context.command.ReleaseTemporaryRT(nameID2);
			num3 = base.settings.sunShaftBlurRadius * (((float)i * 2f + 2f) * 6f) / 768f;
			context.command.SetGlobalVector("_BlurRadius4", new Vector4(num3, num3, 0f, 0f));
		}
		if (vector.z >= 0f)
		{
			context.command.SetGlobalVector("_SunColor", sunSettings.sunColor * sunSettings.sunShaftIntensity);
		}
		else
		{
			context.command.SetGlobalVector("_SunColor", Vector4.zero);
		}
		context.command.SetGlobalTexture("_ColorBuffer", nameID);
		context.command.Blit(context.source, context.destination, this.sunShaftsMaterial, (base.settings.screenBlendMode == SunShaftsEffect.ShaftsScreenBlendMode.Screen) ? 0 : 4);
		context.command.ReleaseTemporaryRT(nameID);
	}

	// Token: 0x040002B8 RID: 696
	[PublicizedFrom(EAccessModifier.Private)]
	public Material sunShaftsMaterial;

	// Token: 0x040002B9 RID: 697
	[PublicizedFrom(EAccessModifier.Private)]
	public Material simpleClearMaterial;

	// Token: 0x040002BA RID: 698
	[PublicizedFrom(EAccessModifier.Private)]
	public Mesh m_FullscreenTriangle;
}
