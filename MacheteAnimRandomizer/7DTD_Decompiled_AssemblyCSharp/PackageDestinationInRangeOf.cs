using System;

// Token: 0x020007C4 RID: 1988
public class PackageDestinationInRangeOf : IPackageDestinationFilter
{
	// Token: 0x06003958 RID: 14680 RVA: 0x001734E4 File Offset: 0x001716E4
	public PackageDestinationInRangeOf(int _entityId, int _range)
	{
		this.entityId = _entityId;
		this.range = _range;
	}

	// Token: 0x06003959 RID: 14681 RVA: 0x001734FA File Offset: 0x001716FA
	public bool Exclude(ClientInfo _cInfo)
	{
		return !GameManager.Instance.World.IsEntityInRange(this.entityId, _cInfo.entityId, this.range);
	}

	// Token: 0x04002E6D RID: 11885
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002E6E RID: 11886
	[PublicizedFrom(EAccessModifier.Private)]
	public int range;
}
