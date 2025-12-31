using System;

// Token: 0x02001250 RID: 4688
public struct Vector2F : IEquatable<Vector2F>
{
	// Token: 0x060092D0 RID: 37584 RVA: 0x003A8408 File Offset: 0x003A6608
	public Vector2F(double angle)
	{
		this.X = (float)Math.Sin(angle);
		this.Y = (float)Math.Cos(angle);
	}

	// Token: 0x060092D1 RID: 37585 RVA: 0x003A8424 File Offset: 0x003A6624
	public Vector2F(float x, float y)
	{
		this.X = x;
		this.Y = y;
	}

	// Token: 0x17000F22 RID: 3874
	// (get) Token: 0x060092D2 RID: 37586 RVA: 0x003A8434 File Offset: 0x003A6634
	public double Length
	{
		get
		{
			return Math.Sqrt((double)(this.X * this.X + this.Y * this.Y));
		}
	}

	// Token: 0x060092D3 RID: 37587 RVA: 0x003A8457 File Offset: 0x003A6657
	public static double Lengthsquared(Vector2F a)
	{
		return (double)(a.X * a.X + a.Y * a.Y);
	}

	// Token: 0x060092D4 RID: 37588 RVA: 0x003A8475 File Offset: 0x003A6675
	public static Vector2F operator -(Vector2F a, Vector2F b)
	{
		return new Vector2F(a.X - b.X, a.Y - b.Y);
	}

	// Token: 0x060092D5 RID: 37589 RVA: 0x003A8496 File Offset: 0x003A6696
	public static bool operator ==(Vector2F a, Vector2F b)
	{
		return a.X == b.X && a.Y == b.Y;
	}

	// Token: 0x060092D6 RID: 37590 RVA: 0x003A84B6 File Offset: 0x003A66B6
	public static bool operator !=(Vector2F a, Vector2F b)
	{
		return a.X != b.X || a.Y != b.Y;
	}

	// Token: 0x060092D7 RID: 37591 RVA: 0x003A84D9 File Offset: 0x003A66D9
	public override int GetHashCode()
	{
		return this.X.GetHashCode() * this.Y.GetHashCode();
	}

	// Token: 0x060092D8 RID: 37592 RVA: 0x003A8457 File Offset: 0x003A6657
	public double Lengthsquared()
	{
		return (double)(this.X * this.X + this.Y * this.Y);
	}

	// Token: 0x060092D9 RID: 37593 RVA: 0x003A84F2 File Offset: 0x003A66F2
	public override bool Equals(object obj)
	{
		return this == (Vector2F)obj;
	}

	// Token: 0x060092DA RID: 37594 RVA: 0x003A8505 File Offset: 0x003A6705
	public bool Equals(Vector2F other)
	{
		return other.X == this.X && other.Y == this.Y;
	}

	// Token: 0x060092DB RID: 37595 RVA: 0x003A8525 File Offset: 0x003A6725
	public override string ToString()
	{
		return this.X.ToCultureInvariantString() + ", " + this.Y.ToCultureInvariantString();
	}

	// Token: 0x04007050 RID: 28752
	public float Y;

	// Token: 0x04007051 RID: 28753
	public float X;

	// Token: 0x04007052 RID: 28754
	public static readonly Vector2F Zero = new Vector2F(0f, 0f);
}
