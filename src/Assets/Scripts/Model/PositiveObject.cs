using System;
using UnityEngine;

public class PositiveObject: MonoBehaviour
{
    public Guid Id { get; set; }

    /// <summary>
    /// Уникальный идентификатор используется для поиска пары - объекта, который был создан в тот же момент
    /// </summary>
    public Guid PairId {  get; set; }
}