namespace CereBro;

public struct Message
{
    public Role Role { get; private set; }

    public string Value { get; private set; }

    public Message(Role role, string message)
    {
        Role = role;
        
        Value = message;
    }
}