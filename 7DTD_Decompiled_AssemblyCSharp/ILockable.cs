using System;
using System.Collections.Generic;

// Token: 0x02000B0A RID: 2826
public interface ILockable
{
	// Token: 0x170008C4 RID: 2244
	// (get) Token: 0x06005791 RID: 22417
	// (set) Token: 0x06005792 RID: 22418
	int EntityId { get; set; }

	// Token: 0x06005793 RID: 22419
	bool IsLocked();

	// Token: 0x06005794 RID: 22420
	void SetLocked(bool _isLocked);

	// Token: 0x06005795 RID: 22421
	PlatformUserIdentifierAbs GetOwner();

	// Token: 0x06005796 RID: 22422
	void SetOwner(PlatformUserIdentifierAbs _userIdentifier);

	// Token: 0x06005797 RID: 22423
	bool IsUserAllowed(PlatformUserIdentifierAbs _userIdentifier);

	// Token: 0x06005798 RID: 22424
	List<PlatformUserIdentifierAbs> GetUsers();

	// Token: 0x06005799 RID: 22425
	bool LocalPlayerIsOwner();

	// Token: 0x0600579A RID: 22426
	bool IsOwner(PlatformUserIdentifierAbs _userIdentifier);

	// Token: 0x0600579B RID: 22427
	bool HasPassword();

	// Token: 0x0600579C RID: 22428
	bool CheckPassword(string _password, PlatformUserIdentifierAbs _userIdentifier, out bool changed);

	// Token: 0x0600579D RID: 22429
	string GetPassword();
}
