using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x020012B2 RID: 4786
[Preserve]
public static class vp_GlobalEventReturn<T, U, V, R>
{
	// Token: 0x06009578 RID: 38264 RVA: 0x003B8A88 File Offset: 0x003B6C88
	[Preserve]
	public static void Register(string name, vp_GlobalCallbackReturn<T, U, V, R> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallbackReturn<T, U, V, R>> list = (List<vp_GlobalCallbackReturn<T, U, V, R>>)vp_GlobalEventReturn<T, U, V, R>.m_Callbacks[name];
		if (list == null)
		{
			list = new List<vp_GlobalCallbackReturn<T, U, V, R>>();
			vp_GlobalEventReturn<T, U, V, R>.m_Callbacks.Add(name, list);
		}
		list.Add(callback);
	}

	// Token: 0x06009579 RID: 38265 RVA: 0x003B8AE4 File Offset: 0x003B6CE4
	[Preserve]
	public static void Unregister(string name, vp_GlobalCallbackReturn<T, U, V, R> callback)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (callback == null)
		{
			throw new ArgumentNullException("callback");
		}
		List<vp_GlobalCallbackReturn<T, U, V, R>> list = (List<vp_GlobalCallbackReturn<T, U, V, R>>)vp_GlobalEventReturn<T, U, V, R>.m_Callbacks[name];
		if (list != null)
		{
			list.Remove(callback);
			return;
		}
		throw vp_GlobalEventInternal.ShowUnregisterException(name);
	}

	// Token: 0x0600957A RID: 38266 RVA: 0x003B8B35 File Offset: 0x003B6D35
	public static R Send(string name, T arg1, U arg2, V arg3)
	{
		return vp_GlobalEventReturn<T, U, V, R>.Send(name, arg1, arg2, arg3, vp_GlobalEventMode.DONT_REQUIRE_LISTENER);
	}

	// Token: 0x0600957B RID: 38267 RVA: 0x003B8B44 File Offset: 0x003B6D44
	public static R Send(string name, T arg1, U arg2, V arg3, vp_GlobalEventMode mode)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentNullException("name");
		}
		if (arg1 == null)
		{
			throw new ArgumentNullException("arg1");
		}
		if (arg2 == null)
		{
			throw new ArgumentNullException("arg2");
		}
		if (arg3 == null)
		{
			throw new ArgumentNullException("arg3");
		}
		List<vp_GlobalCallbackReturn<T, U, V, R>> list = (List<vp_GlobalCallbackReturn<T, U, V, R>>)vp_GlobalEventReturn<T, U, V, R>.m_Callbacks[name];
		if (list != null)
		{
			R result = default(R);
			foreach (vp_GlobalCallbackReturn<T, U, V, R> vp_GlobalCallbackReturn in list)
			{
				result = vp_GlobalCallbackReturn(arg1, arg2, arg3);
			}
			return result;
		}
		if (mode == vp_GlobalEventMode.REQUIRE_LISTENER)
		{
			throw vp_GlobalEventInternal.ShowSendException(name);
		}
		return default(R);
	}

	// Token: 0x040071FF RID: 29183
	[PublicizedFrom(EAccessModifier.Private)]
	public static Hashtable m_Callbacks = vp_GlobalEventInternal.Callbacks;
}
