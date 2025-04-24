using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Tilemaps;

[NativeHeader("Modules/Tilemap/Public/TilemapMarshalling.h")]
[NativeHeader("Modules/Tilemap/Public/TilemapTile.h")]
[NativeHeader("Runtime/Graphics/SpriteFrame.h")]
[NativeType(Header = "Modules/Tilemap/Public/Tilemap.h")]
[NativeHeader("Modules/Grid/Public/Grid.h")]
[NativeHeader("Modules/Grid/Public/GridMarshalling.h")]
[RequireComponent(typeof(Transform))]
public sealed class Tilemap : GridLayout
{
	public enum Orientation
	{
		XY = 0,
		XZ = 1,
		YX = 2,
		YZ = 3,
		ZX = 4,
		ZY = 5,
		Custom = 6
	}

	public extern Grid layoutGrid
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[NativeMethod(Name = "GetAttachedGrid")]
		get;
	}

	public BoundsInt cellBounds => new BoundsInt(origin, size);

	[NativeProperty("TilemapBoundsScripting")]
	public Bounds localBounds
	{
		get
		{
			get_localBounds_Injected(out var ret);
			return ret;
		}
	}

	[NativeProperty("TilemapFrameBoundsScripting")]
	internal Bounds localFrameBounds
	{
		get
		{
			get_localFrameBounds_Injected(out var ret);
			return ret;
		}
	}

	public extern float animationFrameRate
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Color color
	{
		get
		{
			get_color_Injected(out var ret);
			return ret;
		}
		set
		{
			set_color_Injected(ref value);
		}
	}

	public Vector3Int origin
	{
		get
		{
			get_origin_Injected(out var ret);
			return ret;
		}
		set
		{
			set_origin_Injected(ref value);
		}
	}

	public Vector3Int size
	{
		get
		{
			get_size_Injected(out var ret);
			return ret;
		}
		set
		{
			set_size_Injected(ref value);
		}
	}

	[NativeProperty(Name = "TileAnchorScripting")]
	public Vector3 tileAnchor
	{
		get
		{
			get_tileAnchor_Injected(out var ret);
			return ret;
		}
		set
		{
			set_tileAnchor_Injected(ref value);
		}
	}

	public extern Orientation orientation
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		get;
		[MethodImpl(MethodImplOptions.InternalCall)]
		set;
	}

	public Matrix4x4 orientationMatrix
	{
		[NativeMethod(Name = "GetTileOrientationMatrix")]
		get
		{
			get_orientationMatrix_Injected(out var ret);
			return ret;
		}
		[NativeMethod(Name = "SetOrientationMatrix")]
		set
		{
			set_orientationMatrix_Injected(ref value);
		}
	}

	public Vector3 GetCellCenterLocal(Vector3Int position)
	{
		return CellToLocalInterpolated(position) + CellToLocalInterpolated(tileAnchor);
	}

	public Vector3 GetCellCenterWorld(Vector3Int position)
	{
		return LocalToWorld(CellToLocalInterpolated(position) + CellToLocalInterpolated(tileAnchor));
	}

	internal Object GetTileAsset(Vector3Int position)
	{
		return GetTileAsset_Injected(ref position);
	}

	public TileBase GetTile(Vector3Int position)
	{
		return GetTileAsset(position) as TileBase;
	}

	public T GetTile<T>(Vector3Int position) where T : TileBase
	{
		return GetTileAsset(position) as T;
	}

	internal Object[] GetTileAssetsBlock(Vector3Int position, Vector3Int blockDimensions)
	{
		return GetTileAssetsBlock_Injected(ref position, ref blockDimensions);
	}

	public TileBase[] GetTilesBlock(BoundsInt bounds)
	{
		Object[] tileAssetsBlock = GetTileAssetsBlock(bounds.min, bounds.size);
		TileBase[] array = new TileBase[tileAssetsBlock.Length];
		for (int i = 0; i < tileAssetsBlock.Length; i++)
		{
			array[i] = (TileBase)tileAssetsBlock[i];
		}
		return array;
	}

	[FreeFunction(Name = "TilemapBindings::GetTileAssetsBlockNonAlloc", HasExplicitThis = true)]
	internal int GetTileAssetsBlockNonAlloc(Vector3Int startPosition, Vector3Int endPosition, Object[] tiles)
	{
		return GetTileAssetsBlockNonAlloc_Injected(ref startPosition, ref endPosition, tiles);
	}

	public int GetTilesBlockNonAlloc(BoundsInt bounds, TileBase[] tiles)
	{
		return GetTileAssetsBlockNonAlloc(bounds.min, bounds.size, tiles);
	}

	public int GetTilesRangeCount(Vector3Int startPosition, Vector3Int endPosition)
	{
		return GetTilesRangeCount_Injected(ref startPosition, ref endPosition);
	}

	[FreeFunction(Name = "TilemapBindings::GetTileAssetsRangeNonAlloc", HasExplicitThis = true)]
	internal int GetTileAssetsRangeNonAlloc(Vector3Int startPosition, Vector3Int endPosition, Vector3Int[] positions, Object[] tiles)
	{
		return GetTileAssetsRangeNonAlloc_Injected(ref startPosition, ref endPosition, positions, tiles);
	}

	public int GetTilesRangeNonAlloc(Vector3Int startPosition, Vector3Int endPosition, Vector3Int[] positions, TileBase[] tiles)
	{
		return GetTileAssetsRangeNonAlloc(startPosition, endPosition, positions, tiles);
	}

	internal void SetTileAsset(Vector3Int position, Object tile)
	{
		SetTileAsset_Injected(ref position, tile);
	}

	public void SetTile(Vector3Int position, TileBase tile)
	{
		SetTileAsset(position, tile);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void SetTileAssets(Vector3Int[] positionArray, Object[] tileArray);

	public void SetTiles(Vector3Int[] positionArray, TileBase[] tileArray)
	{
		SetTileAssets(positionArray, tileArray);
	}

	[NativeMethod(Name = "SetTileAssetsBlock")]
	private void INTERNAL_CALL_SetTileAssetsBlock(Vector3Int position, Vector3Int blockDimensions, Object[] tileArray)
	{
		INTERNAL_CALL_SetTileAssetsBlock_Injected(ref position, ref blockDimensions, tileArray);
	}

	public void SetTilesBlock(BoundsInt position, TileBase[] tileArray)
	{
		INTERNAL_CALL_SetTileAssetsBlock(position.min, position.size, tileArray);
	}

	[NativeMethod(Name = "SetTileChangeData")]
	public void SetTile(TileChangeData tileChangeData, bool ignoreLockFlags)
	{
		SetTile_Injected(ref tileChangeData, ignoreLockFlags);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "SetTileChangeDataArray")]
	public extern void SetTiles(TileChangeData[] tileChangeDataArray, bool ignoreLockFlags);

	public bool HasTile(Vector3Int position)
	{
		return GetTileAsset(position) != null;
	}

	[NativeMethod(Name = "RefreshTileAsset")]
	public void RefreshTile(Vector3Int position)
	{
		RefreshTile_Injected(ref position);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "TilemapBindings::RefreshTileAssetsNative", HasExplicitThis = true)]
	internal unsafe extern void RefreshTilesNative(void* positions, int count);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[NativeMethod(Name = "RefreshAllTileAssets")]
	public extern void RefreshAllTiles();

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern void SwapTileAsset(Object changeTile, Object newTile);

	public void SwapTile(TileBase changeTile, TileBase newTile)
	{
		SwapTileAsset(changeTile, newTile);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	internal extern bool ContainsTileAsset(Object tileAsset);

	public bool ContainsTile(TileBase tileAsset)
	{
		return ContainsTileAsset(tileAsset);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern int GetUsedTilesCount();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern int GetUsedSpritesCount();

	public int GetUsedTilesNonAlloc(TileBase[] usedTiles)
	{
		return Internal_GetUsedTilesNonAlloc(usedTiles);
	}

	public int GetUsedSpritesNonAlloc(Sprite[] usedSprites)
	{
		return Internal_GetUsedSpritesNonAlloc(usedSprites);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "TilemapBindings::GetUsedTilesNonAlloc", HasExplicitThis = true)]
	internal extern int Internal_GetUsedTilesNonAlloc(Object[] usedTiles);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[FreeFunction(Name = "TilemapBindings::GetUsedSpritesNonAlloc", HasExplicitThis = true)]
	internal extern int Internal_GetUsedSpritesNonAlloc(Object[] usedSprites);

	public Sprite GetSprite(Vector3Int position)
	{
		return GetSprite_Injected(ref position);
	}

	public Matrix4x4 GetTransformMatrix(Vector3Int position)
	{
		GetTransformMatrix_Injected(ref position, out var ret);
		return ret;
	}

	public void SetTransformMatrix(Vector3Int position, Matrix4x4 transform)
	{
		SetTransformMatrix_Injected(ref position, ref transform);
	}

	[NativeMethod(Name = "GetTileColor")]
	public Color GetColor(Vector3Int position)
	{
		GetColor_Injected(ref position, out var ret);
		return ret;
	}

	[NativeMethod(Name = "SetTileColor")]
	public void SetColor(Vector3Int position, Color color)
	{
		SetColor_Injected(ref position, ref color);
	}

	public TileFlags GetTileFlags(Vector3Int position)
	{
		return GetTileFlags_Injected(ref position);
	}

	public void SetTileFlags(Vector3Int position, TileFlags flags)
	{
		SetTileFlags_Injected(ref position, flags);
	}

	public void AddTileFlags(Vector3Int position, TileFlags flags)
	{
		AddTileFlags_Injected(ref position, flags);
	}

	public void RemoveTileFlags(Vector3Int position, TileFlags flags)
	{
		RemoveTileFlags_Injected(ref position, flags);
	}

	[NativeMethod(Name = "GetTileInstantiatedObject")]
	public GameObject GetInstantiatedObject(Vector3Int position)
	{
		return GetInstantiatedObject_Injected(ref position);
	}

	[NativeMethod(Name = "GetTileObjectToInstantiate")]
	public GameObject GetObjectToInstantiate(Vector3Int position)
	{
		return GetObjectToInstantiate_Injected(ref position);
	}

	[NativeMethod(Name = "SetTileColliderType")]
	public void SetColliderType(Vector3Int position, Tile.ColliderType colliderType)
	{
		SetColliderType_Injected(ref position, colliderType);
	}

	[NativeMethod(Name = "GetTileColliderType")]
	public Tile.ColliderType GetColliderType(Vector3Int position)
	{
		return GetColliderType_Injected(ref position);
	}

	[NativeMethod(Name = "GetTileAnimationFrameCount")]
	public int GetAnimationFrameCount(Vector3Int position)
	{
		return GetAnimationFrameCount_Injected(ref position);
	}

	[NativeMethod(Name = "GetTileAnimationFrame")]
	public int GetAnimationFrame(Vector3Int position)
	{
		return GetAnimationFrame_Injected(ref position);
	}

	[NativeMethod(Name = "SetTileAnimationFrame")]
	public void SetAnimationFrame(Vector3Int position, int frame)
	{
		SetAnimationFrame_Injected(ref position, frame);
	}

	[NativeMethod(Name = "GetTileAnimationTime")]
	public float GetAnimationTime(Vector3Int position)
	{
		return GetAnimationTime_Injected(ref position);
	}

	[NativeMethod(Name = "SetTileAnimationTime")]
	public void SetAnimationTime(Vector3Int position, float time)
	{
		SetAnimationTime_Injected(ref position, time);
	}

	public void FloodFill(Vector3Int position, TileBase tile)
	{
		FloodFillTileAsset(position, tile);
	}

	[NativeMethod(Name = "FloodFill")]
	private void FloodFillTileAsset(Vector3Int position, Object tile)
	{
		FloodFillTileAsset_Injected(ref position, tile);
	}

	public void BoxFill(Vector3Int position, TileBase tile, int startX, int startY, int endX, int endY)
	{
		BoxFillTileAsset(position, tile, startX, startY, endX, endY);
	}

	[NativeMethod(Name = "BoxFill")]
	private void BoxFillTileAsset(Vector3Int position, Object tile, int startX, int startY, int endX, int endY)
	{
		BoxFillTileAsset_Injected(ref position, tile, startX, startY, endX, endY);
	}

	public void InsertCells(Vector3Int position, Vector3Int insertCells)
	{
		InsertCells(position, insertCells.x, insertCells.y, insertCells.z);
	}

	public void InsertCells(Vector3Int position, int numColumns, int numRows, int numLayers)
	{
		InsertCells_Injected(ref position, numColumns, numRows, numLayers);
	}

	public void DeleteCells(Vector3Int position, Vector3Int deleteCells)
	{
		DeleteCells(position, deleteCells.x, deleteCells.y, deleteCells.z);
	}

	public void DeleteCells(Vector3Int position, int numColumns, int numRows, int numLayers)
	{
		DeleteCells_Injected(ref position, numColumns, numRows, numLayers);
	}

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ClearAllTiles();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void ResizeBounds();

	[MethodImpl(MethodImplOptions.InternalCall)]
	public extern void CompressBounds();

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_localBounds_Injected(out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_localFrameBounds_Injected(out Bounds ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_color_Injected(out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_color_Injected(ref Color value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_origin_Injected(out Vector3Int ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_origin_Injected(ref Vector3Int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_size_Injected(out Vector3Int ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_size_Injected(ref Vector3Int value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_tileAnchor_Injected(out Vector3 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_tileAnchor_Injected(ref Vector3 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void get_orientationMatrix_Injected(out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	[SpecialName]
	private extern void set_orientationMatrix_Injected(ref Matrix4x4 value);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Object GetTileAsset_Injected(ref Vector3Int position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Object[] GetTileAssetsBlock_Injected(ref Vector3Int position, ref Vector3Int blockDimensions);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetTileAssetsBlockNonAlloc_Injected(ref Vector3Int startPosition, ref Vector3Int endPosition, Object[] tiles);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetTilesRangeCount_Injected(ref Vector3Int startPosition, ref Vector3Int endPosition);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetTileAssetsRangeNonAlloc_Injected(ref Vector3Int startPosition, ref Vector3Int endPosition, Vector3Int[] positions, Object[] tiles);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetTileAsset_Injected(ref Vector3Int position, Object tile);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void INTERNAL_CALL_SetTileAssetsBlock_Injected(ref Vector3Int position, ref Vector3Int blockDimensions, Object[] tileArray);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetTile_Injected(ref TileChangeData tileChangeData, bool ignoreLockFlags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void RefreshTile_Injected(ref Vector3Int position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Sprite GetSprite_Injected(ref Vector3Int position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetTransformMatrix_Injected(ref Vector3Int position, out Matrix4x4 ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetTransformMatrix_Injected(ref Vector3Int position, ref Matrix4x4 transform);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void GetColor_Injected(ref Vector3Int position, out Color ret);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetColor_Injected(ref Vector3Int position, ref Color color);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern TileFlags GetTileFlags_Injected(ref Vector3Int position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetTileFlags_Injected(ref Vector3Int position, TileFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void AddTileFlags_Injected(ref Vector3Int position, TileFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void RemoveTileFlags_Injected(ref Vector3Int position, TileFlags flags);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern GameObject GetInstantiatedObject_Injected(ref Vector3Int position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern GameObject GetObjectToInstantiate_Injected(ref Vector3Int position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetColliderType_Injected(ref Vector3Int position, Tile.ColliderType colliderType);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern Tile.ColliderType GetColliderType_Injected(ref Vector3Int position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetAnimationFrameCount_Injected(ref Vector3Int position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern int GetAnimationFrame_Injected(ref Vector3Int position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetAnimationFrame_Injected(ref Vector3Int position, int frame);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern float GetAnimationTime_Injected(ref Vector3Int position);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void SetAnimationTime_Injected(ref Vector3Int position, float time);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void FloodFillTileAsset_Injected(ref Vector3Int position, Object tile);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void BoxFillTileAsset_Injected(ref Vector3Int position, Object tile, int startX, int startY, int endX, int endY);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void InsertCells_Injected(ref Vector3Int position, int numColumns, int numRows, int numLayers);

	[MethodImpl(MethodImplOptions.InternalCall)]
	private extern void DeleteCells_Injected(ref Vector3Int position, int numColumns, int numRows, int numLayers);
}
