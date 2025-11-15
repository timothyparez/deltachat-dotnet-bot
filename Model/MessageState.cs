public enum MessageState
{
    Undefined = 0,
    InFresh = 10,
    InNoticed = 13,
    InSeen = 16,
    OutPreparing = 18,
    OutDraft = 19,
    OutPending = 20,
    OutFailed = 24,
    OutDelivered = 26,
    OutMdnReceived = 28 //What does MDN stand for?
}
