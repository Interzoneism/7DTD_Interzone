using System;
using System.Globalization;
using UnityEngine.Scripting;

namespace Platform.PSN
{
	// Token: 0x020018DF RID: 6367
	[Preserve]
	[UserIdentifierFactory(EPlatformIdentifier.PSN)]
	public class UserIdentifierFactory : AbsUserIdentifierFactory
	{
		// Token: 0x0600BC0B RID: 48139 RVA: 0x00477438 File Offset: 0x00475638
		public override PlatformUserIdentifierAbs FromId(string _idString)
		{
			Log.Out("[PSN] Creating PSN user identifier from: {0}", new object[]
			{
				_idString
			});
			ulong accountId;
			if (StringParsers.TryParseUInt64(_idString, out accountId, 0, -1, NumberStyles.Integer))
			{
				return new UserIdentifierPSN(accountId);
			}
			Log.Warning("[PSN] Could not parse PSN user from " + _idString);
			return null;
		}
	}
}
