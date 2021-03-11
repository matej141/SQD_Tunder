using System.Collections.Generic;
using Data.MOF;
using Data.DI;

namespace DG
{
    public abstract class DgElement : DiagramSelection
    {
        public readonly List<MofElement> mofElement = new List<MofElement>();
        public readonly List<DiElement> diElement = new List<DiElement>();
    }
}