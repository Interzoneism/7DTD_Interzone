using System;
using UnityEngine;

// Token: 0x02001319 RID: 4889
[Serializable]
public class vp_ItemInstance
{
	// Token: 0x06009847 RID: 38983 RVA: 0x003C8B10 File Offset: 0x003C6D10
	[SerializeField]
	public vp_ItemInstance(vp_ItemType type, int id)
	{
		this.ID = id;
		this.Type = type;
	}

	// Token: 0x06009848 RID: 38984 RVA: 0x003C8B26 File Offset: 0x003C6D26
	public virtual void SetUniqueID()
	{
		this.ID = vp_Utility.UniqueID;
	}

	// Token: 0x04007499 RID: 29849
	[SerializeField]
	public vp_ItemType Type;

	// Token: 0x0400749A RID: 29850
	[SerializeField]
	public int ID;
}
