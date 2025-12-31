using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Audio;
using InControl;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200046A RID: 1130
[UnityEngine.Scripting.Preserve]
public class EntityVehicle : EntityAlive, ILockable
{
	// Token: 0x170003F1 RID: 1009
	// (get) Token: 0x060024AF RID: 9391 RVA: 0x000282C0 File Offset: 0x000264C0
	public override Entity.EnumPositionUpdateMovementType positionUpdateMovementType
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return Entity.EnumPositionUpdateMovementType.Instant;
		}
	}

	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x060024B0 RID: 9392 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsValidAimAssistSnapTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170003F3 RID: 1011
	// (get) Token: 0x060024B1 RID: 9393 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsValidAimAssistSlowdownTarget
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060024B2 RID: 9394 RVA: 0x000E9F94 File Offset: 0x000E8194
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Awake()
	{
		this.bag = new Bag(this);
		base.Awake();
		this.isLocked = false;
	}

	// Token: 0x060024B3 RID: 9395 RVA: 0x000E9FB0 File Offset: 0x000E81B0
	public override void Init(int _entityClass)
	{
		base.Init(_entityClass);
		EntityClass entityClass = EntityClass.list[this.entityClass];
		this.vehicle = new Vehicle(entityClass.entityClassName, this);
		base.transform.tag = "E_Vehicle";
		Vector2i size = LootContainer.GetLootContainer(this.GetLootList(), true).size;
		this.bag.SetupSlots(ItemStack.CreateArray(size.x * size.y));
		Transform physicsTransform = this.PhysicsTransform;
		this.vehicleRB = physicsTransform.GetComponent<Rigidbody>();
		if (this.vehicleRB)
		{
			if (this.vehicleRB.automaticCenterOfMass)
			{
				this.vehicleRB.centerOfMass = new Vector3(0f, 0.1f, 0f);
			}
			this.vehicleRB.sleepThreshold = this.vehicleRB.mass * 0.01f * 0.01f * 0.5f;
			physicsTransform.gameObject.AddComponent<CollisionCallForward>().Entity = this;
			physicsTransform.gameObject.layer = 21;
			Utils.SetTagsIfNoneRecursively(physicsTransform, "E_Vehicle");
			this.SetupDevices();
			this.SetVehicleDriven();
			if (!this.isEntityRemote)
			{
				this.isTryToFall = true;
			}
		}
		this.alertEnabled = false;
		GameManager.Instance.StartCoroutine(this.ApplyCollisionsCoroutine());
	}

	// Token: 0x060024B4 RID: 9396 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void AddCharacterController()
	{
	}

	// Token: 0x060024B5 RID: 9397 RVA: 0x000EA0FC File Offset: 0x000E82FC
	public override void PostInit()
	{
		this.LogVehicle("PostInit {0}, {1} (chunk {2}), rbPos {3}", new object[]
		{
			this,
			this.position,
			World.toChunkXZ(this.position),
			this.vehicleRB.position + Origin.position
		});
		base.transform.rotation = this.qrotation;
		if (this.vehicleRB)
		{
			this.PhysicsResetAndSleep();
			this.PhysicsTransform.rotation = this.qrotation;
			this.SetVehicleDriven();
		}
		this.HandleNavObject();
	}

	// Token: 0x060024B6 RID: 9398 RVA: 0x000EA19D File Offset: 0x000E839D
	public override void InitInventory()
	{
		this.inventory = new EntityVehicle.VehicleInventory(GameManager.Instance, this);
	}

	// Token: 0x060024B7 RID: 9399 RVA: 0x000EA1B0 File Offset: 0x000E83B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupDevices()
	{
		this.SetupMotors();
		this.SetupForces();
		this.SetupWheels();
		this.vehicle.Properties.ParseString(EntityVehicle.PropOnHonkEvent, ref this.onHonkEvent);
	}

	// Token: 0x060024B8 RID: 9400 RVA: 0x000EA1E0 File Offset: 0x000E83E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupForces()
	{
		DynamicProperties properties = this.vehicle.Properties;
		if (properties == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		DynamicProperties dynamicProperties;
		while (num2 < 99 && properties.Classes.TryGetValue("force" + num2.ToString(), out dynamicProperties))
		{
			num++;
			num2++;
		}
		this.forces = new EntityVehicle.Force[num];
		for (int i = 0; i < this.forces.Length; i++)
		{
			EntityVehicle.Force force = new EntityVehicle.Force();
			this.forces[i] = force;
			DynamicProperties dynamicProperties2 = properties.Classes["force" + i.ToString()];
			force.ceiling.x = 9999f;
			force.ceiling.y = 9999f;
			dynamicProperties2.ParseVec("ceiling", ref force.ceiling);
			force.ceiling.y = 1f / Utils.FastMax(0.5f, force.ceiling.y - force.ceiling.x);
			force.force = Vector3.forward;
			dynamicProperties2.ParseVec("force", ref force.force);
			force.trigger = EntityVehicle.Force.Trigger.On;
			dynamicProperties2.ParseEnum<EntityVehicle.Force.Trigger>("trigger", ref force.trigger);
			force.type = EntityVehicle.Force.Type.Relative;
			dynamicProperties2.ParseEnum<EntityVehicle.Force.Type>("type", ref force.type);
		}
	}

	// Token: 0x060024B9 RID: 9401 RVA: 0x000EA344 File Offset: 0x000E8544
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupMotors()
	{
		DynamicProperties properties = this.vehicle.Properties;
		if (properties == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		DynamicProperties dynamicProperties;
		while (num2 < 99 && properties.Classes.TryGetValue("motor" + num2.ToString(), out dynamicProperties))
		{
			num++;
			num2++;
		}
		this.motors = new EntityVehicle.Motor[num];
		Transform meshTransform = this.vehicle.GetMeshTransform();
		for (int i = 0; i < this.motors.Length; i++)
		{
			EntityVehicle.Motor motor = new EntityVehicle.Motor();
			this.motors[i] = motor;
			DynamicProperties dynamicProperties2 = properties.Classes["motor" + i.ToString()];
			string @string = dynamicProperties2.GetString("engine");
			if (@string.Length > 0)
			{
				motor.engine = (this.vehicle.FindPart(@string) as VPEngine);
			}
			motor.engineOffPer = 0f;
			dynamicProperties2.ParseFloat("engineOffPer", ref motor.engineOffPer);
			motor.turbo = 1f;
			dynamicProperties2.ParseFloat("turbo", ref motor.turbo);
			motor.rpmAccelMin = 1f;
			motor.rpmAccelMax = 1f;
			dynamicProperties2.ParseVec("rpmAccel_min_max", ref motor.rpmAccelMin, ref motor.rpmAccelMax);
			motor.rpmDrag = 1f;
			dynamicProperties2.ParseFloat("rpmDrag", ref motor.rpmDrag);
			motor.rpmMax = 1f;
			dynamicProperties2.ParseFloat("rpmMax", ref motor.rpmMax);
			if (motor.rpmMax == 0f)
			{
				motor.rpmMax = 0.001f;
			}
			motor.trigger = EntityVehicle.Motor.Trigger.On;
			dynamicProperties2.ParseEnum<EntityVehicle.Motor.Trigger>("trigger", ref motor.trigger);
			string string2 = dynamicProperties2.GetString("transform");
			if (string2.Length > 0)
			{
				motor.transform = meshTransform.Find(string2);
			}
			float num3 = 0f;
			dynamicProperties2.ParseFloat("axis", ref num3);
			motor.axis = (int)num3;
		}
	}

	// Token: 0x060024BA RID: 9402 RVA: 0x000EA548 File Offset: 0x000E8748
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupWheels()
	{
		DynamicProperties properties = this.vehicle.Properties;
		if (properties == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		DynamicProperties dynamicProperties;
		while (num2 < 99 && properties.Classes.TryGetValue("wheel" + num2.ToString(), out dynamicProperties))
		{
			num++;
			num2++;
		}
		this.wheels = new EntityVehicle.Wheel[num];
		Transform physicsTransform = this.PhysicsTransform;
		Transform meshTransform = this.vehicle.GetMeshTransform();
		for (int i = 0; i < this.wheels.Length; i++)
		{
			EntityVehicle.Wheel wheel = new EntityVehicle.Wheel();
			this.wheels[i] = wheel;
			Transform transform = physicsTransform.Find("Wheel" + i.ToString());
			wheel.wheelC = transform.GetComponent<WheelCollider>();
			wheel.forwardFriction = wheel.wheelC.forwardFriction;
			wheel.forwardStiffnessBase = wheel.forwardFriction.stiffness;
			wheel.sideFriction = wheel.wheelC.sidewaysFriction;
			wheel.sideStiffnessBase = wheel.sideFriction.stiffness;
			DynamicProperties dynamicProperties2 = properties.Classes["wheel" + i.ToString()];
			wheel.motorTorqueScale = 1f;
			wheel.brakeTorqueScale = 1f;
			dynamicProperties2.ParseVec("torqueScale_motor_brake", ref wheel.motorTorqueScale, ref wheel.brakeTorqueScale);
			wheel.bounceSound = "vwheel_bounce";
			dynamicProperties2.ParseString("bounceSound", ref wheel.bounceSound);
			wheel.slideSound = "vwheel_slide";
			dynamicProperties2.ParseString("slideSound", ref wheel.slideSound);
			string @string = dynamicProperties2.GetString("steerTransform");
			if (@string.Length > 0)
			{
				wheel.steerT = meshTransform.Find(@string);
				if (wheel.steerT)
				{
					wheel.steerBaseRot = wheel.steerT.localRotation;
				}
			}
			string string2 = dynamicProperties2.GetString("tireTransform");
			if (string2.Length > 0)
			{
				wheel.tireT = meshTransform.Find(string2);
			}
			wheel.isSteerParentOfTire = (wheel.steerT != wheel.tireT);
			if (dynamicProperties2.GetString("tireSuspensionPercent").Length > 0)
			{
				wheel.tireSuspensionPercent = 1f;
			}
		}
	}

	// Token: 0x060024BB RID: 9403 RVA: 0x000EA78B File Offset: 0x000E898B
	public override void OnXMLChanged()
	{
		this.vehicle.OnXMLChanged();
		this.SetupDevices();
	}

	// Token: 0x060024BC RID: 9404 RVA: 0x000EA79E File Offset: 0x000E899E
	public new void FixedUpdate()
	{
		this.PhysicsFixedUpdate();
	}

	// Token: 0x060024BD RID: 9405 RVA: 0x000EA7A8 File Offset: 0x000E89A8
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsResetAndSleep()
	{
		Rigidbody rigidbody = this.vehicleRB;
		Transform physicsTransform = this.PhysicsTransform;
		Vector3 position = this.position - Origin.position;
		physicsTransform.position = position;
		rigidbody.position = position;
		Quaternion rotation = this.ModelTransform.rotation;
		physicsTransform.rotation = rotation;
		rigidbody.rotation = rotation;
		if (!this.vehicleRB.isKinematic)
		{
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			rigidbody.Sleep();
		}
		this.SetWheelsForces(0f, 1f, 0f, 1f);
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x000EA840 File Offset: 0x000E8A40
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsFixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		Rigidbody rigidbody = this.vehicleRB;
		Transform physicsTransform = this.PhysicsTransform;
		this.wheelMotor = 0f;
		this.wheelBrakes = 0f;
		if (this.isEntityRemote)
		{
			this.vehicleRB.isKinematic = true;
			Vector3 position = Vector3.Lerp(physicsTransform.position, this.position - Origin.position, 0.5f);
			physicsTransform.position = position;
			physicsTransform.rotation = Quaternion.Slerp(physicsTransform.rotation, this.ModelTransform.rotation, 0.3f);
			if (this.incomingRemoteData.Flags > 0)
			{
				this.lastRemoteData = this.currentRemoteData;
				this.currentRemoteData = this.incomingRemoteData;
				this.incomingRemoteData.Flags = 0;
				this.syncPlayTime = 0f;
				this.vehicle.CurrentIsAccel = ((this.currentRemoteData.Flags & 2) > 0);
				this.vehicle.CurrentIsBreak = ((this.currentRemoteData.Flags & 4) > 0);
			}
			if (this.syncPlayTime >= 0f)
			{
				float num = this.syncPlayTime / 0.5f;
				this.syncPlayTime += deltaTime;
				if (num >= 1f)
				{
					num = 1f;
					this.syncPlayTime = -1f;
				}
				float num2 = Mathf.Lerp(this.lastRemoteData.SteeringPercent, this.currentRemoteData.SteeringPercent, num);
				this.vehicle.CurrentSteeringPercent = num2;
				float currentMotorTorquePercent = Mathf.Lerp(this.lastRemoteData.MotorTorquePercent, this.currentRemoteData.MotorTorquePercent, num);
				this.vehicle.CurrentMotorTorquePercent = currentMotorTorquePercent;
				Vector3 vector = Vector3.Lerp(this.lastRemoteData.Velocity, this.currentRemoteData.Velocity, num);
				this.vehicle.CurrentVelocity = vector;
				this.vehicle.CurrentForwardVelocity = Vector3.Dot(vector, physicsTransform.forward);
				this.wheelDir = num2 * this.vehicle.SteerAngleMax;
				this.FixedUpdateMotors();
				this.vehicle.UpdateSimulation();
				int num3 = this.wheels.Length;
				if (num3 > 0 && this.lastRemoteData.parts != null)
				{
					int num4 = 0;
					for (int i = 0; i < num3; i++)
					{
						EntityVehicle.Wheel wheel = this.wheels[i];
						Transform steerT = wheel.steerT;
						if (steerT && wheel.isSteerParentOfTire)
						{
							Quaternion localRotation = Quaternion.Lerp(this.lastRemoteData.parts[num4].rot, this.currentRemoteData.parts[num4].rot, num);
							steerT.localRotation = localRotation;
							num4++;
						}
						Transform tireT = wheel.tireT;
						if (tireT)
						{
							Vector3 localPosition = Vector3.Lerp(this.lastRemoteData.parts[num4].pos, this.currentRemoteData.parts[num4].pos, num);
							tireT.localPosition = localPosition;
							Quaternion localRotation2 = Quaternion.Lerp(this.lastRemoteData.parts[num4].rot, this.currentRemoteData.parts[num4].rot, num);
							tireT.localRotation = localRotation2;
							num4++;
						}
					}
				}
			}
			return;
		}
		this.CheckForOutOfWorld();
		if (!this.RBActive)
		{
			this.PhysicsResetAndSleep();
			this.vehicleRB.isKinematic = true;
			return;
		}
		this.vehicleRB.isKinematic = false;
		if (!this.hasDriver)
		{
			Vector3 vector2 = rigidbody.velocity;
			vector2.x *= 0.98f;
			vector2.z *= 0.98f;
			if (this.GetWheelsOnGround() > 0)
			{
				this.RBNoDriverGndTime += deltaTime;
				float f = this.RBNoDriverGndTime / 8f;
				float num5 = Utils.FastLerp(0.6f, 1f, (0.5f - physicsTransform.up.y) / 0.5f);
				num5 = Utils.FastLerp(1f, num5, Mathf.Pow(f, 3f));
				vector2.x *= num5;
				vector2.z *= num5;
			}
			if (this.collisionGrazeCount >= 2)
			{
				float num6 = vector2.magnitude * 1.4f;
				if (num6 < 1f)
				{
					float num7 = Utils.FastLerpUnclamped((float)Utils.FastMin(3, this.collisionGrazeCount) * 0.29f, 0f, num6);
					vector2 *= 1f - num7;
					rigidbody.angularVelocity *= 1f - num7 * 0.65f;
				}
			}
			vector2.y *= this.vehicle.AirDragVelScale;
			rigidbody.velocity = vector2;
			if (vector2.sqrMagnitude < 0.010000001f && rigidbody.angularVelocity.sqrMagnitude < 0.0049f)
			{
				this.RBNoDriverSleepTime += deltaTime;
				if (this.RBNoDriverSleepTime >= 3f)
				{
					this.RBActive = false;
					this.RBNoDriverSleepTime = 0f;
				}
			}
			else
			{
				this.RBNoDriverSleepTime = 0f;
			}
			this.collisionGrazeCount = 0;
		}
		Vector3 vector3 = this.vehicleRB.velocity;
		float num8 = this.vehicle.MotorTorqueForward;
		float num9 = this.vehicle.VelocityMaxForward;
		this.vehicle.IsTurbo = false;
		if (this.movementInput != null)
		{
			if (this.movementInput.moveForward < 0f)
			{
				num8 = this.vehicle.MotorTorqueBackward;
				num9 = this.vehicle.VelocityMaxBackward;
			}
			if (this.movementInput.running && this.vehicle.CanTurbo && this.movementInput.moveForward != 0f)
			{
				this.vehicle.IsTurbo = true;
				num8 = this.vehicle.MotorTorqueTurboForward;
				num9 = this.vehicle.VelocityMaxTurboForward;
				if (this.movementInput.moveForward < 0f)
				{
					num8 = this.vehicle.MotorTorqueTurboBackward;
					num9 = this.vehicle.VelocityMaxTurboBackward;
				}
			}
		}
		num8 *= this.vehicle.EffectMotorTorquePer;
		num9 *= this.vehicle.EffectVelocityMaxPer;
		float num10 = (num9 > this.velocityMax) ? 2.5f : 1.5f;
		num9 = Mathf.MoveTowards(this.velocityMax, num9, num10 * deltaTime);
		this.velocityMax = num9;
		if (this.CalcWaterDepth(this.vehicle.WaterDragY) > 0f)
		{
			this.timeInWater += deltaTime;
			if (this.vehicle.WaterDragVelScale != 1f)
			{
				vector3 *= this.vehicle.WaterDragVelScale;
			}
			if (this.vehicle.WaterDragVelMaxScale != 1f)
			{
				num9 = Mathf.Lerp(num9, num9 * this.vehicle.WaterDragVelMaxScale, this.timeInWater * 0.5f);
			}
		}
		else
		{
			this.timeInWater = 0f;
		}
		float num11 = Mathf.Sqrt(vector3.x * vector3.x + vector3.z * vector3.z);
		if (num11 > num9)
		{
			float num12 = num9 / num11;
			vector3.x *= num12;
			vector3.z *= num12;
			this.vehicleRB.velocity = vector3;
		}
		float magnitude = vector3.magnitude;
		if (this.vehicle.WaterLiftForce > 0f)
		{
			float num13 = this.CalcWaterDepth(this.vehicle.WaterLiftY);
			if (num13 > 0f)
			{
				float y = Mathf.Lerp(this.vehicle.WaterLiftForce * 0.05f, this.vehicle.WaterLiftForce, num13 / (this.vehicle.WaterLiftDepth + 0.001f));
				this.vehicleRB.AddForce(new Vector3(0f, y, 0f), ForceMode.VelocityChange);
			}
		}
		float num14 = -this.lastRBVel.y;
		if (num14 > 8f && (magnitude < num14 * 0.45f || Vector3.Dot(this.lastRBVel.normalized, vector3.normalized) < 0.2f))
		{
			int num15 = (int)((num14 - 8f) * 4f + 0.999f);
			this.ApplyDamage(num15 * 10);
			this.ApplyCollisionDamageToAttached(num15);
		}
		this.lastRBPos = this.vehicleRB.position;
		this.lastRBRot = this.vehicleRB.rotation;
		this.lastRBVel = vector3;
		this.lastRBAngVel = this.vehicleRB.angularVelocity;
		float num16 = Vector3.Dot(vector3, physicsTransform.forward);
		this.vehicle.CurrentForwardVelocity = num16;
		float frictionPercent = 1f;
		if (this.hasDriver && this.wheels.Length < 4 && base.GetAttachedPlayerLocal().isPlayerInStorm)
		{
			frictionPercent = 0.75f;
			float num17 = 0.04f;
			float y2 = 0.01f;
			this.vehicleRB.AddForce(new Vector3(num17 * 0.707f, y2, num17 * 0.707f), ForceMode.VelocityChange);
		}
		this.motorTorque = 0f;
		this.brakeTorque = 0f;
		if (this.wheels.Length != 0)
		{
			if (this.movementInput != null)
			{
				float num18 = Mathf.Pow(magnitude * 0.1f, 2f);
				float num19 = Mathf.Clamp(1f - num18, 0.15f, 1f);
				this.wheelMotor = this.movementInput.moveForward;
				float steerAngleMax = this.vehicle.SteerAngleMax;
				float num20 = this.vehicle.SteerRate * num19 * deltaTime;
				if (EntityVehicle.isTurnTowardsLook)
				{
					float num21 = 0f;
					if (!Input.GetMouseButton(1))
					{
						vp_FPCamera vp_FPCamera = base.GetAttachedPlayerLocal().vp_FPCamera;
						Vector3 forward = base.transform.forward;
						forward.y = 0f;
						Vector3 forward2 = vp_FPCamera.Forward;
						forward2.y = 0f;
						num21 = Vector3.SignedAngle(forward, forward2, Vector3.up);
						if (num16 < -0.02f)
						{
							if (Mathf.Abs(num21) > 90f)
							{
								num21 += 180f;
								if (num21 > 180f)
								{
									num21 -= 360f;
								}
							}
							num21 = -num21;
						}
					}
					float num22 = num20 * 1.2f;
					if ((this.wheelDir < 0f && this.wheelDir < num21) || (this.wheelDir > 0f && this.wheelDir > num21))
					{
						num22 *= 3f;
					}
					this.wheelDir = Mathf.MoveTowards(this.wheelDir, num21, num22);
					this.wheelDir = Mathf.Clamp(this.wheelDir, -steerAngleMax, steerAngleMax);
				}
				else if (this.movementInput.lastInputController)
				{
					this.wheelDir = Mathf.MoveTowards(this.wheelDir, this.movementInput.moveStrafe * steerAngleMax, num20 * 1.5f);
				}
				else
				{
					float moveStrafe = this.movementInput.moveStrafe;
					float num23 = 0f;
					if (moveStrafe < 0f)
					{
						if (this.wheelDir > 0f)
						{
							num23 -= num20 * num18;
						}
						num23 -= num20;
					}
					if (moveStrafe > 0f)
					{
						if (this.wheelDir < 0f)
						{
							num23 += num20 * num18;
						}
						num23 += num20;
					}
					this.wheelDir += num23;
					this.wheelDir = Mathf.Clamp(this.wheelDir, -steerAngleMax, steerAngleMax);
					if (moveStrafe == 0f)
					{
						this.wheelDir = Mathf.MoveTowards(this.wheelDir, 0f, this.vehicle.SteerCenteringRate * deltaTime);
					}
				}
				if (this.wheelMotor != 0f)
				{
					if (this.wheelMotor > 0f)
					{
						if (num16 < -0.5f)
						{
							this.wheelBrakes = 1f;
						}
					}
					else if (num16 > 0.5f)
					{
						this.wheelBrakes = 1f;
					}
					if (!this.movementInput.running)
					{
						this.wheelMotor *= 0.5f;
					}
				}
				if (this.movementInput.jump)
				{
					this.wheelBrakes = 2f;
				}
				if (this.canHop)
				{
					if (this.movementInput.down && this.GetWheelsOnGround() > 0)
					{
						this.canHop = false;
						Vector3 force = Vector3.Slerp(Vector3.up, physicsTransform.up, 0.5f) * this.vehicle.HopForce.x;
						this.vehicleRB.AddForceAtPosition(force, this.vehicleRB.position + physicsTransform.forward * this.vehicle.HopForce.y, ForceMode.VelocityChange);
					}
				}
				else if (!this.movementInput.down)
				{
					this.canHop = true;
				}
			}
			if (this.wheelMotor != 0f)
			{
				if (this.vehicle.HasEnginePart())
				{
					if (this.IsEngineRunning)
					{
						this.motorTorque = this.wheelMotor * num8;
					}
					else
					{
						this.motorTorque = this.wheelMotor * 50f;
					}
				}
				else if (this.vehicle.GetHealth() > 0)
				{
					this.motorTorque = this.wheelMotor * num8;
				}
				else
				{
					this.motorTorque = this.wheelMotor * 10f;
					if (this.rand.RandomFloat < 0.2f)
					{
						this.vehicleRB.AddRelativeForce(0.15f * this.rand.RandomOnUnitSphere, ForceMode.VelocityChange);
					}
					this.wheelDir = Mathf.Clamp(this.wheelDir + (this.rand.RandomFloat * 2f - 1f) * 5f, -this.vehicle.SteerAngleMax, this.vehicle.SteerAngleMax);
				}
				if (magnitude < 0.15f && this.wheelBrakes == 0f && Utils.FastAbs(physicsTransform.up.y) > 0.34f)
				{
					Vector3 force2 = Quaternion.Euler(0f, this.wheelDir, 0f) * (this.vehicle.UnstickForce * Mathf.Sign(this.wheelMotor) * Vector3.forward);
					this.vehicleRB.AddRelativeForce(force2, ForceMode.VelocityChange);
				}
			}
			this.brakeTorque = this.wheelBrakes * this.vehicle.BrakeTorque;
			this.SetWheelsForces(this.motorTorque, num8, this.brakeTorque, frictionPercent);
			this.UpdateWheelsCollision();
			this.UpdateWheelsSteering();
		}
		this.vehicleRB.velocity *= this.vehicle.AirDragVelScale;
		this.vehicleRB.angularVelocity *= this.vehicle.AirDragAngVelScale;
		this.PhysicsInputMove();
		this.FixedUpdateMotors();
		this.FixedUpdateForces();
		if (this.hasDriver || base.GetFirstAttached())
		{
			if (this.vehicle.TiltUpForce > 0f)
			{
				Vector3 right = physicsTransform.right;
				Mathf.Abs(right.y);
				float num24 = Mathf.Asin(right.y) * 57.29578f;
				float num25 = this.wheelDir / this.vehicle.SteerAngleMax;
				num25 *= 2f;
				num25 = Mathf.LerpUnclamped(0f, num25, Mathf.Pow(magnitude * 0.1f, 2f));
				float tiltAngleMax = this.vehicle.TiltAngleMax;
				num25 = Mathf.Clamp(num25 * tiltAngleMax, -tiltAngleMax, tiltAngleMax);
				float f2 = num24 + num25;
				float num26 = Mathf.Abs(f2);
				if (num26 > this.vehicle.TiltThreshold)
				{
					float num27 = (num26 - this.vehicle.TiltThreshold) * Mathf.Sign(f2) * 0.01f * -this.vehicle.TiltUpForce;
					num27 = Mathf.Clamp(num27, -4f, 4f);
					this.vehicleRB.AddRelativeTorque(0f, 0f, num27, ForceMode.VelocityChange);
				}
				if (num26 < this.vehicle.TiltDampenThreshold)
				{
					Vector3 angularVelocity = this.vehicleRB.angularVelocity;
					float magnitude2 = angularVelocity.magnitude;
					if (magnitude2 > 0f)
					{
						Vector3 rhs = angularVelocity * (1f / magnitude2);
						float num28 = Mathf.Abs(Vector3.Dot(base.transform.forward, rhs));
						this.vehicleRB.angularVelocity -= angularVelocity * (0.02f + this.vehicle.TiltDampening * num28);
					}
				}
			}
			if (this.vehicle.UpForce > 0f)
			{
				Vector3 up = physicsTransform.up;
				float num29 = Mathf.Abs(Mathf.Acos(up.y) * 57.29578f) - this.vehicle.UpAngleMax;
				if (num29 > 0f)
				{
					float num30 = num29 / 90f;
					Vector3 torque = Vector3.Cross(up, Vector3.up) * (num30 * num30 * this.vehicle.UpForce);
					this.vehicleRB.AddRelativeTorque(torque, ForceMode.VelocityChange);
				}
			}
		}
		Vector3 position2 = physicsTransform.position;
		this.SetPosition(position2 + Origin.position, false);
		this.qrotation = physicsTransform.rotation;
		this.rotation = this.qrotation.eulerAngles;
		this.ModelTransform.rotation = this.qrotation;
		this.vehicle.CurrentIsAccel = (this.motorTorque != 0f && this.brakeTorque == 0f);
		this.vehicle.CurrentIsBreak = (this.brakeTorque != 0f);
		this.vehicle.CurrentSteeringPercent = this.wheelDir / this.vehicle.SteerAngleMax;
		this.vehicle.CurrentVelocity = this.vehicleRB.velocity;
		this.vehicle.UpdateSimulation();
		if (!this.isEntityRemote)
		{
			this.syncHighRateTime += deltaTime;
			if (this.syncHighRateTime >= 0.5f)
			{
				this.SendSyncData(32768);
				this.syncHighRateTime = 0f;
			}
			this.syncLowRateTime += deltaTime;
			if (this.syncLowRateTime >= 2f)
			{
				this.SendSyncData(16384);
				this.syncLowRateTime = 0f;
			}
		}
	}

	// Token: 0x060024BF RID: 9407 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void PhysicsInputMove()
	{
	}

	// Token: 0x060024C0 RID: 9408 RVA: 0x000EBA38 File Offset: 0x000E9C38
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdateForces()
	{
		if (this.movementInput == null)
		{
			return;
		}
		float num = 1f;
		for (int i = 0; i < this.forces.Length; i++)
		{
			EntityVehicle.Force force = this.forces[i];
			float num2 = 1f;
			switch (force.trigger)
			{
			case EntityVehicle.Force.Trigger.Off:
				num2 = 0f;
				break;
			case EntityVehicle.Force.Trigger.InputForward:
				num2 = this.movementInput.moveForward;
				break;
			case EntityVehicle.Force.Trigger.InputStrafe:
				num2 = this.movementInput.moveStrafe;
				break;
			case EntityVehicle.Force.Trigger.InputUp:
				num2 = (float)(this.movementInput.jump ? 1 : 0);
				break;
			case EntityVehicle.Force.Trigger.InputDown:
				num2 = (float)(this.movementInput.down ? 1 : 0);
				break;
			case EntityVehicle.Force.Trigger.Motor0:
			case EntityVehicle.Force.Trigger.Motor1:
			case EntityVehicle.Force.Trigger.Motor2:
			case EntityVehicle.Force.Trigger.Motor3:
			case EntityVehicle.Force.Trigger.Motor4:
			case EntityVehicle.Force.Trigger.Motor5:
			case EntityVehicle.Force.Trigger.Motor6:
			case EntityVehicle.Force.Trigger.Motor7:
			{
				EntityVehicle.Motor motor = this.motors[force.trigger - EntityVehicle.Force.Trigger.Motor0];
				num2 = motor.rpm / motor.rpmMax;
				break;
			}
			}
			if (num2 != 0f)
			{
				num2 *= num;
				float num3 = this.position.y - force.ceiling.x;
				if (num3 > 0f)
				{
					num2 *= Utils.FastMax(0f, 1f - num3 * force.ceiling.y);
				}
				EntityVehicle.Force.Type type = force.type;
				if (type != EntityVehicle.Force.Type.Relative)
				{
					if (type == EntityVehicle.Force.Type.RelativeTorque)
					{
						this.vehicleRB.AddRelativeTorque(force.force * num2, ForceMode.VelocityChange);
					}
				}
				else
				{
					this.vehicleRB.AddRelativeForce(force.force * num2, ForceMode.VelocityChange);
				}
			}
		}
	}

	// Token: 0x060024C1 RID: 9409 RVA: 0x000EBBCC File Offset: 0x000E9DCC
	[PublicizedFrom(EAccessModifier.Private)]
	public void FixedUpdateMotors()
	{
		for (int i = 0; i < this.motors.Length; i++)
		{
			EntityVehicle.Motor motor = this.motors[i];
			motor.rpm *= motor.rpmDrag;
			float num = 0f;
			switch (motor.trigger)
			{
			case EntityVehicle.Motor.Trigger.On:
				num = 1f;
				break;
			case EntityVehicle.Motor.Trigger.InputForward:
				if (this.movementInput != null)
				{
					num = this.movementInput.moveForward;
				}
				break;
			case EntityVehicle.Motor.Trigger.InputStrafe:
				if (this.movementInput != null)
				{
					num = this.movementInput.moveStrafe;
				}
				break;
			case EntityVehicle.Motor.Trigger.InputUp:
				if (this.movementInput != null && this.movementInput.jump)
				{
					num = 1f;
				}
				break;
			case EntityVehicle.Motor.Trigger.InputDown:
				if (this.movementInput != null && this.movementInput.down)
				{
					num = 1f;
				}
				break;
			case EntityVehicle.Motor.Trigger.Vel:
				num = this.vehicle.CurrentForwardVelocity / (this.vehicle.VelocityMaxForward + 0.001f);
				if (num < 0.01f)
				{
					num = 0f;
				}
				break;
			}
			if (num != 0f)
			{
				float num2 = 1f;
				if (this.movementInput != null && this.movementInput.running)
				{
					num2 = motor.turbo;
				}
				if (motor.engine != null && !motor.engine.isRunning)
				{
					num *= motor.engineOffPer;
					num2 = 1f;
				}
				num *= num2;
				switch (motor.type)
				{
				case EntityVehicle.Motor.Type.Spin:
					if (this.hasDriver)
					{
						float num3 = Mathf.Lerp(motor.rpmAccelMin, motor.rpmAccelMax, num);
						motor.rpm += num3;
						motor.rpm = Mathf.Min(motor.rpm, motor.rpmMax * num2);
					}
					break;
				}
			}
		}
	}

	// Token: 0x060024C2 RID: 9410 RVA: 0x000EBDA0 File Offset: 0x000E9FA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateMotors()
	{
		for (int i = 0; i < this.motors.Length; i++)
		{
			EntityVehicle.Motor motor = this.motors[i];
			Transform transform = motor.transform;
			if (transform)
			{
				Vector3 localEulerAngles = transform.localEulerAngles;
				ref Vector3 ptr = ref localEulerAngles;
				int axis = motor.axis;
				ptr[axis] += motor.rpm * 360f * Time.deltaTime;
				transform.localEulerAngles = localEulerAngles;
			}
		}
	}

	// Token: 0x060024C3 RID: 9411 RVA: 0x000EBE18 File Offset: 0x000EA018
	[PublicizedFrom(EAccessModifier.Private)]
	public int GetWheelsOnGround()
	{
		int num = 0;
		int num2 = this.wheels.Length;
		for (int i = 0; i < num2; i++)
		{
			if (this.wheels[i].isGrounded)
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x060024C4 RID: 9412 RVA: 0x000EBE50 File Offset: 0x000EA050
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetWheelsForces(float motorTorque, float motorTorqueBase, float brakeTorque, float _frictionPercent)
	{
		this.vehicle.CurrentMotorTorquePercent = motorTorque / motorTorqueBase;
		float num = (_frictionPercent == 1f) ? 1f : (_frictionPercent * 0.33f);
		int num2 = this.wheels.Length;
		for (int i = 0; i < num2; i++)
		{
			EntityVehicle.Wheel wheel = this.wheels[i];
			wheel.wheelC.motorTorque = motorTorque * wheel.motorTorqueScale;
			wheel.wheelC.brakeTorque = brakeTorque * wheel.brakeTorqueScale;
			wheel.forwardFriction.stiffness = wheel.forwardStiffnessBase * _frictionPercent;
			wheel.wheelC.forwardFriction = wheel.forwardFriction;
			wheel.sideFriction.stiffness = wheel.sideStiffnessBase * num;
			wheel.wheelC.sidewaysFriction = wheel.sideFriction;
		}
	}

	// Token: 0x060024C5 RID: 9413 RVA: 0x000EBF14 File Offset: 0x000EA114
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateWheelsCollision()
	{
		float wheelPtlScale = this.vehicle.WheelPtlScale;
		for (int i = 0; i < this.wheels.Length; i++)
		{
			EntityVehicle.Wheel wheel = this.wheels[i];
			wheel.isGrounded = false;
			WheelHit wheelHit;
			if (wheel.wheelC.GetGroundHit(out wheelHit))
			{
				float mass = wheel.wheelC.mass;
				if (wheelHit.normal.y >= 0f)
				{
					wheel.isGrounded = true;
				}
				if (wheelHit.force > 260f * mass)
				{
					this.PlayOneShot(wheel.bounceSound, false, false, false, null);
				}
				float forwardSlip = wheelHit.forwardSlip;
				if (forwardSlip <= -0.9f || forwardSlip >= 0.995f)
				{
					wheel.slideTime += Time.deltaTime;
				}
				else if (Utils.FastAbs(wheelHit.sidewaysSlip) >= 0.19f)
				{
					wheel.slideTime += Time.deltaTime;
				}
				else
				{
					wheel.slideTime = 0f;
				}
				if (wheel.slideTime > 0.2f)
				{
					wheel.slideTime = 0f;
					this.PlayOneShot(wheel.slideSound, false, false, false, null);
				}
				if (wheelPtlScale > 0f && Utils.FastAbs(forwardSlip) >= 0.5f)
				{
					wheel.ptlTime += Time.deltaTime;
					if (wheel.ptlTime > 0.05f)
					{
						wheel.ptlTime = 0f;
						float lightValue = GameManager.Instance.World.GetLightBrightness(World.worldToBlockPos(wheelHit.point)) * 0.5f;
						ParticleEffect pe = new ParticleEffect("tiresmoke", Vector3.zero, lightValue, new Color(1f, 1f, 1f, 1f), null, wheel.wheelC.transform, false);
						Transform transform = GameManager.Instance.SpawnParticleEffectClientForceCreation(pe, -1, false);
						if (transform)
						{
							transform.position = wheelHit.point;
							transform.localScale = new Vector3(wheelPtlScale, wheelPtlScale, wheelPtlScale);
						}
					}
				}
			}
		}
	}

	// Token: 0x060024C6 RID: 9414 RVA: 0x000EC114 File Offset: 0x000EA314
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateWheelsSteering()
	{
		this.wheels[0].wheelC.steerAngle = this.wheelDir;
	}

	// Token: 0x060024C7 RID: 9415 RVA: 0x000EC12E File Offset: 0x000EA32E
	public Vector3 GetRBVelocity()
	{
		return this.lastRBVel;
	}

	// Token: 0x060024C8 RID: 9416 RVA: 0x000EC138 File Offset: 0x000EA338
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void Update()
	{
		base.Update();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && base.GetFirstAttached())
		{
			this.world.entityDistributer.SendFullUpdateNextTick(this);
		}
		if (this.vehicleRB && this.RBActive)
		{
			Quaternion rhs = Quaternion.Euler(0f, this.wheelDir, 0f);
			for (int i = 0; i < this.wheels.Length; i++)
			{
				EntityVehicle.Wheel wheel = this.wheels[i];
				wheel.tireSpinSpeed = Utils.FastLerpUnclamped(wheel.tireSpinSpeed, wheel.wheelC.rotationSpeed, 0.3f);
				wheel.tireSpin += Utils.FastClamp(wheel.tireSpinSpeed * Time.deltaTime, -13f, 13f);
				Vector3 vector;
				Quaternion quaternion;
				wheel.wheelC.GetWorldPose(out vector, out quaternion);
				if (wheel.steerT)
				{
					quaternion = Quaternion.Euler(wheel.tireSpin, 0f, 0f);
					Quaternion quaternion2 = wheel.steerBaseRot * rhs;
					if (!wheel.isSteerParentOfTire)
					{
						quaternion2 *= quaternion;
					}
					wheel.steerT.localRotation = quaternion2;
				}
				if (wheel.tireT)
				{
					if (wheel.tireSuspensionPercent > 0f)
					{
						vector = wheel.tireT.parent.InverseTransformPoint(vector);
						Vector3 localPosition = wheel.tireT.localPosition;
						localPosition.y = vector.y;
						wheel.tireT.localPosition = localPosition;
					}
					if (wheel.steerT)
					{
						if (wheel.isSteerParentOfTire)
						{
							wheel.tireT.localRotation = quaternion;
						}
					}
					else
					{
						wheel.tireT.localRotation = Quaternion.Euler(wheel.tireSpin, 0f, 0f);
					}
				}
			}
		}
		if (this.vehicleRB)
		{
			float deltaTime = Time.deltaTime;
			Vector3 vector2;
			if (!this.isEntityRemote)
			{
				vector2 = this.PhysicsTransform.position + Origin.position;
				this.SetPosition(vector2, false);
				vector2 -= Origin.position;
				this.qrotation = this.PhysicsTransform.rotation;
				this.rotation = this.qrotation.eulerAngles;
				this.ModelTransform.rotation = this.qrotation;
			}
			else
			{
				vector2 = this.ModelTransform.position;
			}
			EntityPlayerLocal attachedPlayerLocal = base.GetAttachedPlayerLocal();
			if (attachedPlayerLocal)
			{
				vp_FPCamera vp_FPCamera = attachedPlayerLocal.vp_FPCamera;
				if (!EntityVehicle.isTurnTowardsLook)
				{
					Vector3 forward = base.transform.forward;
					Vector2 to = new Vector2(forward.x, forward.z);
					this.cameraAngleTarget = Vector2.SignedAngle(this.cameraStartVec, to);
					float num = this.cameraAngle;
					float num2 = Mathf.Abs(Mathf.DeltaAngle(this.cameraAngle, this.cameraAngleTarget));
					this.cameraAngle = Mathf.MoveTowardsAngle(this.cameraAngle, this.cameraAngleTarget, num2 * 0.3f);
					num -= this.cameraAngle;
					vp_FPCamera.Yaw += num;
				}
				float magnitude = this.vehicleRB.velocity.magnitude;
				float num3 = -Mathf.Lerp(this.vehicle.CameraDistance.x, this.vehicle.CameraDistance.y, magnitude / this.vehicle.VelocityMaxForward) * EntityVehicle.cameraDistScale - this.cameraDist;
				if (num3 < 0f)
				{
					this.cameraOutTime += deltaTime;
					if (this.cameraOutTime > 1f)
					{
						num3 *= 0.03f;
						this.cameraDist += num3;
					}
				}
				else if (num3 > 0f)
				{
					this.cameraOutTime = 0f;
					num3 *= 0.22f;
					this.cameraDist += num3;
				}
				vector2.y += 1.8f;
				vector2.y += this.lastRBVel.y * 0.2f;
				this.cameraPos.x = vector2.x;
				this.cameraPos.z = vector2.z;
				float num4 = vector2.y - this.cameraPos.y;
				if ((num4 < 0f && this.cameraVelY > 0f) || (num4 > 0f && this.cameraVelY < 0f))
				{
					this.cameraVelY *= 0.98f;
				}
				this.cameraVelY += num4 * 0.25f * deltaTime;
				this.cameraVelY *= 0.94f;
				this.cameraPos.y = this.cameraPos.y + this.cameraVelY;
				num4 = vector2.y - this.cameraPos.y;
				if (num4 > 2.5f)
				{
					this.cameraPos.y = vector2.y - 2.5f;
					this.cameraVelY = 0f;
				}
				else if (num4 < -2.5f)
				{
					this.cameraPos.y = vector2.y + 2.5f;
					this.cameraVelY = 0f;
				}
				if (this.cameraStartBlend < 1f)
				{
					this.cameraStartBlend += deltaTime * 1.2f;
					vp_FPCamera.DrivingPosition = Vector3.Lerp(this.cameraStartPos, this.cameraPos, this.cameraStartBlend);
					float z = Mathf.Lerp(-0.0001f, this.cameraDist, this.cameraStartBlend);
					vp_FPCamera.Position3rdPersonOffset = new Vector3(0f, 0f, z);
				}
				else
				{
					vp_FPCamera.DrivingPosition = this.cameraPos;
					vp_FPCamera.Position3rdPersonOffset = new Vector3(0f, 0f, this.cameraDist);
				}
			}
		}
		this.UpdateAttachment();
		if (this.RBActive || this.syncPlayTime >= 0f)
		{
			this.UpdateMotors();
		}
		this.vehicle.Update(Time.deltaTime);
		if ((Time.frameCount & 1) == 0)
		{
			this.hitEffectCount = 1;
		}
	}

	// Token: 0x060024C9 RID: 9417 RVA: 0x000EC758 File Offset: 0x000EA958
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTransform()
	{
		if (this.isEntityRemote)
		{
			float t = Time.deltaTime * 10f;
			Transform modelTransform = this.ModelTransform;
			Vector3 position = Vector3.Lerp(modelTransform.position, this.position - Origin.position, t);
			Quaternion rotation = Quaternion.Slerp(modelTransform.rotation, this.qrotation, t);
			modelTransform.SetPositionAndRotation(position, rotation);
		}
	}

	// Token: 0x060024CA RID: 9418 RVA: 0x000EC7B8 File Offset: 0x000EA9B8
	public void CameraChangeRotation(float _newRotation)
	{
		if (EntityVehicle.isTurnTowardsLook)
		{
			EntityPlayerLocal attachedPlayerLocal = base.GetAttachedPlayerLocal();
			if (attachedPlayerLocal)
			{
				attachedPlayerLocal.vp_FPCamera.Yaw += _newRotation;
			}
		}
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x000EC7F0 File Offset: 0x000EA9F0
	public override void OriginChanged(Vector3 _deltaPos)
	{
		base.OriginChanged(_deltaPos);
		Vector3 position = this.position - Origin.position;
		this.ModelTransform.position = position;
		this.PhysicsTransform.position = position;
		if (this.vehicleRB)
		{
			this.vehicleRB.position = position;
		}
		this.cameraPos += _deltaPos;
		this.cameraStartPos += _deltaPos;
		EntityPlayerLocal attachedPlayerLocal = base.GetAttachedPlayerLocal();
		if (attachedPlayerLocal)
		{
			attachedPlayerLocal.vp_FPCamera.DrivingPosition += _deltaPos;
		}
	}

	// Token: 0x060024CC RID: 9420 RVA: 0x000EC890 File Offset: 0x000EAA90
	public override void SetPosition(Vector3 _pos, bool _bUpdatePhysics = true)
	{
		base.SetPosition(_pos, _bUpdatePhysics);
		if (!this.isEntityRemote)
		{
			this.ModelTransform.position = _pos - Origin.position;
		}
	}

	// Token: 0x060024CD RID: 9421 RVA: 0x000EC8B8 File Offset: 0x000EAAB8
	public override void SetRotation(Vector3 _rot)
	{
		base.SetRotation(_rot);
		if (!this.isEntityRemote)
		{
			this.ModelTransform.rotation = this.qrotation;
		}
	}

	// Token: 0x060024CE RID: 9422 RVA: 0x000EC8DA File Offset: 0x000EAADA
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 GetCenterPosition()
	{
		return this.position + this.ModelTransform.up * 0.8f;
	}

	// Token: 0x060024CF RID: 9423 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsQRotationUsed()
	{
		return true;
	}

	// Token: 0x060024D0 RID: 9424 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public override float GetHeight()
	{
		return 1f;
	}

	// Token: 0x060024D1 RID: 9425 RVA: 0x000EC8FC File Offset: 0x000EAAFC
	public void AddRelativeForce(Vector3 forceVec, ForceMode mode = ForceMode.VelocityChange)
	{
		if (this.isEntityRemote)
		{
			return;
		}
		if (!this.RBActive)
		{
			this.RBActive = true;
			this.vehicleRB.isKinematic = false;
		}
		this.vehicleRB.AddRelativeForce(forceVec, mode);
	}

	// Token: 0x060024D2 RID: 9426 RVA: 0x000EC92F File Offset: 0x000EAB2F
	public void AddForce(Vector3 forceVec, ForceMode mode = ForceMode.VelocityChange)
	{
		if (this.isEntityRemote)
		{
			return;
		}
		if (!this.RBActive)
		{
			this.RBActive = true;
			this.vehicleRB.isKinematic = false;
		}
		this.vehicleRB.AddForce(forceVec, mode);
	}

	// Token: 0x060024D3 RID: 9427 RVA: 0x000EC962 File Offset: 0x000EAB62
	public override Vector3 GetVelocityPerSecond()
	{
		if (this.isEntityRemote)
		{
			return this.vehicle.CurrentVelocity;
		}
		return this.vehicleRB.velocity;
	}

	// Token: 0x060024D4 RID: 9428 RVA: 0x000EC984 File Offset: 0x000EAB84
	public void VelocityFlip()
	{
		if (this.isEntityRemote)
		{
			this.vehicle.CurrentVelocity = new Vector3(this.vehicle.CurrentVelocity.x * -1f, this.vehicle.CurrentVelocity.y, this.vehicle.CurrentVelocity.z * -1f);
			return;
		}
		this.vehicleRB.velocity = new Vector3(this.vehicleRB.velocity.x * -1f, this.vehicleRB.velocity.y, this.vehicleRB.velocity.z * -1f);
	}

	// Token: 0x060024D5 RID: 9429 RVA: 0x000ECA34 File Offset: 0x000EAC34
	[PublicizedFrom(EAccessModifier.Private)]
	public void EnterVehicle(EntityAlive _entity)
	{
		int slot = -1;
		_entity.StartAttachToEntity(this, slot);
		if (this.NavObject != null)
		{
			this.NavObject.IsActive = !(_entity is EntityPlayerLocal);
		}
		EntityPlayerLocal entityPlayerLocal = _entity as EntityPlayerLocal;
		if (entityPlayerLocal != null)
		{
			entityPlayerLocal.Waypoints.UpdateEntityVehicleWayPoint(this, false);
		}
	}

	// Token: 0x060024D6 RID: 9430 RVA: 0x000ECA84 File Offset: 0x000EAC84
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetVehicleDriven()
	{
		if (base.AttachedMainEntity != null && !base.AttachedMainEntity.isEntityRemote)
		{
			Utils.SetLayerRecursively(this.vehicleRB.gameObject, 21);
			this.RBActive = true;
			this.vehicleRB.isKinematic = false;
			this.vehicleRB.WakeUp();
			if (this.world.IsRemote())
			{
				this.vehicleRB.velocity = this.vehicle.CurrentVelocity;
			}
			this.lastRBVel = Vector3.zero;
			EntityPlayerLocal entityPlayerLocal = base.AttachedMainEntity as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				entityPlayerLocal.Waypoints.SetWaypointHiddenOnMap(this.entityId, true);
				return;
			}
		}
		else
		{
			Utils.SetLayerRecursively(this.vehicleRB.gameObject, 21);
			if (this.isEntityRemote)
			{
				this.RBActive = false;
			}
		}
	}

	// Token: 0x060024D7 RID: 9431 RVA: 0x000ECB54 File Offset: 0x000EAD54
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateAttachment()
	{
		Entity attachedMainEntity = base.AttachedMainEntity;
		if (this.hasDriver && attachedMainEntity == null)
		{
			this.DriverRemoved();
		}
		if (attachedMainEntity != null && attachedMainEntity.IsDead())
		{
			((EntityAlive)attachedMainEntity).RemoveIKTargets();
			attachedMainEntity.Detach();
			this.DriverRemoved();
		}
		for (int i = this.delayedAttachments.Count - 1; i >= 0; i--)
		{
			EntityVehicle.DelayedAttach delayedAttach = this.delayedAttachments[i];
			Entity entity = GameManager.Instance.World.GetEntity(delayedAttach.entityId);
			if (entity)
			{
				if (!base.IsAttached(entity))
				{
					entity.AttachToEntity(this, delayedAttach.slot);
				}
				this.delayedAttachments.RemoveAt(i);
			}
		}
	}

	// Token: 0x060024D8 RID: 9432 RVA: 0x000ECC10 File Offset: 0x000EAE10
	[PublicizedFrom(EAccessModifier.Private)]
	public void DriverRemoved()
	{
		this.hasDriver = false;
		this.vehicle.SetColors();
		this.vehicle.FireEvent(Vehicle.Event.Stop);
		this.isInteractionLocked = false;
		this.RBNoDriverGndTime = 0f;
		this.RBNoDriverSleepTime = 0f;
		this.collisionGrazeCount = 0;
		if (this.GetWheelsOnGround() > 0 && !this.vehicleRB.isKinematic)
		{
			this.vehicleRB.velocity *= 0.5f;
		}
		if (this.NavObject != null)
		{
			this.NavObject.IsActive = true;
		}
	}

	// Token: 0x060024D9 RID: 9433 RVA: 0x000ECCA4 File Offset: 0x000EAEA4
	public override int AttachEntityToSelf(Entity _entity, int slot = -1)
	{
		slot = base.AttachEntityToSelf(_entity, slot);
		if (slot >= 0)
		{
			EntityAlive entityAlive = (EntityAlive)_entity;
			int seatPose = this.vehicle.GetSeatPose(slot);
			entityAlive.SetVehiclePoseMode(seatPose);
			entityAlive.transform.gameObject.layer = 24;
			entityAlive.m_characterController.Enable(false);
			entityAlive.SetIKTargets(this.vehicle.GetIKTargets(slot));
			this.isInteractionLocked = (base.GetAttachFreeCount() == 0);
			if (this.nativeCollider)
			{
				this.nativeCollider.enabled = !this.isInteractionLocked;
			}
			if (slot == 0)
			{
				this.hasDriver = true;
				this.vehicle.SetColors();
				this.vehicle.FireEvent(Vehicle.Event.Start);
			}
			this.SetVehicleDriven();
			this.vehicle.TriggerUpdateEffects();
			if (!_entity.isEntityRemote && GameManager.Instance.World != null)
			{
				LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_entity as EntityPlayerLocal);
				if (uiforPlayer != null && uiforPlayer.playerInput != null)
				{
					PlayerActionsVehicle vehicleActions = uiforPlayer.playerInput.VehicleActions;
					uiforPlayer.ActionSetManager.Insert(vehicleActions, 1, null);
					this.movementInput = new MovementInput();
					this.CameraInit();
				}
			}
		}
		return slot;
	}

	// Token: 0x060024DA RID: 9434 RVA: 0x000ECDCC File Offset: 0x000EAFCC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void DetachEntity(Entity _entity)
	{
		for (int i = this.delayedAttachments.Count - 1; i >= 0; i--)
		{
			if (this.delayedAttachments[i].entityId == _entity.entityId)
			{
				this.delayedAttachments.RemoveAt(i);
			}
		}
		int num = base.FindAttachSlot(_entity);
		if (num < 0)
		{
			return;
		}
		EntityAlive entityAlive = (EntityAlive)_entity;
		entityAlive.SetVehiclePoseMode(-1);
		entityAlive.RemoveIKTargets();
		int modelLayer = entityAlive.GetModelLayer();
		entityAlive.SetModelLayer(modelLayer, true, null);
		entityAlive.transform.gameObject.layer = 20;
		entityAlive.ModelTransform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		entityAlive.m_characterController.Enable(true);
		if (!_entity.isEntityRemote && GameManager.Instance.World != null)
		{
			LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_entity as EntityPlayerLocal);
			if (uiforPlayer != null)
			{
				PlayerActionsVehicle vehicleActions = uiforPlayer.playerInput.VehicleActions;
				uiforPlayer.ActionSetManager.Remove(vehicleActions, 1, null);
			}
			this.movementInput = null;
		}
		if (num == 0)
		{
			this.DriverRemoved();
		}
		bool isEntityRemote = this.isEntityRemote;
		base.DetachEntity(_entity);
		this.isInteractionLocked = (base.GetAttachFreeCount() == 0);
		if (this.nativeCollider)
		{
			this.nativeCollider.enabled = !this.isInteractionLocked;
		}
		this.SetVehicleDriven();
		this.vehicle.TriggerUpdateEffects();
		if (isEntityRemote && !this.isEntityRemote)
		{
			this.RBActive = true;
			this.RBNoDriverSleepTime = 0f;
			this.vehicleRB.isKinematic = false;
			this.vehicleRB.velocity = this.vehicle.CurrentVelocity;
		}
	}

	// Token: 0x060024DB RID: 9435 RVA: 0x000ECF59 File Offset: 0x000EB159
	public override int AttachToEntity(Entity _entity, int slot = -1)
	{
		return -1;
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x000ECF5C File Offset: 0x000EB15C
	public override AttachedToEntitySlotInfo GetAttachedToInfo(int _slotIdx)
	{
		AttachedToEntitySlotInfo attachedToEntitySlotInfo = new AttachedToEntitySlotInfo();
		attachedToEntitySlotInfo.bKeep3rdPersonModelVisible = true;
		attachedToEntitySlotInfo.bReplaceLocalInventory = true;
		attachedToEntitySlotInfo.pitchRestriction = new Vector2(-30f, 30f);
		attachedToEntitySlotInfo.yawRestriction = new Vector2(-90f, 90f);
		attachedToEntitySlotInfo.enterParentTransform = base.transform;
		attachedToEntitySlotInfo.enterPosition = new Vector3(0f, 0f, -0.201f);
		attachedToEntitySlotInfo.enterRotation = Vector3.zero;
		DynamicProperties propertiesForClass = this.vehicle.GetPropertiesForClass("seat" + _slotIdx.ToString());
		if (propertiesForClass != null)
		{
			propertiesForClass.ParseVec("position", ref attachedToEntitySlotInfo.enterPosition);
			propertiesForClass.ParseVec("rotation", ref attachedToEntitySlotInfo.enterRotation);
			string @string = propertiesForClass.GetString("exit");
			if (@string.Length > 0)
			{
				char[] separator = new char[]
				{
					'~'
				};
				string[] array = @string.Split(separator);
				for (int i = 0; i < array.Length; i++)
				{
					Vector3 vector = StringParsers.ParseVector3(array[i], 0, -1);
					vector.y += 0.02f;
					AttachedToEntitySlotExit item;
					item.position = base.GetPosition() + base.transform.TransformDirection(vector);
					float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
					item.rotation = new Vector3(0f, num + 180f + this.rotation.y, 0f);
					attachedToEntitySlotInfo.exits.Add(item);
				}
			}
		}
		else
		{
			AttachedToEntitySlotExit item2 = default(AttachedToEntitySlotExit);
			item2.position = base.GetPosition() + -2f * base.transform.right;
			item2.rotation = new Vector3(0f, this.rotation.y + 90f, 0f);
			attachedToEntitySlotInfo.exits.Add(item2);
		}
		return attachedToEntitySlotInfo;
	}

	// Token: 0x060024DD RID: 9437 RVA: 0x000ED158 File Offset: 0x000EB358
	public Vector3 GetExitVelocity()
	{
		Vector3 a = this.GetVelocityPerSecond();
		if (this.GetWheelsOnGround() > 0)
		{
			a *= 0.5f;
		}
		return a * 0.7f;
	}

	// Token: 0x060024DE RID: 9438 RVA: 0x000ED190 File Offset: 0x000EB390
	[PublicizedFrom(EAccessModifier.Private)]
	public void CameraInit()
	{
		Transform transform = base.transform;
		Vector3 forward = transform.forward;
		this.cameraStartVec.x = forward.x;
		this.cameraStartVec.y = forward.z;
		this.cameraPos = transform.position;
		this.cameraPos.y = this.cameraPos.y + 1.8f;
		EntityPlayerLocal attachedPlayerLocal = base.GetAttachedPlayerLocal();
		if (attachedPlayerLocal)
		{
			vp_FPCamera vp_FPCamera = attachedPlayerLocal.vp_FPCamera;
			this.cameraStartPos = vp_FPCamera.transform.position;
			this.cameraStartBlend = 0f;
			vp_FPCamera.m_Current3rdPersonBlend = 1f;
			this.cameraDist = -(this.cameraPos - this.cameraStartPos).magnitude;
		}
	}

	// Token: 0x060024DF RID: 9439 RVA: 0x000ED24C File Offset: 0x000EB44C
	public override void OnCollisionForward(Transform t, Collision collision, bool isStay)
	{
		if (this.isEntityRemote)
		{
			return;
		}
		if (!this.RBActive)
		{
			if (this.vehicleRB.velocity.magnitude > 0.01f && this.vehicleRB.angularVelocity.magnitude > 0.05f)
			{
				this.RBActive = true;
			}
			if (this.vehicleRB.isKinematic && (!collision.rigidbody || collision.rigidbody.velocity.magnitude > 0.05f))
			{
				this.RBActive = true;
			}
		}
		Entity entity = null;
		int layer = collision.gameObject.layer;
		if (layer != 16)
		{
			ColliderHitCallForward component = collision.gameObject.GetComponent<ColliderHitCallForward>();
			if (component)
			{
				entity = component.Entity;
			}
			if (!entity)
			{
				entity = this.FindEntity(collision.transform.parent);
			}
			if (!entity)
			{
				Rigidbody rigidbody = collision.rigidbody;
				if (rigidbody)
				{
					entity = this.FindEntity(rigidbody.transform);
				}
			}
		}
		if (entity && entity.IsSpawned())
		{
			if (collision.impulse.sqrMagnitude > 4f)
			{
				Vector3 vector = -collision.relativeVelocity;
				if (layer != 19)
				{
					vector *= 0.4f;
				}
				float num = vector.magnitude + 0.0001f;
				Vector3 vector2 = vector * (1f / num);
				EnumBodyPartHit enumBodyPartHit = EnumBodyPartHit.Torso;
				bool flag = false;
				Vector3 vector3 = Vector3.zero;
				Vector3 vector4 = Vector3.zero;
				int contactCount = collision.contactCount;
				for (int i = 0; i < contactCount; i++)
				{
					ContactPoint contact = collision.GetContact(i);
					vector3 += contact.point;
					vector4 += contact.normal;
					flag |= contact.thisCollider.CompareTag("E_VehicleStrong");
					string tag = contact.otherCollider.tag;
					enumBodyPartHit |= DamageSource.TagToBodyPart(tag);
				}
				vector3 *= 1f / (float)contactCount;
				vector3 += Origin.position;
				vector4 = Vector3.Normalize(vector4);
				float num2 = -Vector3.Dot(vector2, vector4);
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				if (num > 1f)
				{
					float num3 = Vector3.Dot(entity.motion.normalized, vector2);
					if (num3 > 0.2f)
					{
						float num4 = entity.motion.magnitude * 20f;
						num -= num4 * num3;
					}
				}
				float num5 = num * num2;
				float num6 = this.vehicleRB.mass * 0.2f;
				num6 += 20f;
				float massKg = EntityClass.list[entity.entityClass].MassKg;
				float num7 = num6 / massKg;
				float num8 = Utils.FastClamp(num7, 0.25f, 1.6f);
				float num9 = num5 * num8;
				float num10 = Utils.FastClamp(num7, 1f, 1.5f);
				float num11 = num5 / num10;
				if (massKg < 2f)
				{
					num2 = 0f;
					num9 = 0f;
					num11 = 0f;
				}
				EntityPlayer entityPlayer = entity as EntityPlayer;
				if (entityPlayer && (float)entityPlayer.SpawnedTicks <= 80f)
				{
					num9 = 0f;
					num11 = 0f;
				}
				bool flag2 = this.world.IsWorldEvent(World.WorldEvent.BloodMoon);
				bool flag3 = num7 >= 2f && !flag2 && (this.lastRBVel.sqrMagnitude > 10.240001f || num9 > 2.1f);
				vector *= num6 * 0.008f;
				vector.y = Utils.FastMin(50f, vector.y + vector.magnitude * 3f);
				if (num9 > 2.1f)
				{
					int entityId = this.entityId;
					Entity firstAttached = base.GetFirstAttached();
					if (firstAttached)
					{
						entityId = firstAttached.entityId;
					}
					DamageSourceEntity damageSourceEntity = new DamageSourceEntity(EnumDamageSource.External, EnumDamageTypes.Crushing, entityId, vector);
					damageSourceEntity.bodyParts = enumBodyPartHit;
					damageSourceEntity.DismemberChance = 1.2f;
					float num12 = 1f + (num9 - 2.1f) * 12f;
					if (entityPlayer)
					{
						num12 = Utils.FastMin(num12, 10f);
					}
					if (flag)
					{
						num12 *= this.vehicle.EffectEntityDamagePer;
					}
					bool flag4 = entity.IsAlive();
					entity.DamageEntity(damageSourceEntity, (int)num12, false, 1f);
					if ((entity.entityFlags & (EntityFlags.Player | EntityFlags.Zombie | EntityFlags.Animal | EntityFlags.Bandit)) > EntityFlags.None && num12 > 70f)
					{
						this.SpawnParticle("blood_vehicle", entity.entityId, 0.22f);
						if (num12 > 200f)
						{
							this.SpawnParticle("blood_vehicle", entity.entityId, 0.35f);
						}
					}
					float num13 = 1f;
					if (flag2)
					{
						this.velocityMax *= 0.7f;
						num13 *= 15f;
					}
					EntityPlayer entityPlayer2 = firstAttached as EntityPlayer;
					if (entityPlayer2)
					{
						entityPlayer2.MinEventContext.Other = (entity as EntityAlive);
						entityPlayer2.FireEvent(MinEventTypes.onSelfVehicleAttackedOther, true);
					}
					if (flag4 && entity.IsDead())
					{
						flag3 = false;
						if (entityPlayer2)
						{
							EntityAlive entityAlive = entity as EntityAlive;
							if (entityAlive)
							{
								entityPlayer2.AddKillXP(entityAlive, 0.5f);
							}
						}
					}
					else if (num9 >= num13)
					{
						float num14 = num9 * 0.09f;
						if (num9 < 8f && num14 > 0.9f)
						{
							num14 = 0.9f;
						}
						if (this.rand.RandomFloat < num14)
						{
							flag3 = true;
						}
					}
				}
				if (entity.emodel.IsRagdollOn)
				{
					num11 *= 0.3f;
				}
				if (flag3)
				{
					entity.emodel.DoRagdoll(2.5f, enumBodyPartHit, vector, vector3, false);
				}
				if (num11 > 2.1f)
				{
					float num15 = 1f + (num11 - 2.1f) * 28f;
					num15 *= this.vehicle.EffectSelfDamagePer;
					if (flag)
					{
						num15 *= this.vehicle.EffectStrongSelfDamagePer;
					}
					float num16 = (this.Health > 1) ? 1f : 0.1f;
					this.damageAccumulator += num15 * num16;
					this.ApplyAccumulatedDamage();
				}
				if (num > 0.1f && num2 > 0.2f)
				{
					this.velocityMax *= Mathf.LerpUnclamped(1f, 0.4f + num10 * 0.39666668f, num2);
					return;
				}
			}
		}
		else
		{
			Vector3 a = this.lastRBVel;
			float magnitude = a.magnitude;
			float num17 = Utils.FastMax(0f, magnitude - 1.5f) * this.vehicleRB.mass * 0.058333334f;
			if (isStay)
			{
				num17 *= 0.2f;
			}
			if (num17 < 2f)
			{
				this.collisionGrazeCount++;
				return;
			}
			this.collisionBlockDamage = num17;
			this.collisionVelNorm = a * (1f / magnitude);
			this.collisionIgnoreCount = 0;
			int contactCount2 = collision.contactCount;
			for (int j = 0; j < contactCount2; j++)
			{
				ContactPoint contact2 = collision.GetContact(j);
				Ray ray = new Ray(contact2.point + Origin.position + contact2.normal * 0.004f, -contact2.normal);
				bool flag5 = Voxel.Raycast(this.world, ray, 0.03f, -555520021, 69, 0f);
				if (!flag5)
				{
					ray.origin += contact2.normal * -contact2.separation;
					ray.direction = -contact2.normal + this.collisionVelNorm;
					flag5 = Voxel.Raycast(this.world, ray, 0.03f, -555520021, 69, 0f);
				}
				if (flag5 && GameUtils.IsBlockOrTerrain(Voxel.voxelRayHitInfo.tag))
				{
					bool flag6 = false;
					for (int k = 0; k < this.collisionHits.Count; k++)
					{
						if (this.collisionHits[k].hit.blockPos == Voxel.voxelRayHitInfo.hit.blockPos)
						{
							flag6 = true;
							break;
						}
					}
					if (!flag6)
					{
						this.contactPoints.Add(contact2);
						this.collisionHits.Add(Voxel.voxelRayHitInfo.Clone());
					}
				}
				else
				{
					this.collisionIgnoreCount++;
				}
			}
		}
	}

	// Token: 0x060024E0 RID: 9440 RVA: 0x000EDAA3 File Offset: 0x000EBCA3
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ApplyCollisionsCoroutine()
	{
		WaitForFixedUpdate wait = new WaitForFixedUpdate();
		for (;;)
		{
			yield return wait;
			int count = this.contactPoints.Count;
			if (count > 0)
			{
				float num = (this.Health > 1) ? 1f : 0.1f;
				int entityId = this.entityId;
				ItemActionAttack.EnumAttackMode attackMode = ItemActionAttack.EnumAttackMode.RealNoHarvesting;
				if (this.hitEffectCount <= 0)
				{
					attackMode = ItemActionAttack.EnumAttackMode.RealNoHarvestingOrEffects;
				}
				float num2 = 1f / ((float)count + 0.001f);
				float num3 = this.collisionBlockDamage;
				num3 *= num2;
				for (int i = 0; i < count; i++)
				{
					ContactPoint contactPoint = this.contactPoints[i];
					WorldRayHitInfo worldRayHitInfo = this.collisionHits[i];
					float num4 = -Vector3.Dot(contactPoint.normal, this.collisionVelNorm);
					num4 = Mathf.Pow(num4 * 1.01f, 3f);
					num4 = Utils.FastClamp(num4, 0.01f, 1f);
					float num5 = 0f;
					float num6 = 2.5f;
					bool flag = contactPoint.thisCollider.CompareTag("E_VehicleStrong");
					bool flag2 = worldRayHitInfo.tag == "T_Mesh";
					if (flag2)
					{
						if (contactPoint.normal.y < 0.85f)
						{
							num5 = 0.7f + 4f * this.rand.RandomFloat * num4;
							num6 = 0.1f;
						}
					}
					else
					{
						num5 = num3 * num4;
						if (flag)
						{
							num5 *= this.vehicle.EffectBlockDamagePer;
						}
						float vehicleHitScale = worldRayHitInfo.hit.blockValue.Block.VehicleHitScale;
						num5 *= vehicleHitScale;
						num6 /= vehicleHitScale;
						if (num5 < 5f)
						{
							num5 = 0f;
						}
					}
					if (num5 >= 1f)
					{
						List<string> buffActions = null;
						ItemActionAttack.AttackHitInfo attackHitInfo = new ItemActionAttack.AttackHitInfo();
						attackHitInfo.hardnessScale = 1f;
						if (flag2 || !worldRayHitInfo.hit.blockValue.Block.shape.IsTerrain())
						{
							ItemActionAttack.Hit(worldRayHitInfo, entityId, EnumDamageTypes.Bashing, num5, num5, 1f, 1f, 0f, 0.05f, "metal", null, buffActions, attackHitInfo, 1, 0, 0f, null, null, attackMode, null, -1, null);
							int num7 = this.hitEffectCount - 1;
							this.hitEffectCount = num7;
							if (num7 <= 0)
							{
								attackMode = ItemActionAttack.EnumAttackMode.RealNoHarvestingOrEffects;
							}
						}
						if (!attackHitInfo.bBlockHit)
						{
							ChunkCluster chunkCluster = GameManager.Instance.World.ChunkClusters[worldRayHitInfo.hit.clrIdx];
							if (chunkCluster != null)
							{
								Vector3i vector3i = Vector3i.FromVector3Rounded(contactPoint.point + Origin.position);
								for (int j = 0; j >= -1; j--)
								{
									worldRayHitInfo.hit.blockPos.y = vector3i.y + j;
									for (int k = 0; k >= -1; k--)
									{
										worldRayHitInfo.hit.blockPos.z = vector3i.z + k;
										for (int l = 0; l >= -1; l--)
										{
											worldRayHitInfo.hit.blockPos.x = vector3i.x + l;
											if (!chunkCluster.GetBlock(worldRayHitInfo.hit.blockPos).Block.shape.IsTerrain())
											{
												ItemActionAttack.Hit(worldRayHitInfo, entityId, EnumDamageTypes.Bashing, num5, num5, 1f, 1f, 0f, 0.05f, "metal", null, buffActions, attackHitInfo, 1, 0, 0f, null, null, attackMode, null, -1, null);
												int num7 = this.hitEffectCount - 1;
												this.hitEffectCount = num7;
												if (num7 <= 0)
												{
													attackMode = ItemActionAttack.EnumAttackMode.RealNoHarvestingOrEffects;
												}
												if (attackHitInfo.bBlockHit)
												{
													j = -999;
													k = -999;
													break;
												}
											}
										}
									}
								}
							}
						}
						if (attackHitInfo.bKilled && attackHitInfo.bBlockHit)
						{
							BlockModelTree blockModelTree = attackHitInfo.blockBeingDamaged.Block as BlockModelTree;
							if (blockModelTree != null && blockModelTree.isMultiBlock && blockModelTree.multiBlockPos.dim.y >= 12)
							{
								this.velocityMax *= 0.3f;
								this.vehicleRB.AddRelativeForce(Vector3.up * 2.5f, ForceMode.VelocityChange);
								this.vehicleRB.AddRelativeForce(this.collisionVelNorm * 2f, ForceMode.VelocityChange);
							}
						}
						if ((attackHitInfo.bKilled || !attackHitInfo.bBlockHit) && attackHitInfo.hardnessScale > 0f)
						{
							this.collisionIgnoreCount++;
						}
						num5 = Utils.FastMin(num5, (float)attackHitInfo.damageGiven);
					}
					float num8 = num5 * num6;
					num8 *= this.vehicle.EffectSelfDamagePer;
					if (flag)
					{
						num8 *= this.vehicle.EffectStrongSelfDamagePer;
					}
					this.damageAccumulator += num8 * num;
					if (num8 > 50f)
					{
						this.SpawnParticle("blockdestroy_metal", worldRayHitInfo.hit.pos);
					}
				}
				this.ApplyAccumulatedDamage();
				int num9 = this.collisionIgnoreCount - count;
				if (num9 >= 0)
				{
					this.PhysicsRevertCollisionMotion(num9);
				}
				this.contactPoints.Clear();
				this.collisionHits.Clear();
			}
		}
		yield break;
	}

	// Token: 0x060024E1 RID: 9441 RVA: 0x000EDAB4 File Offset: 0x000EBCB4
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyAccumulatedDamage()
	{
		if (this.damageAccumulator >= 1f)
		{
			int num = (int)this.damageAccumulator;
			this.damageAccumulator -= (float)num;
			this.ApplyDamage(num);
		}
	}

	// Token: 0x060024E2 RID: 9442 RVA: 0x000EDAEC File Offset: 0x000EBCEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnParticle(string _particleName, Vector3 _pos)
	{
		Vector3i blockPos = World.worldToBlockPos(_pos);
		float lightBrightness = this.world.GetLightBrightness(blockPos);
		this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(_particleName, _pos, lightBrightness, Color.white, null, null, false), this.entityId, false, false);
	}

	// Token: 0x060024E3 RID: 9443 RVA: 0x000EDB38 File Offset: 0x000EBD38
	[PublicizedFrom(EAccessModifier.Private)]
	public void SpawnParticle(string _particleName, int _entityId, float _offsetY)
	{
		Vector3 pos = new Vector3(0f, _offsetY, 0f);
		this.world.GetGameManager().SpawnParticleEffectServer(new ParticleEffect(_particleName, pos, 1f, Color.white, null, _entityId, ParticleEffect.Attachment.Pelvis), this.entityId, false, false);
	}

	// Token: 0x060024E4 RID: 9444 RVA: 0x000EDB84 File Offset: 0x000EBD84
	[PublicizedFrom(EAccessModifier.Private)]
	public void PhysicsRevertCollisionMotion(int _ignoreExcess)
	{
		if (_ignoreExcess == 0)
		{
			float num = Time.fixedDeltaTime * 0.5f;
			float num2 = this.lastRBVel.x * num;
			float num3 = this.lastRBVel.z * num;
			if (num2 < -0.0001f || num2 > 0.0001f || num3 < -0.0001f || num3 > 0.0001f)
			{
				this.lastRBPos.x = this.lastRBPos.x + num2;
				this.lastRBPos.z = this.lastRBPos.z + num3;
				this.vehicleRB.position = this.lastRBPos;
			}
		}
		Vector3 velocity = this.vehicleRB.velocity;
		velocity.x = this.lastRBVel.x * 0.9f;
		velocity.z = this.lastRBVel.z * 0.9f;
		velocity.y = this.lastRBVel.y * 0.6f + velocity.y * 0.4f;
		this.vehicleRB.velocity = velocity;
		this.vehicleRB.angularVelocity = this.lastRBAngVel;
	}

	// Token: 0x060024E5 RID: 9445 RVA: 0x000EDC8C File Offset: 0x000EBE8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void DrawRayHandle(Vector3 pos, Vector3 dir, Color color, float duration = 0f)
	{
		Vector3 normalized = Vector3.Cross(Vector3.up, dir).normalized;
		Debug.DrawRay(pos, normalized * 0.005f, Color.blue, duration);
		Debug.DrawRay(pos, dir, color, duration);
	}

	// Token: 0x060024E6 RID: 9446 RVA: 0x000EDCD0 File Offset: 0x000EBED0
	[PublicizedFrom(EAccessModifier.Private)]
	public void DrawBlocks(WorldRayHitInfo hitInfo)
	{
		ChunkCluster chunkCluster = GameManager.Instance.World.ChunkClusters[hitInfo.hit.clrIdx];
		if (chunkCluster == null)
		{
			return;
		}
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				for (int k = -1; k <= 1; k++)
				{
					Vector3i blockPos = hitInfo.hit.blockPos;
					blockPos.x += j;
					blockPos.y += i;
					blockPos.z += k;
					Vector3 start = blockPos.ToVector3() - Origin.position;
					BlockValue block = chunkCluster.GetBlock(blockPos);
					Color color = Color.black;
					if (!block.isair)
					{
						if (block.Block.shape.IsTerrain())
						{
							color = Color.yellow;
						}
						else
						{
							color = Color.white;
						}
					}
					Debug.DrawRay(start, Vector3.up, color);
					Debug.DrawRay(start, Vector3.right, color);
					Debug.DrawRay(start, Vector3.forward, color);
				}
			}
		}
	}

	// Token: 0x060024E7 RID: 9447 RVA: 0x000EDDDC File Offset: 0x000EBFDC
	[PublicizedFrom(EAccessModifier.Private)]
	public Entity FindEntity(Transform t)
	{
		Entity componentInChildren = t.GetComponentInChildren<Entity>();
		if (componentInChildren)
		{
			return componentInChildren;
		}
		return t.GetComponentInParent<Entity>();
	}

	// Token: 0x060024E8 RID: 9448 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void entityCollision(Vector3 _motion)
	{
	}

	// Token: 0x060024E9 RID: 9449 RVA: 0x000EDE04 File Offset: 0x000EC004
	public static EntityVehicle FindCollisionEntity(Transform t)
	{
		EntityVehicle entityVehicle = t.GetComponent<EntityVehicle>();
		if (!entityVehicle)
		{
			CollisionCallForward componentInParent = t.GetComponentInParent<CollisionCallForward>();
			if (componentInParent)
			{
				entityVehicle = (componentInParent.Entity as EntityVehicle);
			}
		}
		return entityVehicle;
	}

	// Token: 0x060024EA RID: 9450 RVA: 0x000EDE3C File Offset: 0x000EC03C
	public override float GetBlockDamageScale()
	{
		EntityAlive entityAlive = base.AttachedMainEntity as EntityAlive;
		if (entityAlive)
		{
			return entityAlive.GetBlockDamageScale();
		}
		return 1f;
	}

	// Token: 0x060024EB RID: 9451 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void switchModelView(EnumEntityModelView modelView)
	{
	}

	// Token: 0x060024EC RID: 9452 RVA: 0x00002914 File Offset: 0x00000B14
	public override void MoveEntityHeaded(Vector3 _direction, bool _isDirAbsolute)
	{
	}

	// Token: 0x060024ED RID: 9453 RVA: 0x000EDE6C File Offset: 0x000EC06C
	public override void MoveByAttachedEntity(EntityPlayerLocal _player)
	{
		if (this.movementInput == null)
		{
			return;
		}
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_player);
		if (uiforPlayer == null || uiforPlayer.playerInput == null)
		{
			return;
		}
		PlayerActionsVehicle vehicleActions = uiforPlayer.playerInput.VehicleActions;
		MovementInput movementInput = _player.movementInput;
		if (_player == base.AttachedMainEntity)
		{
			this.movementInput.moveForward = (_player.MoveController.isAutorun ? 1f : vehicleActions.Move.Y);
			this.movementInput.moveStrafe = vehicleActions.Move.X;
			this.movementInput.down = vehicleActions.Hop.IsPressed;
			this.movementInput.jump = vehicleActions.Brake.IsPressed;
			if (EffectManager.GetValue(PassiveEffects.FlipControls, null, 0f, _player, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false) > 0f)
			{
				this.movementInput.moveForward *= -1f;
				this.movementInput.moveStrafe *= -1f;
			}
			this.movementInput.running = _player.movementInput.running;
			this.movementInput.lastInputController = movementInput.lastInputController;
			if (vehicleActions.ToggleTurnMode.WasPressed && !uiforPlayer.windowManager.IsModalWindowOpen())
			{
				EntityVehicle.isTurnTowardsLook = !EntityVehicle.isTurnTowardsLook;
			}
		}
		MovementInput movementInput2 = movementInput;
		movementInput2.rotation.x = movementInput2.rotation.x * this.vehicle.CameraTurnRate.x;
		MovementInput movementInput3 = movementInput;
		movementInput3.rotation.y = movementInput3.rotation.y * this.vehicle.CameraTurnRate.y;
		float num = vehicleActions.Scroll.Value;
		if (vehicleActions.LastInputType == BindingSourceType.DeviceBindingSource)
		{
			num *= 0.25f;
		}
		if (num != 0f)
		{
			EntityVehicle.cameraDistScale += num * -0.5f;
			EntityVehicle.cameraDistScale = Utils.FastClamp(EntityVehicle.cameraDistScale, 0.3f, 1.2f);
			this.cameraOutTime = 999f;
		}
	}

	// Token: 0x060024EE RID: 9454 RVA: 0x000EE06C File Offset: 0x000EC26C
	public bool HasHeadlight()
	{
		VPHeadlight vpheadlight = this.vehicle.FindPart("headlight") as VPHeadlight;
		return vpheadlight != null && (vpheadlight.GetTransform() || vpheadlight.modInstalled);
	}

	// Token: 0x060024EF RID: 9455 RVA: 0x000EE0AA File Offset: 0x000EC2AA
	public void ToggleHeadlight()
	{
		this.IsHeadlightOn = !this.IsHeadlightOn;
	}

	// Token: 0x170003F4 RID: 1012
	// (get) Token: 0x060024F0 RID: 9456 RVA: 0x000EE0BC File Offset: 0x000EC2BC
	// (set) Token: 0x060024F1 RID: 9457 RVA: 0x000EE0EA File Offset: 0x000EC2EA
	public bool IsHeadlightOn
	{
		get
		{
			VPHeadlight vpheadlight = this.vehicle.FindPart("headlight") as VPHeadlight;
			return vpheadlight != null && vpheadlight.IsOn();
		}
		set
		{
			this.vehicle.FireEvent(VehiclePart.Event.LightsOn, null, (float)(value ? 1 : 0));
		}
	}

	// Token: 0x060024F2 RID: 9458 RVA: 0x000EE104 File Offset: 0x000EC304
	public override float GetLightLevel()
	{
		VPHeadlight vpheadlight = this.vehicle.FindPart("headlight") as VPHeadlight;
		if (vpheadlight == null)
		{
			return 0f;
		}
		return vpheadlight.GetLightLevel();
	}

	// Token: 0x060024F3 RID: 9459 RVA: 0x000EE138 File Offset: 0x000EC338
	public void UseHorn(EntityPlayerLocal player)
	{
		string hornSoundName = this.vehicle.GetHornSoundName();
		if (hornSoundName.Length > 0)
		{
			this.PlayOneShot(hornSoundName, false, false, false, null);
		}
		if (this.onHonkEvent != "")
		{
			GameEventManager.Current.HandleAction(this.onHonkEvent, null, player, false, this.position, "", "", false, true, "", null);
		}
	}

	// Token: 0x170003F5 RID: 1013
	// (get) Token: 0x060024F4 RID: 9460 RVA: 0x000EE1A3 File Offset: 0x000EC3A3
	public bool HasDriver
	{
		get
		{
			return this.hasDriver;
		}
	}

	// Token: 0x060024F5 RID: 9461 RVA: 0x000EE1AC File Offset: 0x000EC3AC
	[PublicizedFrom(EAccessModifier.Private)]
	public float CalcWaterDepth(float offsetY)
	{
		Vector3 position = this.position;
		position.y += offsetY;
		Vector3i vector3i = World.worldToBlockPos(position);
		if (this.world.IsWater(vector3i))
		{
			for (int i = 0; i < 5; i++)
			{
				vector3i.y++;
				if (!this.world.IsWater(vector3i))
				{
					break;
				}
			}
			return (float)vector3i.y - position.y;
		}
		return 0f;
	}

	// Token: 0x170003F6 RID: 1014
	// (set) Token: 0x060024F6 RID: 9462 RVA: 0x000EE21B File Offset: 0x000EC41B
	public override int Health
	{
		set
		{
			base.Stats.Health.Value = (float)value;
			if (this.vehicle != null)
			{
				this.vehicle.FireEvent(Vehicle.Event.HealthChanged);
			}
		}
	}

	// Token: 0x060024F7 RID: 9463 RVA: 0x000EE244 File Offset: 0x000EC444
	[PublicizedFrom(EAccessModifier.Protected)]
	public override DamageResponse damageEntityLocal(DamageSource _damageSource, int _strength, bool _criticalHit, float impulseScale)
	{
		DamageResponse damageResponse = new DamageResponse
		{
			Source = _damageSource,
			Strength = _strength,
			Critical = _criticalHit,
			HitDirection = Utils.EnumHitDirection.None,
			MovementState = this.MovementState,
			Random = this.rand.RandomFloat,
			ImpulseScale = impulseScale
		};
		this.ProcessDamageResponseLocal(damageResponse);
		return damageResponse;
	}

	// Token: 0x060024F8 RID: 9464 RVA: 0x000EE2AC File Offset: 0x000EC4AC
	public override void ProcessDamageResponseLocal(DamageResponse _dmResponse)
	{
		DamageSource source = _dmResponse.Source;
		if (source.damageType == EnumDamageTypes.Disease || source.damageType == EnumDamageTypes.Suffocation)
		{
			return;
		}
		this.UpdateInteractionUI();
		int strength = _dmResponse.Strength;
		if (base.AttachedMainEntity && !this.isEntityRemote && this.world.IsWorldEvent(World.WorldEvent.BloodMoon))
		{
			this.velocityMax *= 0.6f;
			this.vehicleRB.AddRelativeForce(_dmResponse.Source.getDirection() * 6f, ForceMode.VelocityChange);
		}
		if (this.attachedEntities != null && _dmResponse.Source.GetSource() == EnumDamageSource.External)
		{
			int strength2 = Utils.FastRoundToInt((float)_dmResponse.Strength * this.vehicle.GetPlayerDamagePercent());
			DamageSource damageSource = new DamageSource(EnumDamageSource.External, EnumDamageTypes.Bashing);
			foreach (Entity entity in this.attachedEntities)
			{
				if (entity != null)
				{
					entity.DamageEntity(damageSource, strength2, false, 1f);
				}
			}
		}
		this.ApplyDamage(strength);
	}

	// Token: 0x060024F9 RID: 9465 RVA: 0x000EE3AC File Offset: 0x000EC5AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyDamage(int damage)
	{
		int num = this.Health;
		if (num <= 0)
		{
			return;
		}
		bool flag = damage >= 99999;
		if (num == 1 || flag)
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.explodeHealth -= (float)damage;
				if (this.explodeHealth <= 0f && (flag || this.rand.RandomFloat < 0.2f))
				{
					this.DropItemsAsBackpack();
					this.Kill();
					GameManager.Instance.ExplosionServer(0, base.GetPosition(), World.worldToBlockPos(base.GetPosition()), base.transform.rotation, EntityClass.list[this.entityClass].explosionData, this.entityId, 0f, false, null);
					return;
				}
			}
		}
		else
		{
			num -= damage;
			if (num <= 1)
			{
				num = 1;
				this.explodeHealth = (float)this.vehicle.GetMaxHealth() * 0.03f;
			}
			this.Health = num;
		}
	}

	// Token: 0x060024FA RID: 9466 RVA: 0x000EE4A0 File Offset: 0x000EC6A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplyCollisionDamageToAttached(int damage)
	{
		DamageSource damageSource = new DamageSource(EnumDamageSource.Internal, EnumDamageTypes.VehicleInside);
		int attachMaxCount = base.GetAttachMaxCount();
		for (int i = 0; i < attachMaxCount; i++)
		{
			Entity attached = base.GetAttached(i);
			if (attached)
			{
				attached.DamageEntity(damageSource, damage, false, 1f);
			}
		}
	}

	// Token: 0x060024FB RID: 9467 RVA: 0x000EE4E8 File Offset: 0x000EC6E8
	public override bool HasImmunity(BuffClass _buffClass)
	{
		return _buffClass.DamageType != EnumDamageTypes.Heat;
	}

	// Token: 0x060024FC RID: 9468 RVA: 0x000EE4F8 File Offset: 0x000EC6F8
	public bool IsLockedForLocalPlayer(EntityAlive _entityFocusing)
	{
		bool flag = this.LocalPlayerIsOwner();
		return this.isLocked && !flag && this.hasLock() && !this.isAllowedUser(PlatformManager.InternalLocalUserIdentifier);
	}

	// Token: 0x060024FD RID: 9469 RVA: 0x000EE534 File Offset: 0x000EC734
	public override EntityActivationCommand[] GetActivationCommands(Vector3i _tePos, EntityAlive _entityFocusing)
	{
		if (this.IsDead())
		{
			return new EntityActivationCommand[0];
		}
		bool flag = this.LocalPlayerIsOwner();
		bool flag2 = !this.isLocked || flag || !this.hasLock() || this.isAllowedUser(PlatformManager.InternalLocalUserIdentifier);
		bool flag3 = base.CanAttach(_entityFocusing) && this.isDriveable();
		bool flag4 = base.IsDriven();
		EntityActivationCommand entityActivationCommand;
		if (!flag4)
		{
			entityActivationCommand = new EntityActivationCommand("drive", "drive", flag3 && flag2, null);
		}
		else
		{
			entityActivationCommand = new EntityActivationCommand("ride", "drive", flag3 && flag2, null);
		}
		return new EntityActivationCommand[]
		{
			entityActivationCommand,
			new EntityActivationCommand("service", "service", flag2, null),
			new EntityActivationCommand("repair", "wrench", this.vehicle.GetRepairAmountNeeded() > 0, null),
			new EntityActivationCommand("lock", "lock", this.hasLock() && !this.isLocked && !flag4, null),
			new EntityActivationCommand("unlock", "unlock", this.hasLock() && this.isLocked && flag, null),
			new EntityActivationCommand("keypad", "keypad", this.hasLock() && this.isLocked && (flag || this.vehicle.PasswordHash != 0), null),
			new EntityActivationCommand("refuel", "gas", this.hasGasCan(_entityFocusing) && this.needsFuel(), null),
			new EntityActivationCommand("take", "hand", !this.hasDriver && flag2, null),
			new EntityActivationCommand("horn", "horn", this.vehicle.HasHorn(), null),
			new EntityActivationCommand("storage", "loot_sack", flag2, null)
		};
	}

	// Token: 0x060024FE RID: 9470 RVA: 0x000EE730 File Offset: 0x000EC930
	public override bool OnEntityActivated(int _indexInBlockActivationCommands, Vector3i _tePos, EntityAlive _entityFocusing)
	{
		EntityPlayerLocal entityPlayerLocal = _entityFocusing as EntityPlayerLocal;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		if (entityPlayerLocal.inventory.IsHoldingItemActionRunning() || uiforPlayer.xui.isUsingItemActionEntryUse)
		{
			return false;
		}
		int num = -1;
		switch (_indexInBlockActivationCommands)
		{
		case 0:
			if ((!(uiforPlayer != null) || !uiforPlayer.windowManager.IsWindowOpen("windowpaging")) && base.CanAttach(_entityFocusing) && _entityFocusing.AttachedToEntity == null && this.isDriveable() && (!this.isLocked || !this.hasLock() || this.LocalPlayerIsOwner() || this.isAllowedUser(PlatformManager.InternalLocalUserIdentifier)))
			{
				if (EffectManager.GetValue(PassiveEffects.NoVehicle, null, 0f, entityPlayerLocal, null, base.EntityClass.Tags, true, true, true, true, true, 1, true, false) > 0f)
				{
					Manager.PlayInsidePlayerHead("twitch_no_attack", -1, 0f, false, false);
				}
				else
				{
					Vector3 vector = this.position - Origin.position;
					vector.y += 0.5f;
					Vector3 up = Vector3.up;
					bool flag = false;
					for (int i = 0; i < 8; i++)
					{
						Vector3 a = Quaternion.AngleAxis((float)(i * 45), up) * base.transform.forward;
						if (Physics.Raycast(vector + a * 0.25f, up, 1.3f, 65536))
						{
							flag = true;
							Vector3 vector2 = _entityFocusing.position - Origin.position;
							vector2.y += 1.1f;
							vector2 = (vector2 - vector).normalized * this.vehicleRB.mass * 0.005f;
							this.AddForce(vector2, ForceMode.VelocityChange);
							break;
						}
					}
					if (!flag)
					{
						this.EnterVehicle(_entityFocusing);
					}
				}
			}
			break;
		case 1:
			num = _indexInBlockActivationCommands;
			break;
		case 2:
			num = _indexInBlockActivationCommands;
			break;
		case 3:
			this.vehicle.SetLocked(true, entityPlayerLocal);
			this.PlayOneShot("misc/locking", true, false, false, null);
			this.SendSyncData(2);
			break;
		case 4:
			this.vehicle.SetLocked(false, entityPlayerLocal);
			this.PlayOneShot("misc/unlocking", true, false, false, null);
			this.SendSyncData(2);
			break;
		case 5:
			this.PlayOneShot("misc/password_type", true, false, false, null);
			XUiC_KeypadWindow.Open(uiforPlayer, this);
			break;
		case 6:
			num = _indexInBlockActivationCommands;
			break;
		case 7:
			num = _indexInBlockActivationCommands;
			break;
		case 8:
			this.UseHorn(entityPlayerLocal);
			break;
		case 9:
			num = _indexInBlockActivationCommands;
			break;
		}
		if (num >= 0)
		{
			this.interactionRequestType = num;
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				this.ValidateInteractingPlayer();
				int entityId = this.interactingPlayerId;
				if (entityId == -1)
				{
					entityId = entityPlayerLocal.entityId;
				}
				this.StartInteraction(entityPlayerLocal.entityId, entityId);
			}
			else
			{
				this.interactingPlayerId = entityPlayerLocal.entityId;
				this.SendSyncData(4096);
				this.interactingPlayerId = -1;
			}
		}
		return false;
	}

	// Token: 0x060024FF RID: 9471 RVA: 0x000EEA20 File Offset: 0x000ECC20
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckInteractionRequest(int _playerId, int _requestId)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (_requestId != -1)
			{
				this.ValidateInteractingPlayer();
				ushort num = 4096;
				if (this.interactingPlayerId == -1)
				{
					this.interactingPlayerId = _playerId;
					num |= 14;
				}
				NetPackageVehicleDataSync package = NetPackageManager.GetPackage<NetPackageVehicleDataSync>().Setup(this, _playerId, num);
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, _playerId, -1, -1, null, 192, false);
				return;
			}
			if (this.interactingPlayerId == _playerId)
			{
				this.interactingPlayerId = -1;
				return;
			}
		}
		else
		{
			this.StartInteraction(_playerId, _requestId);
		}
	}

	// Token: 0x06002500 RID: 9472 RVA: 0x000EEAA5 File Offset: 0x000ECCA5
	[PublicizedFrom(EAccessModifier.Private)]
	public void ValidateInteractingPlayer()
	{
		if (!GameManager.Instance.World.GetEntity(this.interactingPlayerId))
		{
			this.interactingPlayerId = -1;
		}
	}

	// Token: 0x06002501 RID: 9473 RVA: 0x000EEACC File Offset: 0x000ECCCC
	[PublicizedFrom(EAccessModifier.Private)]
	public void StartInteraction(int _playerId, int _requestId)
	{
		EntityPlayerLocal localPlayerFromID = GameManager.Instance.World.GetLocalPlayerFromID(_playerId);
		if (!localPlayerFromID)
		{
			return;
		}
		if (_requestId != _playerId)
		{
			GameManager.ShowTooltip(localPlayerFromID, Localization.Get("ttVehicleInUse", false), string.Empty, "ui_denied", null, false, false, 0f);
			return;
		}
		this.interactingPlayerId = _playerId;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(localPlayerFromID);
		GUIWindowManager windowManager = uiforPlayer.windowManager;
		ushort num = 0;
		switch (this.interactionRequestType)
		{
		case 1:
			((XUiC_VehicleWindowGroup)((XUiWindowGroup)windowManager.GetWindow("vehicle")).Controller).CurrentVehicleEntity = this;
			windowManager.Open("vehicle", true, false, true);
			Manager.BroadcastPlayByLocalPlayer(this.position, "UseActions/service_vehicle");
			return;
		case 2:
			if (XUiM_Vehicle.RepairVehicle(uiforPlayer.xui, this.vehicle))
			{
				num |= 4;
				this.PlayOneShot("crafting/craft_repair_item", true, false, false, null);
			}
			this.StopInteraction(num);
			return;
		case 3:
		case 4:
		case 5:
		case 8:
			break;
		case 6:
			if (this.AddFuelFromInventory(localPlayerFromID))
			{
				num |= 4;
			}
			this.StopInteraction(num);
			return;
		case 7:
			if (!this.bag.IsEmpty())
			{
				GameManager.ShowTooltip(localPlayerFromID, Localization.Get("ttEmptyVehicleBeforePickup", false), string.Empty, "ui_denied", null, false, false, 0f);
				this.StopInteraction(0);
				return;
			}
			if (!this.hasDriver)
			{
				ItemStack itemStack = new ItemStack(this.vehicle.GetUpdatedItemValue(), 1);
				if (localPlayerFromID.inventory.CanTakeItem(itemStack) || localPlayerFromID.bag.CanTakeItem(itemStack))
				{
					GameManager.Instance.CollectEntityServer(this.entityId, localPlayerFromID.entityId);
				}
				else
				{
					GameManager.ShowTooltip(localPlayerFromID, Localization.Get("xuiInventoryFullForPickup", false), string.Empty, "ui_denied", null, false, false, 0f);
				}
			}
			this.StopInteraction(0);
			return;
		case 9:
			((XUiC_VehicleStorageWindowGroup)((XUiWindowGroup)windowManager.GetWindow("vehicleStorage")).Controller).CurrentVehicleEntity = this;
			windowManager.Open("vehicleStorage", true, false, true);
			break;
		default:
			return;
		}
	}

	// Token: 0x06002502 RID: 9474 RVA: 0x000EECD0 File Offset: 0x000ECED0
	public bool CheckUIInteraction()
	{
		EntityPlayerLocal localPlayerFromID = GameManager.Instance.World.GetLocalPlayerFromID(this.interactingPlayerId);
		if (!localPlayerFromID)
		{
			return false;
		}
		float distanceSq = base.GetDistanceSq(localPlayerFromID);
		float num = Constants.cDigAndBuildDistance + 1f;
		return distanceSq <= num * num;
	}

	// Token: 0x06002503 RID: 9475 RVA: 0x000EED18 File Offset: 0x000ECF18
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateInteractionUI()
	{
		if (GameManager.Instance.World == null)
		{
			return;
		}
		for (int i = 0; i < LocalPlayerUI.PlayerUIs.Count; i++)
		{
			LocalPlayerUI localPlayerUI = LocalPlayerUI.PlayerUIs[i];
			if (localPlayerUI != null && localPlayerUI.xui != null && localPlayerUI.windowManager.IsWindowOpen("vehicle"))
			{
				XUiWindowGroup xuiWindowGroup = (XUiWindowGroup)localPlayerUI.windowManager.GetWindow("vehicle");
				if (xuiWindowGroup != null && xuiWindowGroup.Controller != null)
				{
					xuiWindowGroup.Controller.RefreshBindingsSelfAndChildren();
				}
			}
		}
	}

	// Token: 0x06002504 RID: 9476 RVA: 0x000EEDA8 File Offset: 0x000ECFA8
	public void StopUIInteraction()
	{
		this.StopInteraction(14);
	}

	// Token: 0x06002505 RID: 9477 RVA: 0x000EEDB2 File Offset: 0x000ECFB2
	[PublicizedFrom(EAccessModifier.Private)]
	public void StopInteraction(ushort syncFlags = 0)
	{
		this.interactingPlayerId = -1;
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			syncFlags |= 4096;
		}
		if (syncFlags != 0)
		{
			this.SendSyncData(syncFlags);
		}
	}

	// Token: 0x06002506 RID: 9478 RVA: 0x000EEDDC File Offset: 0x000ECFDC
	public void Collect(int _playerId)
	{
		EntityPlayerLocal entityPlayerLocal = this.world.GetEntity(_playerId) as EntityPlayerLocal;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(entityPlayerLocal);
		ItemStack itemStack = new ItemStack(this.vehicle.GetUpdatedItemValue(), 1);
		if (!uiforPlayer.xui.PlayerInventory.AddItem(itemStack))
		{
			GameManager.Instance.ItemDropServer(itemStack, entityPlayerLocal.GetPosition(), Vector3.zero, _playerId, 60f, false);
		}
	}

	// Token: 0x06002507 RID: 9479 RVA: 0x000EEE44 File Offset: 0x000ED044
	[PublicizedFrom(EAccessModifier.Private)]
	public void DropItemsAsBackpack()
	{
		List<ItemStack> list = new List<ItemStack>();
		foreach (ItemStack itemStack in this.bag.GetSlots())
		{
			if (!itemStack.IsEmpty())
			{
				list.Add(itemStack);
			}
		}
		ItemValue updatedItemValue = this.vehicle.GetUpdatedItemValue();
		for (int j = 0; j < updatedItemValue.CosmeticMods.Length; j++)
		{
			ItemValue itemValue = updatedItemValue.CosmeticMods[j];
			if (itemValue != null && !itemValue.IsEmpty())
			{
				list.Add(new ItemStack(itemValue, 1));
			}
		}
		for (int k = 0; k < updatedItemValue.Modifications.Length; k++)
		{
			ItemValue itemValue2 = updatedItemValue.Modifications[k];
			if (itemValue2 != null && !itemValue2.IsEmpty())
			{
				list.Add(new ItemStack(itemValue2, 1));
			}
		}
		Vector3 position = this.position;
		position.y += 0.9f;
		EntityLootContainer entityLootContainer = GameManager.Instance.DropContentInLootContainerServer(-1, "DroppedVehicleContainer", position, list.ToArray(), false);
		if (entityLootContainer)
		{
			Vector3 vector = this.rand.RandomOnUnitSphere * 16f;
			vector.y = Utils.FastAbs(vector.y);
			vector.y += 8f;
			entityLootContainer.AddVelocity(vector);
		}
	}

	// Token: 0x06002508 RID: 9480 RVA: 0x000EEF8F File Offset: 0x000ED18F
	public void AddMaxFuel()
	{
		this.vehicle.AddFuel(this.vehicle.GetMaxFuelLevel());
	}

	// Token: 0x06002509 RID: 9481 RVA: 0x000EEFA8 File Offset: 0x000ED1A8
	public bool AddFuelFromInventory(EntityAlive entity)
	{
		if (this.vehicle.GetFuelPercent() < 1f)
		{
			float maxFuelLevel = this.vehicle.GetMaxFuelLevel();
			float fuelLevel = this.vehicle.GetFuelLevel();
			float f = Mathf.Min(2500f, (maxFuelLevel - fuelLevel) * 25f);
			float num = this.takeFuel(entity, Mathf.CeilToInt(f));
			this.vehicle.AddFuel(num / 25f);
			this.PlayOneShot("useactions/gas_refill", false, false, false, null);
			return true;
		}
		return false;
	}

	// Token: 0x0600250A RID: 9482 RVA: 0x000EF025 File Offset: 0x000ED225
	public int GetFuelCount()
	{
		return Mathf.FloorToInt(this.vehicle.GetFuelLevel() * 25f);
	}

	// Token: 0x0600250B RID: 9483 RVA: 0x000EF040 File Offset: 0x000ED240
	[PublicizedFrom(EAccessModifier.Private)]
	public float takeFuel(EntityAlive _entityFocusing, int count)
	{
		EntityPlayer entityPlayer = _entityFocusing as EntityPlayer;
		if (!entityPlayer)
		{
			return 0f;
		}
		string fuelItem = this.GetVehicle().GetFuelItem();
		if (fuelItem == "")
		{
			return 0f;
		}
		ItemValue item = ItemClass.GetItem(fuelItem, false);
		int num = entityPlayer.inventory.DecItem(item, count, false, null);
		if (num == 0)
		{
			num = entityPlayer.bag.DecItem(item, count, false, null);
			if (num == 0)
			{
				return 0f;
			}
		}
		float num2 = (float)num;
		LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(_entityFocusing as EntityPlayerLocal);
		if (null != uiforPlayer)
		{
			ItemStack @is = new ItemStack(item, num);
			uiforPlayer.xui.CollectedItemList.RemoveItemStack(@is);
		}
		else
		{
			Log.Warning("EntityVehicle::takeFuel - Failed to remove item stack from player's collected item list.");
		}
		return num2;
	}

	// Token: 0x0600250C RID: 9484 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isEntityStatic()
	{
		return true;
	}

	// Token: 0x0600250D RID: 9485 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool CanBePushed()
	{
		return false;
	}

	// Token: 0x0600250E RID: 9486 RVA: 0x000EF0F6 File Offset: 0x000ED2F6
	public Vehicle GetVehicle()
	{
		return this.vehicle;
	}

	// Token: 0x0600250F RID: 9487 RVA: 0x000EF0FE File Offset: 0x000ED2FE
	public void SetBagModified()
	{
		this.SendSyncData(8);
	}

	// Token: 0x06002510 RID: 9488 RVA: 0x000EF108 File Offset: 0x000ED308
	[PublicizedFrom(EAccessModifier.Private)]
	public void SendSyncData(ushort syncFlags)
	{
		NetPackageVehicleDataSync package = NetPackageManager.GetPackage<NetPackageVehicleDataSync>().Setup(this, GameManager.Instance.World.GetPrimaryPlayerId(), syncFlags);
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(package, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, -1, -1, -1, null, 192, false);
	}

	// Token: 0x06002511 RID: 9489 RVA: 0x000EF168 File Offset: 0x000ED368
	public ushort GetSyncFlagsReplicated(ushort syncFlags)
	{
		return syncFlags & 49159;
	}

	// Token: 0x06002512 RID: 9490 RVA: 0x000EF174 File Offset: 0x000ED374
	public override void Read(byte _version, BinaryReader _br)
	{
		base.Read(_version, _br);
		if (_version < 26)
		{
			Log.Warning("Vehicle: Ignoring old data v{0}", new object[]
			{
				_version
			});
			return;
		}
		ushort syncFlags = _br.ReadUInt16();
		this.ReadSyncData(_br, syncFlags, 0);
	}

	// Token: 0x06002513 RID: 9491 RVA: 0x000EF1B8 File Offset: 0x000ED3B8
	public void ReadSyncData(BinaryReader _br, ushort syncFlags, int senderId)
	{
		byte b = _br.ReadByte();
		if ((syncFlags & 32768) > 0)
		{
			this.incomingRemoteData.Flags = _br.ReadInt32();
			this.incomingRemoteData.Flags = (this.incomingRemoteData.Flags | 1);
			this.incomingRemoteData.MotorTorquePercent = (float)_br.ReadInt16() * 0.0001f;
			this.incomingRemoteData.SteeringPercent = (float)_br.ReadInt16() * 0.0001f;
			this.incomingRemoteData.Velocity = StreamUtils.ReadVector3(_br);
			List<EntityVehicle.RemoteData.Part> list = new List<EntityVehicle.RemoteData.Part>(4);
			this.incomingRemoteData.parts = list;
			for (;;)
			{
				byte b2 = _br.ReadByte();
				if (b2 == 0)
				{
					break;
				}
				EntityVehicle.RemoteData.Part item;
				if (b2 == 2)
				{
					item.pos = StreamUtils.ReadVector3(_br);
				}
				else
				{
					item.pos = Vector3.zero;
				}
				item.rot = StreamUtils.ReadQuaterion(_br);
				list.Add(item);
			}
		}
		if ((syncFlags & 16384) > 0)
		{
			this.IsHeadlightOn = _br.ReadBoolean();
		}
		if ((syncFlags & 1) > 0)
		{
			this.delayedAttachments.Clear();
			int num = (int)_br.ReadByte();
			for (int i = 0; i < num; i++)
			{
				int num2 = _br.ReadInt32();
				if (num2 != -1)
				{
					EntityVehicle.DelayedAttach item2;
					item2.entityId = num2;
					item2.slot = i;
					this.delayedAttachments.Add(item2);
				}
				else
				{
					Entity attached = base.GetAttached(i);
					if (attached)
					{
						attached.Detach();
					}
				}
			}
		}
		if ((syncFlags & 2) > 0)
		{
			byte b3 = _br.ReadByte();
			this.isInteractionLocked = ((b3 & 1) > 0);
			this.isLocked = ((b3 & 2) > 0);
			this.vehicle.OwnerId = PlatformUserIdentifierAbs.FromStream(_br, false, false);
			this.vehicle.PasswordHash = _br.ReadInt32();
			this.vehicle.AllowedUsers.Clear();
			int num3 = (int)_br.ReadByte();
			for (int j = 0; j < num3; j++)
			{
				this.vehicle.AllowedUsers.Add(PlatformUserIdentifierAbs.FromStream(_br, false, false));
			}
		}
		if ((syncFlags & 4) > 0)
		{
			int num4 = (int)_br.ReadByte();
			ItemStack[] array = new ItemStack[num4];
			for (int k = 0; k < num4; k++)
			{
				ItemStack itemStack = new ItemStack();
				itemStack.Read(_br);
				array[k] = itemStack;
			}
			this.vehicle.LoadItems(array);
		}
		if ((syncFlags & 8) > 0)
		{
			int num5 = (int)_br.ReadByte();
			ItemStack[] array2 = new ItemStack[num5];
			for (int l = 0; l < num5; l++)
			{
				ItemStack itemStack2 = new ItemStack();
				array2[l] = itemStack2.Read(_br);
			}
			this.bag.SetSlots(array2);
			if (b >= 1)
			{
				if (_br.ReadBoolean())
				{
					this.bag.LockedSlots = new PackedBoolArray(0);
					this.bag.LockedSlots.Read(_br);
				}
				else
				{
					this.bag.LockedSlots = new PackedBoolArray(this.bag.SlotCount);
				}
			}
		}
		if ((syncFlags & 4096) > 0)
		{
			int requestId = _br.ReadInt32();
			this.CheckInteractionRequest(senderId, requestId);
		}
	}

	// Token: 0x06002514 RID: 9492 RVA: 0x000EF4A4 File Offset: 0x000ED6A4
	public override void Write(BinaryWriter _bw, bool _bNetworkWrite)
	{
		base.Write(_bw, _bNetworkWrite);
		ushort num = _bNetworkWrite ? 16399 : 16398;
		_bw.Write(num);
		this.WriteSyncData(_bw, num);
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x000EF4D8 File Offset: 0x000ED6D8
	public void WriteSyncData(BinaryWriter _bw, ushort syncFlags)
	{
		_bw.Write(1);
		if ((syncFlags & 32768) > 0)
		{
			int num = 0;
			if (this.vehicle.CurrentIsAccel)
			{
				num |= 2;
			}
			if (this.vehicle.CurrentIsBreak)
			{
				num |= 4;
			}
			_bw.Write(num);
			_bw.Write((short)(this.vehicle.CurrentMotorTorquePercent * 10000f));
			_bw.Write((short)(this.vehicle.CurrentSteeringPercent * 10000f));
			StreamUtils.Write(_bw, this.vehicle.CurrentVelocity);
			int num2 = this.wheels.Length;
			for (int i = 0; i < num2; i++)
			{
				EntityVehicle.Wheel wheel = this.wheels[i];
				if (wheel.steerT && wheel.isSteerParentOfTire)
				{
					_bw.Write(1);
					StreamUtils.Write(_bw, wheel.steerT.localRotation);
				}
				if (wheel.tireT)
				{
					_bw.Write(2);
					StreamUtils.Write(_bw, wheel.tireT.localPosition);
					StreamUtils.Write(_bw, wheel.tireT.localRotation);
				}
			}
			_bw.Write(0);
		}
		if ((syncFlags & 16384) > 0)
		{
			_bw.Write(this.IsHeadlightOn);
		}
		if ((syncFlags & 1) > 0)
		{
			int attachMaxCount = base.GetAttachMaxCount();
			_bw.Write((byte)attachMaxCount);
			for (int j = 0; j < attachMaxCount; j++)
			{
				Entity attached = base.GetAttached(j);
				_bw.Write(attached ? attached.entityId : -1);
			}
		}
		if ((syncFlags & 2) > 0)
		{
			byte b = 0;
			if (this.isInteractionLocked)
			{
				b |= 1;
			}
			if (this.isLocked)
			{
				b |= 2;
			}
			_bw.Write(b);
			this.vehicle.OwnerId.ToStream(_bw, false);
			_bw.Write(this.vehicle.PasswordHash);
			_bw.Write((byte)this.vehicle.AllowedUsers.Count);
			for (int k = 0; k < this.vehicle.AllowedUsers.Count; k++)
			{
				this.vehicle.AllowedUsers[k].ToStream(_bw, false);
			}
		}
		if ((syncFlags & 4) > 0)
		{
			_bw.Write(1);
			this.vehicle.GetItems()[0].Write(_bw);
		}
		if ((syncFlags & 8) > 0)
		{
			ItemStack[] slots = this.bag.GetSlots();
			_bw.Write((byte)slots.Length);
			for (int l = 0; l < slots.Length; l++)
			{
				slots[l].Write(_bw);
			}
			_bw.Write(this.bag.LockedSlots != null);
			PackedBoolArray lockedSlots = this.bag.LockedSlots;
			if (lockedSlots != null)
			{
				lockedSlots.Write(_bw);
			}
		}
		if ((syncFlags & 4096) > 0)
		{
			_bw.Write(this.interactingPlayerId);
		}
	}

	// Token: 0x06002516 RID: 9494 RVA: 0x000EF78F File Offset: 0x000ED98F
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isDriveable()
	{
		return this.vehicle.IsDriveable();
	}

	// Token: 0x06002517 RID: 9495 RVA: 0x000EF79C File Offset: 0x000ED99C
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool hasStorage()
	{
		return this.vehicle.HasStorage();
	}

	// Token: 0x06002518 RID: 9496 RVA: 0x000EF7A9 File Offset: 0x000ED9A9
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool hasHandlebars()
	{
		return this.vehicle.HasSteering();
	}

	// Token: 0x06002519 RID: 9497 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool HasChassis()
	{
		return true;
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x000EF7B6 File Offset: 0x000ED9B6
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool needsFuel()
	{
		return this.vehicle.HasEnginePart() && this.vehicle.GetFuelPercent() < 1f;
	}

	// Token: 0x0600251B RID: 9499 RVA: 0x000EF7DC File Offset: 0x000ED9DC
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool hasGasCan(EntityAlive _ea)
	{
		string fuelItem = this.GetVehicle().GetFuelItem();
		if (fuelItem == "")
		{
			return false;
		}
		ItemValue item = ItemClass.GetItem(fuelItem, false);
		int num = 0;
		ItemStack[] slots = _ea.bag.GetSlots();
		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].itemValue.type == item.type)
			{
				num++;
			}
		}
		for (int j = 0; j < _ea.inventory.PUBLIC_SLOTS; j++)
		{
			if (_ea.inventory.GetItem(j).itemValue.type == item.type)
			{
				num++;
			}
		}
		return num > 0;
	}

	// Token: 0x0600251C RID: 9500 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool hasLock()
	{
		return true;
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x000EF887 File Offset: 0x000EDA87
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isAllowedUser(PlatformUserIdentifierAbs _userIdentifier)
	{
		return this.vehicle.AllowedUsers.Contains(_userIdentifier);
	}

	// Token: 0x0600251E RID: 9502 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void PlayStepSound(string stepSound, float _volume)
	{
	}

	// Token: 0x0600251F RID: 9503 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void updateTasks()
	{
	}

	// Token: 0x06002520 RID: 9504 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool canDespawn()
	{
		return false;
	}

	// Token: 0x06002521 RID: 9505 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isRadiationSensitive()
	{
		return false;
	}

	// Token: 0x06002522 RID: 9506 RVA: 0x000EF89C File Offset: 0x000EDA9C
	public bool CheckPassword(string _password, PlatformUserIdentifierAbs _steamId, out bool changed)
	{
		changed = false;
		bool flag = Utils.HashString(_password) == this.vehicle.PasswordHash.ToString();
		if (this.LocalPlayerIsOwner())
		{
			if (!flag)
			{
				changed = true;
				this.vehicle.PasswordHash = _password.GetHashCode();
				this.vehicle.AllowedUsers.Clear();
				if (this.vehicle.OwnerId == null)
				{
					this.SetOwner(_steamId);
					this.isLocked = true;
				}
				this.SendSyncData(2);
			}
			return true;
		}
		if (flag)
		{
			this.vehicle.AllowedUsers.Add(_steamId);
			this.SendSyncData(2);
			return true;
		}
		return false;
	}

	// Token: 0x170003F7 RID: 1015
	// (get) Token: 0x06002523 RID: 9507 RVA: 0x000D52DF File Offset: 0x000D34DF
	// (set) Token: 0x06002524 RID: 9508 RVA: 0x00002914 File Offset: 0x00000B14
	public int EntityId
	{
		get
		{
			return this.entityId;
		}
		set
		{
		}
	}

	// Token: 0x06002525 RID: 9509 RVA: 0x000EF939 File Offset: 0x000EDB39
	public bool IsLocked()
	{
		return this.isLocked;
	}

	// Token: 0x06002526 RID: 9510 RVA: 0x000EF941 File Offset: 0x000EDB41
	public void SetLocked(bool _isLocked)
	{
		this.isLocked = _isLocked;
	}

	// Token: 0x06002527 RID: 9511 RVA: 0x000EF94A File Offset: 0x000EDB4A
	public PlatformUserIdentifierAbs GetOwner()
	{
		return this.vehicle.OwnerId;
	}

	// Token: 0x06002528 RID: 9512 RVA: 0x000EF957 File Offset: 0x000EDB57
	public override void OnAddedToWorld()
	{
		this.bSpawned = true;
		this.HandleNavObject();
	}

	// Token: 0x06002529 RID: 9513 RVA: 0x000EF966 File Offset: 0x000EDB66
	public void SetOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		this.vehicle.OwnerId = _userIdentifier;
	}

	// Token: 0x0600252A RID: 9514 RVA: 0x000EF887 File Offset: 0x000EDA87
	public bool IsUserAllowed(PlatformUserIdentifierAbs _userIdentifier)
	{
		return this.vehicle.AllowedUsers.Contains(_userIdentifier);
	}

	// Token: 0x0600252B RID: 9515 RVA: 0x000D5333 File Offset: 0x000D3533
	public List<PlatformUserIdentifierAbs> GetUsers()
	{
		return new List<PlatformUserIdentifierAbs>();
	}

	// Token: 0x0600252C RID: 9516 RVA: 0x000EF974 File Offset: 0x000EDB74
	public bool LocalPlayerIsOwner()
	{
		return this.IsOwner(PlatformManager.InternalLocalUserIdentifier);
	}

	// Token: 0x0600252D RID: 9517 RVA: 0x000EF981 File Offset: 0x000EDB81
	public bool IsOwner(PlatformUserIdentifierAbs _userIdentifier)
	{
		return this.vehicle.OwnerId == null || this.vehicle.OwnerId.Equals(_userIdentifier);
	}

	// Token: 0x0600252E RID: 9518 RVA: 0x000EF9A3 File Offset: 0x000EDBA3
	public bool HasPassword()
	{
		return this.vehicle.PasswordHash != 0;
	}

	// Token: 0x0600252F RID: 9519 RVA: 0x000EF9B3 File Offset: 0x000EDBB3
	public string GetPassword()
	{
		return this.vehicle.PasswordHash.ToString();
	}

	// Token: 0x06002530 RID: 9520 RVA: 0x000EF9C8 File Offset: 0x000EDBC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckForOutOfWorld()
	{
		if (this.bDead)
		{
			return;
		}
		Vector3 vector = this.position;
		if (this.world.AdjustBoundsForPlayers(ref vector, 0.2f))
		{
			if (!this.vehicleRB.isKinematic)
			{
				Vector3 velocity = this.vehicleRB.velocity;
				velocity.x *= -0.5f;
				velocity.z *= -0.5f;
				this.vehicleRB.velocity = velocity;
			}
			vector.y = this.vehicleRB.position.y + Origin.position.y;
			this.SetPosition(vector, true);
			EntityPlayerLocal attachedPlayerLocal = base.GetAttachedPlayerLocal();
			if (attachedPlayerLocal)
			{
				GameManager.ShowTooltip(attachedPlayerLocal, Localization.Get("ttWorldEnd", false), false, false, 0f);
			}
			return;
		}
		Vector3 centerPosition = this.GetCenterPosition();
		Chunk chunk = (Chunk)this.world.GetChunkFromWorldPos((int)centerPosition.x, (int)centerPosition.y, (int)centerPosition.z);
		if (chunk == null || !chunk.IsCollisionMeshGenerated || !chunk.IsDisplayed)
		{
			if (!this.vehicleRB.isKinematic)
			{
				this.vehicleRB.velocity = Vector3.zero;
				this.vehicleRB.angularVelocity = Vector3.zero;
			}
			if (!this.hasDriver)
			{
				this.RBActive = false;
				this.isTryToFall = true;
			}
			return;
		}
		Entity firstAttached = base.GetFirstAttached();
		if (firstAttached && !firstAttached.IsSpawned())
		{
			return;
		}
		if (this.RBActive && !this.IsTerrainBelow(centerPosition))
		{
			int num = this.worldTerrainFailCount + 1;
			this.worldTerrainFailCount = num;
			if (num <= 6)
			{
				if (this.worldTerrainFailCount == 2)
				{
					chunk.NeedsRegeneration = true;
					this.LogVehicle("{0}, {1}, center {2}, rbPos {3}, in ground. Chunk regen {4}", new object[]
					{
						base.transform.parent.name,
						vector.ToCultureInvariantString(),
						centerPosition.ToCultureInvariantString(),
						(this.vehicleRB.position + Origin.position).ToCultureInvariantString(),
						chunk
					});
				}
			}
			else if (this.hasWorldValidPos)
			{
				Vector3 vector2 = this.worldValidPos - vector;
				if (vector2.y < 0f)
				{
					vector2.y = 0f;
				}
				float sqrMagnitude = vector2.sqrMagnitude;
				vector2 = vector2.normalized;
				if (sqrMagnitude > 0.122499995f)
				{
					vector = this.worldValidPos + vector2 * 0.1f;
					this.SetPosition(vector, true);
				}
				if (!this.vehicleRB.isKinematic)
				{
					Vector3 vector3 = this.vehicleRB.velocity;
					if (Vector3.Dot(vector3, vector2) < 0f)
					{
						vector3 *= -0.5f;
					}
					vector3.y = 1f + this.rand.RandomFloat * 2f;
					vector3 += vector2 * 3f;
					this.vehicleRB.velocity = vector3;
					this.vehicleRB.angularVelocity = Vector3.zero;
				}
				this.LogVehicle("{0}, {1}, center {2} in ground. back {3}", new object[]
				{
					base.transform.parent.name,
					vector.ToCultureInvariantString(),
					centerPosition.ToCultureInvariantString(),
					this.worldValidPos.ToCultureInvariantString()
				});
				this.worldValidPos.x = this.worldValidPos.x + (this.rand.RandomFloat - 0.5f) * 2f * 0.05f;
				this.worldValidPos.z = this.worldValidPos.z + (this.rand.RandomFloat - 0.5f) * 2f * 0.05f;
				this.worldValidPos.y = this.worldValidPos.y + 0.001f;
				this.worldValidDelay -= Time.deltaTime;
				if (this.worldValidDelay <= 0f)
				{
					this.worldValidDelay = 1f;
					this.worldValidPos.y = this.worldValidPos.y + 1.2f;
				}
			}
			else
			{
				Vector3 pos = centerPosition;
				pos.y = 257f;
				bool flag = this.IsTerrainBelow(pos);
				if (flag)
				{
					vector.y += 3f;
					this.SetPosition(vector, true);
				}
				this.LogVehicle("{0}, {1}, center {2} (vel {3}, {4}) {5}", new object[]
				{
					base.transform.parent.name,
					vector.ToCultureInvariantString(),
					centerPosition.ToCultureInvariantString(),
					this.vehicleRB.velocity,
					this.vehicleRB.angularVelocity,
					flag ? " in ground. up" : " out of world"
				});
				if (!this.vehicleRB.isKinematic)
				{
					this.vehicleRB.velocity *= 0.5f;
					this.vehicleRB.angularVelocity *= 0.5f;
				}
			}
		}
		else
		{
			this.worldTerrainFailCount = 0;
			if (this.hasWorldValidPos)
			{
				if ((this.worldValidPos - vector).sqrMagnitude > 4f)
				{
					this.worldValidPos = vector;
				}
			}
			else
			{
				this.hasWorldValidPos = true;
				this.worldValidPos = vector;
			}
		}
		if (this.isTryToFall)
		{
			this.isTryToFall = false;
			this.RBActive = true;
			this.vehicleRB.WakeUp();
		}
	}

	// Token: 0x06002531 RID: 9521 RVA: 0x000EFF0C File Offset: 0x000EE10C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsTerrainBelow(Vector3 pos)
	{
		Ray ray = new Ray(pos - Origin.position, Vector3.down);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, 3.4028235E+38f, 1073807360))
		{
			return true;
		}
		Utils.DrawCircleLinesHorzontal(ray.origin, 0.25f, new Color(1f, 1f, 0f), new Color(1f, 0f, 0f), 8, 5f);
		Utils.DrawLine(ray.origin, new Vector3(ray.origin.x, 0f - Origin.position.y, ray.origin.z), new Color(1f, 1f, 0f), new Color(1f, 0f, 0f), 5, 5f);
		ray.origin += new Vector3(0.02f, 0.5f, 0.03f);
		return Physics.SphereCast(ray, 0.1f, out raycastHit, float.MaxValue, 1073807360);
	}

	// Token: 0x06002532 RID: 9522 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override bool IsDeadIfOutOfWorld()
	{
		return false;
	}

	// Token: 0x06002533 RID: 9523 RVA: 0x000F0030 File Offset: 0x000EE230
	public override void CheckPosition()
	{
		base.CheckPosition();
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.Spawned && !this.hasDriver)
		{
			Vector3i vector3i;
			Vector3i vector3i2;
			this.world.GetWorldExtent(out vector3i, out vector3i2);
			if (this.position.y < (float)vector3i.y)
			{
				Chunk chunk = (Chunk)this.world.GetChunkFromWorldPos(new Vector3i((int)this.position.x, (int)this.position.y, (int)this.position.z));
				if (chunk != null && chunk.IsCollisionMeshGenerated && chunk.IsDisplayed)
				{
					this.TeleportToWithinBounds(vector3i.ToVector3(), vector3i2.ToVector3());
				}
			}
		}
	}

	// Token: 0x06002534 RID: 9524 RVA: 0x000F00F4 File Offset: 0x000EE2F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void TeleportToWithinBounds(Vector3 _min, Vector3 _max)
	{
		_min.x += 66f;
		_min.z += 66f;
		_max.x -= 66f;
		_max.z -= 66f;
		Vector3 position = this.position;
		if (position.x < _min.x)
		{
			position.x = _min.x;
		}
		else if (position.x > _max.x)
		{
			position.x = _max.x;
		}
		if (position.z < _min.z)
		{
			position.z = _min.z;
		}
		else if (position.z > _max.z)
		{
			position.z = _max.z;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(new Vector3(position.x, 999f, position.z) - Origin.position, Vector3.down), out raycastHit, 3.4028235E+38f, 1076428800))
		{
			position.y = raycastHit.point.y + Origin.position.y + 1f;
			this.SetPosition(position, true);
			Log.Out("Vehicle out of world. Teleporting to " + position.ToCultureInvariantString());
		}
	}

	// Token: 0x06002535 RID: 9525 RVA: 0x000F0238 File Offset: 0x000EE438
	public void Kill()
	{
		int attachMaxCount = base.GetAttachMaxCount();
		for (int i = 0; i < attachMaxCount; i++)
		{
			Entity attached = base.GetAttached(i);
			if (attached != null)
			{
				attached.Detach();
			}
		}
		this.timeStayAfterDeath = 0;
		this.SetDead();
		this.MarkToUnload();
	}

	// Token: 0x06002536 RID: 9526 RVA: 0x000F0284 File Offset: 0x000EE484
	public override void OnEntityUnload()
	{
		if (this.vehicleRB)
		{
			this.position = this.vehicleRB.position + Origin.position;
			this.rotation = this.vehicleRB.rotation.eulerAngles;
		}
		base.OnEntityUnload();
	}

	// Token: 0x06002537 RID: 9527 RVA: 0x000F02D8 File Offset: 0x000EE4D8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void HandleNavObject()
	{
		if (EntityClass.list[this.entityClass].NavObject != "")
		{
			if (this.LocalPlayerIsOwner())
			{
				this.NavObject = NavObjectManager.Instance.RegisterNavObject(EntityClass.list[this.entityClass].NavObject, this.vehicle.GetMeshTransform(), "", false);
				EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
				if (primaryPlayer != null)
				{
					primaryPlayer.Waypoints.UpdateEntityVehicleWayPoint(this, false);
					return;
				}
			}
			else if (this.NavObject != null)
			{
				NavObjectManager.Instance.UnRegisterNavObject(this.NavObject);
				this.NavObject = null;
			}
		}
	}

	// Token: 0x06002538 RID: 9528 RVA: 0x000F038D File Offset: 0x000EE58D
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogVehicle(string format, params object[] args)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer || base.GetAttachedPlayerLocal())
		{
			format = string.Format("{0} Vehicle {1}", GameManager.frameCount, format);
			Log.Out(format, args);
		}
	}

	// Token: 0x04001B94 RID: 7060
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageBlockScale = 0.058333334f;

	// Token: 0x04001B95 RID: 7061
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageBlockVelReduction = 1.5f;

	// Token: 0x04001B96 RID: 7062
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageBlockMin = 5f;

	// Token: 0x04001B97 RID: 7063
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageBlockSelfPer = 2.5f;

	// Token: 0x04001B98 RID: 7064
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageTerrainSelfPer = 0.1f;

	// Token: 0x04001B99 RID: 7065
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageEntityScale = 12f;

	// Token: 0x04001B9A RID: 7066
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cDamageEntitySelfScale = 28f;

	// Token: 0x04001B9B RID: 7067
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cKillEntityXPPer = 0.5f;

	// Token: 0x04001B9C RID: 7068
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cExitVelScale = 0.5f;

	// Token: 0x04001B9D RID: 7069
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSleepTime = 3f;

	// Token: 0x04001B9E RID: 7070
	public bool IsEngineRunning;

	// Token: 0x04001B9F RID: 7071
	public bool isLocked;

	// Token: 0x04001BA0 RID: 7072
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isInteractionLocked;

	// Token: 0x04001BA1 RID: 7073
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int interactingPlayerId = -1;

	// Token: 0x04001BA2 RID: 7074
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int interactionRequestType;

	// Token: 0x04001BA3 RID: 7075
	public Vehicle vehicle;

	// Token: 0x04001BA4 RID: 7076
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isTryToFall;

	// Token: 0x04001BA5 RID: 7077
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasDriver;

	// Token: 0x04001BA6 RID: 7078
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float timeInWater;

	// Token: 0x04001BA7 RID: 7079
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public MovementInput movementInput;

	// Token: 0x04001BA8 RID: 7080
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool isTurnTowardsLook = true;

	// Token: 0x04001BA9 RID: 7081
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityVehicle.RemoteData incomingRemoteData;

	// Token: 0x04001BAA RID: 7082
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityVehicle.RemoteData currentRemoteData;

	// Token: 0x04001BAB RID: 7083
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public EntityVehicle.RemoteData lastRemoteData;

	// Token: 0x04001BAC RID: 7084
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSyncHighRateDuration = 0.5f;

	// Token: 0x04001BAD RID: 7085
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float syncHighRateTime;

	// Token: 0x04001BAE RID: 7086
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float syncPlayTime = -1f;

	// Token: 0x04001BAF RID: 7087
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float syncLowRateTime;

	// Token: 0x04001BB0 RID: 7088
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cSyncLowRateDuration = 2f;

	// Token: 0x04001BB1 RID: 7089
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Rigidbody vehicleRB;

	// Token: 0x04001BB2 RID: 7090
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool RBActive;

	// Token: 0x04001BB3 RID: 7091
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float RBNoDriverGndTime;

	// Token: 0x04001BB4 RID: 7092
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float RBNoDriverSleepTime;

	// Token: 0x04001BB5 RID: 7093
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lastRBPos;

	// Token: 0x04001BB6 RID: 7094
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Quaternion lastRBRot;

	// Token: 0x04001BB7 RID: 7095
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lastRBVel;

	// Token: 0x04001BB8 RID: 7096
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 lastRBAngVel;

	// Token: 0x04001BB9 RID: 7097
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float velocityMax;

	// Token: 0x04001BBA RID: 7098
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float damageAccumulator;

	// Token: 0x04001BBB RID: 7099
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int hitEffectCount;

	// Token: 0x04001BBC RID: 7100
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float explodeHealth;

	// Token: 0x04001BBD RID: 7101
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool canHop;

	// Token: 0x04001BBE RID: 7102
	public const float cVehicleCameraOffset = 1.8f;

	// Token: 0x04001BBF RID: 7103
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 cameraStartPos;

	// Token: 0x04001BC0 RID: 7104
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraStartBlend;

	// Token: 0x04001BC1 RID: 7105
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 cameraStartVec;

	// Token: 0x04001BC2 RID: 7106
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 cameraPos;

	// Token: 0x04001BC3 RID: 7107
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraDist;

	// Token: 0x04001BC4 RID: 7108
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static float cameraDistScale = 1f;

	// Token: 0x04001BC5 RID: 7109
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraAngle;

	// Token: 0x04001BC6 RID: 7110
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraAngleTarget;

	// Token: 0x04001BC7 RID: 7111
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraOutTime;

	// Token: 0x04001BC8 RID: 7112
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float cameraVelY;

	// Token: 0x04001BC9 RID: 7113
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityVehicle.Force[] forces = new EntityVehicle.Force[0];

	// Token: 0x04001BCA RID: 7114
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityVehicle.Motor[] motors = new EntityVehicle.Motor[0];

	// Token: 0x04001BCB RID: 7115
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float motorTorque;

	// Token: 0x04001BCC RID: 7116
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float brakeTorque;

	// Token: 0x04001BCD RID: 7117
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float wheelDir;

	// Token: 0x04001BCE RID: 7118
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float wheelMotor;

	// Token: 0x04001BCF RID: 7119
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float wheelBrakes;

	// Token: 0x04001BD0 RID: 7120
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public EntityVehicle.Wheel[] wheels = new EntityVehicle.Wheel[0];

	// Token: 0x04001BD1 RID: 7121
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static string PropOnHonkEvent = "HonkEvent";

	// Token: 0x04001BD2 RID: 7122
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string onHonkEvent = "";

	// Token: 0x04001BD3 RID: 7123
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<EntityVehicle.DelayedAttach> delayedAttachments = new List<EntityVehicle.DelayedAttach>();

	// Token: 0x04001BD4 RID: 7124
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float collisionBlockDamage;

	// Token: 0x04001BD5 RID: 7125
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 collisionVelNorm;

	// Token: 0x04001BD6 RID: 7126
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int collisionIgnoreCount;

	// Token: 0x04001BD7 RID: 7127
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<ContactPoint> contactPoints = new List<ContactPoint>();

	// Token: 0x04001BD8 RID: 7128
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<WorldRayHitInfo> collisionHits = new List<WorldRayHitInfo>();

	// Token: 0x04001BD9 RID: 7129
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int collisionGrazeCount;

	// Token: 0x04001BDA RID: 7130
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const float cFuelItemScale = 25f;

	// Token: 0x04001BDB RID: 7131
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const byte cSyncVersion = 1;

	// Token: 0x04001BDC RID: 7132
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncAttachment = 1;

	// Token: 0x04001BDD RID: 7133
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncInteractAndSecurity = 2;

	// Token: 0x04001BDE RID: 7134
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncItem = 4;

	// Token: 0x04001BDF RID: 7135
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncStorage = 8;

	// Token: 0x04001BE0 RID: 7136
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncInteractRequest = 4096;

	// Token: 0x04001BE1 RID: 7137
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncLowRate = 16384;

	// Token: 0x04001BE2 RID: 7138
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncHighRate = 32768;

	// Token: 0x04001BE3 RID: 7139
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncAllNonRates = 15;

	// Token: 0x04001BE4 RID: 7140
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncLowRateAndNonRates = 16399;

	// Token: 0x04001BE5 RID: 7141
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncReplicate = 49159;

	// Token: 0x04001BE6 RID: 7142
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cSyncSave = 16398;

	// Token: 0x04001BE7 RID: 7143
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const byte cSyncInteractAndSecurityFInteracting = 1;

	// Token: 0x04001BE8 RID: 7144
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const byte cSyncInteractAndSecurityFLocked = 2;

	// Token: 0x04001BE9 RID: 7145
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const int cWorldPad = 66;

	// Token: 0x04001BEA RID: 7146
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool hasWorldValidPos;

	// Token: 0x04001BEB RID: 7147
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 worldValidPos;

	// Token: 0x04001BEC RID: 7148
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float worldValidDelay;

	// Token: 0x04001BED RID: 7149
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int worldTerrainFailCount;

	// Token: 0x0200046B RID: 1131
	[PublicizedFrom(EAccessModifier.Private)]
	public struct RemoteData
	{
		// Token: 0x04001BEE RID: 7150
		public const int cFHasData = 1;

		// Token: 0x04001BEF RID: 7151
		public const int cFAccel = 2;

		// Token: 0x04001BF0 RID: 7152
		public const int cFBreak = 4;

		// Token: 0x04001BF1 RID: 7153
		public int Flags;

		// Token: 0x04001BF2 RID: 7154
		public float MotorTorquePercent;

		// Token: 0x04001BF3 RID: 7155
		public float SteeringPercent;

		// Token: 0x04001BF4 RID: 7156
		public Vector3 Velocity;

		// Token: 0x04001BF5 RID: 7157
		public List<EntityVehicle.RemoteData.Part> parts;

		// Token: 0x0200046C RID: 1132
		public struct Part
		{
			// Token: 0x04001BF6 RID: 7158
			public Vector3 pos;

			// Token: 0x04001BF7 RID: 7159
			public Quaternion rot;
		}
	}

	// Token: 0x0200046D RID: 1133
	[PublicizedFrom(EAccessModifier.Protected)]
	public class Force
	{
		// Token: 0x04001BF8 RID: 7160
		public Vector2 ceiling;

		// Token: 0x04001BF9 RID: 7161
		public Vector3 force;

		// Token: 0x04001BFA RID: 7162
		public EntityVehicle.Force.Trigger trigger;

		// Token: 0x04001BFB RID: 7163
		public EntityVehicle.Force.Type type;

		// Token: 0x0200046E RID: 1134
		public enum Trigger
		{
			// Token: 0x04001BFD RID: 7165
			Off,
			// Token: 0x04001BFE RID: 7166
			On,
			// Token: 0x04001BFF RID: 7167
			InputForward,
			// Token: 0x04001C00 RID: 7168
			InputStrafe,
			// Token: 0x04001C01 RID: 7169
			InputUp,
			// Token: 0x04001C02 RID: 7170
			InputDown,
			// Token: 0x04001C03 RID: 7171
			Motor0,
			// Token: 0x04001C04 RID: 7172
			Motor1,
			// Token: 0x04001C05 RID: 7173
			Motor2,
			// Token: 0x04001C06 RID: 7174
			Motor3,
			// Token: 0x04001C07 RID: 7175
			Motor4,
			// Token: 0x04001C08 RID: 7176
			Motor5,
			// Token: 0x04001C09 RID: 7177
			Motor6,
			// Token: 0x04001C0A RID: 7178
			Motor7
		}

		// Token: 0x0200046F RID: 1135
		public enum Type
		{
			// Token: 0x04001C0C RID: 7180
			Relative,
			// Token: 0x04001C0D RID: 7181
			RelativeTorque
		}
	}

	// Token: 0x02000470 RID: 1136
	[PublicizedFrom(EAccessModifier.Protected)]
	public class Motor
	{
		// Token: 0x04001C0E RID: 7182
		public VPEngine engine;

		// Token: 0x04001C0F RID: 7183
		public float engineOffPer;

		// Token: 0x04001C10 RID: 7184
		public float turbo;

		// Token: 0x04001C11 RID: 7185
		public float rpm;

		// Token: 0x04001C12 RID: 7186
		public float rpmAccelMin;

		// Token: 0x04001C13 RID: 7187
		public float rpmAccelMax;

		// Token: 0x04001C14 RID: 7188
		public float rpmDrag;

		// Token: 0x04001C15 RID: 7189
		public float rpmMax;

		// Token: 0x04001C16 RID: 7190
		public EntityVehicle.Motor.Trigger trigger;

		// Token: 0x04001C17 RID: 7191
		public EntityVehicle.Motor.Type type;

		// Token: 0x04001C18 RID: 7192
		public Transform transform;

		// Token: 0x04001C19 RID: 7193
		public int axis;

		// Token: 0x02000471 RID: 1137
		public enum Trigger
		{
			// Token: 0x04001C1B RID: 7195
			Off,
			// Token: 0x04001C1C RID: 7196
			On,
			// Token: 0x04001C1D RID: 7197
			InputForward,
			// Token: 0x04001C1E RID: 7198
			InputStrafe,
			// Token: 0x04001C1F RID: 7199
			InputUp,
			// Token: 0x04001C20 RID: 7200
			InputDown,
			// Token: 0x04001C21 RID: 7201
			Vel
		}

		// Token: 0x02000472 RID: 1138
		public enum Type
		{
			// Token: 0x04001C23 RID: 7203
			Spin,
			// Token: 0x04001C24 RID: 7204
			Relative,
			// Token: 0x04001C25 RID: 7205
			RelativeTorque
		}
	}

	// Token: 0x02000473 RID: 1139
	[PublicizedFrom(EAccessModifier.Protected)]
	public class Wheel
	{
		// Token: 0x04001C26 RID: 7206
		public float motorTorqueScale;

		// Token: 0x04001C27 RID: 7207
		public float brakeTorqueScale;

		// Token: 0x04001C28 RID: 7208
		public string bounceSound;

		// Token: 0x04001C29 RID: 7209
		public string slideSound;

		// Token: 0x04001C2A RID: 7210
		public bool isSteerParentOfTire;

		// Token: 0x04001C2B RID: 7211
		public Transform steerT;

		// Token: 0x04001C2C RID: 7212
		public Quaternion steerBaseRot;

		// Token: 0x04001C2D RID: 7213
		public Transform tireT;

		// Token: 0x04001C2E RID: 7214
		public float tireSpinSpeed;

		// Token: 0x04001C2F RID: 7215
		public float tireSpin;

		// Token: 0x04001C30 RID: 7216
		public float tireSuspensionPercent;

		// Token: 0x04001C31 RID: 7217
		public WheelCollider wheelC;

		// Token: 0x04001C32 RID: 7218
		public WheelFrictionCurve forwardFriction;

		// Token: 0x04001C33 RID: 7219
		public float forwardStiffnessBase;

		// Token: 0x04001C34 RID: 7220
		public WheelFrictionCurve sideFriction;

		// Token: 0x04001C35 RID: 7221
		public float sideStiffnessBase;

		// Token: 0x04001C36 RID: 7222
		public float slideTime;

		// Token: 0x04001C37 RID: 7223
		public float ptlTime;

		// Token: 0x04001C38 RID: 7224
		public bool isGrounded;
	}

	// Token: 0x02000474 RID: 1140
	public class VehicleInventory : Inventory
	{
		// Token: 0x0600253E RID: 9534 RVA: 0x000F0459 File Offset: 0x000EE659
		public VehicleInventory(IGameManager _gameManager, EntityAlive _entity) : base(_gameManager, _entity)
		{
			this.cSlotCount = base.PUBLIC_SLOTS + 1;
			this.SetupSlots();
		}

		// Token: 0x0600253F RID: 9535 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Execute(int _actionIdx, bool _bReleased, PlayerActionsLocal _playerActions = null)
		{
		}

		// Token: 0x06002540 RID: 9536 RVA: 0x000F0477 File Offset: 0x000EE677
		public void SetupSlots()
		{
			this.slots = new ItemInventoryData[this.cSlotCount];
			this.models = new Transform[this.cSlotCount];
			this.m_HoldingItemIdx = 0;
			base.Clear();
		}

		// Token: 0x06002541 RID: 9537 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void updateHoldingItem()
		{
		}

		// Token: 0x04001C39 RID: 7225
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int cSlotCount;
	}

	// Token: 0x02000475 RID: 1141
	public struct DelayedAttach
	{
		// Token: 0x04001C3A RID: 7226
		public int entityId;

		// Token: 0x04001C3B RID: 7227
		public int slot;
	}
}
