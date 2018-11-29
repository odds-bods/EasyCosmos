using System;
using System.Threading.Tasks;

namespace EasyCosmos.Tests.TestModels
{
    class FailureMigration : CosmosMigration
    {
        private bool shouldFail;

        public FailureMigration(bool shouldFail)
        {
            this.shouldFail = shouldFail;
        }

        protected override Task Migrate()
        {
            if (shouldFail) throw new Exception();
            return Task.CompletedTask;
        }
    }
}
