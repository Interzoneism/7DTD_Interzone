using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

// Token: 0x02000A14 RID: 2580
public class NameIdMapping : IMemoryPoolableObject, IDisposable
{
	// Token: 0x1700081B RID: 2075
	// (get) Token: 0x06004F01 RID: 20225 RVA: 0x001F5134 File Offset: 0x001F3334
	public Dictionary<string, int>.Enumerator NamesToIdsIterator
	{
		get
		{
			return this.namesToIds.GetEnumerator();
		}
	}

	// Token: 0x06004F02 RID: 20226 RVA: 0x0000A7E3 File Offset: 0x000089E3
	public NameIdMapping()
	{
	}

	// Token: 0x06004F03 RID: 20227 RVA: 0x001F5141 File Offset: 0x001F3341
	public NameIdMapping(string _filename, int _maxIds)
	{
		this.InitMapping(_filename, _maxIds);
	}

	// Token: 0x06004F04 RID: 20228 RVA: 0x001F5154 File Offset: 0x001F3354
	public void InitMapping(string _filename, int _maxIds)
	{
		this.filename = _filename;
		this.path = Path.GetDirectoryName(_filename);
		if (this.namesToIds == null)
		{
			this.namesToIds = new Dictionary<string, int>(_maxIds);
		}
		if (this.idsToNames == null || this.idsToNames.Length < _maxIds)
		{
			this.idsToNames = new string[_maxIds];
		}
	}

	// Token: 0x06004F05 RID: 20229 RVA: 0x001F51A8 File Offset: 0x001F33A8
	public void AddMapping(int _id, string _name, bool _force = false)
	{
		lock (this)
		{
			if (this.idsToNames[_id] == null || _force)
			{
				this.idsToNames[_id] = _name;
				this.namesToIds[_name] = _id;
				this.isDirty = true;
			}
		}
	}

	// Token: 0x06004F06 RID: 20230 RVA: 0x001F520C File Offset: 0x001F340C
	public int GetIdForName(string _name)
	{
		lock (this)
		{
			int result;
			if (this.namesToIds.TryGetValue(_name, out result))
			{
				return result;
			}
		}
		return -1;
	}

	// Token: 0x06004F07 RID: 20231 RVA: 0x001F5258 File Offset: 0x001F3458
	public string GetNameForId(int _id)
	{
		string result;
		lock (this)
		{
			result = this.idsToNames[_id];
		}
		return result;
	}

	// Token: 0x06004F08 RID: 20232 RVA: 0x001F5298 File Offset: 0x001F3498
	public ArrayListMP<int> createIdTranslationTable(Func<string, int> _getDstId, NameIdMapping.MissingEntryCallbackDelegate _onMissingDestination = null)
	{
		ArrayListMP<int> arrayListMP = new ArrayListMP<int>(MemoryPools.poolInt, Block.MAX_BLOCKS);
		int[] items = arrayListMP.Items;
		for (int i = 0; i < items.Length; i++)
		{
			items[i] = -1;
		}
		for (int j = 0; j < this.idsToNames.Length; j++)
		{
			string text = this.idsToNames[j];
			if (text != null)
			{
				int num = _getDstId(text);
				if (num < 0)
				{
					if (_onMissingDestination == null)
					{
						Log.Error(string.Format("Creating id translation table from \"{0}\" failed: Entry \"{1}\" ({2}) in source map is unknown.", this.filename, text, j));
						return null;
					}
					num = _onMissingDestination(text, j);
					if (num < 0)
					{
						return null;
					}
				}
				items[j] = num;
			}
		}
		return arrayListMP;
	}

	// Token: 0x06004F09 RID: 20233 RVA: 0x001F5338 File Offset: 0x001F3538
	public int ReplaceNames([TupleElementNames(new string[]
	{
		"oldName",
		"newName"
	})] IEnumerable<ValueTuple<string, string>> _replacementList)
	{
		int num = 0;
		lock (this)
		{
			foreach (ValueTuple<string, string> valueTuple in _replacementList)
			{
				string item = valueTuple.Item1;
				string item2 = valueTuple.Item2;
				int num2;
				if (this.namesToIds.TryGetValue(item, out num2))
				{
					this.idsToNames[num2] = item2;
					this.namesToIds.Remove(item);
					this.namesToIds[item2] = num2;
					num++;
				}
			}
			if (num > 0)
			{
				this.isDirty = true;
			}
		}
		return num;
	}

	// Token: 0x06004F0A RID: 20234 RVA: 0x001F53F8 File Offset: 0x001F35F8
	public void SaveIfDirty(bool _async = true)
	{
		if (this.isDirty)
		{
			if (_async)
			{
				ThreadManager.AddSingleTask(delegate(ThreadManager.TaskInfo _info)
				{
					this.WriteToFile();
				}, null, null, true);
				return;
			}
			this.WriteToFile();
		}
	}

	// Token: 0x06004F0B RID: 20235 RVA: 0x001F5424 File Offset: 0x001F3624
	public void WriteToFile()
	{
		try
		{
			if (this.filename == null)
			{
				Log.Error("Can not save mapping, no filename specified");
			}
			else
			{
				if (!SdDirectory.Exists(this.path))
				{
					SdDirectory.CreateDirectory(this.path);
				}
				using (Stream stream = SdFile.Open(this.filename, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
					{
						pooledBinaryWriter.SetBaseStream(stream);
						pooledBinaryWriter.BaseStream.Seek(0L, SeekOrigin.Begin);
						this.SaveToWriter(pooledBinaryWriter);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Error("Could not save file '" + this.filename + "': " + ex.Message);
			Log.Exception(ex);
		}
	}

	// Token: 0x06004F0C RID: 20236 RVA: 0x001F5504 File Offset: 0x001F3704
	public byte[] SaveToArray()
	{
		byte[] result;
		using (PooledExpandableMemoryStream pooledExpandableMemoryStream = MemoryPools.poolMemoryStream.AllocSync(true))
		{
			using (PooledBinaryWriter pooledBinaryWriter = MemoryPools.poolBinaryWriter.AllocSync(false))
			{
				pooledBinaryWriter.SetBaseStream(pooledExpandableMemoryStream);
				this.SaveToWriter(pooledBinaryWriter);
			}
			result = pooledExpandableMemoryStream.ToArray();
		}
		return result;
	}

	// Token: 0x06004F0D RID: 20237 RVA: 0x001F5574 File Offset: 0x001F3774
	[PublicizedFrom(EAccessModifier.Private)]
	public void SaveToWriter(BinaryWriter _writer)
	{
		_writer.Write(1);
		lock (this)
		{
			int num = 0;
			long position = _writer.BaseStream.Position;
			_writer.Write(num);
			for (int i = 0; i < this.idsToNames.Length; i++)
			{
				string text = this.idsToNames[i];
				if (text != null)
				{
					_writer.Write(i);
					_writer.Write(text);
					num++;
				}
			}
			_writer.BaseStream.Position = position;
			_writer.Write(num);
			_writer.BaseStream.Position = _writer.BaseStream.Length;
			this.isDirty = false;
		}
	}

	// Token: 0x06004F0E RID: 20238 RVA: 0x001F5630 File Offset: 0x001F3830
	public bool LoadFromFile()
	{
		bool result;
		try
		{
			if (this.filename == null)
			{
				Log.Error("Can not load mapping, no filename specified");
				result = false;
			}
			else if (!SdFile.Exists(this.filename))
			{
				result = false;
			}
			else
			{
				using (Stream stream = SdFile.OpenRead(this.filename))
				{
					using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
					{
						pooledBinaryReader.SetBaseStream(stream);
						this.LoadFromReader(pooledBinaryReader);
					}
				}
				result = true;
			}
		}
		catch (Exception ex)
		{
			Log.Error("Could not load file '" + this.filename + "': " + ex.Message);
			Log.Exception(ex);
			result = false;
		}
		return result;
	}

	// Token: 0x06004F0F RID: 20239 RVA: 0x001F56FC File Offset: 0x001F38FC
	public bool LoadFromArray(byte[] _data)
	{
		bool result;
		try
		{
			using (MemoryStream memoryStream = new MemoryStream(_data))
			{
				using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false))
				{
					pooledBinaryReader.SetBaseStream(memoryStream);
					this.LoadFromReader(pooledBinaryReader);
				}
			}
			result = true;
		}
		catch (Exception ex)
		{
			Log.Error("Could not load mapping from array: " + ex.Message);
			Log.Exception(ex);
			result = false;
		}
		return result;
	}

	// Token: 0x06004F10 RID: 20240 RVA: 0x001F5790 File Offset: 0x001F3990
	[PublicizedFrom(EAccessModifier.Private)]
	public void LoadFromReader(BinaryReader _reader)
	{
		_reader.ReadInt32();
		lock (this)
		{
			Array.Clear(this.idsToNames, 0, this.idsToNames.Length);
			this.namesToIds.Clear();
			int num = _reader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				int num2 = _reader.ReadInt32();
				string text = _reader.ReadString();
				this.idsToNames[num2] = text;
				this.namesToIds[text] = num2;
			}
			this.isDirty = false;
		}
	}

	// Token: 0x06004F11 RID: 20241 RVA: 0x001F5830 File Offset: 0x001F3A30
	public void Reset()
	{
		this.filename = null;
		this.path = null;
		if (this.idsToNames != null)
		{
			Array.Clear(this.idsToNames, 0, this.idsToNames.Length);
		}
		if (this.namesToIds != null)
		{
			this.namesToIds.Clear();
		}
		this.isDirty = false;
	}

	// Token: 0x06004F12 RID: 20242 RVA: 0x00002914 File Offset: 0x00000B14
	public void Cleanup()
	{
	}

	// Token: 0x06004F13 RID: 20243 RVA: 0x001F5881 File Offset: 0x001F3A81
	[PublicizedFrom(EAccessModifier.Private)]
	public void Dispose()
	{
		MemoryPools.poolNameIdMapping.FreeSync(this);
	}

	// Token: 0x04003C97 RID: 15511
	[PublicizedFrom(EAccessModifier.Private)]
	public const int FILE_VERSION = 1;

	// Token: 0x04003C98 RID: 15512
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, int> namesToIds;

	// Token: 0x04003C99 RID: 15513
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] idsToNames;

	// Token: 0x04003C9A RID: 15514
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04003C9B RID: 15515
	[PublicizedFrom(EAccessModifier.Private)]
	public string path;

	// Token: 0x04003C9C RID: 15516
	[PublicizedFrom(EAccessModifier.Private)]
	public string filename;

	// Token: 0x02000A15 RID: 2581
	// (Invoke) Token: 0x06004F16 RID: 20246
	public delegate int MissingEntryCallbackDelegate(string _entryName, int _sourceId);
}
