namespace InfernalInkSteelSuite.Domain.Sync
{
    /// <summary>
    /// Defines the type of sync operation. Replaces magic strings "Create"/"Update"/"Delete".
    /// </summary>
    public enum SyncOperation
    {
        Create,
        Update,
        Delete
    }
}
