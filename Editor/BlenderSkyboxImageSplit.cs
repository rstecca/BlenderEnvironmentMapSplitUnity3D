using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

public class BlenderSkyboxImageSplit : EditorWindow {

    enum SIDE { BOTTOM, TOP, FRONT, RIGHT, BACK, LEFT, ___ };

    Texture2D tex;

    /// <summary>
    /// Spilt the specified tex into 6 textures of size <i>size</i>
    /// This is used to cut the environment image as exported by Blender 3D.
    /// </summary>
    /// <param name="tex">Tex.</param>
    /// <param name="size">Size.</param>
    public static void Spilt(Texture2D tex, int size)
    {
        Assert.IsTrue(tex.mipmapCount == 1, "Given texture must have generate mipmaps DISABLED");

        string path = Application.dataPath+"/";

        Texture2D[] textures = new Texture2D[6];

        Debug.Log("Texture format: " + tex.format.ToString());

        for (int i = 0; i < (int)SIDE.___; i++)
        {
            int X = size * (i%3);
            int Y = size * Mathf.FloorToInt(i / 3);
            textures [i] = new Texture2D(size, size,TextureFormat.ARGB32, false);
            textures [i].wrapMode = TextureWrapMode.Clamp;
            textures [i].SetPixels(tex.GetPixels(X, Y, size, size));
            textures [i].Apply(false);
            SaveTextureToFile(textures [i], path + tex.name + "_" + ((SIDE)i).ToString() + ".png");
            DestroyImmediate(textures [i]);
        }
        Debug.Log("6 textures saved to " + path);
        AssetDatabase.Refresh();
    }


    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Blender Skybox Image Split")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        BlenderSkyboxImageSplit window = (BlenderSkyboxImageSplit)EditorWindow.GetWindow(typeof(BlenderSkyboxImageSplit));
        window.Show();
    }

    static void SaveTextureToFile (Texture2D texture, string filename) {
        System.IO.File.WriteAllBytes (filename, texture.EncodeToPNG());
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Splits an environment map image as exported from Blender into 6 separate images.");
        tex = (Texture2D) EditorGUILayout.ObjectField("Image", tex, typeof(Texture2D), false);
        if (GUILayout.Button("SPLIT"))
        {
            BlenderSkyboxImageSplit.Spilt(tex, Mathf.FloorToInt(tex.height/2));
            //tex = null;
        }
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(
            "Source image must be:\n- sRGB color : true§- Alpha is Transparency: true\n- Non power of 2: None\n-Read/Write Enabled: true\n- Generate Mip Maps: false\n-Wrap Mode: Clamp\n- Filter Mode: Point(none)\n- Compression: None (importat)",
            GUILayout.MinHeight(300f)
        );
        EditorGUILayout.EndHorizontal();
    }
}
