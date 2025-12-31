using System;
using System.Collections;

namespace Platform.XBL
{
	// Token: 0x0200187F RID: 6271
	public static class PrivilegeStateArrayExtensions
	{
		// Token: 0x0600B97B RID: 47483 RVA: 0x0046D9F8 File Offset: 0x0046BBF8
		public static bool Has(this PrivilegeState[] privilegeStates)
		{
			for (int i = 0; i < privilegeStates.Length; i++)
			{
				if (!privilegeStates[i].Has)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600B97C RID: 47484 RVA: 0x0046DA24 File Offset: 0x0046BC24
		public static void ResolveSilent(this PrivilegeState[] privilegeStates)
		{
			for (int i = 0; i < privilegeStates.Length; i++)
			{
				privilegeStates[i].ResolveSilent();
			}
		}

		// Token: 0x0600B97D RID: 47485 RVA: 0x0046DA49 File Offset: 0x0046BC49
		public static IEnumerator ResolveWithPrompt(this PrivilegeState[] privilegeStates, CoroutineCancellationToken _cancellationToken)
		{
			foreach (PrivilegeState privilegeState in privilegeStates)
			{
				yield return privilegeState.ResolveWithPrompt(_cancellationToken);
				if (_cancellationToken != null && _cancellationToken.IsCancelled())
				{
					yield break;
				}
			}
			PrivilegeState[] array = null;
			yield break;
		}
	}
}
