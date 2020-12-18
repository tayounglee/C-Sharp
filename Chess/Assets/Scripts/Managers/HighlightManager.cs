using System;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class HighlightManager
{
    ChessBoard board;

    GridIndex? hoverIndex;
    GameObject hoverGlow;

    List<GameObject> movableGlows = new List<GameObject>();
    List<GameObject> attackGlows = new List<GameObject>();
    List<GameObject> specialGlows = new List<GameObject>();

    public HighlightManager(ChessBoard board)
    {
        this.board = board;

        hoverGlow = CreateGlow(GlowType.Hover);
    }

    public void ClearState()
    {
        HoverIndex = null;
        ClearMoveHighlight();
        ClearAttackHighlight();
        ClearSpecialHighlight();
    }

    public void BuildMoveHighlight(Pieces piece)
    {
        ClearMoveHighlight();

        var moveSeq = piece.QueryMovable(MoveType.StandardMove);
        moveSeq.Build(board, MoveType.StandardMove);

        for (int i = 0; i < moveSeq.SequenceCount; ++i) {
            var single = moveSeq[i];
            for (int j = 0; j < single.Count; ++j) {
                movableGlows.Add(CreateGlow(GlowType.Movable, single[j]));
            }
        }

        movableGlows.Add(CreateGlow(GlowType.Movable, piece.CellIndex));
    }

    public void BuildAttackHighlight(Pieces piece)
    {
        ClearAttackHighlight();

        var attackSeq = piece.QueryMovable(MoveType.Attack);
        attackSeq.Build(board, MoveType.Attack);

        for (int i = 0; i < attackSeq.SequenceCount; ++i) {
            var single = attackSeq[i];
            for (int j = 0; j < single.Count; ++j) {
                movableGlows.Add(CreateGlow(GlowType.UnderAttack, single[j]));
            }
        }
    }

    public void BuildSpecialHighligh(Pieces piece)
    {
        ClearSpecialHighlight();

        if (piece is King king) {
            var seq = king.QueryCastling(board);
            seq.Build(board, MoveType.StandardMove);

            for (int i = 0; i < seq.SequenceCount; ++i) {
                var single = seq[i];
                for (int j = 0; j < single.Count; ++j) {
                    specialGlows.Add(CreateGlow(GlowType.Special, single[j]));
                }
            }
        }
        else if (piece is Pawn pawn) {
            var seq = pawn.QueryEnpassant(board);
            for (int i = 0; i < seq.SequenceCount; ++i) {
                var single = seq[i];
                for (int j = 0; j < single.Count; ++j) {
                    specialGlows.Add(CreateGlow(GlowType.Special, single[j]));
                }
            }
        }
    }

    public void ClearMoveHighlight() => ClearHighlights(movableGlows);
    public void ClearAttackHighlight() => ClearHighlights(attackGlows);
    public void ClearSpecialHighlight() => ClearHighlights(specialGlows);

    public GridIndex? HoverIndex
    {
        get => hoverIndex;
        set
        {
            hoverIndex = value;
            if (hoverIndex.HasValue) {
                if (!hoverGlow.activeSelf) {
                    hoverGlow.SetActive(true);
                }
                hoverGlow.transform.localPosition = board.QueryLocation(hoverIndex.Value);
            }
            else {
                if (hoverGlow.activeSelf) {
                    hoverGlow.SetActive(false);
                }
            }
        }
    }

    void ClearHighlights(List<GameObject> highlightGlows)
    {
        foreach (var glow in highlightGlows) {
            GameObject.Destroy(glow);
        }
        highlightGlows.Clear();
    }

    GameObject CreateGlow(GlowType type, GridIndex? initialIndex = null)
    {
        Material myMaterial;

        switch (type) {
            case GlowType.Hover:
                myMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/FX/HoverGlow.mat");
                break;
            case GlowType.UnderAttack:
                myMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/FX/AttackGlow.mat");
                break;
            case GlowType.Movable:
                myMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/FX/MoveGlow.mat");
                break;
            case GlowType.Special:
                myMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/FX/SpecialGlow.mat");
                break;
            default:
                throw new ArgumentException();
        }

        var glow = GameObject.CreatePrimitive(PrimitiveType.Cube);
        glow.transform.parent = board.transform;

        glow.transform.localScale = new Vector3(1.0f, 0.01f, 1.0f);
        glow.GetComponent<MeshRenderer>().material = myMaterial;

        if (initialIndex.HasValue) {
            var location = board.QueryLocation(initialIndex.Value);
            glow.transform.localPosition = location;
        }
        else {
            glow.SetActive(false);
        }

        return glow;
    }
}