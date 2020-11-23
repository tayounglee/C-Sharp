using System;
using System.Collections.Generic;

public class MoveSequence
{
    Pieces owner;
    GridIndex ownerIndex;
    List<GridIndex>[] sequences;
    Pieces temp;

    public MoveSequence(Pieces owner, GridIndex ownerIndex, int numSequences)
    {
        this.owner = owner;
        this.ownerIndex = ownerIndex;

        sequences = new List<GridIndex>[numSequences];
        for (int i = 0; i < numSequences; ++i) {
            sequences[i] = new List<GridIndex>();
        }
    }

    public void AddMove(int sequenceIndex, GridIndex move)
    {
        GridIndex final = ownerIndex + move;
        if (final.IsValid) {
            sequences[sequenceIndex].Add(final);
        }
    }

    public int MoveCount(int sequenceIndex)
    {
        return sequences[sequenceIndex].Count;
    }

    public void Build(ChessBoard board, MoveType moveType, bool bBuildCriticalMove = true)
    {
        if (moveType == MoveType.StandardMove) {
            BuildStandardMove(board);
        }
        else {
            BuildAttack(board);
        }

        if (bBuildCriticalMove) {
            BuildCriticalMove(board);
        }
    }

    public bool ContainsMove(GridIndex index)
    {
        for (int i = 0; i < SequenceCount; ++i) {
            var single = this[i];
            for (int j = 0; j < single.Count; ++j) {
                if (index.Equals(single[j])) return true;
            }
        }

        return false;
    }

    void BuildStandardMove(ChessBoard board)
    {
        for (int i = 0; i < SequenceCount; ++i) {
            var single = this[i];
            for (int j = 0; j < single.Count; ++j) {
                
                if (board[single[j]] != null) {
                    
                    single.RemoveRange(j, single.Count - j);
                    break;
                }
            }
        }
    }

    void BuildAttack(ChessBoard board)
    {
        var result = new List<GridIndex>();
        for (int i = 0; i < SequenceCount; ++i) {
            var single = this[i];
            for (int j = 0; j < single.Count; ++j) {
                var target = board[single[j]];

                
                if (target != null) {
                    
                    if (target.Team != owner.Team) {
                        
                        result.Add(single[j]);
                    }

                    
                    break;
                }
            }
        }

        sequences = new List<GridIndex>[1];
        sequences[0] = result;
    }

    void BuildCriticalMove(ChessBoard board)
    {
        var tempBuild = new BuildedData();

        
        for (int i = 0; i < SequenceCount; ++i) {
            var single = this[i];
            for (int j = 0; j < single.Count; ++j) {
                var action = owner.MoveTo(single[j]);

                
                tempBuild.Rebuild(board);

                
                if (tempBuild.IsChecked(owner.Team)) {
                    
                    single.RemoveAt(j);
                    j -= 1;
                }

                action.Undo();
            }
        }
    }

    public List<GridIndex> this[int index] => sequences[index];
    public int SequenceCount => sequences.Length;
    public Pieces Owner => owner;
}