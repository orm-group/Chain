using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Chain.CommandBuilders;
using Tortuga.Chain.Materializers;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.SqlServer.CommandBuilders
{
    /// <summary>
    /// Class SqlServerObjectCommand.
    /// </summary>
    internal abstract class SqlServerObjectCommand : SingleRowDbCommandBuilder<SqlCommand, SqlParameter>
    {
        private readonly IReadOnlyDictionary<string, object> m_ArgumentDictionary;
        private readonly object m_ArgumentValue;
        private readonly TableOrViewMetadata<SqlServerObjectName, SqlDbType> m_Metadata;
        private readonly SqlServerObjectName m_TableName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerObjectCommand" /> class.
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        protected SqlServerObjectCommand(SqlServerDataSourceBase dataSource, SqlServerObjectName tableName, object argumentValue)
            : base(dataSource)
        {
            m_ArgumentValue = argumentValue;
            m_ArgumentDictionary = ArgumentValue as IReadOnlyDictionary<string, object>;
            m_TableName = tableName;
            m_Metadata = DataSource.DatabaseMetadata.GetTableOrView(m_TableName);
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        protected SqlServerObjectName TableName
        {
            get { return m_TableName; }
        }

        /// <summary>
        /// Gets the argument value.
        /// </summary>
        /// <value>The argument value.</value>
        public object ArgumentValue
        {
            get { return m_ArgumentValue; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected IReadOnlyDictionary<string, object> ArgumentDictionary
        {
            get { return m_ArgumentDictionary; }
        }

        /// <summary>
        /// Builds an output clause.
        /// </summary>
        /// <param name="materializer">The materializer.</param>
        /// <param name="returnDeletedColumns">if set to <c>true</c> [return deleted columns].</param>
        /// <returns>System.String.</returns>
        /// <exception cref="DataException"></exception>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected string OutputClause(Materializer<SqlCommand, SqlParameter> materializer, bool returnDeletedColumns)
        {
            if (materializer == null)
                throw new ArgumentNullException("materializer", "materializer is null.");

            var desiredColumns = materializer.DesiredColumns();

            if (desiredColumns == Materializer.NoColumns)
                return null;

            if (desiredColumns == Materializer.AllColumns)
            {
                string prefix = returnDeletedColumns ? "Deleted." : "Inserted.";
                return "OUTPUT " + string.Join(", ", Metadata.Columns.ToList().Select(c => prefix + c.QuotedSqlName));
            }
            else if (desiredColumns.Count > 0)
            {
                var lookup = desiredColumns.ToLookup(c => c);
                var availableColumns = Metadata.Columns.Where(c => lookup.Contains(c.ClrName)).ToList();
                if (availableColumns.Count == 0)
                    throw new MappingException($"None of the requested columns[{ string.Join(", ", desiredColumns) }] where not found on { TableName}");
                string prefix = returnDeletedColumns ? "Deleted." : "Inserted.";
                return "OUTPUT " + string.Join(", ", availableColumns.Select(c => prefix + c.QuotedSqlName));
            }
            else if (Metadata.Columns.Any(c => c.IsPrimaryKey))
            {
                var availableColumns = Metadata.Columns.Where(c => c.IsPrimaryKey).Select(c => "Inserted." + c.QuotedSqlName).ToList();
                return "OUTPUT " + string.Join(", ", availableColumns);
            }
            else if (Metadata.Columns.Any(c => c.IsIdentity))
            {
                var availableColumns = Metadata.Columns.Where(c => c.IsIdentity).Select(c => "Inserted." + c.QuotedSqlName).ToList();
                return "OUTPUT " + string.Join(", ", availableColumns);
            }
            else
            {
                throw new MappingException($"Output was requested, but no columns were specified and the table {TableName} has no primary keys.");
            }
        }



        /// <summary>
        /// Generates a where clause for update/delete operations.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="useKeyAttribute">if set to <c>true</c> use key attribute.</param>
        /// <returns>System.String.</returns>
        protected string WhereClause(List<SqlParameter> parameters, bool useKeyAttribute)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters", "parameters is null.");

            string result;


            if (ArgumentDictionary != null)
            {
                GetKeysFilter filter = (GetKeysFilter.PrimaryKey | GetKeysFilter.ThrowOnMissingProperties);

                var columns = Metadata.GetKeysFor(ArgumentDictionary, filter);

                result = string.Join(" AND ", columns.Select(c => $"{c.QuotedSqlName} = {c.SqlVariableName}"));

                DataSource.LoadDictionaryParameters(ArgumentDictionary, parameters, columns);
            }
            else
            {
                GetPropertiesFilter filter;
                if (useKeyAttribute)
                    filter = (GetPropertiesFilter.ObjectDefinedKey | GetPropertiesFilter.ThrowOnMissingColumns);
                else
                    filter = (GetPropertiesFilter.PrimaryKey | GetPropertiesFilter.ThrowOnMissingProperties);


                var columns = Metadata.GetPropertiesFor(ArgumentValue.GetType(), filter);

                result = string.Join(" AND ", columns.Select(c => $"{c.Column.QuotedSqlName} = {c.Column.SqlVariableName}"));

                DataSource.LoadParameters(ArgumentValue, parameters, columns);
            }

            return result;
        }


        /// <summary>
        /// Gets the table metadata.
        /// </summary>
        /// <value>The metadata.</value>
        public TableOrViewMetadata<SqlServerObjectName, SqlDbType> Metadata
        {
            get { return m_Metadata; }
        }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public new SqlServerDataSourceBase DataSource
        {
            get { return (SqlServerDataSourceBase)base.DataSource; }
        }
    }
}