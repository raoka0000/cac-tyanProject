﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BuckMainScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		LAppLive2DManager.Instance.Dispose ();
		SceneManager.LoadScene ("main");
	}
}
