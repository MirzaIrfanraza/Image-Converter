#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public enum FileFormat
{
    png,jpg,bmp,exr,gif,hdr,iff,pict,psd,tga,tiff
}

public class ImageConverter : MonoBehaviour
{
    #region PUBLIC_VARS
    public Texture2D[] textures;
    public DefaultAsset destinationFolder;
    public FileFormat convertTo;
    #endregion

    #region PRIVATE_VARS

    #endregion

    #region UNITY_CALLBACKS

    #endregion

    #region PUBLIC_METHODS

    public Byte[] ConvertTextureToByteArray(Texture2D texture)
    {
        return texture.EncodeToPNG();
    }
    [ContextMenu("MakeTextureReadable")]
    public void MakeTextureReadable()
    {
        foreach (var textureToConvert in textures)
        {
            string assetPath = AssetDatabase.GetAssetPath( textureToConvert );
            var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
            if ( tImporter != null )
            {
                tImporter.textureType = TextureImporterType.Default;

                tImporter.isReadable = true;
                Debug.Log("true");

                AssetDatabase.ImportAsset( assetPath );
                AssetDatabase.Refresh();
            }
        }
    }
    
    [ContextMenu("Convert")]
    public void Convert()
    {
        MakeTextureReadable();
        Debug.Log(destinationFolder.name);        
        Debug.Log(AssetDatabase.GetAssetPath(destinationFolder));
        string path = AssetDatabase.GetAssetPath(destinationFolder);
        foreach (var texture in textures)
        {
            Texture2D textureToConvert = DeCompress(texture);
            Debug.Log(textureToConvert.name);
            Debug.Log(textureToConvert.height);
            Debug.Log(textureToConvert.width);
            
            Sprite sprite = Sprite.Create(textureToConvert,
                new Rect(0f, 0f, textureToConvert.width, textureToConvert.height), new Vector2(.5f, .5f), 300f);
            string finalPath = path + "/" + textureToConvert.name + "."+convertTo;
            
            File.WriteAllBytes(Application.dataPath + "/../" + finalPath, textureToConvert.EncodeToPNG());
            AssetDatabase.Refresh();
            AssetDatabase.AddObjectToAsset(sprite, finalPath);
            AssetDatabase.SaveAssets();
            Debug.Log("Converted");
        }    
    }
    #endregion

    #region PRIVATE_METHODS
    public Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.name = source.name;
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
    #endregion
}
#endif
