using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Unity.XGamingRuntime;

namespace Platform.XBL
{
	// Token: 0x0200189E RID: 6302
	[PublicizedFrom(EAccessModifier.Internal)]
	public class JoinSessionGameInviteListener : IJoinSessionGameInviteListener
	{
		// Token: 0x0600BA00 RID: 47616 RVA: 0x0046FDC8 File Offset: 0x0046DFC8
		public void Init(IPlatform _owner)
		{
			PlatformManager.NativePlatform.User.UserLoggedIn += delegate(IPlatform _platform)
			{
				XGameInviteRegistrationToken xgameInviteRegistrationToken;
				XblHelpers.Succeeded(SDK.XGameInviteRegisterForEvent(new XGameInviteEventCallback(this.InviteReceivedCallback), out xgameInviteRegistrationToken), "Register for invite event", true, true);
			};
		}

		// Token: 0x0600BA01 RID: 47617 RVA: 0x0046FDE8 File Offset: 0x0046DFE8
		[return: TupleElementNames(new string[]
		{
			"invite",
			"password"
		})]
		public ValueTuple<string, string> TakePendingInvite()
		{
			string item = this.pendingInvite;
			this.pendingInvite = null;
			string item2 = this.pendingPassword;
			this.pendingPassword = null;
			return new ValueTuple<string, string>(item, item2);
		}

		// Token: 0x0600BA02 RID: 47618 RVA: 0x0046FE16 File Offset: 0x0046E016
		public IEnumerator ConnectToInvite(string _invite, string _password = null, Action<bool> _onFinished = null)
		{
			yield return InviteManager.HandleSessionIdInvite(_invite, _password, _onFinished);
			yield break;
		}

		// Token: 0x0600BA03 RID: 47619 RVA: 0x0046FE33 File Offset: 0x0046E033
		public string GetListenerIdentifier()
		{
			return "XBL";
		}

		// Token: 0x0600BA04 RID: 47620 RVA: 0x0046FE3C File Offset: 0x0046E03C
		[PublicizedFrom(EAccessModifier.Private)]
		public void InviteReceivedCallback(IntPtr _, string _inviteuri)
		{
			Log.Out("[XBL] Invite received: '" + _inviteuri + "'");
			string text = this.ParseInviteUri(_inviteuri);
			if (text == null)
			{
				Log.Error("[XBL] Received invite but could not extract connect information");
				return;
			}
			this.pendingInvite = text;
		}

		// Token: 0x0600BA05 RID: 47621 RVA: 0x0046FE7C File Offset: 0x0046E07C
		[PublicizedFrom(EAccessModifier.Private)]
		public string ParseInviteUri(string _inviteUri)
		{
			Match match = JoinSessionGameInviteListener.msInviteUriMatcher.Match(_inviteUri);
			if (!match.Success)
			{
				return null;
			}
			string[] array = match.Groups[3].Value.Split('&', StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split('=', StringSplitOptions.None);
				if (array2[0].EqualsCaseInsensitive("connectionString"))
				{
					return array2[1];
				}
			}
			return null;
		}

		// Token: 0x040091D0 RID: 37328
		[PublicizedFrom(EAccessModifier.Private)]
		public string pendingInvite;

		// Token: 0x040091D1 RID: 37329
		[PublicizedFrom(EAccessModifier.Private)]
		public string pendingPassword;

		// Token: 0x040091D2 RID: 37330
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Regex msInviteUriMatcher = new Regex("^ms-xbl-(\\w+):\\/\\/(\\w+)\\/?\\?(.*)$", RegexOptions.Compiled);
	}
}
