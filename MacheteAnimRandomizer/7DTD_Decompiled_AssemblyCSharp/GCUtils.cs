using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x0200119D RID: 4509
public static class GCUtils
{
	// Token: 0x06008CFA RID: 36090 RVA: 0x00368263 File Offset: 0x00366463
	[PublicizedFrom(EAccessModifier.Private)]
	public static void FullCollect()
	{
		GC.Collect();
	}

	// Token: 0x06008CFB RID: 36091 RVA: 0x0038A957 File Offset: 0x00388B57
	[PublicizedFrom(EAccessModifier.Private)]
	public static void PreUnload()
	{
		GCUtils.FullCollect();
	}

	// Token: 0x06008CFC RID: 36092 RVA: 0x0038A95E File Offset: 0x00388B5E
	[PublicizedFrom(EAccessModifier.Private)]
	public static void PostUnload()
	{
		GC.WaitForPendingFinalizers();
		GCUtils.FullCollect();
	}

	// Token: 0x06008CFD RID: 36093 RVA: 0x0038A96A File Offset: 0x00388B6A
	public static void UnloadAndCollectStart()
	{
		ThreadManager.StartCoroutine(GCUtils.UnloadAndCollectCo());
	}

	// Token: 0x06008CFE RID: 36094 RVA: 0x0038A977 File Offset: 0x00388B77
	public static IEnumerator UnloadAndCollectCo()
	{
		Interlocked.Increment(ref GCUtils.m_working);
		try
		{
			Task preUnload = Task.Run(new Action(GCUtils.PreUnload));
			yield return Resources.UnloadUnusedAssets();
			while (!preUnload.IsCompleted)
			{
				yield return null;
			}
			Task postUnload = Task.Run(new Action(GCUtils.PostUnload));
			while (!postUnload.IsCompleted)
			{
				yield return null;
			}
			preUnload = null;
			postUnload = null;
		}
		finally
		{
			Interlocked.Decrement(ref GCUtils.m_working);
		}
		yield break;
		yield break;
	}

	// Token: 0x06008CFF RID: 36095 RVA: 0x0038A97F File Offset: 0x00388B7F
	public static IEnumerator WaitForIdle()
	{
		while (GCUtils.m_working > 0)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x04006D94 RID: 28052
	[PublicizedFrom(EAccessModifier.Private)]
	public static int m_working;
}
