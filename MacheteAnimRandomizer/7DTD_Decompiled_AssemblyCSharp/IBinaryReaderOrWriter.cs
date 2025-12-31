using System;
using System.IO;
using UnityEngine;

// Token: 0x020011AC RID: 4524
public interface IBinaryReaderOrWriter
{
	// Token: 0x17000EA7 RID: 3751
	// (get) Token: 0x06008D73 RID: 36211
	Stream BaseStream { get; }

	// Token: 0x06008D74 RID: 36212
	bool ReadWrite(bool _value);

	// Token: 0x06008D75 RID: 36213
	byte ReadWrite(byte _value);

	// Token: 0x06008D76 RID: 36214
	sbyte ReadWrite(sbyte _value);

	// Token: 0x06008D77 RID: 36215
	char ReadWrite(char _value);

	// Token: 0x06008D78 RID: 36216
	short ReadWrite(short _value);

	// Token: 0x06008D79 RID: 36217
	ushort ReadWrite(ushort _value);

	// Token: 0x06008D7A RID: 36218
	int ReadWrite(int _value);

	// Token: 0x06008D7B RID: 36219
	uint ReadWrite(uint _value);

	// Token: 0x06008D7C RID: 36220
	long ReadWrite(long _value);

	// Token: 0x06008D7D RID: 36221
	ulong ReadWrite(ulong _value);

	// Token: 0x06008D7E RID: 36222
	float ReadWrite(float _value);

	// Token: 0x06008D7F RID: 36223
	double ReadWrite(double _value);

	// Token: 0x06008D80 RID: 36224
	decimal ReadWrite(decimal _value);

	// Token: 0x06008D81 RID: 36225
	string ReadWrite(string _value);

	// Token: 0x06008D82 RID: 36226
	void ReadWrite(byte[] _buffer, int _index, int _count);

	// Token: 0x06008D83 RID: 36227
	Vector3 ReadWrite(Vector3 _value);
}
