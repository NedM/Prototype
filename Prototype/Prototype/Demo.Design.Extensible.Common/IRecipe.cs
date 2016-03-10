using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.Design.Extensible.Common
{
    public interface IRecipe
    {
        ICollection<IIngredient> GetIngredients();

        decimal SizeAsPercent { get; set; }
    }
}
