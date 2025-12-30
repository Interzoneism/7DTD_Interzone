using System;
using System.IO;
using Epic.OnlineServices;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Platform;
using Epic.OnlineServices.Sanctions;
using UnityEngine;

namespace Platform.EOS
{
	// Token: 0x02001909 RID: 6409
	public class Api : IPlatformApi
	{
		// Token: 0x0600BD5C RID: 48476 RVA: 0x0047BAE4 File Offset: 0x00479CE4
		[PublicizedFrom(EAccessModifier.Private)]
		static Api()
		{
			string launchArgument = GameUtils.GetLaunchArgument("debugeos");
			if (launchArgument != null)
			{
				if (launchArgument == "verbose")
				{
					Api.DebugLevel = Api.EDebugLevel.Verbose;
					return;
				}
				Api.DebugLevel = Api.EDebugLevel.Normal;
			}
		}

		// Token: 0x0600BD5D RID: 48477 RVA: 0x0047BB1F File Offset: 0x00479D1F
		public void Init(IPlatform _owner)
		{
			this.owner = _owner;
		}

		// Token: 0x0600BD5E RID: 48478 RVA: 0x0047BB28 File Offset: 0x00479D28
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnApplicationStateChanged(ApplicationState _applicationState)
		{
			if (this.PlatformInterface == null)
			{
				return;
			}
			ApplicationStatus applicationStatus;
			if (_applicationState != ApplicationState.Foreground)
			{
				if (_applicationState != ApplicationState.Suspended)
				{
					throw new ArgumentOutOfRangeException("_applicationState", _applicationState, "[EOS] OnApplicationStateChanged: ApplicationState is missing a conversion to a EOS.ApplicationStatus");
				}
				applicationStatus = ApplicationStatus.BackgroundSuspended;
			}
			else
			{
				applicationStatus = ApplicationStatus.Foreground;
			}
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.PlatformInterface.SetApplicationStatus(applicationStatus);
			}
		}

		// Token: 0x170015B7 RID: 5559
		// (get) Token: 0x0600BD5F RID: 48479 RVA: 0x0047BBA4 File Offset: 0x00479DA4
		// (set) Token: 0x0600BD60 RID: 48480 RVA: 0x0047BBAC File Offset: 0x00479DAC
		public EApiStatus ClientApiStatus { get; [PublicizedFrom(EAccessModifier.Private)] set; } = EApiStatus.Uninitialized;

		// Token: 0x14000129 RID: 297
		// (add) Token: 0x0600BD61 RID: 48481 RVA: 0x0047BBB8 File Offset: 0x00479DB8
		// (remove) Token: 0x0600BD62 RID: 48482 RVA: 0x0047BC14 File Offset: 0x00479E14
		public event Action ClientApiInitialized
		{
			add
			{
				lock (this)
				{
					this.clientApiInitialized = (Action)Delegate.Combine(this.clientApiInitialized, value);
					if (this.ClientApiStatus == EApiStatus.Ok)
					{
						value();
					}
				}
			}
			remove
			{
				lock (this)
				{
					this.clientApiInitialized = (Action)Delegate.Remove(this.clientApiInitialized, value);
				}
			}
		}

		// Token: 0x0600BD63 RID: 48483 RVA: 0x0047BC60 File Offset: 0x00479E60
		public bool InitClientApis()
		{
			if (this.ClientApiStatus == EApiStatus.Ok)
			{
				return true;
			}
			EosCreds eosCreds = GameManager.IsDedicatedServer ? EosCreds.ServerCredentials : EosCreds.ClientCredentials;
			this.initPlatform(eosCreds, eosCreds.ServerMode);
			return this.ClientApiStatus == EApiStatus.Ok;
		}

		// Token: 0x0600BD64 RID: 48484 RVA: 0x0047BCA1 File Offset: 0x00479EA1
		public bool InitServerApis()
		{
			return this.InitClientApis();
		}

		// Token: 0x0600BD65 RID: 48485 RVA: 0x00002914 File Offset: 0x00000B14
		public void ServerApiLoaded()
		{
		}

		// Token: 0x0600BD66 RID: 48486 RVA: 0x0047BCAC File Offset: 0x00479EAC
		public void Update()
		{
			if (this.ClientApiStatus != EApiStatus.Ok)
			{
				return;
			}
			this.platformTickTimer += Time.unscaledDeltaTime;
			if (GameManager.IsDedicatedServer || this.platformTickTimer >= 0.04f)
			{
				this.platformTickTimer = 0f;
				this.tickDurationStopwatch.Restart();
				object lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					this.PlatformInterface.Tick();
				}
				long num = this.tickDurationStopwatch.ElapsedMicroseconds / 1000L;
				if (Api.DebugLevel != Api.EDebugLevel.Off && num > 20L)
				{
					Log.Warning(string.Format("[EOS] Tick took exceptionally long: {0} ms", num));
				}
			}
		}

		// Token: 0x0600BD67 RID: 48487 RVA: 0x0047BD6C File Offset: 0x00479F6C
		public void Destroy()
		{
			if (this.ClientApiStatus != EApiStatus.Ok)
			{
				return;
			}
			this.ConnectInterface = null;
			object lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.PlatformInterface.Release();
			}
			this.PlatformInterface = null;
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				PlatformInterface.Shutdown();
			}
		}

		// Token: 0x0600BD68 RID: 48488 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
		public float GetScreenBoundsValueFromSystem()
		{
			return 1f;
		}

		// Token: 0x0600BD69 RID: 48489 RVA: 0x0047BDF8 File Offset: 0x00479FF8
		[PublicizedFrom(EAccessModifier.Private)]
		public void initPlatform(EosCreds _creds, bool _serverMode)
		{
			InitializeOptions initializeOptions = new InitializeOptions
			{
				ProductName = "7 Days To Die",
				ProductVersion = Constants.cVersionInformation.SerializableString
			};
			Result result = Result.NotFound;
			object lockObject;
			try
			{
				lockObject = AntiCheatCommon.LockObject;
				lock (lockObject)
				{
					result = PlatformInterface.Initialize(ref initializeOptions);
				}
			}
			catch (DllNotFoundException e)
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				Log.Error("[EOS] Native library or one of its dependencies not found (e.g. no Microsoft Visual C Redistributables 2022)");
				Log.Exception(e);
				Application.Quit(1);
			}
			Log.Out(string.Format("[EOS] Initialize: {0}", result));
			LogLevel logLevel;
			switch (Api.DebugLevel)
			{
			case Api.EDebugLevel.Off:
				logLevel = LogLevel.Warning;
				break;
			case Api.EDebugLevel.Normal:
				logLevel = LogLevel.Info;
				break;
			case Api.EDebugLevel.Verbose:
				logLevel = LogLevel.VeryVerbose;
				break;
			default:
				throw new ArgumentOutOfRangeException("DebugLevel");
			}
			LogLevel logLevel2 = logLevel;
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				LoggingInterface.SetLogLevel(LogCategory.AllCategories, logLevel2);
				string launchArgument = GameUtils.GetLaunchArgument("debugeac");
				if (launchArgument != null)
				{
					LoggingInterface.SetLogLevel(LogCategory.AntiCheat, (launchArgument == "verbose") ? LogLevel.Verbose : LogLevel.Info);
				}
				else
				{
					LoggingInterface.SetLogLevel(LogCategory.AntiCheat, LogLevel.Warning);
				}
				if (logLevel2 == LogLevel.VeryVerbose)
				{
					LoggingInterface.SetLogLevel(LogCategory.Http, LogLevel.Verbose);
				}
				LoggingInterface.SetLogLevel(LogCategory.Analytics, LogLevel.Error);
				LoggingInterface.SetLogLevel(LogCategory.Messaging, LogLevel.Warning);
				LoggingInterface.SetLogLevel(LogCategory.Ecom, LogLevel.Error);
				LoggingInterface.SetLogLevel(LogCategory.Auth, LogLevel.Error);
				LoggingInterface.SetLogLevel(LogCategory.Presence, LogLevel.Warning);
				LoggingInterface.SetLogLevel(LogCategory.Overlay, LogLevel.Warning);
				LoggingInterface.SetLogLevel(LogCategory.Ui, LogLevel.Warning);
				LoggingInterface.SetCallback(new LogMessageFunc(this.logCallback));
			}
			this.PlatformInterface = this.createPlatformInterface(_creds, _serverMode);
			if (this.PlatformInterface == null)
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				Log.Error("[EOS] Failed to create platform");
				return;
			}
			IPlatform nativePlatform = PlatformManager.NativePlatform;
			if (((nativePlatform != null) ? nativePlatform.ApplicationState : null) != null)
			{
				PlatformManager.NativePlatform.ApplicationState.OnApplicationStateChanged += this.OnApplicationStateChanged;
			}
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.ConnectInterface = this.PlatformInterface.GetConnectInterface();
			}
			if (this.ConnectInterface == null)
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				Log.Error("[EOS] Failed to get connect interface");
				return;
			}
			lockObject = AntiCheatCommon.LockObject;
			lock (lockObject)
			{
				this.SanctionsInterface = this.PlatformInterface.GetSanctionsInterface();
			}
			if (this.SanctionsInterface == null)
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				Log.Error("[EOS] Failed to get sanctions interface");
				return;
			}
			this.ClientApiStatus = EApiStatus.Ok;
			Action action = this.clientApiInitialized;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x0600BD6A RID: 48490 RVA: 0x0047C120 File Offset: 0x0047A320
		[PublicizedFrom(EAccessModifier.Private)]
		public PlatformInterface createPlatformInterface(EosCreds _creds, bool _serverMode)
		{
			WindowsOptions windowsOptions = default(WindowsOptions);
			windowsOptions.ProductId = _creds.ProductId;
			windowsOptions.SandboxId = _creds.SandboxId;
			windowsOptions.ClientCredentials = new ClientCredentials
			{
				ClientId = _creds.ClientId,
				ClientSecret = _creds.ClientSecret
			};
			windowsOptions.DeploymentId = _creds.DeploymentId;
			windowsOptions.EncryptionKey = "0000000000000000000000000000000000000000000000000000000000000000";
			windowsOptions.IsServer = _serverMode;
			windowsOptions.Flags = PlatformFlags.DisableOverlay;
			windowsOptions.Flags |= PlatformFlags.DisableSocialOverlay;
			windowsOptions.RTCOptions = null;
			windowsOptions.RTCOptions = new WindowsRTCOptions?(new WindowsRTCOptions
			{
				PlatformSpecificOptions = new WindowsRTCOptionsPlatformSpecificOptions?(new WindowsRTCOptionsPlatformSpecificOptions
				{
					XAudio29DllPath = GameIO.GetGameDir("7DaysToDie_Data/Plugins/x86_64/xaudio2_9redist.dll")
				})
			});
			windowsOptions.CacheDirectory = GameIO.GetUserGameDataDir();
			if (!Directory.Exists(windowsOptions.CacheDirectory))
			{
				Directory.CreateDirectory(windowsOptions.CacheDirectory);
			}
			object lockObject = AntiCheatCommon.LockObject;
			PlatformInterface result;
			lock (lockObject)
			{
				result = PlatformInterface.Create(ref windowsOptions);
			}
			return result;
		}

		// Token: 0x0600BD6B RID: 48491 RVA: 0x0047C294 File Offset: 0x0047A494
		[PublicizedFrom(EAccessModifier.Private)]
		public void logCallback(ref LogMessage _message)
		{
			if (_message.Level == LogLevel.Warning && _message.Category == "LogHttp")
			{
				this.httpWarningCount++;
				if (this.httpWarningCount == 50)
				{
					this.httpNextTime = Time.unscaledTime + 600f;
					return;
				}
				if (this.httpWarningCount > 50)
				{
					if (Time.unscaledTime < this.httpNextTime)
					{
						return;
					}
					Log.Out(string.Format("[EOS] [LogHttp - Warning] Skipped {0} warnings within the last {1} seconds!", this.httpWarningCount - 50, 600f));
					this.httpWarningCount = 0;
				}
			}
			string txt = string.Format("[EOS] [{0} - {1}] {2}", _message.Category, _message.Level.ToStringCached<LogLevel>(), _message.Message);
			LogLevel level = _message.Level;
			if (level > LogLevel.Error)
			{
				if (level <= LogLevel.Info)
				{
					if (level == LogLevel.Warning)
					{
						Log.Warning(txt);
						return;
					}
					if (level != LogLevel.Info)
					{
						goto IL_129;
					}
				}
				else if (level != LogLevel.Verbose && level != LogLevel.VeryVerbose)
				{
					goto IL_129;
				}
				Log.Out(txt);
				return;
			}
			if (level == LogLevel.Off)
			{
				Log.Error(txt);
				throw new ArgumentOutOfRangeException();
			}
			if (level == LogLevel.Fatal || level == LogLevel.Error)
			{
				Log.Error(txt);
				return;
			}
			IL_129:
			throw new ArgumentOutOfRangeException();
		}

		// Token: 0x04009370 RID: 37744
		[PublicizedFrom(EAccessModifier.Private)]
		public const float platformTickInterval = 0.04f;

		// Token: 0x04009371 RID: 37745
		public static readonly Api.EDebugLevel DebugLevel = Api.EDebugLevel.Off;

		// Token: 0x04009372 RID: 37746
		[PublicizedFrom(EAccessModifier.Private)]
		public IPlatform owner;

		// Token: 0x04009373 RID: 37747
		public PlatformInterface PlatformInterface;

		// Token: 0x04009374 RID: 37748
		public ConnectInterface ConnectInterface;

		// Token: 0x04009375 RID: 37749
		public SanctionsInterface SanctionsInterface;

		// Token: 0x04009376 RID: 37750
		[PublicizedFrom(EAccessModifier.Private)]
		public float platformTickTimer;

		// Token: 0x04009377 RID: 37751
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MicroStopwatch tickDurationStopwatch = new MicroStopwatch(false);

		// Token: 0x04009378 RID: 37752
		[PublicizedFrom(EAccessModifier.Internal)]
		public readonly SanctionsCheck eosSanctionsCheck = new SanctionsCheck();

		// Token: 0x0400937A RID: 37754
		[PublicizedFrom(EAccessModifier.Private)]
		public Action clientApiInitialized;

		// Token: 0x0400937B RID: 37755
		[PublicizedFrom(EAccessModifier.Private)]
		public const int httpWarningLimit = 50;

		// Token: 0x0400937C RID: 37756
		[PublicizedFrom(EAccessModifier.Private)]
		public const float httpWarningTimeout = 600f;

		// Token: 0x0400937D RID: 37757
		[PublicizedFrom(EAccessModifier.Private)]
		public int httpWarningCount;

		// Token: 0x0400937E RID: 37758
		[PublicizedFrom(EAccessModifier.Private)]
		public float httpNextTime;

		// Token: 0x0200190A RID: 6410
		public enum EDebugLevel
		{
			// Token: 0x04009380 RID: 37760
			Off,
			// Token: 0x04009381 RID: 37761
			Normal,
			// Token: 0x04009382 RID: 37762
			Verbose
		}
	}
}
