using System.Linq;
using GDFramework_Core.GAS.General;
using GDFramework_Core.GAS.RunningTime.Ability;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;
using GameplayTag = GDFramework_Core.GAS.RunningTime.Tags.GameplayTag;

namespace GDFramework_Core.GAS.RunningTime.Component
{
    /// <summary>
    /// 预设置能力系统组件
    /// </summary>
    [CreateAssetMenu(fileName = "AbilitySystemComponentPreset", menuName = "Gas/AbilitySystemComponentPreset")]
    public class AbilitySystemComponentPreset : ScriptableObject
    {
        private const int WIDTH_LABEL = 70;
        
        private const string ERROR_ABILITY = "能力无法为空!!";
        
        [TitleGroup("Base")]
        [HorizontalGroup("Base/H1", Width = 1 / 3f)]
        [TabGroup("Base/H1/V1", "备注", SdfIconType.InfoSquareFill, TextColor = "#0BFFC5", Order = 1)]
        [HideLabel]
        [MultiLineProperty(10)]
        public string Description;
        
        [TabGroup("Base/H1/V2", "属性集", SdfIconType.PersonLinesFill, TextColor = "#FF7F00", Order = 2)]
        [LabelText(GasTextDefine.ASC_AttributeSet)]
        [LabelWidth(WIDTH_LABEL)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, OnTitleBarGUI = "绘制属性集按钮")]
        [ValueDropdown("@ValueDropdownHelper.AttributeSetChoices", IsUniqueList = true)]
        public string[] AttributeSets;
        
        private void DrawAttributeSetsButtons()
        {
#if UNITY_EDITOR
            if (SirenixEditorGUI.ToolbarButton(SdfIconType.SortAlphaDown))
            {
                AttributeSets = AttributeSets.OrderBy(x => x).ToArray();
            }
#endif
        }
        
        [TabGroup("Base/H1/V3", "Tags", SdfIconType.TagsFill, TextColor = "#45B1FF", Order = 3)]
        [LabelText(GasTextDefine.ASC_BASE_TAG)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, OnTitleBarGUI = "绘制基础Tag按钮")]
        [ValueDropdown("@ValueDropdownHelper.GameplayTagChoices", IsUniqueList = true, HideChildProperties = true)]
        public GameplayTag[] BaseTags;

        private void DrawBaseTagsButtons()
        {
#if UNITY_EDITOR
            if (SirenixEditorGUI.ToolbarButton(SdfIconType.SortAlphaDown))
            {
                BaseTags = BaseTags.OrderBy(x => x.Name).ToArray();
            }
#endif
        }
        
        [HorizontalGroup("Base/H2")]
        [TabGroup("Base/H2/V1", "能力", SdfIconType.YinYang, TextColor = "#D6626E", Order = 1)]
        [LabelText(GasTextDefine.ASC_BASE_ABILITY)]
        [ListDrawerSettings(ShowFoldout = true, ShowItemCount = false, OnTitleBarGUI = "绘制基础技能按钮")]
        [AssetSelector]
        [InfoBox(ERROR_ABILITY, InfoMessageType.Error, VisibleIf = "@IsAbilityNone()")]
        public AbilityAsset[] BaseAbilities;
        
        private void DrawBaseAbilitiesButtons()
        {
#if UNITY_EDITOR
            if (SirenixEditorGUI.ToolbarButton(SdfIconType.SortAlphaDown))
            {
                BaseAbilities = BaseAbilities.OrderBy(x => x.name).ToArray();
            }
#endif
        }

        bool IsAbilityNone()
        {
            return BaseAbilities != null && BaseAbilities.Any(ability => ability == null);
        }
    }
}