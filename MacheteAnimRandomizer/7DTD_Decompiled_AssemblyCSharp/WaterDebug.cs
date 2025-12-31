using System;
using System.Diagnostics;

// Token: 0x02000B70 RID: 2928
public static class WaterDebug
{
	// Token: 0x17000946 RID: 2374
	// (get) Token: 0x06005AE0 RID: 23264 RVA: 0x00247133 File Offset: 0x00245333
	// (set) Token: 0x06005AE1 RID: 23265 RVA: 0x0024713A File Offset: 0x0024533A
	public static WaterDebugManager Manager { [PublicizedFrom(EAccessModifier.Private)] get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000947 RID: 2375
	// (get) Token: 0x06005AE2 RID: 23266 RVA: 0x00247142 File Offset: 0x00245342
	public static bool IsAvailable
	{
		get
		{
			return WaterDebug.Manager != null;
		}
	}

	// Token: 0x17000948 RID: 2376
	// (get) Token: 0x06005AE3 RID: 23267 RVA: 0x0024714C File Offset: 0x0024534C
	// (set) Token: 0x06005AE4 RID: 23268 RVA: 0x0024715E File Offset: 0x0024535E
	public static bool RenderingEnabled
	{
		get
		{
			WaterDebugManager manager = WaterDebug.Manager;
			return manager != null && manager.RenderingEnabled;
		}
		set
		{
			if (WaterDebug.Manager != null)
			{
				WaterDebug.Manager.RenderingEnabled = value;
			}
		}
	}

	// Token: 0x06005AE5 RID: 23269 RVA: 0x00247172 File Offset: 0x00245372
	[Conditional("UNITY_EDITOR")]
	public static void Init()
	{
		if (!WaterSimulationNative.Instance.ShouldEnable)
		{
			return;
		}
		WaterDebugPools.CreatePools();
		WaterDebug.Manager = new WaterDebugManager();
		WaterDebug.RenderingEnabled = false;
	}

	// Token: 0x06005AE6 RID: 23270 RVA: 0x00247196 File Offset: 0x00245396
	[Conditional("UNITY_EDITOR")]
	public static void InitializeForChunk(Chunk _chunk)
	{
		WaterDebugManager manager = WaterDebug.Manager;
		if (manager == null)
		{
			return;
		}
		manager.InitializeDebugRender(_chunk);
	}

	// Token: 0x06005AE7 RID: 23271 RVA: 0x002471A8 File Offset: 0x002453A8
	[Conditional("UNITY_EDITOR")]
	public static void Draw()
	{
		WaterDebugManager manager = WaterDebug.Manager;
		if (manager == null)
		{
			return;
		}
		manager.DebugDraw();
	}

	// Token: 0x06005AE8 RID: 23272 RVA: 0x002471B9 File Offset: 0x002453B9
	[Conditional("UNITY_EDITOR")]
	public static void Cleanup()
	{
		WaterDebugManager manager = WaterDebug.Manager;
		if (manager != null)
		{
			manager.Cleanup();
		}
		WaterDebugPools.Cleanup();
		WaterDebug.Manager = null;
	}
}
