/**
 *
 *  You can modify and use this source freely
 *  only for the development of application related Live2D.
 *
 *  (c) Live2D Inc. All rights reserved.
 */
using UnityEngine;
using System.Collections.Generic;
using live2d;
using live2d.framework;



public class LAppLive2DManager
{
    private static LAppLive2DManager instance;
    public static LAppLive2DManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LAppLive2DManager();
            }

            return instance;
        }
    }

	//  モデル表示用ゲームオブジェクト
    private List<LAppModelProxy> models;
    private bool touchMode2D;

    private int sceneIndex = 0;

    public LAppLive2DManager()
    {
        Live2D.init();
        Live2DFramework.setPlatformManager(new PlatformManager());
        models = new List<LAppModelProxy>();
    }


    public void AddModel(LAppModelProxy item)
    {
        if (LAppDefine.DEBUG_LOG)
        {
            Debug.Log("Add Live2D Model : " + item.sceneNo);
        }

        models.Add(item);
        UpdateScene();
    }


    public void SetTouchMode2D(bool value)
    {
        touchMode2D = value;
    }


    public bool IsTouchMode2D()
    {
        return touchMode2D;
    }


    //Live2D Scnene
    public void UpdateScene()
    {
        bool initFlg = true;
        for (int i = 0; i < models.Count; i++)
        {
            var model = models[i];
            if (sceneIndex == model.sceneNo)
            {
                initFlg = false;
            }
        }

        if (initFlg)
        {
            // このシーン番号で一致するモデルがないので、初期シーンへ
            sceneIndex = 0;
        }

        for (int i = 0; i < models.Count; i++)
        {
            var model = models[i];
            if (sceneIndex == model.sceneNo)
            {
                model.SetVisible(true);
            }
            else
            {
                model.SetVisible(false);
            }
        }
    }

    public void ChangeModel()
    {
        if (LAppDefine.DEBUG_LOG)
        {
            Debug.Log("Live2D Scene : " + sceneIndex);
        }

        sceneIndex++;
        UpdateScene();
    }

    internal void TouchesBegan(Vector3 inputPos)
    {
        for (int i = 0; i < models.Count; i++)
        {
            if (models[i].GetVisible())
            {
                models[i].GetModel().TouchesBegan(inputPos);
            }
        }
    }

    internal void TouchesMoved(Vector3 inputPos)
    {
        for (int i = 0; i < models.Count; i++)
        {
            if (models[i].GetVisible())
            {
                models[i].GetModel().TouchesMoved(inputPos);
            }
        }
    }

    internal void TouchesEnded(Vector3 inputPos)
    {
        for (int i = 0; i < models.Count; i++)
        {
            if (models[i].GetVisible())
            {
                models[i].GetModel().TouchesEnded(inputPos);
            }
        }
    }
}