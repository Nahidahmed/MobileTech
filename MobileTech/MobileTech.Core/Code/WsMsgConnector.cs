namespace MobileTech {
    /// <summary>
    /// This transulates to the action that is perfomed on the object type.
    /// </summary>
    public enum EntityAction {
        Create,
        Update,
        Delete,
    }

    /// <summary>
    /// This translated to the type of object that the action was perfomed on.
    /// </summary>
    public enum EntityType {
        WorkOrder,
        Asset,
        TestMessage,
	}
    /// <summary>
    /// This translated to the type of object that the action was perfomed on.
    /// </summary>
    public enum EntityStatus
    {
        None,
        Successful,
        Fail,
    }

    public enum MsgProcessingState {
        UnInitialized,
        ProcessingMessages,
        WaitingForConnection,
        WaitForMessage,
    }
            
    public abstract class WsMsgConnector {
        //public abstract string ParseKey(string entityKey);
		public abstract void ProcessMessage(int id, EntityAction ea, string ek, string eSK);
    }
}