﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

public class VolunteerSortOptionCollection : IEnumerable
{
    static readonly IEnumerable<BO.VolunteerSortOption> s_enums =
(Enum.GetValues(typeof(BO.VolunteerSortOption)) as IEnumerable<BO.VolunteerSortOption>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class DistanceTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.DistanceType> s_enums =
(Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class RoleCollection : IEnumerable
{
    static readonly IEnumerable<BO.Role> s_enums =
(Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class CallSortOptionCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallInListFields> s_enums =
        (Enum.GetValues(typeof(BO.CallInListFields)) as IEnumerable<BO.CallInListFields>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

public class FilterAndSortByFieldsCollection : IEnumerable
{
    static readonly IEnumerable<BO.FilterAndSortByFields> s_enums =
        (Enum.GetValues(typeof(BO.FilterAndSortByFields)) as IEnumerable<BO.FilterAndSortByFields>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}




