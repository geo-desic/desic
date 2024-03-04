using System.Data;

namespace Desic.Data
{
    public class EnhancedDataReader : EnhancedDataRecord, IEnhancedDataReader
    {
        private readonly IDataReader reader;
        public EnhancedDataReader(IDataReader reader) : base(reader)
        {
            this.reader = reader;
        }

        public int Depth
        {
            get
            {
                return reader.Depth;
            }
        }

        public bool IsClosed
        {
            get
            {
                return reader.IsClosed;
            }
        }

        public int RecordsAffected
        {
            get
            {
                return reader.RecordsAffected;
            }
        }

        public void Close()
        {
            reader.Close();
        }

        public void Dispose()
        {
            reader.Dispose();
        }

        public DataTable GetSchemaTable()
        {
            return reader.GetSchemaTable();
        }

        public bool NextResult()
        {
            return reader.NextResult();
        }

        public bool Read()
        {
            return reader.Read();
        }
    }
}
