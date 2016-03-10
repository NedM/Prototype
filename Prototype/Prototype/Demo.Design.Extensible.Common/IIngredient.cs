using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Demo.Design.Extensible.Common
{
    public interface IIngredient
    {
        string Name { get; }

        string Category { get; }

        decimal Quantity { get; }

        string Units { get; }
    }
}
