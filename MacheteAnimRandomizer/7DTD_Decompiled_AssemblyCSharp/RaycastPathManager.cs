using System;
using System.Collections.Generic;
using RaycastPathing;
using UnityEngine.Scripting;

// Token: 0x0200081E RID: 2078
[Preserve]
public class RaycastPathManager
{
	// Token: 0x17000610 RID: 1552
	// (get) Token: 0x06003BAE RID: 15278 RVA: 0x00180B7F File Offset: 0x0017ED7F
	public static RaycastPathManager Instance
	{
		get
		{
			return RaycastPathManager.instance;
		}
	}

	// Token: 0x06003BAF RID: 15279 RVA: 0x00180B86 File Offset: 0x0017ED86
	public static void Init()
	{
		RaycastPathManager.instance = new RaycastPathManager();
		RaycastPathManager.instance._Init();
	}

	// Token: 0x06003BB0 RID: 15280 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void _Init()
	{
	}

	// Token: 0x06003BB1 RID: 15281 RVA: 0x00180B9C File Offset: 0x0017ED9C
	public void Add(RaycastPath path)
	{
		if (!this.paths.Contains(path))
		{
			this.paths.Add(path);
		}
	}

	// Token: 0x06003BB2 RID: 15282 RVA: 0x00180BB8 File Offset: 0x0017EDB8
	public void Remove(RaycastPath path)
	{
		if (this.paths.Contains(path))
		{
			this.paths.Remove(path);
		}
	}

	// Token: 0x06003BB3 RID: 15283 RVA: 0x00180BD5 File Offset: 0x0017EDD5
	public void Update()
	{
		if (RaycastPathManager.DebugModeEnabled)
		{
			this._DebugDraw();
		}
	}

	// Token: 0x06003BB4 RID: 15284 RVA: 0x00180BE4 File Offset: 0x0017EDE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void _DebugDraw()
	{
		for (int i = 0; i < this.paths.Count; i++)
		{
			this.paths[i].DebugDraw();
		}
	}

	// Token: 0x06003BB5 RID: 15285 RVA: 0x00180C18 File Offset: 0x0017EE18
	public static implicit operator bool(RaycastPathManager exists)
	{
		return exists != null;
	}

	// Token: 0x0400307D RID: 12413
	public static bool DebugModeEnabled;

	// Token: 0x0400307E RID: 12414
	[PublicizedFrom(EAccessModifier.Protected)]
	public List<RaycastPath> paths = new List<RaycastPath>();

	// Token: 0x0400307F RID: 12415
	[PublicizedFrom(EAccessModifier.Private)]
	public static RaycastPathManager instance;
}
