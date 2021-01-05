using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;
using System.Collections.Generic;

public class AutomaticAnimatedTile: EditorWindow
{
    [MenuItem("Tiles/Automatic Animated Tile")]
    static void Init()
    {
        AutomaticAnimatedTile window = ScriptableObject.CreateInstance<AutomaticAnimatedTile>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 400);
        window.ShowPopup();
    }

    string assetName;
    int frameCount;
    float imageSpeed;
    List<Sprite> frameToSpritesheet = new List<Sprite>();

    void OnGUI()
    {
        assetName = EditorGUILayout.TextField("Tile asset name", assetName);
        imageSpeed = EditorGUILayout.FloatField("Speed", imageSpeed);

        var list = frameToSpritesheet;
        int newCount = Mathf.Max(0, EditorGUILayout.IntField("frame count", list.Count));
        frameCount = newCount;
        while (newCount < list.Count)
            list.RemoveAt(list.Count - 1);
        while (newCount > list.Count)
            list.Add(null);

        for (int i = 0; i < list.Count; i++)
        {
            list[i] = (Sprite)EditorGUILayout.ObjectField(list[i], typeof(Sprite), false);
        }
        
        if (GUILayout.Button("Create tile"))
        {
            try
            {
                int numberOfTilesInSpriteSheet;
                {
                    string spriteSheet = AssetDatabase.GetAssetPath(frameToSpritesheet[0]);
                    Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet)
                        .OfType<Sprite>().ToArray();
                    numberOfTilesInSpriteSheet = sprites.Length;
                }
                Sprite[,] frameSpriteArray = new Sprite[numberOfTilesInSpriteSheet, frameCount]; //First index-numberOfTiles second index-frame

                for (int i = 0; i < frameCount; i++)
                {
                    string spriteSheet = AssetDatabase.GetAssetPath(frameToSpritesheet[i]);
                    Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheet)
                        .OfType<Sprite>().ToArray();

                    if (sprites.Length != numberOfTilesInSpriteSheet)
                    {
                        throw new System.Exception("Sprite sheet sizes are not equal!");
                    }

                    for (int j = 0; j < numberOfTilesInSpriteSheet; j++)
                    {
                        frameSpriteArray[j, i] = sprites[j];
                    }
                }

                string outputFolder = "Assets/Editor/Output";
                Debug.Log("Creating assets in: " + outputFolder);

                for (int i = 0; i < numberOfTilesInSpriteSheet; i++)
                {
                    AnimatedTile tile = ScriptableObject.CreateInstance("AnimatedTile") as AnimatedTile;

                    tile.m_MaxSpeed = imageSpeed;
                    tile.m_MinSpeed = imageSpeed;

                    tile.m_AnimatedSprites = new Sprite[frameCount];

                    for (int j = 0; j < frameCount; j++)
                    {
                        tile.m_AnimatedSprites[j] = frameSpriteArray[i, j];
                    }

                    AssetDatabase.CreateAsset(tile, outputFolder + "/" + assetName + i.ToString() + ".asset");
                }

                Debug.Log("Successful");
                this.Close();
            }
            catch (System.Exception e)
            {

                Debug.LogError(e.StackTrace);
            }
        }
        

        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
    }
}
