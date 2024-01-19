using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors;

public class BinDeserializeVisitor : DecoratorFuncVisitor<string, string, string>
{
    public static BinDeserializeVisitor Ins { get; } = new();

    public override string DoAccept(TType type, string byteBufName, string fieldName)
    {
        if (type.IsNullable)
        {
            return $"if({byteBufName}.ReadBool()) {{ {type.Apply(BinUnderlyingDeserializeVisitor.Ins, byteBufName, fieldName)} }} else {{ {fieldName} = null; }}";
        }
        else
        {
            return type.Apply(BinUnderlyingDeserializeVisitor.Ins, byteBufName, fieldName);
        }
    }
}
