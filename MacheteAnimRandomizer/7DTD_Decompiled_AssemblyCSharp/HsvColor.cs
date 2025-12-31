using System;

// Token: 0x020011AB RID: 4523
public struct HsvColor
{
	// Token: 0x17000EA4 RID: 3748
	// (get) Token: 0x06008D6B RID: 36203 RVA: 0x0038DBAC File Offset: 0x0038BDAC
	// (set) Token: 0x06008D6C RID: 36204 RVA: 0x0038DBBB File Offset: 0x0038BDBB
	public float normalizedH
	{
		get
		{
			return (float)this.H / 360f;
		}
		set
		{
			this.H = (double)value * 360.0;
		}
	}

	// Token: 0x17000EA5 RID: 3749
	// (get) Token: 0x06008D6D RID: 36205 RVA: 0x0038DBCF File Offset: 0x0038BDCF
	// (set) Token: 0x06008D6E RID: 36206 RVA: 0x0038DBD8 File Offset: 0x0038BDD8
	public float normalizedS
	{
		get
		{
			return (float)this.S;
		}
		set
		{
			this.S = (double)value;
		}
	}

	// Token: 0x17000EA6 RID: 3750
	// (get) Token: 0x06008D6F RID: 36207 RVA: 0x0038DBE2 File Offset: 0x0038BDE2
	// (set) Token: 0x06008D70 RID: 36208 RVA: 0x0038DBEB File Offset: 0x0038BDEB
	public float normalizedV
	{
		get
		{
			return (float)this.V;
		}
		set
		{
			this.V = (double)value;
		}
	}

	// Token: 0x06008D71 RID: 36209 RVA: 0x0038DBF5 File Offset: 0x0038BDF5
	public HsvColor(double h, double s, double v)
	{
		this.H = h;
		this.S = s;
		this.V = v;
	}

	// Token: 0x06008D72 RID: 36210 RVA: 0x0038DC0C File Offset: 0x0038BE0C
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			"{",
			this.H.ToCultureInvariantString("f2"),
			",",
			this.S.ToCultureInvariantString("f2"),
			",",
			this.V.ToCultureInvariantString("f2"),
			"}"
		});
	}

	// Token: 0x04006DC7 RID: 28103
	public double H;

	// Token: 0x04006DC8 RID: 28104
	public double S;

	// Token: 0x04006DC9 RID: 28105
	public double V;
}
