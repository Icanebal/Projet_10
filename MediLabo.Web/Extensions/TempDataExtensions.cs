using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text.Json;
using MediLabo.Web.Models;

namespace MediLabo.Web.Extensions;

public static class TempDataExtensions
{
    private const string ToastMessagesKey = "ToastMessages";

    public static void AddToastMessage(this ITempDataDictionary tempData, ToastMessage toast)
    {
        var messages = tempData.GetToastMessages();
        messages.Add(toast);
        tempData[ToastMessagesKey] = JsonSerializer.Serialize(messages);
    }

    public static List<ToastMessage> GetToastMessages(this ITempDataDictionary tempData)
    {
        if (tempData[ToastMessagesKey] is string json)
        {
            try
            {
                return JsonSerializer.Deserialize<List<ToastMessage>>(json) ?? new List<ToastMessage>();
            }
            catch
            {
                return new List<ToastMessage>();
            }
        }
        return new List<ToastMessage>();
    }
}
