using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitySweeper
{
    [Serializable]
    public class CollectionData
    {
        public string fileGuid;
        public string fileName;
        public List<string> referenceGuids = new();
        public DateTime timeStamp;
    }

    [Serializable]
    public class TypeDate
    {
        public string guid;
        public string fileName;
        public List<string> typeFullName = new();
        public string assembly;
        public DateTime timeStamp;

        public Type[] types
        {
            get { return typeFullName.Select(c => Type.GetType(c)).ToArray(); }
        }

        public void Add(Type addType)
        {
            assembly = addType.Assembly.FullName;
            var typeName = addType.FullName;
            if (typeFullName.Contains(typeName) == false) typeFullName.Add(typeName);
        }
    }

    public interface IReferenceCollection
    {
        void CollectionFiles();
        void Init(List<CollectionData> refs);
    }
}