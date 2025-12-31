using System;
using System.Collections;
using System.Collections.Generic;
using GamePath;
using Pathfinding;
using UnityEngine;

// Token: 0x02000810 RID: 2064
public class AstarManager : MonoBehaviour
{
	// Token: 0x06003B4F RID: 15183 RVA: 0x0017D500 File Offset: 0x0017B700
	public static void Init(GameObject obj)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (GamePrefs.GetString(EnumGamePrefs.GameWorld) == "Empty")
		{
			return;
		}
		Log.Out("AstarManager Init");
		obj.AddComponent<AstarManager>();
		new ASPPathFinderThread().StartWorkerThreads();
	}

	// Token: 0x06003B50 RID: 15184 RVA: 0x0017D540 File Offset: 0x0017B740
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		AstarManager.Instance = this;
		if (!AstarPath.active)
		{
			UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/AStarPath"), Vector3.zero, Quaternion.identity).transform.SetParent(GameManager.Instance.transform, false);
		}
		this.astar = AstarPath.active;
		ChunkCluster chunkCache = GameManager.Instance.World.ChunkCache;
		chunkCache.OnBlockChangedDelegates += this.OnBlockChanged;
		chunkCache.OnBlockDamagedDelegates += this.OnBlockDamaged;
		this.OriginChanged();
	}

	// Token: 0x06003B51 RID: 15185 RVA: 0x0017D5D0 File Offset: 0x0017B7D0
	public static PathNavigate CreateNavigator(EntityAlive _entity)
	{
		return new ASPPathNavigate(_entity);
	}

	// Token: 0x06003B52 RID: 15186 RVA: 0x0017D5D8 File Offset: 0x0017B7D8
	public static void Cleanup()
	{
		if (!AstarManager.Instance)
		{
			return;
		}
		Log.Out("AstarManager Cleanup");
		ChunkCluster chunkCache = GameManager.Instance.World.ChunkCache;
		chunkCache.OnBlockChangedDelegates -= AstarManager.Instance.OnBlockChanged;
		chunkCache.OnBlockDamagedDelegates -= AstarManager.Instance.OnBlockDamaged;
		PathFinderThread.Instance.Cleanup();
		if (AstarPath.active)
		{
			AstarPath.active.enabled = false;
			UnityEngine.Object.Destroy(AstarPath.active.gameObject);
		}
		UnityEngine.Object.Destroy(AstarManager.Instance);
		AstarManager.Instance = null;
	}

	// Token: 0x06003B53 RID: 15187 RVA: 0x0017D676 File Offset: 0x0017B876
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator Start()
	{
		float elapsedTime = 0f;
		while (this.astar != null)
		{
			if (GamePrefs.GetBool(EnumGamePrefs.DebugStopEnemiesMoving))
			{
				yield return new WaitForSeconds(0.1f);
			}
			else
			{
				yield return new WaitForSeconds(0.1f);
				if (!(GameManager.Instance == null) && GameManager.Instance.World != null)
				{
					elapsedTime += 0.1f;
					if (this.astar.IsAnyWorkItemInProgress)
					{
						this.lastWorkTime = Time.time;
					}
					else if (Time.time - this.lastWorkTime >= 0.4f && !this.astar.IsAnyGraphUpdateInProgress)
					{
						this.UpdateGraphs(elapsedTime);
						int num = this.areaList.Count;
						if (num > 0)
						{
							num = Mathf.Min(20, num);
							int num2 = 0;
							for (int i = 0; i < num; i++)
							{
								AstarManager.Area area = this.areaList[num2];
								area.updateDelay -= elapsedTime;
								if (area.updateDelay > 0f)
								{
									num2++;
								}
								else
								{
									if (area.next == null)
									{
										this.areaList.RemoveAt(num2);
									}
									else
									{
										this.areaList[num2] = area.next;
										num2++;
									}
									Bounds bounds;
									if (!area.isPartial)
									{
										bounds = default(Bounds);
										Vector3 vector = new Vector3((float)area.pos.x, 0f, (float)area.pos.y);
										Vector3 max = vector;
										max.x += 16f;
										max.z += 16f;
										bounds.SetMinMax(vector, max);
									}
									else
									{
										if (!area.hasBlocks)
										{
											goto IL_294;
										}
										bounds = area.bounds.ToBounds();
									}
									Vector3 vector2 = bounds.center;
									vector2.y = 128f;
									vector2 -= this.worldOrigin;
									bounds.center = vector2;
									Vector3 size = bounds.size;
									size.y = 320f;
									bounds.size = size;
									if (this.graphList.Count > 0)
									{
										LayerGridGraphUpdate layerGridGraphUpdate = new LayerGridGraphUpdate();
										layerGridGraphUpdate.bounds = bounds;
										layerGridGraphUpdate.recalculateNodes = true;
										this.astar.UpdateGraphs(layerGridGraphUpdate);
									}
								}
								IL_294:;
							}
						}
						elapsedTime = 0f;
					}
				}
			}
		}
		yield break;
	}

	// Token: 0x06003B54 RID: 15188 RVA: 0x0017D688 File Offset: 0x0017B888
	public void AddLocation(Vector3 pos3d, int size)
	{
		Vector2 vector;
		vector.x = pos3d.x;
		vector.y = pos3d.z;
		AstarManager.Location location = this.FindLocation(vector, size);
		if (location == null)
		{
			location = new AstarManager.Location();
			location.pos = vector;
			location.size = size;
			this.locations.Add(location);
		}
		else
		{
			location.pos = (location.pos + vector) * 0.5f;
		}
		location.duration = 4f;
	}

	// Token: 0x06003B55 RID: 15189 RVA: 0x0017D704 File Offset: 0x0017B904
	public void AddLocationLine(Vector3 startPos, Vector3 endPos, int size)
	{
		startPos.y = 0f;
		endPos.y = 0f;
		Vector3 normalized = (endPos - startPos).normalized;
		Vector3 pos3d = startPos + normalized * ((float)size * 0.4f);
		this.AddLocation(pos3d, size);
	}

	// Token: 0x06003B56 RID: 15190 RVA: 0x0017D758 File Offset: 0x0017B958
	[PublicizedFrom(EAccessModifier.Private)]
	public AstarManager.Location FindLocation(Vector2 pos, int size)
	{
		AstarManager.Location result = null;
		float num = (float)(size * size) * 0.040000003f;
		for (int i = 0; i < this.locations.Count; i++)
		{
			AstarManager.Location location = this.locations[i];
			if (location.size >= size)
			{
				float sqrMagnitude = (location.pos - pos).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					result = location;
					num = sqrMagnitude;
				}
			}
		}
		return result;
	}

	// Token: 0x06003B57 RID: 15191 RVA: 0x0017D7C0 File Offset: 0x0017B9C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateGraphs(float deltaTime)
	{
		World world = GameManager.Instance.World;
		this.mergedLocations.Clear();
		List<EntityPlayer> list = world.Players.list;
		for (int i = 0; i < list.Count; i++)
		{
			EntityPlayer entityPlayer = list[i];
			Vector2 pos;
			pos.x = entityPlayer.position.x;
			pos.y = entityPlayer.position.z;
			this.Merge(pos, 76);
		}
		for (int j = 0; j < this.locations.Count; j++)
		{
			AstarManager.Location location = this.locations[j];
			location.duration -= deltaTime;
			if (location.duration <= 0f)
			{
				this.locations.RemoveAt(j);
				j--;
			}
			else
			{
				this.Merge(location.pos, location.size);
			}
		}
		for (int k = 0; k < this.graphList.Count; k++)
		{
			this.graphList[k].IsUsed = false;
		}
		for (int l = 0; l < this.mergedLocations.Count; l++)
		{
			AstarManager.MergedLocations mergedLocations = this.mergedLocations[l];
			AstarVoxelGrid astarVoxelGrid = this.FindClosestGraph(mergedLocations.pos, mergedLocations.size);
			if (astarVoxelGrid == null)
			{
				astarVoxelGrid = this.AddGraph(mergedLocations.size);
				astarVoxelGrid.SetPos(this.LocalPosToGridPos(mergedLocations.pos - this.worldOriginXZ));
			}
			astarVoxelGrid.IsUsed = true;
			this.UpdateGraphPos(astarVoxelGrid, mergedLocations.pos);
		}
		this.UpdateMoveGraph();
		for (int m = 0; m < this.graphList.Count; m++)
		{
			AstarVoxelGrid astarVoxelGrid2 = this.graphList[m];
			if (!astarVoxelGrid2.IsUsed)
			{
				this.MoveGraphRemove(astarVoxelGrid2);
				this.astar.data.RemoveGraph(astarVoxelGrid2);
				this.graphList.RemoveAt(m);
				m--;
			}
		}
	}

	// Token: 0x06003B58 RID: 15192 RVA: 0x0017D9C0 File Offset: 0x0017BBC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Merge(Vector2 pos, int size)
	{
		bool flag = false;
		for (int i = 0; i < this.mergedLocations.Count; i++)
		{
			AstarManager.MergedLocations mergedLocations = this.mergedLocations[i];
			if (size <= mergedLocations.size && (mergedLocations.pos - pos).sqrMagnitude <= 361f)
			{
				mergedLocations.pos = (mergedLocations.pos + pos) * 0.5f;
				this.mergedLocations[i] = mergedLocations;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			AstarManager.MergedLocations item;
			item.pos = pos;
			item.size = size;
			this.mergedLocations.Add(item);
		}
	}

	// Token: 0x06003B59 RID: 15193 RVA: 0x0017DA64 File Offset: 0x0017BC64
	[PublicizedFrom(EAccessModifier.Private)]
	public int FindMoveIndex(AstarVoxelGrid graph)
	{
		for (int i = 0; i < this.moveList.Count; i++)
		{
			if (this.moveList[i] == graph)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06003B5A RID: 15194 RVA: 0x0017DA9C File Offset: 0x0017BC9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateGraphPos(AstarVoxelGrid graph, Vector2 pos)
	{
		if (graph.IsMoving())
		{
			return;
		}
		Vector2 vector = pos - this.worldOriginXZ;
		if (graph.IsFullUpdateNeeded)
		{
			Vector3 pos2 = this.LocalPosToGridPos(vector);
			graph.SetPos(pos2);
			return;
		}
		Vector2 a = vector;
		a.x -= graph.center.x;
		a.y -= graph.center.z;
		if (Vector2.SqrMagnitude(a) > 100f)
		{
			graph.GridMovePendingPos = pos;
			if (this.FindMoveIndex(graph) < 0)
			{
				this.moveList.Insert(this.moveList.Count, graph);
			}
		}
	}

	// Token: 0x06003B5B RID: 15195 RVA: 0x0017DB3C File Offset: 0x0017BD3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateMoveGraph()
	{
		if (this.moveCurrent != null)
		{
			if (this.moveCurrent.IsMoving())
			{
				return;
			}
			this.moveCurrent = null;
		}
		if (this.moveList.Count > 0)
		{
			AstarVoxelGrid astarVoxelGrid = this.moveList[0];
			this.moveList.RemoveAt(0);
			this.MoveGraph(astarVoxelGrid, astarVoxelGrid.GridMovePendingPos);
		}
	}

	// Token: 0x06003B5C RID: 15196 RVA: 0x0017DB9C File Offset: 0x0017BD9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void MoveGraphRemove(AstarVoxelGrid graph)
	{
		if (this.moveCurrent == graph)
		{
			this.moveCurrent = null;
		}
		int num = this.FindMoveIndex(graph);
		if (num >= 0)
		{
			this.moveList.RemoveAt(num);
		}
	}

	// Token: 0x06003B5D RID: 15197 RVA: 0x0017DBD4 File Offset: 0x0017BDD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void MoveGraph(AstarVoxelGrid graph, Vector2 pos)
	{
		this.moveCurrent = graph;
		Vector2 pos2 = pos - this.worldOriginXZ;
		Vector3 targetPos = this.LocalPosToGridPos(pos2);
		graph.Move(targetPos);
	}

	// Token: 0x06003B5E RID: 15198 RVA: 0x0017DC04 File Offset: 0x0017BE04
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 LocalPosToGridPos(Vector2 pos)
	{
		Vector3 result;
		result.x = Mathf.Round(pos.x);
		result.z = Mathf.Round(pos.y);
		result.y = -32f - this.worldOrigin.y;
		return result;
	}

	// Token: 0x06003B5F RID: 15199 RVA: 0x0017DC50 File Offset: 0x0017BE50
	public void OriginChanged()
	{
		this.worldOrigin = Origin.position;
		this.worldOriginXZ.x = this.worldOrigin.x;
		this.worldOriginXZ.y = this.worldOrigin.z;
		for (int i = 0; i < this.graphList.Count; i++)
		{
			this.graphList[i].IsFullUpdateNeeded = true;
		}
	}

	// Token: 0x06003B60 RID: 15200 RVA: 0x0017DCBC File Offset: 0x0017BEBC
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator Scan()
	{
		if (!this.astar.isScanning)
		{
			this.astar.Scan(null);
		}
		yield return null;
		yield break;
	}

	// Token: 0x06003B61 RID: 15201 RVA: 0x0017DCCC File Offset: 0x0017BECC
	public void OnBlockChanged(Vector3i pos, BlockValue bvOld, sbyte densOld, TextureFullArray texOld, BlockValue bvNew)
	{
		Block block = bvNew.Block;
		bool isSlowUpdate = block is BlockDoor;
		if (!block.isMultiBlock)
		{
			this.UpdateBlock(pos, isSlowUpdate);
			return;
		}
		int rotation = (int)bvNew.rotation;
		int length = block.multiBlockPos.Length;
		for (int i = 0; i < length; i++)
		{
			Vector3i vector3i = block.multiBlockPos.Get(i, bvNew.type, rotation);
			vector3i += pos;
			this.UpdateBlock(vector3i, isSlowUpdate);
		}
	}

	// Token: 0x06003B62 RID: 15202 RVA: 0x0017DD4A File Offset: 0x0017BF4A
	public void OnBlockDamaged(Vector3i _blockPos, BlockValue _blockValue, int _damage, int _attackerEntityId)
	{
		this.UpdateBlock(_blockPos, false);
	}

	// Token: 0x06003B63 RID: 15203 RVA: 0x0017DD54 File Offset: 0x0017BF54
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateBlock(Vector3i blockPos, bool isSlowUpdate)
	{
		Vector2i vector2i = new Vector2i(blockPos.x, blockPos.z);
		Vector2i vector2i2 = vector2i;
		vector2i2.x &= -16;
		vector2i2.y &= -16;
		AstarManager.Area area = this.AddAreaBlock(vector2i);
		area.hasBlocks = true;
		area.isSlowUpdate = isSlowUpdate;
		for (int i = 0; i < 4; i++)
		{
			Vector2i vector2i3 = vector2i;
			int num = i * 2;
			vector2i3.x += AstarManager.updateBlockOffsets[num];
			vector2i3.y += AstarManager.updateBlockOffsets[num + 1];
			Vector2i vector2i4 = vector2i3;
			vector2i4.x &= -16;
			vector2i4.y &= -16;
			if (vector2i4.x != vector2i2.x || vector2i4.y != vector2i2.y)
			{
				this.AddAreaBlock(vector2i3);
			}
		}
	}

	// Token: 0x06003B64 RID: 15204 RVA: 0x0017DE24 File Offset: 0x0017C024
	public static void AddBoundsToUpdate(Bounds _bounds)
	{
		if (AstarManager.Instance == null)
		{
			return;
		}
		Vector2i pos = new Vector2i(Mathf.FloorToInt(_bounds.min.x), Mathf.FloorToInt(_bounds.min.z));
		AstarManager.Area area = AstarManager.Instance.AddArea(pos, true);
		if (!area.isSlowUpdate)
		{
			area.updateDelay = 0f;
		}
	}

	// Token: 0x06003B65 RID: 15205 RVA: 0x0017DE88 File Offset: 0x0017C088
	[PublicizedFrom(EAccessModifier.Private)]
	public AstarManager.Area AddAreaBlock(Vector2i pos)
	{
		AstarManager.Area area = this.AddArea(pos, false);
		if (!area.isPartial)
		{
			area.isPartial = true;
			area.bounds.min = pos;
			area.bounds.max = pos;
		}
		else
		{
			area.bounds.Encapsulate(pos);
		}
		return area;
	}

	// Token: 0x06003B66 RID: 15206 RVA: 0x0017DED4 File Offset: 0x0017C0D4
	[PublicizedFrom(EAccessModifier.Private)]
	public AstarManager.Area AddArea(Vector2i pos, bool noNext)
	{
		pos.x &= -16;
		pos.y &= -16;
		AstarManager.Area area = this.FindArea(pos);
		if (area == null)
		{
			area = new AstarManager.Area();
			area.pos = pos;
			area.updateDelay = 2f;
			this.areaList.Add(area);
			return area;
		}
		if (noNext)
		{
			return area;
		}
		if (area.next != null)
		{
			return area.next;
		}
		if (area.updateDelay < 1.5f)
		{
			AstarManager.Area area2 = new AstarManager.Area();
			area2.pos = pos;
			area2.updateDelay = 2f - area.updateDelay;
			area.next = area2;
			return area2;
		}
		return area;
	}

	// Token: 0x06003B67 RID: 15207 RVA: 0x0017DF7C File Offset: 0x0017C17C
	[PublicizedFrom(EAccessModifier.Private)]
	public AstarManager.Area FindArea(Vector2i pos)
	{
		for (int i = 0; i < this.areaList.Count; i++)
		{
			AstarManager.Area area = this.areaList[i];
			if (area.pos == pos)
			{
				return area;
			}
		}
		return null;
	}

	// Token: 0x06003B68 RID: 15208 RVA: 0x0017DFC0 File Offset: 0x0017C1C0
	[PublicizedFrom(EAccessModifier.Private)]
	public AstarVoxelGrid AddGraph(int size)
	{
		AstarVoxelGrid astarVoxelGrid = this.astar.data.AddGraph(typeof(AstarVoxelGrid)) as AstarVoxelGrid;
		this.graphList.Add(astarVoxelGrid);
		astarVoxelGrid.Init();
		astarVoxelGrid.neighbours = NumNeighbours.Four;
		astarVoxelGrid.uniformEdgeCosts = false;
		astarVoxelGrid.inspectorGridMode = InspectorGridMode.Grid;
		astarVoxelGrid.characterHeight = 1.8f;
		astarVoxelGrid.SetDimensions(size, size, 1f);
		astarVoxelGrid.maxClimb = 1.3f;
		astarVoxelGrid.maxSlope = 60f;
		astarVoxelGrid.mergeSpanRange = 0.1f;
		GraphCollision collision = astarVoxelGrid.collision;
		collision.collisionCheck = true;
		collision.type = ColliderType.Capsule;
		collision.diameter = 0.3f;
		collision.height = 1.5f;
		collision.collisionOffset = 0.15f;
		collision.mask = 65536;
		return astarVoxelGrid;
	}

	// Token: 0x06003B69 RID: 15209 RVA: 0x0017E094 File Offset: 0x0017C294
	[PublicizedFrom(EAccessModifier.Private)]
	public AstarVoxelGrid FindClosestGraph(Vector2 pos, int size)
	{
		Vector2 vector = pos - this.worldOriginXZ;
		AstarVoxelGrid result = null;
		float num = float.MaxValue;
		for (int i = 0; i < this.graphList.Count; i++)
		{
			AstarVoxelGrid astarVoxelGrid = this.graphList[i];
			if (!astarVoxelGrid.IsUsed && astarVoxelGrid.size.x >= (float)size)
			{
				Vector2 a = vector;
				a.x -= astarVoxelGrid.center.x;
				a.y -= astarVoxelGrid.center.z;
				float num2 = Vector2.SqrMagnitude(a);
				if (num2 < num)
				{
					num = num2;
					result = astarVoxelGrid;
				}
			}
		}
		return result;
	}

	// Token: 0x04003008 RID: 12296
	public static AstarManager Instance;

	// Token: 0x04003009 RID: 12297
	public const float cGridHeight = 320f;

	// Token: 0x0400300A RID: 12298
	public const float cGridY = -32f;

	// Token: 0x0400300B RID: 12299
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cGridXZSize = 76;

	// Token: 0x0400300C RID: 12300
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cMoveDist = 10f;

	// Token: 0x0400300D RID: 12301
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cCharHeight = 1.8f;

	// Token: 0x0400300E RID: 12302
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cCharDiameter = 0.3f;

	// Token: 0x0400300F RID: 12303
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cLocationFindPer = 0.2f;

	// Token: 0x04003010 RID: 12304
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cLocationDuration = 4f;

	// Token: 0x04003011 RID: 12305
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cPlayerMergeDist = 19f;

	// Token: 0x04003012 RID: 12306
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cPlayerMergeDistSq = 361f;

	// Token: 0x04003013 RID: 12307
	public const float cUpdateDeltaTime = 0.1f;

	// Token: 0x04003014 RID: 12308
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AstarPath astar;

	// Token: 0x04003015 RID: 12309
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float lastWorkTime;

	// Token: 0x04003016 RID: 12310
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 worldOrigin;

	// Token: 0x04003017 RID: 12311
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 worldOriginXZ;

	// Token: 0x04003018 RID: 12312
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<AstarManager.Area> areaList = new List<AstarManager.Area>();

	// Token: 0x04003019 RID: 12313
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<AstarVoxelGrid> graphList = new List<AstarVoxelGrid>();

	// Token: 0x0400301A RID: 12314
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<AstarManager.Location> locations = new List<AstarManager.Location>();

	// Token: 0x0400301B RID: 12315
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<AstarManager.MergedLocations> mergedLocations = new List<AstarManager.MergedLocations>();

	// Token: 0x0400301C RID: 12316
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<AstarVoxelGrid> moveList = new List<AstarVoxelGrid>();

	// Token: 0x0400301D RID: 12317
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AstarVoxelGrid moveCurrent;

	// Token: 0x0400301E RID: 12318
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static int[] updateBlockOffsets = new int[]
	{
		-1,
		0,
		1,
		0,
		0,
		-1,
		0,
		1
	};

	// Token: 0x02000811 RID: 2065
	public class Area
	{
		// Token: 0x0400301F RID: 12319
		public AstarManager.Area next;

		// Token: 0x04003020 RID: 12320
		public Vector2i pos;

		// Token: 0x04003021 RID: 12321
		public AstarManager.Bounds2i bounds;

		// Token: 0x04003022 RID: 12322
		public bool hasBlocks;

		// Token: 0x04003023 RID: 12323
		public bool isPartial;

		// Token: 0x04003024 RID: 12324
		public bool isSlowUpdate;

		// Token: 0x04003025 RID: 12325
		public float updateDelay;
	}

	// Token: 0x02000812 RID: 2066
	public struct Bounds2i
	{
		// Token: 0x06003B6D RID: 15213 RVA: 0x0017E190 File Offset: 0x0017C390
		public bool Contains(Vector2i pos)
		{
			return pos.x >= this.min.x && pos.x <= this.max.x && pos.y >= this.min.y && pos.y <= this.max.y;
		}

		// Token: 0x06003B6E RID: 15214 RVA: 0x0017E1EC File Offset: 0x0017C3EC
		public void Encapsulate(Vector2i pos)
		{
			if (pos.x < this.min.x)
			{
				this.min.x = pos.x;
			}
			if (pos.x > this.max.x)
			{
				this.max.x = pos.x;
			}
			if (pos.y < this.min.y)
			{
				this.min.y = pos.y;
			}
			if (pos.y > this.max.y)
			{
				this.max.y = pos.y;
			}
		}

		// Token: 0x06003B6F RID: 15215 RVA: 0x0017E28C File Offset: 0x0017C48C
		public Bounds ToBounds()
		{
			Bounds result = default(Bounds);
			result.SetMinMax(new Vector3((float)this.min.x, 0f, (float)this.min.y), new Vector3((float)this.max.x + 0.999999f, 0f, (float)this.max.y + 0.999999f));
			return result;
		}

		// Token: 0x04003026 RID: 12326
		public Vector2i min;

		// Token: 0x04003027 RID: 12327
		public Vector2i max;
	}

	// Token: 0x02000813 RID: 2067
	[PublicizedFrom(EAccessModifier.Private)]
	public class Location
	{
		// Token: 0x04003028 RID: 12328
		public Vector2 pos;

		// Token: 0x04003029 RID: 12329
		public int size;

		// Token: 0x0400302A RID: 12330
		public float duration;
	}

	// Token: 0x02000814 RID: 2068
	[PublicizedFrom(EAccessModifier.Private)]
	public struct MergedLocations
	{
		// Token: 0x0400302B RID: 12331
		public Vector2 pos;

		// Token: 0x0400302C RID: 12332
		public int size;
	}
}
