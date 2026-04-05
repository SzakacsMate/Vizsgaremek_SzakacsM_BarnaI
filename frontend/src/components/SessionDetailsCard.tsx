import type { Session } from "../types/session";

type SessionDetailsCardProps = {
  session: Session;
  onBack: () => void;
  onLeave?: (sessionId: string) => void;
  onDeleteLobby?: (sessionId: string) => void;
  onKick?: (lobbyId: string, userId: string) => void;
  currentUserName?: string;
  currentUserId?: string;
};

export default function SessionDetailsCard({
  session,
  onBack,
  onLeave,
  onDeleteLobby,
  onKick,
  currentUserName,
  currentUserId,
}: SessionDetailsCardProps) {
  const currentPlayerCount = session.players.length;
  const isDm =
    (currentUserName != null && session.dmName === currentUserName) ||
    (currentUserId != null && session.dmName === currentUserId);

  return (
    <section className="session-details-card">
      <div
        className="session-details-left-bar"
        style={{ backgroundColor: session.systemColor }}
      />

      <div className="session-details-content">
        <h2 className="session-details-title">
          {session.title} - Session {session.sessionNumber}
        </h2>

        <div className="session-details-system-row">
          <span
            className="session-details-system"
            style={{ color: session.systemColor }}
          >
            {session.system}
          </span>

          <span
            className={`session-details-status ${
              session.status === "confirmed"
                ? "status-confirmed"
                : "status-pending"
            }`}
          >
            {session.status === "confirmed" ? "Confirmed" : "Pending"}
          </span>
        </div>

        <div
          className="session-details-divider"
          style={{ backgroundColor: session.systemColor }}
        />

        <p className="session-details-text">{session.date}</p>
        <p className="session-details-text">{session.location}</p>
        <p className="session-details-text">Dungeon Master: {session.dmName}</p>

        {session.description && (
          <p className="session-details-text">{session.description}</p>
        )}

        <div className="session-details-players-section">
          <p className="session-details-players-title">Players:</p>

          <ul className="session-details-player-list">
            {session.players.map((player) => (
              <li key={player.id || player.name}>
                {player.name}
                {isDm && onKick && player.id && (
                  <button
                    className="session-action-button kick"
                    onClick={() => onKick(session.id, player.id)}
                  >
                    KICK
                  </button>
                )}
              </li>
            ))}
          </ul>
        </div>

        <div className="session-details-bottom-row">
          <div className="session-details-player-bars">
            {Array.from({ length: session.playerLimit }).map((_, index) => {
              const isFilled = index < currentPlayerCount;

              return (
                <div
                  key={index}
                  className="session-details-player-bar"
                  style={{
                    backgroundColor: isFilled
                      ? session.systemColor
                      : "#6d6d6d",
                  }}
                />
              );
            })}
          </div>

          <p className="session-details-player-count">
            Players: {currentPlayerCount}/{session.playerLimit}
          </p>
        </div>

        <div className="session-details-actions">
          <button className="details-back-button" onClick={onBack}>
            BACK
          </button>

          {onLeave && !isDm && (
            <button
              className="session-action-button leave"
              onClick={() => onLeave(session.id)}
            >
              LEAVE PARTY
            </button>
          )}

          {isDm && onDeleteLobby && (
            <button
              className="session-action-button leave"
              onClick={() => onDeleteLobby(session.id)}
            >
              DELETE LOBBY
            </button>
          )}
        </div>
      </div>
    </section>
  );
}