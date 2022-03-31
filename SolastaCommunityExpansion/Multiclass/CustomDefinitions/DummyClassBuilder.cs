using System;
using SolastaModApi;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaMulticlass.CustomDefinitions
{
    internal sealed class DummyClassBuilder : BaseDefinitionBuilder<CharacterClassDefinition>
    {
        private const string NoContent = "Feature/&NoContentTitle";
        private const string DummyClassName = "DummyClass";
        private const string DummyClassGuid = "062d696ab44146e0b316188f943d8079";

        [Obsolete]
        private DummyClassBuilder(string name, string guid) : base(Rogue, name, guid)
        {
            Definition.GuiPresentation.Title = NoContent;
            Definition.GuiPresentation.Description = NoContent;
        }

        [Obsolete]
        private static CharacterClassDefinition CreateAndAddToDB(string name, string guid)
        {
            return new DummyClassBuilder(name, guid).AddToDB();
        }

        [Obsolete]
        internal static readonly CharacterClassDefinition DummyClass =
            CreateAndAddToDB(DummyClassName, DummyClassGuid);
    }
}
