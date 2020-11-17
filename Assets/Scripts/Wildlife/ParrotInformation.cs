using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wildlife/Parrot")]
public class ParrotInformation : WildlifeInformation
{
    [SerializeField] private GameObject[] prefabChoices = null;

    public override bool TrySpawn(Vector2 pos, out WildlifeBehaviour behaviourScript)
    {
        TileInformationManager.Instance.TryGetTileInformation(new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)), out TileInformation tileInfo);

        if (tileInfo == null)
            throw new System.Exception("Invalid position");

        if (TileLocation.Land.HasFlag(tileInfo.tileLocation))
        {
            GameObject randomPrefab = prefabChoices[Random.Range(0, prefabChoices.Length)];
            GameObject obj = Instantiate(randomPrefab, pos, Quaternion.identity);
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
