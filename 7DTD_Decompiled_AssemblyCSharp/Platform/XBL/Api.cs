using System;
using Unity.XGamingRuntime;

namespace Platform.XBL
{
	// Token: 0x02001898 RID: 6296
	public class Api : IPlatformApi
	{
		// Token: 0x0600B9DF RID: 47583 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x17001527 RID: 5415
		// (get) Token: 0x0600B9E0 RID: 47584 RVA: 0x0046F52E File Offset: 0x0046D72E
		// (set) Token: 0x0600B9E1 RID: 47585 RVA: 0x0046F536 File Offset: 0x0046D736
		public EApiStatus ClientApiStatus { get; [PublicizedFrom(EAccessModifier.Private)] set; } = EApiStatus.Uninitialized;

		// Token: 0x1400011C RID: 284
		// (add) Token: 0x0600B9E2 RID: 47586 RVA: 0x0046F540 File Offset: 0x0046D740
		// (remove) Token: 0x0600B9E3 RID: 47587 RVA: 0x0046F59C File Offset: 0x0046D79C
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

		// Token: 0x0600B9E4 RID: 47588 RVA: 0x0046F5E8 File Offset: 0x0046D7E8
		public bool InitClientApis()
		{
			if (this.ClientApiStatus == EApiStatus.Ok)
			{
				return true;
			}
			if (!XblHelpers.Succeeded(SDK.XGameRuntimeInitialize(), "Initialize gaming runtime", true, false))
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				Log.Error("[XBL] Failed to initialize GDK");
				return false;
			}
			if (!XblHelpers.Succeeded(SDK.CreateDefaultTaskQueue(), "Create default task queue", true, false))
			{
				Log.Error("[XBL] Failed to create task queue");
				this.ClientApiStatus = EApiStatus.PermanentError;
				return false;
			}
			if (!XblHelpers.Succeeded(SDK.XBL.XblInitialize("00000000-0000-0000-0000-0000680ee616"), "Initialize Xbox Live", true, false))
			{
				this.ClientApiStatus = EApiStatus.PermanentError;
				Log.Error("[XBL] Failed to initialize Xbox Live");
				return false;
			}
			Log.Out("[XBL] API loaded");
			this.ClientApiStatus = EApiStatus.Ok;
			Action action = this.clientApiInitialized;
			if (action != null)
			{
				action();
			}
			return true;
		}

		// Token: 0x0600B9E5 RID: 47589 RVA: 0x000197A5 File Offset: 0x000179A5
		public bool InitServerApis()
		{
			return true;
		}

		// Token: 0x0600B9E6 RID: 47590 RVA: 0x00002914 File Offset: 0x00000B14
		public void ServerApiLoaded()
		{
		}

		// Token: 0x0600B9E7 RID: 47591 RVA: 0x0046F699 File Offset: 0x0046D899
		public void Update()
		{
			if (this.ClientApiStatus != EApiStatus.Ok)
			{
				return;
			}
			SDK.XTaskQueueDispatch(0U);
		}

		// Token: 0x0600B9E8 RID: 47592 RVA: 0x0046F6AB File Offset: 0x0046D8AB
		public void Destroy()
		{
			SDK.CloseDefaultXTaskQueue();
			SDK.XBL.XblCleanup(null);
			SDK.XGameRuntimeUninitialize();
			this.ClientApiStatus = EApiStatus.Uninitialized;
		}

		// Token: 0x0600B9E9 RID: 47593 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
		public float GetScreenBoundsValueFromSystem()
		{
			return 1f;
		}

		// Token: 0x040091BD RID: 37309
		public const string SCID = "00000000-0000-0000-0000-0000680ee616";

		// Token: 0x040091BE RID: 37310
		public const int TitleId = 1745806870;

		// Token: 0x040091C0 RID: 37312
		[PublicizedFrom(EAccessModifier.Private)]
		public Action clientApiInitialized;
	}
}
