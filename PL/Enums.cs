using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL;

    internal class VolunteerSortOptionCollection : IEnumerable
    {
        static readonly IEnumerable<BO.VolunteerSortOption> s_enums =
    (Enum.GetValues(typeof(BO.VolunteerSortOption)) as IEnumerable<BO.VolunteerSortOption>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

