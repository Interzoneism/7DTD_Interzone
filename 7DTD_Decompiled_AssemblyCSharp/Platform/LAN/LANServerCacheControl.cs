using System;
using System.Collections.Generic;

namespace Platform.LAN
{
	// Token: 0x020018F5 RID: 6389
	public class LANServerCacheControl
	{
		// Token: 0x0600BCD0 RID: 48336 RVA: 0x00478A81 File Offset: 0x00476C81
		public LANServerCacheControl(TimeSpan updateInterval, TimeSpan timeout)
		{
			this.timeout = timeout;
			this.updateInterval = updateInterval;
		}

		// Token: 0x0600BCD1 RID: 48337 RVA: 0x00478AA4 File Offset: 0x00476CA4
		public bool IsUpdateRequired(string addressString, int port)
		{
			LANServerCacheControl.ServerKey key = new LANServerCacheControl.ServerKey(addressString, port);
			LANServerCacheControl.UpdateTimes updateTimes;
			if (!this.lastServerUpdateTimes.TryGetValue(key, out updateTimes))
			{
				this.lastServerUpdateTimes[key] = new LANServerCacheControl.UpdateTimes
				{
					lastServerCheckedTime = DateTime.Now
				};
				return true;
			}
			TimeSpan t = DateTime.Now - updateTimes.lastServerCheckedTime;
			updateTimes.lastServerCheckedTime = DateTime.Now;
			this.lastServerUpdateTimes[key] = updateTimes;
			if (t > this.timeout)
			{
				Log.Out(string.Format("[{0}] server timed out, update needed. Last checked {1:F2} seconds ago", "LANServerCacheControl", t.TotalSeconds));
				return true;
			}
			TimeSpan t2 = DateTime.Now - updateTimes.lastRulesUpdateTime;
			if (t2 > this.updateInterval)
			{
				Log.Out(string.Format("[{0}] found known server, last updated {1:F2} seconds ago", "LANServerCacheControl", t2.TotalSeconds));
				return true;
			}
			return false;
		}

		// Token: 0x0600BCD2 RID: 48338 RVA: 0x00478B90 File Offset: 0x00476D90
		public void SetUpdated(string addressString, int port)
		{
			LANServerCacheControl.ServerKey key = new LANServerCacheControl.ServerKey(addressString, port);
			LANServerCacheControl.UpdateTimes value;
			if (!this.lastServerUpdateTimes.TryGetValue(key, out value))
			{
				this.lastServerUpdateTimes[key] = new LANServerCacheControl.UpdateTimes
				{
					lastServerCheckedTime = DateTime.Now,
					lastRulesUpdateTime = DateTime.Now
				};
				return;
			}
			value.lastRulesUpdateTime = DateTime.Now;
			this.lastServerUpdateTimes[key] = value;
		}

		// Token: 0x0600BCD3 RID: 48339 RVA: 0x00478BFD File Offset: 0x00476DFD
		public void Clear()
		{
			this.lastServerUpdateTimes.Clear();
		}

		// Token: 0x04009310 RID: 37648
		[PublicizedFrom(EAccessModifier.Private)]
		public TimeSpan timeout;

		// Token: 0x04009311 RID: 37649
		[PublicizedFrom(EAccessModifier.Private)]
		public TimeSpan updateInterval;

		// Token: 0x04009312 RID: 37650
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<LANServerCacheControl.ServerKey, LANServerCacheControl.UpdateTimes> lastServerUpdateTimes = new Dictionary<LANServerCacheControl.ServerKey, LANServerCacheControl.UpdateTimes>();

		// Token: 0x020018F6 RID: 6390
		[PublicizedFrom(EAccessModifier.Private)]
		public struct ServerKey : IEquatable<LANServerCacheControl.ServerKey>
		{
			// Token: 0x0600BCD4 RID: 48340 RVA: 0x00478C0A File Offset: 0x00476E0A
			public ServerKey(string _ipAddress, int _port)
			{
				this.ipAddress = _ipAddress;
				this.port = _port;
			}

			// Token: 0x0600BCD5 RID: 48341 RVA: 0x00478C1A File Offset: 0x00476E1A
			public override int GetHashCode()
			{
				return this.ipAddress.GetHashCode() ^ this.port;
			}

			// Token: 0x0600BCD6 RID: 48342 RVA: 0x00478C2E File Offset: 0x00476E2E
			public bool Equals(LANServerCacheControl.ServerKey _other)
			{
				return this.ipAddress.Equals(_other.ipAddress) && this.port == _other.port;
			}

			// Token: 0x04009313 RID: 37651
			public readonly string ipAddress;

			// Token: 0x04009314 RID: 37652
			public readonly int port;
		}

		// Token: 0x020018F7 RID: 6391
		[PublicizedFrom(EAccessModifier.Private)]
		public struct UpdateTimes
		{
			// Token: 0x04009315 RID: 37653
			public DateTime lastServerCheckedTime;

			// Token: 0x04009316 RID: 37654
			public DateTime lastRulesUpdateTime;
		}
	}
}
