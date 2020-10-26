using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wildlife/Fish")]
public class FishInformation : WildlifeInformation
{
    [SerializeField] private GameObject prefab = null;

    public static float FISH_SEEING_DISTANCE = 3f;
    public static float FISH_SEEING_ANGLE_DOT_PRODUCT = 0.707f; //0.707 is 45 degrees
    public static float FISH_WORLD_WIDTH;

    public void OnEnable()
    {
        FISH_WORLD_WIDTH = prefab.GetComponent<SpriteRenderer>().sprite.rect.width / 16f;
    }

    public override bool TrySpawn(Vector2 pos, out WildlifeBehaviour behaviourScript)
    {
        TileInformation tileInfo = TileInformationManager.Instance.GetTileInformation(new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)));

        if (tileInfo == null)
            throw new System.Exception("Invalid position");

        if (tileInfo.tileLocation == TileLocation.DeepWater) {
            GameObject obj = Instantiate(prefab, pos, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
            behaviourScript = obj.GetComponent<WildlifeBehaviour>();
            return true;
        }
        else
        {
            behaviourScript = null;
            return false;
        }
    }
}
