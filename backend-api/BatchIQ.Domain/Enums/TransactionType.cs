using System;

namespace BatchIQ.Domain.Enums;

public enum TransactionType
{
    Receive = 1,    // into system (purchase, production)
    Issue   = 2,    // out of system (sale, disposal)
    Transfer = 3,   // WH → Store, Shelf B2 → Shelf C3, etc.
    Adjust   = 4    // manual correction
}