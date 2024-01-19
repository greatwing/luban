using Luban.DataExporter.Builtin.Json;
using Luban.Datas;
using Luban.Types;
using Luban.TypeVisitors;
using System.Drawing;

namespace Luban.Typescript.TypeVisitors;

public class BinUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, string>
{
    public static BinUnderlyingDeserializeVisitor Ins { get; } = new BinUnderlyingDeserializeVisitor();

    public string Accept(TBool type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.ReadBool()";
    }

    public string Accept(TByte type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.ReadByte()";
    }

    public string Accept(TShort type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.ReadShort()";
    }

    public string Accept(TInt type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.ReadInt()";
    }

    public string Accept(TLong type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.{(type.IsBigInt ? "ReadLong" : "ReadLongAsNumber")}()";
    }

    public string Accept(TFloat type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.ReadFloat()";
    }

    public string Accept(TDouble type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.ReadDouble()";
    }

    public string Accept(TEnum type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.ReadInt()";
    }

    public string Accept(TString type, string bufName, string fieldName)
    {
        return $"{fieldName} = {bufName}.ReadString()";
    }

    public string Accept(TDateTime type, string bufVarName, string fieldName)
    {
        return $"{fieldName} = {bufVarName}.ReadLongAsNumber()";
    }

    public string Accept(TBean type, string bufName, string fieldName)
    {
        if (type.DefBean.IsAbstractType)
        {
            return $"{fieldName} = {type.DefBean.FullName}.constructorFrom({bufName})";
        }
        else
        {
            return $"{fieldName} = new {type.DefBean.FullName}({bufName})";
        }


        //if (type.DefBean.IsAbstractType)
        //{
        //    return $"{fieldName} = {type.DefBean.FullName}.deserializeFrom({bufName})";
        //}
        //else
        //{
        //    return $"{fieldName} = new {type.DefBean.FullName}(); {fieldName}.deserialize({bufName})";
        //}
    }


    public string Accept(TArray type, string bufVarName, string fieldName)
    {
        return $"{{ {fieldName} = []; for(let i = 0, n = {bufVarName}.ReadSize() ; i < n ; i++) {{ let _e :{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, bufVarName, "_e")}; {fieldName}.push(_e) }} }}";
        //return $"{{ let n = Math.min({bufVarName}.ReadSize(), {bufVarName}.Size); {fieldName} = []; for(let i = 0 ; i < n ; i++) {{ let _e :{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)};{type.ElementType.Apply(this, bufVarName, "_e")}; {fieldName}.push(_e) }} }}";
        //return $"{{ let n = Math.min({bufVarName}.ReadSize(), {bufVarName}.Size); {fieldName} = {GetNewArray(type, "n")}; for(let i = 0 ; i < n ; i++) {{ let _e :{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)};{type.ElementType.Apply(this, bufVarName, "_e")}; {(IsRawArrayElementType(type.ElementType) ? $"{fieldName}[i] = _e" : $"{fieldName}.push(_e)")} }} }}";
    }

    public string Accept(TList type, string bufVarName, string fieldName)
    {
        return $"{{ {fieldName} = []; for(let i = 0, n = {bufVarName}.ReadSize() ; i < n ; i++) {{ let _e :{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)}; {type.ElementType.Apply(this, bufVarName, "_e")}; {fieldName}.push(_e) }} }}";
    }

    public string Accept(TSet type, string bufVarName, string fieldName)
    {
        return $"{{ {fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(); for(let i = 0, n = {bufVarName}.ReadSize() ; i < n ; i++) {{ let _e:{type.ElementType.Apply(DeclaringTypeNameVisitor.Ins)};{type.ElementType.Apply(this, bufVarName, "_e")}; {fieldName}.add(_e);}}}}";
    }

    public string Accept(TMap type, string bufVarName, string fieldName)
    {
        return $"{{ {fieldName} = new {type.Apply(DeclaringTypeNameVisitor.Ins)}(); for(let i = 0, n = {bufVarName}.ReadSize() ; i < n ; i++) {{ let _k:{type.KeyType.Apply(DeclaringTypeNameVisitor.Ins)}; {type.KeyType.Apply(this, bufVarName, "_k")}; let _v:{type.ValueType.Apply(DeclaringTypeNameVisitor.Ins)}; {type.ValueType.Apply(this, bufVarName, "_v")}; {fieldName}.set(_k, _v);  }} }}";
    }

    public static string GetNewArray(TType elementType)
    {
        switch (elementType)
        {
            case TByte _:
                return "new Uint8Array()";
            case TShort _:
            //case TFshort _:
                return "new Int16Array()";
            case TInt _:
            //case TFint _:
                return "new Int32Array()";
            case TLong _:
            //case TFlong _:
                return "new Int64Array()";
            case TFloat _:
                return "new Float32Array()";
            case TDouble _:
                return "new Float64Array()";
            default:
                return "[]";
        }
    }

    public static string GetNewArray(TArray arrayType, string size)
    {
        switch (arrayType.ElementType)
        {
            case TByte _:
                return $"new Uint8Array({size})";
            case TShort _:
            //case TFshort _:
                return $"new Int16Array({size})";
            case TInt _:
            //case TFint _:
                return $"new Int32Array({size})";
            case TLong _:
            //case TFlong _:
                return $"new Int64Array({size})";
            case TFloat _:
                return $"new Float32Array({size})";
            case TDouble _:
                return $"new Float64Array({size})";
            default:
                return "[]";
        }
    }

    private bool IsRawArrayElementType(TType elementType)
    {
        switch (elementType)
        {
            case TByte _:
            case TShort _:
            //case TFshort _:
            case TInt _:
            //case TFint _:
            case TLong _:
            //case TFlong _:
            case TFloat _:
            case TDouble _:
                return true;
            default:
                return false;
        }
    }
}
