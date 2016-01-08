using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace UselessAttributeStripper
{
    public class AttributeStripper
    {
        private readonly string[] _attributeNames;
        private readonly Dictionary<string, int> _stripCountMap;
        private readonly Dictionary<string, int> _stripTotalCountMap;

        public Dictionary<string, int> StripCountMap { get { return _stripCountMap; } }
        public Dictionary<string, int> StripTotalCountMap { get { return _stripTotalCountMap; } }

        public AttributeStripper(string[] attributeNames)
        {
            _attributeNames = attributeNames;
            _stripCountMap = new Dictionary<string, int>();
            _stripTotalCountMap = new Dictionary<string, int>();
        }

        public void ProcessDll(string dllPath)
        {
            _stripCountMap.Clear();

            AssemblyDefinition assemblyDef;

            using (var assemblyStream = new MemoryStream(File.ReadAllBytes(dllPath)))
            {
                assemblyDef = AssemblyDefinition.ReadAssembly(assemblyStream);
            }

            ProcessAssembly(new[] { assemblyDef });

            using (var assemblyStream = File.Create(dllPath))
            {
                assemblyDef.Write(assemblyStream);
            }
        }

        private void ProcessAssembly(AssemblyDefinition[] assemblyDefs)
        {
            foreach (var assemblyDef in assemblyDefs)
            {
                foreach (var moduleDef in assemblyDef.Modules)
                {
                    foreach (var type in moduleDef.Types)
                        RemoveAttributes(type);
                }
            }
        }

        private void RemoveAttributes(TypeDefinition typeDef)
        {
            RemoveAttributes(typeDef.FullName, typeDef.CustomAttributes);

            foreach (var field in typeDef.Fields)
                RemoveAttributes(field.Name, field.CustomAttributes);

            foreach (var property in typeDef.Properties)
                RemoveAttributes(property.Name, property.CustomAttributes);

            foreach (var method in typeDef.Methods)
                RemoveAttributes(method.Name, method.CustomAttributes);

            foreach (var type in typeDef.NestedTypes)
                RemoveAttributes(type);
        }

        private void RemoveAttributes(string ownerName, Collection<CustomAttribute> customAttributes)
        {
            foreach (var attrName in _attributeNames)
            {
                var index = -1;
                for (var i = 0; i < customAttributes.Count; i++)
                {
                    var attr = customAttributes[i];
                    if (attr.Constructor != null && attr.Constructor.DeclaringType.FullName == attrName)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    customAttributes.RemoveAt(index);

                    var count = 0;
                    _stripCountMap.TryGetValue(attrName, out count);
                    _stripCountMap[attrName] = count + 1;

                    var totalCount = 0;
                    _stripTotalCountMap.TryGetValue(attrName, out totalCount);
                    _stripTotalCountMap[attrName] = totalCount + 1;
                }
            }
        }
    }
}
