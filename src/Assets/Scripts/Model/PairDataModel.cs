using System.Collections.Generic;

public class PairDataModel
{
    public ExpressionTypes ExpressionType;

    public List<SpawnedDataModel> SpawnedObjects {  get; set; }

    public PairDataModel(ExpressionTypes expressionType, List<SpawnedDataModel> spawnedObjects)
    {
        ExpressionType = expressionType;
        SpawnedObjects = spawnedObjects;
    }
}