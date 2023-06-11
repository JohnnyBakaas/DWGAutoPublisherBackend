namespace DWGAutoPublisherBackend.Helpers
{
    public class Validator : IValidator
    {
        public bool IsValidStatus(string status)
        {
            return !string.IsNullOrEmpty(status);
        }
    }
}
