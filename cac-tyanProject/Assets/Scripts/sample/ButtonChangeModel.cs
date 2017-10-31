/**
 *
 *  You can modify and use this source freely
 *  only for the development of application related Live2D.
 *
 *  (c) Live2D Inc. All rights reserved.
 */
using UnityEngine;

public class ButtonChangeModel : MonoBehaviour 
{
	void Awake()
	{
		int size = Screen.height / 14 ;
		Rect rctGUISize = new Rect(0,0,size, size);
		this.GetComponent<GUITexture>().pixelInset = rctGUISize ;	
	}
	
	
	void Start() 
	{
	}

	
	void Update()
	{
		// Android、iOSでのモデル切り替え
		if (Application.platform == RuntimePlatform.IPhonePlayer ||
		    Application.platform == RuntimePlatform.Android)
		{
			foreach (Touch t in Input.touches) 
			{
				if (GetComponent<GUITexture>().HitTest (t.position, Camera.main) && t.phase == TouchPhase.Began) 
				{
                    LAppLive2DManager.Instance.ChangeModel();
				}
			}
		}
		
		
	}
	

	void OnMouseDown() 
	{
		// Android、iOS以外のモデル切り替え
		if (Application.platform != RuntimePlatform.IPhonePlayer &&
		    Application.platform != RuntimePlatform.Android)
		{
            LAppLive2DManager.Instance.ChangeModel();
		}
	}
}