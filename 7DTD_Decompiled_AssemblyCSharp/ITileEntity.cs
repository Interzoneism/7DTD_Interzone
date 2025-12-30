using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AEE RID: 2798
public interface ITileEntity
{
	// Token: 0x14000082 RID: 130
	// (add) Token: 0x060055F2 RID: 22002
	// (remove) Token: 0x060055F3 RID: 22003
	event XUiEvent_TileEntityDestroyed Destroyed;

	// Token: 0x17000878 RID: 2168
	// (get) Token: 0x060055F4 RID: 22004
	List<ITileEntityChangedListener> listeners { get; }

	// Token: 0x060055F5 RID: 22005
	void SetUserAccessing(bool _bUserAccessing);

	// Token: 0x060055F6 RID: 22006
	bool IsUserAccessing();

	// Token: 0x060055F7 RID: 22007
	void SetModified();

	// Token: 0x060055F8 RID: 22008
	Chunk GetChunk();

	// Token: 0x060055F9 RID: 22009
	Vector3i ToWorldPos();

	// Token: 0x060055FA RID: 22010
	Vector3 ToWorldCenterPos();

	// Token: 0x17000879 RID: 2169
	// (get) Token: 0x060055FB RID: 22011
	BlockValue blockValue { get; }

	// Token: 0x060055FC RID: 22012
	int GetClrIdx();

	// Token: 0x1700087A RID: 2170
	// (get) Token: 0x060055FD RID: 22013
	int EntityId { get; }

	// Token: 0x1700087B RID: 2171
	// (get) Token: 0x060055FE RID: 22014
	// (set) Token: 0x060055FF RID: 22015
	bool IsRemoving { get; set; }
}
