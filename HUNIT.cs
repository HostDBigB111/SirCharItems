using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace SirCharItems
{
    public class HUnit : PassiveItem
    {
        public static void Register()
        {
            string itemName = "H-Unit";


            string resourceName = "SirCharItems/Resources/HUNIT";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<HUnit>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            string shortDesc = "Downloadable RAM";
            string longDesc = "The more Armor, the more Damage. However, taking Damage to one Armor will destroy all your Armor.\n\n" +
                "This H-Unit turns your brain into Data Storage. A Wireless Data Transmitter is implanted into Your Brain. Though, this Technology might have some Side Effects.";

            ItemBuilder.SetupItem(item, shortDesc, longDesc, "host");

            item.quality = PickupObject.ItemQuality.EXCLUDED;
        }
        protected override void Update()
        {
            PlayerController player = base.Owner;
            float armorCount = base.Owner.healthHaver.Armor;
            this.RemoveStat(PlayerStats.StatType.Damage);
            this.AddStat(PlayerStats.StatType.Damage, (armorCount * 0.5f), StatModifier.ModifyMethod.ADDITIVE);
            player.stats.RecalculateStats(player, true, false);


        }
        private void OnDamaged(PlayerController user)
        {
            PlayerController player = base.Owner;
            base.Owner.healthHaver.Armor = 0;

        }
        public override void Pickup(PlayerController player)
        {
            player.OnReceivedDamage += this.OnDamaged;
            base.Pickup(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnReceivedDamage -= this.OnDamaged;
            return base.Drop(player);
        }
        private void AddStat(PlayerStats.StatType statType, float amount, StatModifier.ModifyMethod method = StatModifier.ModifyMethod.ADDITIVE)
        {
            StatModifier statModifier = new StatModifier
            {
                amount = amount,
                statToBoost = statType,
                modifyType = method
            };
            bool flag = this.passiveStatModifiers == null;
            if (flag)
            {
                this.passiveStatModifiers = new StatModifier[]
                {
                    statModifier
                };
            }
            else
            {
                this.passiveStatModifiers = this.passiveStatModifiers.Concat(new StatModifier[]
                {
                    statModifier
                }).ToArray<StatModifier>();
            }
        }
        private void RemoveStat(PlayerStats.StatType statType)
        {
            List<StatModifier> list = new List<StatModifier>();
            for (int i = 0; i < this.passiveStatModifiers.Length; i++)
            {
                bool flag = this.passiveStatModifiers[i].statToBoost != statType;
                if (flag)
                {
                    list.Add(this.passiveStatModifiers[i]);
                }
            }
            this.passiveStatModifiers = list.ToArray();
        }
    }
}