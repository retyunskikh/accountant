
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Хранение всех предыдущих объектов на увеличесние и уменьшение массы
/// </summary>
public class HistoryManager : MonoBehaviour
{
    private List<PairDataModel> spawnedPairs = new List<PairDataModel>();
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

    public void HistoryAdd(PairDataModel obj)
    {
        spawnedPairs.Add(obj);
    }

    public SpawnedDataModel HistoryLastGet()
    {
        if (spawnedPairs.Count > 0)
        {
            return spawnedPairs.Last().SpawnedObjects.Last();
        }
        return null;
    }

    public int GetSubstructorCount()
    {
        var result = spawnedPairs.Where(x=>x.ExpressionType == ExpressionTypes.SubtractionAndDivision).Count();
        return result;
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
        spawnedPairs = new List<PairDataModel>();
        possiblePlayerMass = PlayerStartMass.Value;
    }
}