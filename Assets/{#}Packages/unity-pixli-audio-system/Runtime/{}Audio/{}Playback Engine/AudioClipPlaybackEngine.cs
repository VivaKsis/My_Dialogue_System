using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipPlaybackEngine : MonoBehaviour
{
	[SerializeField] private AudioClip _audioClip;
	public AudioClip _AudioClip => this._audioClip;

	[SerializeField] private AudioSourcePlayer _audioSourcePlayer;
	public AudioSourcePlayer _AudioSourcePlayer => this._audioSourcePlayer;

	private IEnumerator PlayProcess()
	{
		AudioSourceController audioSourceController = this._audioSourcePlayer.Play(
			audioClip: this._audioClip
		);

		yield return new WaitForSecondsRealtime(this._audioClip.length);

		ObjectPool.Instance.Release(audioSourceController);
	}

	public void Play() => this.StartCoroutine(this.PlayProcess());

	[SerializeField] private bool _playOnStart;
	public bool _PlayOnStart => this._playOnStart;

	[SerializeField] private bool _loop;
	public bool _Loop => this._loop;

	private IEnumerator Start()
	{
		if (this._playOnStart)
		{
			AudioSourceController audioSourceController = this._audioSourcePlayer.AquireAudioSourceController();

			//! Calling `Play` each iteration.
			do
			{
				audioSourceController.Play(
					audioClip: this._audioClip
				);

				yield return new WaitForSecondsRealtime(this._audioClip.length);
			}
			while (this._loop);

			ObjectPool.Instance.Release(audioSourceController);

			//? How to do it without calling `Play` each iteration?
			//? Why is it not recommended in this cae?
			//!? Because you are modifything a state of an entity that has been pooled.
			//!? It sure seems an easier and a better choice, but it may lead to unexpected bugs like something playing again when it shouldn't have.

			//audioSourceController.AudioSource.loop = this._loop;

			//audioSourceController.Play(
			//	audioClip: this._audioClip
			//);

			//yield return new WaitForSecondsRealtime(this._audioClip.length);
		}
	}
}
