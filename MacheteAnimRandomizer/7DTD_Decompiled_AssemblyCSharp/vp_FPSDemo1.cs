using System;
using UnityEngine;

// Token: 0x020012F0 RID: 4848
public class vp_FPSDemo1 : MonoBehaviour
{
	// Token: 0x06009702 RID: 38658 RVA: 0x003BFC38 File Offset: 0x003BDE38
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.m_Demo = new vp_FPSDemoManager(this.Player);
		this.m_Demo.PlayerEventHandler.Register(this);
		this.m_Demo.CurrentFullScreenFadeTime = Time.time;
		this.m_Demo.DrawCrosshair = false;
		this.m_Demo.Input.MouseCursorZones = new Rect[3];
		this.m_Demo.Input.MouseCursorZones[0] = new Rect((float)Screen.width * 0.5f - 370f, 40f, 80f, 80f);
		this.m_Demo.Input.MouseCursorZones[1] = new Rect((float)Screen.width * 0.5f + 290f, 40f, 80f, 80f);
		this.m_Demo.Input.MouseCursorZones[2] = new Rect(0f, 0f, 150f, (float)Screen.height);
		vp_Utility.LockCursor = false;
		this.m_Demo.Camera.RenderingFieldOfView = 20f;
		this.m_Demo.Camera.SnapZoom();
		this.m_Demo.Camera.PositionOffset = new Vector3(0f, 1.75f, 0.1f);
		this.m_AudioSource = this.m_Demo.Camera.gameObject.AddComponent<AudioSource>();
		this.m_Demo.PlayerEventHandler.SetWeapon.Disallow(10000000f);
		this.m_Demo.PlayerEventHandler.SetPrevWeapon.Try = (() => false);
		this.m_Demo.PlayerEventHandler.SetNextWeapon.Try = (() => false);
	}

	// Token: 0x06009703 RID: 38659 RVA: 0x003BFE2D File Offset: 0x003BE02D
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		if (this.m_Demo.PlayerEventHandler != null)
		{
			this.m_Demo.PlayerEventHandler.Unregister(this);
		}
	}

	// Token: 0x06009704 RID: 38660 RVA: 0x003BFE54 File Offset: 0x003BE054
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		this.m_Demo.Update();
		if (this.m_Demo.CurrentScreen == 1 && this.m_Demo.WeaponHandler.CurrentWeapon != null)
		{
			this.m_Demo.WeaponHandler.SetWeapon(0);
		}
		if (this.m_Demo.CurrentScreen == 2)
		{
			if (Input.GetKeyDown(KeyCode.Backspace))
			{
				this.m_Demo.ButtonSelection = 0;
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				this.m_Demo.ButtonSelection = 1;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				this.m_Demo.ButtonSelection = 2;
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				this.m_Demo.ButtonSelection = 3;
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				this.m_Demo.ButtonSelection = 4;
			}
			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				this.m_Demo.ButtonSelection = 5;
			}
			if (Input.GetKeyDown(KeyCode.Alpha6))
			{
				this.m_Demo.ButtonSelection = 6;
			}
			if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				this.m_Demo.ButtonSelection = 7;
			}
			if (Input.GetKeyDown(KeyCode.Alpha8))
			{
				this.m_Demo.ButtonSelection = 8;
			}
			if (Input.GetKeyDown(KeyCode.Alpha9))
			{
				this.m_Demo.ButtonSelection = 9;
			}
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				this.m_Demo.ButtonSelection = 10;
			}
			if (Input.GetKeyDown(KeyCode.Q))
			{
				this.m_Demo.ButtonSelection--;
				if (this.m_Demo.ButtonSelection < 1)
				{
					this.m_Demo.ButtonSelection = 10;
				}
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				this.m_Demo.ButtonSelection++;
				if (this.m_Demo.ButtonSelection > 10)
				{
					this.m_Demo.ButtonSelection = 1;
				}
			}
		}
		this.m_Demo.Input.MouseCursorBlocksMouseLook = false;
		if (this.m_Demo.CurrentScreen != 3 && this.m_ChrashingAirplaneRestoreTimer.Active)
		{
			this.m_ChrashingAirplaneRestoreTimer.Cancel();
		}
	}

	// Token: 0x06009705 RID: 38661 RVA: 0x003C0044 File Offset: 0x003BE244
	[PublicizedFrom(EAccessModifier.Private)]
	public void DemoIntro()
	{
		this.m_Demo.DrawBoxes("part ii: under the hood", "Ultimate FPS features a NEXT-GEN first person camera system with ultra smooth PROCEDURAL ANIMATION of player movements. Camera and weapons are manipulated using over 100 parameters, allowing for a vast range of super-lifelike behaviors.", null, this.m_ImageRightArrow, null, null, true);
		if (this.m_Demo.FirstFrame)
		{
			this.m_Demo.DrawCrosshair = false;
			this.m_Demo.FirstFrame = false;
			this.m_Demo.Camera.RenderingFieldOfView = 20f;
			this.m_Demo.Camera.SnapZoom();
			this.m_Demo.WeaponHandler.SetWeapon(0);
			this.m_Demo.FreezePlayer(this.m_OverviewPos, this.m_OverviewAngle, true);
			this.m_Demo.LastInputTime -= 20f;
			this.m_Demo.RefreshDefaultState();
			this.m_Demo.Input.MouseCursorForced = true;
		}
		this.m_Demo.Input.MouseCursorForced = true;
		this.m_Demo.ForceCameraShake();
	}

	// Token: 0x06009706 RID: 38662 RVA: 0x003C0138 File Offset: 0x003BE338
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetWeapon(int i, string state = null, bool drawCrosshair = true, bool wieldMotion = true)
	{
		this.m_Demo.DrawCrosshair = drawCrosshair;
		if (this.m_Demo.WeaponHandler.CurrentWeaponIndex != i)
		{
			if (this.m_Demo.WeaponHandler.CurrentWeapon != null)
			{
				if (this.m_ExamplesCurrentSel == 0)
				{
					((vp_FPWeapon)this.m_Demo.WeaponHandler.CurrentWeapon).SnapToExit();
				}
				else if (wieldMotion)
				{
					this.m_Demo.WeaponHandler.CurrentWeapon.Wield(false);
				}
			}
			vp_Timer.In(wieldMotion ? 0.2f : 0f, delegate()
			{
				this.m_Demo.WeaponHandler.SetWeapon(i);
				if (this.m_Demo.WeaponHandler.CurrentWeapon != null && wieldMotion)
				{
					this.m_Demo.WeaponHandler.CurrentWeapon.Wield(true);
				}
				if (state != null)
				{
					this.m_Demo.PlayerEventHandler.ResetActivityStates();
					this.m_Demo.PlayerEventHandler.SetState(state, true, true, false);
				}
			}, this.m_WeaponSwitchTimer);
			return;
		}
		if (state != null)
		{
			this.m_Demo.PlayerEventHandler.SetState(state, true, true, false);
		}
	}

	// Token: 0x06009707 RID: 38663 RVA: 0x003C0238 File Offset: 0x003BE438
	[PublicizedFrom(EAccessModifier.Private)]
	public void DemoExamples()
	{
		this.m_Demo.DrawBoxes("examples", "Try MOVING, JUMPING and STRAFING with the demo presets on the left.\nNote that NO ANIMATIONS are used in this demo. Instead, the camera and weapons are manipulated using realtime SPRING PHYSICS, SINUS BOB and NOISE SHAKING.\nCombining this with traditional animations (e.g. reload) can be very powerful!", this.m_ImageLeftArrow, this.m_ImageRightArrow, null, null, true);
		if (this.m_Demo.FirstFrame)
		{
			this.m_AudioSource.Stop();
			this.m_Demo.DrawCrosshair = true;
			this.m_Demo.Teleport(this.m_StartPos, this.m_StartAngle);
			this.m_Demo.FirstFrame = false;
			this.m_UnFreezePosition = this.m_Demo.Controller.transform.position;
			this.m_Demo.ButtonSelection = 0;
			this.m_Demo.WeaponHandler.SetWeapon(3);
			this.m_Demo.PlayerEventHandler.SetState("Freeze", false, true, false);
			this.m_Demo.PlayerEventHandler.SetState("SystemOFF", true, true, false);
			if (this.m_Demo.WeaponHandler.CurrentWeapon != null)
			{
				((vp_FPWeapon)this.m_Demo.WeaponHandler.CurrentWeapon).SnapZoom();
			}
			this.m_Demo.Camera.SnapZoom();
			this.m_Demo.Camera.SnapSprings();
			this.m_Demo.Input.MouseCursorForced = true;
		}
		if (this.m_Demo.ButtonSelection != this.m_ExamplesCurrentSel)
		{
			vp_Utility.LockCursor = true;
			this.m_Demo.ResetState();
			this.m_Demo.PlayerEventHandler.Attack.Stop(0.5f);
			this.m_Demo.Camera.BobStepCallback = null;
			this.m_Demo.Camera.SnapSprings();
			if (this.m_ExamplesCurrentSel == 9 && this.m_Demo.WeaponHandler.CurrentWeapon != null)
			{
				((vp_FPWeapon)this.m_Demo.WeaponHandler.CurrentWeapon).SnapZoom();
				((vp_FPWeapon)this.m_Demo.WeaponHandler.CurrentWeapon).SnapSprings();
				((vp_FPWeapon)this.m_Demo.WeaponHandler.CurrentWeapon).SnapPivot();
			}
			switch (this.m_Demo.ButtonSelection)
			{
			case 0:
				this.m_Demo.PlayerEventHandler.Attack.Stop(10000000f);
				this.m_Demo.DrawCrosshair = true;
				this.m_Demo.Controller.Stop();
				if (this.m_Demo.WeaponHandler.CurrentWeaponIndex == 5)
				{
					this.m_Demo.WeaponHandler.SetWeapon(1);
					this.m_Demo.PlayerEventHandler.SetState("SystemOFF", true, true, false);
				}
				else
				{
					this.m_Demo.Camera.SnapZoom();
					this.m_Demo.PlayerEventHandler.SetState("SystemOFF", true, true, false);
					if (this.m_Demo.WeaponHandler.CurrentWeapon != null)
					{
						this.m_Demo.WeaponHandler.CurrentWeapon.SnapSprings();
						((vp_FPWeapon)this.m_Demo.WeaponHandler.CurrentWeapon).SnapZoom();
					}
				}
				break;
			case 1:
				this.SetWeapon(3, "MafiaBoss", true, true);
				break;
			case 2:
				this.SetWeapon(1, "ModernShooter", true, true);
				break;
			case 3:
				this.SetWeapon(4, "Barbarian", true, true);
				break;
			case 4:
				this.SetWeapon(2, "SniperBreath", true, true);
				this.m_Demo.Controller.Stop();
				this.m_Demo.Teleport(this.m_SniperPos, this.m_SniperAngle);
				break;
			case 5:
				this.SetWeapon(0, "Astronaut", false, true);
				this.m_Demo.Controller.Stop();
				this.m_Demo.Teleport(this.m_AstronautPos, this.m_AstronautAngle);
				break;
			case 6:
				this.SetWeapon(5, "MechOrDino", true, false);
				this.m_UnFreezePosition = this.m_DrunkPos;
				this.m_Demo.Controller.Stop();
				this.m_Demo.Teleport(this.m_MechPos, this.m_MechAngle);
				this.m_Demo.Camera.BobStepCallback = delegate()
				{
					this.m_Demo.Camera.AddForce2(new Vector3(0f, -1f, 0f));
					if (this.m_Demo.WeaponHandler.CurrentWeapon != null)
					{
						((vp_FPWeapon)this.m_Demo.WeaponHandler.CurrentWeapon).AddForce(new Vector3(0f, 0f, 0f), new Vector3(-0.3f, 0f, 0f));
					}
					this.m_AudioSource.pitch = Time.timeScale;
					this.m_AudioSource.PlayOneShot(this.m_StompSound);
				};
				break;
			case 7:
				this.SetWeapon(3, "TankTurret", true, false);
				this.m_Demo.FreezePlayer(this.m_OverviewPos, this.m_OverviewAngle);
				this.m_Demo.Controller.Stop();
				break;
			case 8:
				this.m_Demo.Controller.Stop();
				this.SetWeapon(0, "DrunkPerson", false, true);
				this.m_Demo.Controller.Stop();
				this.m_Demo.Teleport(this.m_DrunkPos, this.m_DrunkAngle);
				this.m_Demo.Camera.StopSprings();
				this.m_Demo.Camera.Refresh();
				break;
			case 9:
				this.SetWeapon(1, "OldSchool", true, true);
				this.m_Demo.Controller.Stop();
				this.m_Demo.Teleport(this.m_OldSchoolPos, this.m_OldSchoolAngle);
				this.m_Demo.Camera.SnapSprings();
				this.m_Demo.Camera.SnapZoom();
				vp_Timer.In(0.3f, delegate()
				{
					if (this.m_Demo.WeaponHandler.CurrentWeapon != null)
					{
						vp_Shooter componentInChildren = this.m_Demo.WeaponHandler.CurrentWeapon.GetComponentInChildren<vp_Shooter>();
						componentInChildren.MuzzleFlashPosition = new Vector3(0.0025736f, -0.0813138f, 1.662671f);
						componentInChildren.Refresh();
					}
				}, null);
				break;
			case 10:
				this.SetWeapon(2, "CrazyCowboy", true, true);
				this.m_Demo.Teleport(this.m_StartPos, this.m_StartAngle);
				this.m_Demo.Controller.Stop();
				break;
			}
			this.m_ExamplesCurrentSel = this.m_Demo.ButtonSelection;
		}
		if (this.m_Demo.ShowGUI)
		{
			this.m_ExamplesCurrentSel = this.m_Demo.ButtonSelection;
			string[] strings = new string[]
			{
				"System OFF",
				"Mafia Boss",
				"Modern Shooter",
				"Barbarian",
				"Sniper Breath",
				"Astronaut",
				"Mech... or Dino?",
				"Tank Turret",
				"Drunk Person",
				"Old School",
				"Crazy Cowboy"
			};
			this.m_Demo.ButtonSelection = this.m_Demo.ToggleColumn(140, 150, this.m_Demo.ButtonSelection, strings, false, true, this.m_ImageRightPointer, this.m_ImageLeftPointer);
		}
		if (this.m_Demo.ShowGUI && vp_Utility.LockCursor)
		{
			GUI.color = new Color(1f, 1f, 1f, this.m_Demo.ClosingDown ? this.m_Demo.GlobalAlpha : 1f);
			GUI.Label(new Rect((float)(Screen.width / 2 - 200), 140f, 400f, 20f), "(Press ENTER to reenable menu)", this.m_Demo.CenterStyle);
			GUI.color = new Color(1f, 1f, 1f, 1f * this.m_Demo.GlobalAlpha);
		}
	}

	// Token: 0x06009708 RID: 38664 RVA: 0x003C0958 File Offset: 0x003BEB58
	[PublicizedFrom(EAccessModifier.Private)]
	public void DemoForces()
	{
		this.m_Demo.DrawBoxes("external forces", "The camera and weapon are mounted on 8 positional and angular SPRINGS.\nEXTERNAL FORCES can be applied to these in various ways, creating unique movement patterns every time. This is useful for shockwaves, explosion knockback and earthquakes.", this.m_ImageLeftArrow, this.m_ImageRightArrow, null, null, true);
		if (this.m_Demo.FirstFrame)
		{
			this.m_Demo.DrawCrosshair = false;
			this.m_Demo.ResetState();
			this.m_Demo.Camera.Load(this.StompingCamera);
			this.m_Demo.Input.Load(this.StompingInput);
			this.m_Demo.WeaponHandler.SetWeapon(1);
			this.m_Demo.Controller.Load(this.SmackController);
			this.m_Demo.Camera.SnapZoom();
			this.m_Demo.FirstFrame = false;
			this.m_Demo.Teleport(this.m_ForcesPos, this.m_ForcesAngle);
			this.m_Demo.ButtonColumnArrowY = -100f;
			this.m_Demo.Input.MouseCursorForced = true;
		}
		if (this.m_Demo.ShowGUI)
		{
			this.m_Demo.ButtonSelection = -1;
			string[] strings = new string[]
			{
				"Earthquake",
				"Boss Stomp",
				"Incoming Artillery",
				"Crashing Airplane"
			};
			this.m_Demo.ButtonSelection = this.m_Demo.ButtonColumn(150, this.m_Demo.ButtonSelection, strings, this.m_ImageRightPointer);
			if (this.m_Demo.ButtonSelection != -1)
			{
				switch (this.m_Demo.ButtonSelection)
				{
				case 0:
					this.m_Demo.Camera.Load(this.StompingCamera);
					this.m_Demo.Input.Load(this.StompingInput);
					this.m_Demo.Controller.Load(this.SmackController);
					this.m_Demo.PlayerEventHandler.CameraEarthQuake.TryStart<Vector3>(new Vector3(0.2f, 0.2f, 10f));
					this.m_Demo.ButtonColumnArrowFadeoutTime = Time.time + 9f;
					this.m_AudioSource.Stop();
					this.m_AudioSource.pitch = Time.timeScale;
					this.m_AudioSource.PlayOneShot(this.m_EarthquakeSound);
					break;
				case 1:
					this.m_Demo.PlayerEventHandler.CameraEarthQuake.Stop(0f);
					this.m_Demo.Camera.Load(this.ArtilleryCamera);
					this.m_Demo.Input.Load(this.ArtilleryInput);
					this.m_Demo.Controller.Load(this.SmackController);
					this.m_Demo.PlayerEventHandler.CameraGroundStomp.Send(1f);
					this.m_Demo.ButtonColumnArrowFadeoutTime = Time.time;
					this.m_AudioSource.Stop();
					this.m_AudioSource.pitch = Time.timeScale;
					this.m_AudioSource.PlayOneShot(this.m_StompSound);
					break;
				case 2:
				{
					this.m_Demo.PlayerEventHandler.CameraEarthQuake.Stop(0f);
					this.m_Demo.Camera.Load(this.ArtilleryCamera);
					this.m_Demo.Input.Load(this.ArtilleryInput);
					this.m_Demo.Controller.Load(this.ArtilleryController);
					this.m_Demo.PlayerEventHandler.CameraBombShake.Send(1f);
					this.m_Demo.Controller.AddForce(UnityEngine.Random.Range(-1.5f, 1.5f), 0.5f, UnityEngine.Random.Range(-1.5f, -0.5f));
					this.m_Demo.ButtonColumnArrowFadeoutTime = Time.time + 1f;
					this.m_AudioSource.Stop();
					this.m_AudioSource.pitch = Time.timeScale;
					this.m_AudioSource.PlayOneShot(this.m_ExplosionSound);
					Vector3 position = this.m_Demo.Controller.transform.TransformPoint(Vector3.forward * (float)UnityEngine.Random.Range(1, 2));
					position.y = this.m_Demo.Controller.transform.position.y + 1f;
					UnityEngine.Object.Instantiate<GameObject>(this.m_ArtilleryFX, position, Quaternion.identity);
					break;
				}
				case 3:
					this.m_Demo.Camera.Load(this.StompingCamera);
					this.m_Demo.Input.Load(this.StompingInput);
					this.m_Demo.Controller.Load(this.SmackController);
					this.m_Demo.PlayerEventHandler.CameraEarthQuake.TryStart<Vector3>(new Vector3(0.25f, 0.2f, 10f));
					this.m_Demo.ButtonColumnArrowFadeoutTime = Time.time + 9f;
					this.m_AudioSource.Stop();
					this.m_AudioSource.pitch = Time.timeScale;
					this.m_AudioSource.PlayOneShot(this.m_EarthquakeSound);
					this.m_Demo.Camera.RenderingFieldOfView = 80f;
					this.m_Demo.Camera.RotationEarthQuakeFactor = 6.5f;
					this.m_Demo.Camera.Zoom();
					vp_Timer.In(9f, delegate()
					{
						this.m_Demo.Camera.RenderingFieldOfView = 60f;
						this.m_Demo.Camera.RotationEarthQuakeFactor = 0f;
						this.m_Demo.Camera.Zoom();
					}, this.m_ChrashingAirplaneRestoreTimer);
					break;
				}
				this.m_Demo.LastInputTime = Time.time;
			}
			this.m_Demo.DrawEditorPreview(this.m_ImageWeaponPosition, this.m_ImageEditorPreview, this.m_ImageEditorScreenshot);
		}
	}

	// Token: 0x06009709 RID: 38665 RVA: 0x003C0F04 File Offset: 0x003BF104
	[PublicizedFrom(EAccessModifier.Private)]
	public void DemoMouseInput()
	{
		this.m_Demo.DrawBoxes("mouse input", "Any good FPS should offer configurable MOUSE SMOOTHING and ACCELERATION.\n• Smoothing interpolates mouse input over several frames to reduce jittering.\n • Acceleration + low mouse sensitivity allows high precision without loss of turn speed.\n• Click the below buttons to compare some example setups.", this.m_ImageLeftArrow, this.m_ImageRightArrow, null, null, true);
		if (this.m_Demo.FirstFrame)
		{
			this.m_Demo.ResetState();
			this.m_AudioSource.Stop();
			this.m_Demo.DrawCrosshair = true;
			this.m_Demo.FreezePlayer(this.m_MouseLookPos, this.m_MouseLookAngle);
			this.m_Demo.FirstFrame = false;
			this.m_Demo.WeaponHandler.SetWeapon(0);
			this.m_Demo.Input.MouseCursorForced = true;
			this.m_Demo.Camera.Load(this.MouseRawUnityCamera);
			this.m_Demo.Input.Load(this.MouseRawUnityInput);
		}
		if (this.m_Demo.ShowGUI)
		{
			int buttonSelection = this.m_Demo.ButtonSelection;
			bool arrow = this.m_Demo.ButtonSelection != 2;
			string[] strings = new string[]
			{
				"Raw Mouse Input",
				"Mouse Smoothing",
				"Low Sens. + Acceleration"
			};
			this.m_Demo.ButtonSelection = this.m_Demo.ToggleColumn(200, 150, this.m_Demo.ButtonSelection, strings, true, arrow, this.m_ImageRightPointer, this.m_ImageLeftPointer);
			if (this.m_Demo.ButtonSelection != buttonSelection)
			{
				switch (this.m_Demo.ButtonSelection)
				{
				case 0:
					this.m_Demo.PlayerEventHandler.ResetActivityStates();
					this.m_Demo.Camera.Load(this.MouseRawUnityCamera);
					this.m_Demo.Input.Load(this.MouseRawUnityInput);
					break;
				case 1:
					this.m_Demo.PlayerEventHandler.ResetActivityStates();
					this.m_Demo.Camera.Load(this.MouseSmoothingCamera);
					this.m_Demo.Input.Load(this.MouseSmoothingInput);
					break;
				case 2:
					this.m_Demo.PlayerEventHandler.ResetActivityStates();
					this.m_Demo.Camera.Load(this.MouseLowSensCamera);
					this.m_Demo.Input.Load(this.MouseLowSensInput);
					break;
				}
				this.m_Demo.LastInputTime = Time.time;
			}
			arrow = true;
			if (this.m_Demo.ButtonSelection != 2)
			{
				GUI.enabled = false;
				arrow = false;
			}
			this.m_Demo.Input.MouseLookAcceleration = this.m_Demo.ButtonToggle(new Rect((float)(Screen.width / 2 + 110), 215f, 90f, 40f), "Acceleration", this.m_Demo.Input.MouseLookAcceleration, arrow, this.m_ImageUpPointer);
			GUI.color = new Color(1f, 1f, 1f, 1f * this.m_Demo.GlobalAlpha);
			GUI.enabled = true;
			this.m_Demo.DrawEditorPreview(this.m_ImageCameraMouse, this.m_ImageEditorPreview, this.m_ImageEditorScreenshot);
		}
	}

	// Token: 0x0600970A RID: 38666 RVA: 0x003C1220 File Offset: 0x003BF420
	[PublicizedFrom(EAccessModifier.Private)]
	public void DemoWeaponPerspective()
	{
		this.m_Demo.DrawBoxes("weapon perspective", "Proper WEAPON PERSPECTIVE is crucial to the final impression of your game!\nThe weapon has its own separate Field of View for full perspective control,\nalong with dynamic position and rotation offset.", this.m_ImageLeftArrow, this.m_ImageRightArrow, null, null, true);
		if (this.m_Demo.FirstFrame)
		{
			this.m_Demo.ResetState();
			this.m_Demo.Camera.Load(this.PerspOldCamera);
			this.m_Demo.Input.Load(this.PerspOldInput);
			this.m_Demo.Camera.SnapZoom();
			this.m_Demo.FirstFrame = false;
			this.m_Demo.FreezePlayer(this.m_OverviewPos, this.m_PerspectiveAngle, true);
			this.m_Demo.Input.MouseCursorForced = true;
			this.m_Demo.WeaponHandler.SetWeapon(3);
			this.m_Demo.SetWeaponPreset(this.PerspOldWeapon, null, true);
			if (this.m_Demo.WeaponHandler.CurrentWeapon != null)
			{
				this.m_Demo.WeaponHandler.CurrentWeapon.SetState("WeaponPersp", true, false, false);
			}
			this.m_Demo.WeaponHandler.SetWeaponLayer(10);
			if (this.m_Demo.WeaponHandler.CurrentWeapon != null)
			{
				((vp_FPWeapon)this.m_Demo.WeaponHandler.CurrentWeapon).SnapZoom();
				this.m_Demo.WeaponHandler.CurrentWeapon.SnapSprings();
				((vp_FPWeapon)this.m_Demo.WeaponHandler.CurrentWeapon).SnapPivot();
			}
		}
		if (this.m_Demo.ShowGUI)
		{
			int buttonSelection = this.m_Demo.ButtonSelection;
			string[] strings = new string[]
			{
				"Old School",
				"1999 Internet Café",
				"Modern Shooter"
			};
			this.m_Demo.ButtonSelection = this.m_Demo.ToggleColumn(200, 150, this.m_Demo.ButtonSelection, strings, true, true, this.m_ImageRightPointer, this.m_ImageLeftPointer);
			if (this.m_Demo.ButtonSelection != buttonSelection)
			{
				switch (this.m_Demo.ButtonSelection)
				{
				case 0:
					this.m_Demo.SetWeaponPreset(this.PerspOldWeapon, null, true);
					break;
				case 1:
					this.m_Demo.SetWeaponPreset(this.Persp1999Weapon, null, true);
					break;
				case 2:
					this.m_Demo.SetWeaponPreset(this.PerspModernWeapon, null, true);
					break;
				}
				this.m_Demo.LastInputTime = Time.time;
			}
			this.m_Demo.DrawEditorPreview(this.m_ImageWeaponPerspective, this.m_ImageEditorPreview, this.m_ImageEditorScreenshot);
		}
	}

	// Token: 0x0600970B RID: 38667 RVA: 0x003C14B8 File Offset: 0x003BF6B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void DemoWeaponLayer()
	{
		this.m_Demo.DrawBoxes("weapon camera", "\nThe weapon can be rendered by a SEPARATE CAMERA so that it never sticks through walls or other geometry. Try toggling the weapon camera ON and OFF below.", this.m_ImageLeftArrow, this.m_ImageRightArrow, null, null, true);
		if (this.m_Demo.FirstFrame)
		{
			this.m_Demo.ResetState();
			this.m_Demo.DrawCrosshair = true;
			this.m_Demo.Camera.Load(this.WallFacingCamera);
			this.m_Demo.Input.Load(this.WallFacingInput);
			this.m_Demo.WeaponHandler.SetWeapon(3);
			this.m_Demo.SetWeaponPreset(this.WallFacingWeapon, null, true);
			this.m_Demo.Camera.SnapZoom();
			this.m_WeaponLayerToggle = false;
			this.m_Demo.FirstFrame = false;
			this.m_Demo.FreezePlayer(this.m_WeaponLayerPos, this.m_WeaponLayerAngle);
			int weaponLayer = this.m_WeaponLayerToggle ? 10 : 0;
			this.m_Demo.WeaponHandler.SetWeaponLayer(weaponLayer);
			this.m_Demo.Input.MouseCursorForced = true;
		}
		if (this.m_Demo.ShowGUI)
		{
			bool weaponLayerToggle = this.m_WeaponLayerToggle;
			this.m_WeaponLayerToggle = this.m_Demo.ButtonToggle(new Rect((float)(Screen.width / 2 - 45), 180f, 100f, 40f), "Weapon Camera", this.m_WeaponLayerToggle, true, this.m_ImageUpPointer);
			if (weaponLayerToggle != this.m_WeaponLayerToggle)
			{
				this.m_Demo.FreezePlayer(this.m_WeaponLayerPos, this.m_WeaponLayerAngle);
				int weaponLayer2 = this.m_WeaponLayerToggle ? 10 : 0;
				this.m_Demo.WeaponHandler.SetWeaponLayer(weaponLayer2);
				this.m_Demo.LastInputTime = Time.time;
			}
		}
	}

	// Token: 0x0600970C RID: 38668 RVA: 0x003C1674 File Offset: 0x003BF874
	[PublicizedFrom(EAccessModifier.Private)]
	public void DemoPivot()
	{
		this.m_Demo.DrawBoxes("weapon pivot", "The PIVOT POINT of the weapon model greatly affects movement pattern.\nManipulating it at runtime can be quite useful, and easy with Ultimate FPS!\nClick the examples below and move the camera around.", this.m_ImageLeftArrow, this.m_ImageRightArrow, delegate
		{
			this.m_Demo.LoadLevel(2);
		}, null, true);
		if (this.m_Demo.FirstFrame)
		{
			this.m_Demo.ResetState();
			this.m_Demo.DrawCrosshair = false;
			this.m_Demo.Camera.Load(this.DefaultCamera);
			this.m_Demo.Input.Load(this.DefaultInput);
			this.m_Demo.Controller.Load(this.ImmobileController);
			this.m_Demo.FirstFrame = false;
			this.m_Demo.FreezePlayer(this.m_OverviewPos, this.m_OverviewAngle);
			this.m_Demo.WeaponHandler.SetWeapon(1);
			this.m_Demo.SetWeaponPreset(this.DefaultWeapon, null, true);
			this.m_Demo.SetWeaponPreset(this.PivotMuzzleWeapon, null, true);
			if (this.m_Demo.WeaponHandler.CurrentWeapon != null)
			{
				((vp_FPWeapon)this.m_Demo.WeaponHandler.CurrentWeapon).SetPivotVisible(true);
			}
			this.m_Demo.Input.MouseCursorForced = true;
			this.m_Demo.WeaponHandler.SetWeaponLayer(10);
		}
		if (this.m_Demo.ShowGUI)
		{
			int buttonSelection = this.m_Demo.ButtonSelection;
			string[] strings = new string[]
			{
				"Muzzle",
				"Grip",
				"Chest",
				"Elbow (Uzi Style)"
			};
			this.m_Demo.ButtonSelection = this.m_Demo.ToggleColumn(200, 150, this.m_Demo.ButtonSelection, strings, true, true, this.m_ImageRightPointer, this.m_ImageLeftPointer);
			if (this.m_Demo.ButtonSelection != buttonSelection)
			{
				switch (this.m_Demo.ButtonSelection)
				{
				case 0:
					this.m_Demo.SetWeaponPreset(this.PivotMuzzleWeapon, null, true);
					break;
				case 1:
					this.m_Demo.SetWeaponPreset(this.PivotWristWeapon, null, true);
					break;
				case 2:
					this.m_Demo.SetWeaponPreset(this.PivotChestWeapon, null, true);
					break;
				case 3:
					this.m_Demo.SetWeaponPreset(this.PivotElbowWeapon, null, true);
					break;
				}
				this.m_Demo.LastInputTime = Time.time;
			}
			this.m_Demo.DrawEditorPreview(this.m_ImageWeaponPivot, this.m_ImageEditorPreview, this.m_ImageEditorScreenshot);
		}
	}

	// Token: 0x0600970D RID: 38669 RVA: 0x003C1900 File Offset: 0x003BFB00
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGUI()
	{
		this.m_Demo.OnGUI();
		switch (this.m_Demo.CurrentScreen)
		{
		case 1:
			this.DemoIntro();
			return;
		case 2:
			this.DemoExamples();
			return;
		case 3:
			this.DemoForces();
			return;
		case 4:
			this.DemoMouseInput();
			return;
		case 5:
			this.DemoWeaponPerspective();
			return;
		case 6:
			this.DemoWeaponLayer();
			return;
		case 7:
			this.DemoPivot();
			return;
		default:
			return;
		}
	}

	// Token: 0x040072DB RID: 29403
	public GameObject Player;

	// Token: 0x040072DC RID: 29404
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_FPSDemoManager m_Demo;

	// Token: 0x040072DD RID: 29405
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int m_ExamplesCurrentSel;

	// Token: 0x040072DE RID: 29406
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_Timer.Handle m_ChrashingAirplaneRestoreTimer = new vp_Timer.Handle();

	// Token: 0x040072DF RID: 29407
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_Timer.Handle m_WeaponSwitchTimer = new vp_Timer.Handle();

	// Token: 0x040072E0 RID: 29408
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool m_WeaponLayerToggle;

	// Token: 0x040072E1 RID: 29409
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_MouseLookPos = new Vector3(-8.093015f, 20.08f, 3.416737f);

	// Token: 0x040072E2 RID: 29410
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_OverviewPos = new Vector3(1.246535f, 32.08f, 21.43753f);

	// Token: 0x040072E3 RID: 29411
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_StartPos = new Vector3(-18.14881f, 20.08f, -24.16859f);

	// Token: 0x040072E4 RID: 29412
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_WeaponLayerPos = new Vector3(-19.43989f, 16.08f, 2.10474f);

	// Token: 0x040072E5 RID: 29413
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_ForcesPos = new Vector3(-8.093015f, 20.08f, 3.416737f);

	// Token: 0x040072E6 RID: 29414
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_MechPos = new Vector3(0.02941191f, 1.08f, -93.50691f);

	// Token: 0x040072E7 RID: 29415
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_DrunkPos = new Vector3(18.48685f, 21.08f, 24.05441f);

	// Token: 0x040072E8 RID: 29416
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_SniperPos = new Vector3(0.8841875f, 33.08f, 21.3446f);

	// Token: 0x040072E9 RID: 29417
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_OldSchoolPos = new Vector3(25.88745f, 0.08f, 23.08822f);

	// Token: 0x040072EA RID: 29418
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 m_AstronautPos = new Vector3(20f, 20f, 16f);

	// Token: 0x040072EB RID: 29419
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Vector3 m_UnFreezePosition = Vector3.zero;

	// Token: 0x040072EC RID: 29420
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_MouseLookAngle = new Vector2(0f, 33.10683f);

	// Token: 0x040072ED RID: 29421
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_OverviewAngle = new Vector2(28.89369f, 224f);

	// Token: 0x040072EE RID: 29422
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_PerspectiveAngle = new Vector2(27f, 223f);

	// Token: 0x040072EF RID: 29423
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_StartAngle = new Vector2(0f, 0f);

	// Token: 0x040072F0 RID: 29424
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_WeaponLayerAngle = new Vector2(0f, -90f);

	// Token: 0x040072F1 RID: 29425
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_ForcesAngle = new Vector2(0f, 0f);

	// Token: 0x040072F2 RID: 29426
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_MechAngle = new Vector3(0f, 180f);

	// Token: 0x040072F3 RID: 29427
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_DrunkAngle = new Vector3(0f, -90f);

	// Token: 0x040072F4 RID: 29428
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_SniperAngle = new Vector2(20f, 180f);

	// Token: 0x040072F5 RID: 29429
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_OldSchoolAngle = new Vector2(0f, 180f);

	// Token: 0x040072F6 RID: 29430
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector2 m_AstronautAngle = new Vector2(0f, 269.5f);

	// Token: 0x040072F7 RID: 29431
	public Texture m_ImageEditorPreview;

	// Token: 0x040072F8 RID: 29432
	public Texture m_ImageEditorPreviewShow;

	// Token: 0x040072F9 RID: 29433
	public Texture m_ImageCameraMouse;

	// Token: 0x040072FA RID: 29434
	public Texture m_ImageWeaponPosition;

	// Token: 0x040072FB RID: 29435
	public Texture m_ImageWeaponPerspective;

	// Token: 0x040072FC RID: 29436
	public Texture m_ImageWeaponPivot;

	// Token: 0x040072FD RID: 29437
	public Texture m_ImageEditorScreenshot;

	// Token: 0x040072FE RID: 29438
	public Texture m_ImageLeftArrow;

	// Token: 0x040072FF RID: 29439
	public Texture m_ImageRightArrow;

	// Token: 0x04007300 RID: 29440
	public Texture m_ImageCheckmark;

	// Token: 0x04007301 RID: 29441
	public Texture m_ImageLeftPointer;

	// Token: 0x04007302 RID: 29442
	public Texture m_ImageRightPointer;

	// Token: 0x04007303 RID: 29443
	public Texture m_ImageUpPointer;

	// Token: 0x04007304 RID: 29444
	public Texture m_ImageCrosshair;

	// Token: 0x04007305 RID: 29445
	public Texture m_ImageFullScreen;

	// Token: 0x04007306 RID: 29446
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public AudioSource m_AudioSource;

	// Token: 0x04007307 RID: 29447
	public AudioClip m_StompSound;

	// Token: 0x04007308 RID: 29448
	public AudioClip m_EarthquakeSound;

	// Token: 0x04007309 RID: 29449
	public AudioClip m_ExplosionSound;

	// Token: 0x0400730A RID: 29450
	public GameObject m_ArtilleryFX;

	// Token: 0x0400730B RID: 29451
	public TextAsset ArtilleryCamera;

	// Token: 0x0400730C RID: 29452
	public TextAsset ArtilleryController;

	// Token: 0x0400730D RID: 29453
	public TextAsset ArtilleryInput;

	// Token: 0x0400730E RID: 29454
	public TextAsset AstronautCamera;

	// Token: 0x0400730F RID: 29455
	public TextAsset AstronautController;

	// Token: 0x04007310 RID: 29456
	public TextAsset AstronautInput;

	// Token: 0x04007311 RID: 29457
	public TextAsset CowboyCamera;

	// Token: 0x04007312 RID: 29458
	public TextAsset CowboyController;

	// Token: 0x04007313 RID: 29459
	public TextAsset CowboyWeapon;

	// Token: 0x04007314 RID: 29460
	public TextAsset CowboyShooter;

	// Token: 0x04007315 RID: 29461
	public TextAsset CowboyInput;

	// Token: 0x04007316 RID: 29462
	public TextAsset CrouchController;

	// Token: 0x04007317 RID: 29463
	public TextAsset CrouchInput;

	// Token: 0x04007318 RID: 29464
	public TextAsset DefaultCamera;

	// Token: 0x04007319 RID: 29465
	public TextAsset DefaultWeapon;

	// Token: 0x0400731A RID: 29466
	public TextAsset DefaultInput;

	// Token: 0x0400731B RID: 29467
	public TextAsset DrunkCamera;

	// Token: 0x0400731C RID: 29468
	public TextAsset DrunkController;

	// Token: 0x0400731D RID: 29469
	public TextAsset DrunkInput;

	// Token: 0x0400731E RID: 29470
	public TextAsset ImmobileCamera;

	// Token: 0x0400731F RID: 29471
	public TextAsset ImmobileController;

	// Token: 0x04007320 RID: 29472
	public TextAsset ImmobileInput;

	// Token: 0x04007321 RID: 29473
	public TextAsset MaceCamera;

	// Token: 0x04007322 RID: 29474
	public TextAsset MaceWeapon;

	// Token: 0x04007323 RID: 29475
	public TextAsset MaceInput;

	// Token: 0x04007324 RID: 29476
	public TextAsset MafiaCamera;

	// Token: 0x04007325 RID: 29477
	public TextAsset MafiaWeapon;

	// Token: 0x04007326 RID: 29478
	public TextAsset MafiaShooter;

	// Token: 0x04007327 RID: 29479
	public TextAsset MafiaInput;

	// Token: 0x04007328 RID: 29480
	public TextAsset MechCamera;

	// Token: 0x04007329 RID: 29481
	public TextAsset MechController;

	// Token: 0x0400732A RID: 29482
	public TextAsset MechWeapon;

	// Token: 0x0400732B RID: 29483
	public TextAsset MechShooter;

	// Token: 0x0400732C RID: 29484
	public TextAsset MechInput;

	// Token: 0x0400732D RID: 29485
	public TextAsset ModernCamera;

	// Token: 0x0400732E RID: 29486
	public TextAsset ModernController;

	// Token: 0x0400732F RID: 29487
	public TextAsset ModernWeapon;

	// Token: 0x04007330 RID: 29488
	public TextAsset ModernShooter;

	// Token: 0x04007331 RID: 29489
	public TextAsset ModernInput;

	// Token: 0x04007332 RID: 29490
	public TextAsset MouseLowSensCamera;

	// Token: 0x04007333 RID: 29491
	public TextAsset MouseLowSensInput;

	// Token: 0x04007334 RID: 29492
	public TextAsset MouseRawUnityCamera;

	// Token: 0x04007335 RID: 29493
	public TextAsset MouseRawUnityInput;

	// Token: 0x04007336 RID: 29494
	public TextAsset MouseSmoothingCamera;

	// Token: 0x04007337 RID: 29495
	public TextAsset MouseSmoothingInput;

	// Token: 0x04007338 RID: 29496
	public TextAsset OldSchoolCamera;

	// Token: 0x04007339 RID: 29497
	public TextAsset OldSchoolController;

	// Token: 0x0400733A RID: 29498
	public TextAsset OldSchoolWeapon;

	// Token: 0x0400733B RID: 29499
	public TextAsset OldSchoolShooter;

	// Token: 0x0400733C RID: 29500
	public TextAsset OldSchoolInput;

	// Token: 0x0400733D RID: 29501
	public TextAsset Persp1999Camera;

	// Token: 0x0400733E RID: 29502
	public TextAsset Persp1999Weapon;

	// Token: 0x0400733F RID: 29503
	public TextAsset Persp1999Input;

	// Token: 0x04007340 RID: 29504
	public TextAsset PerspModernCamera;

	// Token: 0x04007341 RID: 29505
	public TextAsset PerspModernWeapon;

	// Token: 0x04007342 RID: 29506
	public TextAsset PerspModernInput;

	// Token: 0x04007343 RID: 29507
	public TextAsset PerspOldCamera;

	// Token: 0x04007344 RID: 29508
	public TextAsset PerspOldWeapon;

	// Token: 0x04007345 RID: 29509
	public TextAsset PerspOldInput;

	// Token: 0x04007346 RID: 29510
	public TextAsset PivotChestWeapon;

	// Token: 0x04007347 RID: 29511
	public TextAsset PivotElbowWeapon;

	// Token: 0x04007348 RID: 29512
	public TextAsset PivotMuzzleWeapon;

	// Token: 0x04007349 RID: 29513
	public TextAsset PivotWristWeapon;

	// Token: 0x0400734A RID: 29514
	public TextAsset SmackController;

	// Token: 0x0400734B RID: 29515
	public TextAsset SniperCamera;

	// Token: 0x0400734C RID: 29516
	public TextAsset SniperWeapon;

	// Token: 0x0400734D RID: 29517
	public TextAsset SniperShooter;

	// Token: 0x0400734E RID: 29518
	public TextAsset SniperInput;

	// Token: 0x0400734F RID: 29519
	public TextAsset StompingCamera;

	// Token: 0x04007350 RID: 29520
	public TextAsset StompingInput;

	// Token: 0x04007351 RID: 29521
	public TextAsset SystemOFFCamera;

	// Token: 0x04007352 RID: 29522
	public TextAsset SystemOFFController;

	// Token: 0x04007353 RID: 29523
	public TextAsset SystemOFFShooter;

	// Token: 0x04007354 RID: 29524
	public TextAsset SystemOFFWeapon;

	// Token: 0x04007355 RID: 29525
	public TextAsset SystemOFFWeaponGlideIn;

	// Token: 0x04007356 RID: 29526
	public TextAsset SystemOFFInput;

	// Token: 0x04007357 RID: 29527
	public TextAsset TurretCamera;

	// Token: 0x04007358 RID: 29528
	public TextAsset TurretWeapon;

	// Token: 0x04007359 RID: 29529
	public TextAsset TurretShooter;

	// Token: 0x0400735A RID: 29530
	public TextAsset TurretInput;

	// Token: 0x0400735B RID: 29531
	public TextAsset WallFacingCamera;

	// Token: 0x0400735C RID: 29532
	public TextAsset WallFacingWeapon;

	// Token: 0x0400735D RID: 29533
	public TextAsset WallFacingInput;
}
