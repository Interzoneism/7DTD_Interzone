using System;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class TurnTable : MonoBehaviour
{
	// Token: 0x06000091 RID: 145 RVA: 0x0000953C File Offset: 0x0000773C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this._pingPong)
		{
			float t = Mathf.SmoothStep(0f, 1f, Mathf.PingPong(Time.time * this._rotationSpeed, 1f));
			float y = Mathf.Lerp(this._pingPongDegreeOffset - this._pingPongDegreeSpan / 2f, this._pingPongDegreeOffset + this._pingPongDegreeSpan / 2f, t);
			base.transform.localRotation = Quaternion.Euler(0f, y, 0f);
			return;
		}
		base.transform.localRotation *= Quaternion.Euler(0f, this._rotationSpeed * 180f * Time.deltaTime, 0f);
	}

	// Token: 0x040000D1 RID: 209
	public float _rotationSpeed = 1f;

	// Token: 0x040000D2 RID: 210
	public bool _pingPong;

	// Token: 0x040000D3 RID: 211
	public float _pingPongDegreeSpan = 90f;

	// Token: 0x040000D4 RID: 212
	public float _pingPongDegreeOffset;
}
