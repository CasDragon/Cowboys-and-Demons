using BlueprintCore.Utils;
using gun.Firearms;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Kingdom.Blueprints;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gun.Deeds
{
    internal class BleedingShot : AbstractWeaponTrigger, IInitiatorRulebookHandler<RuleAttackWithWeapon>, IRulebookHandler<RuleAttackWithWeapon>, ISubscriber, IInitiatorRulebookSubscriber
    {
        private int type = 0;
        private int cost = 1;
        static string[] BleedingGUIDs = {DeedConfigurator.BasicBleedBuffGUID, DeedConfigurator.STRBleedBuffGUID, DeedConfigurator.DEXBleedBuffGUID,DeedConfigurator.CONBleedBuffGUID };
        public BleedingShot (int type)
        {
            this.type = type;
            if (type == 0) 
            {
                cost = 1;
            }
            else
            {
                cost = 2;
            }
        }

        public void OnEventAboutToTrigger(RuleAttackWithWeapon evt)
        {

        }

        public void OnEventDidTrigger(RuleAttackWithWeapon evt)
        {
            if (evt.AttackRoll.IsHit &&//if we hit the target
                evt.Weapon.Blueprint.Category == BaseFirearm.FirearmCategory && //and the weapon is a firearm
                evt.Initiator.Resources.HasEnoughResource(Grit.GritResource, cost) &&//and have enough grit
                evt.Target.Buffs.GetBuff(BlueprintTool.Get<BlueprintBuff>(BleedingGUIDs[type])) == null//and the target does not already have the selected bleed type
                )
            {
                evt.Initiator.Resources.Spend(Grit.GritResource, cost);
                evt.Target.AddBuff(BlueprintTool.Get<BlueprintBuff>(BleedingGUIDs[type]), Context);//apply bleeding

            }
        }
    }
}
