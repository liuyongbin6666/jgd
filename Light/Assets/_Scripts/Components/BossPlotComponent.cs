using Sirenix.OdinInspector;

namespace Components
{
    /// <summary>
    /// Boss的情节控件
    /// </summary>
    public class BossPlotComponent : PlotComponent
    {
        [LabelText("Boss控件")]public StoryBossComponent Boss;
        protected override void OnBegin()
        {
            base.OnBegin();
            Boss.StoryStart(this);
        }
    }
}