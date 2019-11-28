using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Output
{
    public interface ISink
    {
        void WriteRegion(
            CharInfo[,] buffer,
            short left,
            short top);
    }
}
