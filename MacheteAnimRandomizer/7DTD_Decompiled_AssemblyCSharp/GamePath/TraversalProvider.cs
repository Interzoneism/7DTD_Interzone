using System;
using Pathfinding;

namespace GamePath
{
	// Token: 0x020015CB RID: 5579
	[PublicizedFrom(EAccessModifier.Internal)]
	public class TraversalProvider : ITraversalProvider
	{
		// Token: 0x0600AB4B RID: 43851 RVA: 0x00436D48 File Offset: 0x00434F48
		public bool CanTraverse(Path path, GraphNode node)
		{
			return node.Walkable && (path.enabledTags >> (int)node.Tag & 1) != 0;
		}

		// Token: 0x0600AB4C RID: 43852 RVA: 0x00436D69 File Offset: 0x00434F69
		public uint GetTraversalCost(Path path, GraphNode node)
		{
			return (uint)(node.Penalty * path.CostScale);
		}
	}
}
