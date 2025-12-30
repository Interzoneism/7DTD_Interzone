using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020015B9 RID: 5561
	[Preserve]
	public class RaycastPath
	{
		// Token: 0x17001310 RID: 4880
		// (get) Token: 0x0600AAC5 RID: 43717 RVA: 0x0043420B File Offset: 0x0043240B
		// (set) Token: 0x0600AAC6 RID: 43718 RVA: 0x00434213 File Offset: 0x00432413
		public RaycastPathInfo Info { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001311 RID: 4881
		// (get) Token: 0x0600AAC7 RID: 43719 RVA: 0x0043421C File Offset: 0x0043241C
		public Vector3 Start
		{
			get
			{
				return this.Info.Start;
			}
		}

		// Token: 0x17001312 RID: 4882
		// (get) Token: 0x0600AAC8 RID: 43720 RVA: 0x00434229 File Offset: 0x00432429
		public Vector3 Target
		{
			get
			{
				return this.Info.Target;
			}
		}

		// Token: 0x17001313 RID: 4883
		// (get) Token: 0x0600AAC9 RID: 43721 RVA: 0x00434236 File Offset: 0x00432436
		public Vector3i StartBlockPos
		{
			get
			{
				return this.Info.StartBlockPos;
			}
		}

		// Token: 0x17001314 RID: 4884
		// (get) Token: 0x0600AACA RID: 43722 RVA: 0x00434243 File Offset: 0x00432443
		public Vector3i TargetBlockPos
		{
			get
			{
				return this.Info.TargetBlockPos;
			}
		}

		// Token: 0x17001315 RID: 4885
		// (get) Token: 0x0600AACB RID: 43723 RVA: 0x00434250 File Offset: 0x00432450
		public bool PathStartsIndoors
		{
			get
			{
				return this.Info.PathStartsIndoors;
			}
		}

		// Token: 0x17001316 RID: 4886
		// (get) Token: 0x0600AACC RID: 43724 RVA: 0x0043425D File Offset: 0x0043245D
		public bool PathEndsIndoors
		{
			get
			{
				return this.Info.PathEndsIndoors;
			}
		}

		// Token: 0x0600AACD RID: 43725 RVA: 0x0043426A File Offset: 0x0043246A
		public RaycastPath(Vector3 start, Vector3 target)
		{
			this.Info = new RaycastPathInfo(start, target);
			RaycastPathManager.Instance.Add(this);
		}

		// Token: 0x0600AACE RID: 43726 RVA: 0x004342A0 File Offset: 0x004324A0
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~RaycastPath()
		{
			this.Destruct();
		}

		// Token: 0x0600AACF RID: 43727 RVA: 0x004342CC File Offset: 0x004324CC
		public void Destruct()
		{
			RaycastPathManager.Instance.Remove(this);
		}

		// Token: 0x0600AAD0 RID: 43728 RVA: 0x004342D9 File Offset: 0x004324D9
		public void AddNode(RaycastNode node)
		{
			if (!this.Nodes.Contains(node))
			{
				this.Nodes.Add(node);
			}
		}

		// Token: 0x0600AAD1 RID: 43729 RVA: 0x004342F5 File Offset: 0x004324F5
		public void AddProjectedPoint(Vector3 point)
		{
			if (!this.ProjectedPoints.Contains(point))
			{
				this.ProjectedPoints.Add(point);
			}
		}

		// Token: 0x0600AAD2 RID: 43730 RVA: 0x00434314 File Offset: 0x00432514
		public virtual void DebugDraw()
		{
			for (int i = 0; i < this.ProjectedPoints.Count - 1; i++)
			{
				Utils.DrawLine(World.blockToTransformPos(new Vector3i(this.ProjectedPoints[i] - Origin.position)), World.blockToTransformPos(new Vector3i(this.ProjectedPoints[i + 1] - Origin.position)), Color.white, Color.cyan, 2, 0f);
			}
			for (int j = 0; j < this.Nodes.Count; j++)
			{
				RaycastNode raycastNode = this.Nodes[j];
				for (int k = 0; k < raycastNode.Neighbors.Count; k++)
				{
					RaycastNode raycastNode2 = raycastNode.Neighbors[k];
					for (int l = 0; l < raycastNode2.ChildSolidBlocks.Count; l++)
					{
						RaycastPathUtils.DrawNode(raycastNode2.ChildSolidBlocks[l], Color.red, 0f);
					}
					for (int m = 0; m < raycastNode2.ChildAirBlocks.Count; m++)
					{
						RaycastPathUtils.DrawNode(raycastNode2.ChildAirBlocks[m], Color.cyan, 0f);
					}
				}
				if (raycastNode.Children.Count < 1)
				{
					cPathNodeType nodeType = raycastNode.nodeType;
					if (nodeType != cPathNodeType.Air)
					{
						if (nodeType == cPathNodeType.Door)
						{
							RaycastPathUtils.DrawNode(raycastNode, Color.green, 0f);
						}
					}
					else
					{
						RaycastPathUtils.DrawNode(raycastNode, Color.cyan, 0f);
					}
				}
				else
				{
					for (int n = 0; n < raycastNode.ChildSolidBlocks.Count; n++)
					{
						RaycastPathUtils.DrawNode(raycastNode.ChildSolidBlocks[n], Color.red, 0f);
					}
					for (int num = 0; num < raycastNode.ChildAirBlocks.Count; num++)
					{
						RaycastPathUtils.DrawNode(raycastNode.ChildAirBlocks[num], Color.cyan, 0f);
					}
				}
			}
			for (int num2 = 0; num2 < this.Nodes.Count - 1; num2++)
			{
				RaycastNode raycastNode3 = this.Nodes[num2];
				RaycastNode raycastNode4 = this.Nodes[num2 + 1];
				Utils.DrawLine(raycastNode3.Position - Origin.position, raycastNode4.Position - Origin.position, Color.white, Color.green, 2, 0f);
			}
		}

		// Token: 0x04008557 RID: 34135
		public List<RaycastNode> Nodes = new List<RaycastNode>();

		// Token: 0x04008558 RID: 34136
		public List<Vector3> ProjectedPoints = new List<Vector3>();
	}
}
