using System;

// Token: 0x02000AF8 RID: 2808
public static class TileEntityExtensions
{
	// Token: 0x06005675 RID: 22133 RVA: 0x00233B64 File Offset: 0x00231D64
	public static T GetSelfOrFeature<T>(this ITileEntity _te) where T : class
	{
		T result;
		_te.TryGetSelfOrFeature(out result);
		return result;
	}

	// Token: 0x06005676 RID: 22134 RVA: 0x00233B7C File Offset: 0x00231D7C
	public static bool TryGetSelfOrFeature<T>(this ITileEntity _te, out T _typedTe) where T : class
	{
		if (_te == null)
		{
			_typedTe = default(T);
			return false;
		}
		T t = _te as T;
		if (t != null)
		{
			_typedTe = t;
			return true;
		}
		TileEntityComposite tileEntityComposite = _te as TileEntityComposite;
		if (tileEntityComposite != null)
		{
			_typedTe = tileEntityComposite.GetFeature<T>();
			return _typedTe != null;
		}
		ITileEntityFeature tileEntityFeature = _te as ITileEntityFeature;
		if (tileEntityFeature != null)
		{
			_typedTe = tileEntityFeature.Parent.GetFeature<T>();
			return _typedTe != null;
		}
		_typedTe = default(T);
		return false;
	}
}
