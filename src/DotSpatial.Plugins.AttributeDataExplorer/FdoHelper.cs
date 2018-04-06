// -----------------------------------------------------------------------
// <copyright file="FdoHelper.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System.Data;
using DevExpress.Utils;

namespace DotSpatial.Plugins.AttributeDataExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using OSGeo.FDO.ClientServices;
    using OSGeo.FDO.Commands;
    using OSGeo.FDO.Commands.Feature;
    using OSGeo.FDO.Commands.Schema;
    using OSGeo.FDO.Commands.SpatialContext;
    using OSGeo.FDO.Connections;
    using OSGeo.FDO.Geometry;
    using OSGeo.FDO.Schema;


    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class FdoHelper
    {
        public static DataTable GetDataFromFile(string filename)
        {
            Guard.ArgumentNotNull(filename, "filename");

            // Create an FDO connection to the SHP provider
            var mgr = FeatureAccessManager.GetConnectionManager();
            using (var connection = mgr.CreateConnection("OSGeo.SHP"))
            {
                // Set connection properties
                var props = connection.ConnectionInfo.ConnectionProperties;
                props.SetProperty("DefaultFileLocation", filename);

                // Open the connection
                connection.Open();

                // Check the connection state
                if (connection.ConnectionState == ConnectionState.ConnectionState_Open)
                    Console.WriteLine("Connection was opened successfully.");
                else
                    Console.WriteLine("Connection failed to open.");

                // Create the Select command
                using (ISelect select = (ISelect)connection.CreateCommand(CommandType.CommandType_Select))
                {

                    //TODO: fix
                    //Invalid Feature schema element name 'ef02f72c-67e8-4967-89d5-610b57aa5bcf.6.48F1FC14219145F1663E0030FCAA50D5'; must not contain '.'.
                    // Set the feature class name
                    select.SetFeatureClassName(System.IO.Path.GetFileNameWithoutExtension(filename));

                    // Execute the Select command
                    using (IFeatureReader reader = select.Execute())
                    {
                        DataTable table = new DataTable();
                        PrepareGrid(table, reader);

                        // Read the features
                        try
                        {
                            while (reader.ReadNext())
                            {
                                ProcessSQLReader(table, reader);
                            }
                            return table;
                        }
                        catch (OSGeo.FDO.Common.Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
        }


        private static void ProcessSQLReader(DataTable table, IFeatureReader reader)
        {
            // Get the class definition
            ClassDefinition classDefinition = reader.GetClassDefinition();

            DataRow row = table.NewRow();
            foreach (DataColumn col in table.Columns)
            {
                string identifier = col.ColumnName;
                if (!reader.IsNull(identifier))
                {
                    PropertyDefinition property = classDefinition.Properties[identifier];

                    PropertyType ptype = property.PropertyType;
                    if (ptype != PropertyType.PropertyType_DataProperty)
                    {
                        continue;
                    }

                    var p = (DataPropertyDefinition)property;

                    DataType dtype = p.DataType;
                    switch (dtype)
                    {
                        case DataType.DataType_BLOB:
                            row[identifier] = reader.GetLOB(identifier).Data;
                            break;
                        case DataType.DataType_Boolean:
                            row[identifier] = reader.GetBoolean(identifier);
                            break;
                        case DataType.DataType_Byte:
                            row[identifier] = reader.GetByte(identifier);
                            break;
                        case DataType.DataType_CLOB:
                            row[identifier] = reader.GetLOB(identifier).Data;
                            break;
                        case DataType.DataType_DateTime:
                            row[identifier] = reader.GetDateTime(identifier);
                            break;
                        case DataType.DataType_Decimal:
                            row[identifier] = reader.GetDouble(identifier);
                            break;
                        case DataType.DataType_Double:
                            row[identifier] = reader.GetDouble(identifier);
                            break;
                        case DataType.DataType_Int16:
                            row[identifier] = reader.GetInt16(identifier);
                            break;
                        case DataType.DataType_Int32:
                            row[identifier] = reader.GetInt32(identifier);
                            break;
                        case DataType.DataType_Int64:
                            row[identifier] = reader.GetInt64(identifier);
                            break;
                        case DataType.DataType_Single:
                            row[identifier] = reader.GetSingle(identifier);
                            break;
                        case DataType.DataType_String:
                            row[identifier] = reader.GetString(identifier);
                            break;
                    }
                }
                else
                {
                    row[identifier] = DBNull.Value;
                }
            }
            table.Rows.Add(row);
        }

        /// <summary>
        /// Prepares the data grid for select aggregate query results
        /// </summary>
        /// <param name="table"></param>
        /// <param name="reader"></param>
        private static void PrepareGrid(DataTable table, IFeatureReader reader)
        {
            // Get the class definition
            ClassDefinition classDefinition = reader.GetClassDefinition();

            // Get the column (property) names
            foreach (PropertyDefinition def in classDefinition.Properties)
            {
                var property = def as DataPropertyDefinition;

                // We're not going to display geo properties in the table.
                // We're only interested in data properties.
                if (property != null)
                {
                    table.Columns.Add(def.Name, GetSystemType(property.DataType));
                }
            }
        }

        private static Type GetSystemType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.DataType_BLOB:
                    throw new NotSupportedException();

                case DataType.DataType_Boolean:
                    return typeof(bool);

                case DataType.DataType_Byte:
                    return typeof(byte);

                case DataType.DataType_CLOB:
                    throw new NotSupportedException();

                case DataType.DataType_DateTime:
                    return typeof(DateTime);

                case DataType.DataType_Decimal:
                    return typeof(decimal);

                case DataType.DataType_Double:
                    return typeof(double);

                case DataType.DataType_Int16:
                    return typeof(System.Int16);

                case DataType.DataType_Int32:
                    return typeof(System.Int32);

                case DataType.DataType_Int64:
                    return typeof(System.Int64);

                case DataType.DataType_Single:
                    return typeof(float);

                case DataType.DataType_String:
                    return typeof(string);
            }

            throw new NotSupportedException();
        }

        //// Creates a feature class
        //private static void CreateFeatureClass(IConnection connection)
        //{
        //    // Get the default spatial context (IGetSpatialContexts)
        //    string context = "Default";
        //    using (var getSpatialContexts = (IGetSpatialContexts)connection.CreateCommand(CommandType.CommandType_GetSpatialContexts))
        //    {
        //        using (var reader = getSpatialContexts.Execute())
        //        {
        //            if (reader.ReadNext())
        //                context = reader.GetName();
        //        }
        //    }

        //    // Add the new feature class to the existing schema (IDescribeSchema)
        //    using (var describeSchema = (IDescribeSchema)connection.CreateCommand(CommandType.CommandType_DescribeSchema))
        //    {
        //        // Get the existing schema through IDescribeSchema.Execute()
        //        using (var schemas = describeSchema.Execute())
        //        {
        //            // Get the first schema (or create a new one)
        //            FeatureSchema schema = schemas.Count > 0
        //                                    ? schemas[0]
        //                                    : new FeatureSchema("Default", "The default schema.");

        //            // Create a new FeatureClass object
        //            var featureClass = new FeatureClass("StateCapitals", "A point feature class for the state capitals.");

        //            // Add an identity property
        //            var id = new DataPropertyDefinition("FeatId", "A identity property.")
        //            {
        //                DataType = DataType.DataType_Int32,
        //                IsAutoGenerated = true,
        //                Nullable = false
        //            };

        //            featureClass.Properties.Add(id);
        //            featureClass.IdentityProperties.Add(id);

        //            // Add a Name property
        //            featureClass.Properties.Add(new DataPropertyDefinition("Name", "The name of the city.")
        //            {
        //                DataType = DataType.DataType_String,
        //                Length = 100,
        //                Nullable = false
        //            });

        //            // Add a geometry property (for point features)
        //            featureClass.Properties.Add(new GeometricPropertyDefinition("Geometry", "The geometry property")
        //            {
        //                GeometryTypes = (int)GeometricType.GeometricType_Point,
        //                SpatialContextAssociation = context
        //            });

        //            // Add the new feature class to the schema
        //            schema.Classes.Add(featureClass);

        //            // Apply the schema (IApplySchema command)
        //            using (var applySchema = (IApplySchema)connection.CreateCommand(CommandType.CommandType_ApplySchema))
        //            {
        //                applySchema.FeatureSchema = schema;
        //                applySchema.Execute();
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Insert the features (IInsert)
        /// </summary>
        /// <param name="connection"></param>
        //   private static void InsertFeatures(IConnection connection)
        //  {
        //var factory = new FgfGeometryFactory();

        //// Loop through the state capitals collection
        //foreach (City city in City.GetCapitals())
        //{
        //    // Write the name of the city to the console
        //    Console.WriteLine(city.Name);

        //    // Use the IInsert command to insert the features
        //    using (var insert = (IInsert)connection.CreateCommand(CommandType.CommandType_Insert))
        //    {
        //        // Set the feature class name for the insert command
        //        insert.SetFeatureClassName("Default:StateCapitals");

        //        // Add property value for name
        //        insert.PropertyValues.Add(new PropertyValue("Name", new StringValue(city.Name)));

        //        // Add the property value for geometry
        //        IDirectPosition position = factory.CreatePositionXY(city.Lon, city.Lat);
        //        IGeometry point = factory.CreatePoint(position);
        //        byte[] bytes = factory.GetFgf(point);
        //        insert.PropertyValues.Add(new PropertyValue("Geometry", new GeometryValue(bytes)));

        //        // Execute the insert command
        //        using (var reader = insert.Execute())
        //        {
        //            reader.Close();
        //        }
        //    }
        //}
        //Console.WriteLine("The features have been inserted.");
        //  }


    }
}
