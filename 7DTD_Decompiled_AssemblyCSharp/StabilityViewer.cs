using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02001073 RID: 4211
public class StabilityViewer
{
	// Token: 0x06008554 RID: 34132 RVA: 0x00363798 File Offset: 0x00361998
	public StabilityViewer()
	{
		StabilityViewer.displayObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		StabilityViewer.displayObject.transform.localScale = Vector3.one * 1.01f;
		UnityEngine.Object.Destroy(StabilityViewer.displayObject.GetComponent<BoxCollider>());
		for (int i = 0; i < StabilityViewer.numMaterials; i++)
		{
			StabilityViewer.materials.Add(Resources.Load<Material>("Materials/Stability" + i.ToString()));
		}
		StabilityViewer.stabilityMtrl = Resources.Load<Material>("Materials/Stability");
		StabilityViewer.StabilityViewBoxes = new GameObject();
		StabilityViewer.StabilityViewBoxes.name = "StabilityViewBoxes";
		if (this.debugText == null)
		{
			GameObject gameObject = Resources.Load<GameObject>("Prefabs/StabilityCanvas");
			if (gameObject != null)
			{
				this.textGo = UnityEngine.Object.Instantiate<GameObject>(gameObject);
				if (this.textGo != null)
				{
					this.debugText = this.textGo.GetComponentInChildren<Text>();
					this.debugText.text = "Recalculating Stability.  Please Wait...";
				}
			}
		}
		this.startedSearch = false;
		this.flipFlop = false;
		StabilityViewer.bGatheringChunks = false;
		this.CalculatedStability = false;
	}

	// Token: 0x17000DEA RID: 3562
	// (get) Token: 0x06008555 RID: 34133 RVA: 0x0028FB89 File Offset: 0x0028DD89
	public ChunkCluster ChunkCluster0
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (GameManager.Instance == null)
			{
				return null;
			}
			if (GameManager.Instance.World != null)
			{
				return GameManager.Instance.World.ChunkClusters[0];
			}
			return null;
		}
	}

	// Token: 0x06008556 RID: 34134 RVA: 0x003638CC File Offset: 0x00361ACC
	[PublicizedFrom(EAccessModifier.Private)]
	public void RecalcStability()
	{
		if (this.ChunkCluster0 != null)
		{
			ReaderWriterLockSlim syncRoot = this.ChunkCluster0.GetSyncRoot();
			syncRoot.EnterWriteLock();
			StabilityInitializer stabilityInitializer = new StabilityInitializer(GameManager.Instance.World);
			MicroStopwatch microStopwatch = new MicroStopwatch();
			foreach (Chunk chunk in this.ChunkCluster0.GetChunkArray())
			{
				chunk.ResetStability();
			}
			syncRoot.ExitWriteLock();
			foreach (Chunk chunk2 in this.ChunkCluster0.GetChunkArray())
			{
				stabilityInitializer.DistributeStability(chunk2);
				chunk2.NeedsRegeneration = true;
			}
			Log.Out(string.Concat(new string[]
			{
				"#",
				this.ChunkCluster0.GetChunkArray().Count.ToString(),
				" chunks needed ",
				microStopwatch.ElapsedMilliseconds.ToString(),
				"ms"
			}));
		}
	}

	// Token: 0x06008557 RID: 34135 RVA: 0x00363A04 File Offset: 0x00361C04
	public static GameObject GetBlock(float stability)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(StabilityViewer.displayObject);
		if (stability <= 0f)
		{
			gameObject.GetComponent<MeshRenderer>().material = StabilityViewer.materials[StabilityViewer.numMaterials - 1];
			return gameObject;
		}
		float num = 1f / (float)StabilityViewer.numMaterials;
		bool flag = false;
		for (int i = 0; i < StabilityViewer.numMaterials; i++)
		{
			if (stability >= 1f - num * (float)i)
			{
				gameObject.GetComponent<MeshRenderer>().material = StabilityViewer.materials[i];
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			gameObject.GetComponent<MeshRenderer>().material = StabilityViewer.materials[StabilityViewer.numMaterials - 1];
		}
		return gameObject;
	}

	// Token: 0x06008558 RID: 34136 RVA: 0x00363AA8 File Offset: 0x00361CA8
	public void OnDestroy()
	{
		this.Clear();
	}

	// Token: 0x06008559 RID: 34137 RVA: 0x00363AB0 File Offset: 0x00361CB0
	public void Clear()
	{
		StabilityViewer.TotalIterations = 0;
		StabilityViewer.buildingChunks.Clear();
		StabilityViewer.boxes.Clear();
		UnityEngine.Object.Destroy(StabilityViewer.StabilityViewBoxes);
		StabilityViewer.StabilityViewBoxes = null;
		UnityEngine.Object.Destroy(this.textGo);
		this.textGo = null;
		this.debugText = null;
		UnityEngine.Object.Destroy(StabilityViewer.displayObject);
		StabilityViewer.displayObject = null;
		this.CalculatedStability = false;
	}

	// Token: 0x0600855A RID: 34138 RVA: 0x00363B18 File Offset: 0x00361D18
	public void StartSearch(int _asynCount = 100)
	{
		this.startedSearch = true;
		this.asynCount = _asynCount;
		StabilityViewer.bGatheringChunks = true;
		this.currSearch.x = -this.searchSizeXZ;
		this.currSearch.y = -this.searchSizeY;
		this.currSearch.z = -this.searchSizeXZ;
		this.totalTimer = Time.realtimeSinceStartup;
		this.prevTotalTimer = Time.realtimeSinceStartup;
		this.updateDisplayTimer = 0f;
	}

	// Token: 0x0600855B RID: 34139 RVA: 0x00363B90 File Offset: 0x00361D90
	public void Update()
	{
		if (Time.time > this.updateDisplayTimer + 3f)
		{
			this.updateDisplayTimer = Time.time;
			this.prevTotalTimer = this.totalTime;
			if (!this.CalculatedStability)
			{
				this.RecalcStability();
				this.CalculatedStability = true;
			}
		}
		if (!StabilityViewer.bGatheringChunks)
		{
			return;
		}
		this.totalTime = Time.realtimeSinceStartup - this.totalTimer;
		if (this.debugText != null)
		{
			this.debugText.text = string.Concat(new string[]
			{
				"Chunks Finished: ",
				StabilityViewer.boxes.Count.ToString(),
				" Time( ",
				this.prevTotalTimer.ToCultureInvariantString(),
				" : ",
				this.totalTime.ToCultureInvariantString(),
				" ) GetBlock(): ",
				StabilityViewer.GetBlocks.ToString()
			});
		}
		if (StabilityViewer.buildingChunks.Count > this.asynCount)
		{
			return;
		}
		if (!this.worldIsReady && GameManager.Instance.World != null && Camera.main != null)
		{
			this.mainCam = Camera.main;
			this.worldIsReady = true;
		}
		if (GameManager.Instance.World == null)
		{
			this.worldIsReady = false;
		}
		if (!this.worldIsReady)
		{
			return;
		}
		Vector3i vector3i;
		vector3i.x = Mathf.FloorToInt(this.mainCam.transform.position.x);
		vector3i.y = Mathf.FloorToInt(this.mainCam.transform.position.y);
		vector3i.z = Mathf.FloorToInt(this.mainCam.transform.position.z);
		ChunkCluster chunkCluster = GameManager.Instance.World.ChunkClusters[0];
		Chunk chunk = (Chunk)chunkCluster.GetChunkFromWorldPos(vector3i);
		if (chunk == null)
		{
			return;
		}
		this.startPos = chunk.GetWorldPos();
		this.startPos.y = vector3i.y - vector3i.y % 16;
		Dictionary<Vector3i, GameObject> obj = StabilityViewer.boxes;
		lock (obj)
		{
			List<Vector3i> obj2 = StabilityViewer.buildingChunks;
			lock (obj2)
			{
				if (!StabilityViewer.boxes.ContainsKey(this.startPos) && !StabilityViewer.buildingChunks.Contains(this.startPos))
				{
					StabilityViewer.buildingChunks.Add(this.startPos);
					new StabilityViewer.BuildStabilityBlocks(this.startPos);
				}
			}
		}
		Vector3i zero = Vector3i.zero;
		obj = StabilityViewer.boxes;
		lock (obj)
		{
			List<Vector3i> obj2 = StabilityViewer.buildingChunks;
			lock (obj2)
			{
				zero.x = this.currSearch.x * 16;
				zero.y = this.currSearch.y * 16;
				zero.z = this.currSearch.z * 16;
				Vector3i vector3i2 = this.startPos + zero;
				if (!StabilityViewer.boxes.ContainsKey(vector3i2) && !StabilityViewer.buildingChunks.Contains(vector3i2))
				{
					Chunk chunk2 = (Chunk)chunkCluster.GetChunkFromWorldPos(vector3i2);
					if (chunk2 == null)
					{
						StabilityViewer.boxes.Add(vector3i2, null);
					}
					else if (chunk2.GetAvailable())
					{
						if (chunk2.IsEmpty())
						{
							StabilityViewer.boxes.Add(vector3i2, null);
						}
						else
						{
							StabilityViewer.buildingChunks.Add(vector3i2);
							new StabilityViewer.BuildStabilityBlocks(vector3i2);
						}
					}
					else
					{
						StabilityViewer.buildingChunks.Add(vector3i2);
						new StabilityViewer.BuildStabilityBlocks(vector3i2);
					}
				}
			}
		}
		this.currSearch.x = this.currSearch.x + 1;
		if (this.currSearch.x >= this.searchSizeXZ)
		{
			this.currSearch.x = -this.searchSizeXZ;
			this.currSearch.z = this.currSearch.z + 1;
		}
		if (this.currSearch.z >= this.searchSizeXZ)
		{
			this.currSearch.z = -this.searchSizeXZ;
			this.currSearch.y = this.currSearch.y + 1;
		}
		if (this.currSearch.y >= this.searchSizeY)
		{
			this.currSearch.y = -this.searchSizeY;
			StabilityViewer.bGatheringChunks = false;
			Log.Out("Stability DONE");
		}
	}

	// Token: 0x04006745 RID: 26437
	public static Dictionary<Vector3i, GameObject> boxes = new Dictionary<Vector3i, GameObject>();

	// Token: 0x04006746 RID: 26438
	public static List<Vector3i> buildingChunks = new List<Vector3i>();

	// Token: 0x04006747 RID: 26439
	public bool worldIsReady;

	// Token: 0x04006748 RID: 26440
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i startPos;

	// Token: 0x04006749 RID: 26441
	public static List<Material> materials = new List<Material>();

	// Token: 0x0400674A RID: 26442
	public static GameObject displayObject = null;

	// Token: 0x0400674B RID: 26443
	public static GameObject StabilityViewBoxes = null;

	// Token: 0x0400674C RID: 26444
	public static Material stabilityMtrl = null;

	// Token: 0x0400674D RID: 26445
	public static int numMaterials = 7;

	// Token: 0x0400674E RID: 26446
	public static bool bGatheringChunks = false;

	// Token: 0x0400674F RID: 26447
	public static int TotalIterations = 0;

	// Token: 0x04006750 RID: 26448
	public static int GetBlocks = 0;

	// Token: 0x04006751 RID: 26449
	[PublicizedFrom(EAccessModifier.Private)]
	public Text debugText;

	// Token: 0x04006752 RID: 26450
	public int searchSizeXZ = 5;

	// Token: 0x04006753 RID: 26451
	public int searchSizeY = 2;

	// Token: 0x04006754 RID: 26452
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i currSearch;

	// Token: 0x04006755 RID: 26453
	[PublicizedFrom(EAccessModifier.Private)]
	public int asynCount = 100;

	// Token: 0x04006756 RID: 26454
	[PublicizedFrom(EAccessModifier.Private)]
	public float totalTimer;

	// Token: 0x04006757 RID: 26455
	[PublicizedFrom(EAccessModifier.Private)]
	public float prevTotalTimer;

	// Token: 0x04006758 RID: 26456
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateDisplayTimer;

	// Token: 0x04006759 RID: 26457
	[PublicizedFrom(EAccessModifier.Private)]
	public float totalTime;

	// Token: 0x0400675A RID: 26458
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject textGo;

	// Token: 0x0400675B RID: 26459
	[PublicizedFrom(EAccessModifier.Private)]
	public Camera mainCam;

	// Token: 0x0400675C RID: 26460
	[PublicizedFrom(EAccessModifier.Private)]
	public bool startedSearch;

	// Token: 0x0400675D RID: 26461
	[PublicizedFrom(EAccessModifier.Private)]
	public bool flipFlop;

	// Token: 0x0400675E RID: 26462
	[PublicizedFrom(EAccessModifier.Private)]
	public bool CalculatedStability;

	// Token: 0x02001074 RID: 4212
	public class BuildStabilityBlocks
	{
		// Token: 0x0600855D RID: 34141 RVA: 0x00364095 File Offset: 0x00362295
		public BuildStabilityBlocks(Vector3i _startPos)
		{
			this.startPos = _startPos;
			GameManager.Instance.StartCoroutine(this.RegisterWhenDone());
		}

		// Token: 0x0600855E RID: 34142 RVA: 0x003640B5 File Offset: 0x003622B5
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator RegisterWhenDone()
		{
			List<Vector3i> positions = null;
			List<float> stabilityValues = null;
			yield return new WaitForSeconds(0.01f);
			Chunk chunk = (Chunk)GameManager.Instance.World.ChunkClusters[0].GetChunkFromWorldPos(this.startPos);
			List<Vector3i> buildingChunks;
			if (chunk == null)
			{
				Dictionary<Vector3i, GameObject> boxes = StabilityViewer.boxes;
				lock (boxes)
				{
					buildingChunks = StabilityViewer.buildingChunks;
					lock (buildingChunks)
					{
						StabilityViewer.buildingChunks.Remove(this.startPos);
						StabilityViewer.boxes[this.startPos] = null;
						yield break;
					}
				}
			}
			if (chunk.GetAvailable())
			{
				Dictionary<Vector3i, GameObject> boxes;
				if (chunk.IsEmpty())
				{
					boxes = StabilityViewer.boxes;
					lock (boxes)
					{
						buildingChunks = StabilityViewer.buildingChunks;
						lock (buildingChunks)
						{
							StabilityViewer.buildingChunks.Remove(this.startPos);
							StabilityViewer.boxes[this.startPos] = null;
							goto IL_23C;
						}
					}
				}
				positions = new List<Vector3i>();
				stabilityValues = new List<float>();
				for (int i = 0; i < 16; i++)
				{
					for (int j = 0; j < 16; j++)
					{
						for (int k = 0; k < 16; k++)
						{
							Vector3i other;
							other.x = k;
							other.y = j;
							other.z = i;
							BlockValue block = GameManager.Instance.World.GetBlock(this.startPos + other);
							if (!block.Block.shape.IsTerrain() && !block.isair)
							{
								float blockStability = StabilityCalculator.GetBlockStability(this.startPos + other);
								positions.Add(this.startPos + other);
								stabilityValues.Add(blockStability);
							}
						}
					}
				}
				IL_23C:
				GameObject gameObject = null;
				if (positions != null)
				{
					gameObject = new GameObject();
					gameObject.AddComponent<MeshRenderer>().material = StabilityViewer.stabilityMtrl;
					MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
					meshFilter.mesh = new Mesh();
					Vector3[] array = new Vector3[8 * positions.Count];
					Color[] array2 = new Color[8 * positions.Count];
					int[] array3 = new int[36 * positions.Count];
					for (int l = 0; l < positions.Count; l++)
					{
						for (int m = 0; m < 8; m++)
						{
							array2[l * 8 + m] = Color.white * stabilityValues[l];
						}
						array[l * 8].x = (float)positions[l].x - 0.01f;
						array[l * 8].y = (float)positions[l].y - 0.01f;
						array[l * 8].z = (float)positions[l].z - 0.01f;
						array[l * 8 + 1].x = (float)positions[l].x - 0.01f;
						array[l * 8 + 1].y = (float)positions[l].y - 0.01f;
						array[l * 8 + 1].z = (float)positions[l].z + 1.01f;
						array[l * 8 + 2].x = (float)positions[l].x + 1.01f;
						array[l * 8 + 2].y = (float)positions[l].y - 0.01f;
						array[l * 8 + 2].z = (float)positions[l].z + 1.01f;
						array[l * 8 + 3].x = (float)positions[l].x + 1.01f;
						array[l * 8 + 3].y = (float)positions[l].y - 0.01f;
						array[l * 8 + 3].z = (float)positions[l].z - 0.01f;
						array[l * 8 + 4].x = (float)positions[l].x - 0.01f;
						array[l * 8 + 4].y = (float)positions[l].y + 1.01f;
						array[l * 8 + 4].z = (float)positions[l].z - 0.01f;
						array[l * 8 + 5].x = (float)positions[l].x - 0.01f;
						array[l * 8 + 5].y = (float)positions[l].y + 1.01f;
						array[l * 8 + 5].z = (float)positions[l].z + 1.01f;
						array[l * 8 + 6].x = (float)positions[l].x + 1.01f;
						array[l * 8 + 6].y = (float)positions[l].y + 1.01f;
						array[l * 8 + 6].z = (float)positions[l].z + 1.01f;
						array[l * 8 + 7].x = (float)positions[l].x + 1.01f;
						array[l * 8 + 7].y = (float)positions[l].y + 1.01f;
						array[l * 8 + 7].z = (float)positions[l].z - 0.01f;
						int num = 0;
						array3[l * 36 + num] = l * 8;
						num++;
						array3[l * 36 + num] = 3 + l * 8;
						num++;
						array3[l * 36 + num] = 2 + l * 8;
						num++;
						array3[l * 36 + num] = 2 + l * 8;
						num++;
						array3[l * 36 + num] = 1 + l * 8;
						num++;
						array3[l * 36 + num] = l * 8;
						num++;
						array3[l * 36 + num] = 4 + l * 8;
						num++;
						array3[l * 36 + num] = 5 + l * 8;
						num++;
						array3[l * 36 + num] = 6 + l * 8;
						num++;
						array3[l * 36 + num] = 6 + l * 8;
						num++;
						array3[l * 36 + num] = 7 + l * 8;
						num++;
						array3[l * 36 + num] = 4 + l * 8;
						num++;
						array3[l * 36 + num] = 4 + l * 8;
						num++;
						array3[l * 36 + num] = 7 + l * 8;
						num++;
						array3[l * 36 + num] = 3 + l * 8;
						num++;
						array3[l * 36 + num] = 3 + l * 8;
						num++;
						array3[l * 36 + num] = l * 8;
						num++;
						array3[l * 36 + num] = 4 + l * 8;
						num++;
						array3[l * 36 + num] = 6 + l * 8;
						num++;
						array3[l * 36 + num] = 5 + l * 8;
						num++;
						array3[l * 36 + num] = 1 + l * 8;
						num++;
						array3[l * 36 + num] = 1 + l * 8;
						num++;
						array3[l * 36 + num] = 2 + l * 8;
						num++;
						array3[l * 36 + num] = 6 + l * 8;
						num++;
						array3[l * 36 + num] = 7 + l * 8;
						num++;
						array3[l * 36 + num] = 6 + l * 8;
						num++;
						array3[l * 36 + num] = 2 + l * 8;
						num++;
						array3[l * 36 + num] = 2 + l * 8;
						num++;
						array3[l * 36 + num] = 3 + l * 8;
						num++;
						array3[l * 36 + num] = 7 + l * 8;
						num++;
						array3[l * 36 + num] = 5 + l * 8;
						num++;
						array3[l * 36 + num] = 4 + l * 8;
						num++;
						array3[l * 36 + num] = l * 8;
						num++;
						array3[l * 36 + num] = l * 8;
						num++;
						array3[l * 36 + num] = 1 + l * 8;
						num++;
						array3[l * 36 + num] = 5 + l * 8;
						num++;
					}
					meshFilter.mesh.Clear(false);
					meshFilter.mesh.vertices = array;
					meshFilter.mesh.SetIndices(array3, MeshTopology.Triangles, 0);
					meshFilter.mesh.colors = array2;
					if (StabilityViewer.StabilityViewBoxes != null)
					{
						gameObject.transform.parent = StabilityViewer.StabilityViewBoxes.transform;
					}
					else
					{
						UnityEngine.Object.Destroy(gameObject);
					}
				}
				boxes = StabilityViewer.boxes;
				lock (boxes)
				{
					buildingChunks = StabilityViewer.buildingChunks;
					lock (buildingChunks)
					{
						StabilityViewer.boxes[this.startPos] = gameObject;
						StabilityViewer.buildingChunks.Remove(this.startPos);
						yield break;
					}
				}
			}
			buildingChunks = StabilityViewer.buildingChunks;
			lock (buildingChunks)
			{
				StabilityViewer.buildingChunks.Remove(this.startPos);
				yield break;
			}
			yield break;
		}

		// Token: 0x0400675F RID: 26463
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3i startPos;
	}
}
