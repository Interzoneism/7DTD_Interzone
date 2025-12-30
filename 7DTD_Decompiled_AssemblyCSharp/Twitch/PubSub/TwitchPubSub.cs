using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Twitch.PubSub
{
	// Token: 0x020015A7 RID: 5543
	public class TwitchPubSub
	{
		// Token: 0x0600AA69 RID: 43625 RVA: 0x00432968 File Offset: 0x00430B68
		public void Connect(string userID)
		{
			if (this.cts != null)
			{
				this.cts.Cancel();
			}
			this.cts = new CancellationTokenSource();
			Task.Run(() => this.StartAsync(new TwitchTopic[]
			{
				TwitchTopic.ChannelPoints(userID),
				TwitchTopic.Bits(userID),
				TwitchTopic.Subscription(userID),
				TwitchTopic.HypeTrain(userID),
				TwitchTopic.CreatorGoal(userID)
			}, this.cts.Token));
		}

		// Token: 0x0600AA6A RID: 43626 RVA: 0x004329B7 File Offset: 0x00430BB7
		public void Disconnect()
		{
			this.cts.Cancel();
			TwitchPubSub.reconnect = false;
		}

		// Token: 0x0600AA6B RID: 43627 RVA: 0x004329CC File Offset: 0x00430BCC
		public Task StartAsync(TwitchTopic[] newTopics, CancellationToken token)
		{
			TwitchPubSub.<StartAsync>d__12 <StartAsync>d__;
			<StartAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartAsync>d__.<>4__this = this;
			<StartAsync>d__.newTopics = newTopics;
			<StartAsync>d__.token = token;
			<StartAsync>d__.<>1__state = -1;
			<StartAsync>d__.<>t__builder.Start<TwitchPubSub.<StartAsync>d__12>(ref <StartAsync>d__);
			return <StartAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0600AA6C RID: 43628 RVA: 0x00432A20 File Offset: 0x00430C20
		[PublicizedFrom(EAccessModifier.Private)]
		public void HandleMessage(string receivedMessage, TwitchPubSub.MessageTypes msgType)
		{
			if (msgType != TwitchPubSub.MessageTypes.Standard)
			{
				if (msgType == TwitchPubSub.MessageTypes.HypeStart)
				{
					TwitchManager.Current.StartHypeTrain();
				}
				return;
			}
			JObject jobject = JObject.Parse(receivedMessage);
			string a = jobject["type"].Value<string>();
			if (a == "RESPONSE" && jobject["error"].Value<string>() != "")
			{
				return;
			}
			if (a == "RESPONSE")
			{
				return;
			}
			if (this.HandlePongMessage(receivedMessage))
			{
				return;
			}
			if (this.HandleReconnectMessage(receivedMessage))
			{
				return;
			}
			this.HandleRedemptionsMessages(receivedMessage);
		}

		// Token: 0x0600AA6D RID: 43629 RVA: 0x00432AAC File Offset: 0x00430CAC
		[PublicizedFrom(EAccessModifier.Private)]
		public Task StartListening(IEnumerable<TwitchTopic> topics)
		{
			TwitchPubSub.<StartListening>d__14 <StartListening>d__;
			<StartListening>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartListening>d__.<>4__this = this;
			<StartListening>d__.topics = topics;
			<StartListening>d__.<>1__state = -1;
			<StartListening>d__.<>t__builder.Start<TwitchPubSub.<StartListening>d__14>(ref <StartListening>d__);
			return <StartListening>d__.<>t__builder.Task;
		}

		// Token: 0x0600AA6E RID: 43630 RVA: 0x00432AF8 File Offset: 0x00430CF8
		[PublicizedFrom(EAccessModifier.Private)]
		public void PingTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			string message = "{ \"type\": \"PING\" }";
			this.SendMessageOnSocket(message).GetAwaiter().GetResult();
			this.pongTimer = new System.Timers.Timer(TimeSpan.FromSeconds(10.0).TotalMilliseconds);
			this.pongTimer.Elapsed += this.PongTimer_Elapsed;
			this.pongTimer.Start();
			this.pingAcknowledged = false;
		}

		// Token: 0x0600AA6F RID: 43631 RVA: 0x00432B69 File Offset: 0x00430D69
		[PublicizedFrom(EAccessModifier.Private)]
		public void PongTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			if (!this.pingAcknowledged)
			{
				TwitchPubSub.reconnect = true;
				this.pongTimer.Dispose();
			}
		}

		// Token: 0x0600AA70 RID: 43632 RVA: 0x00432B84 File Offset: 0x00430D84
		[PublicizedFrom(EAccessModifier.Private)]
		public Task SendMessageOnSocket(string message)
		{
			if (this.socket.State != WebSocketState.Open)
			{
				return Task.CompletedTask;
			}
			byte[] bytes = Encoding.ASCII.GetBytes(message);
			return this.socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
		}

		// Token: 0x14000104 RID: 260
		// (add) Token: 0x0600AA71 RID: 43633 RVA: 0x00432BCC File Offset: 0x00430DCC
		// (remove) Token: 0x0600AA72 RID: 43634 RVA: 0x00432C04 File Offset: 0x00430E04
		public event EventHandler<PubSubBitRedemptionMessage.BitRedemptionData> OnBitsRedeemed;

		// Token: 0x14000105 RID: 261
		// (add) Token: 0x0600AA73 RID: 43635 RVA: 0x00432C3C File Offset: 0x00430E3C
		// (remove) Token: 0x0600AA74 RID: 43636 RVA: 0x00432C74 File Offset: 0x00430E74
		public event EventHandler<PubSubSubscriptionRedemptionMessage> OnSubscriptionRedeemed;

		// Token: 0x14000106 RID: 262
		// (add) Token: 0x0600AA75 RID: 43637 RVA: 0x00432CAC File Offset: 0x00430EAC
		// (remove) Token: 0x0600AA76 RID: 43638 RVA: 0x00432CE4 File Offset: 0x00430EE4
		public event EventHandler<PubSubChannelPointMessage.ChannelRedemptionData> OnChannelPointsRedeemed;

		// Token: 0x14000107 RID: 263
		// (add) Token: 0x0600AA77 RID: 43639 RVA: 0x00432D1C File Offset: 0x00430F1C
		// (remove) Token: 0x0600AA78 RID: 43640 RVA: 0x00432D54 File Offset: 0x00430F54
		public event EventHandler<PubSubGoalMessage.Goal> OnGoalAchieved;

		// Token: 0x0600AA79 RID: 43641 RVA: 0x00432D89 File Offset: 0x00430F89
		[PublicizedFrom(EAccessModifier.Private)]
		public bool HandlePongMessage(string message)
		{
			if (message.Contains("\"PONG\""))
			{
				this.pingAcknowledged = true;
				this.pongTimer.Stop();
				this.pongTimer.Dispose();
				return true;
			}
			return false;
		}

		// Token: 0x0600AA7A RID: 43642 RVA: 0x00432DB8 File Offset: 0x00430FB8
		[PublicizedFrom(EAccessModifier.Private)]
		public bool HandleReconnectMessage(string message)
		{
			if (message.Contains("\"RECONNECT\""))
			{
				TwitchPubSub.reconnect = true;
				return true;
			}
			return false;
		}

		// Token: 0x0600AA7B RID: 43643 RVA: 0x00432DD0 File Offset: 0x00430FD0
		[PublicizedFrom(EAccessModifier.Private)]
		public bool HandleRedemptionsMessages(string message)
		{
			JObject jobject = JObject.Parse(message);
			if (jobject["type"].Value<string>() == "MESSAGE")
			{
				string text = jobject["data"]["topic"].Value<string>();
				if (text.StartsWith("channel-points-channel-v1"))
				{
					string message2 = jobject["data"]["message"].Value<string>();
					PubSubChannelPointMessage pubSubChannelPointMessage = null;
					try
					{
						pubSubChannelPointMessage = PubSubChannelPointMessage.Deserialize(message2);
					}
					catch (Exception ex)
					{
						Debug.LogError(ex.ToString());
						Debug.LogError(message2);
					}
					if (this.OnChannelPointsRedeemed != null && pubSubChannelPointMessage != null)
					{
						this.OnChannelPointsRedeemed(null, pubSubChannelPointMessage.data);
					}
					return true;
				}
				if (text.StartsWith("channel-bits-events"))
				{
					string message3 = jobject["data"]["message"].Value<string>();
					PubSubBitRedemptionMessage pubSubBitRedemptionMessage = null;
					try
					{
						pubSubBitRedemptionMessage = PubSubBitRedemptionMessage.Deserialize(message3);
					}
					catch (Exception ex2)
					{
						Debug.LogError(ex2.ToString());
						Debug.LogError(message3);
					}
					if (this.OnBitsRedeemed != null && pubSubBitRedemptionMessage != null)
					{
						this.OnBitsRedeemed(null, pubSubBitRedemptionMessage.data);
					}
					return true;
				}
				if (text.StartsWith("channel-subscribe-events"))
				{
					string message4 = jobject["data"]["message"].Value<string>();
					PubSubSubscriptionRedemptionMessage pubSubSubscriptionRedemptionMessage = null;
					try
					{
						pubSubSubscriptionRedemptionMessage = PubSubSubscriptionRedemptionMessage.Deserialize(message4);
					}
					catch (Exception ex3)
					{
						Debug.LogError(ex3.ToString());
						Debug.LogError(message4);
					}
					if (this.OnSubscriptionRedeemed != null && pubSubSubscriptionRedemptionMessage != null)
					{
						this.OnSubscriptionRedeemed(null, pubSubSubscriptionRedemptionMessage);
					}
					return true;
				}
				if (text.StartsWith("creator-goals-events"))
				{
					string message5 = jobject["data"]["message"].Value<string>();
					PubSubGoalMessage pubSubGoalMessage = null;
					try
					{
						pubSubGoalMessage = PubSubGoalMessage.Deserialize(message5);
					}
					catch (Exception ex4)
					{
						Debug.LogError(ex4.ToString());
						Debug.LogError(message5);
					}
					if (pubSubGoalMessage.type == "goal_achieved" && this.OnGoalAchieved != null && pubSubGoalMessage != null)
					{
						this.OnGoalAchieved(null, pubSubGoalMessage.data.goal);
					}
					return true;
				}
				if (text.StartsWith("hype-train-events-v1"))
				{
					try
					{
						string text2 = jobject["data"]["message"].ToString();
						Debug.LogWarning(text2);
						if (text2.Contains("hype-train-start"))
						{
							TwitchManager.Current.StartHypeTrain();
						}
						else if (text2.Contains("hype-train-level-up"))
						{
							TwitchManager.Current.IncrementHypeTrainLevel();
						}
						else if (text2.Contains("hype-train-end"))
						{
							TwitchManager.Current.EndHypeTrain();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogWarning("Hype Train Error: " + message);
						Debug.LogWarning("Hype Train Exception: " + ex5.ToString());
					}
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600AA7C RID: 43644 RVA: 0x004330C4 File Offset: 0x004312C4
		public void Cleanup()
		{
			if (this.pingTimer != null)
			{
				this.pingTimer.Dispose();
			}
			if (this.pongTimer != null)
			{
				this.pongTimer.Dispose();
			}
			if (this.socket != null)
			{
				this.socket.Dispose();
			}
		}

		// Token: 0x0400850E RID: 34062
		[PublicizedFrom(EAccessModifier.Private)]
		public ClientWebSocket socket;

		// Token: 0x0400850F RID: 34063
		[PublicizedFrom(EAccessModifier.Private)]
		public System.Timers.Timer pingTimer;

		// Token: 0x04008510 RID: 34064
		[PublicizedFrom(EAccessModifier.Private)]
		public System.Timers.Timer pongTimer;

		// Token: 0x04008511 RID: 34065
		[PublicizedFrom(EAccessModifier.Private)]
		public System.Timers.Timer reconnectTimer = new System.Timers.Timer();

		// Token: 0x04008512 RID: 34066
		[PublicizedFrom(EAccessModifier.Private)]
		public bool pingAcknowledged;

		// Token: 0x04008513 RID: 34067
		[PublicizedFrom(EAccessModifier.Private)]
		public static bool reconnect = false;

		// Token: 0x04008514 RID: 34068
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchTopic[] topics;

		// Token: 0x04008515 RID: 34069
		[PublicizedFrom(EAccessModifier.Private)]
		public CancellationTokenSource cts;

		// Token: 0x04008516 RID: 34070
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly TimeSpan[] _ReconnectTimeouts = new TimeSpan[]
		{
			TimeSpan.FromSeconds(1.0),
			TimeSpan.FromSeconds(5.0),
			TimeSpan.FromSeconds(10.0),
			TimeSpan.FromSeconds(30.0),
			TimeSpan.FromMinutes(1.0),
			TimeSpan.FromMinutes(5.0)
		};

		// Token: 0x020015A8 RID: 5544
		[PublicizedFrom(EAccessModifier.Private)]
		public enum MessageTypes
		{
			// Token: 0x0400851C RID: 34076
			Standard,
			// Token: 0x0400851D RID: 34077
			HypeStart
		}
	}
}
