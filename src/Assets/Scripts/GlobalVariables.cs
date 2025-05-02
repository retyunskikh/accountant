public class GlobalVariables
{
    public static float speedDefault = 1;
    public float speedScale;

    private static GlobalVariables _instance;
    public static GlobalVariables Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GlobalVariables();
                obj.speedScale = speedDefault;
                _instance = obj;
            }
            return _instance;
        }
    }

    public void AddSpeedScale(float add)
    {
        speedScale = speedScale + add;
    }

    public void SetDefault()
    {
        speedScale = speedDefault;
    }
}