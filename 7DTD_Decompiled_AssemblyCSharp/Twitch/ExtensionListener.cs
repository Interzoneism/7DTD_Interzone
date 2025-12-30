using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UniLinq;
using UnityEngine;
using UnityEngine.Networking;

namespace Twitch
{
	// Token: 0x02001501 RID: 5377
	public class ExtensionListener
	{
		// Token: 0x0600A649 RID: 42569 RVA: 0x0041B180 File Offset: 0x00419380
		public ExtensionListener()
		{
			this.listener = new HttpListener();
			this.listener.Prefixes.Add("http://localhost:52775/");
		}

		// Token: 0x0600A64A RID: 42570 RVA: 0x0041B1EC File Offset: 0x004193EC
		public void StopListener()
		{
			this.listener.Stop();
			this.setConfig("offline");
			this.displayName = string.Empty;
			TwitchManager.Current.CommandsChanged -= this.pushConfig;
			TwitchVotingManager votingManager = TwitchManager.Current.VotingManager;
			votingManager.VoteEventEnded = (OnGameEventVoteAction)Delegate.Remove(votingManager.VoteEventEnded, new OnGameEventVoteAction(this.pushConfig));
			TwitchVotingManager votingManager2 = TwitchManager.Current.VotingManager;
			votingManager2.VoteStarted = (OnGameEventVoteAction)Delegate.Remove(votingManager2.VoteStarted, new OnGameEventVoteAction(this.pushConfig));
		}

		// Token: 0x0600A64B RID: 42571 RVA: 0x0041B286 File Offset: 0x00419486
		public void RunListener()
		{
			Task.Run(delegate()
			{
				this.StartListener();
			});
		}

		// Token: 0x0600A64C RID: 42572 RVA: 0x0041B29C File Offset: 0x0041949C
		[PublicizedFrom(EAccessModifier.Private)]
		public void StartListener()
		{
			TwitchManager.Current.CommandsChanged += this.pushConfig;
			TwitchVotingManager votingManager = TwitchManager.Current.VotingManager;
			votingManager.VoteEventEnded = (OnGameEventVoteAction)Delegate.Combine(votingManager.VoteEventEnded, new OnGameEventVoteAction(this.pushConfig));
			TwitchVotingManager votingManager2 = TwitchManager.Current.VotingManager;
			votingManager2.VoteStarted = (OnGameEventVoteAction)Delegate.Combine(votingManager2.VoteStarted, new OnGameEventVoteAction(this.pushConfig));
			this.listener.Start();
			while (this.listener.IsListening)
			{
				HttpListenerContext context = this.listener.GetContext();
				HttpListenerRequest request = context.Request;
				Log.Out(request.Url.LocalPath);
				HttpListenerResponse response = context.Response;
				response.AddHeader("Access-Control-Allow-Origin", "*");
				response.ContentType = "application/json";
				string localPath = request.Url.LocalPath;
				if (!(localPath == "/command"))
				{
					if (!(localPath == "/connect"))
					{
						if (localPath == "/bitCmdVerification")
						{
							string actionName = request.QueryString["command"];
							string text = request.QueryString["userId"];
							response.ContentType = "application/json";
							string s = JsonConvert.SerializeObject(new ExtensionListener.BitCmdVerifyResponse
							{
								canSend = TwitchManager.Current.IsActionAvailable(actionName),
								userId = text
							});
							byte[] bytes = Encoding.ASCII.GetBytes(s);
							response.ContentLength64 = (long)bytes.Length;
							response.OutputStream.Write(bytes, 0, bytes.Length);
						}
					}
					else
					{
						if (request.HttpMethod == "GET")
						{
							this.JWT = request.QueryString["token"];
							this.userId = request.QueryString["userId"];
							this.opaqueId = request.QueryString["opaqueId"];
							Log.Out("TOKEN: " + this.JWT);
							this.pushConfigChanges = (this.isConnected = (this.JWT != "" && this.userId != "" && this.opaqueId != ""));
							if (!this.pushConfigChanges)
							{
								Log.Warning("Query string missing from connection call");
							}
						}
						response.ContentLength64 = 0L;
					}
				}
				else
				{
					if (request.HttpMethod == "POST")
					{
						this.commands.Enqueue(new TwitchExtensionCommand(request));
					}
					response.ContentLength64 = 0L;
				}
				response.OutputStream.Close();
			}
		}

		// Token: 0x0600A64D RID: 42573 RVA: 0x0041B53E File Offset: 0x0041973E
		public void OnPartyChanged()
		{
			this.pushConfigChanges = true;
		}

		// Token: 0x0600A64E RID: 42574 RVA: 0x0041B547 File Offset: 0x00419747
		public bool HasCommand()
		{
			return this.commands.Count > 0;
		}

		// Token: 0x0600A64F RID: 42575 RVA: 0x0041B558 File Offset: 0x00419758
		public TwitchExtensionCommand GetCommand()
		{
			Queue<TwitchExtensionCommand> obj = this.commands;
			TwitchExtensionCommand result;
			lock (obj)
			{
				result = this.commands.Dequeue();
			}
			return result;
		}

		// Token: 0x0600A650 RID: 42576 RVA: 0x0041B5A0 File Offset: 0x004197A0
		public void Update()
		{
			if (!this.isConnected)
			{
				return;
			}
			if (this.pushConfigChanges || Time.time - this.lastUpdate > 15f)
			{
				this.UpdateCooldown();
				this.pushConfigChanges = false;
				this.lastUpdate = Time.time;
			}
		}

		// Token: 0x0600A651 RID: 42577 RVA: 0x0041B5DE File Offset: 0x004197DE
		public void UpdateCooldown()
		{
			if (!this.JWT.Equals(string.Empty))
			{
				this.setConfig(this.GetPubSubStatus());
				this.lastUpdate = Time.time;
			}
		}

		// Token: 0x0600A652 RID: 42578 RVA: 0x0041B60C File Offset: 0x0041980C
		[PublicizedFrom(EAccessModifier.Private)]
		public string GetPubSubStatus()
		{
			TwitchManager twitchManager = TwitchManager.Current;
			if (twitchManager.IsVoting || (twitchManager.LocalPlayer != null && twitchManager.LocalPlayer.TwitchVoteLock == TwitchVoteLockTypes.ActionsLocked))
			{
				return "full";
			}
			switch (TwitchManager.Current.CooldownType)
			{
			case TwitchManager.CooldownTypes.Startup:
			case TwitchManager.CooldownTypes.Time:
			case TwitchManager.CooldownTypes.BloodMoonDisabled:
			case TwitchManager.CooldownTypes.QuestDisabled:
				return "full";
			case TwitchManager.CooldownTypes.MaxReached:
			case TwitchManager.CooldownTypes.BloodMoonCooldown:
			case TwitchManager.CooldownTypes.QuestCooldown:
				return "regular";
			case TwitchManager.CooldownTypes.MaxReachedWaiting:
			case TwitchManager.CooldownTypes.SafeCooldown:
			case TwitchManager.CooldownTypes.SafeCooldownExit:
				return "wait";
			}
			return "online";
		}

		// Token: 0x0600A653 RID: 42579 RVA: 0x0041B53E File Offset: 0x0041973E
		[PublicizedFrom(EAccessModifier.Private)]
		public void pushConfig()
		{
			this.pushConfigChanges = true;
		}

		// Token: 0x0600A654 RID: 42580 RVA: 0x0041B69F File Offset: 0x0041989F
		[PublicizedFrom(EAccessModifier.Private)]
		public void setConfig(string status)
		{
			GameManager.Instance.StartCoroutine(this.pushConfig(status, (status != "offline") ? this.getCommands() : string.Empty));
		}

		// Token: 0x0600A655 RID: 42581 RVA: 0x0041B6D0 File Offset: 0x004198D0
		[PublicizedFrom(EAccessModifier.Private)]
		public string getCommands()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.tempCommandList.Clear();
			using (Dictionary<string, TwitchAction>.ValueCollection.Enumerator enumerator = TwitchManager.Current.AvailableCommands.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TwitchAction action = enumerator.Current;
					if (action.HasExtraConditions())
					{
						if (MathUtils.Min(TwitchActionManager.Current.CategoryList.FindIndex((TwitchActionManager.ActionCategory c) => c.Name == action.CategoryNames.Last<string>()), 6) == -1)
						{
							Log.Warning("no type index found for " + action.Name);
						}
						else if (action.PointType == TwitchAction.PointTypes.Bits)
						{
							if (this.broadcaster_type == "partner" || this.broadcaster_type == "affiliate")
							{
								this.tempCommandList.Add(action);
							}
						}
						else
						{
							this.tempCommandList.Add(action);
						}
					}
				}
			}
			this.tempCommandList = (from c in this.tempCommandList
			orderby c.PointType, c.Command
			select c).ToList<TwitchAction>();
			for (int i = 0; i < this.tempCommandList.Count; i++)
			{
				TwitchAction action = this.tempCommandList[i];
				string text = "";
				int num = TwitchActionManager.Current.CategoryList.FindIndex((TwitchActionManager.ActionCategory c) => c.Name == action.CategoryNames.Last<string>());
				num = MathUtils.Min(num, 6);
				switch (action.PointType)
				{
				case TwitchAction.PointTypes.PP:
					if (action.WaitingBlocked)
					{
						text = "*";
					}
					else if (action.CooldownBlocked)
					{
						text = "&";
					}
					else
					{
						text = "!";
					}
					break;
				case TwitchAction.PointTypes.SP:
					if (action.WaitingBlocked)
					{
						text = "+";
					}
					else if (action.CooldownBlocked)
					{
						text = "(";
					}
					else
					{
						text = "#";
					}
					break;
				case TwitchAction.PointTypes.Bits:
					if (action.WaitingBlocked)
					{
						text = "-";
					}
					else if (action.CooldownBlocked)
					{
						text = ")";
					}
					else
					{
						text = "$";
					}
					break;
				}
				if (!(text == ""))
				{
					stringBuilder.Append(num);
					string text2 = action.Command.Replace("#", string.Empty);
					if (action.IsPositive)
					{
						text2 = text2.ToUpper();
					}
					stringBuilder.Append(text2);
					stringBuilder.Append(text);
					stringBuilder.Append(action.CurrentCost);
					stringBuilder.Append(",");
				}
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600A656 RID: 42582 RVA: 0x0041BA14 File Offset: 0x00419C14
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator pushConfig(string status, string cmds)
		{
			if (this.displayName == string.Empty)
			{
				using (UnityWebRequest req = UnityWebRequest.Get("https://api.twitch.tv/helix/users?id=" + this.userId))
				{
					req.SetRequestHeader("Content-Type", "application/json");
					req.SetRequestHeader("Client-Id", TwitchAuthentication.client_id);
					req.SetRequestHeader("Authorization", "Bearer " + TwitchManager.Current.Authentication.oauth.Substring(6));
					yield return req.SendWebRequest();
					if (req.result != UnityWebRequest.Result.Success)
					{
						Log.Warning("Could not get data from Twitch 'User' endpoint");
					}
					else
					{
						TwitchUserDataContainer twitchUserDataContainer = JsonConvert.DeserializeObject<TwitchUserDataContainer>(req.downloadHandler.text);
						this.displayName = twitchUserDataContainer.data[0].display_name;
						TwitchManager.Current.BroadcasterType = (this.broadcaster_type = twitchUserDataContainer.data[0].broadcaster_type);
					}
				}
				UnityWebRequest req = null;
				using (UnityWebRequest req = UnityWebRequest.Get("https://2v3d0ewjcg.execute-api.us-east-1.amazonaws.com/prod/allowlist?displayName=" + this.displayName))
				{
					yield return req.SendWebRequest();
					if (req.result != UnityWebRequest.Result.Success)
					{
						Log.Warning("InBeta Check Failed: " + req.downloadHandler.text);
					}
					else
					{
						ExtensionListener.InBetaResponse inBetaResponse = JsonConvert.DeserializeObject<ExtensionListener.InBetaResponse>(req.downloadHandler.text);
						this.inBeta = inBetaResponse.inBeta;
						Log.Out(string.Format("inBeta: {0}", this.inBeta));
					}
				}
				req = null;
			}
			if (this.inBeta)
			{
				List<string> list = (from c in TwitchActionManager.Current.CategoryList.GetRange(1, 5)
				select c.DisplayName).ToList<string>();
				list.Add("Other");
				List<string> players = new List<string>();
				if (TwitchManager.Current.LocalPlayer != null && TwitchManager.Current.LocalPlayer.Party != null)
				{
					foreach (EntityPlayer entityPlayer in TwitchManager.Current.LocalPlayer.Party.MemberList)
					{
						if (!(entityPlayer is EntityPlayerLocal))
						{
							players.Add(entityPlayer.EntityName);
						}
					}
					players.Insert(0, this.displayName);
				}
				using (UnityWebRequest req = UnityWebRequest.Put("https://2v3d0ewjcg.execute-api.us-east-1.amazonaws.com/prod/set-developer-config", this.constructDevConfigContent(list, players)))
				{
					req.SetRequestHeader("Content-Type", "application/json");
					req.SetRequestHeader("Client-Id", "k6ji189bf7i4ge8il4iczzw7kpgmjt");
					req.SetRequestHeader("Authorization", "Bearer " + this.JWT);
					yield return req.SendWebRequest();
					if (req.result != UnityWebRequest.Result.Success)
					{
						Log.Warning("Failed to set the extension configuration: " + req.downloadHandler.text);
					}
					else
					{
						Log.Out("Extension Configuration set successfully");
					}
				}
				UnityWebRequest req = null;
				using (UnityWebRequest req = UnityWebRequest.Put("https://api.twitch.tv/helix/extensions/configurations", this.constructBroadcasterConfigContent(status, cmds)))
				{
					req.SetRequestHeader("Content-Type", "application/json");
					req.SetRequestHeader("Client-Id", "k6ji189bf7i4ge8il4iczzw7kpgmjt");
					req.SetRequestHeader("Authorization", "Bearer " + this.JWT);
					yield return req.SendWebRequest();
					if (req.result != UnityWebRequest.Result.Success)
					{
						Log.Warning(string.Format("Failed to set the extension configuration: {0}", req.downloadHandler.data));
					}
					else
					{
						Log.Out("Extension Configuration set successfully");
					}
				}
				req = null;
				yield return this.notifyStatusChange(this.constructStatusMessage(status, cmds, players));
				players = null;
			}
			else
			{
				Log.Warning("user is not a part of the extension beta. Cannot set configs.");
			}
			yield break;
			yield break;
		}

		// Token: 0x0600A657 RID: 42583 RVA: 0x0041BA31 File Offset: 0x00419C31
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator notifyStatusChange(string body)
		{
			using (UnityWebRequest request = UnityWebRequest.Put("https://api.twitch.tv/helix/extensions/pubsub", body))
			{
				request.method = "POST";
				request.SetRequestHeader("Content-Type", "application/json");
				request.SetRequestHeader("Client-Id", "k6ji189bf7i4ge8il4iczzw7kpgmjt");
				request.SetRequestHeader("Authorization", "Bearer " + this.JWT);
				yield return request.SendWebRequest();
				if (request.result != UnityWebRequest.Result.Success)
				{
					Log.Warning("Failed to broadcast status change: " + request.downloadHandler.text);
				}
				else
				{
					Log.Out("Status Change Pubsub Message sent successfully");
				}
			}
			UnityWebRequest request = null;
			yield break;
			yield break;
		}

		// Token: 0x0600A658 RID: 42584 RVA: 0x0041BA47 File Offset: 0x00419C47
		[PublicizedFrom(EAccessModifier.Private)]
		public string constructDevConfigContent(List<string> eventTypes, List<string> players)
		{
			return JsonConvert.SerializeObject(new SetDevConfigRequestData
			{
				actionTypes = eventTypes,
				players = players
			});
		}

		// Token: 0x0600A659 RID: 42585 RVA: 0x0041BA64 File Offset: 0x00419C64
		[PublicizedFrom(EAccessModifier.Private)]
		public string constructBroadcasterConfigContent(string status, string cmds)
		{
			SetConfigRequestData setConfigRequestData = new SetConfigRequestData();
			setConfigRequestData.extension_id = "k6ji189bf7i4ge8il4iczzw7kpgmjt";
			setConfigRequestData.segment = "broadcaster";
			setConfigRequestData.broadcaster_id = this.userId;
			setConfigRequestData.version = "0.0.1";
			ConfigContent configContent = new ConfigContent();
			configContent.o = this.opaqueId;
			configContent.d = this.displayName;
			configContent.l = Array.FindIndex<string>(Localization.knownLanguages, (string l) => l == Localization.language).ToString();
			configContent.s = status;
			configContent.c = cmds;
			setConfigRequestData.content = JsonConvert.SerializeObject(configContent);
			return JsonConvert.SerializeObject(setConfigRequestData);
		}

		// Token: 0x0600A65A RID: 42586 RVA: 0x0041BB18 File Offset: 0x00419D18
		[PublicizedFrom(EAccessModifier.Private)]
		public string constructStatusMessage(string status, string cmds, List<string> party)
		{
			PubSubStatusRequestData pubSubStatusRequestData = new PubSubStatusRequestData();
			pubSubStatusRequestData.broadcaster_id = this.userId;
			pubSubStatusRequestData.target = new List<string>
			{
				"broadcast"
			};
			PubSubStatusMessage pubSubStatusMessage = new PubSubStatusMessage();
			pubSubStatusMessage.opaqueId = this.opaqueId;
			pubSubStatusMessage.displayName = this.displayName;
			pubSubStatusMessage.language = Array.FindIndex<string>(Localization.knownLanguages, (string l) => l == Localization.language).ToString();
			pubSubStatusMessage.status = status;
			pubSubStatusMessage.commands = cmds;
			pubSubStatusMessage.party = party;
			pubSubStatusMessage.actionTypes = (from c in TwitchActionManager.Current.CategoryList.GetRange(1, 5)
			select c.DisplayName).ToList<string>();
			pubSubStatusRequestData.message = JsonConvert.SerializeObject(pubSubStatusMessage);
			return JsonConvert.SerializeObject(pubSubStatusRequestData);
		}

		// Token: 0x0400808B RID: 32907
		[PublicizedFrom(EAccessModifier.Private)]
		public const string EXTENSION_ID = "k6ji189bf7i4ge8il4iczzw7kpgmjt";

		// Token: 0x0400808C RID: 32908
		[PublicizedFrom(EAccessModifier.Private)]
		public const string URL = "http://localhost:52775/";

		// Token: 0x0400808D RID: 32909
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly HttpListener listener;

		// Token: 0x0400808E RID: 32910
		[PublicizedFrom(EAccessModifier.Private)]
		public bool pushConfigChanges;

		// Token: 0x0400808F RID: 32911
		[PublicizedFrom(EAccessModifier.Private)]
		public string JWT = string.Empty;

		// Token: 0x04008090 RID: 32912
		[PublicizedFrom(EAccessModifier.Private)]
		public string opaqueId;

		// Token: 0x04008091 RID: 32913
		[PublicizedFrom(EAccessModifier.Private)]
		public string userId;

		// Token: 0x04008092 RID: 32914
		[PublicizedFrom(EAccessModifier.Private)]
		public string displayName = string.Empty;

		// Token: 0x04008093 RID: 32915
		[PublicizedFrom(EAccessModifier.Private)]
		public string broadcaster_type = string.Empty;

		// Token: 0x04008094 RID: 32916
		[PublicizedFrom(EAccessModifier.Private)]
		public bool inBeta;

		// Token: 0x04008095 RID: 32917
		[PublicizedFrom(EAccessModifier.Private)]
		public float lastUpdate;

		// Token: 0x04008096 RID: 32918
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isConnected;

		// Token: 0x04008097 RID: 32919
		[PublicizedFrom(EAccessModifier.Private)]
		public Queue<TwitchExtensionCommand> commands = new Queue<TwitchExtensionCommand>();

		// Token: 0x04008098 RID: 32920
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchAction> tempCommandList = new List<TwitchAction>();

		// Token: 0x02001502 RID: 5378
		[Serializable]
		public class BitCmdVerifyResponse
		{
			// Token: 0x04008099 RID: 32921
			public bool canSend;

			// Token: 0x0400809A RID: 32922
			public string userId;

			// Token: 0x0400809B RID: 32923
			public string command;
		}

		// Token: 0x02001503 RID: 5379
		[Serializable]
		public class InBetaResponse
		{
			// Token: 0x0400809C RID: 32924
			public bool inBeta;
		}
	}
}
