using System;
using UnityEngine.Scripting;

// Token: 0x020006E6 RID: 1766
[Preserve]
public class AuthFinalizer : AuthorizerAbs
{
	// Token: 0x0600341E RID: 13342 RVA: 0x0015E9DA File Offset: 0x0015CBDA
	public AuthFinalizer()
	{
		AuthFinalizer.Instance = this;
	}

	// Token: 0x1700053B RID: 1339
	// (get) Token: 0x0600341F RID: 13343 RVA: 0x0015E9E8 File Offset: 0x0015CBE8
	public override int Order
	{
		get
		{
			return 999;
		}
	}

	// Token: 0x1700053C RID: 1340
	// (get) Token: 0x06003420 RID: 13344 RVA: 0x0015E9EF File Offset: 0x0015CBEF
	public override string AuthorizerName
	{
		get
		{
			return "Finalizer";
		}
	}

	// Token: 0x1700053D RID: 1341
	// (get) Token: 0x06003421 RID: 13345 RVA: 0x00019766 File Offset: 0x00017966
	public override string StateLocalizationKey
	{
		get
		{
			return null;
		}
	}

	// Token: 0x06003422 RID: 13346 RVA: 0x0015E9F8 File Offset: 0x0015CBF8
	public override ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?> Authorize(ClientInfo _clientInfo)
	{
		_clientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageAuthConfirmation>().Setup());
		return new ValueTuple<EAuthorizerSyncResult, GameUtils.KickPlayerData?>(EAuthorizerSyncResult.WaitAsync, null);
	}

	// Token: 0x06003423 RID: 13347 RVA: 0x0015E2C8 File Offset: 0x0015C4C8
	public void ReplyReceived(ClientInfo _cInfo)
	{
		this.authResponsesHandler.AuthorizationAccepted(this, _cInfo);
	}

	// Token: 0x04002AB1 RID: 10929
	public static AuthFinalizer Instance;
}
