using CommandView;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private GameObject _planet;
    private Planet _planetHandler;
    private GameMeta _gameMeta;

    public GameObject[] faces;

    public int turnsElapsed;
    public int money;
    public int nukes;

    public int wumpusLocation;

    public int[] biomeNum;
    public bool[] isColonized;
    public int[] hazardType;

    public int[] troopType;
    public string[] troopName;
    public bool[] isEhausted;

    public SaveData(Planet planet)
    {
        turnsElapsed = _gameMeta.turnsElapsed;
        money = _gameMeta.money;
        nukes = _gameMeta.nukes;

        wumpusLocation = planet.wumpus.location.GetTileNumber();

        int i = 0;
        foreach (GameObject face in planet.faces)
        {
            FaceHandler faceHandler = face.GetComponent<FaceHandler>();
            biomeNum[i] = (int)faceHandler.biomeType;
            isColonized[i] = faceHandler.colonized;
            hazardType[i] = (int)faceHandler.GetHazardObject();
            i++;
        }

        i = 0;
        foreach (TroopMeta troop in _gameMeta.availableTroops)
        {
            troopType[i] = (int)troop.type;
            troopName[i] = troop.name;
            isEhausted[i] = false;
        }
        foreach (TroopMeta troop in _gameMeta.availableTroops)
        {
            troopType[i] = (int)troop.type;
            troopName[i] = troop.name;
            isEhausted[i] = true;
        }
    }
}