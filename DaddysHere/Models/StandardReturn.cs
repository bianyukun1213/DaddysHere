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
            //switch (errorType)
            //{
            //    //1：系统，2：数据库，3：第三方。
            //    case ErrorType.Succeeded:
            //        Code = 0;
            //        Msg = localizer["succeeded"];
            //        Result = result;
            //        break;
            //    case ErrorType.UnknownError:
            //        Code = -1;
            //        Msg = localizer["unknownError"];
            //        break;
            //    case ErrorType.DataNotFound:
            //        Code = 20001;
            //        Msg = localizer["dataNotFound"];
            //        break;
            //    case ErrorType.WrongData:
            //        Code = 20002;
            //        Msg = localizer["wrongData"];
            //        break;
            //    case ErrorType.PermissionDenied:
            //        Code = 20003;
            //        Msg = localizer["permissionDenied"];
            //        break;
            //    case ErrorType.:
            //        Code = 20004;
            //        Msg = localizer["repeatedSubmissionNotAllowed"];
            //        break;
            //    case 20005:
            //        Code = 20005;
            //        Msg = localizer["reachLimitValue"];
            //        break;
            //    case 30001:
            //        Code = 30001;
            //        Msg = localizer["thirdPartyServiceError"];
            //        break;
            //}
            Code = (int)errorType;
            Msg = localizer[errorType.ToString()];
            Result = result;
        }
    }
}
