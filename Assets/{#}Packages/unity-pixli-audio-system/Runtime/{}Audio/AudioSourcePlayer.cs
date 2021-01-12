using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePlayer : MonoBehaviour
{
	[SerializeField] private AudioSourceController _audioSourceControllerPrefab;
	public AudioSourceController _AudioSourceControllerPrefab => this._audioSourceControllerPrefab;

	//public void PlayUI(AudioClip audioClip)
	//{
	//	AudioSourceController audioSourceController = ObjectPool.Instance.Aquire<AudioSourceController>(
	//		poolable: this._audioSourceControllerPrefab
	//	);

	//	audioSourceController.Play(audioClip: audioClip);
	//}

	public AudioSourceController Play(AudioClip audioClip)
	{
		AudioSourceController audioSourceController = ObjectPool.Instance.Aquire<AudioSourceController>(
			poolable: this._audioSourceControllerPrefab
		);

		audioSourceController.Play(audioClip: audioClip);

		return audioSourceController;
	}

	public AudioSourceController Play(AudioClip audioClip, float volume)
	{
		AudioSourceController audioSourceController = ObjectPool.Instance.Aquire<AudioSourceController>(
			poolable: this._audioSourceControllerPrefab
		);

		audioSourceController.Play(
			audioClip: audioClip,
			volume: volume
		);

		return audioSourceController;
	}

	public AudioSourceController PlayDelayed(AudioClip audioClip, float delay)
	{
		AudioSourceController audioSourceController = ObjectPool.Instance.Aquire<AudioSourceController>(
			poolable: this._audioSourceControllerPrefab
		);

		audioSourceController.PlayDelayed(
			audioClip: audioClip,
			delay: delay
		);

		return audioSourceController;
	}

	public AudioSourceController AquireAudioSourceController()
	{
		return ObjectPool.Instance.Aquire<AudioSourceController>(
			poolable: this._audioSourceControllerPrefab
		);
	}
}
