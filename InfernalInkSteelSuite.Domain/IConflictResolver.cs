namespace InfernalInkSteelSuite.Domain
{
    public interface IConflictResolver<T>
    {
        T Resolve(T local, T server);
    }
}
