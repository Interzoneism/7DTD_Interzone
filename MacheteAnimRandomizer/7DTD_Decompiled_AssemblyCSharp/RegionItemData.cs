using System;
using UnityEngine.Scripting;

// Token: 0x02000346 RID: 838
[Preserve]
public class RegionItemData
{
	// Token: 0x06001884 RID: 6276 RVA: 0x00096611 File Offset: 0x00094811
	public RegionItemData(int x, int z, int updateTime)
	{
		this.X = x;
		this.Z = z;
		this.UpdateTime = updateTime;
	}

	// Token: 0x06001885 RID: 6277 RVA: 0x0009662E File Offset: 0x0009482E
	public void Update(int x, int z, int updateTime)
	{
		this.X = x;
		this.Z = z;
		this.UpdateTime = updateTime;
	}

	// Token: 0x06001886 RID: 6278 RVA: 0x00096645 File Offset: 0x00094845
	public void Update(DynamicMeshItem item)
	{
		this.X = item.WorldPosition.x;
		this.Z = item.WorldPosition.z;
		this.UpdateTime = item.UpdateTime;
	}

	// Token: 0x04000FA8 RID: 4008
	public int X;

	// Token: 0x04000FA9 RID: 4009
	public int Z;

	// Token: 0x04000FAA RID: 4010
	public int UpdateTime;
}
