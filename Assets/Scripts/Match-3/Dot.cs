using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DotColor
{
    magenta,
    orange,
    pink,
    purple,
    red,
    salad,
    violet,
    yellow
}

public class Dot : MonoBehaviour, IPoolable
{
    [Header("General Dot Variables")]
    [SerializeField] private DotColor _dotColor;
    public DotColor _DotColor => _dotColor;

    public bool isMatched = false;
    public int column, row;

    [Header("Power-up Variables")]
    [SerializeField] private bool _isHorizontalBomb;
    public bool _IsHorizontalBomb => _isHorizontalBomb;
    [SerializeField] private bool _isVerticalBomb;
    public bool _IsVerticalBomb => _isVerticalBomb;
    [SerializeField] private GameObject _horizontalBomb;
    public GameObject _HorizontalBomb => _horizontalBomb;
    [SerializeField] private GameObject _verticalBomb;
    public GameObject _VerticalBomb => _verticalBomb;

    public Dot otherDot;

    private int previousColumn, previousRow;

    private float swipeAngle = 0, 
                  swipeConfirm = 1f; // to avoid bug, when we just click on dot, but it counts as swipe

    private Board board;
    private Vector2 firstTouchPosition, finalTouchPosition;
    private Vector2 currentPosition, tempPosition;
    private Camera _mainCamera;
    private SpriteRenderer spriteRenderer;
    private Color thisDotColor;

    private const float PI = Mathf.PI;
    private const float dotSpeed = 1f;

    #region Pool
    public GameObject GameObject => this.gameObject;
    public ObjectPool.Pool Pool { get; set; }

    public void OnRelease()
    {
        isMatched = false;
        if(spriteRenderer != null && thisDotColor != null)
        {
            spriteRenderer.color = thisDotColor;
        }
        _isHorizontalBomb = _isVerticalBomb = false;
        for (int a = 0; a < transform.childCount; a++)
        {
            GameObject _gameObject = transform.GetChild(a).gameObject;
            ObjectPool.Instance.Release(_gameObject);
        }
    }

    public void OnAquire()
    {
    }
    #endregion

    private void Start()
    {
        DOTween.Init(true, true, LogBehaviour.Default).SetCapacity(20000,0);
        
        _mainCamera = Camera.main;
        board = GetComponentInParent<Board>();
        currentPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        thisDotColor = spriteRenderer.color;
    }

    private void DOTweenMovement(Vector3 targetPosition)
    {
        transform.DOMove(targetPosition, dotSpeed);

        if (board.allDots[column, row] != gameObject)
        {
            board.allDots[column, row] = this;
        }
        MatchFinder.Instance.FindAllMatches();
    }

    private void Update()
    {
        if (isMatched)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, .2f);
        }

        currentPosition = transform.position;

        if (Mathf.Abs(column - currentPosition.x) > .1)
        {
            DOTweenMovement(new Vector3(column, currentPosition.y, 0));
        }
        else
        {
            tempPosition = new Vector2(column, currentPosition.y);
            currentPosition = tempPosition;
        }
        if (Mathf.Abs(row - currentPosition.y) > .1)
        {
            DOTweenMovement(new Vector3(currentPosition.x, row, 0));
        }
        else
        {
            tempPosition = new Vector2(currentPosition.x, row);
            currentPosition = tempPosition;
        }

    }

    public IEnumerator CheckMoveCoroutine() // if dot doesn't match in the new position, move back
    {
        yield return new WaitForSeconds(.5f);
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.isMatched)
            {
                otherDot.row = row;
                otherDot.column = column;
                row = previousRow;
                column = previousColumn;

                yield return new WaitForSeconds(.3f);
                board.currentState = GameState.move;
                board.currentDot = null;
            }
            else
            {
                board.DestroyMatches();
            }
        }
    }

    private void SetNewCoordinates()
    {
        previousColumn = column;
        previousRow = row;

        if (swipeAngle > -45 && swipeAngle <= 45 && column < board._Width - 1)
        {
            //right swipe
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board._Height - 1)
        {
            //up swipe
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //left swipe
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //down swipe
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCoroutine());
    }

    private void CalculateAngle()
    {
        if ((Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeConfirm) || 
            (Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeConfirm)) // check if player did a proper swipe, not just clicked dot
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / PI;
            SetNewCoordinates();
            board.currentState = GameState.wait;
            board.currentDot = this;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    private Vector3 IncreaseZAxis(Vector3 position) // for proper ScreenToWorldPoint() work, otherwise it will return camera coordinates but not the dot
    {
        return new Vector3 (position.x, position.y, position.z += 10f);
    }

    private void OnMouseDown()
    {
        if(board.currentState == GameState.move)
        {
            firstTouchPosition = _mainCamera.ScreenToWorldPoint(IncreaseZAxis(Input.mousePosition));
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = _mainCamera.ScreenToWorldPoint(IncreaseZAxis(Input.mousePosition));
            CalculateAngle();
        }
    }

    private void SetBombSettings(GameObject arrow)
    {
        arrow.transform.position = transform.position;
        arrow.transform.parent = transform;
        spriteRenderer.color = thisDotColor;
    }

    public void MakeHorizontalBomb()
    {
        _isHorizontalBomb = true;
        SetBombSettings(ObjectPool.Instance.Aquire(_horizontalBomb));
    }

    public void MakeVerticalBomb()
    {
        _isVerticalBomb = true;
        SetBombSettings(ObjectPool.Instance.Aquire(_VerticalBomb));
    }
}

