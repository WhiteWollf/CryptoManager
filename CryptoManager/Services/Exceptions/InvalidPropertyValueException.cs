namespace Services.Exceptions
{
    public class InvalidPropertyValueException : Exception
    {
        public InvalidPropertyValueException(string name, string type) : base($"The {name} field's value has to be {type} type!")
        {

        }
    }
}
