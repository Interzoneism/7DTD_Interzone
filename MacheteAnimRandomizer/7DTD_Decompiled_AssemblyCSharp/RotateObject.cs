using System;
using UnityEngine;

// Token: 0x020004D0 RID: 1232
public class RotateObject : MonoBehaviour
{
	// Token: 0x0600281A RID: 10266 RVA: 0x00104B1B File Offset: 0x00102D1B
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		if (!this.rotateTransform)
		{
			this.rotateTransform = base.transform;
		}
	}

	// Token: 0x0600281B RID: 10267 RVA: 0x00104B36 File Offset: 0x00102D36
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		this.rotateTransform.localEulerAngles += this.RPM * (Time.deltaTime * 6f);
	}

	// Token: 0x04001ED9 RID: 7897
	public Transform rotateTransform;

	// Token: 0x04001EDA RID: 7898
	public Vector3 RPM;
}
