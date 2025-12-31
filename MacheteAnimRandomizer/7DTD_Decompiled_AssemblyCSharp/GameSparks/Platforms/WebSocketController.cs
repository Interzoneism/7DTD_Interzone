using System;
using System.Collections.Generic;
using GameSparks.Core;
using UnityEngine;

namespace GameSparks.Platforms
{
	// Token: 0x02001999 RID: 6553
	public class WebSocketController : MonoBehaviour
	{
		// Token: 0x1700162E RID: 5678
		// (get) Token: 0x0600C0D7 RID: 49367 RVA: 0x004903A5 File Offset: 0x0048E5A5
		// (set) Token: 0x0600C0D8 RID: 49368 RVA: 0x004903AD File Offset: 0x0048E5AD
		public string GSName { get; set; }

		// Token: 0x0600C0D9 RID: 49369 RVA: 0x004903B6 File Offset: 0x0048E5B6
		[PublicizedFrom(EAccessModifier.Private)]
		public void Awake()
		{
			this.GSName = base.name;
		}

		// Token: 0x0600C0DA RID: 49370 RVA: 0x004903C4 File Offset: 0x0048E5C4
		public void AddWebSocket(IControlledWebSocket socket)
		{
			this.webSockets.Add(socket);
			this.websocketCollectionModified = true;
		}

		// Token: 0x0600C0DB RID: 49371 RVA: 0x004903D9 File Offset: 0x0048E5D9
		public void RemoveWebSocket(IControlledWebSocket socket)
		{
			this.webSockets.Remove(socket);
			this.websocketCollectionModified = true;
		}

		// Token: 0x0600C0DC RID: 49372 RVA: 0x004903F0 File Offset: 0x0048E5F0
		[PublicizedFrom(EAccessModifier.Private)]
		public IControlledWebSocket GetSocket(int socketId)
		{
			foreach (IControlledWebSocket controlledWebSocket in this.webSockets)
			{
				if (controlledWebSocket.SocketId == socketId)
				{
					return controlledWebSocket;
				}
			}
			return null;
		}

		// Token: 0x0600C0DD RID: 49373 RVA: 0x0049044C File Offset: 0x0048E64C
		public void GSSocketOnOpen(string data)
		{
			IDictionary<string, object> dictionary = (IDictionary<string, object>)GSJson.From(data);
			if (dictionary == null)
			{
				throw new FormatException("parsed json was null. ");
			}
			if (!dictionary.ContainsKey("socketId"))
			{
				throw new FormatException();
			}
			int socketId = Convert.ToInt32(dictionary["socketId"]);
			IControlledWebSocket socket = this.GetSocket(socketId);
			if (socket != null)
			{
				socket.TriggerOnOpen();
			}
		}

		// Token: 0x0600C0DE RID: 49374 RVA: 0x004904A8 File Offset: 0x0048E6A8
		public void GSSocketOnClose(string data)
		{
			int socketId = Convert.ToInt32(((IDictionary<string, object>)GSJson.From(data))["socketId"]);
			IControlledWebSocket socket = this.GetSocket(socketId);
			if (socket != null)
			{
				socket.TriggerOnClose();
			}
		}

		// Token: 0x0600C0DF RID: 49375 RVA: 0x004904E4 File Offset: 0x0048E6E4
		public void GSSocketOnMessage(string data)
		{
			IDictionary<string, object> dictionary = (IDictionary<string, object>)GSJson.From(data);
			int socketId = Convert.ToInt32(dictionary["socketId"]);
			IControlledWebSocket socket = this.GetSocket(socketId);
			if (socket != null)
			{
				socket.TriggerOnMessage((string)dictionary["message"]);
			}
		}

		// Token: 0x0600C0E0 RID: 49376 RVA: 0x00490530 File Offset: 0x0048E730
		public void GSSocketOnError(string data)
		{
			IDictionary<string, object> dictionary = (IDictionary<string, object>)GSJson.From(data);
			int socketId = Convert.ToInt32(dictionary["socketId"]);
			string message = (string)dictionary["error"];
			IControlledWebSocket socket = this.GetSocket(socketId);
			if (socket != null)
			{
				socket.TriggerOnError(message);
			}
		}

		// Token: 0x0600C0E1 RID: 49377 RVA: 0x0049057C File Offset: 0x0048E77C
		public void ServerToClient(string jsonData)
		{
			IDictionary<string, object> dictionary = GSJson.From(jsonData) as IDictionary<string, object>;
			int socketId = int.Parse(dictionary["socketId"].ToString());
			IControlledWebSocket socket = this.GetSocket(socketId);
			if (socket == null)
			{
				return;
			}
			string a = dictionary["functionName"].ToString();
			if (a == "onError")
			{
				socket.TriggerOnError(dictionary["data"].ToString());
				return;
			}
			if (a == "onMessage")
			{
				socket.TriggerOnMessage(dictionary["data"].ToString());
				return;
			}
			if (a == "onOpen")
			{
				socket.TriggerOnOpen();
				return;
			}
			if (!(a == "onClose"))
			{
				return;
			}
			socket.TriggerOnClose();
		}

		// Token: 0x0600C0E2 RID: 49378 RVA: 0x00490638 File Offset: 0x0048E838
		[PublicizedFrom(EAccessModifier.Private)]
		public void Update()
		{
			this.websocketCollectionModified = false;
			foreach (IControlledWebSocket controlledWebSocket in this.webSockets)
			{
				controlledWebSocket.Update();
				if (this.websocketCollectionModified)
				{
					break;
				}
			}
		}

		// Token: 0x04009655 RID: 38485
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public List<IControlledWebSocket> webSockets = new List<IControlledWebSocket>();

		// Token: 0x04009656 RID: 38486
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool websocketCollectionModified;
	}
}
