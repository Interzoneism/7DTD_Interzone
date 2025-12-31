using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Collections.Generic
{
	// Token: 0x020013F0 RID: 5104
	[DebuggerDisplay("Count={Count}")]
	[Serializable]
	public class HashSetLong : ICollection<long>, IEnumerable<long>, IEnumerable, ISerializable, IDeserializationCallback
	{
		// Token: 0x17001134 RID: 4404
		// (get) Token: 0x06009ED3 RID: 40659 RVA: 0x003F0D9B File Offset: 0x003EEF9B
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x06009ED4 RID: 40660 RVA: 0x003F0DA3 File Offset: 0x003EEFA3
		public HashSetLong()
		{
			this.Init(10, null);
		}

		// Token: 0x06009ED5 RID: 40661 RVA: 0x003F0DB4 File Offset: 0x003EEFB4
		public HashSetLong(IEqualityComparer<long> comparer)
		{
			this.Init(10, comparer);
		}

		// Token: 0x06009ED6 RID: 40662 RVA: 0x003F0DC5 File Offset: 0x003EEFC5
		public HashSetLong(IEnumerable<long> collection) : this(collection, null)
		{
		}

		// Token: 0x06009ED7 RID: 40663 RVA: 0x003F0DD0 File Offset: 0x003EEFD0
		public HashSetLong(IEnumerable<long> collection, IEqualityComparer<long> comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			int capacity = 0;
			ICollection<long> collection2 = collection as ICollection<long>;
			if (collection2 != null)
			{
				capacity = collection2.Count;
			}
			this.Init(capacity, comparer);
			foreach (long item in collection)
			{
				this.Add(item);
			}
		}

		// Token: 0x06009ED8 RID: 40664 RVA: 0x003F0E48 File Offset: 0x003EF048
		[PublicizedFrom(EAccessModifier.Protected)]
		public HashSetLong(SerializationInfo info, StreamingContext context)
		{
			this.si = info;
		}

		// Token: 0x06009ED9 RID: 40665 RVA: 0x003F0E58 File Offset: 0x003EF058
		[PublicizedFrom(EAccessModifier.Private)]
		public void Init(int capacity, IEqualityComparer<long> comparer)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			this.comparer = (comparer ?? EqualityComparer<long>.Default);
			if (capacity == 0)
			{
				capacity = 10;
			}
			capacity = (int)((float)capacity / 0.9f) + 1;
			this.InitArrays(capacity);
			this.generation = 0;
		}

		// Token: 0x06009EDA RID: 40666 RVA: 0x003F0EA8 File Offset: 0x003EF0A8
		[PublicizedFrom(EAccessModifier.Private)]
		public void InitArrays(int size)
		{
			this.table = new int[size];
			this.links = new HashSetLong.Link[size];
			this.empty_slot = -1;
			this.slots = new long[size];
			this.touched = 0;
			this.threshold = (int)((float)this.table.Length * 0.9f);
			if (this.threshold == 0 && this.table.Length != 0)
			{
				this.threshold = 1;
			}
		}

		// Token: 0x06009EDB RID: 40667 RVA: 0x003F0F18 File Offset: 0x003EF118
		[PublicizedFrom(EAccessModifier.Private)]
		public bool SlotsContainsAt(int index, int hash, long item)
		{
			HashSetLong.Link link;
			for (int num = this.table[index] - 1; num != -1; num = link.Next)
			{
				link = this.links[num];
				if (link.HashCode == hash && item == this.slots[num])
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06009EDC RID: 40668 RVA: 0x003F0F61 File Offset: 0x003EF161
		public void CopyTo(long[] array)
		{
			this.CopyTo(array, 0, this.count);
		}

		// Token: 0x06009EDD RID: 40669 RVA: 0x003F0F71 File Offset: 0x003EF171
		public void CopyTo(long[] array, int arrayIndex)
		{
			this.CopyTo(array, arrayIndex, this.count);
		}

		// Token: 0x06009EDE RID: 40670 RVA: 0x003F0F84 File Offset: 0x003EF184
		public void CopyTo(long[] array, int arrayIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			if (arrayIndex > array.Length)
			{
				throw new ArgumentException("index larger than largest valid index of array");
			}
			if (array.Length - arrayIndex < count)
			{
				throw new ArgumentException("Destination array cannot hold the requested elements!");
			}
			int num = 0;
			int num2 = 0;
			while (num < this.touched && num2 < count)
			{
				if (this.GetLinkHashCode(num) != 0)
				{
					array[arrayIndex++] = this.slots[num];
				}
				num++;
			}
		}

		// Token: 0x06009EDF RID: 40671 RVA: 0x003F1004 File Offset: 0x003EF204
		[PublicizedFrom(EAccessModifier.Private)]
		public void Resize()
		{
			int num = HashSetLong.PrimeHelper.ToPrime(this.table.Length << 1 | 1);
			int[] array = new int[num];
			HashSetLong.Link[] array2 = new HashSetLong.Link[num];
			for (int i = 0; i < this.table.Length; i++)
			{
				for (int num2 = this.table[i] - 1; num2 != -1; num2 = this.links[num2].Next)
				{
					int num3 = ((array2[num2].HashCode = (((int)this.slots[num2] ^ (int)(this.slots[num2] >> 32)) | int.MinValue)) & int.MaxValue) % num;
					array2[num2].Next = array[num3] - 1;
					array[num3] = num2 + 1;
				}
			}
			this.table = array;
			this.links = array2;
			long[] destinationArray = new long[num];
			Array.Copy(this.slots, 0, destinationArray, 0, this.touched);
			this.slots = destinationArray;
			this.threshold = (int)((float)num * 0.9f);
		}

		// Token: 0x06009EE0 RID: 40672 RVA: 0x003F110A File Offset: 0x003EF30A
		[PublicizedFrom(EAccessModifier.Private)]
		public int GetLinkHashCode(int index)
		{
			return this.links[index].HashCode & int.MinValue;
		}

		// Token: 0x06009EE1 RID: 40673 RVA: 0x003F1124 File Offset: 0x003EF324
		public bool Add(long item)
		{
			int num = ((int)item ^ (int)(item >> 32)) | int.MinValue;
			int num2 = (num & int.MaxValue) % this.table.Length;
			if (this.SlotsContainsAt(num2, num, item))
			{
				return false;
			}
			int num3 = this.count + 1;
			this.count = num3;
			if (num3 > this.threshold)
			{
				this.Resize();
				num2 = (num & int.MaxValue) % this.table.Length;
			}
			int num4 = this.empty_slot;
			if (num4 == -1)
			{
				num3 = this.touched;
				this.touched = num3 + 1;
				num4 = num3;
			}
			else
			{
				this.empty_slot = this.links[num4].Next;
			}
			this.links[num4].HashCode = num;
			this.links[num4].Next = this.table[num2] - 1;
			this.table[num2] = num4 + 1;
			this.slots[num4] = item;
			this.generation++;
			return true;
		}

		// Token: 0x17001135 RID: 4405
		// (get) Token: 0x06009EE2 RID: 40674 RVA: 0x003F1214 File Offset: 0x003EF414
		public IEqualityComparer<long> Comparer
		{
			get
			{
				return this.comparer;
			}
		}

		// Token: 0x06009EE3 RID: 40675 RVA: 0x003F121C File Offset: 0x003EF41C
		public void Clear()
		{
			this.count = 0;
			Array.Clear(this.table, 0, this.table.Length);
			Array.Clear(this.slots, 0, this.slots.Length);
			Array.Clear(this.links, 0, this.links.Length);
			this.empty_slot = -1;
			this.touched = 0;
			this.generation++;
		}

		// Token: 0x06009EE4 RID: 40676 RVA: 0x003F1288 File Offset: 0x003EF488
		public bool Contains(long item)
		{
			int num = ((int)item ^ (int)(item >> 32)) | int.MinValue;
			int index = (num & int.MaxValue) % this.table.Length;
			return this.SlotsContainsAt(index, num, item);
		}

		// Token: 0x06009EE5 RID: 40677 RVA: 0x003F12C0 File Offset: 0x003EF4C0
		public bool Remove(long item)
		{
			int num = ((int)item ^ (int)(item >> 32)) | int.MinValue;
			int num2 = (num & int.MaxValue) % this.table.Length;
			int num3 = this.table[num2] - 1;
			if (num3 == -1)
			{
				return false;
			}
			int num4 = -1;
			do
			{
				HashSetLong.Link link = this.links[num3];
				if (link.HashCode == num && this.slots[num3] == item)
				{
					break;
				}
				num4 = num3;
				num3 = link.Next;
			}
			while (num3 != -1);
			if (num3 == -1)
			{
				return false;
			}
			this.count--;
			if (num4 == -1)
			{
				this.table[num2] = this.links[num3].Next + 1;
			}
			else
			{
				this.links[num4].Next = this.links[num3].Next;
			}
			this.links[num3].Next = this.empty_slot;
			this.empty_slot = num3;
			this.links[num3].HashCode = 0;
			this.slots[num3] = 0L;
			this.generation++;
			return true;
		}

		// Token: 0x06009EE6 RID: 40678 RVA: 0x003F13D4 File Offset: 0x003EF5D4
		public int RemoveWhere(Predicate<long> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			List<long> list = new List<long>();
			foreach (long num in this)
			{
				if (match(num))
				{
					list.Add(num);
				}
			}
			foreach (long item in list)
			{
				this.Remove(item);
			}
			return list.Count;
		}

		// Token: 0x06009EE7 RID: 40679 RVA: 0x003F1488 File Offset: 0x003EF688
		public void TrimExcess()
		{
			this.Resize();
		}

		// Token: 0x06009EE8 RID: 40680 RVA: 0x003F1490 File Offset: 0x003EF690
		public void IntersectWith(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			HashSetLong other_set = this.ToSet(other);
			this.RemoveWhere((long item) => !other_set.Contains(item));
		}

		// Token: 0x06009EE9 RID: 40681 RVA: 0x003F14D4 File Offset: 0x003EF6D4
		public void ExceptWithHashSetLong(HashSetLong other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				this.Remove(item);
			}
		}

		// Token: 0x06009EEA RID: 40682 RVA: 0x003F1534 File Offset: 0x003EF734
		public void ExceptWith(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				this.Remove(item);
			}
		}

		// Token: 0x06009EEB RID: 40683 RVA: 0x003F158C File Offset: 0x003EF78C
		public bool Overlaps(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				if (this.Contains(item))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06009EEC RID: 40684 RVA: 0x003F15EC File Offset: 0x003EF7EC
		public bool SetEquals(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			HashSetLong hashSetLong = this.ToSet(other);
			if (this.count != hashSetLong.Count)
			{
				return false;
			}
			foreach (long item in this)
			{
				if (!hashSetLong.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06009EED RID: 40685 RVA: 0x003F166C File Offset: 0x003EF86C
		public void SymmetricExceptWith(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in this.ToSet(other))
			{
				if (!this.Add(item))
				{
					this.Remove(item);
				}
			}
		}

		// Token: 0x06009EEE RID: 40686 RVA: 0x003F16D8 File Offset: 0x003EF8D8
		[PublicizedFrom(EAccessModifier.Private)]
		public HashSetLong ToSet(IEnumerable<long> enumerable)
		{
			HashSetLong hashSetLong = enumerable as HashSetLong;
			if (hashSetLong == null || !this.Comparer.Equals(hashSetLong.Comparer))
			{
				hashSetLong = new HashSetLong(enumerable, this.Comparer);
			}
			return hashSetLong;
		}

		// Token: 0x06009EEF RID: 40687 RVA: 0x003F1710 File Offset: 0x003EF910
		public void UnionWithHashSetLong(HashSetLong other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				this.Add(item);
			}
		}

		// Token: 0x06009EF0 RID: 40688 RVA: 0x003F1770 File Offset: 0x003EF970
		public void UnionWith(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				this.Add(item);
			}
		}

		// Token: 0x06009EF1 RID: 40689 RVA: 0x003F17C8 File Offset: 0x003EF9C8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckIsSubsetOf(HashSetLong other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in this)
			{
				if (!other.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06009EF2 RID: 40690 RVA: 0x003F1830 File Offset: 0x003EFA30
		public bool IsSubsetOf(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.count == 0)
			{
				return true;
			}
			HashSetLong hashSetLong = this.ToSet(other);
			return this.count <= hashSetLong.Count && this.CheckIsSubsetOf(hashSetLong);
		}

		// Token: 0x06009EF3 RID: 40691 RVA: 0x003F1874 File Offset: 0x003EFA74
		public bool IsProperSubsetOf(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this.count == 0)
			{
				return true;
			}
			HashSetLong hashSetLong = this.ToSet(other);
			return this.count < hashSetLong.Count && this.CheckIsSubsetOf(hashSetLong);
		}

		// Token: 0x06009EF4 RID: 40692 RVA: 0x003F18B8 File Offset: 0x003EFAB8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool CheckIsSupersetOf(HashSetLong other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			foreach (long item in other)
			{
				if (!this.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06009EF5 RID: 40693 RVA: 0x003F1920 File Offset: 0x003EFB20
		public bool IsSupersetOf(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			HashSetLong hashSetLong = this.ToSet(other);
			return this.count >= hashSetLong.Count && this.CheckIsSupersetOf(hashSetLong);
		}

		// Token: 0x06009EF6 RID: 40694 RVA: 0x003F195C File Offset: 0x003EFB5C
		public bool IsProperSupersetOf(IEnumerable<long> other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			HashSetLong hashSetLong = this.ToSet(other);
			return this.count > hashSetLong.Count && this.CheckIsSupersetOf(hashSetLong);
		}

		// Token: 0x06009EF7 RID: 40695 RVA: 0x003F1996 File Offset: 0x003EFB96
		public static IEqualityComparer<HashSetLong> CreateSetComparer()
		{
			return HashSetLong.setComparer;
		}

		// Token: 0x06009EF8 RID: 40696 RVA: 0x003F19A0 File Offset: 0x003EFBA0
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Version", this.generation);
			info.AddValue("Comparer", this.comparer, typeof(IEqualityComparer<long>));
			info.AddValue("Capacity", (this.table == null) ? 0 : this.table.Length);
			if (this.table != null)
			{
				long[] array = new long[this.count];
				this.CopyTo(array);
				info.AddValue("Elements", array, typeof(long[]));
			}
		}

		// Token: 0x06009EF9 RID: 40697 RVA: 0x003F1A38 File Offset: 0x003EFC38
		public virtual void OnDeserialization(object sender)
		{
			if (this.si != null)
			{
				this.generation = (int)this.si.GetValue("Version", typeof(int));
				this.comparer = (IEqualityComparer<long>)this.si.GetValue("Comparer", typeof(IEqualityComparer<long>));
				int num = (int)this.si.GetValue("Capacity", typeof(int));
				this.empty_slot = -1;
				if (num > 0)
				{
					this.table = new int[num];
					this.slots = new long[num];
					long[] array = (long[])this.si.GetValue("Elements", typeof(long[]));
					if (array == null)
					{
						throw new SerializationException("Missing Elements");
					}
					for (int i = 0; i < array.Length; i++)
					{
						this.Add(array[i]);
					}
				}
				else
				{
					this.table = null;
				}
				this.si = null;
			}
		}

		// Token: 0x06009EFA RID: 40698 RVA: 0x003F1B33 File Offset: 0x003EFD33
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator<long> GetEnumerator()
		{
			return new HashSetLong.Enumerator(this);
		}

		// Token: 0x17001136 RID: 4406
		// (get) Token: 0x06009EFB RID: 40699 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool IsReadOnly
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return false;
			}
		}

		// Token: 0x06009EFC RID: 40700 RVA: 0x003F1B40 File Offset: 0x003EFD40
		[PublicizedFrom(EAccessModifier.Private)]
		public void Add(long item)
		{
			this.Add(item);
		}

		// Token: 0x06009EFD RID: 40701 RVA: 0x003F1B33 File Offset: 0x003EFD33
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return new HashSetLong.Enumerator(this);
		}

		// Token: 0x06009EFE RID: 40702 RVA: 0x003F1B4A File Offset: 0x003EFD4A
		public HashSetLong.Enumerator GetEnumerator()
		{
			return new HashSetLong.Enumerator(this);
		}

		// Token: 0x04007A51 RID: 31313
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const int INITIAL_SIZE = 10;

		// Token: 0x04007A52 RID: 31314
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const float DEFAULT_LOAD_FACTOR = 0.9f;

		// Token: 0x04007A53 RID: 31315
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const int NO_SLOT = -1;

		// Token: 0x04007A54 RID: 31316
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public const int HASH_FLAG = -2147483648;

		// Token: 0x04007A55 RID: 31317
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int[] table;

		// Token: 0x04007A56 RID: 31318
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public HashSetLong.Link[] links;

		// Token: 0x04007A57 RID: 31319
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public long[] slots;

		// Token: 0x04007A58 RID: 31320
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int touched;

		// Token: 0x04007A59 RID: 31321
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int empty_slot;

		// Token: 0x04007A5A RID: 31322
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int count;

		// Token: 0x04007A5B RID: 31323
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int threshold;

		// Token: 0x04007A5C RID: 31324
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public IEqualityComparer<long> comparer;

		// Token: 0x04007A5D RID: 31325
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public SerializationInfo si;

		// Token: 0x04007A5E RID: 31326
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public int generation;

		// Token: 0x04007A5F RID: 31327
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public static readonly HashSetLong.HashSetEqualityComparer setComparer = new HashSetLong.HashSetEqualityComparer();

		// Token: 0x020013F1 RID: 5105
		[PublicizedFrom(EAccessModifier.Private)]
		public struct Link
		{
			// Token: 0x04007A60 RID: 31328
			public int HashCode;

			// Token: 0x04007A61 RID: 31329
			public int Next;
		}

		// Token: 0x020013F2 RID: 5106
		[PublicizedFrom(EAccessModifier.Private)]
		public class HashSetEqualityComparer : IEqualityComparer<HashSetLong>
		{
			// Token: 0x06009F00 RID: 40704 RVA: 0x003F1B60 File Offset: 0x003EFD60
			public bool Equals(HashSetLong lhs, HashSetLong rhs)
			{
				if (lhs == rhs)
				{
					return true;
				}
				if (lhs == null || rhs == null || lhs.Count != rhs.Count)
				{
					return false;
				}
				foreach (long item in lhs)
				{
					if (!rhs.Contains(item))
					{
						return false;
					}
				}
				return true;
			}

			// Token: 0x06009F01 RID: 40705 RVA: 0x003F1BD4 File Offset: 0x003EFDD4
			public int GetHashCode(HashSetLong hashset)
			{
				if (hashset == null)
				{
					return 0;
				}
				IEqualityComparer<long> @default = EqualityComparer<long>.Default;
				int num = 0;
				foreach (long obj in hashset)
				{
					num ^= @default.GetHashCode(obj);
				}
				return num;
			}
		}

		// Token: 0x020013F3 RID: 5107
		[Serializable]
		public struct Enumerator : IEnumerator<long>, IEnumerator, IDisposable
		{
			// Token: 0x06009F03 RID: 40707 RVA: 0x003F1C34 File Offset: 0x003EFE34
			[PublicizedFrom(EAccessModifier.Internal)]
			public Enumerator(HashSetLong hashset)
			{
				this = default(HashSetLong.Enumerator);
				this.hashset = hashset;
				this.stamp = hashset.generation;
			}

			// Token: 0x06009F04 RID: 40708 RVA: 0x003F1C50 File Offset: 0x003EFE50
			public bool MoveNext()
			{
				this.CheckState();
				if (this.next < 0)
				{
					return false;
				}
				while (this.next < this.hashset.touched)
				{
					int num = this.next;
					this.next = num + 1;
					int num2 = num;
					if (this.hashset.GetLinkHashCode(num2) != 0)
					{
						this.current = this.hashset.slots[num2];
						return true;
					}
				}
				this.next = -1;
				return false;
			}

			// Token: 0x17001137 RID: 4407
			// (get) Token: 0x06009F05 RID: 40709 RVA: 0x003F1CBE File Offset: 0x003EFEBE
			public long Current
			{
				get
				{
					return this.current;
				}
			}

			// Token: 0x17001138 RID: 4408
			// (get) Token: 0x06009F06 RID: 40710 RVA: 0x003F1CC6 File Offset: 0x003EFEC6
			public object Current
			{
				[PublicizedFrom(EAccessModifier.Private)]
				get
				{
					this.CheckState();
					if (this.next <= 0)
					{
						throw new InvalidOperationException("Current is not valid");
					}
					return this.current;
				}
			}

			// Token: 0x06009F07 RID: 40711 RVA: 0x003F1CED File Offset: 0x003EFEED
			[PublicizedFrom(EAccessModifier.Private)]
			public void Reset()
			{
				this.CheckState();
				this.next = 0;
			}

			// Token: 0x06009F08 RID: 40712 RVA: 0x003F1CFC File Offset: 0x003EFEFC
			public void Dispose()
			{
				this.hashset = null;
			}

			// Token: 0x06009F09 RID: 40713 RVA: 0x003F1D05 File Offset: 0x003EFF05
			[PublicizedFrom(EAccessModifier.Private)]
			public void CheckState()
			{
				if (this.hashset == null)
				{
					throw new ObjectDisposedException(null);
				}
				if (this.hashset.generation != this.stamp)
				{
					throw new InvalidOperationException("HashSet have been modified while it was iterated over");
				}
			}

			// Token: 0x04007A62 RID: 31330
			[PublicizedFrom(EAccessModifier.Private)]
			[NonSerialized]
			public HashSetLong hashset;

			// Token: 0x04007A63 RID: 31331
			[PublicizedFrom(EAccessModifier.Private)]
			[NonSerialized]
			public int next;

			// Token: 0x04007A64 RID: 31332
			[PublicizedFrom(EAccessModifier.Private)]
			[NonSerialized]
			public int stamp;

			// Token: 0x04007A65 RID: 31333
			[PublicizedFrom(EAccessModifier.Private)]
			[NonSerialized]
			public long current;
		}

		// Token: 0x020013F4 RID: 5108
		[PublicizedFrom(EAccessModifier.Private)]
		public static class PrimeHelper
		{
			// Token: 0x06009F0A RID: 40714 RVA: 0x003F1D34 File Offset: 0x003EFF34
			[PublicizedFrom(EAccessModifier.Private)]
			public static bool TestPrime(int x)
			{
				if ((x & 1) != 0)
				{
					int num = (int)Math.Sqrt((double)x);
					for (int i = 3; i < num; i += 2)
					{
						if (x % i == 0)
						{
							return false;
						}
					}
					return true;
				}
				return x == 2;
			}

			// Token: 0x06009F0B RID: 40715 RVA: 0x003F1D68 File Offset: 0x003EFF68
			[PublicizedFrom(EAccessModifier.Private)]
			public static int CalcPrime(int x)
			{
				for (int i = (x & -2) - 1; i < 2147483647; i += 2)
				{
					if (HashSetLong.PrimeHelper.TestPrime(i))
					{
						return i;
					}
				}
				return x;
			}

			// Token: 0x06009F0C RID: 40716 RVA: 0x003F1D98 File Offset: 0x003EFF98
			public static int ToPrime(int x)
			{
				for (int i = 0; i < HashSetLong.PrimeHelper.primes_table.Length; i++)
				{
					if (x <= HashSetLong.PrimeHelper.primes_table[i])
					{
						return HashSetLong.PrimeHelper.primes_table[i];
					}
				}
				return HashSetLong.PrimeHelper.CalcPrime(x);
			}

			// Token: 0x04007A66 RID: 31334
			[PublicizedFrom(EAccessModifier.Private)]
			public static readonly int[] primes_table = new int[]
			{
				11,
				19,
				37,
				73,
				109,
				163,
				251,
				367,
				557,
				823,
				1237,
				1861,
				2777,
				4177,
				6247,
				9371,
				14057,
				21089,
				31627,
				47431,
				71143,
				106721,
				160073,
				240101,
				360163,
				540217,
				810343,
				1215497,
				1823231,
				2734867,
				4102283,
				6153409,
				9230113,
				13845163
			};
		}
	}
}
