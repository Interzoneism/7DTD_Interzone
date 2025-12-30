using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// Token: 0x020011C7 RID: 4551
public class MemoryTracker
{
	// Token: 0x17000EC4 RID: 3780
	// (get) Token: 0x06008E44 RID: 36420 RVA: 0x0038FED4 File Offset: 0x0038E0D4
	public static MemoryTracker Instance
	{
		get
		{
			if (MemoryTracker.m_Instance == null)
			{
				MemoryTracker.m_Instance = new MemoryTracker();
			}
			return MemoryTracker.m_Instance;
		}
	}

	// Token: 0x06008E45 RID: 36421 RVA: 0x0038FEEC File Offset: 0x0038E0EC
	public void New(object _o)
	{
		Dictionary<object, int> obj = this.refs;
		lock (obj)
		{
			Type type = _o.GetType();
			this.refs[type] = (this.refs.ContainsKey(type) ? (this.refs[type] + 1) : 1);
		}
	}

	// Token: 0x06008E46 RID: 36422 RVA: 0x0038FF58 File Offset: 0x0038E158
	public void Delete(object _o)
	{
		Dictionary<object, int> obj = this.refs;
		lock (obj)
		{
			Type type = _o.GetType();
			this.refs[type] = this.refs[type] - 1;
		}
	}

	// Token: 0x06008E47 RID: 36423 RVA: 0x0038FFB4 File Offset: 0x0038E1B4
	public void SetEstimationFunction(object _o, MemoryTracker.EstimateOwnedBytes _func)
	{
		if (_o == null)
		{
			return;
		}
		Dictionary<Type, MemoryTracker.AllocationsForType> obj = this.allocTypeDict;
		lock (obj)
		{
			Type type = _o.GetType();
			MemoryTracker.AllocationsForType allocationsForType;
			if (!this.allocTypeDict.TryGetValue(type, out allocationsForType))
			{
				allocationsForType = new MemoryTracker.AllocationsForType();
				this.allocTypeDict.Add(type, allocationsForType);
			}
			allocationsForType.allocations.AddLast(new MemoryTracker.Allocation(_o, _func));
		}
	}

	// Token: 0x06008E48 RID: 36424 RVA: 0x00390030 File Offset: 0x0038E230
	[PublicizedFrom(EAccessModifier.Private)]
	public int EstimateSelfBytes()
	{
		int num = 0;
		Dictionary<object, int> obj = this.refs;
		lock (obj)
		{
			num += MemoryTracker.GetUsedSize<object, int>(this.refs);
			num += MemoryTracker.GetUsedSize<object, int>(this.last);
		}
		Dictionary<Type, MemoryTracker.AllocationsForType> obj2 = this.allocTypeDict;
		lock (obj2)
		{
			num += MemoryTracker.GetUsedSize<Type, MemoryTracker.AllocationsForType>(this.allocTypeDict);
			foreach (MemoryTracker.AllocationsForType allocationsForType in this.allocTypeDict.Values)
			{
				num += allocationsForType.allocations.Count * MemoryTracker.GetSize<MemoryTracker.Allocation>();
			}
		}
		return num;
	}

	// Token: 0x06008E49 RID: 36425 RVA: 0x00390118 File Offset: 0x0038E318
	public void Dump()
	{
		Dictionary<string, MemoryTracker.AllocationsForType.Summary> dictionary = new Dictionary<string, MemoryTracker.AllocationsForType.Summary>();
		long num = 0L;
		Dictionary<object, int> obj = this.refs;
		lock (obj)
		{
			Log.Out("---Classes----------------------------------------");
			foreach (KeyValuePair<object, int> keyValuePair in this.refs)
			{
				Log.Out(string.Concat(new string[]
				{
					keyValuePair.Key.ToString(),
					" = ",
					keyValuePair.Value.ToString(),
					" last = ",
					(this.last.ContainsKey(keyValuePair.Key) ? this.last[keyValuePair.Key] : 0).ToString()
				}));
				this.last[keyValuePair.Key] = keyValuePair.Value;
			}
		}
		long totalMemory = GC.GetTotalMemory(false);
		Dictionary<Type, MemoryTracker.AllocationsForType> obj2 = this.allocTypeDict;
		lock (obj2)
		{
			foreach (KeyValuePair<Type, MemoryTracker.AllocationsForType> keyValuePair2 in this.allocTypeDict)
			{
				Type key = keyValuePair2.Key;
				MemoryTracker.AllocationsForType value = keyValuePair2.Value;
				MemoryTracker.AllocationsForType.Summary summary = default(MemoryTracker.AllocationsForType.Summary);
				summary.numGC = value.ClearDeadAllocations();
				foreach (MemoryTracker.Allocation allocation in value.allocations)
				{
					summary.totalBytes += (long)allocation.GetOwnedBytes();
					summary.numInstances++;
				}
				dictionary.Add(key.ToString(), summary);
				num += summary.totalBytes;
			}
		}
		int num2 = this.EstimateSelfBytes();
		dictionary.Add(typeof(MemoryTracker).ToString(), new MemoryTracker.AllocationsForType.Summary((long)num2));
		num += (long)num2;
		double num3 = (double)(totalMemory - num) * 9.5367431640625E-07;
		Log.Out("GC.GetTotalMemory (MB): {0:F2}", new object[]
		{
			(double)totalMemory * 9.5367431640625E-07
		});
		Log.Out("Total Tracked (MB): {0:F2}", new object[]
		{
			(double)num * 9.5367431640625E-07
		});
		Log.Out("Untracked (MB): {0:F2}", new object[]
		{
			num3
		});
		if (num > 0L)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder.Append("Untracked,");
			stringBuilder2.AppendFormat("{0:F2},", num3);
			Log.Out("---Tracked----------------------------------------");
			foreach (KeyValuePair<string, MemoryTracker.AllocationsForType.Summary> keyValuePair3 in dictionary)
			{
				string key2 = keyValuePair3.Key;
				MemoryTracker.AllocationsForType.Summary value2 = keyValuePair3.Value;
				double num4 = (double)value2.totalBytes * 9.5367431640625E-07;
				Log.Out("{0}: {1:F2} MB, Count = {2}, GC Count = {3}", new object[]
				{
					key2,
					num4,
					value2.numInstances,
					value2.numGC
				});
				stringBuilder.AppendFormat("{0},", key2);
				stringBuilder2.AppendFormat("{0:F2},", num4);
			}
			if (stringBuilder.Length > 0)
			{
				Log.Out("---CSV----------------------------------------");
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
				Log.Out(stringBuilder.ToString());
				Log.Out(stringBuilder2.ToString());
			}
		}
	}

	// Token: 0x06008E4A RID: 36426 RVA: 0x00390540 File Offset: 0x0038E740
	[PublicizedFrom(EAccessModifier.Private)]
	public void DrawLabel(float _left, float _top, string _text)
	{
		Rect rect = new Rect(_left, _top, (float)(_text.Length * 20), 30f);
		Rect position = new Rect(rect);
		position.x += 1f;
		position.y += 1f;
		GUI.color = Color.black;
		GUI.Label(position, _text);
		GUI.color = Color.white;
		GUI.Label(rect, _text);
	}

	// Token: 0x06008E4B RID: 36427 RVA: 0x003905B8 File Offset: 0x0038E7B8
	public void DebugOnGui()
	{
		this.DrawLabel(800f, 30f, "Type");
		this.DrawLabel(1000f, 30f, "Count");
		int num = 0;
		Dictionary<object, int> obj = this.refs;
		lock (obj)
		{
			foreach (KeyValuePair<object, int> keyValuePair in this.refs)
			{
				this.DrawLabel(800f, (float)(80 + num * 35), keyValuePair.Key.ToString());
				this.DrawLabel(1000f, (float)(80 + num * 35), keyValuePair.Value.ToString());
				num++;
			}
		}
	}

	// Token: 0x06008E4C RID: 36428 RVA: 0x003906A0 File Offset: 0x0038E8A0
	public static int GetSize<T>()
	{
		return MemoryTracker.GetSize(typeof(T));
	}

	// Token: 0x06008E4D RID: 36429 RVA: 0x003906B1 File Offset: 0x0038E8B1
	public static int GetSize(Type _type)
	{
		if (_type.IsEnum)
		{
			return Marshal.SizeOf(Enum.GetUnderlyingType(_type));
		}
		if (_type.IsValueType)
		{
			return UnsafeUtility.SizeOf(_type);
		}
		return IntPtr.Size;
	}

	// Token: 0x06008E4E RID: 36430 RVA: 0x003906DC File Offset: 0x0038E8DC
	public static int GetSize<T>(T[] _array)
	{
		int num = IntPtr.Size;
		if (_array != null)
		{
			num += MemoryTracker.GetSize<T>() * _array.Length;
		}
		return num;
	}

	// Token: 0x06008E4F RID: 36431 RVA: 0x00390700 File Offset: 0x0038E900
	public static int GetSize<T>(T[][] _doubleArray)
	{
		int num = IntPtr.Size;
		if (_doubleArray != null)
		{
			foreach (T[] array in _doubleArray)
			{
				num += MemoryTracker.GetSize<T>(array);
			}
		}
		return num;
	}

	// Token: 0x06008E50 RID: 36432 RVA: 0x00390734 File Offset: 0x0038E934
	public static int GetSize<T>(T[,] _array)
	{
		int num = IntPtr.Size;
		if (_array != null)
		{
			num += MemoryTracker.GetSize<T>() * _array.GetLength(0) * _array.GetLength(1);
		}
		return num;
	}

	// Token: 0x06008E51 RID: 36433 RVA: 0x00390764 File Offset: 0x0038E964
	public static int GetSize<T>(List<T> _list)
	{
		int num = IntPtr.Size;
		if (_list != null)
		{
			num += _list.Capacity * MemoryTracker.GetSize<T>();
		}
		return num;
	}

	// Token: 0x06008E52 RID: 36434 RVA: 0x0039078C File Offset: 0x0038E98C
	public static int GetUsedSize<TKey, TValue>(IDictionary<TKey, TValue> _dictionary)
	{
		int num = IntPtr.Size;
		if (_dictionary != null)
		{
			num += (MemoryTracker.GetSize<TKey>() + MemoryTracker.GetSize<TValue>()) * _dictionary.Count;
		}
		return num;
	}

	// Token: 0x06008E53 RID: 36435 RVA: 0x003907B8 File Offset: 0x0038E9B8
	public static int GetSize(string stringVal)
	{
		if (stringVal == null)
		{
			return IntPtr.Size;
		}
		return stringVal.Length * 2 + IntPtr.Size;
	}

	// Token: 0x06008E54 RID: 36436 RVA: 0x003907D4 File Offset: 0x0038E9D4
	public static int GetSize(Dictionary<string, string> stringDict)
	{
		int num = 0;
		foreach (KeyValuePair<string, string> keyValuePair in stringDict)
		{
			num += MemoryTracker.GetSize(keyValuePair.Key) + MemoryTracker.GetSize(keyValuePair.Value);
		}
		return num;
	}

	// Token: 0x06008E55 RID: 36437 RVA: 0x0039083C File Offset: 0x0038EA3C
	public static int GetSizeAuto(object _obj)
	{
		if (_obj == null)
		{
			return IntPtr.Size;
		}
		Type type = _obj.GetType();
		if (type.IsEnum)
		{
			return Marshal.SizeOf(Enum.GetUnderlyingType(type));
		}
		if (type.IsValueType)
		{
			return UnsafeUtility.SizeOf(type);
		}
		if (type.IsArray)
		{
			Type elementType = type.GetElementType();
			Array array = _obj as Array;
			int num = IntPtr.Size;
			if (array != null)
			{
				for (int i = 0; i < array.Rank; i++)
				{
					num += array.GetLength(i) * MemoryTracker.GetSize(elementType);
				}
			}
			return num;
		}
		if (typeof(string).IsAssignableFrom(type))
		{
			string text = (string)_obj;
			int num2 = IntPtr.Size;
			if (text != null)
			{
				num2 += MemoryTracker.GetSize(text);
			}
			return num2;
		}
		return IntPtr.Size;
	}

	// Token: 0x04006E28 RID: 28200
	[PublicizedFrom(EAccessModifier.Private)]
	public static MemoryTracker m_Instance;

	// Token: 0x04006E29 RID: 28201
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<object, int> refs = new Dictionary<object, int>();

	// Token: 0x04006E2A RID: 28202
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<object, int> last = new Dictionary<object, int>();

	// Token: 0x04006E2B RID: 28203
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<Type, MemoryTracker.AllocationsForType> allocTypeDict = new Dictionary<Type, MemoryTracker.AllocationsForType>();

	// Token: 0x020011C8 RID: 4552
	// (Invoke) Token: 0x06008E58 RID: 36440
	public delegate int EstimateOwnedBytes(object obj);

	// Token: 0x020011C9 RID: 4553
	public struct Allocation
	{
		// Token: 0x06008E5B RID: 36443 RVA: 0x00390926 File Offset: 0x0038EB26
		public Allocation(object _obj, MemoryTracker.EstimateOwnedBytes _func)
		{
			this.obj = new WeakReference<object>(_obj);
			this.estimateBytesFunc = _func;
		}

		// Token: 0x06008E5C RID: 36444 RVA: 0x0039093C File Offset: 0x0038EB3C
		public int GetOwnedBytes()
		{
			object obj;
			if (this.obj.TryGetTarget(out obj))
			{
				return this.estimateBytesFunc(obj);
			}
			return 0;
		}

		// Token: 0x04006E2C RID: 28204
		public WeakReference<object> obj;

		// Token: 0x04006E2D RID: 28205
		public MemoryTracker.EstimateOwnedBytes estimateBytesFunc;
	}

	// Token: 0x020011CA RID: 4554
	public class AllocationsForType
	{
		// Token: 0x06008E5D RID: 36445 RVA: 0x00390968 File Offset: 0x0038EB68
		public int ClearDeadAllocations()
		{
			int num = 0;
			LinkedListNode<MemoryTracker.Allocation> next;
			for (LinkedListNode<MemoryTracker.Allocation> linkedListNode = this.allocations.First; linkedListNode != null; linkedListNode = next)
			{
				next = linkedListNode.Next;
				object obj;
				if (!linkedListNode.Value.obj.TryGetTarget(out obj) || obj == null)
				{
					this.allocations.Remove(linkedListNode);
					num++;
				}
			}
			return num;
		}

		// Token: 0x04006E2E RID: 28206
		public LinkedList<MemoryTracker.Allocation> allocations = new LinkedList<MemoryTracker.Allocation>();

		// Token: 0x020011CB RID: 4555
		public struct Summary
		{
			// Token: 0x06008E5F RID: 36447 RVA: 0x003909CA File Offset: 0x0038EBCA
			public Summary(long _totalBytes)
			{
				this.totalBytes = _totalBytes;
				this.numInstances = 1;
				this.numGC = 0;
			}

			// Token: 0x04006E2F RID: 28207
			public long totalBytes;

			// Token: 0x04006E30 RID: 28208
			public int numInstances;

			// Token: 0x04006E31 RID: 28209
			public int numGC;
		}
	}
}
