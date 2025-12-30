using System;
using System.IO;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200088B RID: 2187
public static class SimpleMeshFile
{
	// Token: 0x06003FE3 RID: 16355 RVA: 0x001A2AE0 File Offset: 0x001A0CE0
	public static void WriteGameObject(BinaryWriter _bw, GameObject _go)
	{
		try
		{
			_bw.Write(1835365224);
			_bw.Write(6);
			MeshFilter[] componentsInChildren = _go.GetComponentsInChildren<MeshFilter>();
			_bw.Write((short)componentsInChildren.Length);
			int num = 0;
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				_bw.Write(meshFilter.transform.name);
				SimpleMeshFile.writeMesh(_bw, meshFilter.mesh, MeshDescription.meshes[0].textureAtlas.uvMapping);
				num += meshFilter.mesh.vertexCount;
			}
			Log.Out("Saved. Meshes: " + componentsInChildren.Length.ToString() + " Vertices: " + num.ToString());
		}
		finally
		{
		}
	}

	// Token: 0x06003FE4 RID: 16356 RVA: 0x001A2BA0 File Offset: 0x001A0DA0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void writeMesh(BinaryWriter _bw, Mesh _mesh, UVRectTiling[] _uvMapping)
	{
		try
		{
			Vector3[] vertices = _mesh.vertices;
			_bw.Write((uint)vertices.Length);
			for (int i = 0; i < vertices.Length; i++)
			{
				_bw.Write(vertices[i].x);
				_bw.Write(vertices[i].y);
				_bw.Write(vertices[i].z);
			}
			int[] indices = _mesh.GetIndices(0);
			Vector2[] uv = _mesh.uv;
			Vector2[] uv2 = _mesh.uv2;
			_bw.Write((uint)uv.Length);
			for (int j = 0; j < uv.Length; j++)
			{
				int num = (int)uv2[j].x;
				int num2 = -1;
				for (int k = 0; k < _uvMapping.Length; k++)
				{
					if (_uvMapping[k].index == num || k + 1 >= _uvMapping.Length || (float)_uvMapping[k].index + _uvMapping[k].uv.width * _uvMapping[k].uv.height > (float)num)
					{
						num2 = k;
						break;
					}
				}
				if (num2 == -1)
				{
					num2 = 0;
				}
				_bw.Write((short)num2);
				_bw.Write((byte)(num - _uvMapping[num2].index));
				bool value = (double)uv2[j].y > 0.5;
				_bw.Write(value);
				_bw.Write((ushort)(uv[j].x * 10000f));
				_bw.Write((ushort)(uv[j].y * 10000f));
			}
			_bw.Write((uint)indices.Length);
			for (int l = 0; l < indices.Length; l++)
			{
				_bw.Write((ushort)indices[l]);
			}
		}
		finally
		{
		}
	}

	// Token: 0x06003FE5 RID: 16357 RVA: 0x001A2D84 File Offset: 0x001A0F84
	public static GameObject ReadGameObject(PathAbstractions.AbstractedLocation _meshLocation, float _offsetY = 0f, Material _mat = null, bool _bTextureArray = true, bool _markMeshesNoLongerReadable = false, object _userCallbackData = null, SimpleMeshFile.GameObjectLoadedCallback _asyncCallback = null)
	{
		return SimpleMeshFile.ReadGameObject(_meshLocation.FullPath, _offsetY, _mat, _bTextureArray, _markMeshesNoLongerReadable, _userCallbackData, _asyncCallback);
	}

	// Token: 0x06003FE6 RID: 16358 RVA: 0x001A2D9C File Offset: 0x001A0F9C
	public static GameObject ReadGameObject(string _filename, float _offsetY = 0f, Material _mat = null, bool _bTextureArray = true, bool _markMeshesNoLongerReadable = false, object _userCallbackData = null, SimpleMeshFile.GameObjectLoadedCallback _asyncCallback = null)
	{
		try
		{
			return SimpleMeshFile.ReadGameObject(SdFile.OpenRead(_filename), _offsetY, _mat, _bTextureArray, _markMeshesNoLongerReadable, _userCallbackData, _asyncCallback, _filename);
		}
		catch (Exception e)
		{
			Log.Error("Reading mesh " + _filename + " failed:");
			Log.Exception(e);
		}
		return null;
	}

	// Token: 0x06003FE7 RID: 16359 RVA: 0x001A2DF0 File Offset: 0x001A0FF0
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject ReadGameObject(Stream _inputStream, float _offsetY = 0f, Material _mat = null, bool _bTextureArray = true, bool _markMeshesNoLongerReadable = false, object _userCallbackData = null, SimpleMeshFile.GameObjectLoadedCallback _asyncCallback = null, string _identifier = null)
	{
		GameObject result;
		try
		{
			if (_asyncCallback == null)
			{
				try
				{
					using (SimpleMeshFile.SimpleMeshDataArray simpleMeshDataArray = new SimpleMeshFile.SimpleMeshDataArray(Mesh.AllocateWritableMeshData(SimpleMeshFile.readLengthFromHeaderAndReset(_inputStream))))
					{
						SimpleMeshFile.readData(simpleMeshDataArray, _inputStream, _bTextureArray);
						return SimpleMeshFile.CreateUnityObjects(SimpleMeshFile.createMeshInfo(simpleMeshDataArray, _markMeshesNoLongerReadable, _offsetY, _mat));
					}
				}
				finally
				{
					if (_inputStream != null)
					{
						((IDisposable)_inputStream).Dispose();
					}
				}
			}
			SimpleMeshFile.SimpleMeshDataArray meshDatas = new SimpleMeshFile.SimpleMeshDataArray(Mesh.AllocateWritableMeshData(SimpleMeshFile.readLengthFromHeaderAndReset(_inputStream)));
			ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(SimpleMeshFile.asyncReadData), new SimpleMeshFile.AsyncReadInfo(_identifier, _inputStream, meshDatas, _offsetY, _mat, _bTextureArray, _markMeshesNoLongerReadable, _asyncCallback, _userCallbackData), null, true);
			result = null;
		}
		finally
		{
		}
		return result;
	}

	// Token: 0x06003FE8 RID: 16360 RVA: 0x001A2EA8 File Offset: 0x001A10A8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void asyncReadData(ThreadManager.TaskInfo _taskInfo)
	{
		try
		{
			SimpleMeshFile.AsyncReadInfo asyncReadInfo = (SimpleMeshFile.AsyncReadInfo)_taskInfo.parameter;
			try
			{
				using (asyncReadInfo.inputStream)
				{
					SimpleMeshFile.readData(asyncReadInfo.meshDatas, asyncReadInfo.inputStream, asyncReadInfo.bTextureArray);
					asyncReadInfo.success = true;
				}
			}
			catch (Exception e)
			{
				Log.Error((asyncReadInfo.identifier != null) ? ("Reading mesh " + asyncReadInfo.identifier + " failed:") : "Reading mesh failed:");
				Log.Exception(e);
			}
			ThreadManager.AddSingleTaskMainThread("SimpleMeshFile.CreateGameObjects", new ThreadManager.MainThreadTaskFunctionDelegate(SimpleMeshFile.mainThreadGoCallback), asyncReadInfo);
		}
		finally
		{
		}
	}

	// Token: 0x06003FE9 RID: 16361 RVA: 0x001A2F68 File Offset: 0x001A1168
	[PublicizedFrom(EAccessModifier.Private)]
	public static void mainThreadGoCallback(object _parameter)
	{
		try
		{
			SimpleMeshFile.AsyncReadInfo asyncReadInfo = (SimpleMeshFile.AsyncReadInfo)_parameter;
			using (asyncReadInfo.meshDatas)
			{
				GameObject go = null;
				if (asyncReadInfo.success)
				{
					go = SimpleMeshFile.CreateUnityObjects(SimpleMeshFile.createMeshInfo(asyncReadInfo.meshDatas, asyncReadInfo.markMeshesNoLongerReadable, asyncReadInfo.offsetY, asyncReadInfo.mat));
				}
				asyncReadInfo.goCallback(go, asyncReadInfo.userCallbackData);
			}
		}
		finally
		{
		}
	}

	// Token: 0x06003FEA RID: 16362 RVA: 0x001A2FEC File Offset: 0x001A11EC
	[PublicizedFrom(EAccessModifier.Private)]
	public static int readLengthFromHeaderAndReset(Stream _inputStream)
	{
		long position = _inputStream.Position;
		int result;
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(true))
		{
			pooledBinaryReader.SetBaseStream(_inputStream);
			pooledBinaryReader.ReadInt32();
			pooledBinaryReader.ReadByte();
			int num = (int)pooledBinaryReader.ReadInt16();
			_inputStream.Seek(position, SeekOrigin.Begin);
			result = num;
		}
		return result;
	}

	// Token: 0x06003FEB RID: 16363 RVA: 0x001A3050 File Offset: 0x001A1250
	[PublicizedFrom(EAccessModifier.Private)]
	public static void readData(SimpleMeshFile.SimpleMeshDataArray _meshDatas, Stream _inputStream, bool _bTextureArray)
	{
		try
		{
			using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(true))
			{
				pooledBinaryReader.SetBaseStream(_inputStream);
				pooledBinaryReader.ReadInt32();
				int version = (int)pooledBinaryReader.ReadByte();
				int num = (int)pooledBinaryReader.ReadInt16();
				new Mesh.MeshData[num];
				UVRectTiling[] uvMapping = (MeshDescription.meshes.Length != 0) ? MeshDescription.meshes[0].textureAtlas.uvMapping : new UVRectTiling[0];
				for (int i = 0; i < num; i++)
				{
					SimpleMeshFile.SimpleMeshDataArray.ReadFromReader(_meshDatas[i], version, pooledBinaryReader, uvMapping, _bTextureArray);
				}
			}
		}
		finally
		{
		}
	}

	// Token: 0x06003FEC RID: 16364 RVA: 0x001A30F8 File Offset: 0x001A12F8
	public static GameObject CreateUnityObjects(SimpleMeshInfo _meshInfo)
	{
		GameObject result;
		try
		{
			Mesh[] meshes = _meshInfo.meshes;
			string[] meshNames = _meshInfo.meshNames;
			float offsetY = _meshInfo.offsetY;
			Material material = _meshInfo.mat;
			if (!material)
			{
				material = MeshDescription.GetOpaqueMaterial();
			}
			GameObject gameObject = new GameObject();
			Transform transform = gameObject.transform;
			Vector3 localPosition = new Vector3(0f, offsetY, 0f);
			for (int i = 0; i < meshes.Length; i++)
			{
				GameObject gameObject2 = new GameObject(meshNames[i]);
				gameObject2.AddComponent<MeshFilter>().mesh = meshes[i];
				gameObject2.AddComponent<MeshRenderer>().material = material;
				Transform transform2 = gameObject2.transform;
				transform2.SetParent(transform, false);
				transform2.localPosition = localPosition;
			}
			result = gameObject;
		}
		finally
		{
		}
		return result;
	}

	// Token: 0x06003FED RID: 16365 RVA: 0x001A31B8 File Offset: 0x001A13B8
	public static Mesh[] ReadMesh(string _filename, float _offsetY = 0f, Material _mat = null, bool _bTextureArray = true, bool _markMeshesNoLongerReadable = false, object _userCallbackData = null, SimpleMeshFile.GameObjectMeshesReadCallback _asyncCallback = null)
	{
		Mesh[] result;
		try
		{
			result = SimpleMeshFile.ReadMesh(SdFile.OpenRead(_filename), _offsetY, _mat, _bTextureArray, _markMeshesNoLongerReadable, _userCallbackData, _asyncCallback, _filename);
		}
		finally
		{
		}
		return result;
	}

	// Token: 0x06003FEE RID: 16366 RVA: 0x001A31F0 File Offset: 0x001A13F0
	[PublicizedFrom(EAccessModifier.Private)]
	public static Mesh[] ReadMesh(Stream _inputStream, float _offsetY = 0f, Material _mat = null, bool _bTextureArray = true, bool _markMeshesNoLongerReadable = false, object _userCallbackData = null, SimpleMeshFile.GameObjectMeshesReadCallback _asyncCallback = null, string _identifier = null)
	{
		if (_asyncCallback == null)
		{
			try
			{
				using (SimpleMeshFile.SimpleMeshDataArray simpleMeshDataArray = new SimpleMeshFile.SimpleMeshDataArray(Mesh.AllocateWritableMeshData(SimpleMeshFile.readLengthFromHeaderAndReset(_inputStream))))
				{
					SimpleMeshFile.readData(simpleMeshDataArray, _inputStream, _bTextureArray);
					return SimpleMeshFile.SimpleMeshDataArray.ToMeshes(simpleMeshDataArray, _markMeshesNoLongerReadable);
				}
			}
			finally
			{
				if (_inputStream != null)
				{
					((IDisposable)_inputStream).Dispose();
				}
			}
		}
		SimpleMeshFile.SimpleMeshDataArray meshDatas = new SimpleMeshFile.SimpleMeshDataArray(Mesh.AllocateWritableMeshData(SimpleMeshFile.readLengthFromHeaderAndReset(_inputStream)));
		ThreadManager.AddSingleTask(new ThreadManager.TaskFunctionDelegate(SimpleMeshFile.asyncReadMesh), new SimpleMeshFile.AsyncReadInfo(_identifier, _inputStream, meshDatas, _offsetY, _mat, _bTextureArray, _markMeshesNoLongerReadable, _asyncCallback, _userCallbackData), null, true);
		return null;
	}

	// Token: 0x06003FEF RID: 16367 RVA: 0x001A3294 File Offset: 0x001A1494
	[PublicizedFrom(EAccessModifier.Private)]
	public static void asyncReadMesh(ThreadManager.TaskInfo _taskInfo)
	{
		SimpleMeshFile.AsyncReadInfo asyncReadInfo = (SimpleMeshFile.AsyncReadInfo)_taskInfo.parameter;
		try
		{
			using (asyncReadInfo.inputStream)
			{
				SimpleMeshFile.readData(asyncReadInfo.meshDatas, asyncReadInfo.inputStream, asyncReadInfo.bTextureArray);
				asyncReadInfo.success = true;
			}
		}
		catch (Exception e)
		{
			Log.Error((asyncReadInfo.identifier != null) ? ("Reading mesh " + asyncReadInfo.identifier + " failed:") : "Reading mesh failed:");
			Log.Exception(e);
		}
		ThreadManager.AddSingleTaskMainThread("SimpleMeshFile.ReadMesh", new ThreadManager.MainThreadTaskFunctionDelegate(SimpleMeshFile.mainThreadMeshCallback), asyncReadInfo);
	}

	// Token: 0x06003FF0 RID: 16368 RVA: 0x001A3344 File Offset: 0x001A1544
	[PublicizedFrom(EAccessModifier.Private)]
	public static void mainThreadMeshCallback(object _parameter)
	{
		SimpleMeshFile.AsyncReadInfo asyncReadInfo = (SimpleMeshFile.AsyncReadInfo)_parameter;
		using (SimpleMeshFile.SimpleMeshDataArray meshDatas = asyncReadInfo.meshDatas)
		{
			SimpleMeshInfo meshInfo = SimpleMeshFile.createMeshInfo(meshDatas, asyncReadInfo.markMeshesNoLongerReadable, asyncReadInfo.offsetY, asyncReadInfo.mat);
			asyncReadInfo.meshCallback(meshInfo, asyncReadInfo.userCallbackData);
		}
	}

	// Token: 0x06003FF1 RID: 16369 RVA: 0x001A33A8 File Offset: 0x001A15A8
	[PublicizedFrom(EAccessModifier.Private)]
	public static SimpleMeshInfo createMeshInfo(SimpleMeshFile.SimpleMeshDataArray _meshDatas, bool _markMeshesNoLongerReadable, float _offsetY, Material _mat)
	{
		string[] array = new string[_meshDatas.Length];
		Mesh[] meshes = SimpleMeshFile.SimpleMeshDataArray.ToMeshes(_meshDatas, _markMeshesNoLongerReadable);
		for (int i = 0; i < _meshDatas.Length; i++)
		{
			array[i] = _meshDatas[i].Name;
		}
		return new SimpleMeshInfo(array, meshes, _offsetY, _mat);
	}

	// Token: 0x04003356 RID: 13142
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSaveFileVersion = 6;

	// Token: 0x04003357 RID: 13143
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cHeader = 1835365224;

	// Token: 0x0200088C RID: 2188
	// (Invoke) Token: 0x06003FF3 RID: 16371
	public delegate void GameObjectLoadedCallback(GameObject _go, object _userCallbackData);

	// Token: 0x0200088D RID: 2189
	[PublicizedFrom(EAccessModifier.Private)]
	public class AsyncReadInfo
	{
		// Token: 0x06003FF6 RID: 16374 RVA: 0x001A33F5 File Offset: 0x001A15F5
		public AsyncReadInfo(string _identifier, Stream _inputStream, SimpleMeshFile.SimpleMeshDataArray _meshDatas, float _offsetY, Material _mat, bool _bTextureArray, bool _markMeshesNoLongerReadable, SimpleMeshFile.GameObjectMeshesReadCallback _callback, object _userCallbackData) : this(_identifier, _inputStream, _meshDatas, _offsetY, _mat, _bTextureArray, _markMeshesNoLongerReadable)
		{
			this.meshCallback = _callback;
			this.userCallbackData = _userCallbackData;
		}

		// Token: 0x06003FF7 RID: 16375 RVA: 0x001A3418 File Offset: 0x001A1618
		public AsyncReadInfo(string _identifier, Stream _inputStream, SimpleMeshFile.SimpleMeshDataArray _meshDatas, float _offsetY, Material _mat, bool _bTextureArray, bool _markMeshesNoLongerReadable, SimpleMeshFile.GameObjectLoadedCallback _callback, object _userCallbackData) : this(_identifier, _inputStream, _meshDatas, _offsetY, _mat, _bTextureArray, _markMeshesNoLongerReadable)
		{
			this.goCallback = _callback;
			this.userCallbackData = _userCallbackData;
		}

		// Token: 0x06003FF8 RID: 16376 RVA: 0x001A343C File Offset: 0x001A163C
		[PublicizedFrom(EAccessModifier.Private)]
		public AsyncReadInfo(string _identifier, Stream _inputStream, SimpleMeshFile.SimpleMeshDataArray _meshDatas, float _offsetY, Material _mat, bool _bTextureArray, bool _markMeshesNoLongerReadable)
		{
			this.identifier = _identifier;
			this.inputStream = _inputStream;
			this.meshDatas = _meshDatas;
			this.offsetY = _offsetY;
			this.mat = _mat;
			this.bTextureArray = _bTextureArray;
			this.markMeshesNoLongerReadable = _markMeshesNoLongerReadable;
			this.success = false;
		}

		// Token: 0x04003358 RID: 13144
		public readonly string identifier;

		// Token: 0x04003359 RID: 13145
		public readonly Stream inputStream;

		// Token: 0x0400335A RID: 13146
		public readonly SimpleMeshFile.SimpleMeshDataArray meshDatas;

		// Token: 0x0400335B RID: 13147
		public readonly float offsetY;

		// Token: 0x0400335C RID: 13148
		public readonly Material mat;

		// Token: 0x0400335D RID: 13149
		public readonly bool bTextureArray;

		// Token: 0x0400335E RID: 13150
		public readonly bool markMeshesNoLongerReadable;

		// Token: 0x0400335F RID: 13151
		public readonly SimpleMeshFile.GameObjectLoadedCallback goCallback;

		// Token: 0x04003360 RID: 13152
		public readonly SimpleMeshFile.GameObjectMeshesReadCallback meshCallback;

		// Token: 0x04003361 RID: 13153
		public readonly object userCallbackData;

		// Token: 0x04003362 RID: 13154
		public bool success;
	}

	// Token: 0x0200088E RID: 2190
	// (Invoke) Token: 0x06003FFA RID: 16378
	public delegate void GameObjectMeshesReadCallback(SimpleMeshInfo _meshInfo, object _userCallbackData);

	// Token: 0x0200088F RID: 2191
	[PublicizedFrom(EAccessModifier.Private)]
	public sealed class SimpleMeshDataArray : IDisposable
	{
		// Token: 0x06003FFD RID: 16381 RVA: 0x001A348C File Offset: 0x001A168C
		public SimpleMeshDataArray(Mesh.MeshDataArray _array)
		{
			this.meshData = _array;
			this.names = new string[this.meshData.Length];
			this.wrappers = new SimpleMeshFile.SimpleMeshDataArray.SimpleMeshDataWrapper[this.meshData.Length];
			for (int i = 0; i < this.wrappers.Length; i++)
			{
				this.wrappers[i] = new SimpleMeshFile.SimpleMeshDataArray.SimpleMeshDataWrapper(this, i);
			}
		}

		// Token: 0x06003FFE RID: 16382 RVA: 0x001A34F8 File Offset: 0x001A16F8
		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			this.DisposeMeshData();
			this.names = null;
			this.wrappers = null;
		}

		// Token: 0x06003FFF RID: 16383 RVA: 0x001A351E File Offset: 0x001A171E
		[PublicizedFrom(EAccessModifier.Private)]
		public void DisposeMeshData()
		{
			if (this.meshDataDisposed)
			{
				return;
			}
			this.meshDataDisposed = true;
			this.meshData.Dispose();
			this.meshData = default(Mesh.MeshDataArray);
		}

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x06004000 RID: 16384 RVA: 0x001A3547 File Offset: 0x001A1747
		public int Length
		{
			get
			{
				return this.meshData.Length;
			}
		}

		// Token: 0x1700069A RID: 1690
		public SimpleMeshFile.SimpleMeshDataArray.SimpleMeshDataWrapper this[int i]
		{
			get
			{
				return this.wrappers[i];
			}
		}

		// Token: 0x06004002 RID: 16386 RVA: 0x001A3562 File Offset: 0x001A1762
		public void ApplyAndDisposeWritableMeshData(Mesh[] meshes, MeshUpdateFlags flags = MeshUpdateFlags.Default)
		{
			Mesh.ApplyAndDisposeWritableMeshData(this.meshData, meshes, flags);
			this.meshDataDisposed = true;
		}

		// Token: 0x06004003 RID: 16387 RVA: 0x001A3578 File Offset: 0x001A1778
		public static void ReadFromReader(SimpleMeshFile.SimpleMeshDataArray.SimpleMeshDataWrapper _meshDataWrapper, int _version, BinaryReader _br, UVRectTiling[] _uvMapping, bool _bTextureArray)
		{
			try
			{
				Mesh.MeshData meshData = _meshDataWrapper.MeshData;
				_meshDataWrapper.Name = ((_version > 1) ? _br.ReadString() : "mesh");
				long position = _br.BaseStream.Position;
				int num = (int)_br.ReadUInt32();
				int num2;
				if (_version < 6)
				{
					num2 = 6;
				}
				else
				{
					num2 = 12;
				}
				_br.BaseStream.Seek((long)(num * num2), SeekOrigin.Current);
				int num3 = (int)_br.ReadUInt32();
				int num4 = 0;
				if (_version > 2)
				{
					num4 += 2;
				}
				if (_version > 4)
				{
					num4++;
				}
				if (_version > 3)
				{
					num4++;
				}
				num4 += 4;
				_br.BaseStream.Seek((long)(num3 * num4), SeekOrigin.Current);
				int num5 = (int)_br.ReadUInt32();
				int num6 = 2;
				_br.BaseStream.Seek((long)(num5 * num6), SeekOrigin.Current);
				_br.BaseStream.Seek(position, SeekOrigin.Begin);
				VertexAttributeDescriptor[] attributes = new VertexAttributeDescriptor[]
				{
					new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
					new VertexAttributeDescriptor(VertexAttribute.Normal, VertexAttributeFormat.Float32, 3, 1),
					new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 2),
					new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float32, 4, 3)
				};
				meshData.SetVertexBufferParams(num, attributes);
				meshData.SetIndexBufferParams(num5, IndexFormat.UInt16);
				num = (int)_br.ReadUInt32();
				NativeArray<Vector3> vertexData = meshData.GetVertexData<Vector3>(0);
				for (int i = 0; i < num; i++)
				{
					if (_version < 6)
					{
						vertexData[i] = new Vector3((float)_br.ReadInt16() / 100f, (float)_br.ReadInt16() / 100f, (float)_br.ReadInt16() / 100f);
					}
					else
					{
						vertexData[i] = new Vector3(_br.ReadSingle(), _br.ReadSingle(), _br.ReadSingle());
					}
				}
				num3 = (int)_br.ReadUInt32();
				NativeArray<Vector2> vertexData2 = meshData.GetVertexData<Vector2>(2);
				NativeArray<Color> vertexData3 = meshData.GetVertexData<Color>(3);
				for (int j = 0; j < num3; j++)
				{
					int num7 = 0;
					int num8 = 0;
					if (_version > 2)
					{
						num7 = (int)_br.ReadInt16();
					}
					if (_version > 4)
					{
						num8 = (int)_br.ReadByte();
					}
					int num9 = (num7 >= 0 && num7 < _uvMapping.Length) ? (_uvMapping[num7].index + num8) : 0;
					bool flag = false;
					if (_version > 3)
					{
						flag = _br.ReadBoolean();
					}
					vertexData2[j] = new Vector2((float)_br.ReadUInt16() / 10000f, (float)_br.ReadUInt16() / 10000f);
					if (!_bTextureArray && num9 >= 0 && num9 < _uvMapping.Length)
					{
						ref NativeArray<Vector2> ptr = ref vertexData2;
						int index = j;
						ptr[index] += new Vector2(_uvMapping[num9].uv.x, _uvMapping[num9].uv.y);
					}
					vertexData3[j] = new Color(0f, (float)num9, 0f, (float)(flag ? 1 : 0));
				}
				num5 = (int)_br.ReadUInt32();
				NativeArray<ushort> indexData = meshData.GetIndexData<ushort>();
				for (int k = 0; k < num5; k++)
				{
					indexData[k] = _br.ReadUInt16();
				}
				meshData.subMeshCount = 1;
				meshData.SetSubMesh(0, new SubMeshDescriptor(0, num5, MeshTopology.Triangles), MeshUpdateFlags.Default);
			}
			finally
			{
			}
		}

		// Token: 0x06004004 RID: 16388 RVA: 0x001A38A4 File Offset: 0x001A1AA4
		public static Mesh[] ToMeshes(SimpleMeshFile.SimpleMeshDataArray _meshDataArray, bool _markMeshesNoLongerReadable)
		{
			Mesh[] array = new Mesh[_meshDataArray.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Mesh mesh = new Mesh();
				array[i] = mesh;
				mesh.name = "Simple";
			}
			_meshDataArray.ApplyAndDisposeWritableMeshData(array, MeshUpdateFlags.Default);
			foreach (Mesh mesh2 in array)
			{
				mesh2.RecalculateNormals();
				mesh2.RecalculateBounds();
				GameUtils.SetMeshVertexAttributes(mesh2, true);
				mesh2.UploadMeshData(_markMeshesNoLongerReadable);
			}
			return array;
		}

		// Token: 0x04003363 RID: 13155
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disposed;

		// Token: 0x04003364 RID: 13156
		[PublicizedFrom(EAccessModifier.Private)]
		public Mesh.MeshDataArray meshData;

		// Token: 0x04003365 RID: 13157
		[PublicizedFrom(EAccessModifier.Private)]
		public bool meshDataDisposed;

		// Token: 0x04003366 RID: 13158
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] names;

		// Token: 0x04003367 RID: 13159
		[PublicizedFrom(EAccessModifier.Private)]
		public SimpleMeshFile.SimpleMeshDataArray.SimpleMeshDataWrapper[] wrappers;

		// Token: 0x02000890 RID: 2192
		public readonly struct SimpleMeshDataWrapper
		{
			// Token: 0x06004005 RID: 16389 RVA: 0x001A391A File Offset: 0x001A1B1A
			public SimpleMeshDataWrapper(SimpleMeshFile.SimpleMeshDataArray _array, int _offset)
			{
				this.array = _array;
				this.offset = _offset;
			}

			// Token: 0x1700069B RID: 1691
			// (get) Token: 0x06004006 RID: 16390 RVA: 0x001A392A File Offset: 0x001A1B2A
			public Mesh.MeshData MeshData
			{
				get
				{
					return this.array.meshData[this.offset];
				}
			}

			// Token: 0x1700069C RID: 1692
			// (get) Token: 0x06004007 RID: 16391 RVA: 0x001A3942 File Offset: 0x001A1B42
			// (set) Token: 0x06004008 RID: 16392 RVA: 0x001A3956 File Offset: 0x001A1B56
			public string Name
			{
				get
				{
					return this.array.names[this.offset];
				}
				set
				{
					this.array.names[this.offset] = value;
				}
			}

			// Token: 0x04003368 RID: 13160
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly SimpleMeshFile.SimpleMeshDataArray array;

			// Token: 0x04003369 RID: 13161
			[PublicizedFrom(EAccessModifier.Private)]
			public readonly int offset;
		}
	}
}
