public class StageIndex : ModelBase
{
    //关卡Id
    public int Index { get; private set; }

    public void Add()
    {
        Index++;
    }

    public void Reset()
    {
        Index = 0;
    }
}