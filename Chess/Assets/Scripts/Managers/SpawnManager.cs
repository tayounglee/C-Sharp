using UnityEditor;

using UnityEngine;

public class SpawnManager
{
    ChessBoard board;
    GameObject empty;

    public SpawnManager(ChessBoard board)
    {
        this.board = board;
    }

    public void InitialSpawn()
    {
        for (int i = 0; i < 8; ++i) {
            SpawnActor<Pawn>(i, 1, ChessTeam.White);
            SpawnActor<Pawn>(i, 6, ChessTeam.Black);
        }

        SpawnActor<King>(4, 0, ChessTeam.White);
        SpawnActor<Queen>(3, 0, ChessTeam.White);
        SpawnActor<Bishop>(2, 0, ChessTeam.White);
        SpawnActor<Bishop>(5, 0, ChessTeam.White);
        SpawnActor<Knight>(1, 0, ChessTeam.White);
        SpawnActor<Knight>(6, 0, ChessTeam.White);
        SpawnActor<Rook>(0, 0, ChessTeam.White);
        SpawnActor<Rook>(7, 0, ChessTeam.White);

        SpawnActor<King>(4, 7, ChessTeam.Black);
        SpawnActor<Queen>(3, 7, ChessTeam.Black);
        SpawnActor<Bishop>(2, 7, ChessTeam.Black);
        SpawnActor<Bishop>(5, 7, ChessTeam.Black);
        SpawnActor<Knight>(1, 7, ChessTeam.Black);
        SpawnActor<Knight>(6, 7, ChessTeam.Black);
        SpawnActor<Rook>(0, 7, ChessTeam.Black);
        SpawnActor<Rook>(7, 7, ChessTeam.Black);
    }

    public T SpawnActor<T>(GridIndex gridIndex, ChessTeam team) where T : Pieces
    {
        var go = Object.Instantiate(Empty);
        go.name = typeof(T).Name;

        var component = go.AddActorComponent<T>(team, gridIndex, board);
        board[gridIndex] = component;

        go.transform.SetParent(board.transform, false);

        return component;
    }

    public T SpawnActor<T>(int x, int y, ChessTeam team) where T : Pieces => SpawnActor<T>(new GridIndex(x, y), team);

    GameObject Empty
    {
        get
        {
            if (empty == null) {
                empty = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Empty.prefab");
            }
            return empty;
        }
    }
}