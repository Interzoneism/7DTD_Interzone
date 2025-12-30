using System;
using UnityEngine;

// Token: 0x020012C2 RID: 4802
public static class vp_TargetEventReturn<R>
{
	// Token: 0x060095C4 RID: 38340 RVA: 0x003B9E0C File Offset: 0x003B800C
	public static void Register(object target, string eventName, Func<R> callback)
	{
		vp_TargetEventHandler.Register(target, eventName, callback, 4);
	}

	// Token: 0x060095C5 RID: 38341 RVA: 0x003B9B1B File Offset: 0x003B7D1B
	public static void Unregister(object target, string eventName, Func<R> callback)
	{
		vp_TargetEventHandler.Unregister(target, eventName, callback);
	}

	// Token: 0x060095C6 RID: 38342 RVA: 0x003B9B25 File Offset: 0x003B7D25
	public static void Unregister(object target)
	{
		vp_TargetEventHandler.Unregister(target, null, null);
	}

	// Token: 0x060095C7 RID: 38343 RVA: 0x003B9B2F File Offset: 0x003B7D2F
	public static void Unregister(Component component)
	{
		vp_TargetEventHandler.Unregister(component);
	}

	// Token: 0x060095C8 RID: 38344 RVA: 0x003B9E18 File Offset: 0x003B8018
	public static R Send(object target, string eventName, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		R result;
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, false, 4, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				result = ((Func<R>)callback)();
			}
			catch
			{
				eventName += "_";
				continue;
			}
			return result;
		}
		vp_TargetEventHandler.OnNoReceiver(eventName, options);
		result = default(R);
		return result;
	}

	// Token: 0x060095C9 RID: 38345 RVA: 0x003B9E74 File Offset: 0x003B8074
	public static R SendUpwards(Component target, string eventName, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		R result;
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, true, 4, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				result = ((Func<R>)callback)();
			}
			catch
			{
				eventName += "_";
				continue;
			}
			return result;
		}
		vp_TargetEventHandler.OnNoReceiver(eventName, options);
		result = default(R);
		return result;
	}
}
