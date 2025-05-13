using System;
using System.Collections.Generic;

namespace AnnouncementBoard;

public partial class AnnouncementSubCategory
{
    public string SubCategory { get; set; } = null!;

    public string Category { get; set; } = null!;

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    public virtual AnnouncementCategory CategoryNavigation { get; set; } = null!;
}
