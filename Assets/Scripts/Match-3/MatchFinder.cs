using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviourSingleton<MatchFinder>
{
    [SerializeField] private Board _board;
    public Board _Board => _board;

    public List<Dot> currentMatches = new List<Dot>(); // for tracking 4+ combinations and so generating power-ups

    private Dot currentDot, leftDot, rightDot, upDot, downDot;

    private void MakeBomb(Dot dot)
    {
        dot.isMatched = false;
        int typeOfBomb = Random.Range(0, 100);
        if (typeOfBomb < 50)
        {
            dot.MakeHorizontalBomb();
        }
        else if (typeOfBomb >= 50)
        {
            dot.MakeVerticalBomb();
        }
    }

    public void CheckForBombs()
    {
        if (_board.currentDot != null)
        {
            if (_board.currentDot.isMatched)
            {
                MakeBomb(_board.currentDot);
            }
            else if (_board.currentDot.otherDot.isMatched)
            {
                MakeBomb(_board.currentDot.otherDot);
            }
        }
    }

    private List<Dot> MatchFullColumn(int column)
    {
        List<Dot> dots = new List<Dot>();
        for (int a = 0; a < _board._Height; a++)
        {
            if (_board.allDots[column, a] != null)
            {
                dots.Add(_board.allDots[column, a]);
                _board.allDots[column, a].isMatched = true;
            }
        }
        return dots;
    }

    private List<Dot> MatchFullRow(int row)
    {
        List<Dot> dots = new List<Dot>();
        for (int a = 0; a < _board._Height; a++)
        {
            if (_board.allDots[a, row] != null)
            {
                dots.Add(_board.allDots[a, row]);
                _board.allDots[a, row].isMatched = true;
            }
        }
        return dots;
    }

    private void CheckForHorizontalBomb(Dot dot1, Dot dot2)
    {
        if (currentDot._IsHorizontalBomb)
        {
            currentMatches.Union(MatchFullRow(currentDot.row));
        }
        if (dot1._IsHorizontalBomb)
        {
            currentMatches.Union(MatchFullRow(dot1.row));
        }
        if (dot2._IsHorizontalBomb)
        {
            currentMatches.Union(MatchFullRow(dot2.row));
        }
    }

    private void CheckForVerticalBomb(Dot dot1, Dot dot2)
    {
        if (currentDot._IsVerticalBomb)
        {
            currentMatches.Union(MatchFullColumn(currentDot.column));
        }
        if (dot1._IsVerticalBomb)
        {
            currentMatches.Union(MatchFullColumn(dot1.column));
        }
        if (dot2._IsVerticalBomb)
        {
            currentMatches.Union(MatchFullColumn(dot2.column));
        }
    }

    private void ThrowInCurrentMatches(Dot dot1, Dot dot2)
    {
        if (!currentMatches.Contains(dot1))
        {
            currentMatches.Add(dot1);
        }
        if (!currentMatches.Contains(dot2))
        {
            currentMatches.Add(dot2);
        }
        if (!currentMatches.Contains(currentDot))
        {
            currentMatches.Add(currentDot);
        }
    }

    private void CheckIfMatched(Dot dot1, Dot dot2)
    {
        if (dot1._DotColor == currentDot._DotColor && dot2._DotColor == currentDot._DotColor)
        {
            dot1.isMatched = true;
            dot2.isMatched = true;
            currentDot.isMatched = true;

            //EventSystemMatch3.Instance.MatchFind();

            ThrowInCurrentMatches(dot1, dot2);
            CheckForHorizontalBomb(dot1, dot2);
            CheckForVerticalBomb(dot1, dot2);
        }
    }

    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitForSecondsRealtime(.2f);

        for (int a = 0; a < _board._Width; a++)
        {
            for (int b = 0; b < _board._Height; b++)
            {
                currentDot = _board.allDots[a, b];
                if (currentDot != null)
                {
                    if (a > 0 && a < _board._Width - 1)
                    {
                        leftDot = _board.allDots[a - 1, b];
                        rightDot = _board.allDots[a + 1, b];
                        if (leftDot != null && rightDot != null)
                        {
                            CheckIfMatched(leftDot, rightDot);
                        }
                    }
                    if (b > 0 && b < _board._Height - 1)
                    {
                        downDot = _board.allDots[a, b - 1];
                        upDot = _board.allDots[a, b + 1];
                        if (downDot != null && upDot != null)
                        {
                            CheckIfMatched(downDot, upDot);
                        }
                    }
                }
            }
        }
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }
}


