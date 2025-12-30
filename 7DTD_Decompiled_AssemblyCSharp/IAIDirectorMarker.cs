using System;
using UnityEngine;

// Token: 0x020003D6 RID: 982
public interface IAIDirectorMarker
{
	// Token: 0x06001DBF RID: 7615
	void Reference();

	// Token: 0x06001DC0 RID: 7616
	bool Release();

	// Token: 0x06001DC1 RID: 7617
	void Tick(double dt);

	// Token: 0x17000354 RID: 852
	// (get) Token: 0x06001DC2 RID: 7618
	EntityPlayer Player { get; }

	// Token: 0x06001DC3 RID: 7619
	double IntensityForPosition(Vector3 position);

	// Token: 0x17000355 RID: 853
	// (get) Token: 0x06001DC4 RID: 7620
	Vector3 Position { get; }

	// Token: 0x17000356 RID: 854
	// (get) Token: 0x06001DC5 RID: 7621
	Vector3 TargetPosition { get; }

	// Token: 0x17000357 RID: 855
	// (get) Token: 0x06001DC6 RID: 7622
	bool Valid { get; }

	// Token: 0x17000358 RID: 856
	// (get) Token: 0x06001DC7 RID: 7623
	float MaxRadius { get; }

	// Token: 0x17000359 RID: 857
	// (get) Token: 0x06001DC8 RID: 7624
	float Radius { get; }

	// Token: 0x1700035A RID: 858
	// (get) Token: 0x06001DC9 RID: 7625
	float TimeToLive { get; }

	// Token: 0x1700035B RID: 859
	// (get) Token: 0x06001DCA RID: 7626
	float ValidTime { get; }

	// Token: 0x1700035C RID: 860
	// (get) Token: 0x06001DCB RID: 7627
	float Speed { get; }

	// Token: 0x1700035D RID: 861
	// (get) Token: 0x06001DCC RID: 7628
	int Priority { get; }

	// Token: 0x1700035E RID: 862
	// (get) Token: 0x06001DCD RID: 7629
	bool InterruptsNonPlayerAttack { get; }

	// Token: 0x1700035F RID: 863
	// (get) Token: 0x06001DCE RID: 7630
	bool IsDistraction { get; }
}
