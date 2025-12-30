using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MemoryPack;
using MemoryPack.Formatters;
using MemoryPack.Internal;

// Token: 0x02001177 RID: 4471
[MemoryPackable(GenerateType.Object)]
public class DictionarySave<T1, T2> : IMemoryPackable<DictionarySave<T1, T2>>, IMemoryPackFormatterRegister where T2 : class
{
	// Token: 0x17000E8E RID: 3726
	public virtual T2 this[T1 _v]
	{
		get
		{
			if (!DictionarySave<T1, T2>.KeyIsValuetype && _v == null)
			{
				return default(T2);
			}
			T2 result;
			if (this.dic.TryGetValue(_v, out result))
			{
				return result;
			}
			return default(T2);
		}
		set
		{
			this.dic[_v] = value;
		}
	}

	// Token: 0x17000E8F RID: 3727
	// (get) Token: 0x06008BB2 RID: 35762 RVA: 0x00385268 File Offset: 0x00383468
	public Dictionary<T1, T2> Dict
	{
		get
		{
			return this.dic;
		}
	}

	// Token: 0x06008BB3 RID: 35763 RVA: 0x00385270 File Offset: 0x00383470
	public bool ContainsKey(T1 _key)
	{
		return this.dic.ContainsKey(_key);
	}

	// Token: 0x06008BB4 RID: 35764 RVA: 0x0038527E File Offset: 0x0038347E
	public bool TryGetValue(T1 _key, out T2 _value)
	{
		return this.dic.TryGetValue(_key, out _value);
	}

	// Token: 0x06008BB5 RID: 35765 RVA: 0x0038528D File Offset: 0x0038348D
	public void Add(T1 _key, T2 _value)
	{
		this.dic.Add(_key, _value);
	}

	// Token: 0x06008BB6 RID: 35766 RVA: 0x0038529C File Offset: 0x0038349C
	public void Remove(T1 _key)
	{
		this.dic.Remove(_key);
	}

	// Token: 0x06008BB7 RID: 35767 RVA: 0x003852AB File Offset: 0x003834AB
	public void Clear()
	{
		this.dic.Clear();
	}

	// Token: 0x06008BB8 RID: 35768 RVA: 0x003852B8 File Offset: 0x003834B8
	public void MarkToRemove(T1 _v)
	{
		this.toRemove.Add(_v);
	}

	// Token: 0x06008BB9 RID: 35769 RVA: 0x003852C8 File Offset: 0x003834C8
	public void RemoveAllMarked(DictionarySave<T1, T2>.DictionaryRemoveCallback _callback)
	{
		foreach (T1 o in this.toRemove)
		{
			_callback(o);
		}
		this.toRemove.Clear();
	}

	// Token: 0x17000E90 RID: 3728
	// (get) Token: 0x06008BBA RID: 35770 RVA: 0x00385328 File Offset: 0x00383528
	public int Count
	{
		get
		{
			return this.dic.Count;
		}
	}

	// Token: 0x06008BBB RID: 35771 RVA: 0x00385335 File Offset: 0x00383535
	[PublicizedFrom(EAccessModifier.Private)]
	static DictionarySave()
	{
		DictionarySave<T1, T2>.RegisterFormatter();
	}

	// Token: 0x06008BBC RID: 35772 RVA: 0x00385350 File Offset: 0x00383550
	[Preserve]
	public static void RegisterFormatter()
	{
		if (!MemoryPackFormatterProvider.IsRegistered<DictionarySave<T1, T2>>())
		{
			MemoryPackFormatterProvider.Register<DictionarySave<T1, T2>>(new DictionarySave<T1, T2>.DictionarySaveFormatter());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<DictionarySave<T1, T2>[]>())
		{
			MemoryPackFormatterProvider.Register<DictionarySave<T1, T2>[]>(new ArrayFormatter<DictionarySave<T1, T2>>());
		}
		if (!MemoryPackFormatterProvider.IsRegistered<Dictionary<T1, T2>>())
		{
			MemoryPackFormatterProvider.Register<Dictionary<T1, T2>>(new DictionaryFormatter<T1, T2>());
		}
	}

	// Token: 0x06008BBD RID: 35773 RVA: 0x00385388 File Offset: 0x00383588
	[Preserve]
	public static void Serialize(ref MemoryPackWriter writer, [Nullable(new byte[]
	{
		2,
		1,
		1
	})] ref DictionarySave<T1, T2> value)
	{
		if (value == null)
		{
			writer.WriteNullObjectHeader();
			return;
		}
		writer.WriteObjectHeader(3);
		writer.WriteValue<Dictionary<T1, T2>>(value.dic);
		Dictionary<T1, T2> dict = value.Dict;
		writer.WriteValue<Dictionary<T1, T2>>(dict);
		int count = value.Count;
		writer.WriteUnmanaged<int>(count);
	}

	// Token: 0x06008BBE RID: 35774 RVA: 0x003853D4 File Offset: 0x003835D4
	[Preserve]
	public static void Deserialize(ref MemoryPackReader reader, [Nullable(new byte[]
	{
		2,
		1,
		1
	})] ref DictionarySave<T1, T2> value)
	{
		byte b;
		if (!reader.TryReadObjectHeader(out b))
		{
			value = null;
			return;
		}
		Dictionary<T1, T2> dictionary;
		if (b == 3)
		{
			Dictionary<T1, T2> dictionary2;
			int num;
			if (value == null)
			{
				dictionary = reader.ReadValue<Dictionary<T1, T2>>();
				dictionary2 = reader.ReadValue<Dictionary<T1, T2>>();
				reader.ReadUnmanaged<int>(out num);
				goto IL_D1;
			}
			dictionary = value.dic;
			dictionary2 = value.Dict;
			num = value.Count;
			reader.ReadValue<Dictionary<T1, T2>>(ref dictionary);
			reader.ReadValue<Dictionary<T1, T2>>(ref dictionary2);
			reader.ReadUnmanaged<int>(out num);
		}
		else
		{
			if (b > 3)
			{
				MemoryPackSerializationException.ThrowInvalidPropertyCount(typeof(DictionarySave<T1, T2>), 3, b);
				return;
			}
			Dictionary<T1, T2> dictionary2;
			if (value == null)
			{
				dictionary = null;
				dictionary2 = null;
				int num = 0;
			}
			else
			{
				dictionary = value.dic;
				dictionary2 = value.Dict;
				int num = value.Count;
			}
			if (b != 0)
			{
				reader.ReadValue<Dictionary<T1, T2>>(ref dictionary);
				if (b != 1)
				{
					reader.ReadValue<Dictionary<T1, T2>>(ref dictionary2);
					if (b != 2)
					{
						int num;
						reader.ReadUnmanaged<int>(out num);
					}
				}
			}
			if (value == null)
			{
				goto IL_D1;
			}
		}
		value.dic = dictionary;
		return;
		IL_D1:
		value = new DictionarySave<T1, T2>
		{
			dic = dictionary
		};
	}

	// Token: 0x04006D16 RID: 27926
	[MemoryPackInclude]
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<T1, T2> dic = new Dictionary<T1, T2>();

	// Token: 0x04006D17 RID: 27927
	[PublicizedFrom(EAccessModifier.Private)]
	public List<T1> toRemove = new List<T1>();

	// Token: 0x04006D18 RID: 27928
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly bool KeyIsValuetype = typeof(T1).IsValueType;

	// Token: 0x02001178 RID: 4472
	// (Invoke) Token: 0x06008BC1 RID: 35777
	public delegate void DictionaryRemoveCallback(T1 _o);

	// Token: 0x02001179 RID: 4473
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1,
		1,
		1
	})]
	[Preserve]
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class DictionarySaveFormatter : MemoryPackFormatter<DictionarySave<T1, T2>>
	{
		// Token: 0x06008BC4 RID: 35780 RVA: 0x003854DE File Offset: 0x003836DE
		[Preserve]
		public override void Serialize(ref MemoryPackWriter writer, ref DictionarySave<T1, T2> value)
		{
			DictionarySave<T1, T2>.Serialize(ref writer, ref value);
		}

		// Token: 0x06008BC5 RID: 35781 RVA: 0x003854E7 File Offset: 0x003836E7
		[Preserve]
		public override void Deserialize(ref MemoryPackReader reader, ref DictionarySave<T1, T2> value)
		{
			DictionarySave<T1, T2>.Deserialize(ref reader, ref value);
		}
	}
}
