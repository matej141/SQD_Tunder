using System;
using System.Collections.Generic;
using System.Text;

using Data.DI;
using DG;

namespace Data.MOF
{
    public abstract class MofElement : XmiElement
    {
        public readonly List<DiElement> diElement = new List<DiElement>();
        public readonly List<DgElement> dgElement = new List<DgElement>();

    }
}
