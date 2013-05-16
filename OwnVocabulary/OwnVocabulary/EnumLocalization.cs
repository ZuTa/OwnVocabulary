using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace OwnVocabulary
{
    public static class EnumLocalization
    {
        public static string GetStringValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo info = type.GetField(value.ToString());

            LanguageValueAttribute[] attr = (LanguageValueAttribute[])info.GetCustomAttributes(typeof(LanguageValueAttribute), false);

            return attr.Length > 0 ? attr[0].StringValue : null;
        }
    }
}
