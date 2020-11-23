﻿using System;
using System.Collections.Generic;
using System.Linq;

public partial class ActionManager
{
    ChessBoard board;
    Stack<ActionCommand> undoStack = new Stack<ActionCommand>();
    Stack<ActionCommand> redoStack = new Stack<ActionCommand>();
    BuildedData buildedData = new BuildedData();

    GridIndex? selection;
    ChessTeam actionTeam;

    StateMachine actionState;

    public ActionManager(ChessBoard board)
    {
        this.board = board;

        buildedData.Rebuild(board);
        actionTeam = ChessTeam.White;
        actionState = new UnselectedState(this);
    }

    public void ClearState()
    {
        selection = null;
    }

    public void AddClick(GridIndex? click)
    {
        actionState = actionState.Update(click);
    }

    public void Undo()
    {
        if (undoStack.Count != 0) {
            var undo = undoStack.Pop();
            var action = undo.Action;
            var special = undo.Special;

            special?.Undo();
            action.Undo();

            redoStack.Push(undo);
            GenericAction();
        }
    }

    public void Redo()
    {
        if (redoStack.Count != 0) {
            var redo = redoStack.Pop();
            var action = redo.Action;
            var special = redo.Special;

            action.Redo();
            special?.Redo();

            undoStack.Push(redo);
            GenericAction();
        }
    }

    void GenericAction()
    {
        Func<MoveSequence, int> getMovableCount = (seq) => {
            int count = 0;
            
            for (int i = 0; i < seq.SequenceCount; ++i) {
                var single = seq[i];
                count += single.Count;
            }

            return count;
        };

        SwitchActionTeam();
        buildedData.Rebuild(board);

        var currentTeams = from Pieces p in board.GetPieces()
                           where p.Team == actionTeam
                           select p;

        int movableCount = 0;

        foreach (var p in board.GetPieces()) {
            if (p?.Team == actionTeam) {
                var seq = p.QueryMovable(MoveType.StandardMove);
                seq.Build(board, MoveType.StandardMove);
                movableCount += getMovableCount(seq);

                seq = p.QueryMovable(MoveType.Attack);
                seq.Build(board, MoveType.Attack);
                movableCount += getMovableCount(seq);

                if (p is Pawn k) {
                    seq = k.QueryEnpassant(board);
                    movableCount += getMovableCount(seq);
                }
            }
        }

        if (movableCount == 0) {
            if (buildedData.IsChecked(actionTeam)) {
                GameoverEvent?.Invoke(this, GameoverType.Checkmate);
            }
            else {
                GameoverEvent?.Invoke(this, GameoverType.Stalemate);
            }
        }
    }

    void SwitchActionTeam()
    {
        if (actionTeam == ChessTeam.White) {
            actionTeam = ChessTeam.Black;
        }
        else {
            actionTeam = ChessTeam.White;
        }
    }

    void AddAttack(Pieces from, Pieces to)
    {
        if (from.Team == to.Team) {
            if (from == to) {
                
                selection = null;
            }
            else {
                
                selection = to.CellIndex;
            }
        }
        else {
            
            var seq = from.QueryMovable(MoveType.Attack);
            seq.Build(board, MoveType.Attack);

            if (seq.ContainsMove(to.CellIndex)) {
                var fromIndex = from.CellIndex;
                var action = from.MoveTo(to.CellIndex);

                undoStack.Push(ActionCommand.SimpleMove(action));
                redoStack.Clear();
                GenericAction();
                board.Turnover();
            }
        }
    }

    void AddMove(Pieces from, GridIndex to)
    {
        var seq = from.QueryMovable(MoveType.StandardMove);
        seq.Build(board, MoveType.StandardMove);

        if (seq.ContainsMove(to)) {
            var fromIndex = from.CellIndex;
            var action = from.MoveTo(to);

            undoStack.Push(ActionCommand.SimpleMove(action));
            redoStack.Clear();
            GenericAction();
            board.Turnover();
        }

        
        else if (from is King king) {
            AddCastling(king, to);
        }

        
        else if (from is Pawn pawn) {
            AddEnpassant(pawn, to);
        }
    }

    void AddCastling(King from, GridIndex to)
    {
        var castlingSeq = from.QueryCastling(board, out var bQueenSide, out var bKingSide);
        castlingSeq.Build(board, MoveType.StandardMove);

        if (castlingSeq.ContainsMove(to)) {
            var kingX = from.CellIndex.X;
            var kingY = from.CellIndex.Y;

            Rook rook = null;
            GridIndex rookFrom;
            GridIndex rookTo;
            if (to.X > kingX && bKingSide) {
                rookFrom = new GridIndex(7, kingY);
                rook = board[rookFrom] as Rook;
                rookTo = to + new GridIndex(-1, 0);
            }
            else if (to.X < kingX && bQueenSide) {
                rookFrom = new GridIndex(0, kingY);
                rook = board[rookFrom] as Rook;
                rookTo = to + new GridIndex(+1, 0);
            }
            else {
                
                throw new Exception("Unspecified exception");
            }

            if (rook != null) {
                var fromIndex = from.CellIndex;

                var kingAction = from.MoveTo(to);
                var rookAction = rook.MoveTo(rookTo);

                undoStack.Push(ActionCommand.SimpleMove(kingAction, rookAction));
                redoStack.Clear();
                GenericAction();
                board.Turnover();
            }
        }
    }

    void AddEnpassant(Pawn from, GridIndex to)
    {
        var enpassantSeq = from.QueryEnpassant(board);

        if (enpassantSeq.ContainsMove(to)) {
            var targetIdx = new GridIndex(to.X, from.CellIndex.Y);
            var target = board[targetIdx];

            var actionData = target.MoveTo(to);
            var specialData = from.MoveTo(to);

            undoStack.Push(ActionCommand.SimpleMove(actionData, specialData));
            redoStack.Clear();
            GenericAction();
            board.Turnover();
        }
    }

    public bool HasSelection => selection.HasValue;
    public Pieces SelectionPiece => board[selection.Value];
    public BuildedData BuildedData => buildedData;

    public event EventHandler<GameoverType> GameoverEvent;
}