using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200003E RID: 62
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Examples/Item Database")]
public class InvDatabase : MonoBehaviour
{
	// Token: 0x17000027 RID: 39
	// (get) Token: 0x06000145 RID: 325 RVA: 0x0000DD84 File Offset: 0x0000BF84
	public static InvDatabase[] list
	{
		get
		{
			if (InvDatabase.mIsDirty)
			{
				InvDatabase.mIsDirty = false;
				InvDatabase.mList = NGUITools.FindActive<InvDatabase>();
			}
			return InvDatabase.mList;
		}
	}

	// Token: 0x06000146 RID: 326 RVA: 0x0000DDA2 File Offset: 0x0000BFA2
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		InvDatabase.mIsDirty = true;
	}

	// Token: 0x06000147 RID: 327 RVA: 0x0000DDA2 File Offset: 0x0000BFA2
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		InvDatabase.mIsDirty = true;
	}

	// Token: 0x06000148 RID: 328 RVA: 0x0000DDAC File Offset: 0x0000BFAC
	[PublicizedFrom(EAccessModifier.Private)]
	public InvBaseItem GetItem(int id16)
	{
		int i = 0;
		int count = this.items.Count;
		while (i < count)
		{
			InvBaseItem invBaseItem = this.items[i];
			if (invBaseItem.id16 == id16)
			{
				return invBaseItem;
			}
			i++;
		}
		return null;
	}

	// Token: 0x06000149 RID: 329 RVA: 0x0000DDEC File Offset: 0x0000BFEC
	[PublicizedFrom(EAccessModifier.Private)]
	public static InvDatabase GetDatabase(int dbID)
	{
		int i = 0;
		int num = InvDatabase.list.Length;
		while (i < num)
		{
			InvDatabase invDatabase = InvDatabase.list[i];
			if (invDatabase.databaseID == dbID)
			{
				return invDatabase;
			}
			i++;
		}
		return null;
	}

	// Token: 0x0600014A RID: 330 RVA: 0x0000DE24 File Offset: 0x0000C024
	public static InvBaseItem FindByID(int id32)
	{
		InvDatabase database = InvDatabase.GetDatabase(id32 >> 16);
		if (!(database != null))
		{
			return null;
		}
		return database.GetItem(id32 & 65535);
	}

	// Token: 0x0600014B RID: 331 RVA: 0x0000DE54 File Offset: 0x0000C054
	public static InvBaseItem FindByName(string exact)
	{
		int i = 0;
		int num = InvDatabase.list.Length;
		while (i < num)
		{
			InvDatabase invDatabase = InvDatabase.list[i];
			int j = 0;
			int count = invDatabase.items.Count;
			while (j < count)
			{
				InvBaseItem invBaseItem = invDatabase.items[j];
				if (invBaseItem.name == exact)
				{
					return invBaseItem;
				}
				j++;
			}
			i++;
		}
		return null;
	}

	// Token: 0x0600014C RID: 332 RVA: 0x0000DEB8 File Offset: 0x0000C0B8
	public static int FindItemID(InvBaseItem item)
	{
		int i = 0;
		int num = InvDatabase.list.Length;
		while (i < num)
		{
			InvDatabase invDatabase = InvDatabase.list[i];
			if (invDatabase.items.Contains(item))
			{
				return invDatabase.databaseID << 16 | item.id16;
			}
			i++;
		}
		return -1;
	}

	// Token: 0x040001EE RID: 494
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static InvDatabase[] mList;

	// Token: 0x040001EF RID: 495
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool mIsDirty = true;

	// Token: 0x040001F0 RID: 496
	public int databaseID;

	// Token: 0x040001F1 RID: 497
	public List<InvBaseItem> items = new List<InvBaseItem>();

	// Token: 0x040001F2 RID: 498
	public UnityEngine.Object iconAtlas;
}
