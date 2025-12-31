using System;

// Token: 0x0200082E RID: 2094
public class PersistentPlayerName
{
	// Token: 0x17000625 RID: 1573
	// (get) Token: 0x06003C48 RID: 15432 RVA: 0x00183FCB File Offset: 0x001821CB
	public AuthoredText AuthoredName
	{
		get
		{
			return this.playerName;
		}
	}

	// Token: 0x17000626 RID: 1574
	// (get) Token: 0x06003C49 RID: 15433 RVA: 0x00183FD4 File Offset: 0x001821D4
	public string DisplayName
	{
		get
		{
			if (this.cachedDisplayName != null)
			{
				return this.cachedDisplayName + ((this.nameSuffix > 0) ? string.Format("({0})", this.nameSuffix) : "");
			}
			if (!GeneratedTextManager.IsFiltered(this.playerName))
			{
				if (!GeneratedTextManager.IsFiltering(this.playerName))
				{
					GeneratedTextManager.PrefilterText(this.playerName, GeneratedTextManager.TextFilteringMode.Filter);
				}
				return GeneratedTextManager.GetDisplayTextImmediately(this.playerName, false, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes) + ((this.nameSuffix > 0) ? string.Format("({0})", this.nameSuffix) : "");
			}
			this.cachedDisplayName = GeneratedTextManager.GetDisplayTextImmediately(this.playerName, false, GeneratedTextManager.TextFilteringMode.FilterOtherPlatforms, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
			return this.cachedDisplayName + ((this.nameSuffix > 0) ? string.Format("({0})", this.nameSuffix) : "");
		}
	}

	// Token: 0x17000627 RID: 1575
	// (get) Token: 0x06003C4A RID: 15434 RVA: 0x001840C0 File Offset: 0x001822C0
	public string SafeDisplayName
	{
		get
		{
			if (!GeneratedTextManager.IsFiltered(this.playerName))
			{
				if (!GeneratedTextManager.IsFiltering(this.playerName))
				{
					GeneratedTextManager.PrefilterText(this.playerName, GeneratedTextManager.TextFilteringMode.Filter);
				}
				string text = GeneratedTextManager.GetDisplayTextImmediately(this.playerName, false, GeneratedTextManager.TextFilteringMode.FilterOtherPlatforms, GeneratedTextManager.BbCodeSupportMode.SupportedAndAddEscapes);
				if (!text.Equals("{...}") && this.nameSuffix > 0)
				{
					text += string.Format("({0})", this.nameSuffix);
				}
				return text;
			}
			return this.DisplayName;
		}
	}

	// Token: 0x06003C4B RID: 15435 RVA: 0x0018413C File Offset: 0x0018233C
	public PersistentPlayerName(AuthoredText name)
	{
		this.playerName = name;
		GeneratedTextManager.PrefilterText(name, GeneratedTextManager.TextFilteringMode.FilterOtherPlatforms);
	}

	// Token: 0x06003C4C RID: 15436 RVA: 0x00184152 File Offset: 0x00182352
	public void Update(string name, PlatformUserIdentifierAbs author)
	{
		this.cachedDisplayName = null;
		this.nameSuffix = 0;
		this.playerName.Update(name, author);
		GeneratedTextManager.PrefilterText(this.playerName, GeneratedTextManager.TextFilteringMode.FilterOtherPlatforms);
		GameManager.Instance.persistentPlayers.FixNameCollisions(name);
	}

	// Token: 0x06003C4D RID: 15437 RVA: 0x0018418C File Offset: 0x0018238C
	public void Update(AuthoredText name)
	{
		if (this.playerName != name)
		{
			this.cachedDisplayName = null;
			this.nameSuffix = 0;
			this.playerName = name;
			GeneratedTextManager.PrefilterText(this.playerName, GeneratedTextManager.TextFilteringMode.FilterOtherPlatforms);
			GameManager.Instance.persistentPlayers.FixNameCollisions(name.Text);
		}
	}

	// Token: 0x06003C4E RID: 15438 RVA: 0x001841D8 File Offset: 0x001823D8
	public void SetCollisionSuffix(int suffix)
	{
		this.nameSuffix = suffix;
	}

	// Token: 0x040030BE RID: 12478
	[PublicizedFrom(EAccessModifier.Private)]
	public AuthoredText playerName;

	// Token: 0x040030BF RID: 12479
	[PublicizedFrom(EAccessModifier.Private)]
	public string cachedDisplayName;

	// Token: 0x040030C0 RID: 12480
	[PublicizedFrom(EAccessModifier.Private)]
	public int nameSuffix;
}
