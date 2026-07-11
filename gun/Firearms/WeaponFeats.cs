using BepInEx.AssemblyPublicizer;
using global::Kingmaker.Blueprints;
using global::Kingmaker.Blueprints.Facts;
using global::Kingmaker.Blueprints.Items.Weapons;
using global::Kingmaker.Blueprints.JsonSystem;
using global::Kingmaker.Enums;
using global::Kingmaker.PubSubSystem;
using global::Kingmaker.RuleSystem.Rules;
using global::Kingmaker.UnitLogic;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Enums;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.UnitLogic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace gun.Firearms
{
    public class WeaponFocusMythic : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateAttackBonusWithoutTarget>, IRulebookHandler<RuleCalculateAttackBonusWithoutTarget>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public BlueprintWeaponTypeReference m_WeaponType;

        public ModifierDescriptor Descriptor = ModifierDescriptor.UntypedStackable;
        public BlueprintFeatureReference GreaterFocus;

        public BlueprintWeaponType WeaponType => m_WeaponType?.Get();

        public WeaponFocusMythic(BlueprintWeaponTypeReference WeaponType, BlueprintFeatureReference greaterfocus) 
        {
            GreaterFocus = greaterfocus;
            m_WeaponType = WeaponType;
        }

        public void OnEventAboutToTrigger(RuleCalculateAttackBonusWithoutTarget evt)
        {
            if (evt.Weapon != null && evt.Weapon.Blueprint.Type == WeaponType)
            {
                int bonus = 1;
                if (evt.Initiator.GetFeature(GreaterFocus) != null) //if the user has greater weapon focus increase the bonus to 2
                {
                    bonus = 2;
                }
                evt.AddModifier(bonus, base.Fact, Descriptor);
            }
        }

        public void OnEventDidTrigger(RuleCalculateAttackBonusWithoutTarget evt)
        {
        }
    }

    public class WeaponTypeDamageBonusMythic : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateWeaponStats>, IRulebookHandler<RuleCalculateWeaponStats>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public BlueprintWeaponTypeReference m_WeaponType;

        public BlueprintWeaponType WeaponType => m_WeaponType?.Get();

        public WeaponTypeDamageBonusMythic(BlueprintWeaponTypeReference Weapon)
        {
            m_WeaponType = Weapon;
        }

        public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
        {
            int DamageBonus = 0;
            DamageBonus = evt.Initiator.Progression.MythicLevel / 2;
            if (evt.Weapon.Blueprint.Type == WeaponType)
            {
                evt.AddDamageModifier(DamageBonus * base.Fact.GetRank(), base.Fact);
            }
        }

        public void OnEventDidTrigger(RuleCalculateWeaponStats evt)
        {
        }
    }

    public class ImprovedCriticalMythic : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateWeaponStats>, IRulebookHandler<RuleCalculateWeaponStats>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public BlueprintWeaponTypeReference m_WeaponType;

        public BlueprintWeaponType WeaponType => m_WeaponType?.Get();

        public ImprovedCriticalMythic(BlueprintWeaponTypeReference Weapon)
        {
            m_WeaponType = Weapon;
        }

        public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
        {
            if (evt.Weapon.Blueprint.Type == WeaponType)
            {
                evt.AdditionalCriticalMultiplier.Add(new Modifier(1, base.Fact, ModifierDescriptor.UntypedStackable));
            }
        }

        public void OnEventDidTrigger(RuleCalculateWeaponStats evt)
        {
        }
    }

    public class ImprovedImprovedCritical : UnitFactComponentDelegate, IInitiatorRulebookHandler<RuleCalculateWeaponStats>, IRulebookHandler<RuleCalculateWeaponStats>, ISubscriber, IInitiatorRulebookSubscriber
    {
        public BlueprintWeaponTypeReference m_WeaponType;

        public BlueprintWeaponType WeaponType => m_WeaponType?.Get();

        public ImprovedImprovedCritical(BlueprintWeaponTypeReference Weapon)
        {
            m_WeaponType = Weapon;
        }

        public void OnEventAboutToTrigger(RuleCalculateWeaponStats evt)
        {
            if (evt.Weapon.Blueprint.Type == WeaponType)
            {
                evt.CriticalEdgeBonus++;
            }
        }

        public void OnEventDidTrigger(RuleCalculateWeaponStats evt)
        {
        }
    }
}
