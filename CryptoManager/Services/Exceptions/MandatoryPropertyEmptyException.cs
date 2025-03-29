namespace Services.Exceptions
{
    public class MandatoryPropertyEmptyException : Exception
    {
        public MandatoryPropertyEmptyException(string propertyName) : base($"The {propertyName} property is required!")
        {
        }
    }
}
