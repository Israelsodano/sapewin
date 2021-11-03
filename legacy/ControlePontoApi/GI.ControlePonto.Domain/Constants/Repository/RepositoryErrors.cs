using GI.ControlePonto.Domain.Constants.Common.Base;

namespace GI.ControlePonto.Domain.Constants.Repository
{
    public class RepositoryErrors : BaseErrors
    {
        public const string CommitError = "The transaction cannot be completed successfully. " + CheckLog;
        public const string BeginTransactionError = "Could not start transaction. " + CheckLog;
        public const string InitTransaction = "Please start the transaction before committing. " + CheckLog;
        public const string UnitTypeisDifferent = "Unit type is different. " + CheckLog;
    }
}