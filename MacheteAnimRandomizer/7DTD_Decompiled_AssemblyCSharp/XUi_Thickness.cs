using System;

// Token: 0x02000EDB RID: 3803
public class XUi_Thickness
{
	// Token: 0x06007802 RID: 30722 RVA: 0x0030DCFF File Offset: 0x0030BEFF
	public XUi_Thickness(int newLeft, int newTop, int newRight, int newBottom)
	{
		this.left = newLeft;
		this.top = newTop;
		this.right = newRight;
		this.bottom = newBottom;
	}

	// Token: 0x06007803 RID: 30723 RVA: 0x0030DD24 File Offset: 0x0030BF24
	public XUi_Thickness(int newLeftRight, int newTopBottom) : this(newLeftRight, newTopBottom, newLeftRight, newTopBottom)
	{
	}

	// Token: 0x06007804 RID: 30724 RVA: 0x0030DD30 File Offset: 0x0030BF30
	public XUi_Thickness(int newSides) : this(newSides, newSides, newSides, newSides)
	{
	}

	// Token: 0x06007805 RID: 30725 RVA: 0x0030DD3C File Offset: 0x0030BF3C
	public static XUi_Thickness Parse(string _s)
	{
		string[] array = _s.Split(',', StringSplitOptions.None);
		switch (array.Length)
		{
		case 1:
			return new XUi_Thickness(int.Parse(array[0]));
		case 2:
			return new XUi_Thickness(int.Parse(array[0]), int.Parse(array[1]));
		case 4:
			return new XUi_Thickness(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), int.Parse(array[3]));
		}
		return new XUi_Thickness(0);
	}

	// Token: 0x04005B86 RID: 23430
	public int left;

	// Token: 0x04005B87 RID: 23431
	public int top;

	// Token: 0x04005B88 RID: 23432
	public int right;

	// Token: 0x04005B89 RID: 23433
	public int bottom;
}
