using System;
using UnityEngine;

// Token: 0x020010CE RID: 4302
public interface IWireNode
{
	// Token: 0x06008750 RID: 34640
	Vector3 GetStartPosition();

	// Token: 0x06008751 RID: 34641
	Vector3 GetStartPositionOffset();

	// Token: 0x06008752 RID: 34642
	void SetStartPosition(Vector3 pos);

	// Token: 0x06008753 RID: 34643
	void SetStartPositionOffset(Vector3 pos);

	// Token: 0x06008754 RID: 34644
	Vector3 GetEndPosition();

	// Token: 0x06008755 RID: 34645
	Vector3 GetEndPositionOffset();

	// Token: 0x06008756 RID: 34646
	void SetEndPosition(Vector3 pos);

	// Token: 0x06008757 RID: 34647
	void SetEndPositionOffset(Vector3 pos);

	// Token: 0x06008758 RID: 34648
	void SetWireDip(float _dist);

	// Token: 0x06008759 RID: 34649
	float GetWireDip();

	// Token: 0x0600875A RID: 34650
	void SetWireRadius(float _radius);

	// Token: 0x0600875B RID: 34651
	void SetWireCanHide(bool _canHide);

	// Token: 0x0600875C RID: 34652
	void SetVisible(bool _visible);

	// Token: 0x0600875D RID: 34653
	void Reset();

	// Token: 0x0600875E RID: 34654
	void BuildMesh();

	// Token: 0x0600875F RID: 34655
	void SetPulseColor(Color color);

	// Token: 0x06008760 RID: 34656
	void TogglePulse(bool isOn);

	// Token: 0x06008761 RID: 34657
	GameObject GetGameObject();

	// Token: 0x06008762 RID: 34658
	Bounds GetBounds();
}
