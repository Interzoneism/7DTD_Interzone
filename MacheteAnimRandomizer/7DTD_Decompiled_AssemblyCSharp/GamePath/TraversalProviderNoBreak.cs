using System;
using Pathfinding;

namespace GamePath
{
	// Token: 0x020015CC RID: 5580
	[PublicizedFrom(EAccessModifier.Internal)]
	public class TraversalProviderNoBreak : ITraversalProvider
	{
		// Token: 0x0600AB4E RID: 43854 RVA: 0x00436D7B File Offset: 0x00434F7B
		public bool CanTraverse(Path path, GraphNode node)
		{
			return node.Walkable && (path.enabledTags >> (int)node.Tag & 1) != 0 && node.Penalty < 1000U;
		}

		// Token: 0x0600AB4F RID: 43855 RVA: 0x00436DA8 File Offset: 0x00434FA8
		public uint GetTraversalCost(Path path, GraphNode node)
		{
			return node.Penalty;
		}
	}
}
