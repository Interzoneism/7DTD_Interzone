using System;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x02000E91 RID: 3729
[Preserve]
public class XUiC_TwitchVoteEntry : XUiController
{
	// Token: 0x17000BF9 RID: 3065
	// (get) Token: 0x0600759A RID: 30106 RVA: 0x002FE082 File Offset: 0x002FC282
	// (set) Token: 0x0600759B RID: 30107 RVA: 0x002FE08A File Offset: 0x002FC28A
	public XUiC_TwitchWindow Owner { get; set; }

	// Token: 0x17000BFA RID: 3066
	// (get) Token: 0x0600759C RID: 30108 RVA: 0x002FE093 File Offset: 0x002FC293
	// (set) Token: 0x0600759D RID: 30109 RVA: 0x002FE09B File Offset: 0x002FC29B
	public TwitchVoteEntry Vote
	{
		get
		{
			return this.vote;
		}
		set
		{
			this.vote = value;
			this.isDirty = true;
		}
	}

	// Token: 0x0600759E RID: 30110 RVA: 0x002FE0AC File Offset: 0x002FC2AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		bool flag = this.vote != null;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 3057604319U)
		{
			if (num <= 2104701544U)
			{
				if (num != 546437152U)
				{
					if (num != 596770009U)
					{
						if (num == 2104701544U)
						{
							if (bindingName == "votecolor")
							{
								if (flag)
								{
									if (this.vote.Owner.IsHighest(this.vote))
									{
										value = this.positiveColor;
									}
									else
									{
										value = this.negativeColor;
									}
								}
								else
								{
									value = "0,0,0,0";
								}
								return true;
							}
						}
					}
					else if (bindingName == "hasvoteline2")
					{
						if (flag)
						{
							value = (this.vote.VoteClass.VoteLine2 != "").ToString();
						}
						else
						{
							value = "false";
						}
						return true;
					}
				}
				else if (bindingName == "hasvoteline1")
				{
					if (flag)
					{
						value = (this.vote.VoteClass.VoteLine1 != "").ToString();
					}
					else
					{
						value = "false";
					}
					return true;
				}
			}
			else if (num != 2382705624U)
			{
				if (num != 2840813878U)
				{
					if (num == 3057604319U)
					{
						if (bindingName == "voteline2")
						{
							if (flag)
							{
								value = this.vote.VoteClass.VoteLine2;
							}
							return true;
						}
					}
				}
				else if (bindingName == "votename")
				{
					if (flag)
					{
						if (this.vote.Index == 2 && this.vote.Owner.UseMystery)
						{
							value = "?????";
						}
						else
						{
							value = this.vote.VoteClass.Display;
						}
					}
					else
					{
						value = "";
					}
					return true;
				}
			}
			else if (bindingName == "votefill")
			{
				if (flag)
				{
					if (this.vote.VoteCount > 0)
					{
						value = ((float)this.vote.VoteCount / (float)this.vote.Owner.VoteCount).ToString();
					}
					else
					{
						value = "0";
					}
				}
				else
				{
					value = "0";
				}
				return true;
			}
		}
		else if (num <= 3604996386U)
		{
			if (num != 3074381938U)
			{
				if (num != 3366025627U)
				{
					if (num == 3604996386U)
					{
						if (bindingName == "votecommand")
						{
							if (this.isWinner)
							{
								value = "";
							}
							else
							{
								value = (flag ? this.vote.VoteCommand : "");
							}
							return true;
						}
					}
				}
				else if (bindingName == "hasvote")
				{
					value = flag.ToString();
					return true;
				}
			}
			else if (bindingName == "voteline1")
			{
				if (flag)
				{
					value = this.vote.VoteClass.VoteLine1;
				}
				return true;
			}
		}
		else if (num != 3735793316U)
		{
			if (num != 3918524111U)
			{
				if (num == 4025064268U)
				{
					if (bindingName == "line1textcolor")
					{
						if (flag)
						{
							switch (this.vote.VoteClass.DisplayType)
							{
							case TwitchVote.VoteDisplayTypes.GoodBad:
								value = this.textBadColor;
								break;
							case TwitchVote.VoteDisplayTypes.Special:
								value = this.textGoodColor;
								break;
							case TwitchVote.VoteDisplayTypes.HordeBuffed:
								value = this.selectedTextColor;
								break;
							}
						}
						else
						{
							value = "255,255,255";
						}
						return true;
					}
				}
			}
			else if (bindingName == "line2textcolor")
			{
				if (flag)
				{
					if (this.vote.VoteClass.DisplayType == TwitchVote.VoteDisplayTypes.GoodBad)
					{
						value = this.textBadColor;
					}
				}
				else
				{
					value = "255,255,255";
				}
				return true;
			}
		}
		else if (bindingName == "votecount")
		{
			if (flag)
			{
				if (!this.isWinner)
				{
					float num2 = 0f;
					if (this.vote.VoteCount > 0)
					{
						num2 = (float)this.vote.VoteCount / (float)this.vote.Owner.VoteCount;
					}
					value = this.voteCountFormatterInt.Format((int)(num2 * 100f));
				}
				else
				{
					value = "";
				}
			}
			else
			{
				value = "";
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600759F RID: 30111 RVA: 0x002FE4E8 File Offset: 0x002FC6E8
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		if (name == "positive_color")
		{
			this.positiveColor = value;
			return true;
		}
		if (name == "negative_color")
		{
			this.negativeColor = value;
			return true;
		}
		if (name == "disabled_color")
		{
			this.disabledColor = value;
			return true;
		}
		if (name == "selected_color")
		{
			this.selectedTextColor = value;
			return true;
		}
		if (name == "bad_color")
		{
			this.textBadColor = value;
			return true;
		}
		if (!(name == "good_color"))
		{
			return base.ParseAttribute(name, value, _parent);
		}
		this.textGoodColor = value;
		return true;
	}

	// Token: 0x060075A0 RID: 30112 RVA: 0x002BF216 File Offset: 0x002BD416
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleOnCountChanged(XUiController _sender, OnCountChangedEventArgs _e)
	{
		base.RefreshBindings(true);
	}

	// Token: 0x060075A1 RID: 30113 RVA: 0x002FE584 File Offset: 0x002FC784
	public override void Update(float _dt)
	{
		if (this.Vote != null && this.Vote.UIDirty)
		{
			this.isDirty = true;
			this.Vote.UIDirty = false;
		}
		if (this.isDirty)
		{
			base.RefreshBindings(this.isDirty);
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x040059B0 RID: 22960
	[PublicizedFrom(EAccessModifier.Private)]
	public TwitchVoteEntry vote;

	// Token: 0x040059B1 RID: 22961
	public bool isDirty;

	// Token: 0x040059B2 RID: 22962
	public bool isWinner;

	// Token: 0x040059B3 RID: 22963
	[PublicizedFrom(EAccessModifier.Private)]
	public string positiveColor = "0,0,255";

	// Token: 0x040059B4 RID: 22964
	[PublicizedFrom(EAccessModifier.Private)]
	public string negativeColor = "255,0,0";

	// Token: 0x040059B5 RID: 22965
	[PublicizedFrom(EAccessModifier.Private)]
	public string textBadColor = "255,175,175";

	// Token: 0x040059B6 RID: 22966
	[PublicizedFrom(EAccessModifier.Private)]
	public string textGoodColor = "175,175,255";

	// Token: 0x040059B7 RID: 22967
	[PublicizedFrom(EAccessModifier.Private)]
	public string selectedTextColor = "222,206,163";

	// Token: 0x040059B8 RID: 22968
	[PublicizedFrom(EAccessModifier.Private)]
	public string disabledColor = "80,80,80";

	// Token: 0x040059BA RID: 22970
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isReady;

	// Token: 0x040059BB RID: 22971
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<int> voteCountFormatterInt = new CachedStringFormatter<int>((int _i) => _i.ToString() + "%");
}
