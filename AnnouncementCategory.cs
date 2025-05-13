using System;
using System.Collections.Generic;

namespace AnnouncementBoard;

public partial class AnnouncementCategory
{
    public string Category { get; set; } = null!;

    public virtual ICollection<AnnouncementSubCategory> AnnouncementSubCategories { get; set; } = new List<AnnouncementSubCategory>();

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
}
