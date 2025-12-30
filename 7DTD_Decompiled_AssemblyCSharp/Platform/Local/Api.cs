using System;

namespace Platform.Local
{
	// Token: 0x020018EB RID: 6379
	public class Api : IPlatformApi
	{
		// Token: 0x170015A2 RID: 5538
		// (get) Token: 0x0600BC84 RID: 48260 RVA: 0x004784B0 File Offset: 0x004766B0
		public EApiStatus ClientApiStatus { get; }

		// Token: 0x14000125 RID: 293
		// (add) Token: 0x0600BC85 RID: 48261 RVA: 0x004784B8 File Offset: 0x004766B8
		// (remove) Token: 0x0600BC86 RID: 48262 RVA: 0x00002914 File Offset: 0x00000B14
		public event Action ClientApiInitialized
		{
			add
			{
				lock (this)
				{
					value();
				}
			}
			remove
			{
			}
		}

		// Token: 0x0600BC87 RID: 48263 RVA: 0x00002914 File Offset: 0x00000B14
		public void Init(IPlatform _owner)
		{
		}

		// Token: 0x0600BC88 RID: 48264 RVA: 0x000197A5 File Offset: 0x000179A5
		public bool InitClientApis()
		{
			return true;
		}

		// Token: 0x0600BC89 RID: 48265 RVA: 0x000197A5 File Offset: 0x000179A5
		public bool InitServerApis()
		{
			return true;
		}

		// Token: 0x0600BC8A RID: 48266 RVA: 0x00002914 File Offset: 0x00000B14
		public void ServerApiLoaded()
		{
		}

		// Token: 0x0600BC8B RID: 48267 RVA: 0x00002914 File Offset: 0x00000B14
		public void Update()
		{
		}

		// Token: 0x0600BC8C RID: 48268 RVA: 0x00002914 File Offset: 0x00000B14
		public void Destroy()
		{
		}

		// Token: 0x0600BC8D RID: 48269 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
		public float GetScreenBoundsValueFromSystem()
		{
			return 1f;
		}
	}
}
