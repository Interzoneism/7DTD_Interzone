using System;

// Token: 0x0200030E RID: 782
public class ChunkPreviewData
{
	// Token: 0x17000275 RID: 629
	// (get) Token: 0x06001625 RID: 5669 RVA: 0x000813BD File Offset: 0x0007F5BD
	// (set) Token: 0x06001626 RID: 5670 RVA: 0x000813C5 File Offset: 0x0007F5C5
	public Vector3i WorldPosition { get; set; }

	// Token: 0x17000276 RID: 630
	// (get) Token: 0x06001627 RID: 5671 RVA: 0x000813CE File Offset: 0x0007F5CE
	// (set) Token: 0x06001628 RID: 5672 RVA: 0x000813D6 File Offset: 0x0007F5D6
	public Prefab PrefabData { get; set; }

	// Token: 0x17000277 RID: 631
	// (get) Token: 0x06001629 RID: 5673 RVA: 0x000813DF File Offset: 0x0007F5DF
	// (set) Token: 0x0600162A RID: 5674 RVA: 0x000813E7 File Offset: 0x0007F5E7
	public PrefabInstance PrefabInstance { get; set; }
}
