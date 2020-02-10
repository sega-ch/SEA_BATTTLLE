using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dispatcher : MonoBehaviour
{
    static Dictionary<string, int> shipsLeftToAllocate = new Dictionary<string, int>();
    static Dictionary<string, Text> labelF = new Dictionary<string, Text>();

    public GameObject shipPrefab;
    protected static Ship currentShip;
    string dictKey;
    string floorNum;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        dictKey = gameObject.name.Replace("(Clone)", null);
        floorNum = dictKey.Replace("Ship", null);
        var shipsToAllocate = 5 - int.Parse(floorNum);
        if (!shipsLeftToAllocate.ContainsKey(dictKey)) shipsLeftToAllocate.Add(dictKey, shipsToAllocate);
        RefreshStrings();
        FillLabels();
    }
    protected void OnShipClick()
    {
        if (gameObject.name.Contains("(Clone)")) // Проверям кораблик игровой или витринный
        {
            if (currentShip == null) currentShip = GetComponentInChildren<Ship>();
            else if (currentShip.WasAllocatedOnce()) currentShip = null;
        }
        else if (currentShip == null) // sample template
        {
            if (shipsLeftToAllocate[dictKey] < 1) return;
            var shipToPlay = Instantiate(shipPrefab, transform.parent.transform);
            currentShip = shipToPlay.GetComponentInChildren<Ship>();
            shipsLeftToAllocate[dictKey]--;
            RefreshStrings();
        }
    }
    void OnDestroy()
    {
        shipsLeftToAllocate[dictKey]++;
        RefreshStrings();
    }

    void FillLabels()
    {
        var objectLabel = GameObject.Find(dictKey+"label");
        Debug.Log(dictKey+"label");

    }

    protected void RefreshStrings()
    {
        var labels = transform.parent.GetComponentsInChildren<Text>();
        foreach (var label in labels)
            if (label.gameObject.name.Contains("label" + floorNum))
                label.text = shipsLeftToAllocate[dictKey].ToString();
    }
}
