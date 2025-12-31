using System;
using UnityEngine;

// Token: 0x020010CC RID: 4300
public class FaceSpriteAtCamera : MonoBehaviour
{
	// Token: 0x06008730 RID: 34608 RVA: 0x0036BDEB File Offset: 0x00369FEB
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
		if (GameManager.IsDedicatedServer)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	// Token: 0x06008731 RID: 34609 RVA: 0x0036BDEB File Offset: 0x00369FEB
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		if (GameManager.IsDedicatedServer)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	// Token: 0x06008732 RID: 34610 RVA: 0x0036BDFA File Offset: 0x00369FFA
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnEnable()
	{
		this.mainCamera = null;
	}

	// Token: 0x06008733 RID: 34611 RVA: 0x0036BE04 File Offset: 0x0036A004
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		if (this.mainCamera == null)
		{
			this.mainCamera = Camera.main;
		}
		if (this.mainCamera != null)
		{
			base.transform.LookAt(this.mainCamera.transform.position, -Vector3.up);
		}
	}

	// Token: 0x04006919 RID: 26905
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Camera mainCamera;
}
