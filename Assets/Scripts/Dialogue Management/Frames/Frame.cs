using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Frame : MonoBehaviour, IPoolable
{
    #region Pool
    public GameObject GameObject => this.gameObject;
    public ObjectPool.Pool Pool { get; set; }

    public void OnRelease()
    {
    }

    public void OnAquire()
    {
    }
    #endregion

    #region DOTweenAnimation
    [SerializeField] DOTweenAnimation _dOTweenAnimation;
    public DOTweenAnimation DOTweenAnimationHide => _dOTweenAnimation;

    public void HideAnimation()
    {
        if (_dOTweenAnimation != null)
        {
            _dOTweenAnimation.DOPlayBackwards();
        }
    }

    public void ShowAnimation()
    {
        if (_dOTweenAnimation != null)
        {
            _dOTweenAnimation.DOPlayForward();
        }
    }
    #endregion

    [SerializeField] private Vector2 _position;
    public Vector2 Position => _position;

    [SerializeField] private CanvasGroup _canvasGroup;
    public CanvasGroup _CanvasGroup => _canvasGroup;
    [SerializeField] private int _id = -1;
    public int _Id => _id;

    public void ClearFrame()
    {
        for (int a = 0; a < transform.childCount; a++)
        {
            Transform child = transform.GetChild(a);
            try
            {
                ObjectPool.Instance.Release(child.gameObject);
            }
            catch
            {
                // this child is not IPoolable, that's alright
            }
        }
        transform.SetParent(null);
        ObjectPool.Instance.Release(this.gameObject);
    }
    private IEnumerator HideAndClearCoroutine()
    {
        HideAnimation();

        yield return new WaitForSecondsRealtime(1f);

        ClearFrame();
    }

    public void HideAndDestroy()
    {
        StartCoroutine("HideAndClearCoroutine");
    }

    public void Show()
    {
        _canvasGroup.Show();
    }

    public void Hide()
    {
        _canvasGroup.Hide();
    }
}
