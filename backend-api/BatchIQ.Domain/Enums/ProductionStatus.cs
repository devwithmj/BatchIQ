using System;

namespace BatchIQ.Domain.Enums;

public enum ProductionStatus
{
    Planned = 1,        // Production is planned but not started
    InProgress = 2,     // Production is currently underway
    Completed = 3,      // Production completed successfully
    OnHold = 4,         // Production temporarily stopped
    Cancelled = 5,      // Production was cancelled
    QualityFailed = 6   // Production completed but failed quality control
}
