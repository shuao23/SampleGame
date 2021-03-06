﻿using System;
using System.Collections.Generic;

public class MoveManager
{
    private IMove lastUpdated;
    private List<IMove> moves = new List<IMove>();


    public IMove BestCandidate {
        get {
            for (int i = moves.Count - 1; i >= 0; i--)
            {
                if (moves[i].InRightCondition && moves[i].Issued)
                {
                    return moves[i];
                }
            }
            throw new NoMoveCandidatesException();
        }
    }


    public void Clear()
    {
        moves.Clear();
    }

    public void Register(IMove move)
    {
        if (move == null)
        {
            throw new ArgumentNullException("move");
        }

        moves.Add(move);
    }

    /// <summary>
    /// Update only the highest prority move
    /// </summary>
    /// <param name="deltaTime">Change in time</param>
    public IMove Update(float deltaTime)
    {
        IMove updatedMove = null;
        for (int i = moves.Count - 1; i >= 0; i--)
        {
            IMove move = moves[i];
            if (updatedMove == null)
            {
                if (move.Update(deltaTime))
                {
                    updatedMove = move;
                }
            }
            else
            {
                move.Close();
            }
        }

        if(updatedMove != null)
        {
            return updatedMove;
        }

        throw new NoMoveCandidatesException();
    }
}
