using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface ISpellWithCustomFeatures
    {
        List<object> CustomFeatures { get; }

        public IEnumerable<T> GetTypedFeatures<T>() where T: class;
    }
}
