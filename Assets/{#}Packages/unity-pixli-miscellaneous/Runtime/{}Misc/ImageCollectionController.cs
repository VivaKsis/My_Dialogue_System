using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCollectionController : MonoBehaviour
{
	[SerializeField] private Image _image;
	public Image _Image => this._image;

	[SerializeField] private Sprite[] _sprites;
	public Sprite[] _Sprites => this._sprites;

	public void SetSprite(int index) => this._image.sprite = this._sprites[index];

#if UNITY_EDITOR
	private void Reset()
	{
		this._image = this.GetComponent<Image>();
	}
#endif
}
