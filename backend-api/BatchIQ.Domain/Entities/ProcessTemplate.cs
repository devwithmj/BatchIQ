using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities;

/// <summary>
/// Template for standardizing production processes
/// Example: "Pistachio Roasting" template with expected yields
/// </summary>
public class ProcessTemplate
{
    public int Id { get; set; }

    /// <summary>
    /// Name of the process template
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Description of the process
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Type of process this template represents
    /// </summary>
    public ProcessType ProcessType { get; set; }

    /// <summary>
    /// Whether this template is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Expected overall yield percentage
    /// </summary>
    public decimal? ExpectedYieldPercentage { get; set; }

    /// <summary>
    /// Standard processing time in minutes
    /// </summary>
    public int? StandardProcessingTimeMinutes { get; set; }

    /// <summary>
    /// Expected input materials for this process
    /// </summary>
    public ICollection<ProcessTemplateInput> ExpectedInputs { get; set; } = new List<ProcessTemplateInput>();

    /// <summary>
    /// Expected output products from this process
    /// </summary>
    public ICollection<ProcessTemplateOutput> ExpectedOutputs { get; set; } = new List<ProcessTemplateOutput>();

    /// <summary>
    /// Process instructions
    /// </summary>
    public string? Instructions { get; set; }

    /// <summary>
    /// Quality control checkpoints
    /// </summary>
    public string? QualityCheckpoints { get; set; }

    /// <summary>
    /// When this template was created
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this template was last updated
    /// </summary>
    public DateTime? LastUpdatedDate { get; set; }
}

/// <summary>
/// Expected input for a process template
/// </summary>
public class ProcessTemplateInput
{
    public int Id { get; set; }
    public int ProcessTemplateId { get; set; }
    public ProcessTemplate ProcessTemplate { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Expected quantity per batch (can be variable)
    /// </summary>
    public decimal? StandardQuantity { get; set; }
    public SizeUnit? StandardUnit { get; set; }

    /// <summary>
    /// Whether this input is required
    /// </summary>
    public bool IsRequired { get; set; } = true;

    public string? Notes { get; set; }
}

/// <summary>
/// Expected output for a process template
/// </summary>
public class ProcessTemplateOutput
{
    public int Id { get; set; }
    public int ProcessTemplateId { get; set; }
    public ProcessTemplate ProcessTemplate { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Expected yield percentage for this output
    /// </summary>
    public decimal ExpectedYieldPercentage { get; set; }

    /// <summary>
    /// Priority order for this output
    /// </summary>
    public int? Priority { get; set; }

    public string? Notes { get; set; }
}
