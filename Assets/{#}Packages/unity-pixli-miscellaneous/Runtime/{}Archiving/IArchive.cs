using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public interface IArchive<TItem>
{
	void Initialize();
	void Initialize(Random random);

	TItem _SelectedAsset { get; }

	TItem Previous(bool loop = true);
	TItem Next(bool loop = true);
	TItem Random();

	void Reset();
}
