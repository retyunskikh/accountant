using System;
using UnityEngine;

public class PositiveModel: MonoBehaviour
{
    public Guid Id { get; set; }

    /// <summary>
    /// Уникальный идентификатор используется для поиска пары - объекта, который был создан в тот же момент
    /// </summary>
    public Guid PairId {  get; set; }

    public Vector2 CenterPos;
    public float Width;
    public float Height;
    public ExpressionTypes ExpressionType;
    public int MultiplicationValue;


    public PositiveModel(Vector2 centerPos, float width, float height, ExpressionTypes expressionType, Guid pairId, int multiplicationValue)
    {
        CenterPos = centerPos;
        Width = width;
        Height = height;
        ExpressionType = expressionType;
        PairId = pairId;
        MultiplicationValue = multiplicationValue;
    }
}