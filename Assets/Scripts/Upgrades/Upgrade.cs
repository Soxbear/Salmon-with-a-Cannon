using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Upgrades
{
    [CreateAssetMenu(fileName = "New Player Upgrade", menuName = "Upgrades/Player Upgrade", order = 0)]
    public class PlayerUpgrade : ScriptableObject
    {
        public enum Stat
        {
            Health,
            MaxSpeed,
            Acceleration,
            Regeneration,
            RegenerationDelay
        }

        [Serializable]
        public struct StatEffect
        {
            public Stat stat;
            public bool mult;
            public float value;

            public StatEffect(Stat _stat = Stat.Health, bool _mult = false, float _value = 1f)
            {
                stat = _stat;
                mult = _mult;
                value = _value;
            }
        }

        public List<StatEffect> statEffects = new List<StatEffect>();

        public string behaviorName = "Base";

#if UNITY_EDITOR

        [HideInInspector]
        public string behaviorPath;

        private string objectName;

        void Awake()
        {
            objectName = this.name;

            behaviorPath = Application.dataPath + "/Scripts/Upgrades/PlayerUpgrades/" + this.name + ".cs";

            behaviorName = this.name.Replace(" ", string.Empty);

            FileUtil.CopyFileOrDirectory(Application.dataPath + "/Scripts/Upgrades/PlayerUpgrades/Base.cs", behaviorPath);

            File.WriteAllText(behaviorPath,
                File.ReadAllText(Application.dataPath + "/Scripts/Upgrades/PlayerUpgrades/" + this.name + ".cs")
                .Replace("public class Base : PlayerUpgradeBehavior", "public class " + behaviorName + " : PlayerUpgradeBehavior")
                );
        }

        void OnValidate()
        {
            if (objectName != this.name)
            {

            }
        }

        void RenameBehavior()
        {

        }
#endif
    }

    public abstract class PlayerUpgradeBehavior : MonoBehaviour
    {
        public virtual void Activate() { }

        public virtual void Deactivate() { }
    }

    [CustomEditor(typeof(PlayerUpgrade))]
    public class UpgradeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            
            base.OnInspectorGUI();
        }

        //public override VisualElement CreateInspectorGUI()
        //{
        //    VisualElement customInspector = new VisualElement();

        //    ListView statListView = new ListView((target as PlayerUpgrade).statEffects)
        //    {
        //        selectionType = SelectionType.Single,
        //        showAddRemoveFooter = true,
        //        reorderable = true,
        //        reorderMode = ListViewReorderMode.Animated,
        //        makeItem = () =>
        //        {
        //            VisualElement rootElement = new VisualElement();

        //            rootElement.style.flexDirection = FlexDirection.Row;

        //            rootElement.Add(new DropdownField(Enum.GetNames(typeof(PlayerUpgrade.Stat)).ToList(), 0));

        //            rootElement.Add(new DropdownField(new List<string> { "Add", "Multiply" }, 0));

        //            FloatField valueField = new FloatField();
        //            valueField.value = 1;
        //            rootElement.Add(new FloatField());

        //            return rootElement;
        //        },
        //        bindItem = (element, index) =>
        //        {
        //            (element.Children().ElementAt(0) as DropdownField).index = (int)(target as PlayerUpgrade).statEffects[index].stat;
        //            (element.Children().ElementAt(0) as DropdownField).UnregisterAllRemovableCallbacks();
        //            (element.Children().ElementAt(0) as DropdownField).RegisterValueChangedCallback((evt) =>
        //            {
        //                PlayerUpgrade.StatEffect effect = (target as PlayerUpgrade).statEffects[index];
        //                effect.stat = Enum.Parse<PlayerUpgrade.Stat>(evt.newValue);
        //                (target as PlayerUpgrade).statEffects[index] = effect;
        //            });
        //            (element.Children().ElementAt(1) as DropdownField).index = (target as PlayerUpgrade).statEffects[index].mult ? 1 : 0;
        //            (element.Children().ElementAt(1) as DropdownField).UnregisterAllRemovableCallbacks();
        //            (element.Children().ElementAt(1) as DropdownField).RegisterValueChangedCallback((evt) =>
        //            {
        //                Debug.Log(index);
        //                PlayerUpgrade.StatEffect effect = (target as PlayerUpgrade).statEffects[index];
        //                effect.mult = (evt.newValue == "Multiply");
        //                (target as PlayerUpgrade).statEffects[index] = effect;
        //            });
        //            (element.Children().ElementAt(2) as FloatField).value = (target as PlayerUpgrade).statEffects[index].value;
        //            (element.Children().ElementAt(2) as FloatField).UnregisterAllRemovableCallbacks();
        //            (element.Children().ElementAt(2) as FloatField).RegisterValueChangedCallback((evt) =>
        //            {
        //                PlayerUpgrade.StatEffect effect = (target as PlayerUpgrade).statEffects[index];
        //                effect.value = evt.newValue;
        //                (target as PlayerUpgrade).statEffects[index] = effect;
        //            });
        //        },
        //        onAdd = (listView) =>
        //        {
        //            (target as PlayerUpgrade).statEffects.Add(new PlayerUpgrade.StatEffect());
        //            listView.RefreshItems();
        //        },
        //        onRemove = (listView) =>
        //        {
        //            (target as PlayerUpgrade).statEffects.RemoveAt((listView.selectedIndex >= (target as PlayerUpgrade).statEffects.Count) ? ((target as PlayerUpgrade).statEffects.Count - 1) : listView.selectedIndex);
        //            listView.RefreshItems();
        //        },
        //    };

        //    customInspector.Add(statListView);

        //    return customInspector;
        //}
    }
}
