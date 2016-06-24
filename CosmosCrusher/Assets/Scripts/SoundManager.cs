using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

    private List<AudioSource> audioSources = new List<AudioSource>();
    public AudioSource audioPrefab;
    public float volume;
    public AudioClip bulletFireSound;
    public AudioClip shipDestructionSound;
    public AudioClip shipTakeoverSound;
    public AudioClip bossDestructionSound;
    public AudioClip shipSpawnSound;
    public AudioClip menuBackground;
    public AudioClip planetBackground;
    public AudioClip bossBackground;
    private bool paused = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!paused)
        {
            List<AudioSource> finishedSources = new List<AudioSource>();
            foreach (AudioSource source in audioSources)
            {
                if (!source.isPlaying)
                {
                    finishedSources.Add(source);
                }
            }
            foreach (AudioSource source in finishedSources)
            {
                audioSources.Remove(source);
                Destroy(source.gameObject);
            }
        }
	}

    public void PlayBulletFire()
    {
        PlaySound(bulletFireSound, 0.3f, 1.0f);
    }

    public void PlayShipDestruction()
    {
        PlaySound(shipDestructionSound, 1.0f, 1.0f);
    }

    public void PlayShipTakeover()
    {
        PlaySound(shipTakeoverSound, 1.0f, 1.0f);
    }

    public void PlayBossDestruction()
    {
        PlaySound(bossDestructionSound, 1.0f, 0.5f);
    }

    public void PlayShipSpawn()
    {
        PlaySound(shipSpawnSound, 1.0f, 1.0f);
    }

    private void PlaySound(AudioClip sound, float volumeChange, float pitch)
    {
        AudioSource source = Instantiate(audioPrefab) as AudioSource;
        source.clip = sound;
        source.loop = false;
        source.volume = volume * volumeChange;
        source.pitch = pitch;
        source.Play();
        audioSources.Add(source);
    }

    private void PlayBackground(AudioClip sound, float volumChange, float pitch)
    {
        AudioSource source = Instantiate(audioPrefab) as AudioSource;
        source.clip = sound;
        source.loop = true;
        source.volume = volume * volumChange;
        source.pitch = pitch;
        source.Play();
        audioSources.Add(source);
    }
    public void PauseSounds()
    {
        paused = true;
        foreach (AudioSource source in audioSources)
        {
            source.Pause();
        }
    }
    public void UnPauseSounds()
    {
        paused = false;
        foreach (AudioSource source in audioSources)
        {
            source.UnPause();
        }
    }
    public void PlayMenuBackground()
    {
        PlayBackground(menuBackground, 0.3f, 1.0f);
    }
    public void PlayPlanetBackground()
    {
        PlayBackground(planetBackground, 0.1f, 1.0f);
    }
    public void PlayBossBackground()
    {
        PlayBackground(bossBackground, 0.3f, 1.0f);
    }
}
