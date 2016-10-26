namespace FileStorage.Web.Models
{
    /// <summary>
    /// Helper class to validate all stuff on services
    /// </summary>
    public class ModelState
    {
        private string _errorMessage;

        public ModelState()
        {
            IsValid = true;
            TypeOfError = TypeOfServiceError.Success;
        }

        public bool IsValid { get; set; }

        public TypeOfServiceError TypeOfError { get; set; }
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                IsValid = false;
                _errorMessage = value;
            }
        }
    }
}
