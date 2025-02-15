using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    public AudioClip lineClearSound;
    public ParticleSystem explosionEffect; 

    public RectInt Bounds
    {
        get
        {
            Vector2Int pos = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(pos, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();

        if (TetroManager.Instance != null)
        {
            TetroManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("TetroManager instance not found. Lütfen sahnede TetroManager scriptine sahip bir GameObject olduğundan emin olun.");
        }
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        int clearedLines = 0;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                clearedLines++;
            }
            else
            {
                row++;
            }
        }

        if (clearedLines > 0 && TetroManager.Instance != null)
        {
            int[] lineClearScores = { 0, 100, 300, 500, 800 };
            int points = (clearedLines < lineClearScores.Length) ? lineClearScores[clearedLines] : 800;
            TetroManager.Instance.AddScore(points);
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;


        if (explosionEffect != null)
        {
            Vector3Int cellPosition = new Vector3Int(0, row, 0);
            Vector3 worldPosition = tilemap.CellToWorld(cellPosition);
            worldPosition.x = tilemap.transform.position.x;
            worldPosition.y += 0.5f;
            Instantiate(explosionEffect, worldPosition, Quaternion.identity);
        }

        if (lineClearSound != null)
        {
            AudioSource.PlayClipAtPoint(lineClearSound, Camera.main.transform.position);
        }

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        for (int r = row; r < bounds.yMax; r++)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int posAbove = new Vector3Int(col, r + 1, 0);
                TileBase above = tilemap.GetTile(posAbove);

                Vector3Int pos = new Vector3Int(col, r, 0);
                tilemap.SetTile(pos, above);
            }
        }
    }
}