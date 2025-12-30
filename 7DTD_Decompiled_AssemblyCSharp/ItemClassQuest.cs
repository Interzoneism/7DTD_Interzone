using System;
using UnityEngine.Scripting;

// Token: 0x02000569 RID: 1385
[Preserve]
public class ItemClassQuest : ItemClass
{
	// Token: 0x06002CD0 RID: 11472 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanDrop(ItemValue _iv = null)
	{
		return false;
	}

	// Token: 0x06002CD1 RID: 11473 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanStack()
	{
		return false;
	}

	// Token: 0x06002CD2 RID: 11474 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool KeepOnDeath()
	{
		return true;
	}

	// Token: 0x06002CD3 RID: 11475 RVA: 0x0012AF2F File Offset: 0x0012912F
	public new static void Cleanup()
	{
		ItemClassQuest.questItemList = null;
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanPlaceInContainer()
	{
		return false;
	}

	// Token: 0x06002CD5 RID: 11477 RVA: 0x0012AF37 File Offset: 0x00129137
	public static ItemClassQuest GetItemQuestById(ushort _questTypeID)
	{
		if (ItemClassQuest.questItemList == null)
		{
			return null;
		}
		if (_questTypeID < 0 || (int)_questTypeID >= ItemClassQuest.questItemList.Length)
		{
			return null;
		}
		return ItemClassQuest.questItemList[(int)_questTypeID];
	}

	// Token: 0x04002376 RID: 9078
	public static ItemClassQuest[] questItemList = new ItemClassQuest[100];
}
