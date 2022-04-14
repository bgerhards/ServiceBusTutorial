namespace ServiceBusTutorial.Core.Services;

public interface IContactService
{
    string GetName();
    void SetName(string name);
}

public class ContactService : IContactService
{
    private string _name = "George";

    public string GetName()
    {
        return _name;
    }

    public void SetName(string name)
    {
        _name = name;
    }
}