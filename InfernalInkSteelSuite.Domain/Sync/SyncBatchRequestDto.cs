using System.Collections.Generic;

namespace InfernalInkSteelSuite.Domain.Sync
{
    public class SyncBatchRequestDto<T>
    {
        public List<SyncChangeDto<T>> Changes { get; set; } = new();
    }
}
