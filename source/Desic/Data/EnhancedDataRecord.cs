using System;
using System.Collections.Generic;
using System.Data;

namespace Desic.Data
{
    public class EnhancedDataRecord : IEnhancedDataRecord
    {
        private readonly IDataRecord record;
        private readonly Dictionary<string, int> columnOrdinals;
        private readonly bool cacheOrdinals;

        public EnhancedDataRecord(IDataRecord record)
        {
            this.record = record;
            cacheOrdinals = true;
            if (cacheOrdinals)
            {
                columnOrdinals = new Dictionary<string, int>();
            }
        }

        private int GetOrdinalInternal(string name)
        {
            int result;
            if (cacheOrdinals && columnOrdinals.TryGetValue(name, out result))
            {
                return result;
            }
            result = record.GetOrdinal(name);
            if (cacheOrdinals)
            {
                columnOrdinals[name] = result;
            }
            return result;
        }

        public object this[int i]
        {
            get
            {
                return record[i];
            }
        }

        public object this[string name]
        {
            get
            {
                if (cacheOrdinals) return record[GetOrdinalInternal(name)];
                return record[name];
            }
        }

        public int FieldCount
        {
            get
            {
                return record.FieldCount;
            }
        }

        public bool GetBoolean(int i)
        {
            return record.GetBoolean(i);
        }

        public bool GetBoolean(string name)
        {
            return GetBoolean(GetOrdinalInternal(name));
        }

        public bool? GetBooleanNullable(int i)
        {
            if (record.IsDBNull(i)) return null;
            return record.GetBoolean(i);
        }

        public bool? GetBooleanNullable(string name)
        {
            return GetBooleanNullable(GetOrdinalInternal(name));
        }

        public byte GetByte(int i)
        {
            return record.GetByte(i);
        }

        public byte GetByte(string name)
        {
            return GetByte(GetOrdinalInternal(name));
        }

        public byte? GetByteNullable(int i)
        {
            if (record.IsDBNull(i)) return null;
            return record.GetByte(i);
        }

        public byte? GetByteNullable(string name)
        {
            return GetByteNullable(GetOrdinalInternal(name));
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return record.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return record.GetChar(i);
        }

        public char GetChar(string name)
        {
            return GetChar(GetOrdinalInternal(name));
        }

        public char? GetCharNullable(int i)
        {
            if (record.IsDBNull(i)) return null;
            return record.GetChar(i);
        }

        public char? GetCharNullable(string name)
        {
            return GetCharNullable(GetOrdinalInternal(name));
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return record.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return record.GetData(i);
        }

        public IDataReader GetData(string name)
        {
            return GetData(GetOrdinalInternal(name));
        }

        public string GetDataTypeName(int i)
        {
            return record.GetDataTypeName(i);
        }

        public string GetDataTypeName(string name)
        {
            return GetDataTypeName(GetOrdinalInternal(name));
        }

        public DateTime GetDateTime(int i)
        {
            return record.GetDateTime(i);
        }

        public DateTime GetDateTime(string name)
        {
            return GetDateTime(GetOrdinalInternal(name));
        }

        public DateTime? GetDateTimeNullable(int i)
        {
            if (record.IsDBNull(i)) return null;
            return record.GetDateTime(i);
        }

        public DateTime? GetDateTimeNullable(string name)
        {
            return GetDateTimeNullable(GetOrdinalInternal(name));
        }

        public decimal GetDecimal(int i)
        {
            return record.GetDecimal(i);
        }

        public decimal GetDecimal(string name)
        {
            return GetDecimal(GetOrdinalInternal(name));
        }

        public decimal? GetDecimalNullable(int i)
        {
            if (record.IsDBNull(i)) return null;
            return record.GetDecimal(i);
        }

        public decimal? GetDecimalNullable(string name)
        {
            return GetDecimalNullable(GetOrdinalInternal(name));
        }

        public double GetDouble(int i)
        {
            return record.GetDouble(i);
        }

        public double GetDouble(string name)
        {
            return GetDouble(GetOrdinalInternal(name));
        }

        public double? GetDoubleNullable(int i)
        {
            if (record.IsDBNull(i)) return null;
            return record.GetDouble(i);
        }

        public double? GetDoubleNullable(string name)
        {
            return GetDoubleNullable(GetOrdinalInternal(name));
        }

        public Type GetFieldType(int i)
        {
            return record.GetFieldType(i);
        }

        public Type GetFieldType(string name)
        {
            return GetFieldType(GetOrdinalInternal(name));
        }

        public float GetFloat(int i)
        {
            return record.GetFloat(i);
        }

        public float GetFloat(string name)
        {
            return GetFloat(GetOrdinalInternal(name));
        }

        public float? GetFloatNullable(int i)
        {
            if (record.IsDBNull(i)) return null;
            return record.GetFloat(i);
        }

        public float? GetFloatNullable(string name)
        {
            return GetFloatNullable(GetOrdinalInternal(name));
        }

        public Guid GetGuid(int i)
        {
            return record.GetGuid(i);
        }

        public Guid GetGuid(string name)
        {
            return GetGuid(GetOrdinalInternal(name));
        }

        public Guid? GetGuidNullable(int i)
        {
            if (record.IsDBNull(i)) return null;
            return record.GetGuid(i);
        }

        public Guid? GetGuidNullable(string name)
        {
            return GetGuidNullable(GetOrdinalInternal(name));
        }

        public short GetInt16(int i)
        {
            return record.GetInt16(i);
        }

        public short GetInt16(string name)
        {
            return GetInt16(GetOrdinalInternal(name));
        }

        public short? GetInt16Nullable(int i)
        {
            if (record.IsDBNull(i)) return null;
            return record.GetInt16(i);
        }

        public short? GetInt16Nullable(string name)
        {
            return GetInt16Nullable(GetOrdinalInternal(name));
        }

        public int GetInt32(int i)
        {
            return record.GetInt32(i);
        }

        public int GetInt32(string name)
        {
            return GetInt32(GetOrdinalInternal(name));
        }

        public int? GetInt32Nullable(int i)
        {
            if (record.IsDBNull(i)) return null;
            return record.GetInt32(i);
        }

        public int? GetInt32Nullable(string name)
        {
            return GetInt32Nullable(GetOrdinalInternal(name));
        }

        public long GetInt64(int i)
        {
            return record.GetInt64(i);
        }

        public long GetInt64(string name)
        {
            return GetInt64(GetOrdinalInternal(name));
        }

        public long? GetInt64Nullable(int i)
        {
            if (record.IsDBNull(i)) return null;
            return record.GetInt64(i);
        }

        public long? GetInt64Nullable(string name)
        {
            return GetInt64Nullable(GetOrdinalInternal(name));
        }

        public string GetName(int i)
        {
            return record.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return GetOrdinalInternal(name);
        }

        public string GetString(int i)
        {
            return record.GetString(i);
        }

        public string GetString(string name)
        {
            return GetString(GetOrdinalInternal(name));
        }

        public object GetValue(int i)
        {
            return record.GetValue(i);
        }

        public object GetValue(string name)
        {
            return GetValue(GetOrdinalInternal(name));
        }

        public int GetValues(object[] values)
        {
            return record.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return record.IsDBNull(i);
        }

        public bool IsDBNull(string name)
        {
            return IsDBNull(GetOrdinalInternal(name));
        }
    }
}
