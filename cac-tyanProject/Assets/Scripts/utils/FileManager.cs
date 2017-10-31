/**
 *
 *  You can modify and use this source freely
 *  only for the development of application related Live2D.
 *
 *  (c) Live2D Inc. All rights reserved.
 */
using UnityEngine;
using System.IO;
using System;
using live2d;
using System.Text.RegularExpressions;

public class FileManager  
{
	public static TextAsset LoadTextAsset( string path )
	{
		return (TextAsset)Resources.Load( path , typeof(TextAsset) ) ;
	}
	

	public static Texture2D LoadTexture( string path )
	{
		return (Texture2D)Resources.Load( path , typeof(Texture2D) ) ;
	}
	
	
	public static AudioClip LoadAssetsSound(string filename)
	{
		if(LAppDefine.DEBUG_LOG) Debug.Log( "Load voice : "+filename);

		AudioClip player = new AudioClip() ;
        
		try
		{
            player = (AudioClip)(Resources.Load(filename)) as AudioClip;
			
		}
		catch (IOException e)
		{
			Debug.Log( e.StackTrace );
		}

		return player;
	}


	public static byte[] LoadBin(string path)
	{
		TextAsset ta = (TextAsset)Resources.Load( path , typeof(TextAsset) ) ;
		byte[] buf = ta.bytes ;
		return buf;
	}


	public static String LoadString(string path)
	{
		return System.Text.Encoding.GetEncoding("UTF-8").GetString(LoadBin(path));
	}


    public static string getFilename(string url)
    {
        return Regex.Replace(url, ".*/", "");
    }


    public static string getDirName(string url)
    {
        return Regex.Replace(url, "(.*/)(.+)", "$1");
    }
}