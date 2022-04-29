namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface ICustomOnActionFeature
    {
        public void OnBeforeAction(CharacterAction characterAction);
        public void OnAfterAction(CharacterAction characterAction);
    }
}
