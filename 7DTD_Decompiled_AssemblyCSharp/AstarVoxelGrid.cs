using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Pathfinding;
using UnityEngine;

// Token: 0x02000817 RID: 2071
public class AstarVoxelGrid : LayerGridGraph
{
	// Token: 0x06003B7D RID: 15229 RVA: 0x0017E650 File Offset: 0x0017C850
	public void Init()
	{
		if (AstarVoxelGrid.connectionsPool == null)
		{
			AstarVoxelGrid.connectionsPool = new List<Connection[]>[16];
			for (int i = 0; i < 16; i++)
			{
				AstarVoxelGrid.connectionsPool[i] = new List<Connection[]>();
			}
		}
		this.gridMover = new AstarVoxelGrid.ProceduralGridMover();
	}

	// Token: 0x06003B7E RID: 15230 RVA: 0x0017E694 File Offset: 0x0017C894
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitScan()
	{
		base.Scan();
	}

	// Token: 0x06003B7F RID: 15231 RVA: 0x0017E69C File Offset: 0x0017C89C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override IEnumerable<Progress> ScanInternal()
	{
		foreach (Progress progress in this.<>n__0())
		{
			yield return progress;
		}
		yield break;
	}

	// Token: 0x06003B80 RID: 15232 RVA: 0x0017E6AC File Offset: 0x0017C8AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void UpdateArea(GraphUpdateObject o)
	{
		IntRect intRect;
		IntRect a;
		IntRect a2;
		bool flag;
		int num;
		base.CalculateAffectedRegions(o, out intRect, out a, out a2, out flag, out num);
		IntRect b = new IntRect(0, 0, this.width - 1, this.depth - 1);
		IntRect intRect2 = IntRect.Intersection(a, b);
		this.collision.Initialize(base.transform, this.nodeSize);
		intRect2 = IntRect.Intersection(a2, b);
		for (int i = intRect2.xmin; i <= intRect2.xmax; i++)
		{
			for (int j = intRect2.ymin; j <= intRect2.ymax; j++)
			{
				this.RecalculateCell(i, j, true, false);
			}
		}
		a.Expand(1);
		intRect2 = IntRect.Intersection(a, b);
		for (int k = intRect2.xmin; k <= intRect2.xmax; k++)
		{
			for (int l = intRect2.ymin; l <= intRect2.ymax; l++)
			{
				this.CalculateConnections(k, l);
			}
		}
	}

	// Token: 0x06003B81 RID: 15233 RVA: 0x00002914 File Offset: 0x00000B14
	[Conditional("DEBUG_PATHGRIDVALIDATE")]
	[PublicizedFrom(EAccessModifier.Private)]
	public void Validate()
	{
	}

	// Token: 0x06003B82 RID: 15234 RVA: 0x0017E7A8 File Offset: 0x0017C9A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SampleHeights(Vector3 pos)
	{
		this.CheckHeights(pos);
		int num = this.heightsUsed / 2;
		for (int i = 0; i < num; i++)
		{
			AstarVoxelGrid.HitData hitData = this.heights[i];
			this.heights[i] = this.heights[this.heightsUsed - 1 - i];
			this.heights[this.heightsUsed - 1 - i] = hitData;
		}
	}

	// Token: 0x06003B83 RID: 15235 RVA: 0x0017E818 File Offset: 0x0017CA18
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckHeights(Vector3 position)
	{
		this.heightsUsed = 0;
		int num = 0;
		Vector3 vector = position;
		vector.y += 320f;
		PhysicsScene defaultPhysicsScene = Physics.defaultPhysicsScene;
		Vector3 down = Vector3.down;
		AstarVoxelGrid.HitData hitData;
		hitData.blockerFlags = 4096;
		RaycastHit raycastHit;
		while (defaultPhysicsScene.Raycast(vector, down, out raycastHit, 320.01f, 1073807360, QueryTriggerInteraction.Ignore))
		{
			vector.y = raycastHit.point.y - 0.11f;
			hitData.point = raycastHit.point;
			hitData.point.y = hitData.point.y + 0.05f;
			AstarVoxelGrid.cellHits[num] = hitData;
			if (++num >= 512)
			{
				Log.Warning("AstarVoxelGrid CheckHeights too many hits");
				break;
			}
		}
		World world = GameManager.Instance.World;
		ChunkCluster chunkCache = world.ChunkCache;
		Vector3 position2 = Origin.position;
		int type = BlockValue.Air.type;
		int num2 = Utils.Fastfloor(position.x + position2.x);
		int num3 = Utils.Fastfloor(position.z + position2.z);
		Vector3i vector3i = new Vector3i(num2, 0, num3);
		Vector3i vector3i2 = vector3i;
		IChunk chunkFromWorldPos = world.GetChunkFromWorldPos(vector3i);
		if (chunkFromWorldPos == null)
		{
			return;
		}
		int x = World.toBlockXZ(num2);
		int z = World.toBlockXZ(num3);
		int i = 0;
		float num4 = 257f;
		IL_7A7:
		while (i < num)
		{
			float num5 = num4;
			hitData = AstarVoxelGrid.cellHits[i++];
			num4 = hitData.point.y;
			float num6 = num5 - num4;
			vector3i.y = Utils.Fastfloor(num4 + position2.y);
			BlockValue block = this.GetBlock(chunkFromWorldPos, x, vector3i.y, z);
			int type2 = block.type;
			Block block2 = block.Block;
			if (block2.shape.IsTerrain())
			{
				AstarVoxelGrid.HitData[] array = this.heights;
				int num7 = this.heightsUsed;
				this.heightsUsed = num7 + 1;
				array[num7] = hitData;
			}
			else
			{
				if (num6 > 0.95f)
				{
					if (block2.PathType > 0)
					{
						float num8 = (float)Utils.Fastfloor(hitData.point.y);
						if (hitData.point.y - num8 > 0.4f)
						{
							vector3i.y++;
							block = this.GetBlock(chunkFromWorldPos, x, vector3i.y, z);
							type2 = block.type;
							block2 = block.Block;
							num4 = num8 + 1.01f;
							hitData.point.y = num4;
						}
					}
					int num7;
					if (block2.PathType > 0)
					{
						hitData.blockerFlags = 4111;
					}
					else
					{
						if (type2 != type)
						{
							hitData.blockerFlags |= this.CalcBlockingFlags(hitData.point, 0.2f);
							Vector2 pathOffset = block2.GetPathOffset((int)block.rotation);
							hitData.point.x = hitData.point.x + pathOffset.x;
							hitData.point.z = hitData.point.z + pathOffset.y;
						}
						vector3i2.y = vector3i.y + 1;
						BlockValue block3 = this.GetBlock(chunkFromWorldPos, x, vector3i2.y, z);
						Block block4 = block3.Block;
						if (block2.HasTag(BlockTags.Door) || block2.HasTag(BlockTags.ClosetDoor))
						{
							if (!block2.isMultiBlock || !block.ischild || block.parenty == 0)
							{
								hitData.blockerFlags |= 16384;
							}
						}
						else if (block4.HasTag(BlockTags.Door) && (!block4.isMultiBlock || !block3.ischild || block3.parenty == 0))
						{
							hitData.blockerFlags |= 16384;
						}
						if (num6 > 2.95f && (block2.IsElevator((int)block.rotation) || block4.IsElevator((int)block3.rotation)))
						{
							hitData.blockerFlags |= 8192;
							Vector3i vector3i3 = vector3i2;
							BlockValue blockValue = block3;
							Block block5 = block4;
							int num9 = (int)(num5 - 1f + position2.y);
							int num10 = 0;
							while (vector3i3.y <= num9)
							{
								if (block5.IsElevator((int)blockValue.rotation))
								{
									num10 = 0;
								}
								else
								{
									if (!blockValue.isair || num10 >= 1)
									{
										break;
									}
									num10++;
								}
								vector3i3.y++;
								blockValue = this.GetBlock(chunkFromWorldPos, x, vector3i3.y, z);
								block5 = blockValue.Block;
							}
							vector3i3.y -= num10;
							Vector3 vector2 = vector;
							float num11 = num4 + position2.y - -0.2f;
							while ((float)vector3i3.y > num11)
							{
								vector2.y = (float)vector3i3.y - position2.y;
								AstarVoxelGrid.HitData hitData2;
								hitData2.blockerFlags = (8192 | this.CalcBlockingFlags(vector2, 0f));
								hitData2.point.x = vector2.x;
								hitData2.point.z = vector2.z;
								hitData2.point.y = vector2.y + -0.2f;
								AstarVoxelGrid.HitData[] array2 = this.heights;
								num7 = this.heightsUsed;
								this.heightsUsed = num7 + 1;
								array2[num7] = hitData2;
								vector3i3.y--;
							}
						}
					}
					AstarVoxelGrid.HitData[] array3 = this.heights;
					num7 = this.heightsUsed;
					this.heightsUsed = num7 + 1;
					array3[num7] = hitData;
				}
				else
				{
					num4 = num5;
				}
				float num12 = float.MinValue;
				if (i < num)
				{
					num12 = AstarVoxelGrid.cellHits[i].point.y;
				}
				for (;;)
				{
					vector3i.y--;
					vector.y = (float)vector3i.y - position2.y;
					if (vector.y <= num12)
					{
						goto IL_7A7;
					}
					if (vector3i.y < 0)
					{
						break;
					}
					block = this.GetBlock(chunkFromWorldPos, x, vector3i.y, z);
					type2 = block.type;
					if (type2 == type)
					{
						goto IL_7A7;
					}
					block2 = block.Block;
					if (block2.shape.IsTerrain() || block2.IsElevator())
					{
						goto IL_7A7;
					}
					if (!block2.HasTag(BlockTags.Door) && block2.PathType > 0 && block2.IsMovementBlocked(world, vector3i, block, BlockFace.Top))
					{
						bool flag = true;
						for (int j = 0; j < 4; j++)
						{
							Vector2i vector2i = AstarVoxelGrid.neighboursOffsetV2[j];
							Vector3i vector3i4 = vector3i;
							vector3i4.x += vector2i.x;
							vector3i4.z += vector2i.y;
							block = chunkCache.GetBlock(vector3i4);
							block2 = block.Block;
							if (block2.PathType <= 0 || !block2.IsMovementBlocked(world, vector3i4, block, BlockFace.Top))
							{
								vector3i4.y--;
								block = chunkCache.GetBlock(vector3i4);
								block2 = block.Block;
								if (block2.PathType > 0)
								{
									flag = false;
								}
								else if (block2.IsMovementBlocked(world, vector3i4, block, BlockFace.Top))
								{
									Vector3 origin;
									origin.x = (float)vector3i4.x - position2.x;
									origin.y = vector.y + 0.51f;
									origin.z = (float)vector3i4.z - position2.z;
									RaycastHit raycastHit2;
									if (defaultPhysicsScene.Raycast(origin, down, out raycastHit2, 1.6f, 1073807360, QueryTriggerInteraction.Ignore))
									{
										flag = false;
										break;
									}
									break;
								}
							}
						}
						if (!flag)
						{
							hitData.point.y = vector.y + 0.03f;
							hitData.blockerFlags = 4111;
							AstarVoxelGrid.HitData[] array4 = this.heights;
							int num7 = this.heightsUsed;
							this.heightsUsed = num7 + 1;
							array4[num7] = hitData;
						}
					}
				}
				i = int.MaxValue;
			}
		}
	}

	// Token: 0x06003B84 RID: 15236 RVA: 0x0017EFD4 File Offset: 0x0017D1D4
	public override void RecalculateCell(int x, int z, bool resetPenalties = true, bool resetTags = true)
	{
		World world = GameManager.Instance.World;
		if (world == null)
		{
			return;
		}
		if (world.ChunkCache == null)
		{
			return;
		}
		Vector3 vector = base.transform.Transform(new Vector3((float)x + 0.5f, 0f, (float)z + 0.5f));
		this.SampleHeights(vector);
		if (this.heightsUsed > this.layerCount)
		{
			if (this.heightsUsed > 255)
			{
				UnityEngine.Debug.LogError("Too many layers " + this.heightsUsed.ToString());
				return;
			}
			base.AddLayers(this.heightsUsed - this.layerCount);
		}
		Vector3 position = Origin.position;
		int num = Utils.Fastfloor(vector.x + position.x);
		int num2 = Utils.Fastfloor(vector.z + position.z);
		Vector3i vector3i = new Vector3i(num, 0, num2);
		IChunk chunkFromWorldPos = world.GetChunkFromWorldPos(vector3i);
		if (chunkFromWorldPos == null)
		{
			return;
		}
		int x2 = World.toBlockXZ(num);
		int z2 = World.toBlockXZ(num2);
		int num3 = this.width * this.depth;
		int num4 = x + z * this.width;
		int i;
		for (i = 0; i < this.heightsUsed; i++)
		{
			int num5 = num4 + num3 * i;
			AstarVoxelGrid.VoxelNode voxelNode = (AstarVoxelGrid.VoxelNode)this.nodes[num5];
			if (voxelNode == null)
			{
				voxelNode = (this.nodes[num5] = AstarVoxelGrid.levelGridNodePool.Alloc(false));
				voxelNode.Init(this.active);
				voxelNode.NodeInGridIndex = num4;
				voxelNode.LayerCoordinateInGrid = i;
				voxelNode.GraphIndex = this.graphIndex;
			}
			Vector3 point = this.heights[i].point;
			voxelNode.position = (Int3)point;
			vector3i.y = Utils.Fastfloor(point.y + position.y);
			voxelNode.ClearCustomConnections(true);
			voxelNode.Walkable = true;
			int num6 = 0;
			int num7 = 0;
			voxelNode.PenaltyHigh = 0;
			voxelNode.PenaltyLow = 0;
			voxelNode.BlockerFlags = this.heights[i].blockerFlags;
			if ((voxelNode.BlockerFlags & 16384) > 0)
			{
				voxelNode.Tag = 3U;
				voxelNode.Penalty = (uint)num6;
				BlockValue block = this.GetBlock(chunkFromWorldPos, x2, vector3i.y, z2);
				int num8 = block.Block.MaxDamagePlusDowngrades - block.damage;
				voxelNode.PenaltyLow = (num8 + 10) * 20 / 3;
			}
			else
			{
				vector3i.y++;
				BlockValue block2;
				Block block3;
				if (point.y - Mathf.Floor(point.y) > 0.4f)
				{
					vector3i.y++;
					block2 = this.GetBlock(chunkFromWorldPos, x2, vector3i.y, z2);
					block3 = block2.Block;
					if (block3.IsMovementBlocked(world, vector3i, block2, BlockFace.None))
					{
						int num9 = block3.MaxDamagePlusDowngrades - block2.damage;
						voxelNode.PenaltyHigh = (num9 + 10) * 20;
						if (block3.PathType > 0)
						{
							num6 += voxelNode.PenaltyHigh;
							AstarVoxelGrid.VoxelNode voxelNode2 = voxelNode;
							voxelNode2.BlockerFlags |= 240;
						}
						else
						{
							int num10 = (int)this.CalcBlockingFlags(point, 1.5f);
							AstarVoxelGrid.VoxelNode voxelNode3 = voxelNode;
							voxelNode3.BlockerFlags |= (ushort)((num10 & 15) << 4);
						}
					}
					vector3i.y--;
				}
				block2 = this.GetBlock(chunkFromWorldPos, x2, vector3i.y, z2);
				block3 = block2.Block;
				bool flag = false;
				if (block3.IsMovementBlocked(world, vector3i, block2, BlockFace.None))
				{
					int num11 = block3.MaxDamagePlusDowngrades - block2.damage;
					voxelNode.PenaltyHigh += (num11 + 10) * 20;
					if (block3.PathType > 0)
					{
						num6 += voxelNode.PenaltyHigh;
						AstarVoxelGrid.VoxelNode voxelNode4 = voxelNode;
						voxelNode4.BlockerFlags |= 240;
					}
					else
					{
						bool flag2 = false;
						int num12 = i + 1;
						if (num12 < this.heightsUsed && Utils.Fastfloor(this.heights[num12].point.y + position.y) == vector3i.y)
						{
							flag2 = true;
							int blockerFlags = (int)this.heights[num12].blockerFlags;
							if ((blockerFlags & 4096) > 0)
							{
								num6 += voxelNode.PenaltyHigh;
								AstarVoxelGrid.VoxelNode voxelNode5 = voxelNode;
								voxelNode5.BlockerFlags |= 240;
							}
							else
							{
								AstarVoxelGrid.VoxelNode voxelNode6 = voxelNode;
								voxelNode6.BlockerFlags |= (ushort)(((blockerFlags >> 8 | blockerFlags) & 15) << 4);
							}
						}
						if (!flag2)
						{
							int num13 = (int)this.CalcBlockingFlags(point, 1f);
							AstarVoxelGrid.VoxelNode voxelNode7 = voxelNode;
							voxelNode7.BlockerFlags |= (ushort)((num13 & 15) << 4);
						}
					}
				}
				vector3i.y--;
				block2 = this.GetBlock(chunkFromWorldPos, x2, vector3i.y, z2);
				block3 = block2.Block;
				if (block3.IsMovementBlocked(world, vector3i, block2, BlockFace.None))
				{
					int num14 = block3.MaxDamagePlusDowngrades - block2.damage;
					voxelNode.PenaltyLow = (num14 + 10) * 20;
					if (block3.PathType > 0)
					{
						num6 += voxelNode.PenaltyLow;
					}
				}
				if (num7 > 0)
				{
					voxelNode.Tag = 1U;
				}
				else if (flag)
				{
					voxelNode.Tag = 2U;
				}
				else
				{
					voxelNode.Tag = 0U;
				}
				if (block3.IsElevator((int)block2.rotation))
				{
					voxelNode.Tag = 4U;
				}
				if (num6 > 268435455)
				{
					Log.Warning("RecalculateCell {0}, id{1} {2}, pen {3}", new object[]
					{
						vector3i,
						block3.blockID,
						block3.GetBlockName(),
						num6
					});
					if (num6 < 0)
					{
						num6 = 0;
					}
					else
					{
						num6 = 268435455;
					}
				}
				voxelNode.Penalty = (uint)num6;
			}
		}
		int num15 = num4 + num3 * i;
		while (i < this.layerCount)
		{
			LevelGridNode levelGridNode = this.nodes[num15];
			if (levelGridNode != null)
			{
				levelGridNode.Destroy();
				this.nodes[num15] = null;
				AstarVoxelGrid.levelGridNodePool.Free((AstarVoxelGrid.VoxelNode)levelGridNode);
			}
			num15 += num3;
			i++;
		}
	}

	// Token: 0x06003B85 RID: 15237 RVA: 0x0017F5E0 File Offset: 0x0017D7E0
	[PublicizedFrom(EAccessModifier.Private)]
	public ushort CalcBlockingFlags(Vector3 pos, float offsetY)
	{
		PhysicsScene defaultPhysicsScene = Physics.defaultPhysicsScene;
		int num = 0;
		pos.y += 0.2f + offsetY;
		Vector3 vector;
		vector.y = 0f;
		for (int i = 0; i < 4; i++)
		{
			Vector2i vector2i = AstarVoxelGrid.neighboursOffsetV2[i];
			vector.x = (float)vector2i.x;
			vector.z = (float)vector2i.y;
			Vector3 origin = pos - vector * 0.2f;
			RaycastHit raycastHit;
			if (defaultPhysicsScene.SphereCast(origin, 0.1f, vector, out raycastHit, 0.59f, 1073807360, QueryTriggerInteraction.Ignore))
			{
				if (offsetY > 0.5f || raycastHit.normal.y < 0.643f)
				{
					num |= 1 << i;
				}
				else if (Vector3.Dot(vector, raycastHit.normal) > -0.35f)
				{
					num |= 1 << i;
				}
				else
				{
					num |= 256 << i;
				}
			}
		}
		return (ushort)num;
	}

	// Token: 0x06003B86 RID: 15238 RVA: 0x0017F6D8 File Offset: 0x0017D8D8
	public override void CalculateConnections(int x, int z, int layerIndex)
	{
		int num = this.width * this.depth;
		int num2 = z * this.width + x + num * layerIndex;
		AstarVoxelGrid.VoxelNode voxelNode = (AstarVoxelGrid.VoxelNode)this.nodes[num2];
		if (voxelNode == null)
		{
			return;
		}
		voxelNode.ResetAllGridConnections();
		if (!voxelNode.Walkable)
		{
			return;
		}
		float num3 = (float)voxelNode.position.y * 0.001f;
		float num4 = num3 + this.characterHeight;
		((Vector3)voxelNode.position).y += 0.5f;
		if ((voxelNode.BlockerFlags & 8192) > 0 && layerIndex + 1 < this.layerCount)
		{
			LevelGridNode levelGridNode = this.nodes[num2 + num];
			if (levelGridNode != null && (float)levelGridNode.position.y * 0.001f - num3 < 2.1f)
			{
				this.AddConnection(voxelNode, levelGridNode, 500U);
				this.AddConnection(levelGridNode, voxelNode, 250U);
			}
		}
		for (int i = 0; i < 4; i++)
		{
			Vector2i vector2i = AstarVoxelGrid.neighboursOffsetV2[i];
			int num5 = x + vector2i.x;
			if ((ulong)num5 < (ulong)((long)this.width))
			{
				int num6 = z + vector2i.y;
				if ((ulong)num6 < (ulong)((long)this.depth))
				{
					int num7 = num6 * this.width + num5;
					int num8 = 255;
					float num9 = 0f;
					for (int j = 0; j < this.layerCount; j++)
					{
						int num10 = num7 + j * num;
						AstarVoxelGrid.VoxelNode voxelNode2 = (AstarVoxelGrid.VoxelNode)this.nodes[num10];
						if (voxelNode2 != null && voxelNode2.Walkable)
						{
							float num11 = (float)voxelNode2.position.y * 0.001f;
							float num12;
							if (j == this.layerCount - 1 || this.nodes[num10 + num] == null)
							{
								num12 = float.PositiveInfinity;
							}
							else
							{
								num12 = (float)this.nodes[num10 + num].position.y * 0.001f - num11;
								if (num12 <= -0.001f)
								{
									LevelGridNode levelGridNode2 = this.nodes[num10 + num];
									Utils.DrawLine((Vector3)voxelNode.position, (Vector3)voxelNode2.position, new Color(1f, 0f, 1f), new Color(1f, 0.5f, 0f), 3, 5f);
									Utils.DrawLine((Vector3)voxelNode2.position, (Vector3)levelGridNode2.position, new Color(1f, 0f, 0f), new Color(1f, 1f, 0f), 2, 5f);
									Log.Warning("Path node otherHeight bad {0}, {1}, {2}", new object[]
									{
										num12,
										levelGridNode2.position,
										voxelNode2.position
									});
								}
							}
							float num13 = num11 - num3;
							if (num13 < -0.1f)
							{
								if (num13 >= -9.4f && num4 < num11 + num12)
								{
									num8 = j;
									num9 = num13;
								}
							}
							else
							{
								if (num13 >= 1.51f)
								{
									break;
								}
								if (num12 >= 0.7f)
								{
									if (num13 >= 0.6f)
									{
										if ((voxelNode.BlockerFlags & 15) != 15 && ((int)voxelNode.BlockerFlags & 16 << i) == 0 && ((int)voxelNode2.BlockerFlags & 17 << (i ^ 2)) == 0)
										{
											if (num13 >= 1.05f || ((int)voxelNode2.BlockerFlags & 256 << i) == 0)
											{
												if (((int)voxelNode2.BlockerFlags & 256 << (i ^ 2)) == 0)
												{
													this.AddConnection(voxelNode, voxelNode2, (uint)(num13 * 8000f));
													this.AddDummyConnection(voxelNode2, voxelNode);
													if ((voxelNode.BlockerFlags & 8192) == 0)
													{
														num8 = 255;
														break;
													}
													break;
												}
											}
											else
											{
												num8 = j;
												num9 = 0f;
											}
										}
									}
									else if ((voxelNode2.BlockerFlags & 12288) > 0)
									{
										num8 = j;
										num9 = 0f;
									}
								}
							}
						}
					}
					if (num8 != 255)
					{
						int num14 = num7 + num8 * num;
						AstarVoxelGrid.VoxelNode voxelNode3 = (AstarVoxelGrid.VoxelNode)this.nodes[num14];
						bool flag = false;
						int num15 = (int)(voxelNode.BlockerFlags & 15);
						if (num15 == 0 || num15 == 15)
						{
							num15 = (int)(voxelNode.BlockerFlags & 240);
							if (num15 == 0 || num15 == 240)
							{
								num15 = (int)(voxelNode3.BlockerFlags & 15);
								if (num9 <= -0.95f || num15 == 0 || num15 == 15)
								{
									num15 = (int)(voxelNode3.BlockerFlags & 240);
									if (num15 == 0 || num15 == 240)
									{
										flag = true;
									}
								}
							}
						}
						if (!flag)
						{
							int num16 = 0;
							if (((int)voxelNode.BlockerFlags & 16 << i) > 0)
							{
								num16 += voxelNode.PenaltyHigh;
							}
							if (((int)voxelNode3.BlockerFlags & 16 << (i ^ 2)) > 0)
							{
								num16 += voxelNode3.PenaltyHigh;
							}
							if (((int)voxelNode.BlockerFlags & 1 << i) > 0)
							{
								num16 += voxelNode.PenaltyLow;
							}
							if (((int)voxelNode3.BlockerFlags & 1 << (i ^ 2)) > 0 && num9 > -0.95f)
							{
								num16 += voxelNode3.PenaltyLow;
							}
							if (num16 > 0)
							{
								this.AddConnection(voxelNode, voxelNode3, (uint)num16);
								this.AddDummyConnection(voxelNode3, voxelNode);
								num8 = 255;
							}
						}
						voxelNode.SetConnectionValue(i, num8);
					}
				}
			}
		}
	}

	// Token: 0x06003B87 RID: 15239 RVA: 0x0017FC2C File Offset: 0x0017DE2C
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddConnection(GridNodeBase node, GridNodeBase other, uint cost)
	{
		Connection[] connections = node.connections;
		int num = 0;
		if (connections != null)
		{
			for (int i = 0; i < connections.Length; i++)
			{
				if (connections[i].node == other)
				{
					connections[i].cost = cost;
					return;
				}
			}
			num = connections.Length;
		}
		num++;
		Connection[] array = AstarVoxelGrid.AllocConnection(num);
		for (int j = 0; j < num - 1; j++)
		{
			array[j] = connections[j];
		}
		array[num - 1] = new Connection(other, cost, byte.MaxValue);
		node.connections = array;
	}

	// Token: 0x06003B88 RID: 15240 RVA: 0x0017FCC0 File Offset: 0x0017DEC0
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddDummyConnection(GridNodeBase node, GridNodeBase other)
	{
		Connection[] connections = node.connections;
		if (connections != null)
		{
			for (int i = 0; i < connections.Length; i++)
			{
				if (connections[i].node == other)
				{
					return;
				}
			}
		}
		this.AddConnection(node, other, uint.MaxValue);
	}

	// Token: 0x06003B89 RID: 15241 RVA: 0x0017FD00 File Offset: 0x0017DF00
	public static void ClearConnections(GridNodeBase node)
	{
		Connection[] connections = node.connections;
		if (connections != null)
		{
			node.connections = null;
			int num = connections.Length;
			for (int i = 0; i < num; i++)
			{
				AstarVoxelGrid.RemoveConnection((GridNodeBase)connections[i].node, node);
			}
			if (num < 16)
			{
				AstarVoxelGrid.connectionsPool[num].Add(connections);
			}
		}
	}

	// Token: 0x06003B8A RID: 15242 RVA: 0x0017FD58 File Offset: 0x0017DF58
	[PublicizedFrom(EAccessModifier.Private)]
	public static void RemoveConnection(GridNodeBase node, GridNodeBase other)
	{
		Connection[] connections = node.connections;
		if (connections != null)
		{
			int num = connections.Length;
			int i = 0;
			while (i < num)
			{
				if (connections[i].node == other)
				{
					if (num <= 1)
					{
						node.connections = null;
					}
					else
					{
						Connection[] array = AstarVoxelGrid.AllocConnection(num - 1);
						int j;
						for (j = 0; j < i; j++)
						{
							array[j] = connections[j];
						}
						while (j < array.Length)
						{
							array[j] = connections[j + 1];
							j++;
						}
						node.connections = array;
					}
					if (num < 16)
					{
						AstarVoxelGrid.connectionsPool[num].Add(connections);
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
	}

	// Token: 0x06003B8B RID: 15243 RVA: 0x0017FE08 File Offset: 0x0017E008
	[PublicizedFrom(EAccessModifier.Private)]
	public static Connection[] AllocConnection(int count)
	{
		Connection[] array = null;
		if (count < 16)
		{
			List<Connection[]> list = AstarVoxelGrid.connectionsPool[count];
			int count2 = list.Count;
			if (count2 > 0)
			{
				array = list[count2 - 1];
				list.RemoveAt(count2 - 1);
			}
		}
		if (array == null)
		{
			array = new Connection[count];
		}
		return array;
	}

	// Token: 0x06003B8C RID: 15244 RVA: 0x0017FE4D File Offset: 0x0017E04D
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockValue GetBlock(IChunk chunk, int _x, int _y, int _z)
	{
		if (_y >= 256)
		{
			return BlockValue.Air;
		}
		return chunk.GetBlock(_x, _y, _z);
	}

	// Token: 0x06003B8D RID: 15245 RVA: 0x0017FE67 File Offset: 0x0017E067
	public void SetPos(Vector3 pos)
	{
		this.center = pos;
		this.IsFullUpdateNeeded = false;
		this.InitScan();
	}

	// Token: 0x06003B8E RID: 15246 RVA: 0x0017FE7D File Offset: 0x0017E07D
	public void Move(Vector3 targetPos)
	{
		this.gridMover.graph = this;
		this.gridMover.targetPosition = targetPos;
		this.gridMover.UpdateGraph();
	}

	// Token: 0x06003B8F RID: 15247 RVA: 0x0017FEA2 File Offset: 0x0017E0A2
	public bool IsMoving()
	{
		return this.gridMover.updatingGraph;
	}

	// Token: 0x04003034 RID: 12340
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cGridHeight = 320f;

	// Token: 0x04003035 RID: 12341
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cGridHeightPadded = 320.01f;

	// Token: 0x04003036 RID: 12342
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cCollisionMask = 1073807360;

	// Token: 0x04003037 RID: 12343
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cLayerMinHeight = 0.7f;

	// Token: 0x04003038 RID: 12344
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cClimbMinHeight = 0.6f;

	// Token: 0x04003039 RID: 12345
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cClimbMaxHeight = 1.51f;

	// Token: 0x0400303A RID: 12346
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDropOnTopHeight = 0.95f;

	// Token: 0x0400303B RID: 12347
	[PublicizedFrom(EAccessModifier.Private)]
	public const float cDropMaxHeight = 9.4f;

	// Token: 0x0400303C RID: 12348
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cJumpPenalty = 8;

	// Token: 0x0400303D RID: 12349
	public const int cPenaltyPerMeter = 1000;

	// Token: 0x0400303E RID: 12350
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cPenaltyHealthBase = 10;

	// Token: 0x0400303F RID: 12351
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cPenaltyHealthScale = 20;

	// Token: 0x04003040 RID: 12352
	public const uint cDummyPenalty = 4294967295U;

	// Token: 0x04003041 RID: 12353
	public const int cTagOpen = 0;

	// Token: 0x04003042 RID: 12354
	public const int cTagBreak = 1;

	// Token: 0x04003043 RID: 12355
	public const int cTagLowHeight = 2;

	// Token: 0x04003044 RID: 12356
	public const int cTagDoor = 3;

	// Token: 0x04003045 RID: 12357
	public const int cTagLadder = 4;

	// Token: 0x04003046 RID: 12358
	public const int cTagTest = 8;

	// Token: 0x04003047 RID: 12359
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cBlockerFlagLow0 = 1;

	// Token: 0x04003048 RID: 12360
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cBlockerFlagLow = 15;

	// Token: 0x04003049 RID: 12361
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cBlockerFlagHigh0 = 16;

	// Token: 0x0400304A RID: 12362
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cBlockerFlagHigh = 240;

	// Token: 0x0400304B RID: 12363
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cBlockerFlagHighLow0 = 17;

	// Token: 0x0400304C RID: 12364
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cBlockerFlagHighLow = 255;

	// Token: 0x0400304D RID: 12365
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cBlockerFlagSlopeDir0 = 256;

	// Token: 0x0400304E RID: 12366
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cBlockerFlagFloor = 4096;

	// Token: 0x0400304F RID: 12367
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cBlockerFlagLadder = 8192;

	// Token: 0x04003050 RID: 12368
	[PublicizedFrom(EAccessModifier.Private)]
	public const ushort cBlockerFlagDoor = 16384;

	// Token: 0x04003051 RID: 12369
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Vector2i[] neighboursOffsetV2 = new Vector2i[]
	{
		new Vector2i(0, -1),
		new Vector2i(1, 0),
		new Vector2i(0, 1),
		new Vector2i(-1, 0)
	};

	// Token: 0x04003052 RID: 12370
	public bool IsUsed;

	// Token: 0x04003053 RID: 12371
	public bool IsFullUpdateNeeded;

	// Token: 0x04003054 RID: 12372
	[PublicizedFrom(EAccessModifier.Private)]
	public AstarVoxelGrid.ProceduralGridMover gridMover;

	// Token: 0x04003055 RID: 12373
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cRayHitsMax = 512;

	// Token: 0x04003056 RID: 12374
	[PublicizedFrom(EAccessModifier.Private)]
	public static AstarVoxelGrid.HitData[] cellHits = new AstarVoxelGrid.HitData[512];

	// Token: 0x04003057 RID: 12375
	[PublicizedFrom(EAccessModifier.Private)]
	public AstarVoxelGrid.HitData[] heights = new AstarVoxelGrid.HitData[512];

	// Token: 0x04003058 RID: 12376
	[PublicizedFrom(EAccessModifier.Private)]
	public int heightsUsed;

	// Token: 0x04003059 RID: 12377
	[PublicizedFrom(EAccessModifier.Private)]
	public static MemoryPooledObject<AstarVoxelGrid.VoxelNode> levelGridNodePool = new MemoryPooledObject<AstarVoxelGrid.VoxelNode>(100000);

	// Token: 0x0400305A RID: 12378
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cConnectionPoolMax = 16;

	// Token: 0x0400305B RID: 12379
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Connection[]>[] connectionsPool;

	// Token: 0x0400305C RID: 12380
	public Vector2 GridMovePendingPos;

	// Token: 0x02000818 RID: 2072
	public struct HitData
	{
		// Token: 0x0400305D RID: 12381
		public Vector3 point;

		// Token: 0x0400305E RID: 12382
		public ushort blockerFlags;
	}

	// Token: 0x02000819 RID: 2073
	public class VoxelNode : LevelGridNode, IMemoryPoolableObject
	{
		// Token: 0x06003B93 RID: 15251 RVA: 0x00002914 File Offset: 0x00000B14
		public void Reset()
		{
		}

		// Token: 0x06003B94 RID: 15252 RVA: 0x00002914 File Offset: 0x00000B14
		public void Cleanup()
		{
		}

		// Token: 0x06003B95 RID: 15253 RVA: 0x0017FF3E File Offset: 0x0017E13E
		public override void ClearCustomConnections(bool alsoReverse)
		{
			AstarVoxelGrid.ClearConnections(this);
		}

		// Token: 0x06003B96 RID: 15254 RVA: 0x0017FF48 File Offset: 0x0017E148
		public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
		{
			handler.heap.Add(pathNode);
			pathNode.UpdateG(path);
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			LevelGridNode[] nodes = gridGraph.nodes;
			int nodeInGridIndex = base.NodeInGridIndex;
			for (int i = 0; i < 4; i++)
			{
				int connectionValue = base.GetConnectionValue(i);
				if (connectionValue != 255)
				{
					LevelGridNode levelGridNode = nodes[nodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue];
					PathNode pathNode2 = handler.GetPathNode(levelGridNode);
					if (pathNode2 != null && pathNode2.parent == pathNode && pathNode2.pathID == handler.PathID)
					{
						levelGridNode.UpdateRecursiveG(path, pathNode2, handler);
					}
				}
			}
			if (this.connections != null)
			{
				ushort pathID = handler.PathID;
				for (int j = 0; j < this.connections.Length; j++)
				{
					if (this.connections[j].cost != 4294967295U)
					{
						GraphNode node = this.connections[j].node;
						PathNode pathNode3 = handler.GetPathNode(node);
						if (pathNode3.parent == pathNode && pathNode3.pathID == pathID)
						{
							node.UpdateRecursiveG(path, pathNode3, handler);
						}
					}
				}
			}
		}

		// Token: 0x0400305F RID: 12383
		public int PenaltyHigh;

		// Token: 0x04003060 RID: 12384
		public int PenaltyLow;

		// Token: 0x04003061 RID: 12385
		public ushort BlockerFlags;
	}

	// Token: 0x0200081A RID: 2074
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_procedural_grid_mover.php")]
	public class ProceduralGridMover
	{
		// Token: 0x1700060B RID: 1547
		// (get) Token: 0x06003B98 RID: 15256 RVA: 0x0018007B File Offset: 0x0017E27B
		// (set) Token: 0x06003B99 RID: 15257 RVA: 0x00180083 File Offset: 0x0017E283
		public bool updatingGraph { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x06003B9A RID: 15258 RVA: 0x0018008C File Offset: 0x0017E28C
		[PublicizedFrom(EAccessModifier.Private)]
		public Vector3 PointToGraphSpace(Vector3 p)
		{
			return this.graph.transform.InverseTransform(p);
		}

		// Token: 0x06003B9B RID: 15259 RVA: 0x001800A0 File Offset: 0x0017E2A0
		public void UpdateGraph()
		{
			if (this.updatingGraph)
			{
				return;
			}
			this.updatingGraph = true;
			IEnumerator ie = this.UpdateGraphCoroutine();
			AstarPath.active.AddWorkItem(new AstarWorkItem(delegate(IWorkItemContext context, bool force)
			{
				if (force)
				{
					while (ie.MoveNext())
					{
					}
				}
				bool flag;
				try
				{
					flag = !ie.MoveNext();
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
					flag = true;
				}
				if (flag)
				{
					this.updatingGraph = false;
				}
				return flag;
			}));
		}

		// Token: 0x06003B9C RID: 15260 RVA: 0x001800F1 File Offset: 0x0017E2F1
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator UpdateGraphCoroutine()
		{
			Vector3 vector = this.PointToGraphSpace(this.targetPosition) - this.PointToGraphSpace(this.graph.center);
			vector.x = Mathf.Round(vector.x);
			vector.z = Mathf.Round(vector.z);
			vector.y = 0f;
			if (vector == Vector3.zero)
			{
				yield break;
			}
			Int2 offset = new Int2(-Mathf.RoundToInt(vector.x), -Mathf.RoundToInt(vector.z));
			this.graph.center = this.targetPosition;
			this.graph.UpdateTransform();
			int width = this.graph.width;
			int depth = this.graph.depth;
			int layers = this.graph.LayerCount;
			LayerGridGraph layerGridGraph = this.graph as LayerGridGraph;
			GridNodeBase[] nodes;
			if (layerGridGraph != null)
			{
				GridNodeBase[] nodes2 = layerGridGraph.nodes;
				nodes = nodes2;
			}
			else
			{
				GridNodeBase[] nodes2 = this.graph.nodes;
				nodes = nodes2;
			}
			if (this.buffer == null || this.buffer.Length != width * depth)
			{
				this.buffer = new GridNodeBase[width * depth];
			}
			if (Mathf.Abs(offset.x) <= width && Mathf.Abs(offset.y) <= depth)
			{
				IntRect recalculateRect = new IntRect(0, 0, offset.x, offset.y);
				if (recalculateRect.xmin > recalculateRect.xmax)
				{
					int xmax = recalculateRect.xmax;
					recalculateRect.xmax = width + recalculateRect.xmin;
					recalculateRect.xmin = width + xmax;
				}
				if (recalculateRect.ymin > recalculateRect.ymax)
				{
					int ymax = recalculateRect.ymax;
					recalculateRect.ymax = depth + recalculateRect.ymin;
					recalculateRect.ymin = depth + ymax;
				}
				IntRect connectionRect = recalculateRect.Expand(1);
				connectionRect = IntRect.Intersection(connectionRect, new IntRect(0, 0, width, depth));
				int widthStart = width - offset.x;
				int destOffset = offset.x;
				if (offset.x < 0)
				{
					widthStart = -offset.x;
					destOffset += width;
				}
				int num9;
				for (int i = 0; i < layers; i = num9 + 1)
				{
					int num = i * width * depth;
					for (int j = 0; j < depth; j++)
					{
						int num2 = j * width;
						int num3 = (j + offset.y + depth) % depth * width;
						int num4 = num + num2;
						Array.Copy(nodes, num4, this.buffer, num3 + destOffset, widthStart);
						Array.Copy(nodes, num4 + widthStart, this.buffer, num3, width - widthStart);
					}
					for (int k = 0; k < depth; k++)
					{
						int num5 = k * width;
						for (int l = 0; l < width; l++)
						{
							int num6 = num5 + l;
							GridNodeBase gridNodeBase = this.buffer[num6];
							if (gridNodeBase != null)
							{
								gridNodeBase.NodeInGridIndex = num6;
							}
							nodes[num + num6] = gridNodeBase;
						}
						int num7;
						int num8;
						if (k >= recalculateRect.ymin && k < recalculateRect.ymax)
						{
							num7 = 0;
							num8 = depth;
						}
						else
						{
							num7 = recalculateRect.xmin;
							num8 = recalculateRect.xmax;
						}
						for (int m = num7; m < num8; m++)
						{
							GridNodeBase gridNodeBase2 = this.buffer[num5 + m];
							if (gridNodeBase2 != null)
							{
								gridNodeBase2.ClearConnections(false);
							}
						}
					}
					if ((i & 7) == 7)
					{
						yield return null;
					}
					num9 = i;
				}
				int yieldEvery = 160;
				int num10 = Mathf.Max(Mathf.Abs(offset.x), Mathf.Abs(offset.y)) * Mathf.Max(width, depth);
				yieldEvery = Mathf.Max(yieldEvery, num10 / 10);
				int counter = 0;
				for (int i = 0; i < depth; i = num9 + 1)
				{
					int num11;
					int num12;
					if (i >= recalculateRect.ymin && i < recalculateRect.ymax)
					{
						num11 = 0;
						num12 = width;
					}
					else
					{
						num11 = recalculateRect.xmin;
						num12 = recalculateRect.xmax;
					}
					for (int n = num11; n < num12; n++)
					{
						this.graph.RecalculateCell(n, i, false, false);
					}
					counter += num12 - num11;
					if (counter > yieldEvery)
					{
						counter = 0;
						yield return null;
					}
					num9 = i;
				}
				yieldEvery *= 48;
				for (int i = 0; i < depth; i = num9 + 1)
				{
					int num13;
					int num14;
					if (i >= connectionRect.ymin && i < connectionRect.ymax)
					{
						num13 = 0;
						num14 = width;
					}
					else
					{
						num13 = connectionRect.xmin;
						num14 = connectionRect.xmax;
					}
					for (int num15 = num13; num15 < num14; num15++)
					{
						this.graph.CalculateConnections(num15, i);
					}
					counter += (num14 - num13) * layers;
					if (counter > yieldEvery)
					{
						counter = 0;
						yield return null;
					}
					num9 = i;
				}
				yield return null;
				for (int num16 = 0; num16 < depth; num16++)
				{
					for (int num17 = 0; num17 < width; num17++)
					{
						if (num17 == 0 || num16 == 0 || num17 == width - 1 || num16 == depth - 1)
						{
							this.graph.CalculateConnections(num17, num16);
						}
					}
				}
				recalculateRect = default(IntRect);
				connectionRect = default(IntRect);
			}
			else
			{
				int counter = Mathf.Max(depth * width / 20, 1000);
				int yieldEvery = 0;
				int num9;
				for (int destOffset = 0; destOffset < depth; destOffset = num9 + 1)
				{
					for (int num18 = 0; num18 < width; num18++)
					{
						this.graph.RecalculateCell(num18, destOffset, true, true);
					}
					yieldEvery += width;
					if (yieldEvery > counter)
					{
						yieldEvery = 0;
						yield return null;
					}
					num9 = destOffset;
				}
				for (int destOffset = 0; destOffset < depth; destOffset = num9 + 1)
				{
					for (int num19 = 0; num19 < width; num19++)
					{
						this.graph.CalculateConnections(num19, destOffset);
					}
					yieldEvery += width;
					if (yieldEvery > counter)
					{
						yieldEvery = 0;
						yield return null;
					}
					num9 = destOffset;
				}
			}
			yield break;
		}

		// Token: 0x04003062 RID: 12386
		public float updateDistance = 10f;

		// Token: 0x04003063 RID: 12387
		public Vector3 targetPosition;

		// Token: 0x04003064 RID: 12388
		public GridGraph graph;

		// Token: 0x04003065 RID: 12389
		[PublicizedFrom(EAccessModifier.Private)]
		public GridNodeBase[] buffer;
	}
}
