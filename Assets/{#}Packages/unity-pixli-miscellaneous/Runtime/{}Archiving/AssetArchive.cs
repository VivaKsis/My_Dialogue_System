using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[System.Serializable]
public abstract class AssetArchive<TAsset> : ScriptableObject, IArchive<TAsset>
{
	private int _selectedIndex = 0;
	public TAsset _SelectedAsset => this.assets[this._selectedIndex];

	[SerializeField] protected TAsset[] assets;
	public TAsset[] Assets
	{
		get { return this.assets; }
		set
		{
			this.assets = value;
			this.Initialize();
		}
	}

	public TAsset Previous(bool loop = true)
	{
		this._selectedIndex = loop ? ((--this._selectedIndex + this.assets.Length) % this.assets.Length) : Mathf.Clamp(--this._selectedIndex, 0, this.assets.Length - 1);

		return this._SelectedAsset;
	}

	public TAsset Next(bool loop = true)
	{
		this._selectedIndex = loop ? (++this._selectedIndex % this.assets.Length) : Mathf.Clamp(++this._selectedIndex, 0, this.assets.Length - 1);

		return this._SelectedAsset;
	}

	protected Random random;

	public TAsset Random() => this.assets[this.random.Next(maxValue: this.assets.Length)];
	
	public virtual void Initialize(Random random)
	{
		this.random = random;
	}

	public void Initialize() => this.Initialize(new Random());

	public virtual void Reset()
	{
		this._selectedIndex = 0;
	}

	protected virtual void OnEnable()
	{
		this.Reset();

		this.Initialize();
	}
}