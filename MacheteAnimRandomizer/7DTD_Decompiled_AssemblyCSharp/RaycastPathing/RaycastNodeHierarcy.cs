using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020015BE RID: 5566
	[Preserve]
	public class RaycastNodeHierarcy
	{
		// Token: 0x0600AAF4 RID: 43764 RVA: 0x00434868 File Offset: 0x00432A68
		public void SetWayPoint(RaycastNode node)
		{
			this.flowToWaypoint = true;
			this.waypoint = node;
		}

		// Token: 0x0600AAF5 RID: 43765 RVA: 0x00434878 File Offset: 0x00432A78
		public void SetParent(RaycastNode node)
		{
			this.parent = node;
		}

		// Token: 0x0600AAF6 RID: 43766 RVA: 0x00434881 File Offset: 0x00432A81
		public void AddNeighbor(RaycastNode node)
		{
			if (!this.neighbors.Contains(node))
			{
				this.neighbors.Add(node);
			}
		}

		// Token: 0x0600AAF7 RID: 43767 RVA: 0x004348A0 File Offset: 0x00432AA0
		public RaycastNode GetNeighbor(Vector3 pos)
		{
			for (int i = 0; i < this.neighbors.Count; i++)
			{
				if (this.neighbors[i].Center == pos)
				{
					return this.neighbors[i];
				}
			}
			return null;
		}

		// Token: 0x0600AAF8 RID: 43768 RVA: 0x004348EC File Offset: 0x00432AEC
		public void AddChild(RaycastNode node)
		{
			if (!this.children.Contains(node))
			{
				this.children.Add(node);
				if (node.nodeType == cPathNodeType.Air)
				{
					if (!this.childAirBlocks.Contains(node))
					{
						this.childAirBlocks.Add(node);
						return;
					}
				}
				else if (!this.childSolidBlocks.Contains(node))
				{
					this.childSolidBlocks.Add(node);
				}
			}
		}

		// Token: 0x04008570 RID: 34160
		public RaycastNode parent;

		// Token: 0x04008571 RID: 34161
		public List<RaycastNode> neighbors = new List<RaycastNode>();

		// Token: 0x04008572 RID: 34162
		public List<RaycastNode> children = new List<RaycastNode>();

		// Token: 0x04008573 RID: 34163
		public List<RaycastNode> childAirBlocks = new List<RaycastNode>();

		// Token: 0x04008574 RID: 34164
		public List<RaycastNode> childSolidBlocks = new List<RaycastNode>();

		// Token: 0x04008575 RID: 34165
		public bool flowToWaypoint;

		// Token: 0x04008576 RID: 34166
		public RaycastNode waypoint;
	}
}
