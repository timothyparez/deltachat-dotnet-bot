public static class EventType
{    
    //https://github.com/deltachat/deltachat-core-rust/blob/3b91815240a09ec30a84b0ccdc1e77e57be3f4cb/deltachat-rpc-client/src/deltachat_rpc_client/const.py#L25
    public const string INFO = "Info";
    public const string SMTP_CONNECTED = "SmtpConnected";
    public const string IMAP_CONNECTED = "ImapConnected";
    public const string SMTP_MESSAGE_SENT = "SmtpMessageSent";
    public const string IMAP_MESSAGE_DELETED = "ImapMessageDeleted";
    public const string IMAP_MESSAGE_MOVED = "ImapMessageMoved";
    public const string IMAP_INBOX_IDLE = "ImapInboxIdle";
    public const string NEW_BLOB_FILE = "NewBlobFile";
    public const string DELETED_BLOB_FILE = "DeletedBlobFile";
    public const string WARNING = "Warning";
    public const string ERROR = "Error";
    public const string ERROR_SELF_NOT_IN_GROUP = "ErrorSelfNotInGroup";
    public const string MSGS_CHANGED = "MsgsChanged";
    public const string REACTIONS_CHANGED = "ReactionsChanged";
    public const string INCOMING_MSG = "IncomingMsg";
    public const string INCOMING_MSG_BUNCH = "IncomingMsgBunch";
    public const string MSGS_NOTICED = "MsgsNoticed";
    public const string MSG_DELIVERED = "MsgDelivered";
    public const string MSG_FAILED = "MsgFailed";
    public const string MSG_READ = "MsgRead";
    public const string MSG_DELETED = "MsgDeleted";
    public const string CHAT_MODIFIED = "ChatModified";
    public const string CHAT_EPHEMERAL_TIMER_MODIFIED = "ChatEphemeralTimerModified";
    public const string CONTACTS_CHANGED = "ContactsChanged";
    public const string LOCATION_CHANGED = "LocationChanged";
    public const string CONFIGURE_PROGRESS = "ConfigureProgress";
    public const string IMEX_PROGRESS = "ImexProgress";
    public const string IMEX_FILE_WRITTEN = "ImexFileWritten";
    public const string SECUREJOIN_INVITER_PROGRESS = "SecurejoinInviterProgress";
    public const string SECUREJOIN_JOINER_PROGRESS = "SecurejoinJoinerProgress";
    public const string CONNECTIVITY_CHANGED = "ConnectivityChanged";
    public const string SELFAVATAR_CHANGED = "SelfavatarChanged";
    public const string WEBXDC_STATUS_UPDATE = "WebxdcStatusUpdate";
    public const string WEBXDC_INSTANCE_DELETED = "WebxdcInstanceDeleted";
    public const string CHATLIST_CHANGED = "ChatlistChanged";
    public const string CHATLIST_ITEM_CHANGED = "ChatlistItemChanged";
    public const string CONFIG_SYNCED = "ConfigSynced";
    public const string WEBXDC_REALTIME_DATA = "WebxdcRealtimeData";
}
