using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceController : MonoBehaviour, IPoolable
{
	[SerializeField] private AudioSourceControlGroup _audioSourceControlGroup;
	public AudioSourceControlGroup _AudioSourceControlGroup => this._audioSourceControlGroup;

	public AudioSource AudioSource { get; private set; }

	private float _volume;
	public float Volume
	{
		get => this.AudioSource.volume;
		set
		{
			this._volume = value;

			this.AudioSource.volume = this._volume * this._audioSourceControlGroup.Volume;
		}
	}

	//TODO: fix this ugly code.
	private void UpdateVolume(float volume) => this.Volume = this._volume;

	public void Play(AudioClip audioClip)
	{
		this.AudioSource.volume = this._volume * this._audioSourceControlGroup.Volume;
		this.AudioSource.clip = audioClip;

		this.AudioSource.Play();
	}

	public void Play(AudioClip audioClip, float volume)
	{
		this._volume = volume;
		this.AudioSource.volume = this._volume * this._audioSourceControlGroup.Volume;
		this.AudioSource.clip = audioClip;

		this.AudioSource.Play();
	}

	public void PlayDelayed(AudioClip audioClip, float delay)
	{
		this.AudioSource.volume = this._volume * this._audioSourceControlGroup.Volume;
		this.AudioSource.clip = audioClip;

		this.AudioSource.PlayDelayed(delay: delay);
	}

	public void Pause() => this.AudioSource.Pause();
	public void Resume() => this.AudioSource.UnPause();
	public void Stop() => this.AudioSource.Stop();

	public GameObject GameObject => this.gameObject;

	public ObjectPool.Pool Pool { get; set; }

	public void OnRelease()
	{
		this._audioSourceControlGroup._OnVolumeChange.RemoveListener(
			call: this.UpdateVolume
		);

		this._audioSourceControlGroup._MasterAudioSourceControlGroup._OnVolumeChange.RemoveListener(
			call: this.UpdateVolume
		);
	}

	public void OnAquire()
	{
		this._audioSourceControlGroup._OnVolumeChange.AddListener(
			call: this.UpdateVolume
		);
		
		this._audioSourceControlGroup._MasterAudioSourceControlGroup._OnVolumeChange.AddListener(
			call: this.UpdateVolume
		);
	}

	private void Awake()
	{
		this.AudioSource = this.GetComponent<AudioSource>();

		this.Volume = this.AudioSource.volume;
	}
}
