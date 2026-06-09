using System;
using System.Collections.Generic;
using System.Text;

namespace Umbraco.Community.LinkedPages;

public class LinkedPagesConfig
{
    public string? RelationType { get; set; }

    public bool ShowType { get; set; } = true;

    public string IgnoredTypes { get; set; } = "umbMedia,umbDocument,umbMember";
}
