using System;
using System.Text;
using UnityEngine;

// Token: 0x02001102 RID: 4354
[RequireComponent(typeof(TextMesh))]
public class SmartTextMesh : MonoBehaviour
{
	// Token: 0x17000E46 RID: 3654
	// (get) Token: 0x060088C5 RID: 35013 RVA: 0x00375EDA File Offset: 0x003740DA
	// (set) Token: 0x060088C6 RID: 35014 RVA: 0x00375EE2 File Offset: 0x003740E2
	public string UnwrappedText
	{
		get
		{
			return this.unwrappedText;
		}
		set
		{
			if (value != this.unwrappedText)
			{
				this.unwrappedText = (value ?? "");
				this.NeedsLayout = true;
			}
		}
	}

	// Token: 0x17000E47 RID: 3655
	// (get) Token: 0x060088C7 RID: 35015 RVA: 0x00375F09 File Offset: 0x00374109
	// (set) Token: 0x060088C8 RID: 35016 RVA: 0x00375F17 File Offset: 0x00374117
	public float MaxWidthReal
	{
		get
		{
			return this.MaxWidth * 2f;
		}
		set
		{
			this.MaxWidth = value / 2f;
		}
	}

	// Token: 0x060088C9 RID: 35017 RVA: 0x00375F28 File Offset: 0x00374128
	public void Start()
	{
		this.TheMesh = base.GetComponent<TextMesh>();
		if (this.ConvertNewLines && this.UnwrappedText != null)
		{
			this.UnwrappedText = this.UnwrappedText.Replace("\\n", "\n");
		}
		if (this.UnwrappedText == null)
		{
			this.UnwrappedText = "";
		}
	}

	// Token: 0x060088CA RID: 35018 RVA: 0x00375F84 File Offset: 0x00374184
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		if (!this.NeedsLayout)
		{
			return;
		}
		this.NeedsLayout = false;
		if (this.MaxWidth == 0f)
		{
			this.TheMesh.text = this.UnwrappedText;
			return;
		}
		if (this.SeperatedLinesMode)
		{
			this.FormatSeparateLines();
			return;
		}
		this.WrapTextToWidth();
	}

	// Token: 0x060088CB RID: 35019 RVA: 0x00375FD8 File Offset: 0x003741D8
	public bool CanRenderString(string _text)
	{
		foreach (char c in _text.Trim())
		{
			if (c != '\n' && !this.TheMesh.font.HasCharacter(c))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060088CC RID: 35020 RVA: 0x00376020 File Offset: 0x00374220
	public unsafe void WrapTextToWidth()
	{
		this.TheMesh.font.RequestCharactersInTexture(this.unwrappedText, this.TheMesh.fontSize, this.TheMesh.fontStyle);
		float textWidth = this.GetTextWidth(this.TheMesh, " ");
		ReadOnlySpan<char> span = this.unwrappedText;
		span = span.Trim();
		int num = 0;
		int num2 = 1;
		int num3 = 0;
		float num4 = 0f;
		this.wrappedText.Clear();
		while (num < span.Length && num2 <= this.MaxLines)
		{
			int num5 = span.Slice(num).IndexOfAny(' ', '\n');
			if (num5 < 0)
			{
				num5 = span.Length;
			}
			else
			{
				num5 += num;
			}
			ReadOnlySpan<char> s = span.Slice(num, num5 - num);
			float textWidth2 = this.GetTextWidth(this.TheMesh, s);
			float num6 = (num4 > 0f) ? (num4 + textWidth + textWidth2) : textWidth2;
			if (num6 > this.MaxWidthReal)
			{
				if (num4 > 0f)
				{
					this.wrappedText.Append(span.Slice(num3, num - 1 - num3));
					this.wrappedText.Append('\n');
					num3 = (num = num);
					num2++;
					num4 = 0f;
				}
				else
				{
					float num7 = 1.2f * textWidth2 / this.MaxWidthReal;
					int length = (int)((float)s.Length / num7);
					this.wrappedText.Append(s.Slice(0, length));
					this.wrappedText.Append('…');
					this.wrappedText.Append('\n');
					num3 = (num = num5 + 1);
					num2++;
					num4 = 0f;
				}
			}
			else if (num5 >= span.Length || *span[num5] == 10)
			{
				this.wrappedText.Append(span.Slice(num3, num5 - num3));
				this.wrappedText.Append('\n');
				num2++;
				num3 = (num = num5 + 1);
				num4 = 0f;
			}
			else
			{
				num4 = num6;
				num = num5 + 1;
			}
		}
		if (this.wrappedText.Length > 0 && this.wrappedText[this.wrappedText.Length - 1] == '\n')
		{
			num2--;
			this.wrappedText.Length--;
			if (num < span.Length)
			{
				this.wrappedText.Append('…');
			}
		}
		this.TheMesh.text = this.wrappedText.ToString();
	}

	// Token: 0x060088CD RID: 35021 RVA: 0x003762AC File Offset: 0x003744AC
	[PublicizedFrom(EAccessModifier.Private)]
	public void FormatSeparateLines()
	{
		this.TheMesh.font.RequestCharactersInTexture(this.unwrappedText, this.TheMesh.fontSize, this.TheMesh.fontStyle);
		float textWidth = this.GetTextWidth(this.TheMesh, " ");
		ReadOnlySpan<char> span = this.unwrappedText;
		span = span.Trim();
		this.wrappedText.Clear();
		string[] array = span.ToString().Split('\n', StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(' ', StringSplitOptions.None);
			float num = 0f;
			for (int j = 0; j < array2.Length; j++)
			{
				ReadOnlySpan<char> readOnlySpan = array2[j];
				float textWidth2 = this.GetTextWidth(this.TheMesh, readOnlySpan);
				float num2 = (num > 0f) ? (num + textWidth + textWidth2) : textWidth2;
				if (num2 > this.MaxWidthReal)
				{
					this.wrappedText.Append('…');
					break;
				}
				if (j != 0)
				{
					this.wrappedText.Append(' ');
				}
				this.wrappedText.Append(readOnlySpan);
				num = num2;
			}
			if (i < array.Length - 1)
			{
				this.wrappedText.Append('\n');
			}
		}
		this.TheMesh.text = this.wrappedText.ToString();
	}

	// Token: 0x060088CE RID: 35022 RVA: 0x00376410 File Offset: 0x00374610
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe float GetTextWidth(TextMesh _textMesh, ReadOnlySpan<char> _s)
	{
		Font font = _textMesh.font;
		int fontSize = _textMesh.fontSize;
		FontStyle fontStyle = _textMesh.fontStyle;
		int num = 0;
		ReadOnlySpan<char> readOnlySpan = _s;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = (char)(*readOnlySpan[i]);
			CharacterInfo characterInfo;
			if (font.GetCharacterInfo(c, out characterInfo, fontSize, fontStyle))
			{
				num += characterInfo.advance;
			}
			else
			{
				Log.Warning(string.Format("No character info for symbol '{0}'", c));
			}
		}
		return (float)num * _textMesh.characterSize * Mathf.Abs(_textMesh.transform.lossyScale.x) * 0.1f;
	}

	// Token: 0x04006ABB RID: 27323
	public TextMesh TheMesh;

	// Token: 0x04006ABC RID: 27324
	[TextArea]
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public string unwrappedText;

	// Token: 0x04006ABD RID: 27325
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public readonly StringBuilder wrappedText = new StringBuilder();

	// Token: 0x04006ABE RID: 27326
	public float MaxWidth;

	// Token: 0x04006ABF RID: 27327
	public int MaxLines;

	// Token: 0x04006AC0 RID: 27328
	public bool NeedsLayout = true;

	// Token: 0x04006AC1 RID: 27329
	public bool ConvertNewLines;

	// Token: 0x04006AC2 RID: 27330
	public bool SeperatedLinesMode;
}
