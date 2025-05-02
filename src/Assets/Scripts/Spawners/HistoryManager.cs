
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Хранение всех предыдущих мультипликаторов
/// </summary>
public class HistoryManager : MonoBehaviour
{
    private List<SpawnedDataModel> spawnedObjects = new List<SpawnedDataModel>();
    private int possiblePlayerMass = PlayerStartMass.Value;

    private static HistoryManager _instance;
    public static HistoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("History");
                _instance = obj.AddComponent<HistoryManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    public void HistoryAdd(SpawnedDataModel obj)
    {
        spawnedObjects.Add(obj);
    }

    public SpawnedDataModel HistoryLastGet()
    {
        if(spawnedObjects.Count>0)
        {
            return spawnedObjects.Last();
        }
        return null;
    }

    public void PossibleMassAdd(List<SpawnedDataModel> objs)
    {
        var valueAfterAdd = objs.Where(x => x.ExpressionType == ExpressionTypes.Addition).Single().Value + possiblePlayerMass;
        var valueAfterMultiplication = objs.Where(x => x.ExpressionType == ExpressionTypes.Multiplication).Single().Value * possiblePlayerMass;

        if(valueAfterAdd> valueAfterMultiplication)
        {
            possiblePlayerMass = valueAfterAdd;
        }
        else
        {
            possiblePlayerMass = valueAfterMultiplication;
        }
    }

    public void PossibleMassSubtraction(int value)
    {
        possiblePlayerMass -= value;
    }

    public int PossibleMassGet()
    {
        return possiblePlayerMass;
    }

    public void Clear()
    {
        spawnedObjects = new List<SpawnedDataModel>();
        possiblePlayerMass = PlayerStartMass.Value;
    }
}