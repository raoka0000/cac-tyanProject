/**
 *
 *  You can modify and use this source freely
 *  only for the development of application related Live2D.
 *
 *  (c) Live2D Inc. All rights reserved.
 */
using UnityEngine;
using System;
using live2d.framework;


/*
 * 画面の初期化、ビューに関連するイベント処理（タッチ関連）などを行います。
 *
 */
public class LAppView 
{
    private LAppModel model;
	private L2DMatrix44 		deviceToScreen;
	private AccelHelper 		accelHelper;// 加速度センサの制御
	private TouchManager 		touchMgr;// ピンチなど
	private L2DTargetPoint 		dragMgr;// ドラッグによるアニメーションの管理
	
	private Transform			transform;
	
	private bool				isMove = false;
	private float	 			lastX , lastY ;
	
	protected float canvasWidth = -1 ;
	protected float canvasHeight= -1 ;
	
	
	protected Vector2 touchPos_onPlane ;//touch pos on plane , LeftTop(0,0) RightBottom(1,1)
	protected Vector2 touchPos_onModelCanvas ;
	
	//------------------------------------
	//Unity 'Plane' object layout
	
	private Vector3 localP_LT ;//o
	private Vector3 localP_RT ;//x
	private Vector3 localP_LB ;//y



    public LAppView(LAppModel model, Transform tr)
	{
		this.model = model ;
		transform    = tr;
		
		// デバイス座標からスクリーン座標に変換する
		deviceToScreen=new L2DMatrix44();

		// タッチ関係のイベント管理
		touchMgr = new TouchManager();
		
		dragMgr = new L2DTargetPoint();
		
		Bounds bounds = model.GetBounds();
		localP_LT = new Vector3(-(bounds.size.x/2), 0, (bounds.size.z/2)) ;
		localP_RT = new Vector3( (bounds.size.x/2), 0, (bounds.size.z/2)) ;
		localP_LB = new Vector3(-(bounds.size.x/2), 0,-(bounds.size.z/2)) ;
	}


	public void StartAccel()
	{
		// 加速度関係のイベント
		accelHelper = new AccelHelper() ;
	}
	
	
	/*
	 * タッチイベント。
	 */
    public void TouchesBegan(Vector3 inputPos)
	{
		if(LAppLive2DManager.Instance.IsTouchMode2D() == false)
		{ 
			//  カメラの Projection が Perspective である場合（3Dカメラ）
            UpdateTouchPos_3DCamera(inputPos);
		}
		else
		{ 
			//  カメラの Projection が Orthographic である場合（2Dカメラ）
			UpdateTouchPos_2DCamera(inputPos) ;
		}
		
        var p1x = touchPos_onModelCanvas.x;
        var p1y = touchPos_onModelCanvas.y;

        if (LAppDefine.DEBUG_TOUCH_LOG) Debug.Log("touchesBegan" + " (Device) x:" + inputPos.x + " y:" + inputPos.y + " (Local) x:" + p1x + " y:" + p1y);
        touchMgr.TouchBegan(p1x, p1y);

        //float x = TransformDeviceToViewX(touchMgr.GetX());
        //float y = TransformDeviceToViewY(touchMgr.GetY());

        lastX = p1x;
        lastY = p1y;
	}


    public void TouchesMoved(Vector3 inputPos) 
	{
        if (LAppLive2DManager.Instance.IsTouchMode2D() == false)
		{
			//  カメラの Projection が Perspective である場合（3Dカメラ）
            UpdateTouchPos_3DCamera(inputPos);
		}
		else
		{ 
			//  カメラの Projection が Orthographic である場合（2Dカメラ）
			UpdateTouchPos_2DCamera(inputPos) ;
		}

        var p1x = touchPos_onModelCanvas.x;
        var p1y = touchPos_onModelCanvas.y;

        if (LAppDefine.DEBUG_TOUCH_LOG) Debug.Log("touchesMoved" + " (Device) x:" + inputPos.x + " y:" + inputPos.y + " (Local) x:" + p1x + " y:" + p1y);
        touchMgr.TouchesMoved(p1x, p1y);
        float x = TransformDeviceToViewX(touchMgr.GetX());
        float y = TransformDeviceToViewY(touchMgr.GetY());

        dragMgr.Set(x, y);

        const int FLICK_DISTANCE = 100;// この値以上フリックしたらイベント発生

        // フリックイベントの判定

        if (touchMgr.IsSingleTouch() && touchMgr.IsFlickAvailable())
        {
            float flickDist = touchMgr.GetFlickDistance();
            if (flickDist > FLICK_DISTANCE)
            {
                float touchPos_plane2x2_X = touchPos_onPlane.x * 2 - 1;
                float touchPos_plane2x2_Y = -touchPos_onPlane.y * 2 + 1;
                model.FlickEvent(touchPos_plane2x2_X, touchPos_plane2x2_Y);
                touchMgr.DisableFlick();
            }
        }

        if (lastX != p1x && lastY != p1y)
        {
            isMove = true;
        }
	}


    public void TouchesEnded(Vector3 inputPos)
	{
        if (LAppDefine.DEBUG_TOUCH_LOG) Debug.Log("touchesEnded");
        if (LAppLive2DManager.Instance.IsTouchMode2D() == false)
		{ 
			//  カメラの Projection が Perspective である場合（3Dカメラ）
            UpdateTouchPos_3DCamera(inputPos);
		}
		else
		{ 
			//  カメラの Projection が Orthographic である場合（2Dカメラ）
			UpdateTouchPos_2DCamera(inputPos) ;
		}

        dragMgr.Set(0, 0);

        if (!isMove)
        {
            float touchPos_plane2x2_X = touchPos_onPlane.x * 2 - 1;
            float touchPos_plane2x2_Y = -touchPos_onPlane.y * 2 + 1;
            model.TapEvent(touchPos_plane2x2_X, touchPos_plane2x2_Y);
        }
        else
        {
            isMove = false;
        }
	}

	
	public void SetupView( float width, float height )
	{
		float left	 = LAppDefine.VIEW_LOGICAL_LEFT;
		float right	 = LAppDefine.VIEW_LOGICAL_RIGHT;

		float screenW=Math.Abs(left-right);
		deviceToScreen.identity();
		deviceToScreen.multTranslate(-width/2.0f,height/2.0f );
		deviceToScreen.multScale( screenW/width , screenW/width );
		
		canvasWidth  = width;
		canvasHeight = height;
	}
	
	
	public void Update(Vector3 acceleration)
	{
		dragMgr.update();// ドラッグ用パラメータの更新
		model.setDrag(dragMgr.getX(), dragMgr.getY());
		
		accelHelper.SetCurAccel( acceleration ) ;
		
		accelHelper.Update();

		if( accelHelper.GetShake() > 1.5f )
		{
			if(LAppDefine.DEBUG_LOG) Debug.Log("shake event");
			// シェイクモーションを起動する
			model.ShakeEvent() ;
			accelHelper.ResetShake() ;
		}

		model.setAccel(accelHelper.GetAccelX(), accelHelper.GetAccelY(), accelHelper.GetAccelZ());
	}


	private float TransformDeviceToViewX(float deviceX)
	{
		return deviceToScreen.transformX( deviceX );// 論理座標変換した座標を取得。
	}


	private float TransformDeviceToViewY(float deviceY)
	{
		return deviceToScreen.transformY( deviceY );// 論理座標変換した座標を取得。
	}


	/*
	 * カメラの Projection が Perspective である場合（3Dカメラ）
	 */
	protected void UpdateTouchPos_3DCamera(Vector3 inputPos)
	{
		//--- calc local plane coord ---
		{
			Ray ray = Camera.main.ScreenPointToRay(inputPos);
			
			Vector3 worldP_LT = transform.TransformPoint( localP_LT ) ;
			Vector3 worldP_RT = transform.TransformPoint( localP_RT ) ;
			Vector3 worldP_LB = transform.TransformPoint( localP_LB ) ;
			
			Vector3 PO = worldP_LT ;
			Vector3 VX = worldP_RT - worldP_LT ;
			Vector3 VY = worldP_LB - worldP_LT ;
			
			Vector3 PL = ray.origin ;
			Vector3 VL = ray.direction ;
			
			float Dx = PO.x - PL.x ;
			float Dy = PO.y - PL.y ;
			float Dz = PO.z - PL.z ;
			
			float E = (VX.x*VL.y - VX.y*VL.x) ;
			float F = (VY.x*VL.y - VY.y*VL.x) ;
			float G = (Dx*VL.y - Dy*VL.x) ;
			
			float H = (VX.x*VL.z - VX.z*VL.x) ;
			float I = (VY.x*VL.z - VY.z*VL.x) ;
			float J = (Dx*VL.z - Dz*VL.x) ;
			
			float tmp = ( F*H - E*I ) ;

			if( tmp == 0 )
			{
				//not update value
			}
			else
			{
				touchPos_onPlane.x = ( G*I - F*J ) / tmp ;
				touchPos_onPlane.y = ( E*J - G*H ) / tmp ;
			}
		}
		
		//--- calc touchPos on the model canvas
		{
			float touchPos_plane2x2_X =  touchPos_onPlane.x*2 - 1;
			float touchPos_plane2x2_Y = -touchPos_onPlane.y*2 + 1;
			
			L2DModelMatrix m = model.getModelMatrix() ;
			touchPos_onModelCanvas.x = m.invertTransformX(touchPos_plane2x2_X) ;
			touchPos_onModelCanvas.y = m.invertTransformY(touchPos_plane2x2_Y) ;
		}
	}

	
	/*
	 * カメラの Projection が Orthographic である場合（2Dカメラ）
	 */
	protected void UpdateTouchPos_2DCamera(Vector3 inputPos)
	{
		//--- calc local plane coord ---
		{
			Vector3 worldP_LT = transform.TransformPoint( localP_LT ) ;
			Vector3 worldP_RT = transform.TransformPoint( localP_RT ) ;
			Vector3 worldP_LB = transform.TransformPoint( localP_LB ) ;
			
			Vector3 ScreenLT = Camera.main.WorldToScreenPoint( worldP_LT ) ;
			Vector3 ScreenRT = Camera.main.WorldToScreenPoint( worldP_RT ) ;
			Vector3 ScreenLB = Camera.main.WorldToScreenPoint( worldP_LB ) ;
			
			Vector3 sVX = ScreenRT - ScreenLT;
			Vector3 sVY = ScreenLB - ScreenLT;
			
			
			//  ScreenLTを原点としてsVX、sVYのベクトルで入力のinputPosを成分分解する。
			float x = inputPos.x;
			float y = inputPos.y;
			
			float vax = sVX.x;
			float vay = sVX.y;
			
			float vbx = sVY.x;
			float vby = sVY.y;
			
			float p0x = ScreenLT.x;
			float p0y = ScreenLT.y;
			
			float f = vbx*vay-vby*vax;
			float g = p0y*vax-p0x*vay-y*vax+x*vay;
			
			if(f == 0 || vax == 0)
			{
				//not update value
			}
			else
			{
				float t = g/f;
				float k = (x-p0x-t*vbx)/vax;
				touchPos_onPlane.x = k;
				touchPos_onPlane.y = t;
			}
		}
		
		//--- calc touchPos on the model canvas
		{
			//  0..1の座標を-1..1の座標に揃える。Yは左上を原点にしているため反転。
			float touchPos_plane2x2_X =  touchPos_onPlane.x*2 - 1;
			float touchPos_plane2x2_Y = -touchPos_onPlane.y*2 + 1;
			
			L2DModelMatrix m = model.getModelMatrix() ;
			touchPos_onModelCanvas.x = m.invertTransformX(touchPos_plane2x2_X) ;
			touchPos_onModelCanvas.y = m.invertTransformY(touchPos_plane2x2_Y) ;
		}
	}
}