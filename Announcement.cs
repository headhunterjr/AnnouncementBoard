using System;
using System.Collections.Generic;

namespace AnnouncementBoard;

public partial class Announcement
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly CreatedDate { get; set; }

    public bool Status { get; set; }

    public string Category { get; set; } = null!;

    public string SubCategory { get; set; } = null!;

    public virtual AnnouncementCategory CategoryNavigation { get; set; } = null!;

    public virtual AnnouncementSubCategory SubCategoryNavigation { get; set; } = null!;
}
