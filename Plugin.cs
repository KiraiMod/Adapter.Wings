using BepInEx;
using BepInEx.IL2CPP;
using KiraiMod.Core.UI;
using System;
using System.Collections.Generic;
using WingAPI;

namespace KiraiMod.Adapter.Wings
{
    [BepInPlugin("me.KiraiHooks.KiraiMod.Adapter.Wings", "Adapter.Wings", "0.1.0")]
    [BepInDependency(WingAPI.Plugin.GUID)]
    [BepInDependency(Core.UI.Plugin.GUID)]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            Controller.WingInit += wing =>
            {
                Dictionary<string, WingPage> highPages = new();

                 Action<UIGroup> HandleHighGroup = group => HandleGroup(wing.CreatePage(group.name), group);

                UIManager.HighGroupAdded += HandleHighGroup;
                UIManager.HighGroups.ForEach(HandleHighGroup);
            };
        }

        private static void HandleGroup(WingPage page, UIGroup group)
        {
            int pageCounter = 0;

            Action<UIElement> HandleElement = element =>
            {
                if (!element.GetType().IsGenericType)
                    page.CreateButton(element.name, pageCounter++, () => element.Invoke());
                else if (element is UIElement<bool> boolElem)
                {
                    WingToggle toggle = page.CreateToggle(element.name, pageCounter++, boolElem.Bound._value, value => boolElem.Bound.Value = value);
                    boolElem.Changed += value => toggle.State = value;
                } 
                else if (element is UIElement<UIGroup> groupElem)
                {
                    WingPage subpage = page.CreateNestedPage(element.name, pageCounter++);
                    HandleGroup(subpage, groupElem.Bound._value);
                }
            };

            group.ElementAdded += HandleElement;
            group.elements.ForEach(HandleElement);
        }
    }
}
