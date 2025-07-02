using System;

namespace BatchIQ.Domain.Enums;

public enum LocationType
{
    Warehouse,
    Store,
    ExternalCustomer     // For completeness – e.g. wholesale client stocking
}