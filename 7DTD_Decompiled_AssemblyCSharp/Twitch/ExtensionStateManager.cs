using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Twitch
{
	// Token: 0x02001517 RID: 5399
	public class ExtensionStateManager
	{
		// Token: 0x0600A6B2 RID: 42674 RVA: 0x0041D2F0 File Offset: 0x0041B4F0
		public void Init()
		{
			this.userId = TwitchManager.Current.Authentication.userID;
			this.gettingJWT = true;
			GameManager.Instance.StartCoroutine(this.GetJWT(TwitchManager.Current.Authentication.oauth.Substring(6)));
			this.ecm = new ExtensionConfigManager();
			this.ecm.Init();
			this.epm = new ExtensionPubSubManager();
		}

		// Token: 0x0600A6B3 RID: 42675 RVA: 0x0041D360 File Offset: 0x0041B560
		public void OnPartyChanged()
		{
			ExtensionConfigManager extensionConfigManager = this.ecm;
			if (extensionConfigManager == null)
			{
				return;
			}
			extensionConfigManager.OnPartyChanged();
		}

		// Token: 0x0600A6B4 RID: 42676 RVA: 0x0041D372 File Offset: 0x0041B572
		public void PushUserBalance(ValueTuple<string, int> userBalance)
		{
			ExtensionPubSubManager extensionPubSubManager = this.epm;
			if (extensionPubSubManager == null)
			{
				return;
			}
			extensionPubSubManager.PushUserBalance(userBalance);
		}

		// Token: 0x0600A6B5 RID: 42677 RVA: 0x0041D385 File Offset: 0x0041B585
		public void PushViewerChatState(string id, bool hasChatted)
		{
			ExtensionPubSubManager extensionPubSubManager = this.epm;
			if (extensionPubSubManager == null)
			{
				return;
			}
			extensionPubSubManager.PushViewerChatState(id, hasChatted);
		}

		// Token: 0x0600A6B6 RID: 42678 RVA: 0x0041D399 File Offset: 0x0041B599
		public bool CanUseBitCommands()
		{
			return this.ecm.CanUseBitCommands();
		}

		// Token: 0x0600A6B7 RID: 42679 RVA: 0x0041D3A8 File Offset: 0x0041B5A8
		public void Update()
		{
			if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > this.jwtRefreshTime && !this.gettingJWT)
			{
				this.gettingJWT = true;
				GameManager.Instance.StartCoroutine(this.GetJWT(TwitchManager.Current.Authentication.oauth.Substring(6)));
			}
			if (this.jwt != string.Empty && Time.realtimeSinceStartup - this.lastUpdate >= 1f)
			{
				this.epm.Update(this.ecm.UpdatedConfig());
				this.lastUpdate = Time.realtimeSinceStartup;
			}
		}

		// Token: 0x0600A6B8 RID: 42680 RVA: 0x0041D445 File Offset: 0x0041B645
		public void RetrieveJWT()
		{
			GameManager.Instance.StartCoroutine(this.GetJWT(TwitchManager.Current.Authentication.oauth.Substring(6)));
		}

		// Token: 0x0600A6B9 RID: 42681 RVA: 0x0041D46D File Offset: 0x0041B66D
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator GetJWT(string token)
		{
			using (UnityWebRequest req = UnityWebRequest.Get("https://2v3d0ewjcg.execute-api.us-east-1.amazonaws.com/prod/jwt/broadcaster"))
			{
				req.SetRequestHeader("Authorization", this.userId + " " + token);
				yield return req.SendWebRequest();
				if (req.result != UnityWebRequest.Result.Success)
				{
					Log.Warning(string.Format("Could not retrieve JWT: {0}", req.result));
				}
				else
				{
					try
					{
						JObject jobject = JObject.Parse(req.downloadHandler.text);
						if (jobject != null)
						{
							JToken jtoken;
							if (jobject.TryGetValue("token", out jtoken))
							{
								this.jwt = jtoken.ToString();
								this.epm.SetJWT(this.jwt);
								Log.Out("received jwt");
							}
							else
							{
								Log.Warning("Could not parse JWT in message body");
							}
							JToken jtoken2;
							if (jobject.TryGetValue("refreshTime", out jtoken2))
							{
								this.jwtRefreshTime = long.Parse(jtoken2.ToString());
								Log.Out(string.Format("will refresh jwt at {0}", this.jwtRefreshTime));
							}
						}
					}
					catch (Exception ex)
					{
						Log.Warning(ex.Message);
					}
				}
			}
			UnityWebRequest req = null;
			this.gettingJWT = false;
			yield break;
			yield break;
		}

		// Token: 0x0600A6BA RID: 42682 RVA: 0x0041D483 File Offset: 0x0041B683
		public void Cleanup()
		{
			this.ecm.Cleanup();
			this.ecm = null;
			this.epm = null;
		}

		// Token: 0x04008100 RID: 33024
		[PublicizedFrom(EAccessModifier.Private)]
		public string userId;

		// Token: 0x04008101 RID: 33025
		[PublicizedFrom(EAccessModifier.Private)]
		public float lastUpdate = Time.realtimeSinceStartup;

		// Token: 0x04008102 RID: 33026
		[PublicizedFrom(EAccessModifier.Private)]
		public ExtensionConfigManager ecm;

		// Token: 0x04008103 RID: 33027
		[PublicizedFrom(EAccessModifier.Private)]
		public ExtensionPubSubManager epm;

		// Token: 0x04008104 RID: 33028
		[PublicizedFrom(EAccessModifier.Private)]
		public string jwt = string.Empty;

		// Token: 0x04008105 RID: 33029
		[PublicizedFrom(EAccessModifier.Private)]
		public long jwtRefreshTime;

		// Token: 0x04008106 RID: 33030
		[PublicizedFrom(EAccessModifier.Private)]
		public bool gettingJWT;
	}
}
