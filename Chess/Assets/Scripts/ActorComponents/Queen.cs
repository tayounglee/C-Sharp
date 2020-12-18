using UnityEditor;

using UnityEngine;

public class Queen : Pieces
{
    public override void Construct(params object[] args)
    {
        Construct((ChessTeam)args[0], (GridIndex)args[1], (ChessBoard)args[2]);
    }

    void Construct(ChessTeam team, GridIndex gridIndex, ChessBoard board)
    {
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Free Low Poly Chess Set/Queen/Mesh/queen.fbx");
        base.Construct(mesh, team, gridIndex, board);
    }

    public override MoveSequence QueryMovable(MoveType type)
    {
        const int MoveCount = 7;

        GridIndex[] sepIndex = new GridIndex[8] {
            new GridIndex(-1,  1),
            new GridIndex( 1,  1),
            new GridIndex( 1, -1),
            new GridIndex(-1, -1),
            new GridIndex( 0,  1),
            new GridIndex( 1,  0),
            new GridIndex( 0, -1),
            new GridIndex(-1,  0)
        };

        var seq = new MoveSequence(this, CellIndex, sepIndex.Length);

        for (int i = 1; i <= MoveCount; ++i) {
            for (int j = 0; j < sepIndex.Length; ++j) {
                seq.AddMove(j, sepIndex[j] * i);
            }
        }

        return seq;
    }

    public override ChessTeam Team
    {
        get => base.Team;
        protected set
        {
            if (value == ChessTeam.Black) {
                Material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Free Low Poly Chess Set/Queen/Materials/QueenDark.mat");
            }
            else {
                Material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Free Low Poly Chess Set/Queen/Materials/QueenLight.mat");
            }

            base.Team = value;
        }
    }
}