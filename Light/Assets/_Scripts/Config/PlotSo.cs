using Sirenix.OdinInspector;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = "情节", menuName = "配置/情节")]
    public class PlotSo : PlotSoBase
    {
        [ValueDropdown(nameof(GetPlotNames), IsUniqueList = true,
                       DrawDropdownForListElements = false,
                       ExcludeExistingValuesInList = true),
         HideIf(nameof(isStoryFinalize), optionalValue: true), 
         LabelText("下个情节")]
        public string[] nextPlots;
        [ValueDropdown(nameof(GetPlotNames), IsUniqueList = true,
             DrawDropdownForListElements = false,
             ExcludeExistingValuesInList = true),
         HideIf(nameof(isStoryFinalize), optionalValue: true),
         LabelText("禁用情节(开启时)")]
        public string[] closePlots;
        public override string[] NextPlots() => nextPlots;
        public override string[] DisablePlots() => closePlots;
    }
}