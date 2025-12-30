using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200016B RID: 363
public class WaterSplashCubes
{
	// Token: 0x06000AD0 RID: 2768 RVA: 0x00046784 File Offset: 0x00044984
	public WaterSplashCubes()
	{
		if (WaterSplashCubes.instance == null)
		{
			WaterSplashCubes.instance = this;
		}
		WaterSplashCubes.addList = new List<WaterSplashCubes.ParticlePlacement>();
		WaterSplashCubes.removeList = new List<Vector3i>();
		WaterSplashCubes.root = GameObject.Find("WaterSplashes");
		WaterSplashCubes.particleLimiter = GamePrefs.GetFloat(EnumGamePrefs.OptionsGfxWaterPtlLimiter);
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x000467D6 File Offset: 0x000449D6
	public static long MakeKey(int x, int z)
	{
		return ((long)z & 16777215L) << 24 | ((long)x & 16777215L);
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x000467EE File Offset: 0x000449EE
	public static object GetSyncRoot()
	{
		return ((ICollection)WaterSplashCubes.splashes.dict).SyncRoot;
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x00046800 File Offset: 0x00044A00
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject AddParticleEffect(Vector3i pos, BlockFace face, WaterSplashCubes.SplashType type)
	{
		if (WaterSplashCubes.waterFallSplashCubeEffect == null)
		{
			WaterSplashCubes.waterFallSplashCubeEffect = new UnityEngine.Object[EnumUtils.Names<WaterSplashCubes.SplashType>().Count];
			WaterSplashCubes.waterFallSplashCubeEffect[0] = Resources.Load("prefabs/WaterFallSplashCube");
			WaterSplashCubes.waterFallSplashCubeEffect[1] = Resources.Load("prefabs/WaterFallSlopeParticles");
			WaterSplashCubes.waterFallSplashCubeEffect[2] = Resources.Load("prefabs/WaterFallAreaParticles");
			WaterSplashCubes.waterFallSplashCubeEffect[3] = Resources.Load("prefabs/WaterFallSplashCube");
		}
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(WaterSplashCubes.waterFallSplashCubeEffect[(int)type]);
		WaterSplashCubes.particleCount++;
		switch (face)
		{
		case BlockFace.Top:
			gameObject.transform.Rotate(new Vector3(90f, 0f, 0f));
			break;
		case BlockFace.Bottom:
			gameObject.transform.Rotate(new Vector3(-90f, 0f, 0f));
			break;
		case BlockFace.North:
			gameObject.transform.Rotate(new Vector3(0f, 0f, 0f));
			break;
		case BlockFace.West:
			gameObject.transform.Rotate(new Vector3(0f, -90f, 0f));
			break;
		case BlockFace.South:
			gameObject.transform.Rotate(new Vector3(0f, 180f, 0f));
			break;
		case BlockFace.East:
			gameObject.transform.Rotate(new Vector3(0f, 90f, 0f));
			break;
		}
		gameObject.transform.position = new Vector3((float)pos.x + 0.5f, (float)pos.y + 0.5f, (float)pos.z + 0.5f) - Origin.position;
		gameObject.transform.parent = WaterSplashCubes.root.transform;
		if (WaterSplashCubes.particleLimiter < 1f && WaterSplashCubes.particleCount % Mathf.CeilToInt((1f - WaterSplashCubes.particleLimiter) * 6f) != 0)
		{
			gameObject.SetActive(false);
		}
		return gameObject;
	}

	// Token: 0x06000AD4 RID: 2772 RVA: 0x00046A00 File Offset: 0x00044C00
	public static void Update()
	{
		if (WaterSplashCubes.removeList == null)
		{
			return;
		}
		if (WaterSplashCubes.addList == null)
		{
			return;
		}
		List<Vector3i> obj = WaterSplashCubes.removeList;
		lock (obj)
		{
			for (int i = 0; i < WaterSplashCubes.removeList.Count; i++)
			{
				Vector3i vector3i = WaterSplashCubes.removeList[i];
				int x = vector3i.x;
				int y = vector3i.y;
				int z = vector3i.z;
				long key = WaterSplashCubes.MakeKey(x, z);
				object syncRoot = WaterSplashCubes.GetSyncRoot();
				lock (syncRoot)
				{
					if (WaterSplashCubes.splashes.dict.ContainsKey(key))
					{
						DictionaryList<int, GameObject> dictionaryList = WaterSplashCubes.splashes.dict[key];
						if (dictionaryList.dict.ContainsKey(y))
						{
							UnityEngine.Object.DestroyImmediate(dictionaryList.dict[y]);
							dictionaryList.Remove(y);
						}
					}
				}
			}
			WaterSplashCubes.removeList.Clear();
		}
		List<WaterSplashCubes.ParticlePlacement> obj2 = WaterSplashCubes.addList;
		lock (obj2)
		{
			for (int j = 0; j < WaterSplashCubes.addList.Count; j++)
			{
				WaterSplashCubes.ParticlePlacement particlePlacement = WaterSplashCubes.addList[j];
				int x2 = particlePlacement.pos.x;
				int y2 = particlePlacement.pos.y;
				int z2 = particlePlacement.pos.z;
				long key2 = WaterSplashCubes.MakeKey(x2, z2);
				object syncRoot = WaterSplashCubes.GetSyncRoot();
				lock (syncRoot)
				{
					if (!WaterSplashCubes.splashes.dict.ContainsKey(key2))
					{
						DictionaryList<int, GameObject> dictionaryList2 = new DictionaryList<int, GameObject>();
						dictionaryList2.Add(y2, WaterSplashCubes.AddParticleEffect(particlePlacement.pos, particlePlacement.dir, particlePlacement.type));
						WaterSplashCubes.splashes.Add(key2, dictionaryList2);
					}
					else
					{
						DictionaryList<int, GameObject> dictionaryList3 = WaterSplashCubes.splashes.dict[key2];
						if (!dictionaryList3.dict.ContainsKey(y2))
						{
							dictionaryList3.Add(y2, WaterSplashCubes.AddParticleEffect(particlePlacement.pos, particlePlacement.dir, particlePlacement.type));
						}
					}
				}
			}
			WaterSplashCubes.addList.Clear();
		}
		WaterSplashCubes.CleanUp();
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x00046CA4 File Offset: 0x00044EA4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void CleanUp()
	{
		if (Time.time > WaterSplashCubes.cleanUpTimer + 0.15f)
		{
			WaterSplashCubes.cleanUpTimer = Time.time;
			if (WaterSplashCubes.root == null)
			{
				return;
			}
			if (WaterSplashCubes.root.transform == null)
			{
				return;
			}
			if (GameManager.Instance == null)
			{
				return;
			}
			World world = GameManager.Instance.World;
			if (world == null)
			{
				return;
			}
			bool flag = false;
			if (WaterSplashCubes.currentCleanupIndex >= WaterSplashCubes.root.transform.childCount)
			{
				WaterSplashCubes.currentCleanupIndex = WaterSplashCubes.root.transform.childCount;
				flag = true;
			}
			int num = 0;
			while (num < WaterSplashCubes.root.transform.childCount && num < WaterSplashCubes.currentCleanupIndex)
			{
				Transform child = WaterSplashCubes.root.transform.GetChild(num);
				if (child != null)
				{
					Vector3i vector3i = new Vector3i((int)(child.position.x - 0.5f), (int)(child.position.y - 0.5f), (int)(child.position.z - 0.5f));
					if (!world.IsChunkAreaLoaded(vector3i.x, vector3i.y, vector3i.z))
					{
						WaterSplashCubes.RemoveSplashAt(vector3i.x, vector3i.y, vector3i.z);
					}
					else if (!world.IsWater(vector3i) || !world.IsAir((int)(child.position.x - 0.5f), (int)(child.position.y - 0.5f) + 1, (int)(child.position.z - 0.5f)))
					{
						WaterSplashCubes.RemoveSplashAt(vector3i.x, vector3i.y, vector3i.z);
					}
				}
				num++;
			}
			WaterSplashCubes.currentCleanupIndex += 15;
			if (flag)
			{
				WaterSplashCubes.currentCleanupIndex = 0;
			}
		}
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x00046E74 File Offset: 0x00045074
	public static void RemoveSplashAt(int _x, int _y, int _z)
	{
		if (WaterSplashCubes.removeList == null)
		{
			return;
		}
		List<Vector3i> obj = WaterSplashCubes.removeList;
		lock (obj)
		{
			WaterSplashCubes.removeList.Add(new Vector3i(_x, _y, _z));
		}
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x00046EC8 File Offset: 0x000450C8
	public static void AddSplashAt(int _x, int _y, int _z, BlockFace _dir, WaterSplashCubes.SplashType _type)
	{
		if (WaterSplashCubes.particleLimiter <= 0f)
		{
			return;
		}
		if (WaterSplashCubes.addList == null)
		{
			return;
		}
		List<WaterSplashCubes.ParticlePlacement> obj = WaterSplashCubes.addList;
		lock (obj)
		{
			WaterSplashCubes.ParticlePlacement item = new WaterSplashCubes.ParticlePlacement(new Vector3i(_x, _y, _z), _dir, _type);
			WaterSplashCubes.addList.Add(item);
		}
	}

	// Token: 0x06000AD8 RID: 2776 RVA: 0x00046F34 File Offset: 0x00045134
	public static void Clear()
	{
		object syncRoot = WaterSplashCubes.GetSyncRoot();
		lock (syncRoot)
		{
			for (int i = 0; i < WaterSplashCubes.splashes.Count; i++)
			{
				DictionaryList<int, GameObject> dictionaryList = WaterSplashCubes.splashes.list[i];
				for (int j = 0; j < dictionaryList.list.Count; j++)
				{
					UnityEngine.Object.DestroyImmediate(dictionaryList.list[j]);
				}
				dictionaryList.Clear();
			}
			WaterSplashCubes.splashes.Clear();
		}
	}

	// Token: 0x0400098D RID: 2445
	public static WaterSplashCubes instance = null;

	// Token: 0x0400098E RID: 2446
	[PublicizedFrom(EAccessModifier.Private)]
	public static DictionaryList<long, DictionaryList<int, GameObject>> splashes = new DictionaryList<long, DictionaryList<int, GameObject>>();

	// Token: 0x0400098F RID: 2447
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject root;

	// Token: 0x04000990 RID: 2448
	public static List<WaterSplashCubes.ParticlePlacement> addList;

	// Token: 0x04000991 RID: 2449
	public static List<Vector3i> removeList;

	// Token: 0x04000992 RID: 2450
	[PublicizedFrom(EAccessModifier.Private)]
	public static int checkListNum = 0;

	// Token: 0x04000993 RID: 2451
	[PublicizedFrom(EAccessModifier.Private)]
	public static UnityEngine.Object[] waterFallSplashCubeEffect = null;

	// Token: 0x04000994 RID: 2452
	public static float particleLimiter = 1f;

	// Token: 0x04000995 RID: 2453
	[PublicizedFrom(EAccessModifier.Private)]
	public static int particleCount = 0;

	// Token: 0x04000996 RID: 2454
	[PublicizedFrom(EAccessModifier.Private)]
	public static float cleanUpTimer = 0f;

	// Token: 0x04000997 RID: 2455
	[PublicizedFrom(EAccessModifier.Private)]
	public static int currentCleanupIndex = 15;

	// Token: 0x0200016C RID: 364
	public enum SplashType
	{
		// Token: 0x04000999 RID: 2457
		Splash,
		// Token: 0x0400099A RID: 2458
		Slope,
		// Token: 0x0400099B RID: 2459
		Area,
		// Token: 0x0400099C RID: 2460
		Mist
	}

	// Token: 0x0200016D RID: 365
	public class ParticlePlacement
	{
		// Token: 0x06000ADA RID: 2778 RVA: 0x0004700F File Offset: 0x0004520F
		public ParticlePlacement(Vector3i _pos, BlockFace _dir, WaterSplashCubes.SplashType _type)
		{
			this.pos = _pos;
			this.dir = _dir;
			this.type = _type;
		}

		// Token: 0x0400099D RID: 2461
		public Vector3i pos;

		// Token: 0x0400099E RID: 2462
		public BlockFace dir;

		// Token: 0x0400099F RID: 2463
		public WaterSplashCubes.SplashType type;
	}
}
