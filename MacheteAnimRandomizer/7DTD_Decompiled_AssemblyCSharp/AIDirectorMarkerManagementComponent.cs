using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020003C1 RID: 961
[Preserve]
public class AIDirectorMarkerManagementComponent : AIDirectorComponent
{
	// Token: 0x06001D51 RID: 7505 RVA: 0x000B753C File Offset: 0x000B573C
	public override void Tick(double _dt)
	{
		base.Tick(_dt);
		this.TickMarkers(_dt);
	}

	// Token: 0x06001D52 RID: 7506 RVA: 0x000B754C File Offset: 0x000B574C
	[PublicizedFrom(EAccessModifier.Private)]
	public void TickMarkers(double _dt)
	{
		for (int i = this.markers.Count - 1; i >= 0; i--)
		{
			IAIDirectorMarker iaidirectorMarker = this.markers[i];
			iaidirectorMarker.Tick(_dt);
			if (iaidirectorMarker.TimeToLive <= 0f || (iaidirectorMarker.Player != null && iaidirectorMarker.Player.IsDead()))
			{
				this.markers.RemoveAt(i);
				iaidirectorMarker.Release();
			}
		}
	}

	// Token: 0x06001D53 RID: 7507 RVA: 0x000B75C0 File Offset: 0x000B57C0
	public IAIDirectorMarker FindBestMarker(Vector3 _pos, ref double _inOutIntensity)
	{
		IAIDirectorMarker result = null;
		int num = -1;
		for (int i = this.markers.Count - 1; i >= 0; i--)
		{
			IAIDirectorMarker iaidirectorMarker = this.markers[i];
			if (iaidirectorMarker.TimeToLive > 0f)
			{
				double num2 = iaidirectorMarker.IntensityForPosition(_pos);
				if (num2 > 0.0 && iaidirectorMarker.Priority > num)
				{
					num = iaidirectorMarker.Priority;
					result = iaidirectorMarker;
					_inOutIntensity = num2;
				}
			}
		}
		return result;
	}

	// Token: 0x04001417 RID: 5143
	[PublicizedFrom(EAccessModifier.Private)]
	public List<IAIDirectorMarker> markers = new List<IAIDirectorMarker>(256);
}
