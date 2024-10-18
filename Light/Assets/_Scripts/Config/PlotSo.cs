using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "情节", menuName = "配置/情节")]
public class PlotSo : PlotSoBase
{
    [ValueDropdown(nameof(GetPlotNames), IsUniqueList = true,
                   DrawDropdownForListElements = false,
                   ExcludeExistingValuesInList = true),
     HideIf(nameof(isStoryFinalize), optionalValue: true), 
     LabelText("下个情节")]
    public string[] nextPlots;
    public override string[] NextPlots() => nextPlots;
}