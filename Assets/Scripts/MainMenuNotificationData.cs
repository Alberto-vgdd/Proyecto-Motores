public class MainMenuNotificationData {
	private string m_message;
	private string m_subMessage;

	public MainMenuNotificationData(string message, string submessage)
	{
		m_message = message;
		m_subMessage = submessage;
	}

	public string GetMessage()
	{
		return m_message;
	}
	public string GetSubMessage()
	{
		return m_subMessage;
	}
}
