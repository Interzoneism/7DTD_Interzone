using System;

// Token: 0x020005A8 RID: 1448
public interface IMeshGenerator
{
	// Token: 0x06002EA6 RID: 11942
	bool IsLayerEmpty(int _layerIdx);

	// Token: 0x06002EA7 RID: 11943
	bool IsLayerEmpty(int _startLayerIdx, int _endLayerIdx);
}
