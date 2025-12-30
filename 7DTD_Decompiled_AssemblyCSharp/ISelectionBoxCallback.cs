using System;
using UnityEngine;

// Token: 0x02000963 RID: 2403
public interface ISelectionBoxCallback
{
	// Token: 0x0600488A RID: 18570
	bool OnSelectionBoxActivated(string _category, string _name, bool _bActivated);

	// Token: 0x0600488B RID: 18571
	void OnSelectionBoxMoved(string _category, string _name, Vector3 _moveVector);

	// Token: 0x0600488C RID: 18572
	void OnSelectionBoxSized(string _category, string _name, int _dTop, int _dBottom, int _dNorth, int _dSouth, int _dEast, int _dWest);

	// Token: 0x0600488D RID: 18573
	void OnSelectionBoxMirrored(Vector3i _axis);

	// Token: 0x0600488E RID: 18574
	bool OnSelectionBoxDelete(string _category, string _name);

	// Token: 0x0600488F RID: 18575
	bool OnSelectionBoxIsAvailable(string _category, EnumSelectionBoxAvailabilities _criteria);

	// Token: 0x06004890 RID: 18576
	void OnSelectionBoxShowProperties(bool _bVisible, GUIWindowManager _windowManager);

	// Token: 0x06004891 RID: 18577
	void OnSelectionBoxRotated(string _category, string _name);
}
