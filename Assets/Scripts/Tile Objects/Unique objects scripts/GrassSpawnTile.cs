using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpawnTile : MonoBehaviour
{
    [SerializeField] private List<Sprite> grassSprites = null;

    float maxOffset = 0.4f;

    private void Awake()
    {

        int spawnCount = Random.Range(2, 4);

        for (int i = 0; i < spawnCount; i++)
        {
            float xOffset = Random.Range(-maxOffset, maxOffset);
            float yOffset = Random.Range(-maxOffset, maxOffset);

            GameObject grassObject = new GameObject("Grass");
            Transform grassTransform = grassObject.transform;
            grassTransform.SetParent(transform);

            SpriteRenderer renderer = grassObject.AddComponent<SpriteRenderer>();
            renderer.sortingLayerName = "DynamicY";
            renderer.sprite = grassSprites[Random.Range(0, grassSprites.Count)];

            float depth = DynamicZDepth.GetDynamicZDepth(transform.position.y + yOffset, DynamicZDepth.OBJECTS_STANDARD_OFFSET);
            grassTransform.position = new Vector3(transform.position.x + xOffset,
                                                         transform.position.y + yOffset,
                                                         depth);
        }
    }
}
