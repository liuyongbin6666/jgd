using System.Collections.Generic;
using GMVC.Utls;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Utls;

namespace Components
{
    /// <summary>
    /// Boss的情节控件
    /// </summary>
    public class BossPlotComponent : PlotComponentBase
    {
        [LabelText("Boss控件")]public StoryBossComponent Boss;
        protected override bool DisableLines => true;

        protected override void OnBegin()
        {
            if (mode == TextMode.Interaction) Boss.OnSeekEvent.AddListener(SendLines);
            Boss.StoryStart(this);
            if(mode == TextMode.Begin) SendLines();
        }

        protected override void OnFinalization()
        {
            if (mode == TextMode.Finalize) SendLines();
        }

        public override void Active(bool active)
        {
            base.Active(active);
            if (!Boss.IsUnityNull() && !Boss.Enemy.IsUnityNull()) 
                Boss.Enemy.Display(active);
        }
    }
}