namespace PetAdoption
{
    /// <summary>
    /// Enum for types of pets
    /// </summary>
    public enum PetType
    {
        unknown, // Important this is first, since that gets used as the default, in case of failure to parse enum
        dog,
        cat,
        rabbit,
        bird
    }
}
