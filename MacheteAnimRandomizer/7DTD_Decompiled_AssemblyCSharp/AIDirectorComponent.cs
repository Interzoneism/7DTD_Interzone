using System;
using System.IO;

// Token: 0x020003BA RID: 954
public abstract class AIDirectorComponent
{
	// Token: 0x06001D28 RID: 7464 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Connect()
	{
	}

	// Token: 0x06001D29 RID: 7465 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void InitNewGame()
	{
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Tick(double _dt)
	{
	}

	// Token: 0x06001D2B RID: 7467 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Read(BinaryReader _stream, int _version)
	{
	}

	// Token: 0x06001D2C RID: 7468 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Write(BinaryWriter _stream)
	{
	}

	// Token: 0x1700033B RID: 827
	// (get) Token: 0x06001D2D RID: 7469 RVA: 0x000B66AD File Offset: 0x000B48AD
	public GameRandom Random
	{
		get
		{
			return this.Director.random;
		}
	}

	// Token: 0x06001D2E RID: 7470 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public AIDirectorComponent()
	{
	}

	// Token: 0x040013D9 RID: 5081
	public AIDirector Director;
}
