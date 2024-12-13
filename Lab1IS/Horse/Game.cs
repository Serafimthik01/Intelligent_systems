using System.Collections.Generic;

public class Game
{
    public int n;
    public int[,] desk;
    public int[,] step = new int[8, 2] { { 1, -2 }, { 2, -1 }, { 2, 1 }, { 1, 2 }, { -1, 2 }, { -2, 1 }, { -2, -1 }, { -1, -2 } };

    public Game(int x, int y)
    {
        desk = new int[x, y];
    }

    public bool KnightTour(int x, int y)
    {
        if (x < 0 || x >= desk.GetLength(0) || y < 0 || y >= desk.GetLength(1))
            return false;

        if (desk[x, y] != 0)
            return false;

        n++;
        desk[x, y] = n;

        if (n == desk.Length)
        {
            return true;
        }

        List<(int, int)> nextMoves = GetNextMoves(x, y);
        nextMoves.Sort((a, b) =>
        {
            int countA = CountAccessibleCells(a.Item1, a.Item2);
            int countB = CountAccessibleCells(b.Item1, b.Item2);
            return countA.CompareTo(countB);
        });

        foreach (var (nextX, nextY) in nextMoves)
        {
            if (KnightTour(nextX, nextY))
            {
                return true;
            }
        }

        n--;
        desk[x, y] = 0;

        return false;
    }

    private List<(int, int)> GetNextMoves(int x, int y)
    {
        List<(int, int)> moves = new List<(int, int)>();

        for (int i = 0; i < 8; i++)
        {
            int nextX = x + step[i, 0];
            int nextY = y + step[i, 1];

            if (nextX >= 0 && nextX < desk.GetLength(0) && nextY >= 0 && nextY < desk.GetLength(1) && desk[nextX, nextY] == 0)
            {
                moves.Add((nextX, nextY));
            }
        }

        return moves;
    }

    private int CountAccessibleCells(int x, int y)
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            int nextX = x + step[i, 0];
            int nextY = y + step[i, 1];

            if (nextX >= 0 && nextX < desk.GetLength(0) && nextY >= 0 && nextY < desk.GetLength(1) && desk[nextX, nextY] == 0)
            {
                count++;
            }
        }

        return count;
    }
}