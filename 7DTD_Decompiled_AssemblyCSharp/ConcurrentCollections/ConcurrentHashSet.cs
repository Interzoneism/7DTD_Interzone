using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ConcurrentCollections
{
	// Token: 0x0200178E RID: 6030
	[DebuggerDisplay("Count = {Count}")]
	public class ConcurrentHashSet<T> : IReadOnlyCollection<T>, IEnumerable<!0>, IEnumerable, ICollection<T>
	{
		// Token: 0x1700144B RID: 5195
		// (get) Token: 0x0600B4AB RID: 46251 RVA: 0x0045C283 File Offset: 0x0045A483
		public static int DefaultConcurrencyLevel
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return Math.Max(2, Environment.ProcessorCount);
			}
		}

		// Token: 0x1700144C RID: 5196
		// (get) Token: 0x0600B4AC RID: 46252 RVA: 0x0045C290 File Offset: 0x0045A490
		public int Count
		{
			get
			{
				int num = 0;
				int toExclusive = 0;
				try
				{
					this.AcquireAllLocks(ref toExclusive);
					for (int i = 0; i < this._tables.CountPerLock.Length; i++)
					{
						num += this._tables.CountPerLock[i];
					}
				}
				finally
				{
					this.ReleaseLocks(0, toExclusive);
				}
				return num;
			}
		}

		// Token: 0x1700144D RID: 5197
		// (get) Token: 0x0600B4AD RID: 46253 RVA: 0x0045C2F8 File Offset: 0x0045A4F8
		public bool IsEmpty
		{
			get
			{
				int toExclusive = 0;
				try
				{
					this.AcquireAllLocks(ref toExclusive);
					for (int i = 0; i < this._tables.CountPerLock.Length; i++)
					{
						if (this._tables.CountPerLock[i] != 0)
						{
							return false;
						}
					}
				}
				finally
				{
					this.ReleaseLocks(0, toExclusive);
				}
				return true;
			}
		}

		// Token: 0x0600B4AE RID: 46254 RVA: 0x0045C360 File Offset: 0x0045A560
		public ConcurrentHashSet() : this(ConcurrentHashSet<T>.DefaultConcurrencyLevel, 31, true, null)
		{
		}

		// Token: 0x0600B4AF RID: 46255 RVA: 0x0045C371 File Offset: 0x0045A571
		public ConcurrentHashSet(Action<T> onRemovalFaiuire) : this(ConcurrentHashSet<T>.DefaultConcurrencyLevel, 31, true, null)
		{
			this.OnRemovalFailure = onRemovalFaiuire;
		}

		// Token: 0x0600B4B0 RID: 46256 RVA: 0x0045C389 File Offset: 0x0045A589
		public ConcurrentHashSet(int concurrencyLevel, int capacity) : this(concurrencyLevel, capacity, false, null)
		{
		}

		// Token: 0x0600B4B1 RID: 46257 RVA: 0x0045C395 File Offset: 0x0045A595
		public ConcurrentHashSet(IEnumerable<T> collection) : this(collection, null)
		{
		}

		// Token: 0x0600B4B2 RID: 46258 RVA: 0x0045C39F File Offset: 0x0045A59F
		public ConcurrentHashSet(IEqualityComparer<T> comparer) : this(ConcurrentHashSet<T>.DefaultConcurrencyLevel, 31, true, comparer)
		{
		}

		// Token: 0x0600B4B3 RID: 46259 RVA: 0x0045C3B0 File Offset: 0x0045A5B0
		public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer) : this(comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this.InitializeFromCollection(collection);
		}

		// Token: 0x0600B4B4 RID: 46260 RVA: 0x0045C3CE File Offset: 0x0045A5CE
		public ConcurrentHashSet(int concurrencyLevel, IEnumerable<T> collection, IEqualityComparer<T> comparer) : this(concurrencyLevel, 31, false, comparer)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			this.InitializeFromCollection(collection);
		}

		// Token: 0x0600B4B5 RID: 46261 RVA: 0x0045C3F0 File Offset: 0x0045A5F0
		public ConcurrentHashSet(int concurrencyLevel, int capacity, IEqualityComparer<T> comparer) : this(concurrencyLevel, capacity, false, comparer)
		{
		}

		// Token: 0x0600B4B6 RID: 46262 RVA: 0x0045C3FC File Offset: 0x0045A5FC
		[PublicizedFrom(EAccessModifier.Private)]
		public ConcurrentHashSet(int concurrencyLevel, int capacity, bool growLockArray, IEqualityComparer<T> comparer)
		{
			if (concurrencyLevel < 1)
			{
				throw new ArgumentOutOfRangeException("concurrencyLevel");
			}
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			if (capacity < concurrencyLevel)
			{
				capacity = concurrencyLevel;
			}
			object[] array = new object[concurrencyLevel];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new object();
			}
			int[] countPerLock = new int[array.Length];
			ConcurrentHashSet<T>.Node[] array2 = new ConcurrentHashSet<T>.Node[capacity];
			this._tables = new ConcurrentHashSet<T>.Tables(array2, array, countPerLock);
			this._growLockArray = growLockArray;
			this._budget = array2.Length / array.Length;
			this._comparer = (comparer ?? EqualityComparer<T>.Default);
		}

		// Token: 0x0600B4B7 RID: 46263 RVA: 0x0045C496 File Offset: 0x0045A696
		public bool Add(T item)
		{
			return this.AddInternal(item, this._comparer.GetHashCode(item), true);
		}

		// Token: 0x0600B4B8 RID: 46264 RVA: 0x0045C4AC File Offset: 0x0045A6AC
		public void Clear()
		{
			int toExclusive = 0;
			try
			{
				this.AcquireAllLocks(ref toExclusive);
				ConcurrentHashSet<T>.Tables tables = new ConcurrentHashSet<T>.Tables(new ConcurrentHashSet<T>.Node[31], this._tables.Locks, new int[this._tables.CountPerLock.Length]);
				this._tables = tables;
				this._budget = Math.Max(1, tables.Buckets.Length / tables.Locks.Length);
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
		}

		// Token: 0x0600B4B9 RID: 46265 RVA: 0x0045C534 File Offset: 0x0045A734
		public bool Contains(T item)
		{
			int hashCode = this._comparer.GetHashCode(item);
			ConcurrentHashSet<T>.Tables tables = this._tables;
			int bucket = ConcurrentHashSet<T>.GetBucket(hashCode, tables.Buckets.Length);
			for (ConcurrentHashSet<T>.Node node = Volatile.Read<ConcurrentHashSet<T>.Node>(ref tables.Buckets[bucket]); node != null; node = node.Next)
			{
				if (hashCode == node.Hashcode && this._comparer.Equals(node.Item, item))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600B4BA RID: 46266 RVA: 0x0045C5A8 File Offset: 0x0045A7A8
		public bool TryRemove(T item)
		{
			int hashCode = this._comparer.GetHashCode(item);
			for (;;)
			{
				ConcurrentHashSet<T>.Tables tables = this._tables;
				int num;
				int num2;
				ConcurrentHashSet<T>.GetBucketAndLockNo(hashCode, out num, out num2, tables.Buckets.Length, tables.Locks.Length);
				object obj = tables.Locks[num2];
				lock (obj)
				{
					if (tables != this._tables)
					{
						continue;
					}
					ConcurrentHashSet<T>.Node node = null;
					for (ConcurrentHashSet<T>.Node node2 = tables.Buckets[num]; node2 != null; node2 = node2.Next)
					{
						if (hashCode == node2.Hashcode && this._comparer.Equals(node2.Item, item))
						{
							if (node == null)
							{
								Volatile.Write<ConcurrentHashSet<T>.Node>(ref tables.Buckets[num], node2.Next);
							}
							else
							{
								node.Next = node2.Next;
							}
							tables.CountPerLock[num2]--;
							return true;
						}
						node = node2;
					}
				}
				break;
			}
			Action<T> onRemovalFailure = this.OnRemovalFailure;
			if (onRemovalFailure != null)
			{
				onRemovalFailure(item);
			}
			return false;
		}

		// Token: 0x0600B4BB RID: 46267 RVA: 0x0045C6C8 File Offset: 0x0045A8C8
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x0600B4BC RID: 46268 RVA: 0x0045C6D0 File Offset: 0x0045A8D0
		public IEnumerator<T> GetEnumerator()
		{
			ConcurrentHashSet<T>.Node[] buckets = this._tables.Buckets;
			int num;
			for (int i = 0; i < buckets.Length; i = num + 1)
			{
				ConcurrentHashSet<T>.Node current;
				for (current = Volatile.Read<ConcurrentHashSet<T>.Node>(ref buckets[i]); current != null; current = current.Next)
				{
					yield return current.Item;
				}
				current = null;
				num = i;
			}
			yield break;
		}

		// Token: 0x0600B4BD RID: 46269 RVA: 0x0045C6DF File Offset: 0x0045A8DF
		[PublicizedFrom(EAccessModifier.Private)]
		public void Add(T item)
		{
			this.Add(item);
		}

		// Token: 0x1700144E RID: 5198
		// (get) Token: 0x0600B4BE RID: 46270 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public bool IsReadOnly
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return false;
			}
		}

		// Token: 0x0600B4BF RID: 46271 RVA: 0x0045C6EC File Offset: 0x0045A8EC
		[PublicizedFrom(EAccessModifier.Private)]
		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException("arrayIndex");
			}
			int toExclusive = 0;
			try
			{
				this.AcquireAllLocks(ref toExclusive);
				int num = 0;
				int num2 = 0;
				while (num2 < this._tables.Locks.Length && num >= 0)
				{
					num += this._tables.CountPerLock[num2];
					num2++;
				}
				if (array.Length - num < arrayIndex || num < 0)
				{
					throw new ArgumentException("The index is equal to or greater than the length of the array, or the number of elements in the set is greater than the available space from index to the end of the destination array.");
				}
				this.CopyToItems(array, arrayIndex);
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
		}

		// Token: 0x0600B4C0 RID: 46272 RVA: 0x0045C790 File Offset: 0x0045A990
		[PublicizedFrom(EAccessModifier.Private)]
		public bool Remove(T item)
		{
			return this.TryRemove(item);
		}

		// Token: 0x0600B4C1 RID: 46273 RVA: 0x0045C79C File Offset: 0x0045A99C
		[PublicizedFrom(EAccessModifier.Private)]
		public void InitializeFromCollection(IEnumerable<T> collection)
		{
			foreach (T t in collection)
			{
				this.AddInternal(t, this._comparer.GetHashCode(t), false);
			}
			if (this._budget == 0)
			{
				this._budget = this._tables.Buckets.Length / this._tables.Locks.Length;
			}
		}

		// Token: 0x0600B4C2 RID: 46274 RVA: 0x0045C820 File Offset: 0x0045AA20
		public bool TryFirst(out T returnValue)
		{
			if (this._tables == null || this._tables.Buckets == null || this._tables.Buckets.Length == 0)
			{
				returnValue = default(T);
				return false;
			}
			ConcurrentHashSet<T>.Node node = this._tables.Buckets.FirstOrDefault((ConcurrentHashSet<T>.Node d) => d != null);
			if (node == null)
			{
				returnValue = default(T);
				return false;
			}
			returnValue = node.Item;
			return true;
		}

		// Token: 0x0600B4C3 RID: 46275 RVA: 0x0045C8AC File Offset: 0x0045AAAC
		public bool TryRemoveFirst(out T returnValue)
		{
			if (this._tables == null || this._tables.Buckets == null || this._tables.Buckets.Length == 0)
			{
				returnValue = default(T);
				return false;
			}
			ConcurrentHashSet<T>.Node node = this._tables.Buckets.FirstOrDefault((ConcurrentHashSet<T>.Node d) => d != null);
			if (node == null)
			{
				returnValue = default(T);
				return false;
			}
			returnValue = node.Item;
			this.TryRemove(returnValue);
			return true;
		}

		// Token: 0x0600B4C4 RID: 46276 RVA: 0x0045C944 File Offset: 0x0045AB44
		[PublicizedFrom(EAccessModifier.Private)]
		public bool AddInternal(T item, int hashcode, bool acquireLock)
		{
			checked
			{
				ConcurrentHashSet<T>.Tables tables;
				bool flag;
				for (;;)
				{
					tables = this._tables;
					int num;
					int num2;
					ConcurrentHashSet<T>.GetBucketAndLockNo(hashcode, out num, out num2, tables.Buckets.Length, tables.Locks.Length);
					flag = false;
					bool flag2 = false;
					try
					{
						if (acquireLock)
						{
							Monitor.Enter(tables.Locks[num2], ref flag2);
						}
						if (tables != this._tables)
						{
							continue;
						}
						for (ConcurrentHashSet<T>.Node node = tables.Buckets[num]; node != null; node = node.Next)
						{
							if (hashcode == node.Hashcode && this._comparer.Equals(node.Item, item))
							{
								return false;
							}
						}
						Volatile.Write<ConcurrentHashSet<T>.Node>(ref tables.Buckets[num], new ConcurrentHashSet<T>.Node(item, hashcode, tables.Buckets[num]));
						tables.CountPerLock[num2]++;
						if (tables.CountPerLock[num2] > this._budget)
						{
							flag = true;
						}
					}
					finally
					{
						if (flag2)
						{
							Monitor.Exit(tables.Locks[num2]);
						}
					}
					break;
				}
				if (flag)
				{
					this.GrowTable(tables);
				}
				return true;
			}
		}

		// Token: 0x0600B4C5 RID: 46277 RVA: 0x0045CA50 File Offset: 0x0045AC50
		[PublicizedFrom(EAccessModifier.Private)]
		public static int GetBucket(int hashcode, int bucketCount)
		{
			return (hashcode & int.MaxValue) % bucketCount;
		}

		// Token: 0x0600B4C6 RID: 46278 RVA: 0x0045CA5B File Offset: 0x0045AC5B
		[PublicizedFrom(EAccessModifier.Private)]
		public static void GetBucketAndLockNo(int hashcode, out int bucketNo, out int lockNo, int bucketCount, int lockCount)
		{
			bucketNo = (hashcode & int.MaxValue) % bucketCount;
			lockNo = bucketNo % lockCount;
		}

		// Token: 0x0600B4C7 RID: 46279 RVA: 0x0045CA70 File Offset: 0x0045AC70
		[PublicizedFrom(EAccessModifier.Private)]
		public void GrowTable(ConcurrentHashSet<T>.Tables tables)
		{
			int toExclusive = 0;
			try
			{
				this.AcquireLocks(0, 1, ref toExclusive);
				if (tables == this._tables)
				{
					long num = 0L;
					for (int i = 0; i < tables.CountPerLock.Length; i++)
					{
						num += (long)tables.CountPerLock[i];
					}
					if (num < (long)(tables.Buckets.Length / 4))
					{
						this._budget = 2 * this._budget;
						if (this._budget < 0)
						{
							this._budget = int.MaxValue;
						}
					}
					else
					{
						int num2 = 0;
						bool flag = false;
						object[] array;
						checked
						{
							try
							{
								num2 = tables.Buckets.Length * 2 + 1;
								while (num2 % 3 == 0 || num2 % 5 == 0 || num2 % 7 == 0)
								{
									num2 += 2;
								}
								if (num2 > 2146435071)
								{
									flag = true;
								}
							}
							catch (OverflowException)
							{
								flag = true;
							}
							if (flag)
							{
								num2 = 2146435071;
								this._budget = int.MaxValue;
							}
							this.AcquireLocks(1, tables.Locks.Length, ref toExclusive);
							array = tables.Locks;
						}
						if (this._growLockArray && tables.Locks.Length < 1024)
						{
							array = new object[tables.Locks.Length * 2];
							Array.Copy(tables.Locks, 0, array, 0, tables.Locks.Length);
							for (int j = tables.Locks.Length; j < array.Length; j++)
							{
								array[j] = new object();
							}
						}
						ConcurrentHashSet<T>.Node[] array2 = new ConcurrentHashSet<T>.Node[num2];
						int[] array3 = new int[array.Length];
						for (int k = 0; k < tables.Buckets.Length; k++)
						{
							checked
							{
								ConcurrentHashSet<T>.Node next;
								for (ConcurrentHashSet<T>.Node node = tables.Buckets[k]; node != null; node = next)
								{
									next = node.Next;
									int num3;
									int num4;
									ConcurrentHashSet<T>.GetBucketAndLockNo(node.Hashcode, out num3, out num4, array2.Length, array.Length);
									array2[num3] = new ConcurrentHashSet<T>.Node(node.Item, node.Hashcode, array2[num3]);
									array3[num4]++;
								}
							}
						}
						this._budget = Math.Max(1, array2.Length / array.Length);
						this._tables = new ConcurrentHashSet<T>.Tables(array2, array, array3);
					}
				}
			}
			finally
			{
				this.ReleaseLocks(0, toExclusive);
			}
		}

		// Token: 0x0600B4C8 RID: 46280 RVA: 0x0045CCB0 File Offset: 0x0045AEB0
		[PublicizedFrom(EAccessModifier.Private)]
		public void AcquireAllLocks(ref int locksAcquired)
		{
			this.AcquireLocks(0, 1, ref locksAcquired);
			this.AcquireLocks(1, this._tables.Locks.Length, ref locksAcquired);
		}

		// Token: 0x0600B4C9 RID: 46281 RVA: 0x0045CCD4 File Offset: 0x0045AED4
		[PublicizedFrom(EAccessModifier.Private)]
		public void AcquireLocks(int fromInclusive, int toExclusive, ref int locksAcquired)
		{
			object[] locks = this._tables.Locks;
			for (int i = fromInclusive; i < toExclusive; i++)
			{
				bool flag = false;
				try
				{
					Monitor.Enter(locks[i], ref flag);
				}
				finally
				{
					if (flag)
					{
						locksAcquired++;
					}
				}
			}
		}

		// Token: 0x0600B4CA RID: 46282 RVA: 0x0045CD24 File Offset: 0x0045AF24
		[PublicizedFrom(EAccessModifier.Private)]
		public void ReleaseLocks(int fromInclusive, int toExclusive)
		{
			for (int i = fromInclusive; i < toExclusive; i++)
			{
				Monitor.Exit(this._tables.Locks[i]);
			}
		}

		// Token: 0x0600B4CB RID: 46283 RVA: 0x0045CD54 File Offset: 0x0045AF54
		[PublicizedFrom(EAccessModifier.Private)]
		public void CopyToItems(T[] array, int index)
		{
			foreach (ConcurrentHashSet<T>.Node node in this._tables.Buckets)
			{
				while (node != null)
				{
					array[index] = node.Item;
					index++;
					node = node.Next;
				}
			}
		}

		// Token: 0x04008D00 RID: 36096
		[PublicizedFrom(EAccessModifier.Private)]
		public const int DefaultCapacity = 31;

		// Token: 0x04008D01 RID: 36097
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MaxLockNumber = 1024;

		// Token: 0x04008D02 RID: 36098
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly IEqualityComparer<T> _comparer;

		// Token: 0x04008D03 RID: 36099
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly bool _growLockArray;

		// Token: 0x04008D04 RID: 36100
		[PublicizedFrom(EAccessModifier.Private)]
		public int _budget;

		// Token: 0x04008D05 RID: 36101
		[PublicizedFrom(EAccessModifier.Private)]
		public volatile ConcurrentHashSet<T>.Tables _tables;

		// Token: 0x04008D06 RID: 36102
		public Action<T> OnRemovalFailure;

		// Token: 0x0200178F RID: 6031
		[PublicizedFrom(EAccessModifier.Private)]
		public class Tables
		{
			// Token: 0x0600B4CC RID: 46284 RVA: 0x0045CDA1 File Offset: 0x0045AFA1
			public Tables(ConcurrentHashSet<T>.Node[] buckets, object[] locks, int[] countPerLock)
			{
				this.Buckets = buckets;
				this.Locks = locks;
				this.CountPerLock = countPerLock;
			}

			// Token: 0x04008D07 RID: 36103
			public readonly ConcurrentHashSet<T>.Node[] Buckets;

			// Token: 0x04008D08 RID: 36104
			public readonly object[] Locks;

			// Token: 0x04008D09 RID: 36105
			public volatile int[] CountPerLock;
		}

		// Token: 0x02001790 RID: 6032
		[PublicizedFrom(EAccessModifier.Private)]
		public class Node
		{
			// Token: 0x0600B4CD RID: 46285 RVA: 0x0045CDC0 File Offset: 0x0045AFC0
			public Node(T item, int hashcode, ConcurrentHashSet<T>.Node next)
			{
				this.Item = item;
				this.Hashcode = hashcode;
				this.Next = next;
			}

			// Token: 0x04008D0A RID: 36106
			public readonly T Item;

			// Token: 0x04008D0B RID: 36107
			public readonly int Hashcode;

			// Token: 0x04008D0C RID: 36108
			public volatile ConcurrentHashSet<T>.Node Next;
		}
	}
}
