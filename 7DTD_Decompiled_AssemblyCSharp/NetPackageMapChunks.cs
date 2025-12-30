using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x0200075C RID: 1884
[Preserve]
public class NetPackageMapChunks : NetPackage
{
	// Token: 0x17000592 RID: 1426
	// (get) Token: 0x060036F0 RID: 14064 RVA: 0x000197A5 File Offset: 0x000179A5
	public override int Channel
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x17000593 RID: 1427
	// (get) Token: 0x060036F1 RID: 14065 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool Compress
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000594 RID: 1428
	// (get) Token: 0x060036F2 RID: 14066 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x060036F3 RID: 14067 RVA: 0x00168BCC File Offset: 0x00166DCC
	public NetPackageMapChunks Setup(int _entityId, List<int> _chunks, List<ushort[]> _mapPieces)
	{
		this.entityId = _entityId;
		this.chunks = new List<int>(_chunks);
		this.mapPieces = new List<ushort[]>(_mapPieces);
		return this;
	}

	// Token: 0x060036F4 RID: 14068 RVA: 0x00168BF0 File Offset: 0x00166DF0
	public override void read(PooledBinaryReader _reader)
	{
		this.chunks = new List<int>();
		this.mapPieces = new List<ushort[]>();
		this.entityId = _reader.ReadInt32();
		int num = (int)_reader.ReadUInt16();
		for (int i = 0; i < num; i++)
		{
			this.chunks.Add(_reader.ReadInt32());
			ushort[] array = new ushort[256];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = _reader.ReadUInt16();
			}
			this.mapPieces.Add(array);
		}
	}

	// Token: 0x060036F5 RID: 14069 RVA: 0x00168C74 File Offset: 0x00166E74
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		bool flag = true;
		ushort num = (ushort)this.chunks.Count;
		long position = _writer.BaseStream.Position;
		_writer.Write(num);
		for (int i = 0; i < this.chunks.Count; i++)
		{
			ushort[] array = this.mapPieces[i];
			if (array.Length != 256)
			{
				Log.Warning("Player map data for entityid {0} of invalid size {1}", new object[]
				{
					this.entityId,
					array.Length
				});
				num -= 1;
				flag = false;
			}
			else
			{
				_writer.Write(this.chunks[i]);
				for (int j = 0; j < array.Length; j++)
				{
					_writer.Write(array[j]);
				}
			}
		}
		if (!flag)
		{
			long position2 = _writer.BaseStream.Position;
			_writer.BaseStream.Position = position;
			_writer.Write(num);
			_writer.BaseStream.Position = position2;
		}
	}

	// Token: 0x060036F6 RID: 14070 RVA: 0x00168D80 File Offset: 0x00166F80
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityPlayer entityPlayer = _world.GetEntity(this.entityId) as EntityPlayer;
		if (entityPlayer != null && entityPlayer.ChunkObserver.mapDatabase != null)
		{
			entityPlayer.ChunkObserver.mapDatabase.Add(this.chunks, this.mapPieces);
		}
	}

	// Token: 0x060036F7 RID: 14071 RVA: 0x00168DD5 File Offset: 0x00166FD5
	public override int GetLength()
	{
		return 4 + ((this.chunks != null) ? (this.chunks.Count * 8) : 0) + ((this.mapPieces != null) ? (this.mapPieces.Count * 512) : 0);
	}

	// Token: 0x04002C86 RID: 11398
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002C87 RID: 11399
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> chunks;

	// Token: 0x04002C88 RID: 11400
	[PublicizedFrom(EAccessModifier.Private)]
	public List<ushort[]> mapPieces;
}
