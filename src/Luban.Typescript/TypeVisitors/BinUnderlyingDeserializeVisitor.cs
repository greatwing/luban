using Luban.DataExporter.Builtin.Json;
using Luban.Datas;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Typescript.TypeVisitors;

public class BinUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, string>
{
    public static BinUnderlyingDeserializeVisitor Ins { get; } = new BinUnderlyingDeserializeVisitor();

    public string Accept(TBool type, string buffVar, string fieldName)
    {
        return $"{fieldName} = {buffVar}.ReadBool()";
    }

    public string Accept(TByte type, string buffVar, string fieldName)
    {
        return $"{fieldName} = {buffVar}";
    }

    public string Accept(TShort type, string buffVar, string fieldName)
    {
        return $"{fieldName} = {buffVar}";
    }

    public string Accept(TInt type, string buffVar, string fieldName)
    {
        return $"{fieldName} = {buffVar}";
    }

    public string Accept(TLong type, string buffVar, string fieldName)
    {
        return $"{fieldName} = {buffVar}";
    }

    public string Accept(TFloat type, string buffVar, string fieldName)
    {
        return $"{fieldName} = {buffVar}";
    }

    public string Accept(TDouble type, string buffVar, string fieldName)
    {
        return $"{fieldName} = {buffVar}";
    }

    public string Accept(TEnum type, string buffVar, string fieldName)
    {
        return $"{fieldName} = {buffVar}";
    }

    public string Accept(TString type, string buffVar, string fieldName)
    {
        return $"{fieldName} = {buffVar}.ReadString()";
    }

    public string Accept(TDateTime type, string buffVar, string fieldName)
    {
        return $"{fieldName} = {buffVar}";
    }

    public string Accept(TBean type, string buffVar, string fieldName)
    {
        if (type.DefBean.IsAbstractType)
        {
            return $"{fieldName} = {type.DefBean.FullName}.constructorFrom({buffVar})";
        }
        else
        {
            return $"{fieldName} = new {type.DefBean.FullName}({buffVar})";
        }
    }

    public string Accept(TArray type, string buffVar, string fieldName)
    {
        return $"{{ {fieldName} = []; for(let _ele of {buffVar}) {{ let _e; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.push(_e);}}}}";
    }

    public string Accept(TList type, string buffVar, string fieldName)
    {
        return $"{{ {fieldName} = []; for(let _ele of {buffVar}) {{ let _e; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.push(_e);}}}}";
    }

    public string Accept(TSet type, string buffVar, string fieldName)
    {
        if (type.Apply(SimpleJsonTypeVisitor.Ins))
        {
            return $"{fieldName} = {buffVar}";
        }
        else
        {
            return $"{{ {fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(); for(var _ele of {buffVar}) {{ let _e; {type.ElementType.Apply(this, "_ele", "_e")}; {fieldName}.add(_e);}}}}";
        }
    }

    public string Accept(TMap type, string buffVar, string fieldName)
    {
        return $"{fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(); for(var _entry_ of {buffVar}) {{ let _k; {type.KeyType.Apply(this, "_entry_[0]", "_k")};  let _v;  {type.ValueType.Apply(this, "_entry_[1]", "_v")}; {fieldName}.set(_k, _v);  }}";
    }
}
