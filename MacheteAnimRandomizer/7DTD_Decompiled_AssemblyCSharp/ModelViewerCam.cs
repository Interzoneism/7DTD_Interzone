using System;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class ModelViewerCam : MonoBehaviour
{
	// Token: 0x06000016 RID: 22 RVA: 0x0000230C File Offset: 0x0000050C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		this.envMaterial.SetTexture("_Tex", this.envTexture[0]);
		DynamicGI.UpdateEnvironment();
		RenderSettings.skybox.SetFloat("_Rotation", (float)this.nextRotation);
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00002358 File Offset: 0x00000558
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		this.rotationX += Input.GetAxis("Mouse X") * this.cameraSensitivity * Time.deltaTime;
		this.rotationY += Input.GetAxis("Mouse Y") * this.cameraSensitivity * Time.deltaTime;
		this.rotationY = Mathf.Clamp(this.rotationY, -90f, 90f);
		base.transform.localRotation = Quaternion.AngleAxis(this.rotationX, Vector3.up);
		base.transform.localRotation *= Quaternion.AngleAxis(this.rotationY, Vector3.left);
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			base.transform.position += base.transform.forward * (this.normalMoveSpeed * this.fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			base.transform.position += base.transform.right * (this.normalMoveSpeed * this.fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
		}
		else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
		{
			base.transform.position += base.transform.forward * (this.normalMoveSpeed * this.slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
			base.transform.position += base.transform.right * (this.normalMoveSpeed * this.slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
		}
		else
		{
			base.transform.position += base.transform.forward * this.normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
			base.transform.position += base.transform.right * this.normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.E))
		{
			base.transform.position += base.transform.up * this.climbSpeed * Time.deltaTime;
		}
		if (Input.GetKey(KeyCode.Q))
		{
			base.transform.position -= base.transform.up * this.climbSpeed * Time.deltaTime;
		}
		if (Input.GetKeyDown(KeyCode.End) && Cursor.lockState == CursorLockMode.Locked)
		{
			Cursor.lockState = SoftCursor.DefaultCursorLockState;
			Cursor.visible = true;
		}
		float axis = Input.GetAxis("Mouse ScrollWheel");
		if (axis > 0f)
		{
			this.currentTexture++;
		}
		else if (axis < 0f)
		{
			this.currentTexture--;
		}
		if (this.currentTexture < 0)
		{
			this.currentTexture = this.envTexture.Length - 1;
		}
		if (this.currentTexture > this.envTexture.Length - 1)
		{
			this.currentTexture = 0;
		}
		this.envMaterial.SetTexture("_Tex", this.envTexture[this.currentTexture]);
		DynamicGI.UpdateEnvironment();
		if (Input.GetKeyDown(KeyCode.F))
		{
			this.flashlight.enabled = !this.flashlight.enabled;
		}
		if (Input.GetMouseButton(0))
		{
			this.nextRotation++;
			RenderSettings.skybox.SetFloat("_Rotation", (float)(this.nextRotation * this.skyRotationSpeed));
		}
		if (Input.GetMouseButtonDown(1))
		{
			this.toggleBool = !this.toggleBool;
			this.spheres.SetActive(this.toggleBool);
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			this.toggleBoolOff = !this.toggleBoolOff;
			this.characters.SetActive(this.toggleBoolOff);
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			this.toggleBoolAnimalsOff = !this.toggleBoolAnimalsOff;
			this.animals.SetActive(this.toggleBoolAnimalsOff);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			this.toggleBoolOffPlane = !this.toggleBoolOffPlane;
			this.plane.SetActive(this.toggleBoolOffPlane);
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			this.sunlight.enabled = !this.sunlight.enabled;
		}
		if (Input.GetKey(KeyCode.R))
		{
			this.nextSunRotation++;
			this.sunlight.transform.localEulerAngles = new Vector3(30f, (float)this.nextSunRotation, 0f);
		}
	}

	// Token: 0x04000007 RID: 7
	public float cameraSensitivity = 90f;

	// Token: 0x04000008 RID: 8
	public float climbSpeed = 4f;

	// Token: 0x04000009 RID: 9
	public float normalMoveSpeed = 10f;

	// Token: 0x0400000A RID: 10
	public float slowMoveFactor = 0.25f;

	// Token: 0x0400000B RID: 11
	public float fastMoveFactor = 3f;

	// Token: 0x0400000C RID: 12
	public Material envMaterial;

	// Token: 0x0400000D RID: 13
	public Texture[] envTexture;

	// Token: 0x0400000E RID: 14
	public int currentTexture;

	// Token: 0x0400000F RID: 15
	public Light flashlight;

	// Token: 0x04000010 RID: 16
	public Light sunlight;

	// Token: 0x04000011 RID: 17
	public GameObject spheres;

	// Token: 0x04000012 RID: 18
	public GameObject characters;

	// Token: 0x04000013 RID: 19
	public GameObject plane;

	// Token: 0x04000014 RID: 20
	public GameObject animals;

	// Token: 0x04000015 RID: 21
	public int skyRotationSpeed;

	// Token: 0x04000016 RID: 22
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int nextRotation;

	// Token: 0x04000017 RID: 23
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int nextSunRotation;

	// Token: 0x04000018 RID: 24
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float rotationX = 180f;

	// Token: 0x04000019 RID: 25
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float rotationY;

	// Token: 0x0400001A RID: 26
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool toggleBool = true;

	// Token: 0x0400001B RID: 27
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool toggleBoolOff;

	// Token: 0x0400001C RID: 28
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool toggleBoolAnimalsOff;

	// Token: 0x0400001D RID: 29
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool toggleBoolOffPlane;

	// Token: 0x0400001E RID: 30
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isPaused;
}
