namespace SylviaNG.Community.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when an authenticated caller is not permitted to perform an action on a
    /// specific resource (e.g. editing someone else's profile) - distinct from the
    /// role-based 403s the [Authorize] policies already produce.
    /// </summary>
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message)
        {
        }
    }
}
