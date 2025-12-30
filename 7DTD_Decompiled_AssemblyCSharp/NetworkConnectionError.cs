using System;

// Token: 0x020007D3 RID: 2003
public enum NetworkConnectionError
{
	// Token: 0x04002EA7 RID: 11943
	InternalDirectConnectFailed = -5,
	// Token: 0x04002EA8 RID: 11944
	EmptyConnectTarget,
	// Token: 0x04002EA9 RID: 11945
	IncorrectParameters,
	// Token: 0x04002EAA RID: 11946
	CreateSocketOrThreadFailure,
	// Token: 0x04002EAB RID: 11947
	AlreadyConnectedToAnotherServer,
	// Token: 0x04002EAC RID: 11948
	NoError,
	// Token: 0x04002EAD RID: 11949
	ConnectionFailed = 15,
	// Token: 0x04002EAE RID: 11950
	AlreadyConnectedToServer,
	// Token: 0x04002EAF RID: 11951
	TooManyConnectedPlayers = 18,
	// Token: 0x04002EB0 RID: 11952
	RSAPublicKeyMismatch = 21,
	// Token: 0x04002EB1 RID: 11953
	ConnectionBanned,
	// Token: 0x04002EB2 RID: 11954
	InvalidPassword,
	// Token: 0x04002EB3 RID: 11955
	NATTargetNotConnected = 69,
	// Token: 0x04002EB4 RID: 11956
	NATTargetConnectionLost = 71,
	// Token: 0x04002EB5 RID: 11957
	NATPunchthroughFailed = 73,
	// Token: 0x04002EB6 RID: 11958
	InvalidPort,
	// Token: 0x04002EB7 RID: 11959
	RestartRequired
}
