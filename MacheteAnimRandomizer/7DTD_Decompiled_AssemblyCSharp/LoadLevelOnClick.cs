using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200004C RID: 76
[AddComponentMenu("NGUI/Examples/Load Level On Click")]
public class LoadLevelOnClick : MonoBehaviour
{
	// Token: 0x06000181 RID: 385 RVA: 0x0000EBC6 File Offset: 0x0000CDC6
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnClick()
	{
		if (!string.IsNullOrEmpty(this.levelName))
		{
			SceneManager.LoadScene(this.levelName);
		}
	}

	// Token: 0x0400022F RID: 559
	public string levelName;
}
