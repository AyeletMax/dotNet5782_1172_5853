using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

internal class VolunteerSortFieldCollection : IEnumerable
{
    static readonly IEnumerable<BO.VolunteerSortField> s_enums =
        (System.Enum.GetValues(typeof(BO.VolunteerSortField)) as IEnumerable<BO.VolunteerSortField>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class CallInListFieldsCollection : IEnumerable
{
    static readonly IEnumerable<BO.CallInListFields> s_enums =
        (System.Enum.GetValues(typeof(BO.CallInListFields)) as IEnumerable<BO.CallInListFields>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class RoleCollection : IEnumerable
{
    static readonly IEnumerable<BO.Role> s_enums =
        (System.Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

internal class DistanceTypeCollection : IEnumerable
{
    static readonly IEnumerable<BO.DistanceType> s_enums =
        (System.Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;
    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
