using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Interfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class PowerBundleContext
{
    internal const string UseCustomRestPowerFunctorName = "UseCustomRestPower";

    internal static void Load()
    {
        ServiceRepository.GetService<IFunctorService>()
            .RegisterFunctor(UseCustomRestPowerFunctorName, new FunctorUseCustomRestPower());
    }

    private sealed class FunctorUseCustomRestPower : Functor
    {
        private bool _powerUsed;

        public override IEnumerator Execute(
            [NotNull] FunctorParametersDescription functorParameters,
            FunctorExecutionContext context)
        {
            var functor = this;
            var powerName = functorParameters.StringParameter;
            var power = PowerBundle.GetPower(powerName);

            if (!power && !DatabaseHelper.TryGetDefinition(powerName, out power))
            {
                yield break;
            }

            var ruleChar = functorParameters.RestingHero;
            var usablePower = PowerProvider.Get(power, ruleChar);

            if (power.EffectDescription.TargetType == TargetType.Self)
            {
                GameLocationCharacter fromActor = null;
                var retarget = power.GetFirstSubFeatureOfType<IRetargetCustomRestPower>();
                if (retarget != null)
                {
                    fromActor = retarget.GetTarget(ruleChar);
                }

                fromActor ??= GameLocationCharacter.GetFromActor(ruleChar);

                var actionService = ServiceRepository.GetService<IGameLocationActionService>();
                var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();

                if (fromActor != null)
                {
                    functor._powerUsed = false;

                    var actionParams = new CharacterActionParams(fromActor, ActionDefinitions.Id.PowerNoCost)
                    {
                        ActionModifiers = { new ActionModifier() },
                        RulesetEffect = implementationService
                            .InstantiateEffectPower(fromActor.RulesetCharacter, usablePower, true),
                        UsablePower = usablePower,
                        TargetCharacters = { fromActor },
                        SkipAnimationsAndVFX = true
                    };

                    actionService.ExecuteAction(actionParams, functor.ActionExecuted, false);

                    while (!functor._powerUsed)
                    {
                        yield return null;
                    }
                }
                else
                {
                    var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();

                    formsParams.FillSourceAndTarget(ruleChar, ruleChar);
                    formsParams.FillFromActiveEffect(implementationService
                        .InstantiateEffectPower(ruleChar, usablePower, false));
                    formsParams.effectSourceType = EffectSourceType.Power;

                    ruleChar.UsePower(usablePower);
                    ruleChar.LogCharacterUsedPower(power, $"Feedback/&{power.Name}UsedWhileTravellingFormat");
                }
            }

            Trace.LogWarning("Unable to assign targets to power");
        }

        private void ActionExecuted(CharacterAction action)
        {
            _powerUsed = true;
        }
    }
}
