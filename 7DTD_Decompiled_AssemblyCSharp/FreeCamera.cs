using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class FreeCamera : MonoBehaviour
{
	// Token: 0x0600008E RID: 142 RVA: 0x000091A7 File Offset: 0x000073A7
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.targetPosition = base.transform.position;
		this.targetRotation = base.transform.rotation;
	}

	// Token: 0x0600008F RID: 143 RVA: 0x000091CC File Offset: 0x000073CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (!Input.GetMouseButtonDown(1) && Input.GetMouseButton(1))
		{
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				Vector3 eulerAngles = this.light.transform.eulerAngles;
				eulerAngles.y += Input.GetAxis("Mouse X") * this.turnSpeed;
				eulerAngles.x -= Input.GetAxis("Mouse Y") * this.turnSpeed;
				this.light.transform.rotation = Quaternion.Euler(eulerAngles);
			}
			else
			{
				Vector3 eulerAngles2 = this.targetRotation.eulerAngles;
				eulerAngles2.y += Input.GetAxis("Mouse X") * this.turnSpeed;
				eulerAngles2.x -= Input.GetAxis("Mouse Y") * this.turnSpeed;
				this.targetRotation = Quaternion.Euler(eulerAngles2);
			}
		}
		if (Input.GetMouseButton(2))
		{
			float d = Input.GetAxis("Mouse X") * this.panSpeed * Time.deltaTime;
			float d2 = Input.GetAxis("Mouse Y") * this.panSpeed * Time.deltaTime;
			this.targetPosition -= base.transform.right * d + base.transform.up * d2;
		}
		float d3 = Input.GetKey(KeyCode.Q) ? (this.moveSpeed * Time.deltaTime) : 0f;
		float d4 = Input.GetKey(KeyCode.E) ? (this.moveSpeed * Time.deltaTime) : 0f;
		float d5 = Input.GetAxis("Horizontal") * this.moveSpeed * Time.deltaTime;
		float d6 = Input.GetAxis("Vertical") * this.moveSpeed * Time.deltaTime;
		float d7 = Input.GetKey(KeyCode.LeftShift) ? this.shiftSpeed : 1f;
		this.targetPosition += d7 * (base.transform.right * d5 + base.transform.forward * d6 + base.transform.up * d3 - base.transform.up * d4);
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (Input.GetMouseButton(1))
		{
			this.targetPosition += base.transform.forward * axis * this.zoomSpeed;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, Time.deltaTime * this.moveSmoothing);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, Time.deltaTime * this.turnSmoothing);
	}

	// Token: 0x040000C4 RID: 196
	public Light light;

	// Token: 0x040000C5 RID: 197
	public float moveSpeed = 10f;

	// Token: 0x040000C6 RID: 198
	public float turnSpeed = 4f;

	// Token: 0x040000C7 RID: 199
	public float zoomSpeed = 10f;

	// Token: 0x040000C8 RID: 200
	public float panSpeed = 10f;

	// Token: 0x040000C9 RID: 201
	public float shiftSpeed = 4f;

	// Token: 0x040000CA RID: 202
	public float moveSmoothing = 5f;

	// Token: 0x040000CB RID: 203
	public float turnSmoothing = 5f;

	// Token: 0x040000CC RID: 204
	public float zoomSmoothing = 5f;

	// Token: 0x040000CD RID: 205
	public float panSmoothing = 5f;

	// Token: 0x040000CE RID: 206
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 targetPosition;

	// Token: 0x040000CF RID: 207
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion targetRotation;

	// Token: 0x040000D0 RID: 208
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isMouseInWindow = true;
}
