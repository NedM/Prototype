using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.Design.Extensible.Common
{
    public interface IPrepareRecipe
    {
        void Chop(IRecipe recipe);
        void Peel(IRecipe recipe);
        void Heat(IRecipe recipe);
        void Cool(IRecipe recipe);
    }
}
