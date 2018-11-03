using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.Materializers;
using System.Collections.Immutable;
using System.Xml.Linq;

#if !DataTable_Missing
using System.Data;
#endif

namespace Tortuga.Chain.CommandBuilders
{


    /// <summary>
    /// This is the base class for command builders that can potentially return multiple rows.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command type.</typeparam>
    /// <typeparam name="TParameter">The type of the t parameter type.</typeparam>
    public abstract class MultipleRowDbCommandBuilder<TCommand, TParameter> : SingleRowDbCommandBuilder<TCommand, TParameter>, IMultipleRowDbCommandBuilder
        where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected MultipleRowDbCommandBuilder(ICommandDataSource<TCommand, TParameter> dataSource) : base(dataSource) { }


        /// <summary>
        /// Indicates the results should be materialized as a list of booleans.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<bool>> ToBooleanList(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new BooleanListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of booleans.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<bool>> ToBooleanList(ListOptions listOptions = ListOptions.None)
        {
            return new BooleanListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of byte arrays.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<byte[]>> ToByteArrayList(ListOptions listOptions = ListOptions.None)
        {
            return new ByteArrayListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of byte arrays.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<byte[]>> ToByteArrayList(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new ByteArrayListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <param name="collectionOptions">The collection options.</param>
        /// <returns></returns>
        public IConstructibleMaterializer<List<TObject>> ToCollection<TObject>(CollectionOptions collectionOptions = CollectionOptions.None)
            where TObject : class
        {
            return ToCollection<TObject, List<TObject>>(collectionOptions);
        }

        /// <summary>
        /// Materializes the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <param name="collectionOptions">The collection options.</param>
        /// <returns></returns>
        /// <exception cref="MappingException">
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IConstructibleMaterializer<TCollection> ToCollection<TObject, TCollection>(CollectionOptions collectionOptions = CollectionOptions.None)
            where TObject : class
            where TCollection : ICollection<TObject>, new()
        {
            return new CollectionMaterializer<TCommand, TParameter, TObject, TCollection>(this, collectionOptions);
        }

#if !DataTable_Missing
        /// <summary>
        /// Indicates the results should be materialized as a DataSet.
        /// </summary>
        public ILink<DataTable> ToDataTable()
        {
            return new DataTableMaterializer<TCommand, TParameter>(this);
        }
#endif

        /// <summary>
        /// Indicates the results should be materialized as a list of DateTime.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<DateTime>> ToDateTimeList(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new DateTimeListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of DateTime.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<DateTime>> ToDateTimeList(ListOptions listOptions = ListOptions.None)
        {
            return new DateTimeListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of DateTimeOffset.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<DateTimeOffset>> ToDateTimeOffsetList(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new DateTimeOffsetListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of DateTimeOffset.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<DateTimeOffset>> ToDateTimeOffsetList(ListOptions listOptions = ListOptions.None)
        {
            return new DateTimeOffsetListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of DateTimeOffset.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;DateTimeOffset&gt;&gt;.</returns>
        public ILink<HashSet<DateTimeOffset>> ToDateTimeOffsetSet(ListOptions listOptions = ListOptions.None)
        {
            return new DateTimeOffsetSetMaterializer<TCommand, TParameter>(this, null, listOptions);

        }

        /// <summary>
        /// Indicates the results should be materialized as a set of DateTimeOffset.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;DateTimeOffset&gt;&gt;.</returns>
        public ILink<HashSet<DateTimeOffset>> ToDateTimeOffsetSet(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new DateTimeOffsetSetMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of DateTime.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;DateTime&gt;&gt;.</returns>
        public ILink<HashSet<DateTime>> ToDateTimeSet(ListOptions listOptions = ListOptions.None)
        {
            return new DateTimeSetMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of DateTime.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;DateTime&gt;&gt;.</returns>
        public ILink<HashSet<DateTime>> ToDateTimeSet(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new DateTimeSetMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<decimal>> ToDecimalList(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new DecimalListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<decimal>> ToDecimalList(ListOptions listOptions = ListOptions.None)
        {
            return new DecimalListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Decimal&gt;&gt;.</returns>
        public ILink<HashSet<decimal>> ToDecimalSet(ListOptions listOptions = ListOptions.None)
        {
            return new DecimalSetMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of numbers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Decimal&gt;&gt;.</returns>
        public ILink<HashSet<decimal>> ToDecimalSet(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new DecimalSetMaterializer<TCommand, TParameter>(this, columnName, listOptions);

        }

        /// <summary>
        /// Materializes the result as a dictionary of objects.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <param name="keyColumn">The key column.</param>
        /// <param name="dictionaryOptions">The dictionary options.</param>
        /// <returns></returns>
        public IConstructibleMaterializer<Dictionary<TKey, TObject>> ToDictionary<TKey, TObject>(string keyColumn, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
            where TObject : class
        {
            return ToDictionary<TKey, TObject, Dictionary<TKey, TObject>>(keyColumn, dictionaryOptions);
        }

        /// <summary>
        /// Materializes the result as a dictionary of objects.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <typeparam name="TDictionary">The type of dictionary.</typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IConstructibleMaterializer<TDictionary> ToDictionary<TKey, TObject, TDictionary>(string keyColumn, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
            where TObject : class
            where TDictionary : IDictionary<TKey, TObject>, new()
        {
            return new DictionaryMaterializer<TCommand, TParameter, TKey, TObject, TDictionary>(this, keyColumn, dictionaryOptions);
        }

        /// <summary>
        /// Materializes the result as a dictionary of objects.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <param name="keyFunction">The key function.</param>
        /// <param name="dictionaryOptions">The dictionary options.</param>
        /// <returns></returns>
        public IConstructibleMaterializer<Dictionary<TKey, TObject>> ToDictionary<TKey, TObject>(Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
            where TObject : class
        {
            return ToDictionary<TKey, TObject, Dictionary<TKey, TObject>>(keyFunction, dictionaryOptions);
        }

        /// <summary>
        /// Materializes the result as a dictionary of objects.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <typeparam name="TDictionary">The type of dictionary.</typeparam>
        /// <param name="keyFunction">The key function.</param>
        /// <param name="dictionaryOptions">The dictionary options.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IConstructibleMaterializer<TDictionary> ToDictionary<TKey, TObject, TDictionary>(Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
            where TObject : class
            where TDictionary : IDictionary<TKey, TObject>, new()
        {
            return new DictionaryMaterializer<TCommand, TParameter, TKey, TObject, TDictionary>(this, keyFunction, dictionaryOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<double>> ToDoubleList(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new DoubleListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<double>> ToDoubleList(ListOptions listOptions = ListOptions.None)
        {
            return new DoubleListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Double&gt;&gt;.</returns>
        public ILink<HashSet<double>> ToDoubleSet(ListOptions listOptions = ListOptions.None)
        {
            return new DoubleSetMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of numbers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Double&gt;&gt;.</returns>
        public ILink<HashSet<double>> ToDoubleSet(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new DoubleSetMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Materializes the result as a list of dynamically typed objects.
        /// </summary>
        /// <returns></returns>
        public ILink<List<dynamic>> ToDynamicCollection()
        {
            return new DynamicCollectionMaterializer<TCommand, TParameter>(this);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of Guids.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<Guid>> ToGuidList(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new GuidListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of Guids.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<Guid>> ToGuidList(ListOptions listOptions = ListOptions.None)
        {
            return new GuidListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of Guids.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;Guid&gt;&gt;.</returns>
        public ILink<HashSet<Guid>> ToGuidSet(ListOptions listOptions = ListOptions.None)
        {
            return new GuidSetMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of Guids.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;Guid&gt;&gt;.</returns>
        public ILink<HashSet<Guid>> ToGuidSet(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new GuidSetMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Materializes the result as an immutable array of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <param name="collectionOptions">The collection options.</param>
        /// <returns>Tortuga.Chain.IConstructibleMaterializer&lt;System.Collections.Immutable.ImmutableArray&lt;TObject&gt;&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        /// <remarks>In theory this will offer better performance than ToImmutableList if you only intend to read the result.</remarks>
        public IConstructibleMaterializer<ImmutableArray<TObject>> ToImmutableArray<TObject>(CollectionOptions collectionOptions = CollectionOptions.None)
    where TObject : class
        {
            return new ImmutableArrayMaterializer<TCommand, TParameter, TObject>(this, collectionOptions);
        }

        /// <summary>
        /// Materializes the result as a immutable dictionary of objects.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <param name="keyFunction">The key function.</param>
        /// <param name="dictionaryOptions">The dictionary options.</param>
        /// <returns></returns>
        public IConstructibleMaterializer<ImmutableDictionary<TKey, TObject>> ToImmutableDictionary<TKey, TObject>(Func<TObject, TKey> keyFunction, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
            where TObject : class
        {
            return new ImmutableDictionaryMaterializer<TCommand, TParameter, TKey, TObject>(this, keyFunction, dictionaryOptions);
        }

        /// <summary>
        /// Materializes the result as a immutable dictionary of objects.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <param name="keyColumn">The key column.</param>
        /// <param name="dictionaryOptions">The dictionary options.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public IConstructibleMaterializer<ImmutableDictionary<TKey, TObject>> ToImmutableDictionary<TKey, TObject>(string keyColumn, DictionaryOptions dictionaryOptions = DictionaryOptions.None)
            where TObject : class
        {
            return new ImmutableDictionaryMaterializer<TCommand, TParameter, TKey, TObject>(this, keyColumn, dictionaryOptions);
        }

        /// <summary>
        /// Materializes the result as an immutable list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <param name="collectionOptions">The collection options.</param>
        /// <returns>Tortuga.Chain.IConstructibleMaterializer&lt;System.Collections.Immutable.ImmutableList&lt;TObject&gt;&gt;.</returns>
        /// <exception cref="MappingException"></exception>
        /// <remarks>In theory this will offer better performance than ToImmutableArray if you intend to further modify the result.</remarks>
        public IConstructibleMaterializer<ImmutableList<TObject>> ToImmutableList<TObject>(CollectionOptions collectionOptions = CollectionOptions.None)
        where TObject : class
        {
            return new ImmutableListMaterializer<TCommand, TParameter, TObject>(this, collectionOptions);
        }
        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<short>> ToInt16List(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new Int16ListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<short>> ToInt16List(ListOptions listOptions = ListOptions.None)
        {
            return new Int16ListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Int16&gt;&gt;.</returns>
        public ILink<HashSet<short>> ToInt16Set(ListOptions listOptions = ListOptions.None)
        {
            return new Int16SetMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of integers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Int16&gt;&gt;.</returns>
        public ILink<HashSet<short>> ToInt16Set(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new Int16SetMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<int>> ToInt32List(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new Int32ListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<int>> ToInt32List(ListOptions listOptions = ListOptions.None)
        {
            return new Int32ListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Int32&gt;&gt;.</returns>
        public ILink<HashSet<int>> ToInt32Set(ListOptions listOptions = ListOptions.None)
        {
            return new Int32SetMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of integers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Int32&gt;&gt;.</returns>
        public ILink<HashSet<int>> ToInt32Set(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new Int32SetMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<long>> ToInt64List(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new Int64ListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<long>> ToInt64List(ListOptions listOptions = ListOptions.None)
        {
            return new Int64ListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Int64&gt;&gt;.</returns>
        public ILink<HashSet<long>> ToInt64Set(ListOptions listOptions = ListOptions.None)
        {
            return new Int64SetMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of integers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Int64&gt;&gt;.</returns>
        public ILink<HashSet<long>> ToInt64Set(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new Int64SetMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<float>> ToSingleList(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new SingleListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<float>> ToSingleList(ListOptions listOptions = ListOptions.None)
        {
            return new SingleListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Single&gt;&gt;.</returns>
        public ILink<HashSet<float>> ToSingleSet(ListOptions listOptions = ListOptions.None)
        {
            return new SingleSetMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of numbers.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.Single&gt;&gt;.</returns>
        public ILink<HashSet<float>> ToSingleSet(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new SingleSetMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of strings.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<string>> ToStringList(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new StringListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of strings.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<string>> ToStringList(ListOptions listOptions = ListOptions.None)
        {
            return new StringListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of strings.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.String&gt;&gt;.</returns>
        public ILink<HashSet<string>> ToStringSet(ListOptions listOptions = ListOptions.None)
        {
            return new StringSetMaterializer<TCommand, TParameter>(this, null, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a set of strings.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;System.String&gt;&gt;.</returns>
        public ILink<HashSet<string>> ToStringSet(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new StringSetMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Indicates the results should be materialized as a Table.
        /// </summary>
        public ILink<Table> ToTable()
        {
            return new TableMaterializer<TCommand, TParameter>(this);
        }

        /// <summary>
        /// Indicates the results should be materialized as a list of TimeSpan.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<TimeSpan>> ToTimeSpanList(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new TimeSpanListMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }
        /// <summary>
        /// Indicates the results should be materialized as a list of TimeSpan.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public ILink<List<TimeSpan>> ToTimeSpanList(ListOptions listOptions = ListOptions.None)
        {
            return new TimeSpanListMaterializer<TCommand, TParameter>(this, null, listOptions);
        }
        /// <summary>
        /// Indicates the results should be materialized as a set of TimeSpan.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;TimeSpan&gt;&gt;.</returns>
        public ILink<HashSet<TimeSpan>> ToTimeSpanSet(ListOptions listOptions = ListOptions.None)
        {
            return new TimeSpanSetMaterializer<TCommand, TParameter>(this, null, listOptions);

        }

        /// <summary>
        /// Indicates the results should be materialized as a set of TimeSpan.
        /// </summary>
        /// <param name="columnName">Name of the desired column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>ILink&lt;HashSet&lt;TimeSpan&gt;&gt;.</returns>
        public ILink<HashSet<TimeSpan>> ToTimeSpanSet(string columnName, ListOptions listOptions = ListOptions.None)
        {
            return new TimeSpanSetMaterializer<TCommand, TParameter>(this, columnName, listOptions);
        }

        /// <summary>
        /// Materializes the result as a list of XDocuments.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns>Tortuga.Chain.ILink&lt;System.Collections.Generic.List&lt;XDocument&gt;&gt;.</returns>
        public ILink<List<XElement>> ToXElement(ListOptions listOptions = ListOptions.None) { return new XElementListMaterializer<TCommand, TParameter>(this, null, listOptions); }

        /// <summary>
        /// Materializes the result as a list of XDocuments.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="listOptions">The list options.</param>
        /// <returns>Tortuga.Chain.ILink&lt;System.Collections.Generic.List&lt;XDocument&gt;&gt;.</returns>
        public ILink<List<XElement>> ToXElement(string columnName, ListOptions listOptions = ListOptions.None) { return new XElementListMaterializer<TCommand, TParameter>(this, columnName, listOptions); }
    }
}
