using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020015BC RID: 5564
	[Preserve]
	public class RaycastNode
	{
		// Token: 0x0600AADB RID: 43739 RVA: 0x0043461B File Offset: 0x0043281B
		public RaycastNode(Vector3 pos, float scale = 1f, int depth = 0)
		{
			this.info = new RaycastNodeInfo(pos, scale, depth);
			this.hierarchy = new RaycastNodeHierarcy();
		}

		// Token: 0x0600AADC RID: 43740 RVA: 0x0043463C File Offset: 0x0043283C
		public RaycastNode(Vector3 min, Vector3 max, float scale = 1f, int depth = 0)
		{
			this.info = new RaycastNodeInfo(min, max, scale, depth);
			this.hierarchy = new RaycastNodeHierarcy();
		}

		// Token: 0x1700131B RID: 4891
		// (get) Token: 0x0600AADD RID: 43741 RVA: 0x0043465F File Offset: 0x0043285F
		public Vector3 Position
		{
			get
			{
				return this.info.Position;
			}
		}

		// Token: 0x1700131C RID: 4892
		// (get) Token: 0x0600AADE RID: 43742 RVA: 0x0043466C File Offset: 0x0043286C
		public Vector3 Center
		{
			get
			{
				return this.info.Center;
			}
		}

		// Token: 0x1700131D RID: 4893
		// (get) Token: 0x0600AADF RID: 43743 RVA: 0x00434679 File Offset: 0x00432879
		public Vector3i BlockPos
		{
			get
			{
				return this.info.BlockPos;
			}
		}

		// Token: 0x1700131E RID: 4894
		// (get) Token: 0x0600AAE0 RID: 43744 RVA: 0x00434686 File Offset: 0x00432886
		public float Scale
		{
			get
			{
				return this.info.Scale;
			}
		}

		// Token: 0x1700131F RID: 4895
		// (get) Token: 0x0600AAE1 RID: 43745 RVA: 0x00434693 File Offset: 0x00432893
		public int Depth
		{
			get
			{
				return this.info.Depth;
			}
		}

		// Token: 0x17001320 RID: 4896
		// (get) Token: 0x0600AAE2 RID: 43746 RVA: 0x004346A0 File Offset: 0x004328A0
		public Vector3 Min
		{
			get
			{
				return this.info.Min;
			}
		}

		// Token: 0x17001321 RID: 4897
		// (get) Token: 0x0600AAE3 RID: 43747 RVA: 0x004346AD File Offset: 0x004328AD
		public Vector3 Max
		{
			get
			{
				return this.info.Max;
			}
		}

		// Token: 0x17001322 RID: 4898
		// (get) Token: 0x0600AAE4 RID: 43748 RVA: 0x004346BA File Offset: 0x004328BA
		public RaycastNode Parent
		{
			get
			{
				return this.hierarchy.parent;
			}
		}

		// Token: 0x17001323 RID: 4899
		// (get) Token: 0x0600AAE5 RID: 43749 RVA: 0x004346C7 File Offset: 0x004328C7
		public List<RaycastNode> Neighbors
		{
			get
			{
				return this.hierarchy.neighbors;
			}
		}

		// Token: 0x0600AAE6 RID: 43750 RVA: 0x004346D4 File Offset: 0x004328D4
		public void SetParent(RaycastNode node)
		{
			this.hierarchy.parent = node;
		}

		// Token: 0x0600AAE7 RID: 43751 RVA: 0x004346E2 File Offset: 0x004328E2
		public void AddNeighbor(RaycastNode node)
		{
			this.hierarchy.neighbors.Add(node);
		}

		// Token: 0x0600AAE8 RID: 43752 RVA: 0x004346F5 File Offset: 0x004328F5
		public RaycastNode GetNeighbor(Vector3 pos)
		{
			return this.hierarchy.GetNeighbor(pos);
		}

		// Token: 0x0600AAE9 RID: 43753 RVA: 0x00434703 File Offset: 0x00432903
		public void AddChild(RaycastNode node)
		{
			this.hierarchy.AddChild(node);
		}

		// Token: 0x17001324 RID: 4900
		// (get) Token: 0x0600AAEA RID: 43754 RVA: 0x00434711 File Offset: 0x00432911
		public List<RaycastNode> Children
		{
			get
			{
				return this.hierarchy.children;
			}
		}

		// Token: 0x17001325 RID: 4901
		// (get) Token: 0x0600AAEB RID: 43755 RVA: 0x0043471E File Offset: 0x0043291E
		public List<RaycastNode> ChildAirBlocks
		{
			get
			{
				return this.hierarchy.childAirBlocks;
			}
		}

		// Token: 0x17001326 RID: 4902
		// (get) Token: 0x0600AAEC RID: 43756 RVA: 0x0043472B File Offset: 0x0043292B
		public List<RaycastNode> ChildSolidBlocks
		{
			get
			{
				return this.hierarchy.childSolidBlocks;
			}
		}

		// Token: 0x17001327 RID: 4903
		// (get) Token: 0x0600AAED RID: 43757 RVA: 0x00434738 File Offset: 0x00432938
		public RaycastNode Waypoint
		{
			get
			{
				return this.hierarchy.waypoint;
			}
		}

		// Token: 0x0600AAEE RID: 43758 RVA: 0x00434745 File Offset: 0x00432945
		public void SetWaypoint(RaycastNode node)
		{
			this.hierarchy.SetWayPoint(node);
		}

		// Token: 0x17001328 RID: 4904
		// (get) Token: 0x0600AAEF RID: 43759 RVA: 0x00434753 File Offset: 0x00432953
		public bool FlowToWaypoint
		{
			get
			{
				return this.hierarchy.flowToWaypoint;
			}
		}

		// Token: 0x0600AAF0 RID: 43760 RVA: 0x00434760 File Offset: 0x00432960
		public void SetType(cPathNodeType _nodeType)
		{
			this.nodeType = _nodeType;
		}

		// Token: 0x0600AAF1 RID: 43761 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void DebugDraw()
		{
		}

		// Token: 0x04008566 RID: 34150
		[PublicizedFrom(EAccessModifier.Private)]
		public RaycastNodeInfo info;

		// Token: 0x04008567 RID: 34151
		[PublicizedFrom(EAccessModifier.Private)]
		public RaycastNodeHierarcy hierarchy;

		// Token: 0x04008568 RID: 34152
		public cPathNodeType nodeType;
	}
}
