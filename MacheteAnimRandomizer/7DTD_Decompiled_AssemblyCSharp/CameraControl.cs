using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020010C3 RID: 4291
public class CameraControl : MonoBehaviour
{
	// Token: 0x06008713 RID: 34579 RVA: 0x0036AD3C File Offset: 0x00368F3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.originalRotation = base.transform.localRotation;
		this.textObject.GetComponentInChildren<Text>().text = "PAUSED";
		this.cameraLight = base.transform.GetComponent<Light>();
		this.cameraLight.enabled = false;
	}

	// Token: 0x06008714 RID: 34580 RVA: 0x0036AD8C File Offset: 0x00368F8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			this.bPaused = !this.bPaused;
		}
		this.textObject.SetActive(this.bPaused);
		if (this.bPaused)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.F))
		{
			this.cameraLight.enabled = !this.cameraLight.enabled;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis > 0f)
		{
			this.cameraLight.spotAngle += 3f;
		}
		else if (axis < 0f)
		{
			this.cameraLight.spotAngle -= 3f;
		}
		Vector3 a = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			a += base.transform.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			a -= base.transform.forward;
		}
		if (Input.GetKey(KeyCode.A))
		{
			a -= base.transform.right;
		}
		if (Input.GetKey(KeyCode.D))
		{
			a += base.transform.right;
		}
		if (Input.GetKey(KeyCode.Space))
		{
			a += base.transform.up;
		}
		if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
		{
			a -= base.transform.up;
		}
		float d = Input.GetKey(KeyCode.LeftShift) ? (this.speed * 2f) : this.speed;
		base.transform.position += a * d;
		this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX;
		this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY;
		Quaternion rhs = Quaternion.AngleAxis(this.rotationX, Vector3.up);
		Quaternion rhs2 = Quaternion.AngleAxis(this.rotationY, -Vector3.right);
		base.transform.localRotation = this.originalRotation * rhs * rhs2;
	}

	// Token: 0x040068DA RID: 26842
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion originalRotation;

	// Token: 0x040068DB RID: 26843
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Light cameraLight;

	// Token: 0x040068DC RID: 26844
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float rotationX;

	// Token: 0x040068DD RID: 26845
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float rotationY;

	// Token: 0x040068DE RID: 26846
	public float sensitivityX = 2f;

	// Token: 0x040068DF RID: 26847
	public float sensitivityY = 2f;

	// Token: 0x040068E0 RID: 26848
	public float speed = 0.1f;

	// Token: 0x040068E1 RID: 26849
	public GameObject textObject;

	// Token: 0x040068E2 RID: 26850
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bPaused;
}
