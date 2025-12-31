using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000046 RID: 70
[RequireComponent(typeof(UITexture))]
public class DownloadTexture : MonoBehaviour
{
	// Token: 0x06000168 RID: 360 RVA: 0x0000E7A8 File Offset: 0x0000C9A8
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator Start()
	{
		UnityWebRequest www = UnityWebRequest.Get(this.url);
		yield return www.SendWebRequest();
		this.mTex = DownloadHandlerTexture.GetContent(www);
		if (this.mTex != null)
		{
			UITexture component = base.GetComponent<UITexture>();
			component.mainTexture = this.mTex;
			if (this.pixelPerfect)
			{
				component.MakePixelPerfect();
			}
		}
		www.Dispose();
		yield break;
	}

	// Token: 0x06000169 RID: 361 RVA: 0x0000E7B7 File Offset: 0x0000C9B7
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDestroy()
	{
		if (this.mTex != null)
		{
			UnityEngine.Object.Destroy(this.mTex);
		}
	}

	// Token: 0x0400021B RID: 539
	public string url = "http://www.yourwebsite.com/logo.png";

	// Token: 0x0400021C RID: 540
	public bool pixelPerfect = true;

	// Token: 0x0400021D RID: 541
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Texture2D mTex;
}
