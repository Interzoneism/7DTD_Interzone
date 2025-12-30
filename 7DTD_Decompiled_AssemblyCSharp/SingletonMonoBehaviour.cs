using System;
using UnityEngine;

// Token: 0x02001213 RID: 4627
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
	// Token: 0x0600905C RID: 36956 RVA: 0x0039927C File Offset: 0x0039747C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		if (this.IsPersistant)
		{
			if (!SingletonMonoBehaviour<T>.Instance)
			{
				SingletonMonoBehaviour<T>.Instance = (T)((object)this);
				SingletonMonoBehaviour<T>.Instance.singletonCreated();
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else
		{
			SingletonMonoBehaviour<T>.Instance = (T)((object)this);
			SingletonMonoBehaviour<T>.Instance.singletonCreated();
		}
		SingletonMonoBehaviour<T>.Instance.singletonAwake();
	}

	// Token: 0x0600905D RID: 36957 RVA: 0x003992FF File Offset: 0x003974FF
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDestroy()
	{
		if (this == SingletonMonoBehaviour<T>.Instance)
		{
			this.singletonDestroy();
		}
	}

	// Token: 0x0600905E RID: 36958 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void singletonAwake()
	{
	}

	// Token: 0x0600905F RID: 36959 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void singletonCreated()
	{
	}

	// Token: 0x06009060 RID: 36960 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void singletonDestroy()
	{
	}

	// Token: 0x04006F42 RID: 28482
	public static T Instance;

	// Token: 0x04006F43 RID: 28483
	public bool IsPersistant;
}
