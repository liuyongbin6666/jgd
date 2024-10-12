using GMVC.Core;

public class StoryManager : ModelBase
{
    public int[] StoryId { get; private set; } = new[] { 1, 2, 3, 4, 5 };

    public int GetStoryId()
    {
        return StoryId[Game.World.Stage.StageIndex.Index];
    }
}