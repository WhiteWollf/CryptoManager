using Services.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;

namespace Services.Services;

public class ObjectValidatorService<TObject> where TObject : class
{
    TObject myObject;

    public ObjectValidatorService(TObject myObject)
    {
        this.myObject = myObject;
    }

    public void IsValid()
    {
        PropertyInfo[] props = myObject.GetType().GetProperties();
        foreach (PropertyInfo prop in props)
        {
            if (Attribute.IsDefined(prop, typeof(RequiredAttribute)))
            {
                if (prop.GetValue(myObject) == null || string.IsNullOrEmpty(prop.GetValue(myObject).ToString()))
                {
                    throw new MandatoryPropertyEmptyException(prop.Name);
                }
            }
            if (Attribute.IsDefined(prop, typeof(EmailAddressAttribute)))
            {
                string value = prop.GetValue(myObject).ToString();
                /*  if (!Regex.Match(value, @"/^[a-zA-Z0-9\.\-+#]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-]$/").Success)
                  {
                      throw new InvalidPropertyValueException(prop.Name, "e-mail cím");
                  }*/
                if (!value.Contains("@") ||
                    value.IndexOf("@") == value.Length - 1 ||
                    value.IndexOf("@") == 0 ||
                    value.Count(x => x == '@') > 1
                    )
                {
                    throw new InvalidPropertyValueException(prop.Name, "e-mail cím");
                }
                if (!_isValidDomain(value))
                {
                    throw new InvalidEmailDomainException();
                }
            }
        }
    }

    private bool _isValidDomain(string email)
    {
        string domain = email.Split("@")[1];
        try
        {
            IPHostEntry entry = Dns.GetHostEntry(domain);
            if (entry != null)
            {
                return true;
            }
        }
        catch
        { }
        return false;
    }
}
