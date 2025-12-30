using System;
using UnityEngine.Scripting;

// Token: 0x020008BF RID: 2239
[Preserve]
public class ObjectiveFetchKeep : ObjectiveFetch
{
	// Token: 0x06004181 RID: 16769 RVA: 0x001A7F65 File Offset: 0x001A6165
	public ObjectiveFetchKeep()
	{
		this.KeepItems = true;
	}

	// Token: 0x06004182 RID: 16770 RVA: 0x001A7F74 File Offset: 0x001A6174
	public override BaseObjective Clone()
	{
		ObjectiveFetchKeep objectiveFetchKeep = new ObjectiveFetchKeep();
		this.CopyValues(objectiveFetchKeep);
		objectiveFetchKeep.KeepItems = this.KeepItems;
		return objectiveFetchKeep;
	}
}
