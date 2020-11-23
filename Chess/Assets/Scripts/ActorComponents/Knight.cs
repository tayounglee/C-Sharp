using UnityEditor;

using UnityEngine;

public class Knight : Pieces
{
    public override void Construct(params object[] args)
    {
        Construct((ChessTeam)args[0], (GridIndex)args[1], (ChessBoard)args[2]);
    }

    void Construct(ChessTeam team, GridIndex gridIndex, ChessBoard board)
    {
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Free Low Poly Chess Set/Knight/Mesh/knight.fbx");
        base.Construct(mesh, team, gridIndex, board);
    }

    public override MoveSequence QueryMovable(MoveType type)
    {
        var seq = new MoveSequence(this, CellIndex, 8);

        seq.AddMove(0, new GridIndex(-2, 1));
        seq.AddMove(1, new GridIndex(-1, 2));
        seq.AddMove(2, new GridIndex(1, 2));
        seq.AddMove(3, new GridIndex(2, 1));
        seq.AddMove(4, new GridIndex(2, -1));
        seq.AddMove(5, new GridIndex(1, -2));
        seq.AddMove(6, new GridIndex(-1, -2));
        seq.AddMove(7, new GridIndex(-2, -1));

        return seq;
    }

    public override ChessTeam Team
    {
        get => base.Team;
        protected set
        {
            if (value == ChessTeam.Black) {
                Material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Free Low Poly Chess Set/Knight/Materials/KnightDark.mat");
            }
            else {
                Material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Free Low Poly Chess Set/Knight/Materials/KnightLight.mat");
            }

            base.Team = value;
        }
    }
}