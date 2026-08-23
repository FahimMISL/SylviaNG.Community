namespace SylviaNG.Community.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when a downstream external system (e.g. Keycloak's Admin API) fails or rejects a
    /// call in a way this service can't route around - distinct from this API's own auth/permission
    /// exceptions, since the caller was allowed to perform the action but the external dependency
    /// couldn't complete it.
    /// </summary>
    public class ExternalServiceException : Exception
    {
        public ExternalServiceException(string message) : base(message)
        {
        }

        public ExternalServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
