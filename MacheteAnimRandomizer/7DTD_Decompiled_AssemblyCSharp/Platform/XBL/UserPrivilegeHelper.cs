using System;
using System.Collections;
using System.Linq;
using Unity.XGamingRuntime;

namespace Platform.XBL
{
	// Token: 0x0200187D RID: 6269
	public class UserPrivilegeHelper
	{
		// Token: 0x0600B972 RID: 47474 RVA: 0x0046D7A4 File Offset: 0x0046B9A4
		public UserPrivilegeHelper(XUserHandle userHandle)
		{
			this.AllAllowed = new PrivilegeState[]
			{
				this.Multiplayer = new PrivilegeState(userHandle, XUserPrivilege.Multiplayer),
				this.Communications = new PrivilegeState(userHandle, XUserPrivilege.Communications),
				this.CrossPlay = new PrivilegeState(userHandle, XUserPrivilege.CrossPlay),
				this.UserGeneratedContent = new PrivilegeState(userHandle, XUserPrivilege.UserGeneratedContent)
			};
			this.MultiplayerAllowed = new PrivilegeState[]
			{
				this.Multiplayer,
				this.UserGeneratedContent
			};
			this.CommunicationAllowed = new PrivilegeState[]
			{
				this.Communications
			};
			this.CrossPlayAllowed = new PrivilegeState[]
			{
				this.CrossPlay
			};
		}

		// Token: 0x0600B973 RID: 47475 RVA: 0x0046D867 File Offset: 0x0046BA67
		[PublicizedFrom(EAccessModifier.Private)]
		public static IEnumerator ResolveAllowed(bool canPrompt, CoroutineCancellationToken _cancellationToken = null, params PrivilegeState[] privilegeStates)
		{
			if (!canPrompt)
			{
				privilegeStates.ResolveSilent();
				return Enumerable.Empty<object>().GetEnumerator();
			}
			return privilegeStates.ResolveWithPrompt(_cancellationToken);
		}

		// Token: 0x0600B974 RID: 47476 RVA: 0x0046D884 File Offset: 0x0046BA84
		public IEnumerator ResolvePermissions(EUserPerms _perms, bool _canPrompt, CoroutineCancellationToken _cancellationToken = null)
		{
			if (_perms.HasMultiplayer() || _perms.HasHostMultiplayer())
			{
				yield return UserPrivilegeHelper.ResolveAllowed(_canPrompt, _cancellationToken, this.MultiplayerAllowed);
				if (_cancellationToken != null && _cancellationToken.IsCancelled())
				{
					yield break;
				}
			}
			if (_perms.HasCommunication())
			{
				yield return UserPrivilegeHelper.ResolveAllowed(_canPrompt, _cancellationToken, this.CommunicationAllowed);
				if (_cancellationToken != null && _cancellationToken.IsCancelled())
				{
					yield break;
				}
			}
			if (_perms.HasCrossplay())
			{
				yield return UserPrivilegeHelper.ResolveAllowed(_canPrompt, _cancellationToken, this.CrossPlayAllowed);
				if (_cancellationToken != null)
				{
					_cancellationToken.IsCancelled();
				}
				yield break;
			}
			yield break;
		}

		// Token: 0x0400910E RID: 37134
		public readonly PrivilegeState Multiplayer;

		// Token: 0x0400910F RID: 37135
		public readonly PrivilegeState Communications;

		// Token: 0x04009110 RID: 37136
		public readonly PrivilegeState CrossPlay;

		// Token: 0x04009111 RID: 37137
		public readonly PrivilegeState UserGeneratedContent;

		// Token: 0x04009112 RID: 37138
		public readonly PrivilegeState[] AllAllowed;

		// Token: 0x04009113 RID: 37139
		public readonly PrivilegeState[] MultiplayerAllowed;

		// Token: 0x04009114 RID: 37140
		public readonly PrivilegeState[] CommunicationAllowed;

		// Token: 0x04009115 RID: 37141
		public readonly PrivilegeState[] CrossPlayAllowed;
	}
}
