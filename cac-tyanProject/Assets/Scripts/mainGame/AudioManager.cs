using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
	// シングルトン
	private static AudioManager _instance;
	public static AudioManager instance {
		get {
			if (_instance == null) {
				// シーン上から取得する
				_instance = FindObjectOfType<AudioManager> ();
				if (_instance == null) {
					_instance = new GameObject ("AudioManager").AddComponent<AudioManager> ();
				}
			}
			return _instance;
		}
	}
		
	public AudioSource audioSource;
	public AudioClip questionButtonSound;
	public AudioClip[] stageBGMs;

	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
	}


	public void PlayQuestionButtonSound(){
		audioSource.PlayOneShot (questionButtonSound);
	}

	public void PlayBGM(int stageId){
		audioSource.clip = stageBGMs [stageId - 1];
		audioSource.Play ();
	}

}
