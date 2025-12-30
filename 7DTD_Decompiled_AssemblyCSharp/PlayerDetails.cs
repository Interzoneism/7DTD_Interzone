using System;
using GameSparks.Core;
using UnityEngine;

// Token: 0x02000F99 RID: 3993
[Serializable]
public class PlayerDetails
{
	// Token: 0x06007F3B RID: 32571 RVA: 0x0033A81A File Offset: 0x00338A1A
	public PlayerDetails(string _displayName, string _userId, GSData _responseData)
	{
		this.displayName = _displayName;
		this.userId = _userId;
	}

	// Token: 0x04006219 RID: 25113
	public string displayName;

	// Token: 0x0400621A RID: 25114
	[HideInInspector]
	public string userEmail;

	// Token: 0x0400621B RID: 25115
	public string userId;
}
