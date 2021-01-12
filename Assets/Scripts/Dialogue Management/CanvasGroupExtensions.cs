using UnityEngine;
public static class CanvasGroupExtensions {

	public static bool IsHidden (this CanvasGroup canvasGroup)
    {
		if (canvasGroup.alpha == 1f && canvasGroup.interactable == true && canvasGroup.blocksRaycasts == true)
        {
			return false;
		}
		return true;
    }

	public static void Show(this CanvasGroup canvasGroup) {
		canvasGroup.alpha = 1f;
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
	}

	public static void Hide(this CanvasGroup canvasGroup) {
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}
}