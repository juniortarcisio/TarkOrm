﻿using System;
using System.Data;
using TarkOrm.Mapping;

namespace TarkOrm
{

    // Stage 1 - Querying
    //TarkQueryBuilderClass?
    // - Query prepare (no params) query prepare can be similar 
    // - Query executing -> similar
    // - Receiving a DataReader
    public partial class TarkQueryBuilder
    {
        public TarkOrm _tarkDataAccess;

        public TarkQueryBuilder(TarkOrm tarkDataAccess)
        {
            _tarkDataAccess = tarkDataAccess;
        }

        public TarkQueryBuilderMocker GetMockCommand()
        {
            return new TarkQueryBuilderMocker(_tarkDataAccess);
        }

        public string TableHint;
        
        public string GetMapperTablePath<T>()
        {
            Type type = typeof(T);

            var typeMapping = (TarkTypeMapping<T>) TarkConfigurationMapping.ManageMapping<T>();
            
            if (!String.IsNullOrWhiteSpace(typeMapping.Database))
            {
                return String.Format("{0}.{1}.{2} {3}", typeMapping.Database, typeMapping.Schema, typeMapping.Table, TableHint);
            }
            else
            {
                return String.Format("{0}.{1} {2}", typeMapping.Schema, typeMapping.Table, TableHint);
            }
        }
    }

    public class TarkQueryBuilderMocker
    {
        public TarkOrm _tarkDataAccess;

        public TarkQueryBuilderMocker(TarkOrm tarkDataAccess)
        {
            _tarkDataAccess = tarkDataAccess;
        }

        public IDbCommand GetById<T>(params object[] keyValues)
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.GetById<T>(keyValues);
            return _tarkDataAccess.MockCommand;
        }

        public IDbCommand GetAll<T>()
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.GetAll<T>();
            return _tarkDataAccess.MockCommand;
        }

        public IDbCommand Add<T>(T entity)
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.Add(entity);
            return _tarkDataAccess.MockCommand;
        }

        public IDbCommand Update<T>(T entity)
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.Update(entity);
            return _tarkDataAccess.MockCommand;
        }

        public IDbCommand Remove<T>(T entity)
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.Remove(entity);
            return _tarkDataAccess.MockCommand;
        }

        public IDbCommand RemoveById<T>(params object[] keyValues)
        {
            _tarkDataAccess.MockEnabled = true;
            _tarkDataAccess.RemoveById<T>(keyValues);
            return _tarkDataAccess.MockCommand;
        }
    }

    public static class TableHints
    {        
        public static class SQLServer
        {
            public static string NOLOCK = "WITH(NOLOCK)";
        }
    }
}
