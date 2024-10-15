using GMVC.Core;

public class StoryManager : ModelBase
{
    //故事Id
    private int[] StoryId { get; set; } 

    public void SetStory()
    {
        StoryId = new[] { 1, 2, 3, 4, 5 } ;
    }

    public int GetStoryId()
    {
        return StoryId[Game.World.Stage.StageIndex.Index];
    }
}