using System;
using System.Collections.Generic;
using InControl;
using Platform;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000FBC RID: 4028
public class GUIWindowConsole : GUIWindowUGUI
{
	// Token: 0x17000D5D RID: 3421
	// (get) Token: 0x06008055 RID: 32853 RVA: 0x00341D83 File Offset: 0x0033FF83
	public override string UIPrefabPath
	{
		get
		{
			return "GUI/Prefabs/ConsoleWindow";
		}
	}

	// Token: 0x06008056 RID: 32854 RVA: 0x00341D8C File Offset: 0x0033FF8C
	public GUIWindowConsole() : base(GUIWindowConsole.ID)
	{
		Log.LogCallbacks += this.LogCallback;
		this.alwaysUsesMouseCursor = true;
		this.components = this.canvas.GetComponent<GUIWindowConsoleComponents>();
		this.scrollRect = this.components.scrollRect;
		this.contentRect = this.components.contentRect;
		this.commandField = this.components.commandField;
		this.commandField.onSubmit.AddListener(new UnityAction<string>(this.EnterCommand));
		this.commandField.shouldActivateOnSelect = !TouchScreenKeyboard.isSupported;
		this.components.closeButton.onClick.AddListener(new UnityAction(this.CloseConsole));
		this.components.openLogsButton.onClick.AddListener(delegate()
		{
			GameIO.OpenExplorer(Application.consoleLogPath);
		});
		if ((DeviceFlag.XBoxSeriesS | DeviceFlag.XBoxSeriesX | DeviceFlag.PS5).IsCurrent())
		{
			this.components.openLogsButton.gameObject.SetActive(false);
		}
		for (int i = 0; i < 5; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.components.consoleLinePrefab);
			gameObject.SetActive(false);
			gameObject.transform.SetParent(this.contentRect, false);
			this.linePool.Push(gameObject.GetComponent<Text>());
		}
		PlatformManager.NativePlatform.Input.OnLastInputStyleChanged += this.Input_OnLastInputStyleChanged;
	}

	// Token: 0x06008057 RID: 32855 RVA: 0x00341F3D File Offset: 0x0034013D
	[PublicizedFrom(EAccessModifier.Private)]
	public void Input_OnLastInputStyleChanged(PlayerInputManager.InputStyle _inputStyle)
	{
		if (_inputStyle == PlayerInputManager.InputStyle.Keyboard)
		{
			this.components.controllerPrompts.SetActive(false);
			return;
		}
		this.components.controllerPrompts.SetActive(true);
		this.components.RefreshButtonPrompts();
	}

	// Token: 0x06008058 RID: 32856 RVA: 0x00341F74 File Offset: 0x00340174
	[PublicizedFrom(EAccessModifier.Private)]
	public Text AllocText()
	{
		Text text;
		if (this.linePool.TryPop(out text))
		{
			text.gameObject.SetActive(true);
			return text;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.components.consoleLinePrefab);
		gameObject.transform.SetParent(this.contentRect, false);
		return gameObject.GetComponent<Text>();
	}

	// Token: 0x06008059 RID: 32857 RVA: 0x00341FC5 File Offset: 0x003401C5
	[PublicizedFrom(EAccessModifier.Private)]
	public void FreeText(Text _text)
	{
		_text.gameObject.SetActive(false);
		this.linePool.Push(_text);
	}

	// Token: 0x0600805A RID: 32858 RVA: 0x00341FE0 File Offset: 0x003401E0
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddDisplayedLine(GUIWindowConsole.ConsoleLine _line)
	{
		foreach (StringSpan stringSpan in _line.text.GetSplitAnyEnumerator(GUIWindowConsole.lineSeparators, StringSplitOptions.RemoveEmptyEntries))
		{
			Text text;
			if (this.displayedLines.Count == 300)
			{
				text = this.displayedLines.Dequeue();
			}
			else
			{
				text = this.AllocText();
			}
			if (stringSpan.Length > 500)
			{
				text.text = SpanUtils.Concat(stringSpan.Slice(0, 500), "...");
			}
			else
			{
				text.text = stringSpan.ToString();
			}
			text.color = _line.GetLogColor();
			text.transform.SetAsLastSibling();
			this.displayedLines.Enqueue(text);
		}
	}

	// Token: 0x0600805B RID: 32859 RVA: 0x003420C0 File Offset: 0x003402C0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearDisplayedLines()
	{
		Text text;
		while (this.displayedLines.TryDequeue(out text))
		{
			this.FreeText(text);
		}
	}

	// Token: 0x0600805C RID: 32860 RVA: 0x003420E5 File Offset: 0x003402E5
	public void Shutdown()
	{
		Log.LogCallbacks -= this.LogCallback;
	}

	// Token: 0x0600805D RID: 32861 RVA: 0x003420F8 File Offset: 0x003402F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void LogCallback(string _msg, string _trace, LogType _type)
	{
		switch (_type)
		{
		case LogType.Assert:
			this.openConsole(_msg);
			break;
		case LogType.Exception:
			this.openConsole(_msg);
			break;
		}
		this.internalAddLine(new GUIWindowConsole.ConsoleLine(_msg, _trace, _type));
	}

	// Token: 0x0600805E RID: 32862 RVA: 0x00342134 File Offset: 0x00340334
	[PublicizedFrom(EAccessModifier.Private)]
	public void openConsole(string _logString)
	{
		if (Submission.Enabled)
		{
			return;
		}
		if (_logString.StartsWith("Can't send RPC"))
		{
			return;
		}
		if (_logString.StartsWith("You are trying to load data from"))
		{
			return;
		}
		this.windowManager.OpenIfNotOpen(GUIWindowConsole.ID, false, false, true);
	}

	// Token: 0x0600805F RID: 32863 RVA: 0x00342170 File Offset: 0x00340370
	public void AddLines(string[] _lines)
	{
		for (int i = 0; i < _lines.Length; i++)
		{
			this.AddLine(_lines[i]);
		}
	}

	// Token: 0x06008060 RID: 32864 RVA: 0x00342194 File Offset: 0x00340394
	public void AddLines(List<string> _lines)
	{
		for (int i = 0; i < _lines.Count; i++)
		{
			this.AddLine(_lines[i]);
		}
	}

	// Token: 0x06008061 RID: 32865 RVA: 0x003421BF File Offset: 0x003403BF
	public void AddLine(string _line)
	{
		this.internalAddLine(new GUIWindowConsole.ConsoleLine(_line, string.Empty, LogType.Log));
	}

	// Token: 0x06008062 RID: 32866 RVA: 0x003421D4 File Offset: 0x003403D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void internalAddLine(GUIWindowConsole.ConsoleLine consoleLine)
	{
		Queue<GUIWindowConsole.ConsoleLine> obj = this.linesToAdd;
		lock (obj)
		{
			this.linesToAdd.Enqueue(consoleLine);
			while (this.linesToAdd.Count > 300)
			{
				this.linesToAdd.Dequeue();
			}
		}
	}

	// Token: 0x06008063 RID: 32867 RVA: 0x0034223C File Offset: 0x0034043C
	public override void Update()
	{
		base.Update();
		this.scrolledToBottom = (this.scrollRect.verticalNormalizedPosition < 0.1f);
		bool flag = false;
		Queue<GUIWindowConsole.ConsoleLine> obj = this.linesToAdd;
		lock (obj)
		{
			if (this.linesToAdd.Count > 0)
			{
				flag = true;
				foreach (GUIWindowConsole.ConsoleLine line in this.linesToAdd)
				{
					this.AddDisplayedLine(line);
				}
				this.linesToAdd.Clear();
			}
		}
		if (flag && this.scrolledToBottom)
		{
			Canvas.ForceUpdateCanvases();
			this.scrollRect.verticalNormalizedPosition = 0f;
		}
		if (this.bFirstTime)
		{
			if (!TouchScreenKeyboard.isSupported)
			{
				this.commandField.Select();
				this.commandField.ActivateInputField();
			}
			this.scrollRect.verticalNormalizedPosition = 0f;
			this.bFirstTime = false;
		}
		if (this.bUpdateCursor)
		{
			this.commandField.MoveTextEnd(false);
			this.bUpdateCursor = false;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			this.PreviousCommand();
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			this.NextCommand();
		}
		else if (Input.GetKeyDown(KeyCode.PageUp))
		{
			float num = this.CalculateNormalizedPageSize();
			this.scrollRect.verticalNormalizedPosition = Math.Min(this.scrollRect.verticalNormalizedPosition + num, 1f);
		}
		else if (Input.GetKeyDown(KeyCode.PageDown))
		{
			float num2 = this.CalculateNormalizedPageSize();
			this.scrollRect.verticalNormalizedPosition = Math.Max(this.scrollRect.verticalNormalizedPosition - num2, 0f);
		}
		PlayerActionsLocal playerInput = this.playerUI.playerInput;
		PlayerActionsGUI playerActionsGUI = (playerInput != null) ? playerInput.GUIActions : null;
		if (playerActionsGUI == null)
		{
			return;
		}
		if (playerActionsGUI.Submit.WasPressed)
		{
			this.EnterCommand(this.commandField.text);
		}
		else if (playerActionsGUI.DPad_Up.WasPressed && playerActionsGUI.DPad_Up.LastDeviceClass != InputDeviceClass.Keyboard)
		{
			this.PreviousCommand();
		}
		else if (playerActionsGUI.DPad_Down.WasPressed && playerActionsGUI.DPad_Down.LastDeviceClass != InputDeviceClass.Keyboard)
		{
			this.NextCommand();
		}
		else if (playerActionsGUI.DPad_Left.WasPressed && playerActionsGUI.DPad_Down.LastDeviceClass != InputDeviceClass.Keyboard)
		{
			IVirtualKeyboard virtualKeyboard = PlatformManager.NativePlatform.VirtualKeyboard;
			if (virtualKeyboard != null)
			{
				virtualKeyboard.Open("Enter Command", this.commandField.text, new Action<bool, string>(this.OnTextReceived), UIInput.InputType.Standard, false, 200U);
			}
		}
		else if (this.playerUI.playerInput.PermanentActions.Cancel.WasReleased || PlayerActionsGlobal.Instance.Console.WasPressed)
		{
			this.CloseConsole();
		}
		float y = playerActionsGUI.Camera.Vector.y;
		if (y != 0f)
		{
			float num3 = this.CalculateNormalizedPageSize();
			this.scrollRect.verticalNormalizedPosition = Math.Max(this.scrollRect.verticalNormalizedPosition + num3 * y * 0.05f, 0f);
		}
	}

	// Token: 0x06008064 RID: 32868 RVA: 0x0034256C File Offset: 0x0034076C
	[PublicizedFrom(EAccessModifier.Private)]
	public float CalculateNormalizedPageSize()
	{
		float height = this.scrollRect.viewport.rect.height;
		float num = this.scrollRect.content.rect.height - height;
		if (num > height)
		{
			return Math.Max(height / num, 0.01f);
		}
		return 1f;
	}

	// Token: 0x06008065 RID: 32869 RVA: 0x003425C4 File Offset: 0x003407C4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTextReceived(bool _success, string _text)
	{
		if (_success)
		{
			this.commandField.text = _text;
		}
	}

	// Token: 0x06008066 RID: 32870 RVA: 0x003425D5 File Offset: 0x003407D5
	[PublicizedFrom(EAccessModifier.Private)]
	public void CloseConsole()
	{
		this.windowManager.Close(this, false);
		this.commandField.text = string.Empty;
	}

	// Token: 0x06008067 RID: 32871 RVA: 0x003425F4 File Offset: 0x003407F4
	[PublicizedFrom(EAccessModifier.Private)]
	public void EnterCommand(string _command)
	{
		if (_command.Length > 0)
		{
			if (_command == "clear")
			{
				this.Clear();
			}
			else
			{
				this.scrollRect.verticalNormalizedPosition = 0f;
				this.internalAddLine(new GUIWindowConsole.ConsoleLine("> " + _command, string.Empty, LogType.Log));
				if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
				{
					this.AddLines(SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteSync(_command, null));
				}
				else
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageConsoleCmdServer>().Setup(_command), false);
				}
			}
			if (this.lastCommands.Count == 0 || !this.lastCommands[this.lastCommands.Count - 1].Equals(_command))
			{
				if (this.lastCommands.Contains(_command))
				{
					this.lastCommands.Remove(_command);
				}
				this.lastCommands.Add(_command);
			}
			this.lastCommandsIdx = this.lastCommands.Count;
			this.commandField.text = "";
			if (!TouchScreenKeyboard.isSupported)
			{
				this.commandField.Select();
				this.commandField.ActivateInputField();
			}
		}
	}

	// Token: 0x06008068 RID: 32872 RVA: 0x00342718 File Offset: 0x00340918
	[PublicizedFrom(EAccessModifier.Private)]
	public void PreviousCommand()
	{
		if (this.lastCommands.Count > 0)
		{
			this.lastCommandsIdx = Mathf.Max(0, this.lastCommandsIdx - 1);
			this.commandField.text = this.lastCommands[this.lastCommandsIdx];
			if (!TouchScreenKeyboard.isSupported)
			{
				this.commandField.Select();
				this.commandField.ActivateInputField();
			}
			this.bUpdateCursor = true;
		}
	}

	// Token: 0x06008069 RID: 32873 RVA: 0x00342788 File Offset: 0x00340988
	[PublicizedFrom(EAccessModifier.Private)]
	public void NextCommand()
	{
		if (this.lastCommands.Count > 0)
		{
			this.lastCommandsIdx = Mathf.Min(this.lastCommands.Count, this.lastCommandsIdx + 1);
			if (this.lastCommandsIdx < this.lastCommands.Count)
			{
				this.commandField.text = this.lastCommands[this.lastCommandsIdx];
				this.bUpdateCursor = true;
				if (!TouchScreenKeyboard.isSupported)
				{
					this.commandField.Select();
					this.commandField.ActivateInputField();
					return;
				}
			}
			else
			{
				this.commandField.text = string.Empty;
			}
		}
	}

	// Token: 0x0600806A RID: 32874 RVA: 0x00342828 File Offset: 0x00340A28
	public void Clear()
	{
		this.ClearDisplayedLines();
	}

	// Token: 0x0600806B RID: 32875 RVA: 0x00342830 File Offset: 0x00340A30
	public override void OnOpen()
	{
		base.OnOpen();
		if (GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) && this.windowManager.IsWindowOpen(XUiC_InGameDebugMenu.ID))
		{
			this.bShouldReopenGebugMenu = true;
			this.windowManager.Close(XUiC_InGameDebugMenu.ID);
		}
		else
		{
			this.bShouldReopenGebugMenu = false;
		}
		this.commandField.text = string.Empty;
		this.bFirstTime = true;
		this.isInputActive = true;
		if (UIInput.selection != null)
		{
			UIInput.selection.isSelected = false;
		}
	}

	// Token: 0x0600806C RID: 32876 RVA: 0x003428B4 File Offset: 0x00340AB4
	public override void OnClose()
	{
		this.scrollRect.verticalNormalizedPosition = 0f;
		base.OnClose();
		if (GamePrefs.GetBool(EnumGamePrefs.DebugMenuEnabled) && this.bShouldReopenGebugMenu)
		{
			this.windowManager.Open(XUiC_InGameDebugMenu.ID, false, false, true);
		}
		this.bShouldReopenGebugMenu = false;
		this.isInputActive = false;
	}

	// Token: 0x0400632C RID: 25388
	public static string ID = typeof(GUIWindowConsole).Name;

	// Token: 0x0400632D RID: 25389
	[PublicizedFrom(EAccessModifier.Private)]
	public bool scrolledToBottom = true;

	// Token: 0x0400632E RID: 25390
	[PublicizedFrom(EAccessModifier.Private)]
	public Queue<GUIWindowConsole.ConsoleLine> linesToAdd = new Queue<GUIWindowConsole.ConsoleLine>(301);

	// Token: 0x0400632F RID: 25391
	[PublicizedFrom(EAccessModifier.Private)]
	public Queue<Text> displayedLines = new Queue<Text>();

	// Token: 0x04006330 RID: 25392
	[PublicizedFrom(EAccessModifier.Private)]
	public const int maxConsoleLines = 300;

	// Token: 0x04006331 RID: 25393
	[PublicizedFrom(EAccessModifier.Private)]
	public Stack<Text> linePool = new Stack<Text>();

	// Token: 0x04006332 RID: 25394
	[PublicizedFrom(EAccessModifier.Private)]
	public List<string> lastCommands = new List<string>();

	// Token: 0x04006333 RID: 25395
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastCommandsIdx;

	// Token: 0x04006334 RID: 25396
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFirstTime;

	// Token: 0x04006335 RID: 25397
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bUpdateCursor;

	// Token: 0x04006336 RID: 25398
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bShouldReopenGebugMenu;

	// Token: 0x04006337 RID: 25399
	[PublicizedFrom(EAccessModifier.Private)]
	public GUIWindowConsoleComponents components;

	// Token: 0x04006338 RID: 25400
	[PublicizedFrom(EAccessModifier.Private)]
	public ScrollRect scrollRect;

	// Token: 0x04006339 RID: 25401
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform contentRect;

	// Token: 0x0400633A RID: 25402
	[PublicizedFrom(EAccessModifier.Private)]
	public InputField commandField;

	// Token: 0x0400633B RID: 25403
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string[] lineSeparators = new string[]
	{
		"\r\n",
		"\r",
		"\n"
	};

	// Token: 0x02000FBD RID: 4029
	[PublicizedFrom(EAccessModifier.Private)]
	public struct ConsoleLine
	{
		// Token: 0x0600806E RID: 32878 RVA: 0x00342942 File Offset: 0x00340B42
		public ConsoleLine(string _text, string _stackTrace, LogType _type)
		{
			this.text = _text;
			this.stackTrace = _stackTrace;
			this.type = _type;
		}

		// Token: 0x0600806F RID: 32879 RVA: 0x0034295C File Offset: 0x00340B5C
		public Color GetLogColor()
		{
			switch (this.type)
			{
			case LogType.Error:
			case LogType.Assert:
			case LogType.Exception:
				return Color.red;
			case LogType.Warning:
				return Color.yellow;
			case LogType.Log:
				return Color.white;
			default:
				return Color.white;
			}
		}

		// Token: 0x0400633C RID: 25404
		public string text;

		// Token: 0x0400633D RID: 25405
		public LogType type;

		// Token: 0x0400633E RID: 25406
		public string stackTrace;
	}
}
