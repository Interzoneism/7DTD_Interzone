using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000391 RID: 913
[Preserve]
public class BlockEntityData
{
	// Token: 0x06001B41 RID: 6977 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public BlockEntityData()
	{
	}

	// Token: 0x06001B42 RID: 6978 RVA: 0x000AAE20 File Offset: 0x000A9020
	public BlockEntityData(BlockValue _blockValue, Vector3i _pos)
	{
		this.pos = _pos;
		this.blockValue = _blockValue;
	}

	// Token: 0x06001B43 RID: 6979 RVA: 0x000AAE38 File Offset: 0x000A9038
	[PublicizedFrom(EAccessModifier.Private)]
	public void GetRenderers()
	{
		if (this.matPropBlock == null)
		{
			this.matPropBlock = new MaterialPropertyBlock();
		}
		if (this.renderers != null)
		{
			this.renderers.Clear();
		}
		else
		{
			this.renderers = new List<Renderer>();
		}
		this.transform.GetComponentsInChildren<Renderer>(true, this.renderers);
	}

	// Token: 0x06001B44 RID: 6980 RVA: 0x000AAE8A File Offset: 0x000A908A
	public void Cleanup()
	{
		if (this.renderers != null)
		{
			this.renderers.Clear();
		}
	}

	// Token: 0x06001B45 RID: 6981 RVA: 0x000AAEA0 File Offset: 0x000A90A0
	public void SetMaterialColor(string name, Color value)
	{
		this.GetRenderers();
		if (this.renderers == null)
		{
			return;
		}
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		for (int i = 0; i < this.renderers.Count; i++)
		{
			if (this.renderers[i] != null)
			{
				this.renderers[i].GetPropertyBlock(this.matPropBlock);
				this.matPropBlock.SetColor(name, value);
				this.renderers[i].SetPropertyBlock(this.matPropBlock);
			}
		}
	}

	// Token: 0x06001B46 RID: 6982 RVA: 0x000AAF2C File Offset: 0x000A912C
	public void SetMaterialValue(string name, float value)
	{
		this.GetRenderers();
		if (this.renderers == null)
		{
			return;
		}
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		for (int i = 0; i < this.renderers.Count; i++)
		{
			if (this.renderers[i] != null)
			{
				this.renderers[i].GetPropertyBlock(this.matPropBlock);
				this.matPropBlock.SetFloat(name, value);
				this.renderers[i].SetPropertyBlock(this.matPropBlock);
			}
		}
	}

	// Token: 0x06001B47 RID: 6983 RVA: 0x000AAFB8 File Offset: 0x000A91B8
	public void SetMaterialColor(Color color)
	{
		this.GetRenderers();
		for (int i = 0; i < this.renderers.Count; i++)
		{
			if (this.renderers[i] != null)
			{
				this.renderers[i].GetPropertyBlock(this.matPropBlock);
				this.matPropBlock.SetColor("_Color", color);
				this.renderers[i].SetPropertyBlock(this.matPropBlock);
			}
		}
	}

	// Token: 0x06001B48 RID: 6984 RVA: 0x00002914 File Offset: 0x00000B14
	public void UpdateTemperature()
	{
	}

	// Token: 0x06001B49 RID: 6985 RVA: 0x000AB034 File Offset: 0x000A9234
	public override string ToString()
	{
		string str = "EntityBlockCreationData ";
		BlockValue blockValue = this.blockValue;
		return str + blockValue.ToString();
	}

	// Token: 0x04001226 RID: 4646
	[PublicizedFrom(EAccessModifier.Private)]
	public MaterialPropertyBlock matPropBlock;

	// Token: 0x04001227 RID: 4647
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Renderer> renderers;

	// Token: 0x04001228 RID: 4648
	public BlockValue blockValue;

	// Token: 0x04001229 RID: 4649
	public Vector3i pos;

	// Token: 0x0400122A RID: 4650
	public Transform transform;

	// Token: 0x0400122B RID: 4651
	public bool bHasTransform;

	// Token: 0x0400122C RID: 4652
	public bool bRenderingOn;

	// Token: 0x0400122D RID: 4653
	public bool bNeedsTemperature;
}
