using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a grid of plane tiles around a moving center transform and swaps their textures
/// based on deterministic Perlin noise so nearby tiles blend naturally.
/// </summary>
public class ProceduralTileGenerator : MonoBehaviour
{
    [Header("Generation Source")]
    [Tooltip("Transform that drives new tile generation as it moves across the world.")]
    [SerializeField] private Transform _centerTransform;

    [Tooltip("Prefab containing a plane mesh and renderer. Will be instanced per tile.")]
    [SerializeField] private GameObject _tilePrefab;

    [Tooltip("Half the number of tiles spawned along each axis (radius). 0 = single tile.")]
    [Min(0)]
    [SerializeField] private int _tileRadius = 4;

    [Tooltip("World-space size applied to each tile instance (X = width, Y = depth).")]
    [SerializeField] private Vector2 _tileSize = new Vector2(5f, 5f);

    [Tooltip("Automatically populate the initial tile set on enable when a center is present.")]
    [SerializeField] private bool _generateOnEnable = true;

    [Header("Noise Settings")]
    [Tooltip("Scale factor for the Perlin sampling. Smaller values create slower texture variance.")]
    [SerializeField] private float _noiseScale = 0.15f;

    [Tooltip("Offset applied to the Perlin sample so multiple generators can differ.")]
    [SerializeField] private Vector2 _noiseOffset = Vector2.zero;

    [Tooltip("Textures randomly assigned per tile using deterministic Perlin noise.")]
    [SerializeField] private Texture[] _tileTextures = default;

    private readonly Dictionary<Vector2Int, GameObject> _activeTiles = new Dictionary<Vector2Int, GameObject>();
    private readonly List<Vector2Int> _cleanupBuffer = new List<Vector2Int>(32);

    private Vector2Int _currentCenterCoord;
    private MaterialPropertyBlock _propertyBlock;

    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();

        if (_tileSize.x <= 0f || _tileSize.y <= 0f)
        {
            _tileSize = Vector2.one;
            Debug.LogWarning("ProceduralTileGenerator: Tile size must be positive. Reverting to 1x1.");
        }
    }

    private void OnEnable()
    {
        if (_generateOnEnable && _centerTransform != null)
        {
            _currentCenterCoord = WorldToTileCoord(_centerTransform.position);
            RefreshTiles();
        }
    }

    private void OnDisable()
    {
        ClearTiles();
    }

    private void Update()
    {
        if (_centerTransform == null || _tilePrefab == null)
        {
            return;
        }

        Vector2Int newCoord = WorldToTileCoord(_centerTransform.position);
        if (_activeTiles.Count == 0)
        {
            _currentCenterCoord = newCoord;
            RefreshTiles();
            return;
        }

        if (newCoord != _currentCenterCoord)
        {
            _currentCenterCoord = newCoord;
            RefreshTiles();
        }
    }

    /// <summary>
    /// Clears every generated tile immediately.
    /// </summary>
    public void ClearTiles()
    {
        foreach (GameObject tile in _activeTiles.Values)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }

        _activeTiles.Clear();
    }

    /// <summary>
    /// Forces a manual rebuild regardless of whether the center moved.
    /// </summary>
    public void ForceRegenerate()
    {
        if (_centerTransform == null)
        {
            Debug.LogWarning("ProceduralTileGenerator: Cannot regenerate without a center transform.");
            return;
        }

        _currentCenterCoord = WorldToTileCoord(_centerTransform.position);
        RefreshTiles();
    }

    private void RefreshTiles()
    {
        if (_tilePrefab == null)
        {
            Debug.LogWarning("ProceduralTileGenerator: Unable to generate tiles without a prefab reference.");
            return;
        }

        EnsureTilesAround(_currentCenterCoord);
        CleanupDistantTiles(_currentCenterCoord);
    }

    private void EnsureTilesAround(Vector2Int centerCoord)
    {
        for (int x = -_tileRadius; x <= _tileRadius; x++)
        {
            for (int y = -_tileRadius; y <= _tileRadius; y++)
            {
                Vector2Int coord = new Vector2Int(centerCoord.x + x, centerCoord.y + y);

                if (_activeTiles.ContainsKey(coord))
                {
                    continue;
                }

                GameObject tileInstance = Instantiate(_tilePrefab, transform);
                tileInstance.name = $"Tile_{coord.x}_{coord.y}";
                tileInstance.transform.position = TileCoordToWorld(coord);

                Renderer tileRenderer = tileInstance.GetComponentInChildren<Renderer>();
                if (tileRenderer != null)
                {
                    ApplyTileScale(tileInstance, tileRenderer);
                    ApplyTexture(tileRenderer, coord);
                }
                else
                {
                    Vector3 localScale = tileInstance.transform.localScale;
                    localScale.x = _tileSize.x;
                    localScale.z = _tileSize.y;
                    tileInstance.transform.localScale = localScale;
                    Debug.LogWarning("ProceduralTileGenerator: Tile prefab is missing a Renderer component.");
                }

                _activeTiles.Add(coord, tileInstance);
            }
        }
    }

    private void CleanupDistantTiles(Vector2Int centerCoord)
    {
        _cleanupBuffer.Clear();

        foreach (KeyValuePair<Vector2Int, GameObject> entry in _activeTiles)
        {
            if (Mathf.Abs(entry.Key.x - centerCoord.x) > _tileRadius ||
                Mathf.Abs(entry.Key.y - centerCoord.y) > _tileRadius)
            {
                _cleanupBuffer.Add(entry.Key);
            }
        }

        for (int i = 0; i < _cleanupBuffer.Count; i++)
        {
            Vector2Int coord = _cleanupBuffer[i];
            if (_activeTiles.TryGetValue(coord, out GameObject tile) && tile != null)
            {
                Destroy(tile);
            }

            _activeTiles.Remove(coord);
        }
    }

    private void ApplyTexture(Renderer renderer, Vector2Int coord)
    {
        if (_tileTextures == null || _tileTextures.Length == 0)
        {
            return;
        }

        float perlinValue = Mathf.PerlinNoise(
            (coord.x + _noiseOffset.x) * _noiseScale,
            (coord.y + _noiseOffset.y) * _noiseScale);

        float hashValue = Mathf.Abs(Mathf.Sin(Vector2.Dot(new Vector2(coord.x, coord.y), new Vector2(12.9898f, 78.233f))) * 43758.5453f);
        float pseudoRandom = Mathf.Repeat(hashValue, 1f);
        float blendedSample = Mathf.Clamp01((perlinValue + pseudoRandom) * 0.5f);

        int textureIndex = Mathf.Clamp(Mathf.FloorToInt(blendedSample * _tileTextures.Length), 0, _tileTextures.Length - 1);

        renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.Clear();
        Texture selectedTexture = _tileTextures[textureIndex];
        _propertyBlock.SetTexture("_MainTex", selectedTexture);
        _propertyBlock.SetTexture("_BaseMap", selectedTexture);
        renderer.SetPropertyBlock(_propertyBlock);
    }

    private void ApplyTileScale(GameObject tileInstance, Renderer renderer)
    {
        if (renderer == null)
        {
            return;
        }

        Vector3 rendererSize = renderer.bounds.size;
        Vector3 currentScale = tileInstance.transform.localScale;

        float scaleX = rendererSize.x <= 0.0001f ? 1f : _tileSize.x / rendererSize.x;
        float scaleZ = rendererSize.z <= 0.0001f ? 1f : _tileSize.y / rendererSize.z;

        tileInstance.transform.localScale = new Vector3(currentScale.x * scaleX, currentScale.y, currentScale.z * scaleZ);
    }

    private Vector2Int WorldToTileCoord(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / Mathf.Max(_tileSize.x, 0.0001f));
        int y = Mathf.FloorToInt(worldPosition.z / Mathf.Max(_tileSize.y, 0.0001f));
        return new Vector2Int(x, y);
    }

    private Vector3 TileCoordToWorld(Vector2Int coord)
    {
        float x = coord.x * _tileSize.x;
        float z = coord.y * _tileSize.y;
        return new Vector3(x, 0f, z);
    }
}
