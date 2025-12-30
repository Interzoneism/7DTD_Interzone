using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020015BF RID: 5567
	[Preserve]
	public class FloodFillPath : RaycastPath
	{
		// Token: 0x0600AAFA RID: 43770 RVA: 0x00434985 File Offset: 0x00432B85
		public FloodFillPath(Vector3 start, Vector3 target) : base(start, target)
		{
		}

		// Token: 0x0600AAFB RID: 43771 RVA: 0x004349A8 File Offset: 0x00432BA8
		public bool IsPosOpen(Vector3 pos)
		{
			return this.open.Find((FloodFillNode n) => n.Position == pos) != null;
		}

		// Token: 0x0600AAFC RID: 43772 RVA: 0x004349DC File Offset: 0x00432BDC
		public bool IsPosClosed(Vector3 pos)
		{
			return this.closed.Find((FloodFillNode n) => n.Position == pos) != null;
		}

		// Token: 0x0600AAFD RID: 43773 RVA: 0x00434A10 File Offset: 0x00432C10
		public FloodFillNode getLowestScore()
		{
			FloodFillNode result = null;
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			for (int i = 0; i < this.open.Count; i++)
			{
				FloodFillNode floodFillNode = this.open[i];
				if (floodFillNode.F <= num && floodFillNode.Heuristic < num2)
				{
					result = floodFillNode;
					num = floodFillNode.F;
					num2 = floodFillNode.Heuristic;
				}
			}
			return result;
		}

		// Token: 0x0600AAFE RID: 43774 RVA: 0x00434A78 File Offset: 0x00432C78
		public override void DebugDraw()
		{
			for (int i = 0; i < this.closed.Count; i++)
			{
				FloodFillNode floodFillNode = this.closed[i];
				if (floodFillNode.nodeType == cPathNodeType.Air)
				{
					RaycastPathUtils.DrawBounds(floodFillNode.BlockPos, Color.yellow, 0f, 1f);
				}
			}
			base.DebugDraw();
		}

		// Token: 0x04008577 RID: 34167
		public List<FloodFillNode> open = new List<FloodFillNode>();

		// Token: 0x04008578 RID: 34168
		public List<FloodFillNode> closed = new List<FloodFillNode>();
	}
}
