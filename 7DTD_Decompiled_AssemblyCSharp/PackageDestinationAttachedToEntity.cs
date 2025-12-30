using System;

// Token: 0x020007C3 RID: 1987
public class PackageDestinationAttachedToEntity : IPackageDestinationFilter
{
	// Token: 0x06003957 RID: 14679 RVA: 0x001734D9 File Offset: 0x001716D9
	public bool Exclude(ClientInfo _cInfo)
	{
		return !_cInfo.bAttachedToEntity;
	}
}
