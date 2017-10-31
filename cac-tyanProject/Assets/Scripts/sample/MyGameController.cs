using UnityEngine;

[ExecuteInEditMode]
public class MyGameController : MonoBehaviour {
    private float lastX = -1;
    private float lastY = -1;

	// Use this for initialization
	void Awake () {

        var camera=GameObject.Find("Main Camera");
        if (camera!=null)
	    {
            if ( camera.GetComponent<Camera>().orthographic)
            {
                LAppLive2DManager.Instance.SetTouchMode2D(true);

            }
            else
            {
                Debug.Log("\"Main Camera\" Projection : Perspective");

                LAppLive2DManager.Instance.SetTouchMode2D(false);

            }

        }

	}
	
	// Update is called once per frame
	void Update () {
        // タッチイベントの取得
        if (Input.GetMouseButtonDown(0))
        {
            lastX = Input.mousePosition.x;
            lastY = Input.mousePosition.y;
            LAppLive2DManager.Instance.TouchesBegan(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            if (lastX == Input.mousePosition.x && lastY == Input.mousePosition.y)
            {
                return;
            }
            lastX = Input.mousePosition.x;
            lastY = Input.mousePosition.y;
            LAppLive2DManager.Instance.TouchesMoved(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lastX = -1;
            lastY = -1;
            LAppLive2DManager.Instance.TouchesEnded(Input.mousePosition);
        }

        // Androidのバックボタンで終了
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }
	}
}