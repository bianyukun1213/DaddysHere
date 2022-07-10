using DaddysHere.Resources;
using Microsoft.Extensions.Localization;

namespace DaddysHere.Models
{
    public class StandardReturn
    {
        public enum ErrorType
        {
            Succeeded = 0,
            UnknownError = -1,
            DataNotFound = 20001,
            WrongData = 20002,
            PermissionDenied = 20003,
            RepeatedSubmissionNotAllowed = 20004,
            LimitValueReached = 20005,
            ThirdPartyServiceError = 30001
        }
        public int Code { get; set; }
        public string? Msg { get; set; }
        public object? Result { get; set; }
        public StandardReturn(IStringLocalizer<SharedResource> localizer, ErrorType errorType = ErrorType.Succeeded, object? result = null)
        {
            Code = (int)errorType;
            Msg = localizer[errorType.ToString()];
            Result = result;
        }
    }
}
