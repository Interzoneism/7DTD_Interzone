using System;
using UnityEngine;

// Token: 0x02001318 RID: 4888
public class vp_ItemIdentifier : MonoBehaviour
{
	// Token: 0x06009842 RID: 38978 RVA: 0x003C8AC4 File Offset: 0x003C6CC4
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		vp_TargetEventReturn<vp_ItemType>.Register(base.transform, "GetItemType", new Func<vp_ItemType>(this.GetItemType));
		vp_TargetEventReturn<int>.Register(base.transform, "GetItemID", new Func<int>(this.GetItemID));
	}

	// Token: 0x06009843 RID: 38979 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
	}

	// Token: 0x06009844 RID: 38980 RVA: 0x003C8B00 File Offset: 0x003C6D00
	public virtual vp_ItemType GetItemType()
	{
		return this.Type;
	}

	// Token: 0x06009845 RID: 38981 RVA: 0x003C8B08 File Offset: 0x003C6D08
	public virtual int GetItemID()
	{
		return this.ID;
	}

	// Token: 0x04007497 RID: 29847
	public vp_ItemType Type;

	// Token: 0x04007498 RID: 29848
	public int ID;
}
