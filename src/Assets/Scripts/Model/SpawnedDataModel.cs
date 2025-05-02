using UnityEngine;
public class SpawnedDataModel
{
    public int Value;

    public ExpressionTypes ExpressionType;

    public SpawnedDataModel(int value, ExpressionTypes expressionType)
    {
        Value = value;
        ExpressionType = expressionType;
    }
}