using System;
using System.IO;

// Token: 0x02000AE2 RID: 2786
public class AuthoredText
{
	// Token: 0x17000875 RID: 2165
	// (get) Token: 0x060055B8 RID: 21944 RVA: 0x00230D43 File Offset: 0x0022EF43
	// (set) Token: 0x060055B9 RID: 21945 RVA: 0x00230D4B File Offset: 0x0022EF4B
	public string Text { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000876 RID: 2166
	// (get) Token: 0x060055BA RID: 21946 RVA: 0x00230D54 File Offset: 0x0022EF54
	// (set) Token: 0x060055BB RID: 21947 RVA: 0x00230D5C File Offset: 0x0022EF5C
	public PlatformUserIdentifierAbs Author { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060055BC RID: 21948 RVA: 0x00230D65 File Offset: 0x0022EF65
	public AuthoredText()
	{
		this.Update(string.Empty, null);
	}

	// Token: 0x060055BD RID: 21949 RVA: 0x00230D79 File Offset: 0x0022EF79
	public AuthoredText(string _text, PlatformUserIdentifierAbs _author)
	{
		this.Update(_text, _author);
	}

	// Token: 0x060055BE RID: 21950 RVA: 0x00230D89 File Offset: 0x0022EF89
	public void Update(string _text, PlatformUserIdentifierAbs _author)
	{
		this.Author = _author;
		this.Text = _text;
	}

	// Token: 0x060055BF RID: 21951 RVA: 0x00230D9C File Offset: 0x0022EF9C
	public static AuthoredText FromStream(BinaryReader _reader)
	{
		if (_reader == null)
		{
			return null;
		}
		if (!_reader.ReadBoolean())
		{
			return null;
		}
		string text = _reader.ReadString();
		PlatformUserIdentifierAbs author = PlatformUserIdentifierAbs.FromStream(_reader, false, false);
		AuthoredText authoredText = new AuthoredText();
		authoredText.Update(text, author);
		return authoredText;
	}

	// Token: 0x060055C0 RID: 21952 RVA: 0x00230DD5 File Offset: 0x0022EFD5
	public static void ToStream(AuthoredText _instance, BinaryWriter _writer)
	{
		if (_writer == null)
		{
			return;
		}
		if (_instance == null)
		{
			_writer.Write(0);
			return;
		}
		_writer.Write(1);
		_writer.Write(_instance.Text);
		_instance.Author.ToStream(_writer, false);
	}

	// Token: 0x060055C1 RID: 21953 RVA: 0x00230E06 File Offset: 0x0022F006
	public static AuthoredText Clone(AuthoredText _cloneFrom)
	{
		if (_cloneFrom == null)
		{
			return null;
		}
		return new AuthoredText(_cloneFrom.Text, _cloneFrom.Author);
	}
}
