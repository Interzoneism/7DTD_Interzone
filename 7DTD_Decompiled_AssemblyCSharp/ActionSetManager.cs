using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using InControl;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020004D4 RID: 1236
[UnityEngine.Scripting.Preserve]
public class ActionSetManager
{
	// Token: 0x1700041B RID: 1051
	// (get) Token: 0x0600282A RID: 10282 RVA: 0x00104E40 File Offset: 0x00103040
	public bool Empty
	{
		get
		{
			return this.PlayerActions.Count <= 0;
		}
	}

	// Token: 0x1700041C RID: 1052
	// (get) Token: 0x0600282B RID: 10283 RVA: 0x00104E53 File Offset: 0x00103053
	public PlayerActionSet Top
	{
		get
		{
			return this.PlayerActions[this.PlayerActions.Count - 1];
		}
	}

	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x0600282C RID: 10284 RVA: 0x00104E6D File Offset: 0x0010306D
	public static ActionSetManager.EDebugLevel DebugLevel
	{
		get
		{
			return ActionSetManager.debug;
		}
	}

	// Token: 0x0600282D RID: 10285 RVA: 0x00104E74 File Offset: 0x00103074
	[PublicizedFrom(EAccessModifier.Private)]
	static ActionSetManager()
	{
		string launchArgument = GameUtils.GetLaunchArgument("debuginput");
		if (launchArgument != null)
		{
			if (launchArgument == "verbose")
			{
				ActionSetManager.debug = ActionSetManager.EDebugLevel.Verbose;
				return;
			}
			ActionSetManager.debug = ActionSetManager.EDebugLevel.Normal;
		}
	}

	// Token: 0x0600282E RID: 10286 RVA: 0x00104EB0 File Offset: 0x001030B0
	public void Insert(PlayerActionSet _playerAction, int _index, string _windowName = null)
	{
		if (ActionSetManager.debug != ActionSetManager.EDebugLevel.Off)
		{
			Log.Out("LocalPlayerInput.Insert ({2} - {0}):{1}", new object[]
			{
				_playerAction.GetType().FullName,
				(ActionSetManager.debug == ActionSetManager.EDebugLevel.Verbose) ? ("\n" + StackTraceUtility.ExtractStackTrace()) : "",
				_windowName
			});
		}
		if (_playerAction == null)
		{
			Log.Warning("LocalPlayerInput::Insert - Inserting a null input onto stack.");
		}
		if (!this.Empty)
		{
			this.Top.Enabled = false;
		}
		_playerAction.Enabled = false;
		this.PlayerActions.Insert(_index, _playerAction);
		this.Top.Enabled = true;
		if (ActionSetManager.debug != ActionSetManager.EDebugLevel.Off)
		{
			this.LogActionSets();
		}
	}

	// Token: 0x0600282F RID: 10287 RVA: 0x00104F58 File Offset: 0x00103158
	public void Remove(PlayerActionSet _playerAction, int _minIndex, string _windowName = null)
	{
		if (ActionSetManager.debug != ActionSetManager.EDebugLevel.Off)
		{
			Log.Out("LocalPlayerInput.Remove ({2} - {0}):{1}", new object[]
			{
				_playerAction.GetType().FullName,
				(ActionSetManager.debug == ActionSetManager.EDebugLevel.Verbose) ? ("\n" + StackTraceUtility.ExtractStackTrace()) : "",
				_windowName
			});
		}
		if (_playerAction == null)
		{
			Log.Warning("LocalPlayerInput::Remove - Trying to remove a null input from the stack.");
		}
		if (this.Empty)
		{
			Log.Warning("LocalPlayerInput::Remove - Removing input from an empty stack.");
			return;
		}
		this.Top.Enabled = false;
		_playerAction.Enabled = false;
		int num = -1;
		for (int i = this.PlayerActions.Count - 1; i >= _minIndex; i--)
		{
			if (this.PlayerActions[i] == _playerAction)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			this.PlayerActions.RemoveAt(num);
		}
		else
		{
			Log.Warning(string.Format("LocalPlayerInput::Remove - Failed to find action set of type '{0}' with a min index of {1} to remove.", _playerAction.GetType().FullName, _minIndex));
		}
		if (!this.Empty)
		{
			this.Top.Enabled = true;
		}
		if (ActionSetManager.debug != ActionSetManager.EDebugLevel.Off)
		{
			this.LogActionSets();
		}
	}

	// Token: 0x06002830 RID: 10288 RVA: 0x00105062 File Offset: 0x00103262
	public void Push(PlayerActionSet _playerAction)
	{
		if (_playerAction == null)
		{
			Log.Warning("LocalPlayerInput::Push - Pushing a null input onto stack.");
		}
		this.PushInternal(_playerAction, null);
	}

	// Token: 0x06002831 RID: 10289 RVA: 0x00105079 File Offset: 0x00103279
	public void Push(GUIWindow _window)
	{
		if (((_window != null) ? _window.GetActionSet() : null) == null)
		{
			Log.Warning("LocalPlayerInput::Push - Pushing a null input onto stack.");
		}
		this.PushInternal((_window != null) ? _window.GetActionSet() : null, (_window != null) ? _window.Id : null);
	}

	// Token: 0x06002832 RID: 10290 RVA: 0x001050B4 File Offset: 0x001032B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void PushInternal(PlayerActionSet _playerAction, string _windowName = null)
	{
		if (ActionSetManager.debug != ActionSetManager.EDebugLevel.Off)
		{
			Log.Out("LocalPlayerInput.Push ({2} - {0}):{1}", new object[]
			{
				_playerAction.GetType().FullName,
				(ActionSetManager.debug == ActionSetManager.EDebugLevel.Verbose) ? ("\n" + StackTraceUtility.ExtractStackTrace()) : "",
				_windowName
			});
		}
		if (!this.Empty)
		{
			this.Top.Enabled = false;
		}
		this.PlayerActions.Add(_playerAction);
		this.Top.Enabled = true;
		if (ActionSetManager.debug != ActionSetManager.EDebugLevel.Off)
		{
			this.LogActionSets();
		}
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x00105144 File Offset: 0x00103344
	public void Pop(GUIWindow _window = null)
	{
		if (ActionSetManager.debug != ActionSetManager.EDebugLevel.Off)
		{
			Log.Out("LocalPlayerInput.Pop ({1}):{0}", new object[]
			{
				(ActionSetManager.debug == ActionSetManager.EDebugLevel.Verbose) ? ("\n" + StackTraceUtility.ExtractStackTrace()) : "",
				(_window != null) ? _window.Id : null
			});
		}
		if (this.Empty)
		{
			Log.Warning("LocalPlayerInput::Pop - Popping input from an empty stack.");
			return;
		}
		int index = this.PlayerActions.Count - 1;
		if (_window != null)
		{
			PlayerActionsBase actionSet = _window.GetActionSet();
			if (actionSet != null && actionSet != this.PlayerActions[index])
			{
				Log.Warning("LocalPlayerInput::Pop - Tried to pop a different action set from what belongs to window " + _window.Id);
				return;
			}
		}
		this.Top.Enabled = false;
		this.PlayerActions.RemoveAt(index);
		if (!this.Empty)
		{
			this.Top.Enabled = true;
		}
		if (ActionSetManager.debug != ActionSetManager.EDebugLevel.Off)
		{
			this.LogActionSets();
		}
	}

	// Token: 0x06002834 RID: 10292 RVA: 0x00105224 File Offset: 0x00103424
	public void LogActionSets()
	{
		string text = "";
		for (int i = 0; i < this.PlayerActions.Count; i++)
		{
			text = string.Concat(new string[]
			{
				text,
				this.PlayerActions[i].GetType().Name,
				" (",
				this.PlayerActions[i].Enabled.ToString(),
				"), "
			});
		}
		string text2 = "";
		IPlatform nativePlatform = PlatformManager.NativePlatform;
		int? num;
		if (nativePlatform == null)
		{
			num = null;
		}
		else
		{
			PlayerInputManager input = nativePlatform.Input;
			if (input == null)
			{
				num = null;
			}
			else
			{
				ReadOnlyCollection<PlayerActionsBase> actionSets = input.ActionSets;
				num = ((actionSets != null) ? new int?(actionSets.Count) : null);
			}
		}
		int? num2 = num;
		if (num2.GetValueOrDefault() > 0)
		{
			for (int j = 0; j < PlatformManager.NativePlatform.Input.ActionSets.Count; j++)
			{
				text2 += string.Format("{0} ({1}), ", PlatformManager.NativePlatform.Input.ActionSets[j].GetType().Name, PlatformManager.NativePlatform.Input.ActionSets[j].Enabled);
			}
		}
		Log.Out("ActionSets: Stack: {0} --- All: {1}", new object[]
		{
			text,
			text2
		});
	}

	// Token: 0x06002835 RID: 10293 RVA: 0x00105389 File Offset: 0x00103589
	public void Reset()
	{
		while (!this.Empty)
		{
			this.Pop(null);
		}
	}

	// Token: 0x04001EE5 RID: 7909
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<PlayerActionSet> PlayerActions = new List<PlayerActionSet>();

	// Token: 0x04001EE6 RID: 7910
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ActionSetManager.EDebugLevel debug = ActionSetManager.EDebugLevel.Off;

	// Token: 0x020004D5 RID: 1237
	public enum EDebugLevel
	{
		// Token: 0x04001EE8 RID: 7912
		Off,
		// Token: 0x04001EE9 RID: 7913
		Normal,
		// Token: 0x04001EEA RID: 7914
		Verbose
	}
}
