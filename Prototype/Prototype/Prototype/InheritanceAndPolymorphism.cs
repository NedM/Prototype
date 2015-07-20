
using System.Collections.Generic;
using System.Diagnostics;

namespace Prototype
{
    public class BaseFoo
    {
        protected string Name;

        public BaseFoo() : this("BaseFoo") { }

        public BaseFoo(string name)
        {
            Debug.WriteLine("Hello from BaseFoo Constructor! {0}", this);
            Name = name;
            Debug.WriteLine("Goodbye from BaseFoo Constructor! {0}", this);
        }

        public override string ToString()
        {
            return string.Format("My name is {0}", string.IsNullOrEmpty(Name) ? "[NOT SET]" : Name);
        }
    }

    public class SubFoo : BaseFoo
    {
        public SubFoo() : this("NONE")
        {
        }

        public SubFoo(string name) : base(name)
        {
            Debug.WriteLine("Hello from SubFoo Constructor! {0}", this);
            Name = "SubFoo";
            Debug.WriteLine("Goodbye from SubFoo Constructor! {0}", this);
        }
    }

    public class BaseFooCollection : ICollection<BaseFoo>
    {
        public void Add(BaseFoo item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(BaseFoo item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(BaseFoo[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public int Count
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool Remove(BaseFoo item)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator<BaseFoo> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }

    public class SubFooCollection : BaseFooCollection, ICollection<SubFoo>
    {
        public void Add(SubFoo item)
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(SubFoo item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(SubFoo[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(SubFoo item)
        {
            throw new System.NotImplementedException();
        }

        public new IEnumerator<SubFoo> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
