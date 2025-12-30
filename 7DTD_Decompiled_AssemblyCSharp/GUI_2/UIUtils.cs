using System;
using System.Collections.Generic;
using InControl;
using Platform;
using UnityEngine;

namespace GUI_2
{
	// Token: 0x020013F6 RID: 5110
	public static class UIUtils
	{
		// Token: 0x17001139 RID: 4409
		// (get) Token: 0x06009F10 RID: 40720 RVA: 0x003F1DF9 File Offset: 0x003EFFF9
		public static UIAtlas IconAtlas
		{
			get
			{
				return UIUtils.symbolAtlas;
			}
		}

		// Token: 0x06009F11 RID: 40721 RVA: 0x003F1E00 File Offset: 0x003F0000
		public static string GetSpriteName(UIUtils.ButtonIcon _icon)
		{
			if (PlayerInputManager.InputStyleFromSelectedIconStyle() == PlayerInputManager.InputStyle.PS4)
			{
				return "PS5_" + UIUtils.buttonIconMap[_icon];
			}
			return "XB_" + UIUtils.buttonIconMap[_icon];
		}

		// Token: 0x06009F12 RID: 40722 RVA: 0x003F1E38 File Offset: 0x003F0038
		public static UIUtils.ButtonIcon GetButtonIconForAction(PlayerAction _action)
		{
			if (_action == null)
			{
				return UIUtils.ButtonIcon.None;
			}
			DeviceBindingSource deviceBindingSource = _action.GetBindingOfType(true) as DeviceBindingSource;
			if (deviceBindingSource == null)
			{
				if (UIUtils.loggedMissingBindingSources.Add(_action))
				{
					Log.Warning("UIUtils: No device binding source could be found for PlayerAction {0}", new object[]
					{
						_action.Name
					});
				}
				return UIUtils.ButtonIcon.None;
			}
			UIUtils.ButtonIcon result;
			if (UIUtils.iconControlMap.TryGetValue(deviceBindingSource.Control, out result))
			{
				return result;
			}
			Log.Warning("UIUtils: Could not assign a ButtonIcon for device control {0}", new object[]
			{
				deviceBindingSource.Control.ToString()
			});
			return UIUtils.ButtonIcon.None;
		}

		// Token: 0x06009F13 RID: 40723 RVA: 0x003F1EC9 File Offset: 0x003F00C9
		public static void LoadAtlas()
		{
			UIUtils.symbolAtlas = Resources.Load<UIAtlas>("GUI/Prefabs/SymbolAtlas");
		}

		// Token: 0x04007A68 RID: 31336
		[PublicizedFrom(EAccessModifier.Private)]
		public static UIAtlas symbolAtlas;

		// Token: 0x04007A69 RID: 31337
		[PublicizedFrom(EAccessModifier.Private)]
		public const string sprite_PS = "PS5_";

		// Token: 0x04007A6A RID: 31338
		[PublicizedFrom(EAccessModifier.Private)]
		public const string sprite_XB = "XB_";

		// Token: 0x04007A6B RID: 31339
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<UIUtils.ButtonIcon, string> buttonIconMap = new EnumDictionary<UIUtils.ButtonIcon, string>
		{
			{
				UIUtils.ButtonIcon.FaceButtonSouth,
				"Button_Action1"
			},
			{
				UIUtils.ButtonIcon.FaceButtonNorth,
				"Button_Action4"
			},
			{
				UIUtils.ButtonIcon.FaceButtonEast,
				"Button_Action2"
			},
			{
				UIUtils.ButtonIcon.FaceButtonWest,
				"Button_Action3"
			},
			{
				UIUtils.ButtonIcon.ConfirmButton,
				"Button_Action1"
			},
			{
				UIUtils.ButtonIcon.CancelButton,
				"Button_Action2"
			},
			{
				UIUtils.ButtonIcon.LeftBumper,
				"Button_LeftBumper"
			},
			{
				UIUtils.ButtonIcon.LeftTrigger,
				"Button_LeftTrigger"
			},
			{
				UIUtils.ButtonIcon.RightBumper,
				"Button_RightBumper"
			},
			{
				UIUtils.ButtonIcon.RightTrigger,
				"Button_RightTrigger"
			},
			{
				UIUtils.ButtonIcon.LeftStick,
				"Button_LeftStick"
			},
			{
				UIUtils.ButtonIcon.LeftStickUpDown,
				"Button_LeftStickUpDown"
			},
			{
				UIUtils.ButtonIcon.LeftStickLeftRight,
				"Button_LeftStickLeftRight"
			},
			{
				UIUtils.ButtonIcon.LeftStickButton,
				"Button_LeftStickButton"
			},
			{
				UIUtils.ButtonIcon.RightStick,
				"Button_RightStick"
			},
			{
				UIUtils.ButtonIcon.RightStickUpDown,
				"Button_RightStickUpDown"
			},
			{
				UIUtils.ButtonIcon.RightStickLeftRight,
				"Button_RightStickLeftRight"
			},
			{
				UIUtils.ButtonIcon.RightStickButton,
				"Button_RightStickButton"
			},
			{
				UIUtils.ButtonIcon.DPadLeft,
				"Button_DPadLeft"
			},
			{
				UIUtils.ButtonIcon.DPadRight,
				"Button_DPadRight"
			},
			{
				UIUtils.ButtonIcon.DPadUp,
				"Button_DPadUp"
			},
			{
				UIUtils.ButtonIcon.DPadDown,
				"Button_DPadDown"
			},
			{
				UIUtils.ButtonIcon.StartButton,
				"Button_Start"
			},
			{
				UIUtils.ButtonIcon.BackButton,
				"Button_Back"
			},
			{
				UIUtils.ButtonIcon.None,
				""
			}
		};

		// Token: 0x04007A6C RID: 31340
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<InputControlType, UIUtils.ButtonIcon> iconControlMap = new EnumDictionary<InputControlType, UIUtils.ButtonIcon>
		{
			{
				InputControlType.Action1,
				UIUtils.ButtonIcon.FaceButtonSouth
			},
			{
				InputControlType.Action2,
				UIUtils.ButtonIcon.FaceButtonEast
			},
			{
				InputControlType.Action3,
				UIUtils.ButtonIcon.FaceButtonWest
			},
			{
				InputControlType.Action4,
				UIUtils.ButtonIcon.FaceButtonNorth
			},
			{
				InputControlType.LeftBumper,
				UIUtils.ButtonIcon.LeftBumper
			},
			{
				InputControlType.RightBumper,
				UIUtils.ButtonIcon.RightBumper
			},
			{
				InputControlType.LeftTrigger,
				UIUtils.ButtonIcon.LeftTrigger
			},
			{
				InputControlType.RightTrigger,
				UIUtils.ButtonIcon.RightTrigger
			},
			{
				InputControlType.LeftStickButton,
				UIUtils.ButtonIcon.LeftStickButton
			},
			{
				InputControlType.RightStickButton,
				UIUtils.ButtonIcon.RightStickButton
			},
			{
				InputControlType.DPadUp,
				UIUtils.ButtonIcon.DPadUp
			},
			{
				InputControlType.DPadDown,
				UIUtils.ButtonIcon.DPadDown
			},
			{
				InputControlType.DPadLeft,
				UIUtils.ButtonIcon.DPadLeft
			},
			{
				InputControlType.DPadRight,
				UIUtils.ButtonIcon.DPadRight
			},
			{
				InputControlType.Start,
				UIUtils.ButtonIcon.StartButton
			},
			{
				InputControlType.Menu,
				UIUtils.ButtonIcon.StartButton
			},
			{
				InputControlType.Options,
				UIUtils.ButtonIcon.StartButton
			},
			{
				InputControlType.Plus,
				UIUtils.ButtonIcon.StartButton
			},
			{
				InputControlType.Select,
				UIUtils.ButtonIcon.BackButton
			},
			{
				InputControlType.View,
				UIUtils.ButtonIcon.BackButton
			},
			{
				InputControlType.TouchPadButton,
				UIUtils.ButtonIcon.BackButton
			},
			{
				InputControlType.Minus,
				UIUtils.ButtonIcon.BackButton
			},
			{
				InputControlType.None,
				UIUtils.ButtonIcon.None
			}
		};

		// Token: 0x04007A6D RID: 31341
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly HashSet<PlayerAction> loggedMissingBindingSources = new HashSet<PlayerAction>();

		// Token: 0x020013F7 RID: 5111
		public enum ButtonIcon
		{
			// Token: 0x04007A6F RID: 31343
			FaceButtonSouth,
			// Token: 0x04007A70 RID: 31344
			FaceButtonNorth,
			// Token: 0x04007A71 RID: 31345
			FaceButtonEast,
			// Token: 0x04007A72 RID: 31346
			FaceButtonWest,
			// Token: 0x04007A73 RID: 31347
			ConfirmButton,
			// Token: 0x04007A74 RID: 31348
			CancelButton,
			// Token: 0x04007A75 RID: 31349
			LeftBumper,
			// Token: 0x04007A76 RID: 31350
			RightBumper,
			// Token: 0x04007A77 RID: 31351
			LeftTrigger,
			// Token: 0x04007A78 RID: 31352
			RightTrigger,
			// Token: 0x04007A79 RID: 31353
			LeftStick,
			// Token: 0x04007A7A RID: 31354
			LeftStickUpDown,
			// Token: 0x04007A7B RID: 31355
			LeftStickLeftRight,
			// Token: 0x04007A7C RID: 31356
			LeftStickButton,
			// Token: 0x04007A7D RID: 31357
			RightStick,
			// Token: 0x04007A7E RID: 31358
			RightStickUpDown,
			// Token: 0x04007A7F RID: 31359
			RightStickLeftRight,
			// Token: 0x04007A80 RID: 31360
			RightStickButton,
			// Token: 0x04007A81 RID: 31361
			DPadLeft,
			// Token: 0x04007A82 RID: 31362
			DPadRight,
			// Token: 0x04007A83 RID: 31363
			DPadUp,
			// Token: 0x04007A84 RID: 31364
			DPadDown,
			// Token: 0x04007A85 RID: 31365
			StartButton,
			// Token: 0x04007A86 RID: 31366
			BackButton,
			// Token: 0x04007A87 RID: 31367
			None,
			// Token: 0x04007A88 RID: 31368
			Count
		}
	}
}
