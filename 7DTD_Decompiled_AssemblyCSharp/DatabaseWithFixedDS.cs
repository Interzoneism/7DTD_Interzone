using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02001154 RID: 4436
public abstract class DatabaseWithFixedDS<KEY, DATA> where KEY : struct where DATA : class
{
	// Token: 0x06008AF8 RID: 35576 RVA: 0x0038273C File Offset: 0x0038093C
	public DatabaseWithFixedDS(int _magicBytes, int _sizeofKey, int _maxCountOfDataSets, int _sizeOfDataSet, KEY _invalidKeyValue, int _oldMaxCountOfDataSets = -1)
	{
		this.magicBytes = _magicBytes;
		this.sizeofKey = _sizeofKey;
		this.maxCountOfDataSets = _maxCountOfDataSets;
		this.oldMaxCountOfDataSets = _oldMaxCountOfDataSets;
		this.sizeOfDataSet = _sizeOfDataSet;
		this.invalidKeyValue = _invalidKeyValue;
	}

	// Token: 0x06008AF9 RID: 35577 RVA: 0x003827A0 File Offset: 0x003809A0
	public List<KEY> GetAllKeys()
	{
		DictionaryKeyList<KEY, int> obj = this.catalog;
		List<KEY> list;
		lock (obj)
		{
			list = this.catalog.list;
		}
		return list;
	}

	// Token: 0x06008AFA RID: 35578 RVA: 0x003827E8 File Offset: 0x003809E8
	public DATA GetDS(KEY _key)
	{
		DictionaryKeyList<KEY, int> obj = this.catalog;
		DATA data;
		lock (obj)
		{
			if (!this.catalog.dict.ContainsKey(_key))
			{
				data = default(DATA);
				data = data;
			}
			else
			{
				data = this.database[_key];
			}
		}
		return data;
	}

	// Token: 0x06008AFB RID: 35579 RVA: 0x00382850 File Offset: 0x00380A50
	public void SetDS(KEY _key, DATA _data)
	{
		DictionaryKeyList<KEY, int> obj = this.catalog;
		lock (obj)
		{
			if (this.catalog.dict.Count >= this.maxCountOfDataSets)
			{
				int num = this.maxCountOfDataSets / 10;
				int num2 = 0;
				while (this.catalog.list.Count > 0 && num2 < num)
				{
					KEY key = this.catalog.list[0];
					this.catalog.Remove(key);
					this.database.Remove(key);
					num2++;
				}
				this.dirty.Clear();
				for (int i = 0; i < this.catalog.list.Count; i++)
				{
					this.dirty[this.catalog.list[i]] = true;
				}
			}
			if (!this.catalog.dict.ContainsKey(_key))
			{
				this.catalog.Add(_key, this.catalog.list.Count);
			}
			this.database[_key] = _data;
			this.dirty[_key] = true;
		}
	}

	// Token: 0x06008AFC RID: 35580 RVA: 0x00382998 File Offset: 0x00380B98
	public bool ContainsDS(KEY _key)
	{
		DictionaryKeyList<KEY, int> obj = this.catalog;
		bool result;
		lock (obj)
		{
			result = this.catalog.dict.ContainsKey(_key);
		}
		return result;
	}

	// Token: 0x06008AFD RID: 35581 RVA: 0x003829E8 File Offset: 0x00380BE8
	public int CountDS()
	{
		DictionaryKeyList<KEY, int> obj = this.catalog;
		int count;
		lock (obj)
		{
			count = this.catalog.list.Count;
		}
		return count;
	}

	// Token: 0x06008AFE RID: 35582 RVA: 0x00382A34 File Offset: 0x00380C34
	public virtual void Clear()
	{
		DictionaryKeyList<KEY, int> obj = this.catalog;
		lock (obj)
		{
			this.catalog.Clear();
			this.database.Clear();
			this.dirty.Clear();
		}
	}

	// Token: 0x06008AFF RID: 35583 RVA: 0x00382A90 File Offset: 0x00380C90
	public void Load(string _dir, string _filename)
	{
		try
		{
			if (SdFile.Exists(_dir + "/" + _filename))
			{
				using (Stream stream = SdFile.OpenRead(_dir + "/" + _filename))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(stream);
						int num = pooledBinaryReader.ReadInt32();
						if (num != this.magicBytes)
						{
							Log.Error(string.Format("Map file has invalid magic bytes: 0x{0:X8}", num));
						}
						else
						{
							uint version = (uint)pooledBinaryReader.ReadByte();
							if (this.arrayReadBuf == null)
							{
								this.arrayReadBuf = new byte[this.sizeOfDataSet];
							}
							pooledBinaryReader.Read(this.arrayReadBuf, 0, 3);
							this.read(pooledBinaryReader, version);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error(string.Concat(new string[]
			{
				"Could not load file '",
				_dir,
				"/",
				_filename,
				"': ",
				ex.Message
			}));
			Log.Exception(ex);
		}
	}

	// Token: 0x06008B00 RID: 35584 RVA: 0x00382BC8 File Offset: 0x00380DC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void read(BinaryReader _br, uint _version)
	{
		DictionaryKeyList<KEY, int> obj = this.catalog;
		lock (obj)
		{
			int num = this.oldMaxCountOfDataSets;
			if (_version <= 2U && this.oldMaxCountOfDataSets < 0)
			{
				throw new Exception("Can not load old dataset file, unknown size of key list");
			}
			if (_version > 2U)
			{
				num = _br.ReadInt32();
			}
			this.catalog.Clear();
			this.dirty.Clear();
			uint num2 = _br.ReadUInt32();
			for (uint num3 = 0U; num3 < num2; num3 += 1U)
			{
				KEY key = this.readKey(_br);
				this.catalog.Add(key, (int)num3);
			}
			uint num4 = num2;
			while ((ulong)num4 < (ulong)((long)num))
			{
				this.readKey(_br);
				num4 += 1U;
			}
			this.database.Clear();
			for (int i = 0; i < this.catalog.list.Count; i++)
			{
				_br.Read(this.arrayReadBuf, 0, this.arrayReadBuf.Length);
				DATA data = this.allocateDataStorage();
				this.copyFromRead(this.arrayReadBuf, data);
				this.database[this.catalog.list[i]] = data;
			}
			if (num != this.maxCountOfDataSets)
			{
				if (num > this.maxCountOfDataSets)
				{
					int num5 = num - this.maxCountOfDataSets;
					int num6 = 0;
					while (this.catalog.list.Count > 0 && num6 < num5)
					{
						KEY key2 = this.catalog.list[0];
						this.catalog.Remove(key2);
						this.database.Remove(key2);
						num6++;
					}
					this.dirty.Clear();
					for (int j = 0; j < this.catalog.list.Count; j++)
					{
						this.dirty[this.catalog.list[j]] = true;
					}
				}
				else
				{
					this.dirty.Clear();
					for (int k = 0; k < this.catalog.list.Count; k++)
					{
						this.dirty[this.catalog.list[k]] = true;
					}
				}
			}
		}
	}

	// Token: 0x06008B01 RID: 35585 RVA: 0x00382E14 File Offset: 0x00381014
	public void Save(string _dir, string _filename)
	{
		try
		{
			if (!SdDirectory.Exists(_dir))
			{
				SdDirectory.CreateDirectory(_dir);
			}
			lock (this)
			{
				using (Stream stream = SdFile.Open(_dir + "/" + _filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
				{
					using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
					{
						pooledBinaryWriter.SetBaseStream(stream);
						pooledBinaryWriter.BaseStream.Seek(0L, SeekOrigin.Begin);
						pooledBinaryWriter.Write(this.magicBytes);
						pooledBinaryWriter.Write(3);
						pooledBinaryWriter.Write(0);
						pooledBinaryWriter.Write(0);
						pooledBinaryWriter.Write(0);
						this.write(pooledBinaryWriter);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error(string.Concat(new string[]
			{
				"Could not save file '",
				_dir,
				"/",
				_filename,
				"': ",
				ex.Message
			}));
			Log.Exception(ex);
		}
	}

	// Token: 0x06008B02 RID: 35586 RVA: 0x00382F44 File Offset: 0x00381144
	[PublicizedFrom(EAccessModifier.Private)]
	public void write(BinaryWriter _bw)
	{
		DictionaryKeyList<KEY, int> obj = this.catalog;
		lock (obj)
		{
			_bw.Write((uint)this.maxCountOfDataSets);
			_bw.Write((uint)this.catalog.list.Count);
			for (int i = 0; i < this.catalog.list.Count; i++)
			{
				this.writeKey(_bw, this.catalog.list[i]);
			}
			for (int j = this.catalog.list.Count; j < this.maxCountOfDataSets; j++)
			{
				this.writeKey(_bw, this.invalidKeyValue);
			}
			int num = 0;
			if (this.arrayWriteBuf == null)
			{
				this.arrayWriteBuf = new byte[this.sizeOfDataSet];
			}
			int count = this.catalog.list.Count;
			for (int k = 0; k < count; k++)
			{
				if (!this.dirty.ContainsKey(this.catalog.list[k]) || !this.dirty[this.catalog.list[k]])
				{
					_bw.Seek(this.sizeOfDataSet, SeekOrigin.Current);
				}
				else
				{
					DATA data = this.database[this.catalog.list[k]];
					this.copyToWrite(data, this.arrayWriteBuf);
					_bw.Write(this.arrayWriteBuf, 0, this.arrayWriteBuf.Length);
					num++;
				}
			}
			this.dirty.Clear();
			Array.Clear(this.arrayWriteBuf, 0, this.arrayWriteBuf.Length);
			int num2 = (int)Mathf.Pow(2f, Mathf.Ceil(Mathf.Log((float)count) / Mathf.Log(2f)));
			int num3 = 16 + this.maxCountOfDataSets * this.sizeofKey + num2 * this.sizeOfDataSet;
			if (_bw.BaseStream.Length < (long)num3)
			{
				for (int l = count; l < num2; l++)
				{
					_bw.Write(this.arrayWriteBuf, 0, this.arrayWriteBuf.Length);
				}
			}
			else if (_bw.BaseStream.Length > (long)num3)
			{
				_bw.BaseStream.SetLength((long)num3);
			}
		}
	}

	// Token: 0x06008B03 RID: 35587
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract KEY readKey(BinaryReader _br);

	// Token: 0x06008B04 RID: 35588
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void writeKey(BinaryWriter _bw, KEY _key);

	// Token: 0x06008B05 RID: 35589
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void copyFromRead(byte[] _dataRead, DATA _data);

	// Token: 0x06008B06 RID: 35590
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void copyToWrite(DATA _data, byte[] _dataWrite);

	// Token: 0x06008B07 RID: 35591
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract DATA allocateDataStorage();

	// Token: 0x04006CA8 RID: 27816
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cHeaderSize = 16;

	// Token: 0x04006CA9 RID: 27817
	[PublicizedFrom(EAccessModifier.Private)]
	public const int VERSION = 3;

	// Token: 0x04006CAA RID: 27818
	[PublicizedFrom(EAccessModifier.Private)]
	public int magicBytes;

	// Token: 0x04006CAB RID: 27819
	[PublicizedFrom(EAccessModifier.Private)]
	public int sizeofKey;

	// Token: 0x04006CAC RID: 27820
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxCountOfDataSets;

	// Token: 0x04006CAD RID: 27821
	[PublicizedFrom(EAccessModifier.Private)]
	public int oldMaxCountOfDataSets;

	// Token: 0x04006CAE RID: 27822
	[PublicizedFrom(EAccessModifier.Private)]
	public int sizeOfDataSet;

	// Token: 0x04006CAF RID: 27823
	[PublicizedFrom(EAccessModifier.Private)]
	public KEY invalidKeyValue;

	// Token: 0x04006CB0 RID: 27824
	[PublicizedFrom(EAccessModifier.Private)]
	public DictionaryKeyList<KEY, int> catalog = new DictionaryKeyList<KEY, int>();

	// Token: 0x04006CB1 RID: 27825
	[PublicizedFrom(EAccessModifier.Private)]
	public DictionarySave<KEY, DATA> database = new DictionarySave<KEY, DATA>();

	// Token: 0x04006CB2 RID: 27826
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<KEY, bool> dirty = new Dictionary<KEY, bool>();

	// Token: 0x04006CB3 RID: 27827
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] arrayReadBuf;

	// Token: 0x04006CB4 RID: 27828
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] arrayWriteBuf;
}
