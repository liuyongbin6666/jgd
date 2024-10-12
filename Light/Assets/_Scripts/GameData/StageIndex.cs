public class StageIndex : ModelBase
{
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