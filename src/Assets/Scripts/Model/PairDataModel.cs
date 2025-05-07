using System.Collections.Generic;

public class PairDataModel
{
    public ExpressionTypes ExpressionType;

    public List<SpawnedShortDataModel> SpawnedObjects {  get; set; }

    public PairDataModel(ExpressionTypes expressionType, List<SpawnedShortDataModel> spawnedObjects)
    {
        ExpressionType = expressionType;
        SpawnedObjects = spawnedObjects;
    }
}