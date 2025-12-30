using System;
using System.IO;

// Token: 0x02000882 RID: 2178
public class PrefabInsideDataFile
{
	// Token: 0x06003F78 RID: 16248 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public PrefabInsideDataFile()
	{
	}

	// Token: 0x06003F79 RID: 16249 RVA: 0x0019EB1A File Offset: 0x0019CD1A
	public PrefabInsideDataFile(PrefabInsideDataFile _other)
	{
		this.size = _other.size;
		this.data = _other.data;
	}

	// Token: 0x06003F7A RID: 16250 RVA: 0x0019EB3A File Offset: 0x0019CD3A
	public void Init(Vector3i _size)
	{
		this.size = _size;
		this.data = null;
	}

	// Token: 0x06003F7B RID: 16251 RVA: 0x0019EB4A File Offset: 0x0019CD4A
	[PublicizedFrom(EAccessModifier.Private)]
	public void Alloc()
	{
		this.data = new byte[this.size.x * this.size.y * this.size.z + 7 >> 3];
	}

	// Token: 0x06003F7C RID: 16252 RVA: 0x0019EB7E File Offset: 0x0019CD7E
	public void Add(int _offset)
	{
		if (this.data == null)
		{
			this.Alloc();
		}
		byte[] array = this.data;
		int num = _offset >> 3;
		array[num] |= (byte)(1 << (_offset & 7));
	}

	// Token: 0x06003F7D RID: 16253 RVA: 0x0019EBAC File Offset: 0x0019CDAC
	public void Add(int x, int y, int z)
	{
		int offset = x + y * this.size.x + z * this.size.x * this.size.y;
		this.Add(offset);
	}

	// Token: 0x06003F7E RID: 16254 RVA: 0x0019EBEC File Offset: 0x0019CDEC
	public bool Contains(int x, int y, int z)
	{
		if (this.data == null)
		{
			return false;
		}
		int num = x + y * this.size.x + z * this.size.x * this.size.y;
		return ((int)this.data[num >> 3] & 1 << (num & 7)) > 0;
	}

	// Token: 0x06003F7F RID: 16255 RVA: 0x0019EC43 File Offset: 0x0019CE43
	public PrefabInsideDataFile Clone()
	{
		return new PrefabInsideDataFile(this);
	}

	// Token: 0x06003F80 RID: 16256 RVA: 0x0019EC4C File Offset: 0x0019CE4C
	public void Load(string _filename, Vector3i _size)
	{
		this.Init(_size);
		if (!SdFile.Exists(_filename))
		{
			return;
		}
		try
		{
			using (Stream stream = SdFile.OpenRead(_filename))
			{
				using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
				{
					pooledBinaryReader.SetBaseStream(stream);
					try
					{
						this.Read(pooledBinaryReader);
					}
					catch (Exception e)
					{
						Log.Error("PrefabInsideDataFile Load {0}, expected data len {1}. Probably outdated ins file, please re-save to fix. Read error:", new object[]
						{
							_filename,
							this.data.Length
						});
						Log.Exception(e);
					}
				}
			}
		}
		catch (Exception e2)
		{
			Log.Exception(e2);
		}
	}

	// Token: 0x06003F81 RID: 16257 RVA: 0x0019ED10 File Offset: 0x0019CF10
	[PublicizedFrom(EAccessModifier.Private)]
	public void Read(BinaryReader _br)
	{
		int num = (int)_br.ReadByte();
		int num2 = _br.ReadInt32();
		if (num <= 1)
		{
			for (int i = 0; i < num2; i++)
			{
				int x = (int)_br.ReadByte();
				int y = (int)_br.ReadByte();
				int z = (int)_br.ReadByte();
				this.Add(x, y, z);
			}
			return;
		}
		if (num2 > 0)
		{
			this.Alloc();
			_br.Read(this.data, 0, num2);
		}
	}

	// Token: 0x06003F82 RID: 16258 RVA: 0x0019ED74 File Offset: 0x0019CF74
	public void Save(string _filename)
	{
		try
		{
			using (Stream stream = SdFile.Open(_filename, FileMode.Create))
			{
				using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
				{
					pooledBinaryWriter.SetBaseStream(stream);
					this.Write(pooledBinaryWriter);
				}
			}
		}
		catch (Exception e)
		{
			Log.Exception(e);
		}
	}

	// Token: 0x06003F83 RID: 16259 RVA: 0x0019EDEC File Offset: 0x0019CFEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(2);
		int num = (this.data != null) ? this.data.Length : 0;
		_bw.Write(num);
		if (num > 0)
		{
			_bw.Write(this.data, 0, num);
		}
	}

	// Token: 0x04003320 RID: 13088
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSaveVersion = 2;

	// Token: 0x04003321 RID: 13089
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3i size;

	// Token: 0x04003322 RID: 13090
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] data;
}
