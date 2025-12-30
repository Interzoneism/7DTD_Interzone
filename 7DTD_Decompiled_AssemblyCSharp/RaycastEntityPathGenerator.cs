using System;
using System.Collections;
using System.Collections.Generic;
using RaycastPathing;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200081F RID: 2079
[Preserve]
public class RaycastEntityPathGenerator
{
	// Token: 0x17000611 RID: 1553
	// (get) Token: 0x06003BB7 RID: 15287 RVA: 0x00180C31 File Offset: 0x0017EE31
	// (set) Token: 0x06003BB8 RID: 15288 RVA: 0x00180C39 File Offset: 0x0017EE39
	public World GameWorld { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000612 RID: 1554
	// (get) Token: 0x06003BB9 RID: 15289 RVA: 0x00180C42 File Offset: 0x0017EE42
	// (set) Token: 0x06003BBA RID: 15290 RVA: 0x00180C4A File Offset: 0x0017EE4A
	public EntityAlive Entity { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000613 RID: 1555
	// (get) Token: 0x06003BBB RID: 15291 RVA: 0x00180C53 File Offset: 0x0017EE53
	// (set) Token: 0x06003BBC RID: 15292 RVA: 0x00180C5B File Offset: 0x0017EE5B
	public virtual RaycastPath Path { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x17000614 RID: 1556
	// (get) Token: 0x06003BBD RID: 15293 RVA: 0x00180C64 File Offset: 0x0017EE64
	// (set) Token: 0x06003BBE RID: 15294 RVA: 0x00180C6C File Offset: 0x0017EE6C
	public bool isBuildingPath { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000615 RID: 1557
	// (get) Token: 0x06003BBF RID: 15295 RVA: 0x00180C75 File Offset: 0x0017EE75
	// (set) Token: 0x06003BC0 RID: 15296 RVA: 0x00180C7D File Offset: 0x0017EE7D
	public bool isPathReady { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06003BC1 RID: 15297 RVA: 0x00180C86 File Offset: 0x0017EE86
	public RaycastEntityPathGenerator(World _world, EntityAlive _entity)
	{
		this.GameWorld = _world;
		this.Entity = _entity;
	}

	// Token: 0x06003BC2 RID: 15298 RVA: 0x00180C9C File Offset: 0x0017EE9C
	public Vector3[] pathToArray()
	{
		Vector3[] array = new Vector3[this.Path.Nodes.Count - 1];
		for (int i = 0; i < this.Path.Nodes.Count; i++)
		{
			array[i] = this.Path.Nodes[i].Position;
		}
		return array;
	}

	// Token: 0x06003BC3 RID: 15299 RVA: 0x00180CFC File Offset: 0x0017EEFC
	public List<Vector3> pathToList()
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < this.Path.Nodes.Count; i++)
		{
			list.Add(this.Path.Nodes[i].Position);
		}
		list.Reverse();
		return list;
	}

	// Token: 0x06003BC4 RID: 15300 RVA: 0x00180D4D File Offset: 0x0017EF4D
	public void CreatePath(Vector3 start, Vector3 end, float speed, bool canBreakBlocks, float yHeightOffset = 0f)
	{
		this.cleanupPath();
		this.InitPath(start, end);
		this.beginPathProc();
	}

	// Token: 0x06003BC5 RID: 15301 RVA: 0x00180D63 File Offset: 0x0017EF63
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void InitPath(Vector3 start, Vector3 end)
	{
		this.Path = new RaycastPath(start, end);
	}

	// Token: 0x06003BC6 RID: 15302 RVA: 0x00180D72 File Offset: 0x0017EF72
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual IEnumerator BuildPathProc()
	{
		this.finalizePathProc();
		yield break;
	}

	// Token: 0x06003BC7 RID: 15303 RVA: 0x00180D81 File Offset: 0x0017EF81
	[PublicizedFrom(EAccessModifier.Private)]
	public void beginPathProc()
	{
		this.isBuildingPath = true;
		this.StartCoroutine(this.BuildPathProc());
	}

	// Token: 0x06003BC8 RID: 15304 RVA: 0x00180D96 File Offset: 0x0017EF96
	[PublicizedFrom(EAccessModifier.Private)]
	public void abortPathProc()
	{
		this.StopCoroutine(this.BuildPathProc());
		this.isBuildingPath = false;
	}

	// Token: 0x06003BC9 RID: 15305 RVA: 0x00180DAB File Offset: 0x0017EFAB
	[PublicizedFrom(EAccessModifier.Protected)]
	public void finalizePathProc()
	{
		this.isBuildingPath = false;
		this.isPathReady = true;
	}

	// Token: 0x06003BCA RID: 15306 RVA: 0x00180DBB File Offset: 0x0017EFBB
	public void Clear()
	{
		this.cleanupPath();
	}

	// Token: 0x06003BCB RID: 15307 RVA: 0x00180DC3 File Offset: 0x0017EFC3
	[PublicizedFrom(EAccessModifier.Private)]
	public void cleanupPath()
	{
		this.isPathReady = false;
		if (this.Path != null)
		{
			this.abortPathProc();
			this.Path.Destruct();
			this.Path = null;
		}
	}

	// Token: 0x06003BCC RID: 15308 RVA: 0x00180DEC File Offset: 0x0017EFEC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void StartCoroutine(IEnumerator task)
	{
		GameManager.Instance.StartCoroutine(task);
	}

	// Token: 0x06003BCD RID: 15309 RVA: 0x00180DFA File Offset: 0x0017EFFA
	[PublicizedFrom(EAccessModifier.Protected)]
	public void StopCoroutine(IEnumerator task)
	{
		GameManager.Instance.StopCoroutine(task);
	}

	// Token: 0x06003BCE RID: 15310 RVA: 0x00180E07 File Offset: 0x0017F007
	public bool IsConfinedSpace(Vector3 pos, float size, bool debugDraw = false)
	{
		return RaycastPathWorldUtils.IsConfinedSpace(this.GameWorld, pos, size, debugDraw);
	}
}
