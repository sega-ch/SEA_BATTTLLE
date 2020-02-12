using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameField : MonoBehaviour
{
    public enum CellState
    {
        Empty, Misdelivered, Occupied, Misplaced, Hit
    }
    public GameObject cellPrefab;
    public float bottomLeftX, bottomLeftY;
    static Bounds[,] BoundsOfCells;
    static int[,] fieldBody = new int[10, 10];
    static float cellSize;
    static Vector3 bottomLeftCorner;
    // Start is called before the first frame update
    void Start()
    {
        var sprRenderer = cellPrefab.GetComponent<SpriteRenderer>();
        cellSize = sprRenderer.bounds.size.x;

        BoundsOfCells = new Bounds[Height(), Width()];
        GenerateField();
        
    }
    void GenerateField() // Генерация игрового поля
    {
        for (int i = 0; i < Height(); i++)
        {
            for (int j = 0; j < Width(); j++)
            {
                var cellPos = new Vector2(bottomLeftX + j * cellSize, bottomLeftY + i * cellSize);
                Instantiate(cellPrefab, cellPos, Quaternion.identity);
                var cellBounds = new Bounds(cellPos, new Vector2(cellSize, cellSize));
                BoundsOfCells[i, j] = cellBounds;
            }
        }
    }
    static int Width()
    {
        return fieldBody.GetLength(1);
    }
    static int Height()
    {
        return fieldBody.GetLength(0);
    }
    public static void CheckShipLocation(Ship ship, Vector3 mousePosition) // Проверяем положение корабля
    {
        var bottomLeftCell = BoundsOfCells[0, 0];
        var upperRightCell = BoundsOfCells[Height() - 1, Width() - 1];
        bottomLeftCorner = bottomLeftCell.min;
        var upperRightCorner = upperRightCell.max;

        var isOverField = mousePosition.x > bottomLeftCorner.x & mousePosition.x < upperRightCorner.x & mousePosition.y > bottomLeftCorner.y & mousePosition.y < upperRightCorner.y;
        if (!isOverField)
        {
            ship.IsPositionCorrect = false;
            ship.IsWithInCell = false;
            return;
        }
        var cellNormalPos = GetCellNormalPosition(mousePosition);
        var x = (int)cellNormalPos.x;
        var y = (int)cellNormalPos.y;
        ship.IsPositionCorrect = Appropriate(ship, x, y);
        ship.IsWithInCell = true;
        ship.CellPosition = BoundsOfCells[y, x].center;
    }
    static bool Appropriate(Ship ship, int x, int y)
    {
        int[] dx = { -1, -1, -1, 0, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };
        for (int i = 0; i < ship.FloorsNum(); i++)
        {
            if (!IsWithinMatrix(x, y))
            {
                return false;
            }
            for (int j = 0; j < 9; j++)
            {
                int shiftX = x + dx[j];
                int shiftY = y + dy[j];
                bool isPositionCorrect;
                bool isNotOutOfArray = shiftX >= 0 & shiftX < Width() & shiftY >= 0 & shiftY < Height();
                if (isNotOutOfArray)
                {
                    isPositionCorrect = IsWithinMatrix(x, y) & fieldBody[shiftY, shiftX] != (int)CellState.Occupied;
                    if (!isPositionCorrect) return false;
                }
            }
            if (ship.orientation == Ship.Orientation.Horizontal)
            {
                x++;
            }
            else y--;
        }
        return true;
    }
    static bool IsWithinMatrix(int x, int y)
    {
        return x >= 0 & x < Width() & y >= 0 & y < Height();
    }
    static void SetCellStateUnderneathShip(Ship ship, CellState cellState)
    {
        var cellNormalPos = GetCellNormalPosition(ship.CellPosition);
        int x = (int)cellNormalPos.x, y = (int)cellNormalPos.y;
        for (int i = 0; i < ship.FloorsNum(); i++)
        {
            if (x >= Width() || y >= Height()) break;
            fieldBody[y, x] = (int)cellState;
            if (ship.orientation == Ship.Orientation.Horizontal)
            {
                x++;
            }
            else y--;
        }


        //for (int i = 0; i < height(); i++)
        //{
        //    string line = "";

        //    debug.log(line);
        //    for (int j = 0; j < width(); j++)
        //    {

        //    }
        //}

    }
    static Vector2 GetCellNormalPosition(Vector2 worldPosition)
    {
        var dx = worldPosition.x - bottomLeftCorner.x;
        var dy = worldPosition.y - bottomLeftCorner.y;
        var x = (int)(dx / cellSize);
        var y = (int)(dy / cellSize);
        return new Vector2(x, y);
    }
    public static void RegisterShip(Ship ship)
    {
        SetCellStateUnderneathShip(ship, CellState.Occupied);
    }
    public static void TakeShipOff(Ship ship)
    {
        if(ship.IsPositionCorrect) SetCellStateUnderneathShip(ship, CellState.Empty);
    }
}
