using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.XGamingRuntime;
using Unity.XGamingRuntime.Interop;

namespace Platform.XBL
{
	// Token: 0x02001884 RID: 6276
	public static class XblHelpers
	{
		// Token: 0x14000119 RID: 281
		// (add) Token: 0x0600B991 RID: 47505 RVA: 0x0046DD30 File Offset: 0x0046BF30
		// (remove) Token: 0x0600B992 RID: 47506 RVA: 0x0046DD64 File Offset: 0x0046BF64
		public static event XblHelpers.ErrorDelegate OnError;

		// Token: 0x0600B993 RID: 47507 RVA: 0x0046DD98 File Offset: 0x0046BF98
		public static bool Succeeded(int _hresult, string _operationFriendlyName, bool _logToConsole = true, bool _printSuccess = false)
		{
			if (Unity.XGamingRuntime.Interop.HR.SUCCEEDED(_hresult))
			{
				if (_printSuccess && _logToConsole)
				{
					Log.Out("[XBL] Success: " + _operationFriendlyName);
				}
				return true;
			}
			string text;
			if (!XblHelpers.hresultToFriendlyErrorLookup.TryGetValue(_hresult, out text))
			{
				text = _operationFriendlyName + " failed. Error code: " + XblHelpers.GetHRName(_hresult);
			}
			if (_logToConsole)
			{
				Log.Error(string.Format("[XBL] Error: 0x{0:X8} - {1}", _hresult, text));
			}
			XblHelpers.ErrorDelegate onError = XblHelpers.OnError;
			if (onError != null)
			{
				onError(_hresult, _operationFriendlyName, text);
			}
			return false;
		}

		// Token: 0x0600B994 RID: 47508 RVA: 0x0046DE14 File Offset: 0x0046C014
		[PublicizedFrom(EAccessModifier.Private)]
		static XblHelpers()
		{
			XblHelpers.hresultToFriendlyErrorLookup[-2143330041] = "IAP_UNEXPECTED: Does the player you are signed in as have a license for the game? You can get one by downloading your game from the store and purchasing it first. If you can't find your game in the store, have you published it in Partner Center?";
			XblHelpers.hresultToFriendlyErrorLookup[-2015035361] = "Missing Game Config";
		}

		// Token: 0x0600B995 RID: 47509 RVA: 0x0046DED4 File Offset: 0x0046C0D4
		public static string GetHRName(int hr)
		{
			string result;
			if (!XblHelpers.s_hrToName.TryGetValue(hr, out result))
			{
				return "UNKNOWN";
			}
			return result;
		}

		// Token: 0x0600B996 RID: 47510 RVA: 0x0046DEF8 File Offset: 0x0046C0F8
		public static void LogHR(int hr, string identifier, bool failWarn = false)
		{
			bool flag = Unity.XGamingRuntime.Interop.HR.SUCCEEDED(hr);
			string text = flag ? "SUCCEEDED" : "FAILED";
			string txt = string.Format("[HResult] {0} (0x{1:X8} = {2}) {3}", new object[]
			{
				text,
				hr,
				XblHelpers.GetHRName(hr),
				identifier
			});
			if (flag)
			{
				Log.Out(txt);
				return;
			}
			if (failWarn)
			{
				Log.Warning(txt);
				return;
			}
			Log.Error(txt);
		}

		// Token: 0x0400912E RID: 37166
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly Dictionary<int, string> hresultToFriendlyErrorLookup = new Dictionary<int, string>();

		// Token: 0x0400912F RID: 37167
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly IReadOnlyDictionary<int, string> s_hrToName = (from f in new Type[]
		{
			typeof(Unity.XGamingRuntime.Interop.HR),
			typeof(Unity.XGamingRuntime.HR),
			typeof(HREx)
		}.SelectMany((Type t) => t.GetFields(BindingFlags.Static | BindingFlags.Public))
		where f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(int)
		select f).ToDictionary((FieldInfo f) => (int)f.GetValue(null), (FieldInfo f) => f.Name);

		// Token: 0x02001885 RID: 6277
		// (Invoke) Token: 0x0600B998 RID: 47512
		public delegate void ErrorDelegate(int _hresult, string _operationFriendlyName, string _errorMessage);
	}
}
