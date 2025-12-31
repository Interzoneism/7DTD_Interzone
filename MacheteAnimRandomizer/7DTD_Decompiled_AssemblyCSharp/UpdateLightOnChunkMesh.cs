using System;
using UnityEngine;

// Token: 0x02001092 RID: 4242
public class UpdateLightOnChunkMesh : MonoBehaviour
{
	// Token: 0x060085E7 RID: 34279 RVA: 0x0036638D File Offset: 0x0036458D
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		if (GameManager.IsDedicatedServer)
		{
			base.enabled = false;
			return;
		}
		this.Reset();
	}

	// Token: 0x060085E8 RID: 34280 RVA: 0x003663A4 File Offset: 0x003645A4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		this.checkLight();
	}

	// Token: 0x060085E9 RID: 34281 RVA: 0x003663B4 File Offset: 0x003645B4
	public void SetChunkMesh(VoxelMesh _chunkMesh)
	{
		this.chunkMesh = _chunkMesh;
	}

	// Token: 0x060085EA RID: 34282 RVA: 0x003663C0 File Offset: 0x003645C0
	public void Reset()
	{
		this.chunkMesh = null;
		this.lastTimeLightBrightnessChecked = 0f;
		this.lastSunLight = (this.lastBlockLight = byte.MaxValue);
	}

	// Token: 0x060085EB RID: 34283 RVA: 0x003663F3 File Offset: 0x003645F3
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (Time.time - this.lastTimeLightBrightnessChecked < this.cTimeBrightnessUpdate)
		{
			return;
		}
		this.lastTimeLightBrightnessChecked = Time.time;
		this.checkLight();
	}

	// Token: 0x060085EC RID: 34284 RVA: 0x00366424 File Offset: 0x00364624
	[PublicizedFrom(EAccessModifier.Private)]
	public void checkLight()
	{
		if (this.chunkMesh == null)
		{
			return;
		}
		GameManager instance = GameManager.Instance;
		if (!instance || !instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		World world = instance.World;
		if (world == null)
		{
			return;
		}
		if (this.meshFilter == null)
		{
			this.meshFilter = base.transform.GetComponent<MeshFilter>();
		}
		if (this.meshRenderer == null)
		{
			this.meshRenderer = base.transform.GetComponent<MeshRenderer>();
		}
		if (this.meshFilter == null || this.meshRenderer == null)
		{
			return;
		}
		byte v;
		byte v2;
		world.GetSunAndBlockColors(World.worldToBlockPos(base.transform.position + Origin.position), out v, out v2);
		byte v3;
		byte v4;
		world.GetSunAndBlockColors(World.worldToBlockPos(base.transform.position + Vector3.up + Origin.position), out v3, out v4);
		byte b = Utils.FastMax(v, v3);
		byte b2 = Utils.FastMax(v2, v4);
		Color value = world.IsDaytime() ? world.m_WorldEnvironment.GetSunLightColor() : world.m_WorldEnvironment.GetMoonLightColor();
		value.a = 1f;
		if (b != this.lastSunLight || b2 != this.lastBlockLight || !value.Equals(this.lastSunMoonLight))
		{
			this.lastSunLight = b;
			this.lastBlockLight = b2;
			this.lastSunMoonLight = value;
			this.meshFilter.mesh.colors = this.chunkMesh.UpdateColors(b, b2);
			this.meshRenderer.material.SetColor("_SunMoonlight", value);
		}
	}

	// Token: 0x040067F7 RID: 26615
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastTimeLightBrightnessChecked;

	// Token: 0x040067F8 RID: 26616
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cTimeBrightnessUpdate = 0.5f;

	// Token: 0x040067F9 RID: 26617
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public byte lastSunLight;

	// Token: 0x040067FA RID: 26618
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public byte lastBlockLight;

	// Token: 0x040067FB RID: 26619
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Color lastSunMoonLight;

	// Token: 0x040067FC RID: 26620
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshFilter meshFilter;

	// Token: 0x040067FD RID: 26621
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshRenderer meshRenderer;

	// Token: 0x040067FE RID: 26622
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public VoxelMesh chunkMesh;
}
