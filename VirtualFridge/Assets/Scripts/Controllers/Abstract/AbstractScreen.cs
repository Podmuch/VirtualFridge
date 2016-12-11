using UnityEngine;
using System.Collections;

public abstract class AbstractScreen : MonoBehaviour
{
    #region SCENE REFERENCES

    public GameObject ScreenGameObject;
    public CanvasGroup ScreenCanvasGroup;

    #endregion

    protected virtual float AnimationTime { get { return 0.2f; } }
    protected bool isClickable = true;
    private Coroutine currentCoroutine = null;

    public virtual void Show()
    {
        isClickable = true;
        ScreenGameObject.SetActive(true);
        BeginAnimationCoroutine(true);
    }

    public virtual void Hide()
    {
        if (ScreenGameObject.activeInHierarchy)
        {
            isClickable = false;
            BeginAnimationCoroutine(false);
        }
    }

    public virtual void SetStateImmediately(bool isShow)
    {
        isClickable = isShow;
        ScreenGameObject.SetActive(isShow);
        ScreenCanvasGroup.alpha = isShow ? 1 : 0;
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
    }

    public virtual void UpdateData() { }

    private void BeginAnimationCoroutine(bool isShow)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(AnimationCoroutine(isShow));
    }

    protected virtual IEnumerator AnimationCoroutine(bool isShow)
    {
        float initialTime = isShow ? ScreenCanvasGroup.alpha * AnimationTime : (1 - ScreenCanvasGroup.alpha) * AnimationTime;
        for (float time = initialTime; time < AnimationTime; time += Time.deltaTime)
        {
            ScreenCanvasGroup.alpha = isShow ? time / AnimationTime : (1 - (time / AnimationTime));
            yield return 0;
        }
        ScreenCanvasGroup.alpha = isShow ? 1 : 0;
        ScreenGameObject.SetActive(isShow);
    }
}