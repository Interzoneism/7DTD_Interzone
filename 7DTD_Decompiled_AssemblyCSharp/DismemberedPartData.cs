using System;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class DismemberedPartData
{
	// Token: 0x17000056 RID: 86
	// (get) Token: 0x060004C4 RID: 1220 RVA: 0x0002295A File Offset: 0x00020B5A
	// (set) Token: 0x060004C5 RID: 1221 RVA: 0x00022962 File Offset: 0x00020B62
	public Vector3 rot { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000057 RID: 87
	// (get) Token: 0x060004C6 RID: 1222 RVA: 0x0002296B File Offset: 0x00020B6B
	// (set) Token: 0x060004C7 RID: 1223 RVA: 0x00022973 File Offset: 0x00020B73
	public bool hasRotOffset { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060004C8 RID: 1224 RVA: 0x0002297C File Offset: 0x00020B7C
	public void SetRot(Vector3 _rot)
	{
		this.hasRotOffset = true;
		this.rot = _rot;
	}

	// Token: 0x17000058 RID: 88
	// (get) Token: 0x060004C9 RID: 1225 RVA: 0x0002298C File Offset: 0x00020B8C
	// (set) Token: 0x060004CA RID: 1226 RVA: 0x00022994 File Offset: 0x00020B94
	public bool Invalid { get; set; }

	// Token: 0x060004CB RID: 1227 RVA: 0x0002299D File Offset: 0x00020B9D
	public string Log()
	{
		return string.Format(" property: {0} prefabPath: {1} target: {2} damageTag {3}", new object[]
		{
			this.propertyKey,
			this.prefabPath,
			this.targetBone,
			this.damageTypeKey
		});
	}

	// Token: 0x04000550 RID: 1360
	public string propertyKey;

	// Token: 0x04000551 RID: 1361
	public string prefabPath;

	// Token: 0x04000552 RID: 1362
	public string targetBone;

	// Token: 0x04000553 RID: 1363
	public string damageTypeKey;

	// Token: 0x04000554 RID: 1364
	public bool isDetachable;

	// Token: 0x04000555 RID: 1365
	public Vector3 scale;

	// Token: 0x04000556 RID: 1366
	public Vector3 offset;

	// Token: 0x04000557 RID: 1367
	public bool attachToParent;

	// Token: 0x04000558 RID: 1368
	public string[] particlePaths;

	// Token: 0x04000559 RID: 1369
	public bool useMask;

	// Token: 0x0400055A RID: 1370
	public bool isLinked;

	// Token: 0x0400055B RID: 1371
	public bool scaleOutLimb;

	// Token: 0x0400055C RID: 1372
	public string solTarget;

	// Token: 0x0400055D RID: 1373
	public Vector3 solScale;

	// Token: 0x0400055E RID: 1374
	public bool hasSolScale;

	// Token: 0x0400055F RID: 1375
	public string childTargetObj;

	// Token: 0x04000560 RID: 1376
	public string insertBoneObj;

	// Token: 0x04000561 RID: 1377
	public string dismemberMatPath;
}
