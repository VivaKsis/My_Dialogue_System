using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    [SerializeField] private int _height = 10;
    public int _Height => _height;

    [SerializeField] private int _width = 10;
    public int _Width => _width;

    [SerializeField] private int _offset = 20;
    public int _Offset => _offset;

    [SerializeField] private GameObject[] _availableDots;
    public GameObject[] _AvailableDots => _availableDots;

    [SerializeField] private GameObject _tilePrefab;
    public GameObject _TilePrefab => _tilePrefab;

    public Dot[,] allDots { get;  private set; }
    public Dot currentDot;

    public GameState currentState = GameState.move;

    private Dot dot;
    private GameObject tileBackground, dotObject;
    private int dotToUse,
                nullCount = 0,
                maxIteration = 0;  // avoid infinite loop


    private bool MatchesAt(int column, int row, GameObject dotPrefab)
    {
        dot = dotPrefab.GetComponent<Dot>();

        if (column > 1)
        {
            if (allDots[column - 1, row]._DotColor == dot._DotColor && allDots[column - 2, row]._DotColor == dot._DotColor)
            {
                return true;
            }
        }

        if (row > 1)
        {
            if (allDots[column, row - 1]._DotColor == dot._DotColor && allDots[column, row - 2]._DotColor == dot._DotColor)
            {
                return true;
            }
        }
        return false;
    }

    private void InstantiateDot(int column, int row)
    {
        do
        {
            dotToUse = Random.Range(0, _availableDots.Length);
            maxIteration++;
        }
        while (MatchesAt(column, row, _availableDots[dotToUse]) && maxIteration < 100);
        maxIteration = 0;

        dotObject = ObjectPool.Instance.Aquire(gameObject: _availableDots[dotToUse]);

        dotObject.transform.SetParent(parent: this.transform,
                                       worldPositionStays: false);
        dotObject.transform.localPosition = new Vector2(column, row + _offset);

        dotObject.name = "( " + column + ", " + row + " )";

        dot = dotObject.GetComponent<Dot>();
        dot.row = row;
        dot.column = column;
        allDots[column, row] = dot;
    }

    private bool MatchesOnBoard()
    {
        for (int a = 0; a < _width; a++)
        {
            for (int b = 0; b < _height; b++)
            {
                if(allDots[a,b] != null && allDots[a, b].isMatched)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void RefillBoard()
    {
        for (int a = 0; a < _width; a++)
        {
            for (int b = 0; b < _height; b++)
            {
                if (allDots[a, b] == null)
                {
                    InstantiateDot(a, b);
                }
            }
        }
    }

    private IEnumerator FillBoardCoroutine()
    {
        RefillBoard();
        yield return new WaitForSecondsRealtime(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSecondsRealtime(.5f);
            DestroyMatches();
        }
        MatchFinder.Instance.currentMatches.Clear();

        yield return new WaitForSecondsRealtime(.5f);
        currentState = GameState.move;
    }

    private IEnumerator DecreaseRowCoroutine()
    {
        for (int a = 0; a < _width; a++)
        {
            for (int b = 0; b < _height; b++)
            {
                if(allDots[a,b] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[a, b].row -= nullCount;
                    allDots[a, b] = null;
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSecondsRealtime(.4f);

        StartCoroutine(FillBoardCoroutine());
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].isMatched)
        {
            if (MatchFinder.Instance.currentMatches.Count == 4)
            {
                MatchFinder.Instance.CheckForBombs();
            }
            MatchFinder.Instance.currentMatches.Remove(allDots[column, row]);
            ObjectPool.Instance.Release(gameObject: allDots[column, row].gameObject);
            allDots[column, row] = null;
            currentDot = null;
        }
    }

    public void DestroyMatches()
    {
        for (int a = 0; a < _width; a++)
        {
            for (int b = 0; b < _height; b++)
            {
                if (allDots[a, b] != null)
                {
                    DestroyMatchesAt(a, b);
                }
            }
        }
        StartCoroutine(DecreaseRowCoroutine());
    }

    private void SetUp()
    {
        for (int a = 0; a < _width; a++)
        {
            for (int b = 0; b < _height; b++)
            {
                tileBackground = Instantiate(original: _tilePrefab,
                                             position: new Vector2(a, b + _offset),
                                             rotation: Quaternion.identity,
                                             parent: transform); ;
                tileBackground.name = "( " + a + ", " + b + " )";

                InstantiateDot(a, b);
            }
        }
    }

    void Start()
    {
        allDots = new Dot[_width, _height];
        SetUp();
    }

    /*public List<Dot> GetFullColumn(int column)
    {
        List<Dot> dots = new List<Dot>();
        for (int a = 0; a < _Height; a++)
        {
            if (allDots[column, a] != null)
            {
                dots.Add(allDots[column, a]);
                allDots[column, a].isMatched = true;
            }
        }
        return dots;
    }

    public List<Dot> GetFullRow(int row)
    {
        List<Dot> dots = new List<Dot>();
        for (int a = 0; a < _Height; a++)
        {
            if (allDots[a, row] != null)
            {
                dots.Add(allDots[a, row]);
                allDots[a, row].isMatched = true;
            }
        }
        return dots;
    }*/
}
