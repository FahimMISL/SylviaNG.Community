namespace SylviaNG.Community.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when login credentials are missing/invalid - distinct from ForbiddenException
    /// (an authenticated caller lacking permission) and from the role-based 403s the
    /// [Authorize] policies produce.
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
