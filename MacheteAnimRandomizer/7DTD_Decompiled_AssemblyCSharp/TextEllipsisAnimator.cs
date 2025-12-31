using System;
using System.Text;
using System.Text.RegularExpressions;

// Token: 0x0200122E RID: 4654
public class TextEllipsisAnimator
{
	// Token: 0x17000F0C RID: 3852
	// (get) Token: 0x0600916A RID: 37226 RVA: 0x0039F26E File Offset: 0x0039D46E
	public int animationStates
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (!this.isChinese)
			{
				return 4;
			}
			return 3;
		}
	}

	// Token: 0x0600916B RID: 37227 RVA: 0x0039F27C File Offset: 0x0039D47C
	public TextEllipsisAnimator(string _input, UILabel _uiLabel)
	{
		this.uiLabel = _uiLabel;
		this.isChinese = (Localization.language == "schinese" || Localization.language == "tchinese");
		this.lastEllipsisState = (this.isChinese ? TextEllipsisAnimator.ellipsisEndingsChinese[2] : TextEllipsisAnimator.ellipsisEndings[3]);
		this.Init(_input);
	}

	// Token: 0x0600916C RID: 37228 RVA: 0x0039F2FC File Offset: 0x0039D4FC
	public TextEllipsisAnimator(string _input, XUiV_Label _label)
	{
		this.uiLabel = _label.Label;
		this.xuiLabel = _label;
		this.isChinese = (Localization.language == "schinese" || Localization.language == "tchinese");
		this.lastEllipsisState = (this.isChinese ? TextEllipsisAnimator.ellipsisEndingsChinese[2] : TextEllipsisAnimator.ellipsisEndings[3]);
		this.Init(_input);
	}

	// Token: 0x0600916D RID: 37229 RVA: 0x0039F388 File Offset: 0x0039D588
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init(string _input)
	{
		this.currentEllipsisState = this.animationStates - 1;
		this.sb = new StringBuilder();
		if (TextEllipsisAnimator.ellipsisBreakPatternEastern == null)
		{
			TextEllipsisAnimator.ellipsisBreakPatternEastern = (this.isChinese ? new Regex("([\\u3008-\\u9FFF])(\\u2026\\n\\u2026)") : new Regex("([\\u3008-\\u9FFF])(\\.\\n\\.\\.|\\.\\.\\n\\.)"));
		}
		this.SetBaseString(_input, TextEllipsisAnimator.AnimationMode.All);
	}

	// Token: 0x0600916E RID: 37230 RVA: 0x0039F3E0 File Offset: 0x0039D5E0
	public void GetNextAnimatedString(float _dt)
	{
		if (this.totalEllipsis == 0 || this.baseString == null || this.uiLabel == null)
		{
			return;
		}
		this.ellipsisTimer += _dt;
		if (this.ellipsisTimer >= 0.5f)
		{
			this.currentEllipsisState = (this.currentEllipsisState + 1) % this.animationStates;
			this.ellipsisTimer = 0f;
			this.UpdateLabel();
		}
	}

	// Token: 0x0600916F RID: 37231 RVA: 0x0039F450 File Offset: 0x0039D650
	public void SetBaseString(string _input, TextEllipsisAnimator.AnimationMode _mode = TextEllipsisAnimator.AnimationMode.All)
	{
		if (_input == null || this.uiLabel == null || (this.xuiLabel != null && !this.xuiLabel.SupportBbCode) || _mode == TextEllipsisAnimator.AnimationMode.Off)
		{
			this.baseString = null;
			return;
		}
		this.sb.Clear();
		this.mostRecentAlphaBB = "[FF]";
		this.cycles = 0;
		this.totalEllipsis = 0;
		this.sb.Append(_input);
		if (_mode == TextEllipsisAnimator.AnimationMode.Final)
		{
			int i;
			for (i = this.sb.Length - 1; i > 0; i--)
			{
				if (!char.IsWhiteSpace(this.sb[i]))
				{
					break;
				}
				this.sb.Remove(i, 1);
			}
			while (i >= 3 && this.sb[i] == '.' && this.sb[i - 1] == '.')
			{
				if (this.sb[i - 2] != '.')
				{
					break;
				}
				this.sb.Remove(i - 2, 3);
				i -= 3;
				this.totalEllipsis = 1;
			}
			while (i >= 1)
			{
				if (this.sb[i] != '…')
				{
					break;
				}
				this.sb.Remove(i, 1);
				i--;
				this.totalEllipsis = 1;
			}
			while (i > 0 && char.IsWhiteSpace(this.sb[i]))
			{
				this.sb.Remove(i, 1);
				i--;
			}
			if (this.totalEllipsis > 0)
			{
				this.sb.Append(this.lastEllipsisState);
			}
		}
		else if (_mode == TextEllipsisAnimator.AnimationMode.All)
		{
			for (int n = 0; n < this.sb.Length; n++)
			{
				if (this.sb[n] == '[' && n + 3 < this.sb.Length && this.sb[n + 3] == ']' && this.IsHexDigit(this.sb[n + 1]) && this.IsHexDigit(this.sb[n + 2]))
				{
					this.mostRecentAlphaBB = this.sb[n].ToString() + this.sb[n + 1].ToString() + this.sb[n + 2].ToString() + this.sb[n + 3].ToString();
					n += 3;
				}
				if (this.sb[n] == '…')
				{
					int num = 0;
					while (n + num + 1 < this.sb.Length && this.sb[n + num + 1] == '…')
					{
						num++;
					}
					if (num > 0)
					{
						this.sb.Remove(n + 1, num);
					}
					this.sb.Remove(n, 1);
					this.sb.Insert(n, this.lastEllipsisState + this.mostRecentAlphaBB);
					n += this.lastEllipsisState.Length - 1 + 4;
					this.totalEllipsis++;
				}
			}
			for (int k = 0; k < this.sb.Length - 2; k++)
			{
				if (this.sb[k] == '[' && k + 3 < this.sb.Length && this.sb[k + 3] == ']' && this.IsHexDigit(this.sb[k + 1]) && this.IsHexDigit(this.sb[k + 2]))
				{
					this.mostRecentAlphaBB = this.sb[k].ToString() + this.sb[k + 1].ToString() + this.sb[k + 2].ToString() + this.sb[k + 3].ToString();
					k += 3;
				}
				if (this.sb[k] == '.' && this.sb[k + 1] == '.' && this.sb[k + 2] == '.')
				{
					this.sb.Remove(k, 3);
					this.sb.Insert(k, this.lastEllipsisState + this.mostRecentAlphaBB);
					k += 3 - this.lastEllipsisState.Length + 4;
					this.totalEllipsis++;
				}
			}
		}
		this.baseString = this.sb.ToString();
		XUiV_Label xuiV_Label = this.xuiLabel;
		if (xuiV_Label != null)
		{
			xuiV_Label.SetText(this.baseString);
		}
		int processedMatches = 0;
		for (int l = 0; l < this.totalEllipsis; l++)
		{
			XUiV_Label xuiV_Label2 = this.xuiLabel;
			if (xuiV_Label2 != null)
			{
				xuiV_Label2.UpdateData();
			}
			if (this.xuiLabel != null && this.xuiLabel.Text == this.xuiLabel.Label.processedText)
			{
				break;
			}
			bool hadEastern = false;
			int j = 0;
			this.baseString = TextEllipsisAnimator.ellipsisBreakPatternEastern.Replace(this.uiLabel.processedText, delegate(Match m)
			{
				int num2;
				if (j == processedMatches)
				{
					hadEastern = true;
					num2 = processedMatches;
					processedMatches = num2 + 1;
					return m.Groups[1].Value + "\n" + this.lastEllipsisState;
				}
				num2 = j;
				j = num2 + 1;
				return m.Value;
			});
			if (!hadEastern)
			{
				j = 0;
				this.baseString = TextEllipsisAnimator.ellipsisBreakPattern.Replace(this.uiLabel.processedText, delegate(Match m)
				{
					int num2;
					if (j == processedMatches)
					{
						num2 = processedMatches;
						processedMatches = num2 + 1;
						return "\n" + m.Groups[1].Value + this.lastEllipsisState;
					}
					num2 = j;
					j = num2 + 1;
					return m.Value;
				});
				XUiV_Label xuiV_Label3 = this.xuiLabel;
				if (xuiV_Label3 != null)
				{
					xuiV_Label3.SetText(this.baseString);
				}
			}
		}
		this.UpdateLabel();
	}

	// Token: 0x06009170 RID: 37232 RVA: 0x0039FA28 File Offset: 0x0039DC28
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLabel()
	{
		if (this.totalEllipsis > 0)
		{
			if (this.cycles < this.animationStates)
			{
				this.cycles++;
				if (this.isChinese)
				{
					this.animatedStrings[this.currentEllipsisState] = this.baseString.Replace(this.lastEllipsisState, TextEllipsisAnimator.ellipsisEndingsChinese[this.currentEllipsisState]);
				}
				else
				{
					this.animatedStrings[this.currentEllipsisState] = this.baseString.Replace(this.lastEllipsisState, TextEllipsisAnimator.ellipsisEndings[this.currentEllipsisState]);
				}
			}
			if (this.xuiLabel != null)
			{
				this.xuiLabel.Text = this.animatedStrings[this.currentEllipsisState];
				this.xuiLabel.UpdateData();
				return;
			}
			this.uiLabel.text = this.animatedStrings[this.currentEllipsisState];
			return;
		}
		else
		{
			if (this.xuiLabel != null)
			{
				this.xuiLabel.SetText(this.baseString);
				return;
			}
			this.uiLabel.text = this.baseString;
			return;
		}
	}

	// Token: 0x06009171 RID: 37233 RVA: 0x0039FB2B File Offset: 0x0039DD2B
	[PublicizedFrom(EAccessModifier.Private)]
	public bool IsHexDigit(char c)
	{
		return char.IsDigit(c) || (char.ToLower(c) >= 'a' && char.ToLower(c) <= 'f');
	}

	// Token: 0x04006F8E RID: 28558
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string[] ellipsisEndings = new string[]
	{
		"[00]...",
		".[00]..",
		"..[00].",
		"..."
	};

	// Token: 0x04006F8F RID: 28559
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string[] ellipsisEndingsChinese = new string[]
	{
		"[00]……",
		"…[00]…",
		"……"
	};

	// Token: 0x04006F90 RID: 28560
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly bool isChinese;

	// Token: 0x04006F91 RID: 28561
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string lastEllipsisState;

	// Token: 0x04006F92 RID: 28562
	[PublicizedFrom(EAccessModifier.Private)]
	public static Regex ellipsisBreakPatternEastern = null;

	// Token: 0x04006F93 RID: 28563
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex ellipsisBreakPattern = new Regex("(\\b\\w+\\b)(\\n\\.\\.\\.|\\.\\n\\.\\.|\\.\\.\\n\\.)");

	// Token: 0x04006F94 RID: 28564
	[PublicizedFrom(EAccessModifier.Private)]
	public const char ellipsisChar = '…';

	// Token: 0x04006F95 RID: 28565
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label xuiLabel;

	// Token: 0x04006F96 RID: 28566
	[PublicizedFrom(EAccessModifier.Private)]
	public UILabel uiLabel;

	// Token: 0x04006F97 RID: 28567
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentEllipsisState;

	// Token: 0x04006F98 RID: 28568
	[PublicizedFrom(EAccessModifier.Private)]
	public float ellipsisTimer;

	// Token: 0x04006F99 RID: 28569
	[PublicizedFrom(EAccessModifier.Private)]
	public int cycles;

	// Token: 0x04006F9A RID: 28570
	[PublicizedFrom(EAccessModifier.Private)]
	public string baseString;

	// Token: 0x04006F9B RID: 28571
	[PublicizedFrom(EAccessModifier.Private)]
	public int totalEllipsis;

	// Token: 0x04006F9C RID: 28572
	[PublicizedFrom(EAccessModifier.Private)]
	public string[] animatedStrings = new string[4];

	// Token: 0x04006F9D RID: 28573
	[PublicizedFrom(EAccessModifier.Private)]
	public string mostRecentAlphaBB = "[FF]";

	// Token: 0x04006F9E RID: 28574
	[PublicizedFrom(EAccessModifier.Private)]
	public StringBuilder sb;

	// Token: 0x0200122F RID: 4655
	public enum AnimationMode
	{
		// Token: 0x04006FA0 RID: 28576
		Off,
		// Token: 0x04006FA1 RID: 28577
		Final,
		// Token: 0x04006FA2 RID: 28578
		All
	}
}
