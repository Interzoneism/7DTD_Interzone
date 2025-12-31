using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000B73 RID: 2931
public class WaterDebugRendererLayer : IMemoryPoolableObject
{
	// Token: 0x1700094B RID: 2379
	// (get) Token: 0x06005AF0 RID: 23280 RVA: 0x0024739E File Offset: 0x0024559E
	// (set) Token: 0x06005AF1 RID: 23281 RVA: 0x002473A6 File Offset: 0x002455A6
	public bool IsInitialized { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06005AF2 RID: 23282 RVA: 0x002473B0 File Offset: 0x002455B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitializeData()
	{
		this.transforms = new Matrix4x4[4096];
		this.colors = new float4[4096];
		this.normalizedMass = new float[4096];
		this.RegenerateTransforms();
		Origin.OriginChanged = (Action<Vector3>)Delegate.Combine(Origin.OriginChanged, new Action<Vector3>(this.OnOriginChanged));
		this.IsInitialized = true;
	}

	// Token: 0x06005AF3 RID: 23283 RVA: 0x0024741C File Offset: 0x0024561C
	[PublicizedFrom(EAccessModifier.Private)]
	public void RegenerateTransforms()
	{
		Vector3 vector = Vector3.one * 0.9f;
		Vector3 b = (Vector3.one - vector) * 0.5f;
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				for (int k = 0; k < 16; k++)
				{
					int num = this.CoordToIndex(i, j, k);
					this.transforms[num] = Matrix4x4.TRS(this.layerOrigin + new Vector3((float)i, (float)j, (float)k) - Origin.position + b + Vector3.one * 0.5f, Quaternion.identity, vector);
				}
			}
		}
		Vector3 b2 = this.layerOrigin - Origin.position;
		Vector3 a = this.layerOrigin - Origin.position + new Vector3(16f, 16f, 16f);
		this.bounds = new Bounds((a + b2) / 2f, a - b2);
		this.transformsHaveChanged = true;
	}

	// Token: 0x06005AF4 RID: 23284 RVA: 0x00247550 File Offset: 0x00245750
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnOriginChanged(Vector3 _origin)
	{
		if (this.IsInitialized)
		{
			this.RegenerateTransforms();
		}
	}

	// Token: 0x06005AF5 RID: 23285 RVA: 0x00247560 File Offset: 0x00245760
	public void SetLayerOrigin(Vector3 _origin)
	{
		this.layerOrigin = _origin;
		if (this.IsInitialized)
		{
			this.RegenerateTransforms();
		}
	}

	// Token: 0x06005AF6 RID: 23286 RVA: 0x00247577 File Offset: 0x00245777
	[PublicizedFrom(EAccessModifier.Private)]
	public int CoordToIndex(int _x, int _y, int _z)
	{
		return _x + 16 * _y + 256 * _z;
	}

	// Token: 0x06005AF7 RID: 23287 RVA: 0x00247588 File Offset: 0x00245788
	public void SetWater(int _x, int _y, int _z, float mass)
	{
		float num = mass / 19500f;
		if (!this.IsInitialized)
		{
			if (num < 0.01f)
			{
				return;
			}
			this.InitializeData();
		}
		int num2 = this.CoordToIndex(_x, _y, _z);
		if (this.normalizedMass[num2] < 0.01f && num > 0.01f)
		{
			this.totalWater++;
		}
		else if (this.normalizedMass[num2] > 0.01f && num < 0.01f)
		{
			this.totalWater--;
		}
		this.normalizedMass[num2] = num;
		float s = math.max((mass - 19500f) / 65535f, 0f);
		this.colors[num2] = math.lerp(WaterDebugRendererLayer.waterColor, WaterDebugRendererLayer.overfullColor, s);
		this.massesHaveChanged = true;
	}

	// Token: 0x06005AF8 RID: 23288 RVA: 0x00247650 File Offset: 0x00245850
	public void Draw()
	{
		if (this.totalWater == 0)
		{
			return;
		}
		if (this.materialProperties == null)
		{
			this.materialProperties = new MaterialPropertyBlock();
			this.materialProperties.SetFloat("_ScaleCutoff", 0.01f);
		}
		if (this.transformsBuffer == null)
		{
			this.transformsBuffer = new ComputeBuffer(4096, 64);
			this.materialProperties.SetBuffer("_Transforms", this.transformsBuffer);
		}
		if (this.colorBuffer == null)
		{
			this.colorBuffer = new ComputeBuffer(4096, 16);
			this.materialProperties.SetBuffer("_Colors", this.colorBuffer);
		}
		if (this.transformsHaveChanged)
		{
			this.transformsBuffer.SetData(this.transforms);
			this.transformsHaveChanged = false;
		}
		if (this.massBuffer == null)
		{
			this.massBuffer = new ComputeBuffer(4096, 4);
			this.materialProperties.SetBuffer("_Scales", this.massBuffer);
		}
		if (this.massesHaveChanged)
		{
			this.massBuffer.SetData(this.normalizedMass);
			this.colorBuffer.SetData(this.colors);
			this.massesHaveChanged = false;
		}
		Graphics.DrawMeshInstancedProcedural(WaterDebugAssets.CubeMesh, 0, WaterDebugAssets.DebugMaterial, this.bounds, 4096, this.materialProperties, ShadowCastingMode.On, true, 0, null, LightProbeUsage.BlendProbes, null);
	}

	// Token: 0x06005AF9 RID: 23289 RVA: 0x00247798 File Offset: 0x00245998
	public void Reset()
	{
		if (this.IsInitialized)
		{
			Origin.OriginChanged = (Action<Vector3>)Delegate.Remove(Origin.OriginChanged, new Action<Vector3>(this.OnOriginChanged));
			this.transforms = null;
			this.normalizedMass = null;
			this.IsInitialized = false;
		}
		this.totalWater = 0;
		this.transformsHaveChanged = false;
		this.massesHaveChanged = false;
	}

	// Token: 0x06005AFA RID: 23290 RVA: 0x002477F8 File Offset: 0x002459F8
	public void Cleanup()
	{
		if (this.transformsBuffer != null)
		{
			this.transformsBuffer.Dispose();
			this.transformsBuffer = null;
		}
		if (this.massBuffer != null)
		{
			this.massBuffer.Dispose();
			this.massBuffer = null;
		}
		if (this.colorBuffer != null)
		{
			this.colorBuffer.Dispose();
			this.colorBuffer = null;
		}
	}

	// Token: 0x04004583 RID: 17795
	public const int dimX = 16;

	// Token: 0x04004584 RID: 17796
	public const int dimY = 16;

	// Token: 0x04004585 RID: 17797
	public const int dimZ = 16;

	// Token: 0x04004586 RID: 17798
	public const int elementsPerLayer = 4096;

	// Token: 0x04004587 RID: 17799
	[PublicizedFrom(EAccessModifier.Private)]
	public const float SCALE_CUTOFF = 0.01f;

	// Token: 0x04004588 RID: 17800
	[PublicizedFrom(EAccessModifier.Private)]
	public const float RENDER_SCALE = 0.9f;

	// Token: 0x04004589 RID: 17801
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float4 waterColor = new float4(0f, 0f, 1f, 1f);

	// Token: 0x0400458A RID: 17802
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly float4 overfullColor = new float4(1f, 0f, 1f, 1f);

	// Token: 0x0400458B RID: 17803
	[PublicizedFrom(EAccessModifier.Private)]
	public MaterialPropertyBlock materialProperties;

	// Token: 0x0400458C RID: 17804
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 layerOrigin;

	// Token: 0x0400458D RID: 17805
	[PublicizedFrom(EAccessModifier.Private)]
	public Bounds bounds;

	// Token: 0x0400458E RID: 17806
	[PublicizedFrom(EAccessModifier.Private)]
	public Matrix4x4[] transforms;

	// Token: 0x0400458F RID: 17807
	[PublicizedFrom(EAccessModifier.Private)]
	public float4[] colors;

	// Token: 0x04004590 RID: 17808
	[PublicizedFrom(EAccessModifier.Private)]
	public bool transformsHaveChanged;

	// Token: 0x04004591 RID: 17809
	[PublicizedFrom(EAccessModifier.Private)]
	public float[] normalizedMass;

	// Token: 0x04004592 RID: 17810
	[PublicizedFrom(EAccessModifier.Private)]
	public bool massesHaveChanged;

	// Token: 0x04004593 RID: 17811
	[PublicizedFrom(EAccessModifier.Private)]
	public ComputeBuffer transformsBuffer;

	// Token: 0x04004594 RID: 17812
	[PublicizedFrom(EAccessModifier.Private)]
	public ComputeBuffer massBuffer;

	// Token: 0x04004595 RID: 17813
	[PublicizedFrom(EAccessModifier.Private)]
	public ComputeBuffer colorBuffer;

	// Token: 0x04004597 RID: 17815
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalWater;
}
