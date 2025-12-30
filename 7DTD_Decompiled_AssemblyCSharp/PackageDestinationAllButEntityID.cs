using System;

// Token: 0x020007C2 RID: 1986
public class PackageDestinationAllButEntityID : IPackageDestinationFilter
{
	// Token: 0x06003954 RID: 14676 RVA: 0x001734B0 File Offset: 0x001716B0
	public PackageDestinationAllButEntityID(int _excludedEntityId)
	{
		this.entityId = _excludedEntityId;
	}

	// Token: 0x06003955 RID: 14677 RVA: 0x001734BF File Offset: 0x001716BF
	public bool Exclude(ClientInfo _cInfo)
	{
		return !_cInfo.bAttachedToEntity || _cInfo.entityId == this.entityId;
	}

	// Token: 0x04002E6C RID: 11884
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;
}
