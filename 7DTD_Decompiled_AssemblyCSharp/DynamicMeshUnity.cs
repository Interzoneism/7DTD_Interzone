using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;

// Token: 0x02000364 RID: 868
public class DynamicMeshUnity
{
	// Token: 0x06001982 RID: 6530 RVA: 0x0009C6A0 File Offset: 0x0009A8A0
	public static int RoundChunk(int value)
	{
		if (value < 0)
		{
			value -= 15;
		}
		return value / 16 * 16;
	}

	// Token: 0x06001983 RID: 6531 RVA: 0x0009C6B3 File Offset: 0x0009A8B3
	public static int RoundChunk(float value)
	{
		return DynamicMeshUnity.RoundChunk((int)value);
	}

	// Token: 0x06001984 RID: 6532 RVA: 0x0009C6BC File Offset: 0x0009A8BC
	public static string GetItemPath(long key)
	{
		return DynamicMeshFile.MeshLocation + key.ToString() + ".update";
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x0009C6B3 File Offset: 0x0009A8B3
	public static int GetChunkPositionFromWorldPosition(float pos)
	{
		return DynamicMeshUnity.RoundChunk((int)pos);
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x0009C6D4 File Offset: 0x0009A8D4
	public static int GetChunkPositionFromWorldPosition(int pos)
	{
		return DynamicMeshUnity.RoundChunk(pos);
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x0009C6DC File Offset: 0x0009A8DC
	public static long GetRegionKeyFromItemKey(long itemKey)
	{
		return DynamicMeshUnity.GetRegionKeyFromWorldPosition(DynamicMeshUnity.GetWorldPosFromKey(itemKey));
	}

	// Token: 0x06001988 RID: 6536 RVA: 0x0009C6E9 File Offset: 0x0009A8E9
	public static long GetRegionKeyFromWorldPosition(Vector3i pos)
	{
		return WorldChunkCache.MakeChunkKey(World.toChunkXZ(DynamicMeshUnity.RoundRegion(pos.x)), World.toChunkXZ(DynamicMeshUnity.RoundRegion(pos.z)));
	}

	// Token: 0x06001989 RID: 6537 RVA: 0x0009C710 File Offset: 0x0009A910
	public static Vector2 GetXZFromKey(long key)
	{
		return new Vector2((float)(WorldChunkCache.extractX(key) * 16), (float)(WorldChunkCache.extractZ(key) * 16));
	}

	// Token: 0x0600198A RID: 6538 RVA: 0x0009C72B File Offset: 0x0009A92B
	public static Vector3i GetWorldPosFromKey(long key)
	{
		return new Vector3i(WorldChunkCache.extractX(key) * 16, 0, WorldChunkCache.extractZ(key) * 16);
	}

	// Token: 0x0600198B RID: 6539 RVA: 0x0009C745 File Offset: 0x0009A945
	public static string GetDebugPositionFromKey(long key)
	{
		return string.Format("{0},{1}", WorldChunkCache.extractX(key) * 16, WorldChunkCache.extractZ(key) * 16);
	}

	// Token: 0x0600198C RID: 6540 RVA: 0x0009C745 File Offset: 0x0009A945
	public static string GetDebugPositionKey(long key)
	{
		return string.Format("{0},{1}", WorldChunkCache.extractX(key) * 16, WorldChunkCache.extractZ(key) * 16);
	}

	// Token: 0x0600198D RID: 6541 RVA: 0x0009C76D File Offset: 0x0009A96D
	public static int GetChunkSectionX(long key)
	{
		return WorldChunkCache.extractX(key);
	}

	// Token: 0x0600198E RID: 6542 RVA: 0x0009C775 File Offset: 0x0009A975
	public static int GetChunkSectionZ(long key)
	{
		return WorldChunkCache.extractZ(key);
	}

	// Token: 0x0600198F RID: 6543 RVA: 0x0009C77D File Offset: 0x0009A97D
	public static int GetWorldXFromKey(long key)
	{
		return WorldChunkCache.extractX(key) * 16;
	}

	// Token: 0x06001990 RID: 6544 RVA: 0x0009C788 File Offset: 0x0009A988
	public static int GetWorldZFromKey(long key)
	{
		return WorldChunkCache.extractZ(key) * 16;
	}

	// Token: 0x06001991 RID: 6545 RVA: 0x0009C793 File Offset: 0x0009A993
	public static long GetRegionKeyFromWorldPosition(int worldX, int worldZ)
	{
		return WorldChunkCache.MakeChunkKey(World.toChunkXZ(DynamicMeshUnity.RoundRegion(worldX)), World.toChunkXZ(DynamicMeshUnity.RoundRegion(worldZ)));
	}

	// Token: 0x06001992 RID: 6546 RVA: 0x0009C7B0 File Offset: 0x0009A9B0
	public static Vector3i GetRegionPositionFromWorldPosition(int worldX, int worldZ)
	{
		return new Vector3i(DynamicMeshUnity.RoundRegion(worldX), 0, DynamicMeshUnity.RoundRegion(worldZ));
	}

	// Token: 0x06001993 RID: 6547 RVA: 0x0009C7C4 File Offset: 0x0009A9C4
	public static Vector3i GetRegionPositionFromWorldPosition(Vector3i worldPos)
	{
		return new Vector3i(DynamicMeshUnity.RoundRegion(worldPos.x), 0, DynamicMeshUnity.RoundRegion(worldPos.z));
	}

	// Token: 0x06001994 RID: 6548 RVA: 0x0009C7E2 File Offset: 0x0009A9E2
	public static int RoundRegion(int value)
	{
		if (value < 0)
		{
			value -= 159;
		}
		return value / 160 * 160;
	}

	// Token: 0x06001995 RID: 6549 RVA: 0x0009C7FE File Offset: 0x0009A9FE
	public static int RoundRegion(float value)
	{
		return DynamicMeshUnity.RoundRegion((int)value);
	}

	// Token: 0x06001996 RID: 6550 RVA: 0x0009C807 File Offset: 0x0009AA07
	public static Vector3i GetRegionPositionFromWorldPosition(Vector3 worldPos)
	{
		return new Vector3i(DynamicMeshUnity.RoundRegion(worldPos.x), 0, DynamicMeshUnity.RoundRegion(worldPos.z));
	}

	// Token: 0x06001997 RID: 6551 RVA: 0x0009C825 File Offset: 0x0009AA25
	public static long GetItemKey(int worldX, int worldZ)
	{
		return WorldChunkCache.MakeChunkKey(World.toChunkXZ(worldX), World.toChunkXZ(worldZ));
	}

	// Token: 0x06001998 RID: 6552 RVA: 0x0009C838 File Offset: 0x0009AA38
	public static int GetItemPosition(int pos)
	{
		return World.toChunkXZ(pos) * 16;
	}

	// Token: 0x06001999 RID: 6553 RVA: 0x0009C843 File Offset: 0x0009AA43
	public static float Distance(Vector3i a, Vector3i b)
	{
		return Mathf.Abs(Mathf.Sqrt(Mathf.Pow((float)(a.x - b.x), 2f) + Mathf.Pow((float)(a.z - b.z), 2f)));
	}

	// Token: 0x0600199A RID: 6554 RVA: 0x0009C880 File Offset: 0x0009AA80
	public static float Distance(Vector3i a, Vector3 b)
	{
		return Mathf.Abs(Mathf.Sqrt(Mathf.Pow((float)a.x - b.x, 2f) + Mathf.Pow((float)a.z - b.z, 2f)));
	}

	// Token: 0x0600199B RID: 6555 RVA: 0x0009C8BD File Offset: 0x0009AABD
	public static float Distance(int x1, int y1, int x2, int y2)
	{
		return Mathf.Abs(Mathf.Sqrt(Mathf.Pow((float)(x1 - x2), 2f) + Mathf.Pow((float)(y1 - y2), 2f)));
	}

	// Token: 0x0600199C RID: 6556 RVA: 0x0009C8E6 File Offset: 0x0009AAE6
	public static void log(string msg)
	{
		if (DynamicMeshManager.ShowDebug && DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(msg);
		}
	}

	// Token: 0x0600199D RID: 6557 RVA: 0x0009824B File Offset: 0x0009644B
	[PublicizedFrom(EAccessModifier.Private)]
	public static void LogMsg(string msg)
	{
		if (DynamicMeshManager.DoLog)
		{
			DynamicMeshManager.LogMsg(msg);
		}
	}

	// Token: 0x0600199E RID: 6558 RVA: 0x0009C8FC File Offset: 0x0009AAFC
	public static Vector3i GetChunkPositionFromWorldPosition(Vector3i worldPosition)
	{
		return new Vector3i(DynamicMeshUnity.RoundChunk(worldPosition.x), worldPosition.y, DynamicMeshUnity.RoundChunk(worldPosition.z));
	}

	// Token: 0x0600199F RID: 6559 RVA: 0x0009C920 File Offset: 0x0009AB20
	public static bool IsInBuffer(float x, float z, int bufferSize, int xIndex, int zIndex)
	{
		int num = (int)(x / 160f);
		int num2 = (int)(z / 160f);
		if (x < 0f)
		{
			num--;
		}
		if (z < 0f)
		{
			num2--;
		}
		return zIndex >= num2 - bufferSize && zIndex <= num2 + bufferSize && xIndex >= num - bufferSize && xIndex <= num + bufferSize;
	}

	// Token: 0x060019A0 RID: 6560 RVA: 0x0009C978 File Offset: 0x0009AB78
	public static void WaitCoroutine(IEnumerator func)
	{
		while (func.MoveNext())
		{
			if (func.Current != null)
			{
				IEnumerator func2;
				try
				{
					func2 = (IEnumerator)func.Current;
				}
				catch (InvalidCastException)
				{
					if (func.Current.GetType() == typeof(WaitForSeconds))
					{
						Log.Warning("Skipped call to WaitForSeconds. Use WaitForSecondsRealtime instead.");
					}
					break;
				}
				DynamicMeshUnity.WaitCoroutine(func2);
			}
		}
	}

	// Token: 0x060019A1 RID: 6561 RVA: 0x0009C9E4 File Offset: 0x0009ABE4
	public static long GetMeshSize(GameObject go)
	{
		long num = 0L;
		MeshFilter component = go.GetComponent<MeshFilter>();
		if (component != null)
		{
			Mesh sharedMesh = component.sharedMesh;
			if (sharedMesh != null)
			{
				num += Profiler.GetRuntimeMemorySizeLong(sharedMesh);
			}
		}
		foreach (object obj in go.transform)
		{
			Transform transform = (Transform)obj;
			num += DynamicMeshUnity.GetMeshSize(transform.gameObject);
		}
		return num;
	}

	// Token: 0x060019A2 RID: 6562 RVA: 0x0009CA78 File Offset: 0x0009AC78
	public static Bounds GetBoundsFromVertsJustY(ArrayListMP<Vector3> verts, Bounds bounds)
	{
		float num = verts[0].y;
		float num2 = verts[0].y;
		for (int i = 0; i < verts.Count; i++)
		{
			Vector3 vector = verts[i];
			num = Math.Min(num, vector.y);
			num2 = Math.Max(num2, vector.y);
		}
		Vector3 min = new Vector3(0f, num, 0f);
		Vector3 max = new Vector3(0f, num2, 0f);
		bounds.SetMinMax(min, max);
		return bounds;
	}

	// Token: 0x060019A3 RID: 6563 RVA: 0x0009CB08 File Offset: 0x0009AD08
	public static void DeleteDynamicMeshData(ICollection<long> chunks)
	{
		string str;
		HashSetLong hashSetLong;
		DynamicMeshUnity.GetOrCreateDynamicMeshChunksList(out str, out hashSetLong);
		if (!hashSetLong.Overlaps(chunks))
		{
			return;
		}
		if (DynamicMeshManager.Instance != null && GamePrefs.GetBool(EnumGamePrefs.DynamicMeshEnabled))
		{
			DynamicMeshUnity.tempRegions.Clear();
			foreach (long num in chunks)
			{
				if (hashSetLong.Contains(num))
				{
					DynamicMeshManager instance = DynamicMeshManager.Instance;
					DynamicMeshItem dynamicMeshItem = (instance != null) ? instance.GetItemOrNull(num) : null;
					if (dynamicMeshItem == null)
					{
						Log.Error(string.Format("Failed to retrieve valid DynamicMeshItem for cached dynamic mesh chunk key: {0}.", num));
					}
					else
					{
						DynamicMeshManager.Instance.RemoveItem(dynamicMeshItem, true);
						string itemPath = DynamicMeshUnity.GetItemPath(num);
						if (SdFile.Exists(itemPath))
						{
							SdFile.Delete(itemPath);
						}
						DynamicMeshRegion region = dynamicMeshItem.GetRegion();
						DynamicMeshUnity.tempRegions.Add(region);
					}
				}
			}
			foreach (DynamicMeshRegion dynamicMeshRegion in DynamicMeshUnity.tempRegions)
			{
				dynamicMeshRegion.CleanUp();
				if (dynamicMeshRegion.LoadedItems.Count == 0 && dynamicMeshRegion.UnloadedItems.Count == 0)
				{
					string path = dynamicMeshRegion.Path;
					if (SdFile.Exists(path))
					{
						SdFile.Delete(path);
					}
				}
				else
				{
					DynamicMeshThread.AddRegionUpdateData(dynamicMeshRegion.WorldPosition.x, dynamicMeshRegion.WorldPosition.z, true);
				}
			}
			DynamicMeshUnity.tempRegions.Clear();
			return;
		}
		DynamicMeshUnity.tempRegionKeys.Clear();
		foreach (long num2 in chunks)
		{
			if (hashSetLong.Contains(num2))
			{
				string path2 = str + num2.ToString() + ".update";
				if (SdFile.Exists(path2))
				{
					SdFile.Delete(path2);
				}
				int value = WorldChunkCache.extractX(num2) * 16;
				int value2 = WorldChunkCache.extractZ(num2) * 16;
				long item = WorldChunkCache.MakeChunkKey(World.toChunkXZ(DynamicMeshUnity.RoundRegion(value)), World.toChunkXZ(DynamicMeshUnity.RoundRegion(value2)));
				DynamicMeshUnity.tempRegionKeys.Add(item);
				hashSetLong.Remove(num2);
			}
		}
		foreach (long num3 in DynamicMeshUnity.tempRegionKeys)
		{
			string path3 = str + num3.ToString() + ".group";
			if (SdFile.Exists(path3))
			{
				SdFile.Delete(path3);
			}
		}
		DynamicMeshUnity.tempRegionKeys.Clear();
	}

	// Token: 0x060019A4 RID: 6564 RVA: 0x0009CDC0 File Offset: 0x0009AFC0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void GetOrCreateDynamicMeshChunksList(out string meshLocation, out HashSetLong keys)
	{
		string text = SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? GameIO.GetSaveGameDir() : GameIO.GetSaveGameLocalDir();
		if (!DynamicMeshUnity.cachedDynamicMeshChunksList.Item1.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
		{
			string text2 = text + "/DynamicMeshes/";
			DynamicMeshUnity.cachedDynamicMeshChunksList.Item1 = text2;
			DynamicMeshUnity.cachedDynamicMeshChunksList.Item2.Clear();
			SdDirectoryInfo sdDirectoryInfo = new SdDirectoryInfo(text2);
			if (sdDirectoryInfo.Exists)
			{
				using (IEnumerator<SdFileInfo> enumerator = sdDirectoryInfo.EnumerateFiles("*.update", SearchOption.TopDirectoryOnly).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						long item;
						if (long.TryParse(Path.GetFileNameWithoutExtension(enumerator.Current.Name), out item))
						{
							DynamicMeshUnity.cachedDynamicMeshChunksList.Item2.Add(item);
						}
					}
				}
			}
		}
		meshLocation = DynamicMeshUnity.cachedDynamicMeshChunksList.Item1;
		keys = DynamicMeshUnity.cachedDynamicMeshChunksList.Item2;
	}

	// Token: 0x060019A5 RID: 6565 RVA: 0x0009CEAC File Offset: 0x0009B0AC
	public static void AddDisabledImposterChunk(long key)
	{
		string text;
		HashSetLong hashSetLong;
		DynamicMeshUnity.GetOrCreateDynamicMeshChunksList(out text, out hashSetLong);
		hashSetLong.Add(key);
	}

	// Token: 0x060019A6 RID: 6566 RVA: 0x0009CECC File Offset: 0x0009B0CC
	public static void RemoveDisabledImposterChunk(long key)
	{
		string text;
		HashSetLong hashSetLong;
		DynamicMeshUnity.GetOrCreateDynamicMeshChunksList(out text, out hashSetLong);
		hashSetLong.Remove(key);
	}

	// Token: 0x060019A7 RID: 6567 RVA: 0x0009CEEA File Offset: 0x0009B0EA
	public static void ClearCachedDynamicMeshChunksList()
	{
		DynamicMeshUnity.cachedDynamicMeshChunksList.Item1 = string.Empty;
		DynamicMeshUnity.cachedDynamicMeshChunksList.Item2.Clear();
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x0009CF0A File Offset: 0x0009B10A
	[Conditional("UNITY_STANDALONE")]
	public static void EnsureDMDirectoryExists()
	{
		if (!SdDirectory.Exists(DynamicMeshFile.MeshLocation))
		{
			SdDirectory.CreateDirectory(DynamicMeshFile.MeshLocation);
		}
	}

	// Token: 0x0400107D RID: 4221
	public const int RegionSize = 160;

	// Token: 0x0400107E RID: 4222
	[PublicizedFrom(EAccessModifier.Private)]
	public static HashSet<DynamicMeshRegion> tempRegions = new HashSet<DynamicMeshRegion>();

	// Token: 0x0400107F RID: 4223
	[PublicizedFrom(EAccessModifier.Private)]
	public static HashSet<long> tempRegionKeys = new HashSet<long>();

	// Token: 0x04001080 RID: 4224
	[TupleElementNames(new string[]
	{
		"path",
		"keys"
	})]
	[PublicizedFrom(EAccessModifier.Private)]
	public static ValueTuple<string, HashSetLong> cachedDynamicMeshChunksList = new ValueTuple<string, HashSetLong>(string.Empty, new HashSetLong());
}
