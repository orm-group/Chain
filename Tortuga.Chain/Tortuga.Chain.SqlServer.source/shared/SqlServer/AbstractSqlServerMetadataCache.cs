using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Anchor;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.Metadata;

#if SQL_SERVER

using AbstractDbType = System.Data.SqlDbType;

#elif SQL_SERVER_OLEDB

using AbstractDbType = System.Data.OleDb.OleDbType;

#endif

namespace Tortuga.Chain.SqlServer
{
#if SQL_SERVER

    /// <summary>Class AbstractSqlServerMetadataCache.</summary>
    public abstract class AbstractSqlServerMetadataCache : DatabaseMetadataCache<SqlServerObjectName, AbstractDbType>
#elif SQL_SERVER_OLEDB

    /// <summary>Class AbstractSqlServerMetadataCache.</summary>
    public abstract class AbstractOleDbSqlServerMetadataCache : DatabaseMetadataCache<SqlServerObjectName, AbstractDbType>
#endif

    {
        /// <summary>
        /// Gets the metadata for a table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override sealed TableOrViewMetadata<SqlServerObjectName, AbstractDbType> GetTableOrView(SqlServerObjectName tableName)
        {
            return OnGetTableOrView(tableName);
        }

        //C# doesn't allow us to change the return type so we're using this as a thunk.
        internal abstract TableOrViewMetadata<SqlServerObjectName, AbstractDbType> OnGetTableOrView(SqlServerObjectName tableName);
    }

#if SQL_SERVER

    partial class SqlServerMetadataCache : AbstractSqlServerMetadataCache
#elif SQL_SERVER_OLEDB

    partial class OleDbSqlServerMetadataCache : AbstractOleDbSqlServerMetadataCache
#endif
    {
        internal readonly DbConnectionStringBuilder m_ConnectionBuilder;
        internal readonly ConcurrentDictionary<SqlServerObjectName, ScalarFunctionMetadata<SqlServerObjectName, AbstractDbType>> m_ScalarFunctions = new ConcurrentDictionary<SqlServerObjectName, ScalarFunctionMetadata<SqlServerObjectName, AbstractDbType>>();
        internal readonly ConcurrentDictionary<SqlServerObjectName, StoredProcedureMetadata<SqlServerObjectName, AbstractDbType>> m_StoredProcedures = new ConcurrentDictionary<SqlServerObjectName, StoredProcedureMetadata<SqlServerObjectName, AbstractDbType>>();

        internal readonly ConcurrentDictionary<SqlServerObjectName, TableFunctionMetadata<SqlServerObjectName, AbstractDbType>> m_TableFunctions = new ConcurrentDictionary<SqlServerObjectName, TableFunctionMetadata<SqlServerObjectName, AbstractDbType>>();

        internal readonly ConcurrentDictionary<SqlServerObjectName, SqlServerTableOrViewMetadata<AbstractDbType>> m_Tables = new ConcurrentDictionary<SqlServerObjectName, SqlServerTableOrViewMetadata<AbstractDbType>>();

        internal readonly ConcurrentDictionary<Type, TableOrViewMetadata<SqlServerObjectName, AbstractDbType>> m_TypeTableMap = new ConcurrentDictionary<Type, TableOrViewMetadata<SqlServerObjectName, AbstractDbType>>();

        //internal readonly ConcurrentDictionary<Type, string> m_UdtTypeMap = new ConcurrentDictionary<Type, string>();
        internal readonly ConcurrentDictionary<SqlServerObjectName, UserDefinedTypeMetadata<SqlServerObjectName, AbstractDbType>> m_UserDefinedTypes = new ConcurrentDictionary<SqlServerObjectName, UserDefinedTypeMetadata<SqlServerObjectName, AbstractDbType>>();

        internal string m_DatabaseName;
        internal string m_DefaultSchema;

        /*
        /// <summary>
        /// It is necessary to map some types to their corresponding UDT Names in Sql Server. For example, SqlGeometry and SqlGeography.
        /// </summary>
        /// <param name="type">The type to be mapped</param>
        /// <param name="udtName">The name that SQL server sees</param>
        public void AddUdtTypeName(Type type, string udtName)
        {
            m_UdtTypeMap[type] = udtName;
        }
        */

        /// <summary>
        /// Gets the metadata for a scalar function.
        /// </summary>
        /// <param name="scalarFunctionName">Name of the scalar function.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override ScalarFunctionMetadata<SqlServerObjectName, AbstractDbType> GetScalarFunction(SqlServerObjectName scalarFunctionName)
        {
            return m_ScalarFunctions.GetOrAdd(scalarFunctionName, GetScalarFunctionInternal);
        }

        /// <summary>
        /// Gets the scalar functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all scalar functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<ScalarFunctionMetadata<SqlServerObjectName, AbstractDbType>> GetScalarFunctions()
        {
            return m_ScalarFunctions.GetValues();
        }

        /// <summary>
        /// Gets the stored procedure's metadata.
        /// </summary>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override StoredProcedureMetadata<SqlServerObjectName, AbstractDbType> GetStoredProcedure(SqlServerObjectName procedureName)
        {
            return m_StoredProcedures.GetOrAdd(procedureName, GetStoredProcedureInternal);
        }

        /// <summary>
        /// Gets the stored procedures that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all stored procedures were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<StoredProcedureMetadata<SqlServerObjectName, AbstractDbType>> GetStoredProcedures()
        {
            return m_StoredProcedures.GetValues();
        }

        /// <summary>
        /// Gets the metadata for a table function.
        /// </summary>
        /// <param name="tableFunctionName">Name of the table function.</param>
        /// <returns>Null if the object could not be found.</returns>
        public override TableFunctionMetadata<SqlServerObjectName, AbstractDbType> GetTableFunction(SqlServerObjectName tableFunctionName)
        {
            return m_TableFunctions.GetOrAdd(tableFunctionName, GetTableFunctionInternal);
        }

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<TableFunctionMetadata<SqlServerObjectName, AbstractDbType>> GetTableFunctions()
        {
            return m_TableFunctions.GetValues();
        }

        ///// <summary>
        ///// Gets the UDT name of the indicated type.
        ///// </summary>
        ///// <param name="type">The type.</param>
        ///// <returns></returns>
        ///// <remarks>You may add custom UDTs to this list using AddUdtTypeName</remarks>
        //internal string GetUdtName(Type type)
        //{
        //    string result;
        //    m_UdtTypeMap.TryGetValue(type, out result);
        //    return result;
        //}

        /// <summary>
        /// Returns the table or view derived from the class's name and/or Table attribute.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <returns></returns>
        public override TableOrViewMetadata<SqlServerObjectName, AbstractDbType> GetTableOrViewFromClass<TObject>()
        {
            var type = typeof(TObject);
            TableOrViewMetadata<SqlServerObjectName, AbstractDbType> result;
            if (m_TypeTableMap.TryGetValue(type, out result))
                return result;

            var typeInfo = MetadataCache.GetMetadata(type);
            if (!string.IsNullOrEmpty(typeInfo.MappedTableName))
            {
                if (string.IsNullOrEmpty(typeInfo.MappedSchemaName))
                    result = GetTableOrView(new SqlServerObjectName(typeInfo.MappedTableName));
                else
                    result = GetTableOrView(new SqlServerObjectName(typeInfo.MappedSchemaName, typeInfo.MappedTableName));
                m_TypeTableMap[type] = result;
                return result;
            }

            //infer schema from namespace
            var schema = type.Namespace;
            if (schema?.Contains(".") ?? false)
                schema = schema.Substring(schema.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1);
            var name = type.Name;

            try
            {
                result = GetTableOrView(new SqlServerObjectName(schema, name));
                m_TypeTableMap[type] = result;
                return result;
            }
            catch (MissingObjectException) { }

            //that didn't work, so try the default schema
            result = GetTableOrView(new SqlServerObjectName(null, name));
            m_TypeTableMap[type] = result;
            return result;
        }

        /// <summary>
        /// Gets the tables and views that were loaded by this cache.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Call Preload before invoking this method to ensure that all tables and views were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.
        /// </remarks>
        public override IReadOnlyCollection<TableOrViewMetadata<SqlServerObjectName, AbstractDbType>> GetTablesAndViews()
        {
            return m_Tables.GetValues();
        }

        /// <summary>
        /// Gets the metadata for a user defined type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>UserDefinedTypeMetadata&lt;SqlServerObjectName, SqlDbType&gt;.</returns>
        public override UserDefinedTypeMetadata<SqlServerObjectName, AbstractDbType> GetUserDefinedType(SqlServerObjectName typeName)
        {
            return m_UserDefinedTypes.GetOrAdd(typeName, GetUserDefinedTypeInternal);
        }

        /// <summary>
        /// Gets the table-valued functions that were loaded by this cache.
        /// </summary>
        /// <returns>ICollection&lt;UserDefinedTypeMetadata&lt;SqlServerObjectName, SqlDbType&gt;&gt;.</returns>
        /// <remarks>Call Preload before invoking this method to ensure that all table-valued functions were loaded from the database's schema. Otherwise only the objects that were actually used thus far will be returned.</remarks>
        public override IReadOnlyCollection<UserDefinedTypeMetadata<SqlServerObjectName, AbstractDbType>> GetUserDefinedTypes()
        {
            return m_UserDefinedTypes.GetValues();
        }

        /// <summary>
        /// Preloads all of the metadata for this data source.
        /// </summary>
        public override void Preload()
        {
            PreloadTables();
            PreloadViews();
            PreloadStoredProcedures();
            PreloadTableFunctions();
            PreloadUserDefinedTypes();
            PreloadScalarFunctions();
        }

        /// <summary>
        /// Resets the metadata cache, clearing out all cached metadata.
        /// </summary>
        public override void Reset()
        {
            m_StoredProcedures.Clear();
            m_TableFunctions.Clear();
            m_Tables.Clear();
            m_TypeTableMap.Clear();
            //m_UdtTypeMap.Clear();
            m_UserDefinedTypes.Clear();
            m_ScalarFunctions.Clear();
            m_ScalarFunctions.Clear();
            m_DefaultSchema = null;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        internal static void AdjustTypeDetails(string typeName, ref int? maxLength, ref int? precision, ref int? scale, out string fullTypeName)
        {
            switch (typeName)
            {
                case "bigint":
                case "bit":
                case "date":
                case "datetime":
                case "timestamp":
                case "tinyint":
                case "uniqueidentifier":
                case "smallint":
                case "sql_variant":
                case "float":
                case "int":
                    maxLength = null;
                    precision = null;
                    scale = null;
                    fullTypeName = typeName;
                    break;

                case "binary":
                case "char":
                    precision = null;
                    scale = null;
                    fullTypeName = $"{typeName}({maxLength})";
                    break;

                case "datetime2":
                case "datetimeoffset":
                case "time":
                    maxLength = null;
                    precision = null;
                    fullTypeName = $"{typeName}({scale})";
                    break;

                case "numeric":
                case "decimal":
                    fullTypeName = $"{typeName}({precision},{scale})";
                    break;

                case "nchar":
                    maxLength = maxLength / 2;
                    precision = null;
                    scale = null;
                    fullTypeName = $"nchar({maxLength})";
                    break;

                case "nvarchar":
                    maxLength = maxLength / 2;
                    precision = null;
                    scale = null;
                    if (maxLength > 0)
                        fullTypeName = $"nvarchar({maxLength})";
                    else
                        fullTypeName = $"nvarchar(max)";
                    break;

                case "varbinary":
                case "varchar":
                    precision = null;
                    scale = null;
                    if (maxLength > 0)
                        fullTypeName = $"{typeName}({maxLength})";
                    else
                        fullTypeName = $"{typeName}(max)";
                    break;

                default:
                    if (maxLength <= 0)
                        maxLength = 0;
                    if (precision <= 0)
                        precision = 0;
                    if (scale <= 0)
                        scale = 0;
                    fullTypeName = typeName;
                    break;
            }
        }

        /// <summary>
        /// Parse a string and return the database specific representation of the object name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected override SqlServerObjectName ParseObjectName(string name)
        {
            return new SqlServerObjectName(name);
        }

        internal override TableOrViewMetadata<SqlServerObjectName, AbstractDbType> OnGetTableOrView(SqlServerObjectName tableName)
        {
            return GetTableOrView(tableName);
        }
    }
}