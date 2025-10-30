namespace MediLabo.Web.Models;

public class ToastMessage
{
    public ToastType Type { get; set; }
    public string Message { get; set; }

    public ToastMessage(ToastType type, string message)
    {
        Type = type;
        Message = message;
    }

    public ToastMessage()
    {
        Type = ToastType.None;
        Message = string.Empty;
    }
}
