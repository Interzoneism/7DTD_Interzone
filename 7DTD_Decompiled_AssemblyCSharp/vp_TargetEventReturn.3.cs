using System;
using UnityEngine;

// Token: 0x020012C4 RID: 4804
public static class vp_TargetEventReturn<T, U, R>
{
	// Token: 0x060095CF RID: 38351 RVA: 0x003B9F9C File Offset: 0x003B819C
	public static void Register(object target, string eventName, Func<T, U, R> callback)
	{
		vp_TargetEventHandler.Register(target, eventName, callback, 6);
	}

	// Token: 0x060095D0 RID: 38352 RVA: 0x003B9B1B File Offset: 0x003B7D1B
	public static void Unregister(object target, string eventName, Func<T, U, R> callback)
	{
		vp_TargetEventHandler.Unregister(target, eventName, callback);
	}

	// Token: 0x060095D1 RID: 38353 RVA: 0x003B9B25 File Offset: 0x003B7D25
	public static void Unregister(object target)
	{
		vp_TargetEventHandler.Unregister(target, null, null);
	}

	// Token: 0x060095D2 RID: 38354 RVA: 0x003B9FA8 File Offset: 0x003B81A8
	public static R Send(object target, string eventName, T arg1, U arg2, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		R result;
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, false, 6, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				result = ((Func<T, U, R>)callback)(arg1, arg2);
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

	// Token: 0x060095D3 RID: 38355 RVA: 0x003BA008 File Offset: 0x003B8208
	public static R SendUpwards(Component target, string eventName, T arg1, U arg2, vp_TargetEventOptions options = vp_TargetEventOptions.DontRequireReceiver)
	{
		R result;
		for (;;)
		{
			Delegate callback = vp_TargetEventHandler.GetCallback(target, eventName, true, 6, options);
			if (callback == null)
			{
				break;
			}
			try
			{
				result = ((Func<T, U, R>)callback)(arg1, arg2);
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
