using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Twitch
{
	// Token: 0x02001556 RID: 5462
	public class TwitchChannelPointEventEntry : BaseTwitchEventEntry
	{
		// Token: 0x0600A7F6 RID: 42998 RVA: 0x00422A9A File Offset: 0x00420C9A
		public override bool IsValid(int amount = -1, string name = "", TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.SubTierTypes.Any)
		{
			return this.ChannelPointTitle == name;
		}

		// Token: 0x0600A7F7 RID: 42999 RVA: 0x00422AA8 File Offset: 0x00420CA8
		public TwitchChannelPointEventEntry.CreateCustomReward SetupRewardEntry(string channelID)
		{
			return new TwitchChannelPointEventEntry.CreateCustomReward
			{
				broadcaster_id = channelID,
				title = this.ChannelPointTitle,
				cost = this.Cost,
				is_max_per_user_per_stream_enabled = (this.MaxPerUserPerStream > 0),
				max_per_user_per_stream = this.MaxPerUserPerStream,
				is_max_per_stream_enabled = (this.MaxPerStream > 0),
				max_per_stream = this.MaxPerStream,
				is_global_cooldown_enabled = (this.GlobalCooldown > 0),
				global_cooldown_seconds = this.GlobalCooldown
			};
		}

		// Token: 0x0600A7F8 RID: 43000 RVA: 0x00422B2A File Offset: 0x00420D2A
		public static IEnumerator CreateCustomRewardPost(TwitchChannelPointEventEntry.CreateCustomReward _rd, Action<string> _onSucess, Action<string> _onFail)
		{
			yield return new WaitUntil(() => TwitchManager.Current.Authentication != null && TwitchManager.Current.Authentication.oauth != "" && TwitchManager.Current.Authentication.userID != "");
			Log.Out("creating Custom reward on: " + TwitchManager.Current.Authentication.userID);
			string uri = "https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id=" + TwitchManager.Current.Authentication.userID;
			string bodyData = JsonUtility.ToJson(_rd);
			using (UnityWebRequest req = UnityWebRequest.Put(uri, bodyData))
			{
				req.method = "POST";
				req.SetRequestHeader("Authorization", "Bearer " + TwitchManager.Current.Authentication.oauth.Substring(6));
				req.SetRequestHeader("Client-Id", TwitchAuthentication.client_id);
				req.SetRequestHeader("Content-Type", "application/json");
				yield return req.SendWebRequest();
				if (req.result == UnityWebRequest.Result.Success)
				{
					Log.Out("sucessfully created Custom Channel Points Reward");
					_onSucess(req.downloadHandler.text);
				}
				else
				{
					TwitchChannelPointEventEntry.ErrorResponse errorResponse = JsonConvert.DeserializeObject<TwitchChannelPointEventEntry.ErrorResponse>(req.downloadHandler.text);
					if (errorResponse != null)
					{
						_onFail("response code: " + errorResponse.status + "\nmessage: " + errorResponse.message);
					}
					else
					{
						_onFail("Something went wrong. Please Try again.");
					}
				}
			}
			UnityWebRequest req = null;
			yield break;
			yield break;
		}

		// Token: 0x0600A7F9 RID: 43001 RVA: 0x00422B47 File Offset: 0x00420D47
		public static IEnumerator DeleteCustomRewardsDelete(string id, Action<string> _onSucess, Action<string> _onFail)
		{
			string uri = string.Format("https://api.twitch.tv/helix/channel_points/custom_rewards?broadcaster_id={0}&id={1}", TwitchManager.Current.Authentication.userID, id);
			using (UnityWebRequest req = UnityWebRequest.Delete(uri))
			{
				req.method = "DELETE";
				req.SetRequestHeader("Authorization", "Bearer " + TwitchManager.Current.Authentication.oauth.Substring(6));
				req.SetRequestHeader("Client-Id", TwitchAuthentication.client_id);
				yield return req.SendWebRequest();
				if (req.result == UnityWebRequest.Result.Success)
				{
					_onSucess("Success");
				}
				else
				{
					Debug.Log(string.Format("response code: {0}", req.responseCode));
					if (req.responseCode == 404L)
					{
						_onSucess("Not Found");
					}
					else
					{
						TwitchChannelPointEventEntry.ErrorResponse errorResponse = JsonConvert.DeserializeObject<TwitchChannelPointEventEntry.ErrorResponse>(req.downloadHandler.text);
						if (errorResponse != null)
						{
							_onFail(errorResponse.message);
						}
						else
						{
							_onFail("Something went wrong. Please Try again.");
						}
					}
				}
			}
			UnityWebRequest req = null;
			yield break;
			yield break;
		}

		// Token: 0x04008244 RID: 33348
		public string ChannelPointTitle = "";

		// Token: 0x04008245 RID: 33349
		public int Cost = 1000;

		// Token: 0x04008246 RID: 33350
		public int MaxPerUserPerStream;

		// Token: 0x04008247 RID: 33351
		public int MaxPerStream;

		// Token: 0x04008248 RID: 33352
		public int GlobalCooldown;

		// Token: 0x04008249 RID: 33353
		public string ChannelPointID = "";

		// Token: 0x0400824A RID: 33354
		public bool AutoCreate = true;

		// Token: 0x02001557 RID: 5463
		[Serializable]
		public class CreateCustomRewards
		{
			// Token: 0x0400824B RID: 33355
			public List<TwitchChannelPointEventEntry.CreateCustomReward> data = new List<TwitchChannelPointEventEntry.CreateCustomReward>();
		}

		// Token: 0x02001558 RID: 5464
		[Serializable]
		public class CreateCustomReward
		{
			// Token: 0x0400824C RID: 33356
			public string broadcaster_id;

			// Token: 0x0400824D RID: 33357
			public string title;

			// Token: 0x0400824E RID: 33358
			public string background_color = "#F13030";

			// Token: 0x0400824F RID: 33359
			public int cost;

			// Token: 0x04008250 RID: 33360
			public int max_per_user_per_stream;

			// Token: 0x04008251 RID: 33361
			public bool is_max_per_user_per_stream_enabled;

			// Token: 0x04008252 RID: 33362
			public int max_per_stream;

			// Token: 0x04008253 RID: 33363
			public bool is_max_per_stream_enabled;

			// Token: 0x04008254 RID: 33364
			public int global_cooldown_seconds;

			// Token: 0x04008255 RID: 33365
			public bool is_global_cooldown_enabled;
		}

		// Token: 0x02001559 RID: 5465
		[Serializable]
		public class CreateCustomRewardResponses
		{
			// Token: 0x04008256 RID: 33366
			public List<TwitchChannelPointEventEntry.CreateCustomRewardResponse> data;
		}

		// Token: 0x0200155A RID: 5466
		[Serializable]
		public class CreateCustomRewardResponse
		{
			// Token: 0x04008257 RID: 33367
			public string id;

			// Token: 0x04008258 RID: 33368
			public string title;
		}

		// Token: 0x0200155B RID: 5467
		[Serializable]
		public class ErrorResponse
		{
			// Token: 0x04008259 RID: 33369
			public string status;

			// Token: 0x0400825A RID: 33370
			public string message;
		}
	}
}
