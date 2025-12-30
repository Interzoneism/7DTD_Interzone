using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimpleJson2
{
	// Token: 0x0200197D RID: 6525
	[GeneratedCode("simple-json", "1.0.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class JsonObject : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		// Token: 0x0600BFF2 RID: 49138 RVA: 0x0048D0BE File Offset: 0x0048B2BE
		public JsonObject()
		{
			this._members = new Dictionary<string, object>();
		}

		// Token: 0x0600BFF3 RID: 49139 RVA: 0x0048D0D1 File Offset: 0x0048B2D1
		public JsonObject(IEqualityComparer<string> comparer)
		{
			this._members = new Dictionary<string, object>(comparer);
		}

		// Token: 0x1700160D RID: 5645
		public object this[int index]
		{
			get
			{
				return JsonObject.GetAtIndex(this._members, index);
			}
		}

		// Token: 0x0600BFF5 RID: 49141 RVA: 0x0048D0F4 File Offset: 0x0048B2F4
		[PublicizedFrom(EAccessModifier.Internal)]
		public static object GetAtIndex(IDictionary<string, object> obj, int index)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (index >= obj.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			int num = 0;
			foreach (KeyValuePair<string, object> keyValuePair in obj)
			{
				if (num++ == index)
				{
					return keyValuePair.Value;
				}
			}
			return null;
		}

		// Token: 0x0600BFF6 RID: 49142 RVA: 0x0048D170 File Offset: 0x0048B370
		public void Add(string key, object value)
		{
			this._members.Add(key, value);
		}

		// Token: 0x0600BFF7 RID: 49143 RVA: 0x0048D17F File Offset: 0x0048B37F
		public bool ContainsKey(string key)
		{
			return this._members.ContainsKey(key);
		}

		// Token: 0x1700160E RID: 5646
		// (get) Token: 0x0600BFF8 RID: 49144 RVA: 0x0048D18D File Offset: 0x0048B38D
		public ICollection<string> Keys
		{
			get
			{
				return this._members.Keys;
			}
		}

		// Token: 0x0600BFF9 RID: 49145 RVA: 0x0048D19A File Offset: 0x0048B39A
		public bool Remove(string key)
		{
			return this._members.Remove(key);
		}

		// Token: 0x0600BFFA RID: 49146 RVA: 0x0048D1A8 File Offset: 0x0048B3A8
		public bool TryGetValue(string key, out object value)
		{
			return this._members.TryGetValue(key, out value);
		}

		// Token: 0x1700160F RID: 5647
		// (get) Token: 0x0600BFFB RID: 49147 RVA: 0x0048D1B7 File Offset: 0x0048B3B7
		public ICollection<object> Values
		{
			get
			{
				return this._members.Values;
			}
		}

		// Token: 0x17001610 RID: 5648
		public object this[string key]
		{
			get
			{
				return this._members[key];
			}
			set
			{
				this._members[key] = value;
			}
		}

		// Token: 0x0600BFFE RID: 49150 RVA: 0x0048D1E1 File Offset: 0x0048B3E1
		public void Add(KeyValuePair<string, object> item)
		{
			this._members.Add(item.Key, item.Value);
		}

		// Token: 0x0600BFFF RID: 49151 RVA: 0x0048D1FC File Offset: 0x0048B3FC
		public void Clear()
		{
			this._members.Clear();
		}

		// Token: 0x0600C000 RID: 49152 RVA: 0x0048D209 File Offset: 0x0048B409
		public bool Contains(KeyValuePair<string, object> item)
		{
			return this._members.ContainsKey(item.Key) && this._members[item.Key] == item.Value;
		}

		// Token: 0x0600C001 RID: 49153 RVA: 0x0048D23C File Offset: 0x0048B43C
		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			int num = this.Count;
			foreach (KeyValuePair<string, object> keyValuePair in this)
			{
				array[arrayIndex++] = keyValuePair;
				if (--num <= 0)
				{
					break;
				}
			}
		}

		// Token: 0x17001611 RID: 5649
		// (get) Token: 0x0600C002 RID: 49154 RVA: 0x0048D2AC File Offset: 0x0048B4AC
		public int Count
		{
			get
			{
				return this._members.Count;
			}
		}

		// Token: 0x17001612 RID: 5650
		// (get) Token: 0x0600C003 RID: 49155 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600C004 RID: 49156 RVA: 0x0048D2B9 File Offset: 0x0048B4B9
		public bool Remove(KeyValuePair<string, object> item)
		{
			return this._members.Remove(item.Key);
		}

		// Token: 0x0600C005 RID: 49157 RVA: 0x0048D2CD File Offset: 0x0048B4CD
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return this._members.GetEnumerator();
		}

		// Token: 0x0600C006 RID: 49158 RVA: 0x0048D2CD File Offset: 0x0048B4CD
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this._members.GetEnumerator();
		}

		// Token: 0x0600C007 RID: 49159 RVA: 0x0048D2DF File Offset: 0x0048B4DF
		public override string ToString()
		{
			return SimpleJson2.SerializeObject(this);
		}

		// Token: 0x04009614 RID: 38420
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, object> _members;
	}
}
