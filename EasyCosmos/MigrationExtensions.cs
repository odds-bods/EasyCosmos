using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyCosmos
{
    public static class MigrationExtensions
    {
        public static int ReadMigrationNumber(this CosmosMigration migration)
        {
            var success = TryReadNumber(migration.GetType().Name, out var number);
            return success ? number : -1;
        }
        
        public static Type[] FindMigrations(this Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t =>
                    typeof(CosmosMigration).IsAssignableFrom(t) &&
                    t.Name.StartsWith("_")
                );
            return types.ToArray();
        }

        public static Dictionary<int, Type> OrderByNumber
            (this IEnumerable<Type> migrations)
        {
            return migrations.ToDictionaryByMigrationNumber().OrderByNumber();
        }

        public static Dictionary<int, Type> OrderByNumber(this IDictionary<int, Type> migrations)
        {
            return migrations.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public static Dictionary<int, Type> ToDictionaryByMigrationNumber(
            this IEnumerable<Type> migrations)
        {
            var withNumber = migrations.Select(m =>
            {
                var success = TryReadNumber(m.Name, out var number);
               
                return !success ? new {number = -1, migration = m} : new {number = number, migration = m};
            });

            return withNumber.Where(m => m.number >= 0).ToDictionary(kv => kv.number, kv => kv.migration);
        }

        public static Dictionary<int, Type> SkipMigrations(this IDictionary<int, Type> migrations,
            int lastNumber)
        {
            return migrations.Where(m => m.Key > lastNumber)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        private static bool TryReadNumber(string nameOfType, out int number)
        {
           return int.TryParse(
                nameOfType
                    .Split('_')
                    .First(s => !string.IsNullOrEmpty(s))
                , out number
            );
        }
    }
}