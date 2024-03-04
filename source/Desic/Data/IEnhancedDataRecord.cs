using System;
using System.Data;

namespace Desic.Data
{
    public interface IEnhancedDataRecord : IDataRecord
    {
        bool GetBoolean(string name);
        bool? GetBooleanNullable(int i);
        bool? GetBooleanNullable(string name);
        byte GetByte(string name);
        byte? GetByteNullable(int i);
        byte? GetByteNullable(string name);
        char GetChar(string name);
        char? GetCharNullable(int i);
        char? GetCharNullable(string name);
        IDataReader GetData(string name);
        string GetDataTypeName(string name);
        DateTime GetDateTime(string name);
        DateTime? GetDateTimeNullable(int i);
        DateTime? GetDateTimeNullable(string name);
        decimal GetDecimal(string name);
        decimal? GetDecimalNullable(int i);
        decimal? GetDecimalNullable(string name);
        double GetDouble(string name);
        double? GetDoubleNullable(int i);
        double? GetDoubleNullable(string name);
        Type GetFieldType(string name);
        float GetFloat(string name);
        float? GetFloatNullable(int i);
        float? GetFloatNullable(string name);
        Guid GetGuid(string name);
        Guid? GetGuidNullable(int i);
        Guid? GetGuidNullable(string name);
        short GetInt16(string name);
        short? GetInt16Nullable(int i);
        short? GetInt16Nullable(string name);
        int GetInt32(string name);
        int? GetInt32Nullable(int i);
        int? GetInt32Nullable(string name);
        long GetInt64(string name);
        long? GetInt64Nullable(int i);
        long? GetInt64Nullable(string name);
        string GetString(string name);
        object GetValue(string name);
        bool IsDBNull(string name);
    }
}
