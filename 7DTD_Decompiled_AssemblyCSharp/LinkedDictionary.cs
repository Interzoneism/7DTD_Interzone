using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Token: 0x020011B7 RID: 4535
public class LinkedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>
{
	// Token: 0x06008DBE RID: 36286 RVA: 0x0038E82A File Offset: 0x0038CA2A
	public LinkedDictionary()
	{
		this.m_dictionary = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();
		this.m_list = new LinkedList<KeyValuePair<TKey, TValue>>();
	}

	// Token: 0x06008DBF RID: 36287 RVA: 0x0038E848 File Offset: 0x0038CA48
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return this.m_list.GetEnumerator();
	}

	// Token: 0x06008DC0 RID: 36288 RVA: 0x0038E85A File Offset: 0x0038CA5A
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator GetEnumerator()
	{
		return this.GetEnumerator();
	}

	// Token: 0x06008DC1 RID: 36289 RVA: 0x0038E862 File Offset: 0x0038CA62
	[PublicizedFrom(EAccessModifier.Private)]
	public void Add(KeyValuePair<TKey, TValue> item)
	{
		this.Add(item.Key, item.Value);
	}

	// Token: 0x06008DC2 RID: 36290 RVA: 0x0038E878 File Offset: 0x0038CA78
	public void Clear()
	{
		this.m_dictionary.Clear();
		this.m_list.Clear();
	}

	// Token: 0x06008DC3 RID: 36291 RVA: 0x0038E890 File Offset: 0x0038CA90
	[PublicizedFrom(EAccessModifier.Private)]
	public bool Contains(KeyValuePair<TKey, TValue> item)
	{
		LinkedListNode<KeyValuePair<TKey, TValue>> linkedListNode;
		return this.m_dictionary.TryGetValue(item.Key, out linkedListNode) && EqualityComparer<TValue>.Default.Equals(linkedListNode.Value.Value, item.Value);
	}

	// Token: 0x06008DC4 RID: 36292 RVA: 0x0038E8D4 File Offset: 0x0038CAD4
	[PublicizedFrom(EAccessModifier.Private)]
	public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
	{
		if (array == null)
		{
			throw new ArgumentNullException("array");
		}
		if (arrayIndex < 0 || arrayIndex > array.Length)
		{
			throw new ArgumentOutOfRangeException("arrayIndex");
		}
		if (array.Length - arrayIndex < this.m_list.Count)
		{
			throw new ArgumentException("Not have enough space after the given arrayIndex.", "array");
		}
		LinkedListNode<KeyValuePair<TKey, TValue>> first = this.m_list.First;
		int i = arrayIndex;
		LinkedListNode<KeyValuePair<TKey, TValue>> linkedListNode = first;
		while (i < array.Length)
		{
			array[arrayIndex] = linkedListNode.Value;
			i++;
			linkedListNode = linkedListNode.Next;
		}
	}

	// Token: 0x06008DC5 RID: 36293 RVA: 0x0038E958 File Offset: 0x0038CB58
	[PublicizedFrom(EAccessModifier.Private)]
	public bool Remove(KeyValuePair<TKey, TValue> item)
	{
		LinkedListNode<KeyValuePair<TKey, TValue>> linkedListNode;
		return this.m_dictionary.TryGetValue(item.Key, out linkedListNode) && EqualityComparer<TValue>.Default.Equals(linkedListNode.Value.Value, item.Value) && this.Remove(item.Key);
	}

	// Token: 0x17000EAF RID: 3759
	// (get) Token: 0x06008DC6 RID: 36294 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public bool IsReadOnly
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return false;
		}
	}

	// Token: 0x06008DC7 RID: 36295 RVA: 0x0038E9AD File Offset: 0x0038CBAD
	public void Add(TKey key, TValue value)
	{
		if (this.m_dictionary.ContainsKey(key))
		{
			throw new ArgumentException("Already contains key.", "key");
		}
		this.m_dictionary.Add(key, this.m_list.AddLast(new KeyValuePair<TKey, TValue>(key, value)));
	}

	// Token: 0x06008DC8 RID: 36296 RVA: 0x0038E9EB File Offset: 0x0038CBEB
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ContainsKey(TKey key)
	{
		return this.m_dictionary.ContainsKey(key);
	}

	// Token: 0x06008DC9 RID: 36297 RVA: 0x0038E9FC File Offset: 0x0038CBFC
	public bool Remove(TKey key)
	{
		LinkedListNode<KeyValuePair<TKey, TValue>> node;
		if (!this.m_dictionary.Remove(key, out node))
		{
			return false;
		}
		this.m_list.Remove(node);
		return true;
	}

	// Token: 0x06008DCA RID: 36298 RVA: 0x0038E9EB File Offset: 0x0038CBEB
	public bool ContainsKey(TKey key)
	{
		return this.m_dictionary.ContainsKey(key);
	}

	// Token: 0x06008DCB RID: 36299 RVA: 0x0038EA28 File Offset: 0x0038CC28
	public bool TryGetValue(TKey key, out TValue value)
	{
		LinkedListNode<KeyValuePair<TKey, TValue>> linkedListNode;
		if (!this.m_dictionary.TryGetValue(key, out linkedListNode))
		{
			value = default(TValue);
			return false;
		}
		value = linkedListNode.Value.Value;
		return true;
	}

	// Token: 0x17000EB0 RID: 3760
	public TValue this[TKey key]
	{
		get
		{
			return this.m_dictionary[key].Value.Value;
		}
		set
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> linkedListNode;
			if (this.m_dictionary.TryGetValue(key, out linkedListNode))
			{
				linkedListNode.Value = new KeyValuePair<TKey, TValue>(key, value);
				return;
			}
			this.m_dictionary.Add(key, this.m_list.AddLast(new KeyValuePair<TKey, TValue>(key, value)));
		}
	}

	// Token: 0x17000EB1 RID: 3761
	// (get) Token: 0x06008DCE RID: 36302 RVA: 0x0038EAD5 File Offset: 0x0038CCD5
	public IEnumerable<TKey> Keys
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.Keys;
		}
	}

	// Token: 0x17000EB2 RID: 3762
	// (get) Token: 0x06008DCF RID: 36303 RVA: 0x0038EADD File Offset: 0x0038CCDD
	public IEnumerable<TValue> Values
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.Values;
		}
	}

	// Token: 0x17000EB3 RID: 3763
	// (get) Token: 0x06008DD0 RID: 36304 RVA: 0x0038EAE5 File Offset: 0x0038CCE5
	public ICollection<TKey> Keys
	{
		get
		{
			return new LinkedDictionary<TKey, TValue>.KeyCollection(this);
		}
	}

	// Token: 0x17000EB4 RID: 3764
	// (get) Token: 0x06008DD1 RID: 36305 RVA: 0x0038EAF2 File Offset: 0x0038CCF2
	public ICollection<TValue> Values
	{
		get
		{
			return new LinkedDictionary<TKey, TValue>.ValueCollection(this);
		}
	}

	// Token: 0x17000EB5 RID: 3765
	// (get) Token: 0x06008DD2 RID: 36306 RVA: 0x0038EAFF File Offset: 0x0038CCFF
	public int Count
	{
		get
		{
			return this.m_list.Count;
		}
	}

	// Token: 0x04006E00 RID: 28160
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> m_dictionary;

	// Token: 0x04006E01 RID: 28161
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly LinkedList<KeyValuePair<TKey, TValue>> m_list;

	// Token: 0x020011B8 RID: 4536
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly struct KeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable
	{
		// Token: 0x06008DD3 RID: 36307 RVA: 0x0038EB0C File Offset: 0x0038CD0C
		public KeyCollection(LinkedDictionary<TKey, TValue> parent)
		{
			this.m_parent = parent;
		}

		// Token: 0x06008DD4 RID: 36308 RVA: 0x0038EB15 File Offset: 0x0038CD15
		public IEnumerator<TKey> GetEnumerator()
		{
			return (from pair in this.m_parent.m_list
			select pair.Key).GetEnumerator();
		}

		// Token: 0x06008DD5 RID: 36309 RVA: 0x0038EB4B File Offset: 0x0038CD4B
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06008DD6 RID: 36310 RVA: 0x0000E8AD File Offset: 0x0000CAAD
		public void Add(TKey item)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06008DD7 RID: 36311 RVA: 0x0038EB53 File Offset: 0x0038CD53
		public void Clear()
		{
			this.m_parent.Clear();
		}

		// Token: 0x06008DD8 RID: 36312 RVA: 0x0038EB60 File Offset: 0x0038CD60
		public bool Contains(TKey item)
		{
			return this.m_parent.ContainsKey(item);
		}

		// Token: 0x06008DD9 RID: 36313 RVA: 0x0038EB70 File Offset: 0x0038CD70
		public void CopyTo(TKey[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (array.Length - arrayIndex < this.m_parent.m_list.Count)
			{
				throw new ArgumentException("Not have enough space after the given arrayIndex.", "array");
			}
			LinkedListNode<KeyValuePair<TKey, TValue>> first = this.m_parent.m_list.First;
			int i = arrayIndex;
			LinkedListNode<KeyValuePair<TKey, TValue>> linkedListNode = first;
			while (i < array.Length)
			{
				array[arrayIndex] = linkedListNode.Value.Key;
				i++;
				linkedListNode = linkedListNode.Next;
			}
		}

		// Token: 0x06008DDA RID: 36314 RVA: 0x0038EC04 File Offset: 0x0038CE04
		public bool Remove(TKey item)
		{
			return item != null && this.m_parent.Remove(item);
		}

		// Token: 0x17000EB6 RID: 3766
		// (get) Token: 0x06008DDB RID: 36315 RVA: 0x0038EC1C File Offset: 0x0038CE1C
		public int Count
		{
			get
			{
				return this.m_parent.m_list.Count;
			}
		}

		// Token: 0x17000EB7 RID: 3767
		// (get) Token: 0x06008DDC RID: 36316 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04006E02 RID: 28162
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly LinkedDictionary<TKey, TValue> m_parent;
	}

	// Token: 0x020011BA RID: 4538
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly struct ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable
	{
		// Token: 0x06008DE0 RID: 36320 RVA: 0x0038EC43 File Offset: 0x0038CE43
		public ValueCollection(LinkedDictionary<TKey, TValue> parent)
		{
			this.m_parent = parent;
		}

		// Token: 0x06008DE1 RID: 36321 RVA: 0x0038EC4C File Offset: 0x0038CE4C
		public IEnumerator<TValue> GetEnumerator()
		{
			return (from pair in this.m_parent.m_list
			select pair.Value).GetEnumerator();
		}

		// Token: 0x06008DE2 RID: 36322 RVA: 0x0038EC82 File Offset: 0x0038CE82
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06008DE3 RID: 36323 RVA: 0x0000E8AD File Offset: 0x0000CAAD
		public void Add(TValue item)
		{
			throw new NotSupportedException();
		}

		// Token: 0x06008DE4 RID: 36324 RVA: 0x0038EC8A File Offset: 0x0038CE8A
		public void Clear()
		{
			this.m_parent.Clear();
		}

		// Token: 0x06008DE5 RID: 36325 RVA: 0x0038EC98 File Offset: 0x0038CE98
		public bool Contains(TValue item)
		{
			return this.m_parent.m_list.Any((KeyValuePair<TKey, TValue> pair) => !EqualityComparer<TValue>.Default.Equals(pair.Value, item));
		}

		// Token: 0x06008DE6 RID: 36326 RVA: 0x0038ECD0 File Offset: 0x0038CED0
		public void CopyTo(TValue[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0 || arrayIndex > array.Length)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (array.Length - arrayIndex < this.m_parent.m_list.Count)
			{
				throw new ArgumentException("Not have enough space after the given arrayIndex.", "array");
			}
			LinkedListNode<KeyValuePair<TKey, TValue>> first = this.m_parent.m_list.First;
			int i = arrayIndex;
			LinkedListNode<KeyValuePair<TKey, TValue>> linkedListNode = first;
			while (i < array.Length)
			{
				array[arrayIndex] = linkedListNode.Value.Value;
				i++;
				linkedListNode = linkedListNode.Next;
			}
		}

		// Token: 0x06008DE7 RID: 36327 RVA: 0x0038ED64 File Offset: 0x0038CF64
		public bool Remove(TValue item)
		{
			for (LinkedListNode<KeyValuePair<TKey, TValue>> linkedListNode = this.m_parent.m_list.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (EqualityComparer<TValue>.Default.Equals(linkedListNode.Value.Value, item))
				{
					this.m_parent.Remove(linkedListNode.Value.Key);
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000EB8 RID: 3768
		// (get) Token: 0x06008DE8 RID: 36328 RVA: 0x0038EDC8 File Offset: 0x0038CFC8
		public int Count
		{
			get
			{
				return this.m_parent.m_list.Count;
			}
		}

		// Token: 0x17000EB9 RID: 3769
		// (get) Token: 0x06008DE9 RID: 36329 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x04006E05 RID: 28165
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly LinkedDictionary<TKey, TValue> m_parent;
	}
}
