using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000B00 RID: 2816
public interface IPowered
{
	// Token: 0x06005704 RID: 22276
	Vector3i GetParent();

	// Token: 0x06005705 RID: 22277
	PowerItem GetPowerItem();

	// Token: 0x06005706 RID: 22278
	void DrawWires();

	// Token: 0x06005707 RID: 22279
	void RemoveWires();

	// Token: 0x06005708 RID: 22280
	void MarkWireDirty();

	// Token: 0x06005709 RID: 22281
	void MarkChanged();

	// Token: 0x0600570A RID: 22282
	void AddWireData(Vector3i child);

	// Token: 0x0600570B RID: 22283
	Vector3 GetWireOffset();

	// Token: 0x0600570C RID: 22284
	int GetRequiredPower();

	// Token: 0x0600570D RID: 22285
	bool CanHaveParent(IPowered newParent);

	// Token: 0x0600570E RID: 22286
	void SetParentWithWireTool(IPowered parent, int entityID);

	// Token: 0x0600570F RID: 22287
	void RemoveParentWithWiringTool(int wiringEntityID);

	// Token: 0x06005710 RID: 22288
	void SetWireData(List<Vector3i> wireChildren);

	// Token: 0x06005711 RID: 22289
	void SendWireData();

	// Token: 0x06005712 RID: 22290
	void CreateWireDataFromPowerItem();
}
