using System;
using System.Collections;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000B1C RID: 2844
public class EventSubClient
{
	// Token: 0x14000086 RID: 134
	// (add) Token: 0x06005895 RID: 22677 RVA: 0x0023CC04 File Offset: 0x0023AE04
	// (remove) Token: 0x06005896 RID: 22678 RVA: 0x0023CC3C File Offset: 0x0023AE3C
	public event Action<JObject> OnEventReceived;

	// Token: 0x06005897 RID: 22679 RVA: 0x0023CC71 File Offset: 0x0023AE71
	public EventSubClient(string broadcasterID, string accessToken, string clientId)
	{
		this.broadcasterUserID = broadcasterID;
		this.accessToken = accessToken;
		this.clientId = clientId;
	}

	// Token: 0x06005898 RID: 22680 RVA: 0x0023CCAC File Offset: 0x0023AEAC
	public void Connect()
	{
		if (this.isRunning)
		{
			return;
		}
		this.ws = new ClientWebSocket();
		this.ws.ConnectAsync(this.twitchWsUri, CancellationToken.None).Wait();
		Log.Out("Connected to Twitch EventSub WebSocket");
		this.isRunning = true;
		this.receiveCoroutine = GameManager.Instance.StartCoroutine(this.ReceiveLoopCoroutine());
	}

	// Token: 0x06005899 RID: 22681 RVA: 0x0023CD0F File Offset: 0x0023AF0F
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ReceiveLoopCoroutine()
	{
		byte[] buffer = new byte[8192];
		while (this.isRunning && this.ws.State == WebSocketState.Open)
		{
			ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
			Task<WebSocketReceiveResult> receiveTask = this.ws.ReceiveAsync(segment, CancellationToken.None);
			while (!receiveTask.IsCompleted)
			{
				yield return null;
			}
			WebSocketReceiveResult result = receiveTask.Result;
			if (result.MessageType == WebSocketMessageType.Close)
			{
				Log.Out("WebSocket closed by server.");
				this.isRunning = false;
				yield break;
			}
			StringBuilder messageBuilder = new StringBuilder();
			messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
			while (!result.EndOfMessage)
			{
				receiveTask = this.ws.ReceiveAsync(segment, CancellationToken.None);
				while (!receiveTask.IsCompleted)
				{
					yield return null;
				}
				result = receiveTask.Result;
				messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
			}
			string text = messageBuilder.ToString();
			Log.Out("Received raw message: " + text);
			this.HandleMessage(text);
			segment = default(ArraySegment<byte>);
			receiveTask = null;
			messageBuilder = null;
		}
		yield break;
	}

	// Token: 0x0600589A RID: 22682 RVA: 0x0023CD20 File Offset: 0x0023AF20
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleMessage(string json)
	{
		EventSubMessage eventSubMessage = JsonConvert.DeserializeObject<EventSubMessage>(json);
		if (eventSubMessage == null)
		{
			Log.Warning("Invalid EventSub message received.");
			return;
		}
		string messageType = eventSubMessage.Metadata.MessageType;
		if (messageType == "session_welcome")
		{
			JToken jtoken = eventSubMessage.Payload["session"];
			string text;
			if (jtoken == null)
			{
				text = null;
			}
			else
			{
				JToken jtoken2 = jtoken["id"];
				text = ((jtoken2 != null) ? jtoken2.ToString() : null);
			}
			this.sessionID = (text ?? string.Empty);
			Log.Out("Session ID: " + this.sessionID);
			GameManager.Instance.StartCoroutine(this.SubscribeToEvents());
			return;
		}
		if (!(messageType == "notification"))
		{
			if (!(messageType == "session_keepalive"))
			{
				if (!(messageType == "session_reconnect"))
				{
					Log.Out("Unhandled message type: " + eventSubMessage.Metadata.MessageType);
				}
				else
				{
					JToken jtoken3 = eventSubMessage.Payload["session"];
					string text2;
					if (jtoken3 == null)
					{
						text2 = null;
					}
					else
					{
						JToken jtoken4 = jtoken3["reconnect_url"];
						text2 = ((jtoken4 != null) ? jtoken4.ToString() : null);
					}
					string text3 = text2;
					if (!string.IsNullOrEmpty(text3))
					{
						Log.Out("Reconnecting to: " + text3);
						this.Reconnect(text3);
						return;
					}
				}
				return;
			}
			Log.Out("Received Keep-Alive");
			return;
		}
		else
		{
			Action<JObject> onEventReceived = this.OnEventReceived;
			if (onEventReceived == null)
			{
				return;
			}
			onEventReceived(eventSubMessage.Payload);
			return;
		}
	}

	// Token: 0x0600589B RID: 22683 RVA: 0x0023CE7D File Offset: 0x0023B07D
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator SubscribeToEvents()
	{
		yield return this.CreateEventSubSubscription("channel.subscribe");
		yield return this.CreateEventSubSubscription("channel.channel_points_custom_reward_redemption.add");
		yield return this.CreateEventSubSubscription("channel.subscription.message");
		yield return this.CreateEventSubSubscription("channel.subscription.gift");
		yield return this.CreateEventSubSubscription("channel.bits.use");
		yield return this.CreateEventSubSubscription("channel.hype_train.begin");
		yield return this.CreateEventSubSubscription("channel.hype_train.progress");
		yield return this.CreateEventSubSubscription("channel.hype_train.end");
		yield break;
	}

	// Token: 0x0600589C RID: 22684 RVA: 0x0023CE8C File Offset: 0x0023B08C
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator CreateEventSubSubscription(string eventType)
	{
		JObject jobject = new JObject();
		jobject["type"] = eventType;
		jobject["version"] = "1";
		string propertyName = "condition";
		JObject jobject2 = new JObject();
		jobject2["broadcaster_user_id"] = this.broadcasterUserID;
		jobject[propertyName] = jobject2;
		string propertyName2 = "transport";
		JObject jobject3 = new JObject();
		jobject3["method"] = "websocket";
		jobject3["session_id"] = this.sessionID;
		jobject[propertyName2] = jobject3;
		string s = jobject.ToString();
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		UnityWebRequest req = new UnityWebRequest("https://api.twitch.tv/helix/eventsub/subscriptions", "POST");
		req.uploadHandler = new UploadHandlerRaw(bytes);
		req.downloadHandler = new DownloadHandlerBuffer();
		req.SetRequestHeader("Content-Type", "application/json");
		req.SetRequestHeader("Authorization", "Bearer " + this.accessToken);
		req.SetRequestHeader("Client-Id", this.clientId);
		yield return req.SendWebRequest();
		if (req.result == UnityWebRequest.Result.Success)
		{
			Log.Out("Successfully subscribed to " + eventType);
		}
		else
		{
			Log.Warning(string.Concat(new string[]
			{
				"Failed to subscribe to ",
				eventType,
				": ",
				req.error,
				" | ",
				req.downloadHandler.text
			}));
		}
		yield break;
	}

	// Token: 0x0600589D RID: 22685 RVA: 0x0023CEA4 File Offset: 0x0023B0A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Reconnect(string newUrl)
	{
		if (!this.isRunning)
		{
			return;
		}
		this.Disconnect();
		this.ws = new ClientWebSocket();
		this.ws.ConnectAsync(new Uri(newUrl), CancellationToken.None).Wait();
		Log.Out("Reconnected to new WebSocket URL");
		this.isRunning = true;
		this.receiveCoroutine = GameManager.Instance.StartCoroutine(this.ReceiveLoopCoroutine());
	}

	// Token: 0x0600589E RID: 22686 RVA: 0x0023CF10 File Offset: 0x0023B110
	public void Disconnect()
	{
		if (!this.isRunning)
		{
			return;
		}
		this.isRunning = false;
		if (this.receiveCoroutine != null)
		{
			GameManager.Instance.StopCoroutine(this.receiveCoroutine);
		}
		if (this.ws.State == WebSocketState.Open)
		{
			this.ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None).Wait();
		}
		Log.Out("Disconnected from Twitch EventSub WebSocket");
	}

	// Token: 0x0600589F RID: 22687 RVA: 0x0023CF7C File Offset: 0x0023B17C
	public void Cleanup()
	{
		if (this.cleanedUp)
		{
			return;
		}
		this.cleanedUp = true;
		this.Disconnect();
		ClientWebSocket clientWebSocket = this.ws;
		if (clientWebSocket != null)
		{
			clientWebSocket.Dispose();
		}
		Log.Out("EventSubClient resources cleaned up.");
	}

	// Token: 0x040043BB RID: 17339
	[PublicizedFrom(EAccessModifier.Private)]
	public ClientWebSocket ws;

	// Token: 0x040043BC RID: 17340
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Uri twitchWsUri = new Uri("wss://eventsub.wss.twitch.tv/ws");

	// Token: 0x040043BD RID: 17341
	[PublicizedFrom(EAccessModifier.Private)]
	public Coroutine receiveCoroutine;

	// Token: 0x040043BE RID: 17342
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isRunning;

	// Token: 0x040043BF RID: 17343
	[PublicizedFrom(EAccessModifier.Private)]
	public string sessionID = string.Empty;

	// Token: 0x040043C0 RID: 17344
	[PublicizedFrom(EAccessModifier.Private)]
	public string broadcasterUserID;

	// Token: 0x040043C1 RID: 17345
	[PublicizedFrom(EAccessModifier.Private)]
	public string accessToken;

	// Token: 0x040043C2 RID: 17346
	[PublicizedFrom(EAccessModifier.Private)]
	public string clientId;

	// Token: 0x040043C3 RID: 17347
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cleanedUp;
}
