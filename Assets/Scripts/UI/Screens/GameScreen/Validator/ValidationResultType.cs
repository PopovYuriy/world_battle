namespace UI.Screens.GameScreen.Validator
{
    public enum ValidationResultType : byte
    {
        None = 0,
        Valid = 1,
        NotFoundInVocabulary = 2,
        AlreadyUsed = 3
    }
}