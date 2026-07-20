namespace StudyBuddy.API.Execption
{
    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException(string email) 
        {

        }
    }
}
