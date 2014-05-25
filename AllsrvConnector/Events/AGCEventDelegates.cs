using System;
using AGCLib;

namespace FreeAllegiance.Tag.Events
{
	/// <summary>
	/// The delegate for a AdminPagedAGCEvent
	/// </summary>
	public delegate void AdminPagedAGCEventDelegate (object sender, AdminPagedAGCEventArgs e);

	/// <summary>
	/// The delegate for a ChatMessageAGCEvent
	/// </summary>
	public delegate void ChatMessageAGCEventDelegate (object sender, ChatMessageAGCEventArgs e);

	/// <summary>
	/// The delegate for a GameCreatedAGCEvent
	/// </summary>
	public delegate void GameCreatedAGCEventDelegate (object sender, GameCreatedAGCEventArgs e);

	/// <summary>
	/// The delegate for a GameDestroyedAGCEvent
	/// </summary>
	public delegate void GameDestroyedAGCEventDelegate (object sender, GameDestroyedAGCEventArgs e);

	/// <summary>
	/// The delegate for a GameEndedAGCEvent
	/// </summary>
	public delegate void GameEndedAGCEventDelegate (object sender, GameEndedAGCEventArgs e);

	/// <summary>
	/// The delegate for a GameOverAGCEvent
	/// </summary>
	public delegate void GameOverAGCEventDelegate (object sender, GameOverAGCEventArgs e);

	/// <summary>
	/// The delegate for a GameStartedAGCEvent
	/// </summary>
	public delegate void GameStartedAGCEventDelegate (object sender, GameStartedAGCEventArgs e);

	/// <summary>
	/// The delegate for a LobbyConnectedAGCEvent
	/// </summary>
	public delegate void LobbyConnectedAGCEventDelegate (object sender, LobbyConnectedAGCEventArgs e);

	/// <summary>
	/// The delegate for a LobbyDisconnectedAGCEvent
	/// </summary>
	public delegate void LobbyDisconnectedAGCEventDelegate (object sender, LobbyDisconnectedAGCEventArgs e);

	/// <summary>
	/// The delegate for a LobbyDisconnectingAGCEvent
	/// </summary>
	public delegate void LobbyDisconnectingAGCEventDelegate (object sender, LobbyDisconnectingAGCEventArgs e);

	/// <summary>
	/// The delegate for a LobbyLostAGCEvent
	/// </summary>
	public delegate void LobbyLostAGCEventDelegate (object sender, LobbyLostAGCEventArgs e);

	/// <summary>
	/// The delegate for a PlayerDroppedAGCEvent
	/// </summary>
	public delegate void PlayerDroppedAGCEventDelegate (object sender, PlayerDroppedAGCEventArgs e);

	/// <summary>
	/// The delegate for a PlayerJoinedTeamAGCEvent
	/// </summary>
	public delegate void PlayerJoinedTeamAGCEventDelegate (object sender, PlayerJoinedTeamAGCEventArgs e);

	/// <summary>
	/// The delegate for a PlayerLeftTeamAGCEvent
	/// </summary>
	public delegate void PlayerLeftTeamAGCEventDelegate (object sender, PlayerLeftTeamAGCEventArgs e);

	/// <summary>
	/// The delegate for a PlayerLoggedInAGCEvent
	/// </summary>
	public delegate void PlayerLoggedInAGCEventDelegate (object sender, PlayerLoggedInAGCEventArgs e);

	/// <summary>
	/// The delegate for a PlayerLoggedOutAGCEvent
	/// </summary>
	public delegate void PlayerLoggedOutAGCEventDelegate (object sender, PlayerLoggedOutAGCEventArgs e);

	/// <summary>
	/// The delegate for a ShipKilledAGCEvent
	/// </summary>
	public delegate void ShipKilledAGCEventDelegate (object sender, ShipKilledAGCEventArgs e);

	/// <summary>
	/// The delegate for a StationCapturedAGCEvent
	/// </summary>
	public delegate void StationCapturedAGCEventDelegate (object sender, StationCapturedAGCEventArgs e);

	/// <summary>
	/// The delegate for a StationCreatedAGCEvent
	/// </summary>
	public delegate void StationCreatedAGCEventDelegate (object sender, StationCreatedAGCEventArgs e);

	/// <summary>
	/// The delegate for a StationDestroyedAGCEvent
	/// </summary>
	public delegate void StationDestroyedAGCEventDelegate (object sender, StationDestroyedAGCEventArgs e);

	/// <summary>
	/// The delegate for a TeamInfoChangedAGCEvent
	/// </summary>
	public delegate void TeamInfoChangedAGCEventDelegate (object sender, TeamInfoChangedAGCEventArgs e);

	/// <summary>
	/// The delegate for a TerminateAGCEvent
	/// </summary>
	public delegate void TerminateAGCEventDelegate (object sender, TerminateAGCEventArgs e);
}
