
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Хранение всех предыдущих мультипликаторов
/// </summary>
public class HistoryManager : MonoBehaviour
{
    private List<SpawnedObject> multiplications = new List<SpawnedObject>();
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

    public void AddHistory(SpawnedObject obj)
    {
        multiplications.Add(obj);
    }

    public void PossibleMassAdd(List<SpawnedObject> objs)
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

    public int GetAddPossibleMass()
    {
        return possiblePlayerMass;
    }

    public void Clear()
    {
        multiplications = new List<SpawnedObject>();
        possiblePlayerMass = PlayerStartMass.Value;
    }
}