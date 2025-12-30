using System;

// Token: 0x020007C1 RID: 1985
public class PackageDestinationSingleEntityID : IPackageDestinationFilter
{
	// Token: 0x06003952 RID: 14674 RVA: 0x00173484 File Offset: 0x00171684
	public PackageDestinationSingleEntityID(int _entityId)
	{
		this.entityId = _entityId;
	}

	// Token: 0x06003953 RID: 14675 RVA: 0x00173493 File Offset: 0x00171693
	public bool Exclude(ClientInfo _cInfo)
	{
		return !_cInfo.bAttachedToEntity || _cInfo.entityId != this.entityId;
	}

	// Token: 0x04002E6B RID: 11883
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;
}
