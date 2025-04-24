namespace System.Net;

internal static class ExceptionHelper
{
	internal static NotImplementedException MethodNotImplementedException => new NotImplementedException(global::SR.GetString("This method is not implemented by this class."));

	internal static NotImplementedException PropertyNotImplementedException => new NotImplementedException(global::SR.GetString("This property is not implemented by this class."));

	internal static WebException TimeoutException => new WebException("The operation has timed out.");

	internal static NotSupportedException MethodNotSupportedException => new NotSupportedException(global::SR.GetString("This method is not supported by this class."));

	internal static NotSupportedException PropertyNotSupportedException => new NotSupportedException(global::SR.GetString("This property is not supported by this class."));

	internal static WebException IsolatedException => new WebException(NetRes.GetWebStatusString("net_requestaborted", WebExceptionStatus.KeepAliveFailure), WebExceptionStatus.KeepAliveFailure, WebExceptionInternalStatus.Isolated, null);

	internal static WebException RequestAbortedException => new WebException(NetRes.GetWebStatusString("net_requestaborted", WebExceptionStatus.RequestCanceled), WebExceptionStatus.RequestCanceled);

	internal static WebException CacheEntryNotFoundException => new WebException(NetRes.GetWebStatusString("net_requestaborted", WebExceptionStatus.CacheEntryNotFound), WebExceptionStatus.CacheEntryNotFound);

	internal static WebException RequestProhibitedByCachePolicyException => new WebException(NetRes.GetWebStatusString("net_requestaborted", WebExceptionStatus.RequestProhibitedByCachePolicy), WebExceptionStatus.RequestProhibitedByCachePolicy);
}
