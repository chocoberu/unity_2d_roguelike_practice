using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null; 

    public float lowPitchRange = .95f; // 원음 피치의 95%
    public float highPitchRange = 1.05f; // 원음 피치의 105%

	// Use this for initialization
	void Awake ()
     { // 싱글턴 패턴, 한번에 단 하나만 존재 가능한 오브젝트
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
	}
	
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }
    public void RandomizeSfx(params AudioClip[] clips) // params -> 콤마로 구분된 같은 타입의 여러 매개변수를 한번에 받게함
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
	
}
