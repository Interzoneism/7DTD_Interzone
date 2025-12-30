using System;
using UnityEngine;

// Token: 0x0200038B RID: 907
public interface IPowerSystemCamera
{
	// Token: 0x06001AFF RID: 6911
	void SetPitch(float pitch);

	// Token: 0x06001B00 RID: 6912
	void SetYaw(float yaw);

	// Token: 0x06001B01 RID: 6913
	float GetPitch();

	// Token: 0x06001B02 RID: 6914
	float GetYaw();

	// Token: 0x06001B03 RID: 6915
	Transform GetCameraTransform();

	// Token: 0x06001B04 RID: 6916
	void SetUserAccessing(bool userAccessing);

	// Token: 0x06001B05 RID: 6917
	bool HasCone();

	// Token: 0x06001B06 RID: 6918
	void SetConeColor(Color _color);

	// Token: 0x06001B07 RID: 6919
	Color GetOriginalConeColor();

	// Token: 0x06001B08 RID: 6920
	void SetConeActive(bool _active);

	// Token: 0x06001B09 RID: 6921
	bool GetConeActive();

	// Token: 0x06001B0A RID: 6922
	bool HasLaser();

	// Token: 0x06001B0B RID: 6923
	void SetLaserColor(Color _color);

	// Token: 0x06001B0C RID: 6924
	Color GetOriginalLaserColor();

	// Token: 0x06001B0D RID: 6925
	void SetLaserActive(bool _active);

	// Token: 0x06001B0E RID: 6926
	bool GetLaserActive();
}
