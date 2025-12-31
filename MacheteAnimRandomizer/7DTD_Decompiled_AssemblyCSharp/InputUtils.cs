using System;
using System.Collections.Generic;
using InControl;
using Platform;
using UnityEngine;

// Token: 0x020004D7 RID: 1239
public static class InputUtils
{
	// Token: 0x06002849 RID: 10313 RVA: 0x00105C50 File Offset: 0x00103E50
	public static void EnableAllPlayerActions(bool _enable)
	{
		if (ActionSetManager.DebugLevel != ActionSetManager.EDebugLevel.Off)
		{
			Log.Out("EnableAllPlayerActions: " + _enable.ToString());
		}
		if (!_enable)
		{
			InputUtils.previousState.Clear();
			using (IEnumerator<PlayerActionsBase> enumerator = PlatformManager.NativePlatform.Input.ActionSets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PlayerActionsBase playerActionsBase = enumerator.Current;
					if (ActionSetManager.DebugLevel == ActionSetManager.EDebugLevel.Verbose)
					{
						Log.Out(string.Format("PAS: {0} IsInDict {1}", playerActionsBase, InputUtils.previousState.ContainsKey(playerActionsBase)));
					}
					if (!InputUtils.previousState.ContainsKey(playerActionsBase))
					{
						if (ActionSetManager.DebugLevel == ActionSetManager.EDebugLevel.Verbose)
						{
							Log.Out(string.Format("Disabling: {0} was {1}", playerActionsBase, playerActionsBase.Enabled));
						}
						InputUtils.previousState.Add(playerActionsBase, playerActionsBase.Enabled);
						playerActionsBase.Enabled = false;
					}
				}
				return;
			}
		}
		foreach (PlayerActionsBase playerActionsBase2 in PlatformManager.NativePlatform.Input.ActionSets)
		{
			if (InputUtils.previousState.ContainsKey(playerActionsBase2))
			{
				if (ActionSetManager.DebugLevel == ActionSetManager.EDebugLevel.Verbose)
				{
					Log.Out(string.Format("PrevState contains: {0} was {1}", playerActionsBase2, InputUtils.previousState[playerActionsBase2]));
				}
				playerActionsBase2.Enabled = InputUtils.previousState[playerActionsBase2];
			}
			else
			{
				if (ActionSetManager.DebugLevel == ActionSetManager.EDebugLevel.Verbose)
				{
					Log.Out(string.Format("PrevState does not contain: {0}", playerActionsBase2));
				}
				playerActionsBase2.Enabled = true;
			}
		}
	}

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x0600284A RID: 10314 RVA: 0x00105DE4 File Offset: 0x00103FE4
	public static bool IsMac
	{
		get
		{
			if (InputUtils.isMac == null)
			{
				RuntimePlatform platform = Application.platform;
				InputUtils.isMac = new bool?(platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer);
			}
			return InputUtils.isMac.Value;
		}
	}

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x0600284B RID: 10315 RVA: 0x00105E21 File Offset: 0x00104021
	public static bool ControlKeyPressed
	{
		get
		{
			if (!InputUtils.IsMac)
			{
				return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			}
			return Input.GetKey(KeyCode.LeftMeta) || Input.GetKey(KeyCode.RightMeta);
		}
	}

	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x0600284C RID: 10316 RVA: 0x00105E5B File Offset: 0x0010405B
	public static bool ShiftKeyPressed
	{
		get
		{
			return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		}
	}

	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x0600284D RID: 10317 RVA: 0x00105E75 File Offset: 0x00104075
	public static bool AltKeyPressed
	{
		get
		{
			return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		}
	}

	// Token: 0x04001EEC RID: 7916
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<PlayerActionSet, bool> previousState = new Dictionary<PlayerActionSet, bool>();

	// Token: 0x04001EED RID: 7917
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool? isMac;
}
