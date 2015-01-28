using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace NET4.TestClasses
{
    [Serializable]
    [DataContract(Name = "CoolCollection")]
    public class Containers : IEnumerable<Container>
    {

        [DataMember(Name = "CoolContainers")]
        private IList<Container> _containers;

        public Containers()
        {
            _containers = new List<Container>();
        }

        public IEnumerator<Container> GetEnumerator()
        {
            return _containers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_containers).GetEnumerator();
        }

        public void Add(Container item)
        {
            _containers.Add(item);
        }

        public void Clear()
        {
            _containers.Clear();
        }

        public bool Contains(Container item)
        {
            return _containers.Contains(item);
        }

        public void CopyTo(Container[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Container item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _containers.Count; }
        }

        public bool IsReadOnly
        {
            get { return _containers.IsReadOnly; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Container container in _containers)
            {
                sb.AppendFormat("Name : {0}, Width : {1}\n", container.Name, container.Width);
            }

            return sb.ToString();
        }
    }

    [Serializable]
    [DataContract(Name = "CoolContainer")]
    public class Container
    {
        [DataMember(Name = "CoolName")]
        public string Name { get; set; }

        [DataMember(Name = "CoolWidth")]
        public int Width { get; set; }
    }
}