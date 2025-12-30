using System;
using System.Collections.Generic;

// Token: 0x020007E8 RID: 2024
public class CompanionGroup
{
	// Token: 0x170005E2 RID: 1506
	public EntityAlive this[int index]
	{
		get
		{
			return this.MemberList[index];
		}
	}

	// Token: 0x170005E3 RID: 1507
	// (get) Token: 0x06003A34 RID: 14900 RVA: 0x0017710B File Offset: 0x0017530B
	public int Count
	{
		get
		{
			return this.MemberList.Count;
		}
	}

	// Token: 0x06003A35 RID: 14901 RVA: 0x00177118 File Offset: 0x00175318
	public void Add(EntityAlive entity)
	{
		this.MemberList.Add(entity);
		OnCompanionGroupChanged onGroupChanged = this.OnGroupChanged;
		if (onGroupChanged == null)
		{
			return;
		}
		onGroupChanged();
	}

	// Token: 0x06003A36 RID: 14902 RVA: 0x00177136 File Offset: 0x00175336
	public void Remove(EntityAlive entity)
	{
		this.MemberList.Remove(entity);
		OnCompanionGroupChanged onGroupChanged = this.OnGroupChanged;
		if (onGroupChanged == null)
		{
			return;
		}
		onGroupChanged();
	}

	// Token: 0x06003A37 RID: 14903 RVA: 0x00177155 File Offset: 0x00175355
	public int IndexOf(EntityAlive entity)
	{
		return this.MemberList.IndexOf(entity);
	}

	// Token: 0x04002F0C RID: 12044
	public OnCompanionGroupChanged OnGroupChanged;

	// Token: 0x04002F0D RID: 12045
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityAlive> MemberList = new List<EntityAlive>();
}
