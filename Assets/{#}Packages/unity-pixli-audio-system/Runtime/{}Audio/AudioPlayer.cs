using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviourSingleton<AudioPlayer>
{
	public enum Type
	{
		Ambience,
		Music,
		SoundEffect
	}

	[SerializeField] private AudioSourcePlayer[] _audioSourcePlayers = new AudioSourcePlayer[System.Enum.GetNames(typeof(Type)).Length];
	public AudioSourcePlayer[] _AudioSourcePlayers => this._audioSourcePlayers;

	private IEnumerator InvokeAfter(float seconds, Action action)
	{
		yield return new WaitForSecondsRealtime(time: seconds);

		action.Invoke();
	}

	public AudioSourceController Play(AudioClip audioClip, Type type)
	{
		AudioSourceController audioSourceController = this._audioSourcePlayers[(int)type].Play(
			audioClip: audioClip
		);

		this.StartCoroutine(
			routine: this.InvokeAfter(
				seconds: audioClip.length,
				action: () =>
				{
					ObjectPool.Instance.Release(audioSourceController);
				}
			)
		);

		return audioSourceController;
	}

	public AudioSourceController Play(AudioClip audioClip, float volume, Type type)
	{
		AudioSourceController audioSourceController = this._audioSourcePlayers[(int)type].Play(
			audioClip: audioClip,
			volume: volume
		);

		this.StartCoroutine(
			routine: this.InvokeAfter(
				seconds: audioClip.length,
				action: () =>
				{
					ObjectPool.Instance.Release(audioSourceController);
				}
			)
		);

		return audioSourceController;
	}

	public AudioSourceController PlayDelayed(AudioClip audioClip, float delay, Type type)
	{
		AudioSourceController audioSourceController = this._audioSourcePlayers[(int)type].PlayDelayed(
			audioClip: audioClip,
			delay: delay
		);

		this.StartCoroutine(
			routine: this.InvokeAfter(
				seconds: audioClip.length,
				action: () =>
				{
					ObjectPool.Instance.Release(audioSourceController);
				}
			)
		);

		return audioSourceController;
	}
}
