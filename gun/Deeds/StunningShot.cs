using BlueprintCore.Utils;
using gun.Firearms;
using Kingmaker.EntitySystem.Stats;
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
    internal class StunningShot : AbstractWeaponTrigger, IInitiatorRulebookHandler<RuleAttackWithWeapon>, IRulebookHandler<RuleAttackWithWeapon>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public void OnEventAboutToTrigger(RuleAttackWithWeapon evt)
        {
           
        }

        public void OnEventDidTrigger(RuleAttackWithWeapon evt)
        {
            if (evt.AttackRoll.IsHit &&//if we hit the target
                evt.Weapon.Blueprint.Category == BaseFirearm.FirearmCategory && //and the weapon is a firearm
                evt.Initiator.Resources.HasEnoughResource(Grit.GritResource, 2) &&//and have enough grit
                evt.Target.Buffs.GetBuff(BlueprintTool.Get<BlueprintBuff>("09d39b38bb7c6014394b6daced9bacd3")) == null//and the target is not already stunned
                )
            {
                int DC = 10;
                DC += evt.Initiator.Stats.Wisdom.Bonus;
                DC += evt.Initiator.Progression.CharacterLevel/2;//DC is 10 + wis +1/2 level
                RuleSavingThrow StunSave = new RuleSavingThrow(evt.Target,SavingThrowType.Fortitude, DC);
                Context.TriggerRule(StunSave);
                //RulebookEventBus.Subscribe(StunSave);//still not apearing in the combat log

                evt.Initiator.Resources.Spend(Grit.GritResource, 2);
                if (!StunSave.IsPassed)//if the target fails the save
                {
                    evt.Target.AddBuff(BlueprintTool.Get<BlueprintBuff>("09d39b38bb7c6014394b6daced9bacd3"), Context, TimeSpan.FromSeconds(6));//stunned for 1 round 6 seconds)
                }

            }
        }
    }
}
