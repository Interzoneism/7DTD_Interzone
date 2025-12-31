using System;
using System.Threading;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Profiling;

// Token: 0x0200028C RID: 652
public class WinFormInstance : IConsoleServer
{
	// Token: 0x06001281 RID: 4737 RVA: 0x000734E8 File Offset: 0x000716E8
	public WinFormInstance()
	{
		try
		{
			this.windowThread = new Thread(new ThreadStart(this.windowThreadMain))
			{
				Name = "WinFormInstance"
			};
			this.windowThread.SetApartmentState(ApartmentState.STA);
			this.windowThread.Start();
			Thread.Sleep(250);
			Log.Out("Started Terminal Window");
		}
		catch (Exception ex)
		{
			string str = "Error in WinFormInstance.ctor: ";
			Exception ex2 = ex;
			Log.Out(str + ((ex2 != null) ? ex2.ToString() : null));
		}
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x0007357C File Offset: 0x0007177C
	[PublicizedFrom(EAccessModifier.Private)]
	public void windowThreadMain()
	{
		this.form = new WinFormConnection(this);
		Log.Out("WinThread started");
		System.Windows.Forms.Application.ThreadException += this.ApplicationOnThreadException;
		System.Windows.Forms.Application.Run(this.form);
		Profiler.EndThreadProfiling();
		this.form = null;
		Log.Out("WinThread ended");
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x000735D1 File Offset: 0x000717D1
	[PublicizedFrom(EAccessModifier.Private)]
	public void ApplicationOnThreadException(object _sender, ThreadExceptionEventArgs _threadExceptionEventArgs)
	{
		Log.Error("TerminalWindow Exeption:");
		Log.Exception(_threadExceptionEventArgs.Exception);
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x000735E8 File Offset: 0x000717E8
	public void Disconnect()
	{
		if (this.form == null)
		{
			return;
		}
		Log.Out("Closing Terminal Window");
		WinFormConnection winFormConnection = this.form;
		this.form = null;
		winFormConnection.CloseTerminal();
		this.windowThread.Join();
		Log.Out("Ended Terminal Window");
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x00073624 File Offset: 0x00071824
	public void SendLine(string _line)
	{
		if (_line != null && this.form != null)
		{
			this.form.SendLine(_line);
		}
	}

	// Token: 0x06001286 RID: 4742 RVA: 0x0007363D File Offset: 0x0007183D
	public void SendLog(string _formattedMessage, string _plainMessage, string _trace, LogType _type, DateTime _timestamp, long _uptime)
	{
		if (_formattedMessage != null && this.form != null)
		{
			this.form.SendLog(_formattedMessage, _plainMessage, _trace, _type, _timestamp, _uptime);
		}
	}

	// Token: 0x04000C08 RID: 3080
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Thread windowThread;

	// Token: 0x04000C09 RID: 3081
	[PublicizedFrom(EAccessModifier.Private)]
	public bool stopThread;

	// Token: 0x04000C0A RID: 3082
	[PublicizedFrom(EAccessModifier.Private)]
	public WinFormConnection form;
}
