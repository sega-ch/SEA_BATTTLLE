using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dispatcher : MonoBehaviour
{
    static Dictionary<string, int> shipsLeftToAllocate = new Dictionary<string, int>();
    static Dictionary<string, Text> shipsLabels = new Dictionary<string, Text>();

    bool isWorkingInstance = true;
    public GameObject shipPrefab;
    protected static Ship currentShip;
    string dictKey;
    string floorNum;
    // Start is called before the first frame update
    protected virtual void Start()

    {
        isWorkingInstance = name.Contains("(Clone)");
        dictKey = gameObject.name.Replace("(Clone)", null);
        floorNum = dictKey.Replace("Ship", null);
        var shipsToAllocate = 5 - int.Parse(floorNum);
        if (!shipsLeftToAllocate.ContainsKey(dictKey))
        {
            shipsLeftToAllocate.Add(dictKey, shipsToAllocate);
            FillLabels();
        } 
        RefreshStrings();
    }
    protected void OnShipClick()
    {
        if (isWorkingInstance) // Проверям кораблик игровой или витринный
        {
            if (currentShip == null)
            {
                currentShip = GetComponentInChildren<Ship>();
            }
            else if (currentShip.IsPositionCorrect)
            {
                if (!currentShip.WasAllocatedOnce()) shipsLeftToAllocate[dictKey]--;
                Debug.Log(shipsLeftToAllocate[dictKey]);
                RefreshStrings();
                currentShip = null;
            }
        }
        else if (currentShip == null) // sample template
        
        {
            if (shipsLeftToAllocate[dictKey] < 1) return;
            var shipToPlay = Instantiate(shipPrefab, transform.parent.transform);
            currentShip = shipToPlay.GetComponentInChildren<Ship>();
        }
    }
    void OnDestroy()
    {
       
    }

    void FillLabels()
    {
        var textBlock = GameObject.Find("label "+dictKey).GetComponent<Text>();
        shipsLabels.Add(textBlock.name.Replace("label ", null), textBlock);
    }

    protected void RefreshStrings()
    {
        shipsLabels[dictKey].text = shipsLeftToAllocate[dictKey] + "x";
    }
}
