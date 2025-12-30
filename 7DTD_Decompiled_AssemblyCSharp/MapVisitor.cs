using System;
using System.Collections;
using UnityEngine;

// Token: 0x020011BD RID: 4541
public class MapVisitor
{
	// Token: 0x140000F6 RID: 246
	// (add) Token: 0x06008DEF RID: 36335 RVA: 0x0038EE0C File Offset: 0x0038D00C
	// (remove) Token: 0x06008DF0 RID: 36336 RVA: 0x0038EE44 File Offset: 0x0038D044
	public event MapVisitor.VisitChunkDelegate OnVisitChunk;

	// Token: 0x140000F7 RID: 247
	// (add) Token: 0x06008DF1 RID: 36337 RVA: 0x0038EE7C File Offset: 0x0038D07C
	// (remove) Token: 0x06008DF2 RID: 36338 RVA: 0x0038EEB4 File Offset: 0x0038D0B4
	public event MapVisitor.VisitMapDoneDelegate OnVisitMapDone;

	// Token: 0x17000EBA RID: 3770
	// (get) Token: 0x06008DF3 RID: 36339 RVA: 0x0038EEE9 File Offset: 0x0038D0E9
	public Vector3i ChunkPosStart
	{
		get
		{
			return this.chunkPos1;
		}
	}

	// Token: 0x17000EBB RID: 3771
	// (get) Token: 0x06008DF4 RID: 36340 RVA: 0x0038EEF1 File Offset: 0x0038D0F1
	public Vector3i ChunkPosEnd
	{
		get
		{
			return this.chunkPos2;
		}
	}

	// Token: 0x17000EBC RID: 3772
	// (get) Token: 0x06008DF5 RID: 36341 RVA: 0x0038EEF9 File Offset: 0x0038D0F9
	public Vector3i WorldPosStart
	{
		get
		{
			return new Vector3i(this.chunkPos1.x << 4, 0, this.chunkPos1.z << 4);
		}
	}

	// Token: 0x17000EBD RID: 3773
	// (get) Token: 0x06008DF6 RID: 36342 RVA: 0x0038EF1B File Offset: 0x0038D11B
	public Vector3i WorldPosEnd
	{
		get
		{
			return new Vector3i((this.chunkPos2.x + 1 << 4) - 1, 255, (this.chunkPos2.z + 1 << 4) - 1);
		}
	}

	// Token: 0x06008DF7 RID: 36343 RVA: 0x0038EF4C File Offset: 0x0038D14C
	public MapVisitor(Vector3i _worldPos1, Vector3i _worldPos2)
	{
		int x = _worldPos1.x;
		int z = _worldPos1.z;
		int x2 = _worldPos2.x;
		int z2 = _worldPos2.z;
		this.chunkPos1 = new Vector3i(World.toChunkXZ((x <= x2) ? x : x2), 0, World.toChunkXZ((z <= z2) ? z : z2));
		this.chunkPos2 = new Vector3i(World.toChunkXZ((x <= x2) ? x2 : x), 0, World.toChunkXZ((z <= z2) ? z2 : z));
	}

	// Token: 0x06008DF8 RID: 36344 RVA: 0x0038EFC7 File Offset: 0x0038D1C7
	public void Start()
	{
		if (!this.hasBeenStarted)
		{
			this.coroutine = ThreadManager.StartCoroutine(this.visitCo());
			this.hasBeenStarted = true;
		}
	}

	// Token: 0x06008DF9 RID: 36345 RVA: 0x0038EFE9 File Offset: 0x0038D1E9
	public void Stop()
	{
		if (this.hasBeenStarted && this.coroutine != null)
		{
			ThreadManager.StopCoroutine(this.coroutine);
			GameManager.Instance.RemoveChunkObserver(this.observer);
			this.observer = null;
			this.coroutine = null;
		}
	}

	// Token: 0x06008DFA RID: 36346 RVA: 0x0038F024 File Offset: 0x0038D224
	public bool IsRunning()
	{
		return this.coroutine != null;
	}

	// Token: 0x06008DFB RID: 36347 RVA: 0x0038F02F File Offset: 0x0038D22F
	public bool HasBeenStarted()
	{
		return this.hasBeenStarted;
	}

	// Token: 0x06008DFC RID: 36348 RVA: 0x0038F037 File Offset: 0x0038D237
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator visitCo()
	{
		int viewDim = 8;
		int num = this.chunkPos2.x - this.chunkPos1.x + 1;
		int num2 = this.chunkPos2.z - this.chunkPos1.z + 1;
		int chunksTotal = num * num2;
		int chunksDone = 0;
		float startTime = Time.time;
		int curChunkX = Math.Min(this.chunkPos1.x + viewDim, this.chunkPos2.x);
		int curChunkZ = Math.Min(this.chunkPos1.z + viewDim, this.chunkPos2.z);
		this.observer = GameManager.Instance.AddChunkObserver(this.chunkPosToBlockPos(curChunkX, curChunkZ), false, viewDim, -1);
		yield return null;
		while (curChunkX - viewDim <= this.chunkPos2.x && curChunkZ - viewDim <= this.chunkPos2.z)
		{
			this.observer.SetPosition(this.chunkPosToBlockPos(curChunkX, curChunkZ));
			int num3;
			for (int xOffset = -viewDim; xOffset <= viewDim; xOffset = num3 + 1)
			{
				for (int zOffset = -viewDim; zOffset <= viewDim; zOffset = num3 + 1)
				{
					if (curChunkX + xOffset >= this.chunkPos1.x && curChunkZ + zOffset >= this.chunkPos1.z && curChunkX + xOffset <= this.chunkPos2.x && curChunkZ + zOffset <= this.chunkPos2.z)
					{
						Chunk chunk;
						while ((chunk = (GameManager.Instance.World.GetChunkSync(curChunkX + xOffset, curChunkZ + zOffset) as Chunk)) == null || chunk.NeedsDecoration)
						{
							yield return null;
						}
						num3 = chunksDone;
						chunksDone = num3 + 1;
						if (this.OnVisitChunk != null)
						{
							float elapsedSeconds = Time.time - startTime;
							this.OnVisitChunk(chunk, chunksDone, chunksTotal, elapsedSeconds);
						}
					}
					num3 = zOffset;
				}
				num3 = xOffset;
			}
			curChunkX += viewDim * 2 + 1;
			if (curChunkX - viewDim > this.chunkPos2.x)
			{
				curChunkX = Math.Min(this.chunkPos1.x + viewDim, this.chunkPos2.x);
				curChunkZ += viewDim * 2 + 1;
			}
		}
		yield return null;
		GameManager.Instance.RemoveChunkObserver(this.observer);
		this.observer = null;
		float elapsedSeconds2 = Time.time - startTime;
		if (this.OnVisitMapDone != null)
		{
			this.OnVisitMapDone(chunksDone, elapsedSeconds2);
		}
		this.coroutine = null;
		yield break;
	}

	// Token: 0x06008DFD RID: 36349 RVA: 0x0038F046 File Offset: 0x0038D246
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 chunkPosToBlockPos(int _x, int _z)
	{
		return new Vector3((float)this.chunkXZtoBlockXZ(_x), 0f, (float)this.chunkXZtoBlockXZ(_z));
	}

	// Token: 0x06008DFE RID: 36350 RVA: 0x0038F062 File Offset: 0x0038D262
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkXZtoBlockXZ(int _xz)
	{
		if (_xz >= 0)
		{
			return _xz * 16;
		}
		return _xz * 16 + 1;
	}

	// Token: 0x04006E0B RID: 28171
	[PublicizedFrom(EAccessModifier.Private)]
	public Coroutine coroutine;

	// Token: 0x04006E0C RID: 28172
	[PublicizedFrom(EAccessModifier.Private)]
	public ChunkManager.ChunkObserver observer;

	// Token: 0x04006E0D RID: 28173
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Vector3i chunkPos1;

	// Token: 0x04006E0E RID: 28174
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Vector3i chunkPos2;

	// Token: 0x04006E0F RID: 28175
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasBeenStarted;

	// Token: 0x020011BE RID: 4542
	// (Invoke) Token: 0x06008E00 RID: 36352
	public delegate void VisitChunkDelegate(Chunk _chunk, int _chunksVisited, int _chunksTotal, float _elapsedSeconds);

	// Token: 0x020011BF RID: 4543
	// (Invoke) Token: 0x06008E04 RID: 36356
	public delegate void VisitMapDoneDelegate(int _chunks, float _elapsedSeconds);
}
