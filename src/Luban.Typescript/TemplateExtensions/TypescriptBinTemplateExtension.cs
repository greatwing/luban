using Luban.Types;
using Luban.Typescript.TypeVisitors;
using Scriban.Runtime;

namespace Luban.Typescript.TemplateExtensions;

public class TypescriptBinTemplateExtension : ScriptObject
{
    public static string Deserialize(string fieldName, string buffVar, TType type)
    {
        return type.Apply(BinDeserializeVisitor.Ins, buffVar, fieldName);
    }
}
