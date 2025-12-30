using System;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001445 RID: 5189
	public class PathNode
	{
		// Token: 0x0600A103 RID: 41219 RVA: 0x003FD129 File Offset: 0x003FB329
		public PathNode(Vector2i position, float pathCost, PathNode next)
		{
			this.position = position;
			this.pathCost = pathCost;
			this.next = next;
		}

		// Token: 0x0600A104 RID: 41220 RVA: 0x0000A7E3 File Offset: 0x000089E3
		public PathNode()
		{
		}

		// Token: 0x0600A105 RID: 41221 RVA: 0x003FD146 File Offset: 0x003FB346
		public void Set(Vector2i position, float pathCost, PathNode next)
		{
			this.position = position;
			this.pathCost = pathCost;
			this.next = next;
		}

		// Token: 0x0600A106 RID: 41222 RVA: 0x003FD15D File Offset: 0x003FB35D
		public void Reset()
		{
			this.next = null;
			this.nextListElem = null;
		}

		// Token: 0x04007BFC RID: 31740
		public Vector2i position;

		// Token: 0x04007BFD RID: 31741
		public float pathCost;

		// Token: 0x04007BFE RID: 31742
		public PathNode next;

		// Token: 0x04007BFF RID: 31743
		public PathNode nextListElem;
	}
}
