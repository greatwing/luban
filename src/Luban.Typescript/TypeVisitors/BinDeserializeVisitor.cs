using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors;

public class BinDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
{
    public static BinDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string buffVar, string fieldName)
    {
        if (type.IsNullable)
        {
            return $"if({buffVar} != undefined) {{ {type.Apply(BinUnderlyingDeserializeVisitor.Ins, buffVar, fieldName)} }} else {{ {fieldName} = undefined }}";
        }
        else
        {
            return type.Apply(BinUnderlyingDeserializeVisitor.Ins, buffVar, fieldName);
        }
    }
}
