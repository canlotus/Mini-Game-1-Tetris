using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    private float stepDelay;
    public float lockDelay = 0.5f;
    private float stepTime, lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.data = data;
        this.position = position;

        rotationIndex = 0;
        stepDelay = TetroManager.pieceStepDelay;
        stepTime = Time.time + stepDelay;
        lockTime = 0f;

        cells = new Vector3Int[data.cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        board.Clear(this);
        lockTime += Time.deltaTime;
        stepDelay = TetroManager.pieceStepDelay;

        if (Time.time > stepTime) Step();
        board.Set(this);
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        if (!Move(Vector2Int.down))
        {
            lockTime += Time.deltaTime;

            if (lockTime >= lockDelay)
            {
                Lock();
            }
        }
        else
        {
            lockTime = 0f;
        }
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPos = position + (Vector3Int)translation;

        if (!board.IsValidPosition(this, newPos))
        {
            return false;
        }

        position = newPos;
        lockTime = 0f;
        return true;
    }

    private void Lock()
    {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
    }

    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        if (!board.IsValidPosition(this, position))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
            int y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }


    public void OnMoveLeftButton()
    {
        board.Clear(this);
        Move(Vector2Int.left);
        board.Set(this);
    }

    public void OnMoveRightButton()
    {
        board.Clear(this);
        Move(Vector2Int.right);
        board.Set(this);
    }

    public void OnRotateButton()
    {
        board.Clear(this);
        Rotate(-1);
        board.Set(this);
    }

    public void OnHardDropButton()
    {
        board.Clear(this);
        while (Move(Vector2Int.down)) ;
        Lock();
    }
}