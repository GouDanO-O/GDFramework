using System;
using System.Collections.Generic;

namespace GDFramework_Core.GAS.RunningTime.AttributeSet
{
    public static class AttributeSetUtility
    {
        public static Dictionary<Type, string> AttrSetNameCacheDict { get; private set; }
        
        public static void Cache(Dictionary<Type,string> typeToName)
        {
            AttrSetNameCacheDict = typeToName;
        }

        public static string AttributeSetName(Type attrSetType)
        {
            if(AttrSetNameCacheDict==null)
                return attrSetType.Name;
            
            return AttrSetNameCacheDict.TryGetValue(attrSetType, out var value) ? value : attrSetType.Name;
        }
    }
}