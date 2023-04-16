using Azure.Messaging.ServiceBus;

namespace ServiceBusTool.ServiceBus;

public record EntityPath
{
    private const string SubscriptionSlug = "Subscriptions";
    
    private const string ErrorMessageValidQueueOrSubscription =
        @"Must be of one of the formats
    - {queue}, 
    - {queue}/${sub-queue}
    - {topic}/Subscriptions/subscription-name
    - {topic}/Subscriptions/{subscription}/${sub-queue}";
    
    private const string ErrorMessageValidSubscription =
        @"Must be of the format
    - {topic}/Subscriptions/subscription-name";

    /// <summary>
    /// Parse EntityPath from a string.
    /// </summary>
    /// <remarks>
    /// topic-name/Subscriptions/subscription-name
    /// </remarks>
    /// <param name="entityPathStr"></param>
    /// <returns></returns>
    public static EntityPath Parse(string entityPathStr)
    {
        var parts = entityPathStr.Split("/");
        var entityPath = new EntityPath { IsQueue = false };

        return parts.Length switch
        {
            1 => entityPath with { Queue = parts[0], IsQueue = true },
            2 => entityPath with { Queue = parts[0], SubQueue = parts[1] },
            3 => entityPath with { Topic = parts[0], Subscription = parts[2] },
            4 => entityPath with { Topic = parts[0], Subscription = parts[2], SubQueue = parts[3] },
            _ => entityPath
        };
    }

    public static bool IsValidSubscription(string entityPathStr, out string errorMessage)
    {
        if (!IsValid(entityPathStr, out _) || Parse(entityPathStr).IsQueue)
        {
            errorMessage = ErrorMessageValidSubscription;
        }
        else
        {
            errorMessage = null;
        }

        return errorMessage == null;
    }

    public static bool IsValid(string entityPathStr, out string errorMessage)
    {
        errorMessage = null;
        var parts = entityPathStr.Split("/");
        if (parts.Length == 0)
        {
            errorMessage = ErrorMessageValidQueueOrSubscription;
        }
        else if (parts.Length == 2)
        {
            if (!Enum.TryParse(typeof(SubQueue), parts[1], ignoreCase: true, out _))
            {
                errorMessage = $"{parts[1]} is not a valid sub queue";
            }
        }
        else if (parts.Length == 3)
        {
            if (parts[2] != SubscriptionSlug)
            {
                errorMessage = $"When the entity path contains three parts the second part should be '{SubscriptionSlug}'";
            }
        }
        else if (parts.Length == 4)
        {
            if (!Enum.TryParse(typeof(SubQueue), parts[3], ignoreCase: true, out _))
            {
                errorMessage = $"{parts[3]} is not a valid sub queue";
            }
        }

        return errorMessage != null;
    }
    
    public bool IsQueue { get; init; }
    
    public string Topic { get; init; }
    
    public string Subscription { get; init; }
    
    public string Queue { get; init; }
    
    public string SubQueue { get; init; }
}