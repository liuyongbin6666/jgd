using GMVC.Utls;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Utls;

namespace Components
{
    /// <summary>
    /// Boss的情节控件
    /// </summary>
    public class BossPlotComponent : PlotComponent
    {
        [LabelText("Boss控件")]public StoryBossComponent Boss;
        [LabelText("Boss同步情节禁用")]public bool DisableWhenPlotInActive;
        protected override void OnBegin()
        {
            base.OnBegin();
            Boss.StoryStart(this);
        }

        public override void Active(bool active)
        {
            base.Active(active);
            if (DisableWhenPlotInActive && !active && !Boss.IsUnityNull() && !Boss.Enemy.IsUnityNull())
                 Boss.Enemy.Display(false);
        }
    }
}