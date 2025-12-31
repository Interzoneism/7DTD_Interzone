using System;
using System.Collections.Generic;

// Token: 0x020011C6 RID: 4550
public class DynamicObjectPool<T> where T : new()
{
	// Token: 0x06008E35 RID: 36405 RVA: 0x0038FCC4 File Offset: 0x0038DEC4
	public DynamicObjectPool(int objectsPerBlock)
	{
		this.Init(objectsPerBlock, int.MaxValue);
	}

	// Token: 0x06008E36 RID: 36406 RVA: 0x0038FCD8 File Offset: 0x0038DED8
	public DynamicObjectPool(int objectsPerBlock, int maxObjects)
	{
		this.Init(objectsPerBlock, maxObjects);
	}

	// Token: 0x06008E37 RID: 36407 RVA: 0x0038FCE8 File Offset: 0x0038DEE8
	public void AllocateBlock(int numObjects)
	{
		for (int i = 0; i < numObjects; i++)
		{
			this.push(Activator.CreateInstance<T>());
		}
		this.m_numAllocatedObjects += numObjects;
	}

	// Token: 0x06008E38 RID: 36408 RVA: 0x0038FD1A File Offset: 0x0038DF1A
	public T Allocate()
	{
		if (this.m_numFreeObjects < 1)
		{
			this.AllocateBlock(this.m_numObjectsPerBlock);
		}
		this.m_numUsedObjects++;
		return this.pop();
	}

	// Token: 0x06008E39 RID: 36409 RVA: 0x0038FD48 File Offset: 0x0038DF48
	public T[] Allocate(int numToAllocate)
	{
		if (numToAllocate < 1)
		{
			return null;
		}
		T[] array = new T[numToAllocate];
		while (this.m_numFreeObjects < numToAllocate)
		{
			this.AllocateBlock(this.m_numObjectsPerBlock);
		}
		for (int i = 0; i < numToAllocate; i++)
		{
			array[i] = this.Allocate();
		}
		return array;
	}

	// Token: 0x06008E3A RID: 36410 RVA: 0x0038FD93 File Offset: 0x0038DF93
	public void Free(T obj)
	{
		this.push(obj);
		this.m_numUsedObjects--;
	}

	// Token: 0x06008E3B RID: 36411 RVA: 0x0038FDAC File Offset: 0x0038DFAC
	public void Free(T[] array)
	{
		foreach (T obj in array)
		{
			this.Free(obj);
		}
	}

	// Token: 0x06008E3C RID: 36412 RVA: 0x0038FDD8 File Offset: 0x0038DFD8
	public void Compact()
	{
		this.m_numFreeObjects = 0;
		this.m_numAllocatedObjects -= this.m_freeObjects.Count;
		this.m_freeObjects = new List<T>(this.m_numObjectsPerBlock);
	}

	// Token: 0x17000EC0 RID: 3776
	// (get) Token: 0x06008E3D RID: 36413 RVA: 0x0038FE0A File Offset: 0x0038E00A
	public int NumAllocatedObjects
	{
		get
		{
			return this.m_numAllocatedObjects;
		}
	}

	// Token: 0x17000EC1 RID: 3777
	// (get) Token: 0x06008E3E RID: 36414 RVA: 0x0038FE12 File Offset: 0x0038E012
	public int NumUsedObjects
	{
		get
		{
			return this.m_numUsedObjects;
		}
	}

	// Token: 0x17000EC2 RID: 3778
	// (get) Token: 0x06008E3F RID: 36415 RVA: 0x0038FE1A File Offset: 0x0038E01A
	public int NumFreeObjects
	{
		get
		{
			return this.m_numFreeObjects;
		}
	}

	// Token: 0x17000EC3 RID: 3779
	// (get) Token: 0x06008E40 RID: 36416 RVA: 0x0038FE22 File Offset: 0x0038E022
	public int MaxObjects
	{
		get
		{
			return this.m_maxObjects;
		}
	}

	// Token: 0x06008E41 RID: 36417 RVA: 0x0038FE2A File Offset: 0x0038E02A
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init(int objectsPerBlock, int maxObjects)
	{
		this.m_numObjectsPerBlock = objectsPerBlock;
		this.m_maxObjects = maxObjects;
		this.m_freeObjects = new List<T>(objectsPerBlock);
	}

	// Token: 0x06008E42 RID: 36418 RVA: 0x0038FE48 File Offset: 0x0038E048
	[PublicizedFrom(EAccessModifier.Private)]
	public void push(T t)
	{
		if (this.m_numFreeObjects >= this.m_freeObjects.Count)
		{
			this.m_freeObjects.Add(t);
		}
		else
		{
			this.m_freeObjects[this.m_numFreeObjects] = t;
		}
		this.m_numFreeObjects++;
	}

	// Token: 0x06008E43 RID: 36419 RVA: 0x0038FE98 File Offset: 0x0038E098
	[PublicizedFrom(EAccessModifier.Private)]
	public T pop()
	{
		if (this.m_numFreeObjects < 1)
		{
			return default(T);
		}
		List<T> freeObjects = this.m_freeObjects;
		int num = this.m_numFreeObjects - 1;
		this.m_numFreeObjects = num;
		return freeObjects[num];
	}

	// Token: 0x04006E22 RID: 28194
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_numAllocatedObjects;

	// Token: 0x04006E23 RID: 28195
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_numUsedObjects;

	// Token: 0x04006E24 RID: 28196
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_numFreeObjects;

	// Token: 0x04006E25 RID: 28197
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_numObjectsPerBlock;

	// Token: 0x04006E26 RID: 28198
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_maxObjects;

	// Token: 0x04006E27 RID: 28199
	[PublicizedFrom(EAccessModifier.Private)]
	public List<T> m_freeObjects;
}
