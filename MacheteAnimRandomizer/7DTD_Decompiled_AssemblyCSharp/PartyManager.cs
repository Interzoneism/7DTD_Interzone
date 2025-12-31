using System;
using System.Collections.Generic;

// Token: 0x0200080A RID: 2058
public class PartyManager
{
	// Token: 0x170005FB RID: 1531
	// (get) Token: 0x06003B15 RID: 15125 RVA: 0x0017C7CC File Offset: 0x0017A9CC
	public static PartyManager Current
	{
		get
		{
			PartyManager result;
			if ((result = PartyManager.instance) == null)
			{
				result = (PartyManager.instance = new PartyManager());
			}
			return result;
		}
	}

	// Token: 0x170005FC RID: 1532
	// (get) Token: 0x06003B16 RID: 15126 RVA: 0x0017C7E2 File Offset: 0x0017A9E2
	public static bool HasInstance
	{
		get
		{
			return PartyManager.instance != null;
		}
	}

	// Token: 0x06003B17 RID: 15127 RVA: 0x0017C7EC File Offset: 0x0017A9EC
	[PublicizedFrom(EAccessModifier.Private)]
	public PartyManager()
	{
		this.voice = PartyVoice.Instance;
	}

	// Token: 0x06003B18 RID: 15128 RVA: 0x0017C80C File Offset: 0x0017AA0C
	public Party CreateParty()
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return null;
		}
		Party party = new Party();
		int partyID = this.nextPartyID + 1;
		this.nextPartyID = partyID;
		party.PartyID = partyID;
		Party party2 = party;
		this.partyList.Add(party2);
		return party2;
	}

	// Token: 0x06003B19 RID: 15129 RVA: 0x0017C854 File Offset: 0x0017AA54
	public Party CreateClientParty(World world, int partyID, int leaderIndex, int[] partyMembers, string voiceLobbyId)
	{
		Party party = new Party
		{
			PartyID = partyID,
			LeaderIndex = leaderIndex,
			VoiceLobbyId = voiceLobbyId
		};
		this.partyList.Add(party);
		party.UpdateMemberList(world, partyMembers);
		return party;
	}

	// Token: 0x06003B1A RID: 15130 RVA: 0x0017C893 File Offset: 0x0017AA93
	public void RemoveParty(Party party)
	{
		if (this.partyList.Contains(party))
		{
			this.partyList.Remove(party);
		}
	}

	// Token: 0x06003B1B RID: 15131 RVA: 0x0017C8B0 File Offset: 0x0017AAB0
	public Party GetParty(int partyID)
	{
		for (int i = 0; i < this.partyList.Count; i++)
		{
			if (this.partyList[i].PartyID == partyID)
			{
				return this.partyList[i];
			}
		}
		return null;
	}

	// Token: 0x06003B1C RID: 15132 RVA: 0x0017C8F5 File Offset: 0x0017AAF5
	public void Cleanup()
	{
		this.partyList.Clear();
		this.nextPartyID = 0;
	}

	// Token: 0x06003B1D RID: 15133 RVA: 0x0017C909 File Offset: 0x0017AB09
	public void Update()
	{
		this.voice.Update();
	}

	// Token: 0x04002FF4 RID: 12276
	[PublicizedFrom(EAccessModifier.Private)]
	public static PartyManager instance;

	// Token: 0x04002FF5 RID: 12277
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly PartyVoice voice;

	// Token: 0x04002FF6 RID: 12278
	[PublicizedFrom(EAccessModifier.Private)]
	public int nextPartyID;

	// Token: 0x04002FF7 RID: 12279
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<Party> partyList = new List<Party>();
}
