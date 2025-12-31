using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000701 RID: 1793
[Preserve]
public class NetPackageAuthState : NetPackage
{
	// Token: 0x17000554 RID: 1364
	// (get) Token: 0x060034D2 RID: 13522 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool FlushQueue
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000555 RID: 1365
	// (get) Token: 0x060034D3 RID: 13523 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x17000556 RID: 1366
	// (get) Token: 0x060034D4 RID: 13524 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedBeforeAuth
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060034D5 RID: 13525 RVA: 0x00161C68 File Offset: 0x0015FE68
	public NetPackageAuthState Setup(string _authStateKey)
	{
		this.stateKey = (_authStateKey ?? "");
		return this;
	}

	// Token: 0x060034D6 RID: 13526 RVA: 0x00161C7B File Offset: 0x0015FE7B
	public override void read(PooledBinaryReader _reader)
	{
		this.stateKey = _reader.ReadString();
	}

	// Token: 0x060034D7 RID: 13527 RVA: 0x00161C89 File Offset: 0x0015FE89
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.stateKey);
	}

	// Token: 0x060034D8 RID: 13528 RVA: 0x00161CA0 File Offset: 0x0015FEA0
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsConnected)
		{
			Log.Out("Login: " + this.stateKey);
			if (!string.IsNullOrEmpty(this.stateKey))
			{
				string text = Localization.Get(this.stateKey, false);
				string format = text;
				object platformDisplayName = PlatformManager.NativePlatform.PlatformDisplayName;
				IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
				text = string.Format(format, platformDisplayName, (crossplatformPlatform != null) ? crossplatformPlatform.PlatformDisplayName : null);
				XUiC_ProgressWindow.SetText(LocalPlayerUI.primaryUI, text, true);
			}
		}
	}

	// Token: 0x060034D9 RID: 13529 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override int GetLength()
	{
		return 4;
	}

	// Token: 0x04002B20 RID: 11040
	[PublicizedFrom(EAccessModifier.Private)]
	public string stateKey;
}
