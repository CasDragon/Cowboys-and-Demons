using BlueprintCore.Utils;
using gun.Firearms;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UI.Models.Log.CombatLog_ThreadSystem.LogThreads.Common;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Visual.Animation.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace gun.Deeds
{
    internal class TargetedShotHead : Kingmaker.UnitLogic.Mechanics.Actions.ContextAction
    {
        public override string GetCaption()
        {
            return "TargetingHead";
        }

        public override void RunAction()
        {
            RuleAttackWithWeapon Attack = new RuleAttackWithWeapon(Context.MaybeCaster, Context.MainTarget.Unit, Context.MaybeCaster.GetFirstWeapon(), 0);
            Context.TriggerRule(Attack);
            if (Attack.AttackRoll.IsHit)
            {
                Context.MainTarget.Unit.AddBuff(BlueprintTool.GetRef<BlueprintBuffReference>("886c7407dc629dc499b9f1465ff382df"), Context, TimeSpan.FromSeconds(6));
            }
        }
    }
    internal class TargetedShotArms : Kingmaker.UnitLogic.Mechanics.Actions.ContextAction
    {
        public override string GetCaption()
        {
            return "TargetingArms";
        }

        public override void RunAction()
        {
            int DC = Context.MainTarget.Unit.Stats.AC.Touch;//firearms resolve against touch
            RuleSkillCheck Disarm = new RuleSkillCheck(Context.MaybeCaster, StatType.BaseAttackBonus, DC);
            Disarm.AddModifier(Context.MaybeCaster.Stats.Dexterity, Kingmaker.Enums.ModifierDescriptor.DexterityBonus);
            //in theory should add other bonuses to attack rolls but not sure how so will leave as is for now
            Context.TriggerRule(Disarm);
            if (Disarm.Success)
            {
                int degrees = Disarm.RollResult - DC;
                degrees /= 5;
                int seconds = (degrees * 6) + 6;//lasts 1 round plus 1 per 5 the attack exceeds AC by
                Context.MainTarget.Unit.AddBuff(BlueprintTool.GetRef<BlueprintBuffReference>("f7db19748af8b69469073485a65f37cf"), Context, TimeSpan.FromSeconds(seconds));
            }
        }
    }

    internal class TargetedShotLegs : Kingmaker.UnitLogic.Mechanics.Actions.ContextAction
    {
        public override string GetCaption()
        {
            return "TargetingLegs";
        }

        public override void RunAction()
        {
            RuleAttackWithWeapon Attack = new RuleAttackWithWeapon(Context.MaybeCaster, Context.MainTarget.Unit, Context.MaybeCaster.GetFirstWeapon(), 0);
            Context.TriggerRule(Attack);
            if (Attack.AttackRoll.IsHit)
            {
                Context.MainTarget.Unit.AddBuff(BlueprintTool.GetRef<BlueprintBuffReference>("24cf3deb078d3df4d92ba24b176bda97"), Context, TimeSpan.FromSeconds(6));
            }
        }
    }
}
