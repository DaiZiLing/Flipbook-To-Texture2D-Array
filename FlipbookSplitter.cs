using UnityEngine;
using UnityEditor;

public class FlipbookSplitter : EditorWindow
{
    public Texture2D flipbookTexture;
    public string outputFilePath = "Assets/T_OceanWave.asset";

    public int rowIndex = 4;
    public int colIndex = 4;

    [MenuItem("Window/Tools/Flipbook Splitter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FlipbookSplitter));
    }

    void OnGUI()
    {
        GUILayout.Label("Flipbook Splitter", EditorStyles.boldLabel);

        flipbookTexture = (Texture2D)EditorGUILayout.ObjectField("Flipbook Texture", flipbookTexture, typeof(Texture2D), false);
        outputFilePath = EditorGUILayout.TextField("Output File Path", outputFilePath);
        rowIndex = EditorGUILayout.IntField("Row Index", rowIndex);
        colIndex = EditorGUILayout.IntField("Column Index", colIndex);

        if (GUILayout.Button("Split and Create Texture2DArray"))
        {
            SplitFlipbookIntoTextures();
        }
    }

    void SplitFlipbookIntoTextures()
    {
        if (flipbookTexture == null)
        {
            Debug.LogError("Please assign a flipbook texture.");
            return;
        }

        // Enable Read/Write
        string texturePath = AssetDatabase.GetAssetPath(flipbookTexture);
        TextureImporter textureImporter = AssetImporter.GetAtPath(texturePath) as TextureImporter;
        textureImporter.isReadable = true;
        AssetDatabase.ImportAsset(texturePath);

        int cellSize = flipbookTexture.width / colIndex;
        Texture2DArray textureArray = new Texture2DArray(cellSize, cellSize, rowIndex * colIndex, TextureFormat.RGBA32, false);

        for (int i = 0; i < rowIndex * colIndex; i++)
        {
            int currentRowIndex = i / colIndex;
            int currentColIndex = i % colIndex;

            Texture2D cellTexture = new Texture2D(cellSize, cellSize, TextureFormat.RGBA32, false);
            cellTexture.SetPixels(flipbookTexture.GetPixels(currentColIndex * cellSize, currentRowIndex * cellSize, cellSize, cellSize));
            cellTexture.Apply();

            textureArray.SetPixels(cellTexture.GetPixels(), i);
            DestroyImmediate(cellTexture);
        }

        textureArray.Apply();

        AssetDatabase.CreateAsset(textureArray, outputFilePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Disable Read/Write
        textureImporter.isReadable = false;
        AssetDatabase.ImportAsset(texturePath);

        Debug.Log("Flipbook split and Texture2DArray created: " + outputFilePath);
    }
}