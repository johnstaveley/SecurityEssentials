// ReSharper disable All
namespace SecurityEssentials.Core.Identity
{
    public enum HashStrategyKind
    {
        PBKDF2_5009Iterations = 0,
        PBKDF2_8000Iterations = 1,
        Argon2_48kWorkCost = 2
    }
}