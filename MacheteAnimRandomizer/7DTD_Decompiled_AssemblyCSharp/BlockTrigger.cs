using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000197 RID: 407
public class BlockTrigger
{
	// Token: 0x170000D4 RID: 212
	// (get) Token: 0x06000C52 RID: 3154 RVA: 0x000542D8 File Offset: 0x000524D8
	// (set) Token: 0x06000C53 RID: 3155 RVA: 0x000542E0 File Offset: 0x000524E0
	public Chunk Chunk
	{
		get
		{
			return this.chunk;
		}
		set
		{
			this.chunk = value;
			long num = 0L;
			if (this.chunk != null)
			{
				num = this.chunk.Key;
			}
			this.chunkKey = num;
		}
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x00054312 File Offset: 0x00052512
	public BlockTrigger(Chunk chunkNew)
	{
		this.Chunk = chunkNew;
	}

	// Token: 0x170000D5 RID: 213
	// (get) Token: 0x06000C55 RID: 3157 RVA: 0x00054342 File Offset: 0x00052542
	public BlockValue BlockValue
	{
		get
		{
			return this.Chunk.GetBlock(this.LocalChunkPos);
		}
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x00054358 File Offset: 0x00052558
	public void Refresh(FastTags<TagGroup.Global> questTag)
	{
		this.chunk = (Chunk)GameManager.Instance.World.GetChunkSync(this.chunkKey);
		if (this.chunk == null)
		{
			string format = "BlockTrigger.Refresh: Chunk null. ChunkKey={0}, LocalChunkPos={1}, PrefabInstance={2}. From: {3}";
			object[] array = new object[4];
			array[0] = this.chunkKey;
			array[1] = this.LocalChunkPos;
			int num = 2;
			PrefabTriggerData triggerDataOwner = this.TriggerDataOwner;
			object obj;
			if (triggerDataOwner == null)
			{
				obj = null;
			}
			else
			{
				PrefabInstance prefabInstance = triggerDataOwner.PrefabInstance;
				obj = ((prefabInstance != null) ? prefabInstance.name : null);
			}
			array[num] = obj;
			array[3] = StackTraceUtility.ExtractStackTrace();
			Log.Error(string.Format(format, array));
			return;
		}
		BlockValue blockValue = this.BlockValue;
		blockValue.Block.OnTriggerRefresh(this, blockValue, questTag);
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x00054400 File Offset: 0x00052600
	public void Read(PooledBinaryReader _br)
	{
		this.currentVersion = _br.ReadUInt16();
		if (this.currentVersion >= 2)
		{
			this.NeedsTriggered = (BlockTrigger.TriggeredStates)_br.ReadByte();
		}
		int num = (int)_br.ReadByte();
		this.TriggersIndices.Clear();
		for (int i = 0; i < num; i++)
		{
			this.TriggersIndices.Add(_br.ReadByte());
		}
		num = (int)_br.ReadByte();
		this.TriggeredByIndices.Clear();
		for (int j = 0; j < num; j++)
		{
			this.TriggeredByIndices.Add(_br.ReadByte());
		}
		num = (int)_br.ReadByte();
		this.TriggeredValues.Clear();
		for (int k = 0; k < num; k++)
		{
			this.TriggeredValues.Add(_br.ReadByte());
		}
		if (this.currentVersion >= 3)
		{
			this.ExcludeIcon = _br.ReadBoolean();
		}
		if (this.currentVersion >= 4)
		{
			this.UseOrForMultipleTriggers = _br.ReadBoolean();
		}
		if (this.currentVersion >= 5)
		{
			this.Unlock = _br.ReadBoolean();
		}
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x000544FC File Offset: 0x000526FC
	public void Write(PooledBinaryWriter _bw)
	{
		_bw.Write(5);
		_bw.Write((byte)this.NeedsTriggered);
		_bw.Write((byte)this.TriggersIndices.Count);
		for (int i = 0; i < this.TriggersIndices.Count; i++)
		{
			_bw.Write(this.TriggersIndices[i]);
		}
		_bw.Write((byte)this.TriggeredByIndices.Count);
		for (int j = 0; j < this.TriggeredByIndices.Count; j++)
		{
			_bw.Write(this.TriggeredByIndices[j]);
		}
		_bw.Write((byte)this.TriggeredValues.Count);
		for (int k = 0; k < this.TriggeredValues.Count; k++)
		{
			_bw.Write(this.TriggeredValues[k]);
		}
		_bw.Write(this.ExcludeIcon);
		_bw.Write(this.UseOrForMultipleTriggers);
		_bw.Write(this.Unlock);
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x000545F0 File Offset: 0x000527F0
	public BlockTrigger Clone()
	{
		BlockTrigger blockTrigger = new BlockTrigger(this.Chunk);
		blockTrigger.LocalChunkPos = this.LocalChunkPos;
		blockTrigger.TriggersIndices.Clear();
		blockTrigger.TriggeredByIndices.Clear();
		blockTrigger.TriggeredValues.Clear();
		for (int i = 0; i < this.TriggersIndices.Count; i++)
		{
			blockTrigger.TriggersIndices.Add(this.TriggersIndices[i]);
		}
		for (int j = 0; j < this.TriggeredByIndices.Count; j++)
		{
			blockTrigger.TriggeredByIndices.Add(this.TriggeredByIndices[j]);
		}
		for (int k = 0; k < this.TriggeredValues.Count; k++)
		{
			blockTrigger.TriggeredValues.Add(this.TriggeredValues[k]);
		}
		blockTrigger.ExcludeIcon = this.ExcludeIcon;
		blockTrigger.UseOrForMultipleTriggers = this.UseOrForMultipleTriggers;
		blockTrigger.Unlock = this.Unlock;
		return blockTrigger;
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x000546E4 File Offset: 0x000528E4
	public void CopyFrom(BlockTrigger _other)
	{
		this.LocalChunkPos = _other.LocalChunkPos;
		this.TriggersIndices.Clear();
		this.TriggeredByIndices.Clear();
		this.TriggeredValues.Clear();
		for (int i = 0; i < _other.TriggersIndices.Count; i++)
		{
			this.TriggersIndices.Add(_other.TriggersIndices[i]);
		}
		for (int j = 0; j < _other.TriggeredByIndices.Count; j++)
		{
			this.TriggeredByIndices.Add(_other.TriggeredByIndices[j]);
		}
		for (int k = 0; k < _other.TriggeredValues.Count; k++)
		{
			this.TriggeredValues.Add(_other.TriggeredValues[k]);
		}
		_other.ExcludeIcon = this.ExcludeIcon;
		_other.UseOrForMultipleTriggers = this.UseOrForMultipleTriggers;
		_other.Unlock = this.Unlock;
	}

	// Token: 0x06000C5B RID: 3163 RVA: 0x000547C9 File Offset: 0x000529C9
	public void SetTriggersFlag(byte index)
	{
		if (!this.TriggersIndices.Contains(index))
		{
			this.TriggersIndices.Add(index);
		}
	}

	// Token: 0x06000C5C RID: 3164 RVA: 0x000547E5 File Offset: 0x000529E5
	public void RemoveTriggersFlag(byte index)
	{
		this.TriggersIndices.Remove(index);
	}

	// Token: 0x06000C5D RID: 3165 RVA: 0x000547F4 File Offset: 0x000529F4
	public void RemoveAllTriggersFlags()
	{
		this.TriggersIndices.Clear();
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x00054801 File Offset: 0x00052A01
	public bool HasTriggers(byte index)
	{
		return this.TriggersIndices.Contains(index);
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x0005480F File Offset: 0x00052A0F
	public bool HasAnyTriggers()
	{
		return this.TriggersIndices.Count > 0;
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x0005481F File Offset: 0x00052A1F
	public void SetTriggeredByFlag(byte index)
	{
		if (!this.TriggeredByIndices.Contains(index))
		{
			this.TriggeredByIndices.Add(index);
		}
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x0005483B File Offset: 0x00052A3B
	public void RemoveTriggeredByFlag(byte index)
	{
		this.TriggeredByIndices.Remove(index);
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x0005484A File Offset: 0x00052A4A
	public bool HasTriggeredBy(byte index)
	{
		return this.TriggeredByIndices.Contains(index);
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x00054858 File Offset: 0x00052A58
	public bool HasAnyTriggeredBy()
	{
		return this.TriggeredByIndices.Count > 0;
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x00054868 File Offset: 0x00052A68
	public void SetTriggeredValueFlag(byte index)
	{
		if (this.TriggeredValues.Contains(index))
		{
			this.TriggeredValues.Remove(index);
			return;
		}
		this.TriggeredValues.Add(index);
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x00054894 File Offset: 0x00052A94
	public bool CheckIsTriggered()
	{
		if (this.UseOrForMultipleTriggers)
		{
			using (List<byte>.Enumerator enumerator = this.TriggeredByIndices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int num = (int)enumerator.Current;
					if (!this.TriggeredValues.Contains((byte)num))
					{
						return true;
					}
				}
			}
			return false;
		}
		using (List<byte>.Enumerator enumerator = this.TriggeredByIndices.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				int num2 = (int)enumerator.Current;
				if (!this.TriggeredValues.Contains((byte)num2))
				{
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x00054950 File Offset: 0x00052B50
	public string TriggerDisplay()
	{
		if (this.TriggeredByIndices.Count != 0 && this.TriggersIndices.Count == 0)
		{
			return string.Format("[0000FF]{0}[-]", string.Join<byte>(",", this.TriggeredByIndices));
		}
		if (this.TriggersIndices.Count != 0 && this.TriggeredByIndices.Count == 0)
		{
			return string.Format("[FF0000]{0}[-][0000FF]{1}[-]", string.Join<byte>(",", this.TriggersIndices), string.Join<byte>(",", this.TriggeredByIndices));
		}
		return string.Format("[FF0000]{0}[-] | [0000FF]{1}[-]", string.Join<byte>(",", this.TriggersIndices), string.Join<byte>(",", this.TriggeredByIndices));
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x00054A04 File Offset: 0x00052C04
	public Vector3i ToWorldPos()
	{
		if (this.Chunk != null)
		{
			return new Vector3i(this.Chunk.X * 16, this.Chunk.Y * 256, this.Chunk.Z * 16) + this.LocalChunkPos;
		}
		return Vector3i.zero;
	}

	// Token: 0x06000C68 RID: 3176 RVA: 0x00054A5C File Offset: 0x00052C5C
	public void TriggerUpdated(List<BlockChangeInfo> _blockChanges)
	{
		BlockValue block = this.Chunk.GetBlock(this.LocalChunkPos);
		if (_blockChanges != null)
		{
			block.Block.OnTriggerChanged(this, this.Chunk, this.ToWorldPos(), block, _blockChanges);
			return;
		}
		block.Block.OnTriggerChanged(this, this.Chunk, this.ToWorldPos(), block);
	}

	// Token: 0x06000C69 RID: 3177 RVA: 0x00054AB4 File Offset: 0x00052CB4
	public void OnTriggered(EntityPlayer _player, World _world, int index, List<BlockChangeInfo> _blockChanges, BlockTrigger _triggeredBy = null)
	{
		this.SetTriggeredValueFlag((byte)index);
		if (this.CheckIsTriggered())
		{
			BlockValue block = this.Chunk.GetBlock(this.LocalChunkPos);
			block.Block.OnTriggered(_player, _world, this.Chunk.ClrIdx, this.ToWorldPos(), block, _blockChanges, _triggeredBy);
			this.TriggeredValues.Clear();
		}
	}

	// Token: 0x04000A51 RID: 2641
	[PublicizedFrom(EAccessModifier.Protected)]
	public const ushort version = 5;

	// Token: 0x04000A52 RID: 2642
	[PublicizedFrom(EAccessModifier.Protected)]
	public ushort currentVersion;

	// Token: 0x04000A53 RID: 2643
	public Vector3i LocalChunkPos;

	// Token: 0x04000A54 RID: 2644
	[PublicizedFrom(EAccessModifier.Private)]
	public long chunkKey;

	// Token: 0x04000A55 RID: 2645
	[PublicizedFrom(EAccessModifier.Private)]
	public Chunk chunk;

	// Token: 0x04000A56 RID: 2646
	public PrefabTriggerData TriggerDataOwner;

	// Token: 0x04000A57 RID: 2647
	public List<byte> TriggersIndices = new List<byte>();

	// Token: 0x04000A58 RID: 2648
	public List<byte> TriggeredByIndices = new List<byte>();

	// Token: 0x04000A59 RID: 2649
	public List<byte> TriggeredValues = new List<byte>();

	// Token: 0x04000A5A RID: 2650
	public bool ExcludeIcon;

	// Token: 0x04000A5B RID: 2651
	public bool UseOrForMultipleTriggers;

	// Token: 0x04000A5C RID: 2652
	public bool Unlock;

	// Token: 0x04000A5D RID: 2653
	public BlockTrigger.TriggeredStates NeedsTriggered;

	// Token: 0x02000198 RID: 408
	public enum TriggeredStates
	{
		// Token: 0x04000A5F RID: 2655
		NotTriggered,
		// Token: 0x04000A60 RID: 2656
		NeedsTriggered,
		// Token: 0x04000A61 RID: 2657
		HasTriggered
	}
}
