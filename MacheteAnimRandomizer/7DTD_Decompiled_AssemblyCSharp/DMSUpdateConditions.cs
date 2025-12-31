using System;

// Token: 0x02000379 RID: 889
public struct DMSUpdateConditions
{
	// Token: 0x17000301 RID: 769
	// (set) Token: 0x06001A59 RID: 6745 RVA: 0x000A3E2F File Offset: 0x000A202F
	public bool DoesPlayerExist
	{
		set
		{
			this.SetBoolHolder(value, 128);
		}
	}

	// Token: 0x17000302 RID: 770
	// (set) Token: 0x06001A5A RID: 6746 RVA: 0x000A3E3D File Offset: 0x000A203D
	public bool IsGameUnPaused
	{
		set
		{
			this.SetBoolHolder(value, 64);
		}
	}

	// Token: 0x17000303 RID: 771
	// (set) Token: 0x06001A5B RID: 6747 RVA: 0x000A3E48 File Offset: 0x000A2048
	public bool IsDMSInitialized
	{
		set
		{
			this.SetBoolHolder(value, 32);
		}
	}

	// Token: 0x17000304 RID: 772
	// (set) Token: 0x06001A5C RID: 6748 RVA: 0x000A3E53 File Offset: 0x000A2053
	public bool IsDMSEnabled
	{
		set
		{
			this.SetBoolHolder(value, 16);
		}
	}

	// Token: 0x17000305 RID: 773
	// (get) Token: 0x06001A5D RID: 6749 RVA: 0x000A3E5E File Offset: 0x000A205E
	public bool CanUpdate
	{
		get
		{
			return this.BoolHolder == 240;
		}
	}

	// Token: 0x06001A5E RID: 6750 RVA: 0x000A3E6D File Offset: 0x000A206D
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetBoolHolder(bool _value, byte _place)
	{
		if (_value)
		{
			this.BoolHolder |= _place;
			return;
		}
		this.BoolHolder &= ~_place;
	}

	// Token: 0x04001115 RID: 4373
	[PublicizedFrom(EAccessModifier.Private)]
	public byte BoolHolder;
}
