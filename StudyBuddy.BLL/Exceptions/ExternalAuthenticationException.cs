namespace StudyBuddy.BLL.Exceptions
{
    public class ExternalAuthenticationException : Exception
    {
        public ExternalAuthenticationException(string provider)
            : base($"{provider} authentication failed.")
        {
        }
    }
}