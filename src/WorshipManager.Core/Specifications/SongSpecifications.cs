using WorshipManager.Core.Entities;

namespace WorshipManager.Core.Specifications;

public class ActiveSongsSpec : BaseSpecification<Song>
{
    public ActiveSongsSpec(int organizationId)
        : base(s => s.OrganizationId == organizationId && s.IsActive)
    {
        ApplyOrderBy(s => s.Title);
    }
}

public class SongsByCategorySpec : BaseSpecification<Song>
{
    public SongsByCategorySpec(int organizationId, string category)
        : base(s => s.OrganizationId == organizationId && s.Category == category && s.IsActive)
    {
        ApplyOrderBy(s => s.Title);
    }
}

public class SongSearchSpec : BaseSpecification<Song>
{
    public SongSearchSpec(int organizationId, string searchTerm)
        : base(s => s.OrganizationId == organizationId && s.IsActive &&
            (s.Title.Contains(searchTerm) || (s.Artist != null && s.Artist.Contains(searchTerm))))
    {
        ApplyOrderBy(s => s.Title);
    }
}
