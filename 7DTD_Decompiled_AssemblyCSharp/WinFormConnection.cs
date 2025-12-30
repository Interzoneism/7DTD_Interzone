using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using UnityEngine;

// Token: 0x0200028D RID: 653
public partial class WinFormConnection : Form, IConsoleConnection
{
	// Token: 0x06001287 RID: 4743 RVA: 0x00073660 File Offset: 0x00071860
	public WinFormConnection(WinFormInstance _owner)
	{
		this.initialize();
		ModEvents.GameStartDone.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameStartDoneData>(this.OnGameStartDone));
		ModEvents.GameAwake.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameAwakeData>(this.OnGameAwake));
	}

	// Token: 0x06001288 RID: 4744 RVA: 0x000736F2 File Offset: 0x000718F2
	public void CloseTerminal()
	{
		if (base.InvokeRequired)
		{
			base.BeginInvoke(new Action(this.CloseTerminal));
			return;
		}
		this.forceClose = true;
		base.Close();
		System.Windows.Forms.Application.Exit();
	}

	// Token: 0x06001289 RID: 4745 RVA: 0x00073724 File Offset: 0x00071924
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnClosing(CancelEventArgs _e)
	{
		base.OnClosing(_e);
		if (this.forceClose)
		{
			return;
		}
		_e.Cancel = true;
		if (this.shutdownRequested)
		{
			return;
		}
		if (MessageBox.Show("Really shut down the 7 Days to Die server?", "Shutdown", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		this.shutdownRequested = true;
		Log.Out("Shutdown game from Terminal Window");
		ThreadManager.AddSingleTaskMainThread("Shutdown", delegate(object _)
		{
			UnityEngine.Application.Quit();
		}, null);
	}

	// Token: 0x0600128A RID: 4746 RVA: 0x000737A2 File Offset: 0x000719A2
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnClosed(EventArgs _e)
	{
		base.OnClosed(_e);
		ModEvents.GameStartDone.UnregisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameStartDoneData>(this.OnGameStartDone));
		ModEvents.GameAwake.UnregisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameAwakeData>(this.OnGameAwake));
	}

	// Token: 0x0600128B RID: 4747 RVA: 0x000737D8 File Offset: 0x000719D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void initialize()
	{
		base.SuspendLayout();
		base.ClientSize = new Size(1000, 600);
		this.Text = "Starting - 7 Days to Die Dedicated Server Console";
		base.Icon = new Icon(this.iconPath);
		this.BackColor = System.Drawing.Color.Black;
		this.ForeColor = WinFormConnection.logColorNormal;
		this.consoleOutputBox = new RichTextBox
		{
			Dock = DockStyle.Fill,
			Multiline = true,
			ScrollBars = RichTextBoxScrollBars.Both,
			Font = new System.Drawing.Font(FontFamily.GenericMonospace, 10f),
			ReadOnly = true,
			BackColor = this.BackColor,
			ForeColor = this.ForeColor,
			BorderStyle = BorderStyle.None
		};
		base.Controls.Add(this.consoleOutputBox);
		IntPtr handle = this.consoleOutputBox.Handle;
		this.commandInputBox = new TextBox
		{
			Dock = DockStyle.Bottom,
			Multiline = false,
			Text = "",
			Font = new System.Drawing.Font(FontFamily.GenericMonospace, 12f),
			Enabled = false,
			AutoCompleteMode = AutoCompleteMode.Append,
			AutoCompleteSource = AutoCompleteSource.CustomSource,
			BackColor = System.Drawing.Color.LightGray,
			ForeColor = System.Drawing.Color.Black,
			BorderStyle = BorderStyle.FixedSingle
		};
		this.commandInputBox.KeyDown += this.CommandInputBoxOnKeyDown;
		this.commandInputBox.AutoCompleteCustomSource = new AutoCompleteStringCollection();
		base.Controls.Add(this.commandInputBox);
		this.CreateHandle();
		base.ResumeLayout();
		base.PerformLayout();
	}

	// Token: 0x0600128C RID: 4748 RVA: 0x00073960 File Offset: 0x00071B60
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGameAwake(ref ModEvents.SGameAwakeData _data)
	{
		this.Text = string.Format("{0} - Port {1} - Loading - 7 Days to Die Dedicated Server Console", GamePrefs.GetString(EnumGamePrefs.ServerName), GamePrefs.GetInt(EnumGamePrefs.ServerPort));
		foreach (IConsoleCommand consoleCommand in SingletonMonoBehaviour<SdtdConsole>.Instance.GetCommands())
		{
			this.commandInputBox.AutoCompleteCustomSource.AddRange(consoleCommand.GetCommands());
		}
	}

	// Token: 0x0600128D RID: 4749 RVA: 0x000739E4 File Offset: 0x00071BE4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnGameStartDone(ref ModEvents.SGameStartDoneData _data)
	{
		this.Text = string.Format("{0} - Port {1} - Running - 7 Days to Die Dedicated Server Console", GamePrefs.GetString(EnumGamePrefs.ServerName), GamePrefs.GetInt(EnumGamePrefs.ServerPort));
		this.commandInputBox.Enabled = true;
		this.commandInputBox.Clear();
		this.commandInputBox.Focus();
	}

	// Token: 0x0600128E RID: 4750 RVA: 0x00073A37 File Offset: 0x00071C37
	[PublicizedFrom(EAccessModifier.Private)]
	public void CommandInputBoxOnKeyDown(object _sender, KeyEventArgs _keyEventArgs)
	{
		if (_keyEventArgs.KeyCode == Keys.Return)
		{
			this.execCommand();
		}
	}

	// Token: 0x0600128F RID: 4751 RVA: 0x00073A4C File Offset: 0x00071C4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void execCommand()
	{
		if (this.commandInputBox.Enabled && this.commandInputBox.Text.Length > 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.ExecuteAsync(this.commandInputBox.Text, this);
			this.commandInputBox.Clear();
		}
	}

	// Token: 0x06001290 RID: 4752 RVA: 0x00073A9C File Offset: 0x00071C9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddLine(string _text, System.Drawing.Color _color)
	{
		if (this.consoleOutputBox.InvokeRequired)
		{
			if (this.addLineDelegate == null)
			{
				this.addLineDelegate = new Action<string, System.Drawing.Color>(this.AddLine);
			}
			base.BeginInvoke(this.addLineDelegate, new object[]
			{
				_text,
				_color
			});
			return;
		}
		this.consoleOutputBox.SelectionStart = this.consoleOutputBox.TextLength;
		this.consoleOutputBox.SelectionLength = 0;
		this.consoleOutputBox.SelectionColor = _color;
		this.consoleOutputBox.AppendText(_text + "\n");
		this.consoleOutputBox.SelectionColor = this.consoleOutputBox.ForeColor;
		if (this.consoleOutputBox.Lines.Length > 1000)
		{
			int num = this.consoleOutputBox.Lines.Length - 500;
			string[] array = new string[500];
			for (int i = 0; i < 500; i++)
			{
				array[i] = this.consoleOutputBox.Lines[i + num];
			}
			this.consoleOutputBox.Lines = array;
		}
		this.consoleOutputBox.SelectionStart = this.consoleOutputBox.TextLength;
		this.consoleOutputBox.ScrollToCaret();
	}

	// Token: 0x06001291 RID: 4753 RVA: 0x00073BCD File Offset: 0x00071DCD
	public void SendLine(string _line)
	{
		this.AddLine(_line, WinFormConnection.logColorCommandReply);
	}

	// Token: 0x06001292 RID: 4754 RVA: 0x00073BDC File Offset: 0x00071DDC
	public void SendLines(List<string> _output)
	{
		foreach (string line in _output)
		{
			this.SendLine(line);
		}
	}

	// Token: 0x06001293 RID: 4755 RVA: 0x00073C2C File Offset: 0x00071E2C
	public void SendLog(string _formattedMessage, string _plainMessage, string _trace, LogType _type, DateTime _timestamp, long _uptime)
	{
		if (this.IsLogLevelEnabled(_type))
		{
			System.Drawing.Color color = WinFormConnection.logColorNormal;
			switch (_type)
			{
			case LogType.Error:
			case LogType.Assert:
			case LogType.Exception:
				color = WinFormConnection.logColorError;
				break;
			case LogType.Warning:
				color = WinFormConnection.logColorWarning;
				break;
			}
			this.AddLine(_formattedMessage, color);
		}
	}

	// Token: 0x06001294 RID: 4756 RVA: 0x00073C7C File Offset: 0x00071E7C
	public void EnableLogLevel(LogType _type, bool _enable)
	{
		if (_enable)
		{
			this.enabledLogLevels.Add(_type);
			return;
		}
		this.enabledLogLevels.Remove(_type);
	}

	// Token: 0x06001295 RID: 4757 RVA: 0x00073C9C File Offset: 0x00071E9C
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsLogLevelEnabled(LogType _type)
	{
		return this.enabledLogLevels.Contains(_type);
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x00073CAA File Offset: 0x00071EAA
	public string GetDescription()
	{
		return "Terminal Window";
	}

	// Token: 0x04000C0B RID: 3083
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string iconPath = GameIO.GetGameDir("Data") + "/7dtd_icon.ico";

	// Token: 0x04000C0C RID: 3084
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly HashSet<LogType> enabledLogLevels = new HashSet<LogType>
	{
		LogType.Log,
		LogType.Warning,
		LogType.Error,
		LogType.Exception,
		LogType.Assert
	};

	// Token: 0x04000C0D RID: 3085
	[PublicizedFrom(EAccessModifier.Private)]
	public RichTextBox consoleOutputBox;

	// Token: 0x04000C0E RID: 3086
	[PublicizedFrom(EAccessModifier.Private)]
	public TextBox commandInputBox;

	// Token: 0x04000C0F RID: 3087
	[PublicizedFrom(EAccessModifier.Private)]
	public bool forceClose;

	// Token: 0x04000C10 RID: 3088
	[PublicizedFrom(EAccessModifier.Private)]
	public bool shutdownRequested;

	// Token: 0x04000C11 RID: 3089
	[PublicizedFrom(EAccessModifier.Private)]
	public const int lineLimit = 500;

	// Token: 0x04000C12 RID: 3090
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly System.Drawing.Color logColorCommandReply = System.Drawing.Color.LightCyan;

	// Token: 0x04000C13 RID: 3091
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly System.Drawing.Color logColorNormal = System.Drawing.Color.LimeGreen;

	// Token: 0x04000C14 RID: 3092
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly System.Drawing.Color logColorWarning = System.Drawing.Color.Yellow;

	// Token: 0x04000C15 RID: 3093
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly System.Drawing.Color logColorError = System.Drawing.Color.Red;

	// Token: 0x04000C16 RID: 3094
	[PublicizedFrom(EAccessModifier.Private)]
	public Action<string, System.Drawing.Color> addLineDelegate;
}
