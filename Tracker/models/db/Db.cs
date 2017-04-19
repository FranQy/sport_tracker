using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Tracker.models.db
{
    public class Db
    {
        private SQLiteConnection _conn;

        private static Dictionary<String, Db> _instantions = new Dictionary<string, Db>();

        private Db(String database)
        {
            _conn = new SQLiteConnection(database + ".sqlite");
        }
        public static Db connect(String database)
        {
            if (_instantions.ContainsKey(database))
                return _instantions[database];

            Db _this = new Db(database);
            _instantions[database] = _this;
            return _this;
        }

        public async Task<long> insert(object obj)
        {
            try
            {
                if (_conn.Insert(obj) == 1)
                {
                    return _conn.ExecuteScalar<long>(@"select last_insert_rowid()");
                }
            }
            catch (Exception e)
            {
                await this.createTable(obj.GetType());
            }

            return 0;
        }

        private async  Task<bool> createTable(Type t)
        {
            var result = 0;
            try
            {
                result = _conn.CreateTable(t);
            }
            catch (Exception e)
            {
                result = 0;
            }
            return result != 0;
        }

        public async Task<bool> update(object obj)
        {
            var result = 0;
            try
            {
                result = _conn.Update(obj);
            }
            catch (Exception e)
            {
                result = 0;
            }
            return result != 0;
        }


        public async Task<T> getRow<T>(string query) where T : class, new()
        {
            T elem = null;
            try
            {
                elem = _conn.Query<T>(query)[0];
            }
            catch (Exception e)
            {
                elem = null;
            }

            return elem;
        }
        public async Task<List<T>> getAll<T>(string query) where T : new()
        {
            List<T> elem = null;
            try
            {
                elem = _conn.Query<T>(query);
            }
            catch (Exception e)
            {
                elem = null;
            }
            return elem;
        }


    }


}
