using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000C4 RID: 196
public class DismemberedPart
{
	// Token: 0x060004CD RID: 1229 RVA: 0x000229D3 File Offset: 0x00020BD3
	public void SetDetachedTransform(Transform _detach, Transform _group)
	{
		this.detachT = _detach;
		this.detachRenderer = _group.GetComponentInChildren<MeshRenderer>();
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x000229E8 File Offset: 0x00020BE8
	public void FadeDetached(float value)
	{
		if (this.detachRenderer)
		{
			if (!this.fadeOutInit)
			{
				this.detachMats.Clear();
				this.detachMats.AddRange(this.detachRenderer.materials);
				this.fadeOutInit = true;
			}
			for (int i = 0; i < this.detachMats.Count; i++)
			{
				Material material = this.detachMats[i];
				if (material.HasProperty("_Fade"))
				{
					material.SetFloat("_Fade", value);
				}
			}
			this.detachRenderer.SetMaterials(this.detachMats);
		}
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x00022A80 File Offset: 0x00020C80
	public DismemberedPart(DismemberedPartData _data, uint _bodyDamageFlag, EnumDamageTypes _damageType)
	{
		this.Data = _data;
		this.bodyDamageFlag = _bodyDamageFlag;
		this.damageType = _damageType;
	}

	// Token: 0x17000059 RID: 89
	// (get) Token: 0x060004D0 RID: 1232 RVA: 0x00022AD4 File Offset: 0x00020CD4
	public string prefabPath
	{
		get
		{
			return this.Data.prefabPath;
		}
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x00022AE4 File Offset: 0x00020CE4
	public void Update()
	{
		this.elapsedTime += Time.deltaTime;
		if (this.pivotT && this.overrideHeadSize != 1f)
		{
			if (!this.startValuesSet)
			{
				this.startingHeadSize = this.overrideHeadSize;
				this.startingScale = this.pivotT.localScale;
				this.startValuesSet = true;
			}
			float t = this.elapsedTime / this.overrideHeadDismemberScaleTime;
			this.overrideHeadSize = Mathf.Lerp(this.startingHeadSize, 1f, t);
			this.pivotT.localScale = Vector3.Lerp(this.startingScale, Vector3.one, t);
		}
		if (!this.ReadyForCleanup)
		{
			if (this.elapsedTime > this.lifeTime - 0.5f)
			{
				this.fadeTime += Time.deltaTime;
				this.FadeDetached(Mathf.Lerp(1f, 0f, this.fadeTime / 0.5f));
			}
			if (this.elapsedTime >= this.lifeTime)
			{
				this.ReadyForCleanup = true;
			}
		}
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x00022BEE File Offset: 0x00020DEE
	public void Hide()
	{
		if (this.prefabT)
		{
			this.prefabT.gameObject.SetActive(false);
		}
	}

	// Token: 0x060004D3 RID: 1235 RVA: 0x00022C0E File Offset: 0x00020E0E
	public void CleanupDetached()
	{
		if (this.detachT)
		{
			UnityEngine.Object.Destroy(this.detachT.gameObject);
			this.detachT = null;
		}
	}

	// Token: 0x04000565 RID: 1381
	public DismemberedPartData Data;

	// Token: 0x04000566 RID: 1382
	public uint bodyDamageFlag;

	// Token: 0x04000567 RID: 1383
	public EnumDamageTypes damageType;

	// Token: 0x04000568 RID: 1384
	public Transform prefabT;

	// Token: 0x04000569 RID: 1385
	public Transform detachT;

	// Token: 0x0400056A RID: 1386
	public bool ReadyForCleanup;

	// Token: 0x0400056B RID: 1387
	public float lifeTime = 10f;

	// Token: 0x0400056C RID: 1388
	[PublicizedFrom(EAccessModifier.Private)]
	public float elapsedTime;

	// Token: 0x0400056D RID: 1389
	public Transform targetT;

	// Token: 0x0400056E RID: 1390
	public Transform pivotT;

	// Token: 0x0400056F RID: 1391
	public float overrideHeadSize = 1f;

	// Token: 0x04000570 RID: 1392
	public float overrideHeadDismemberScaleTime = 1f;

	// Token: 0x04000571 RID: 1393
	[PublicizedFrom(EAccessModifier.Private)]
	public bool startValuesSet;

	// Token: 0x04000572 RID: 1394
	[PublicizedFrom(EAccessModifier.Private)]
	public float startingHeadSize;

	// Token: 0x04000573 RID: 1395
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 startingScale;

	// Token: 0x04000574 RID: 1396
	[PublicizedFrom(EAccessModifier.Private)]
	public MeshRenderer detachRenderer;

	// Token: 0x04000575 RID: 1397
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Material> detachMats = new List<Material>();

	// Token: 0x04000576 RID: 1398
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cFadeTime = 0.5f;

	// Token: 0x04000577 RID: 1399
	[PublicizedFrom(EAccessModifier.Private)]
	public float fadeTime;

	// Token: 0x04000578 RID: 1400
	[PublicizedFrom(EAccessModifier.Private)]
	public bool fadeOutInit;
}
