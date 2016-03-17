using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using Ninject.Injection;

namespace Demo.Design.Extensible
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Dependency Injection Frameworks Setup
            
            //Unity: http://unity.codeplex.com/
            IUnityContainer container = new UnityContainer();

            //Ninject: http://www.ninject.org/
            //ConstructorInjector ninjector = new ConstructorInjector();

            //Managed Extensibility Framework (MEF): https://mef.codeplex.com/ | https://msdn.microsoft.com/en-us/library/dd460648(v=vs.110).aspx
            System.ComponentModel.Composition.Hosting.AggregateCatalog catalog =
                new System.ComponentModel.Composition.Hosting.AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(string).Assembly));

            #endregion Dependency Injection Frameworks Setup

            //Kitchen!
            //Given a collection of orders/recipes, need to 

            //PREP
            // chop, skin/peel, slice, mash, chunk, dice, julienne, etc

            //HEAT
            // boil, saute, simmer, bake, broil, toast, flambe, roast, char, singe, fry, etc

            //COOL
            // freeze, chill, ice, etc

            //PLATE
            // bowl, plate, board, etc

            //FINISH
        }
    }
}
