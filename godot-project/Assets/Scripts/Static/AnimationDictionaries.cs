using System.Collections.Generic;
using Tater.Scripts.Static;

namespace Tater.Scripts.Static;

public class AnimationDictionaries
{
    public static readonly Dictionary<WizardAnimation, string> ParseWizardAnimation = new Dictionary<WizardAnimation, string>()
    {
        { WizardAnimation.Idle, "Wizard_Armature|Idle_001" },
        { WizardAnimation.Moving, "Wizard_Armature|moving" },
        { WizardAnimation.Casting, "Wizard_Armature|casting" }
    };
    
    public static readonly Dictionary<EnemyAnimation, string> ParseEnemyAnimation = new Dictionary<EnemyAnimation, string>()
    {
        { EnemyAnimation.Walk, "Walk" },
        { EnemyAnimation.Death, "Death" },
    };
}