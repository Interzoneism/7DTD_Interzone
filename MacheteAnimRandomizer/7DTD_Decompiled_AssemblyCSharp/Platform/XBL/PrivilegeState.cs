using System;
using System.Collections;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL
{
	// Token: 0x02001881 RID: 6273
	public class PrivilegeState
	{
		// Token: 0x0600B984 RID: 47492 RVA: 0x0046DB1D File Offset: 0x0046BD1D
		public PrivilegeState(XUserHandle userHandle, XUserPrivilege privilege)
		{
			this.m_userHandle = userHandle;
			this.m_privilege = privilege;
			this.m_has = false;
			this.m_denyReason = (XUserPrivilegeDenyReason)4294967295U;
		}

		// Token: 0x17001520 RID: 5408
		// (get) Token: 0x0600B985 RID: 47493 RVA: 0x0046DB41 File Offset: 0x0046BD41
		public bool Has
		{
			get
			{
				return this.m_has;
			}
		}

		// Token: 0x17001521 RID: 5409
		// (get) Token: 0x0600B986 RID: 47494 RVA: 0x0046DB49 File Offset: 0x0046BD49
		public XUserPrivilegeDenyReason DenyReason
		{
			get
			{
				return this.m_denyReason;
			}
		}

		// Token: 0x0600B987 RID: 47495 RVA: 0x0046DB54 File Offset: 0x0046BD54
		public void ResolveSilent()
		{
			int hr = SDK.XUserCheckPrivilege(this.m_userHandle, XUserPrivilegeOptions.None, this.m_privilege, out this.m_has, out this.m_denyReason);
			XblHelpers.LogHR(hr, string.Format("{0} checked privilege '{1}' = {2} ({3})", new object[]
			{
				"XUserCheckPrivilege",
				this.m_privilege,
				this.m_has,
				this.m_denyReason
			}), false);
			if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr))
			{
				return;
			}
			this.m_has = false;
			this.m_denyReason = (XUserPrivilegeDenyReason)4294967295U;
		}

		// Token: 0x0600B988 RID: 47496 RVA: 0x0046DBE0 File Offset: 0x0046BDE0
		public IEnumerator ResolveWithPrompt(CoroutineCancellationToken _cancellationToken = null)
		{
			this.ResolveSilent();
			if (this.m_has)
			{
				yield break;
			}
			bool uiOpen = true;
			SDK.XUserResolvePrivilegeWithUiAsync(this.m_userHandle, XUserPrivilegeOptions.None, this.m_privilege, delegate(int hr)
			{
				CoroutineCancellationToken cancellationToken2 = _cancellationToken;
				if (cancellationToken2 != null && cancellationToken2.IsCancelled())
				{
					return;
				}
				try
				{
					XblHelpers.LogHR(hr, "XUserResolvePrivilegeWithUiCompleted", false);
				}
				finally
				{
					uiOpen = false;
				}
			});
			while (uiOpen)
			{
				CoroutineCancellationToken cancellationToken = _cancellationToken;
				if (cancellationToken != null && cancellationToken.IsCancelled())
				{
					yield break;
				}
				yield return null;
			}
			this.ResolveSilent();
			yield break;
		}

		// Token: 0x04009122 RID: 37154
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XUserHandle m_userHandle;

		// Token: 0x04009123 RID: 37155
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly XUserPrivilege m_privilege;

		// Token: 0x04009124 RID: 37156
		[PublicizedFrom(EAccessModifier.Private)]
		public bool m_has;

		// Token: 0x04009125 RID: 37157
		[PublicizedFrom(EAccessModifier.Private)]
		public XUserPrivilegeDenyReason m_denyReason;
	}
}
