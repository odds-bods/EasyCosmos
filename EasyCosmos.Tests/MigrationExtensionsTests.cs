using System;
using EasyCosmos.Tests.TestModels;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace EasyCosmos.Tests
{
    public class MigrationExtensionsTests
    {
        [Fact]
        public void CanFindMigrationTypes()
        {
            var migrations = GetType().Assembly.FindMigrations();
            Assert.NotNull(migrations);
            foreach (var migrationType in migrations)
            {
                Assert.True(migrationType.IsSubclassOf(typeof(CosmosMigration)));
            }
        }

        [Fact]
        public void OrdersMigrationsByNumericalOrder()
        {
            var migrations = new [] {
                typeof(_0001_TestMigrationFirst),
                typeof(_0007__TestMigrationSeven) };

            var ordered = migrations.OrderByNumber();

            Assert.True(ordered.First().Key == 1);
            Assert.True(ordered.Last().Key == 7);
        }

        [Fact]
        public void CorrectlyGetsVersionNumber()
        {
            var migrations = new [] {
                typeof(_0001_TestMigrationFirst) };

            var withNumber = migrations.ToDictionaryByMigrationNumber();

            Assert.True(withNumber.ContainsKey(1));            
        }

        [Fact]
        public void WillSkipSpecifiedMigrations()
        {
            var migrations = new Dictionary<int, Type>
            {
                {
                    1, typeof(_0001_TestMigrationFirst)
                },
                {
                    2, typeof(_0002_TestMigrationSecond)
                },
                {
                    7, typeof (_0007__TestMigrationSeven)  
                },
                {
                    8, typeof (_0008__TestMigrationEight)
                },
            };

            var skipToEight = migrations.SkipMigrations(7);

            Assert.True(skipToEight.Count == 1);
            Assert.True(skipToEight.First().Key == 8);
            Assert.True(skipToEight[8] == typeof(_0008__TestMigrationEight));
        }

        [Fact]
        public void AbleToReadCosmosMigrationNumber()
        {
            var migration = new _0003_TimedMigration();
            var number = migration.ReadMigrationNumber();
            Assert.Equal(number, 3 );
        }
    }
}
