using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000952 RID: 2386
public class CharacterGazeController : MonoBehaviour
{
	// Token: 0x17000791 RID: 1937
	// (get) Token: 0x0600480D RID: 18445 RVA: 0x001C2F94 File Offset: 0x001C1194
	public Transform LookAtTarget
	{
		get
		{
			if (this.lookAtTarget == null)
			{
				this.lookAtTarget = new GameObject("LookAtTarget").transform;
				this.lookAtTarget.parent = this.rootTransform;
				this.lookAtTarget.localPosition = this.lookAtTargetDefaultPosition;
				return this.lookAtTarget;
			}
			return this.lookAtTarget;
		}
	}

	// Token: 0x17000792 RID: 1938
	// (get) Token: 0x0600480E RID: 18446 RVA: 0x001C2FF4 File Offset: 0x001C11F4
	public GameRandom Random
	{
		get
		{
			if (this.random != null)
			{
				return this.random;
			}
			return this.random = GameRandomManager.Instance.CreateGameRandom();
		}
	}

	// Token: 0x0600480F RID: 18447 RVA: 0x001C3024 File Offset: 0x001C1224
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.entityAlive = base.GetComponentInParent<EntityAlive>();
		if (this.eyeMaterial == null || this.eyeMaterial.shader.name != "Game/SDCS/Eye")
		{
			Debug.LogError("Eye Material is not valid");
			base.enabled = false;
			return;
		}
		this.gazeTimer = Time.realtimeSinceStartup + this.Random.RandomRange(0.25f, 2f);
	}

	// Token: 0x06004810 RID: 18448 RVA: 0x001C309A File Offset: 0x001C129A
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		CharacterGazeController.instances.Add(this);
	}

	// Token: 0x06004811 RID: 18449 RVA: 0x001C30A7 File Offset: 0x001C12A7
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		if (CharacterGazeController.instances.Contains(this))
		{
			CharacterGazeController.instances.Remove(this);
		}
	}

	// Token: 0x06004812 RID: 18450 RVA: 0x001C30C2 File Offset: 0x001C12C2
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		if (CharacterGazeController.instances.Contains(this))
		{
			CharacterGazeController.instances.Remove(this);
		}
		if (this.lookAtTarget != null)
		{
			UnityEngine.Object.Destroy(this.lookAtTarget.gameObject);
		}
	}

	// Token: 0x06004813 RID: 18451 RVA: 0x001C30FC File Offset: 0x001C12FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (this.entityAlive != null)
		{
			if (this.entityAlive.emodel.IsRagdollActive)
			{
				return;
			}
			this.isDead = this.entityAlive.IsDead();
		}
		this.UpdateLookAtTarget();
		this.UpdateHeadRotation();
		this.UpdateEyeGaze();
	}

	// Token: 0x06004814 RID: 18452 RVA: 0x001C3150 File Offset: 0x001C1350
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLookAtTarget()
	{
		CharacterGazeController characterGazeController = null;
		float num = float.PositiveInfinity;
		for (int i = CharacterGazeController.instances.Count - 1; i >= 0; i--)
		{
			CharacterGazeController characterGazeController2 = CharacterGazeController.instances[i];
			if (characterGazeController2 == null)
			{
				CharacterGazeController.instances.RemoveAt(i);
			}
			else if (characterGazeController2.enabled && characterGazeController2 != this)
			{
				Vector3 to = characterGazeController2.headTransform.position - this.headTransform.position;
				float num2 = Vector3.Angle(this.headTransform.forward, to);
				float magnitude = to.magnitude;
				if (num2 < this.eyeLookAtTargetAngle && magnitude <= this.maxLookAtDistance && num2 < num)
				{
					characterGazeController = characterGazeController2;
					num = num2;
				}
			}
		}
		if (characterGazeController != null)
		{
			this.lookAtCamera = true;
			this.LookAtTarget.position = characterGazeController.headTransform.position;
		}
		else
		{
			EntityPlayerLocal entityPlayerLocal;
			if (GameManager.Instance != null && GameManager.Instance.World != null)
			{
				entityPlayerLocal = GameManager.Instance.World.GetPrimaryPlayer();
			}
			else
			{
				entityPlayerLocal = null;
			}
			Vector3 vector = this.rootTransform.TransformPoint(this.lookAtTargetDefaultPosition);
			if (entityPlayerLocal != null && entityPlayerLocal.cameraTransform != null)
			{
				vector = entityPlayerLocal.cameraTransform.position;
			}
			else if (Camera.main != null)
			{
				vector = Camera.main.transform.position;
			}
			Vector3 to2 = vector - this.headTransform.position;
			float num3 = Vector3.Angle(this.headTransform.forward, to2);
			float magnitude2 = to2.magnitude;
			if (num3 < this.eyeLookAtTargetAngle && magnitude2 <= this.maxLookAtDistance)
			{
				this.lookAtCamera = true;
				this.LookAtTarget.position = vector;
			}
			else
			{
				this.lookAtCamera = false;
				this.LookAtTarget.localPosition = this.lookAtTargetDefaultPosition;
			}
		}
		if (Time.realtimeSinceStartup > this.gazeTimer)
		{
			this.lookatOffsetIndex = this.Random.RandomRange(0, this.lookatOffsets.Count);
			this.gazeTimer = Time.realtimeSinceStartup + this.Random.RandomRange(0.25f, 2f);
		}
	}

	// Token: 0x06004815 RID: 18453 RVA: 0x001C337B File Offset: 0x001C157B
	public void SnapNextUpdate()
	{
		this.shouldSnapHeadNextUpdate = true;
		this.shouldSnapEyesNextUpdate = true;
	}

	// Token: 0x06004816 RID: 18454 RVA: 0x001C338C File Offset: 0x001C158C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateHeadRotation()
	{
		Vector3 normalized = (this.LookAtTarget.position - this.headTransform.position).normalized;
		Vector3 normalized2 = (normalized + this.headTransform.forward).normalized;
		if (Vector3.Angle(this.headTransform.forward, normalized) < this.headLookAtTargetAngle && !this.isDead)
		{
			if (this.shouldSnapHeadNextUpdate)
			{
				this.currentHeadRotation = Quaternion.FromToRotation(this.headTransform.forward, normalized2);
				this.shouldSnapHeadNextUpdate = false;
			}
			else
			{
				this.currentHeadRotation = Quaternion.Slerp(this.currentHeadRotation, Quaternion.FromToRotation(this.headTransform.forward, normalized2), this.headRotationSpeed * Time.deltaTime);
			}
		}
		else if (this.shouldSnapHeadNextUpdate)
		{
			this.currentHeadRotation = Quaternion.identity;
			this.shouldSnapHeadNextUpdate = false;
		}
		else
		{
			this.currentHeadRotation = Quaternion.Slerp(this.currentHeadRotation, Quaternion.identity, this.headRotationSpeed * Time.deltaTime);
		}
		this.headTransform.rotation = this.currentHeadRotation * this.headTransform.rotation;
	}

	// Token: 0x06004817 RID: 18455 RVA: 0x001C34B0 File Offset: 0x001C16B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateEyeGaze()
	{
		Vector3 localPosition = this.LookAtTarget.localPosition;
		if (!this.lookAtCamera)
		{
			this.LookAtTarget.position += this.lookatOffsets[this.lookatOffsetIndex];
		}
		Vector3 toDirection = this.LookAtTarget.position - this.headTransform.TransformPoint(this.leftEyeLocalPosition);
		Vector3 toDirection2 = this.LookAtTarget.position - this.headTransform.TransformPoint(this.rightEyeLocalPosition);
		Quaternion b = Quaternion.FromToRotation(this.leftEyeTransform.forward, toDirection);
		Quaternion b2 = Quaternion.FromToRotation(this.rightEyeTransform.forward, toDirection2);
		float num = Mathf.Sin(Time.time * this.twitchSpeed) * 0.5f + 0.5f;
		if (this.isDead)
		{
			b = Quaternion.identity;
			b2 = Quaternion.identity;
		}
		if (this.shouldSnapEyesNextUpdate)
		{
			this.currentLeftEyeRotation = b;
			this.currentRightEyeRotation = b2;
			this.shouldSnapEyesNextUpdate = false;
		}
		else
		{
			this.currentLeftEyeRotation = Quaternion.Slerp(this.currentLeftEyeRotation, b, this.eyeRotationSpeed * num * Time.deltaTime);
			this.currentRightEyeRotation = Quaternion.Slerp(this.currentRightEyeRotation, b2, this.eyeRotationSpeed * num * Time.deltaTime);
		}
		this.currentLeftEyeRotation.x = Utils.FastClamp(this.currentLeftEyeRotation.x, -0.2f, 0.2f);
		this.currentLeftEyeRotation.y = Utils.FastClamp(this.currentLeftEyeRotation.y, -0.4f, 0.4f);
		this.currentRightEyeRotation.x = Utils.FastClamp(this.currentRightEyeRotation.x, -0.2f, 0.2f);
		this.currentRightEyeRotation.y = Utils.FastClamp(this.currentRightEyeRotation.y, -0.4f, 0.4f);
		this.eyeMaterial.SetVector("_LeftEyeRotation", new Vector4(-this.currentLeftEyeRotation.x, this.currentLeftEyeRotation.y, this.currentLeftEyeRotation.z, this.currentLeftEyeRotation.w));
		this.eyeMaterial.SetVector("_RightEyeRotation", new Vector4(-this.currentRightEyeRotation.x, this.currentRightEyeRotation.y, this.currentRightEyeRotation.z, this.currentRightEyeRotation.w));
		this.eyeMaterial.SetVector("_LeftEyePosition", this.leftEyeLocalPosition);
		this.eyeMaterial.SetVector("_RightEyePosition", this.rightEyeLocalPosition);
		this.LookAtTarget.localPosition = localPosition;
	}

	// Token: 0x06004818 RID: 18456 RVA: 0x001C3752 File Offset: 0x001C1952
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void Cleanup()
	{
		CharacterGazeController.instances.Clear();
	}

	// Token: 0x04003718 RID: 14104
	public static List<CharacterGazeController> instances = new List<CharacterGazeController>();

	// Token: 0x04003719 RID: 14105
	public Transform leftEyeTransform;

	// Token: 0x0400371A RID: 14106
	public Transform rightEyeTransform;

	// Token: 0x0400371B RID: 14107
	public Vector3 leftEyeLocalPosition;

	// Token: 0x0400371C RID: 14108
	public Vector3 rightEyeLocalPosition;

	// Token: 0x0400371D RID: 14109
	public Transform rootTransform;

	// Token: 0x0400371E RID: 14110
	public Transform neckTransform;

	// Token: 0x0400371F RID: 14111
	public Transform headTransform;

	// Token: 0x04003720 RID: 14112
	[Range(0f, 100f)]
	public float eyeRotationSpeed = 5f;

	// Token: 0x04003721 RID: 14113
	[Range(0f, 100f)]
	public float headRotationSpeed = 5f;

	// Token: 0x04003722 RID: 14114
	[Range(0f, 50f)]
	public float twitchSpeed = 10f;

	// Token: 0x04003723 RID: 14115
	public float eyeLookAtTargetAngle = 5f;

	// Token: 0x04003724 RID: 14116
	public float headLookAtTargetAngle = 5f;

	// Token: 0x04003725 RID: 14117
	[Range(0f, 20f)]
	public float maxLookAtDistance = 10f;

	// Token: 0x04003726 RID: 14118
	public Material eyeMaterial;

	// Token: 0x04003727 RID: 14119
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool lookAtCamera;

	// Token: 0x04003728 RID: 14120
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform lookAtTarget;

	// Token: 0x04003729 RID: 14121
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameRandom random;

	// Token: 0x0400372A RID: 14122
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<Vector3> lookatOffsets = new List<Vector3>
	{
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(0f, 0f, 0f),
		new Vector3(-1f, 0f, 0f),
		new Vector3(1f, 0f, 0f),
		new Vector3(-2f, 0f, 0f),
		new Vector3(2f, 0f, 0f),
		new Vector3(0f, 0f, -1f),
		new Vector3(0f, 0f, 1f),
		new Vector3(0f, 0f, -2f),
		new Vector3(0f, 0f, 2f)
	};

	// Token: 0x0400372B RID: 14123
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityAlive entityAlive;

	// Token: 0x0400372C RID: 14124
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion currentLeftEyeRotation;

	// Token: 0x0400372D RID: 14125
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion currentRightEyeRotation;

	// Token: 0x0400372E RID: 14126
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion currentHeadRotation;

	// Token: 0x0400372F RID: 14127
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float gazeTimer;

	// Token: 0x04003730 RID: 14128
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int lookatOffsetIndex;

	// Token: 0x04003731 RID: 14129
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool shouldSnapHeadNextUpdate;

	// Token: 0x04003732 RID: 14130
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool shouldSnapEyesNextUpdate;

	// Token: 0x04003733 RID: 14131
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isDead;

	// Token: 0x04003734 RID: 14132
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lookAtTargetDefaultPosition = new Vector3(0f, 1.7f, 10f);
}
