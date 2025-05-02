using UnityEngine;
public class SpawnedObject : MonoBehaviour
{
    public int Value;

    public ExpressionTypes ExpressionType;

    public SpawnedObject(int value, ExpressionTypes expressionType)
    {
        Value = value;
        ExpressionType = expressionType;
    }
}