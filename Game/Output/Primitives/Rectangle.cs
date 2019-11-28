using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Output.Primitives
{
    public sealed class Rectangle : IDrawable
    {
        private readonly CharInfo[,] pattern;

        public Rectangle(CharInfo[,] pattern)
        {
            this.pattern = pattern;
        }

        public void Draw(ISink sink)
        {
            throw new NotImplementedException();
        }
    }
}
