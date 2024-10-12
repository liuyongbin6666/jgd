using GMVC.Core;

public class StoryManager : ModelBase
{
    //故事Id
    public int[] StoryId { get; private set; } 

    public void SetStory()
    {
        StoryId = new[] { 1, 2, 3, 4, 5 } ;
    }
}