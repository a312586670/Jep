namespace Jep.Net.Email
{
	/// <summary>
	/// Smtpop event declaration
	/// </summary>
	public delegate void EventHandler (object sender, System.EventArgs e);
	
	/// <summary>
	/// The connexion event
	/// </summary>
	public delegate void ConnectEventHandler (object sender, ConnectEventParam e);

	/// <summary>
	/// The authentified event
	/// </summary>
	public delegate void AuthentifiedEventHandler (object sender, AuthentifiedEventParam e);

	/// <summary>
	/// The received event
	/// </summary>
	public delegate void ReceivedEventHandler (object sender, ReceivedEventParam e);

	/// <summary>
	/// The Dialog with the server. Fire for each sended command and each received answer
	/// </summary>
	public delegate void ServerDialogEventHandler (object sender, ServerDialogEventParam e);

	/// <summary>
	/// The Dialog with the server. Fire for each received answer
	/// </summary>
	public delegate void ServerAnswerEventHandler (object sender, ServerAnswerEventParam e);

	/// <summary>
	/// The Dialog with the server. Fire for each sended command
	/// </summary>
	public delegate void ClientCommandEventHandler (object sender, ClientCommandEventParam e);


	
}