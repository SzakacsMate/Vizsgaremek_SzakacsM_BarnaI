import type { Session } from "../types/session";

type MySessionCardProps = {
  session: Session;
  onOpen: (session: Session) => void;
  onJoin?: (sessionId: string) => void;
  onLeave?: (sessionId: string) => void;
  onDeleteLobby?: (sessionId: string) => void;
  currentUserName?: string;
  currentUserId?: string;
};

export default function MySessionCard({
  session,
  onOpen,
  onJoin,
  onLeave,
  onDeleteLobby,
  currentUserName,
  currentUserId,
}: MySessionCardProps) {
  const isDm =
    (currentUserName != null && session.dmName === currentUserName) ||
    (currentUserId != null && session.dmName === currentUserId);
  const currentPlayerCount = session.players.length;

  return (
    <article
      className={`my-session-card ${
        session.status === "confirmed" ? "card-confirmed" : "card-pending"
      }`}
      onClick={() => onOpen(session)}
    >
      <div
        className="my-session-left-bar"
        style={{ backgroundColor: session.systemColor }}
      />

      <div className="my-session-content">
        <h2 className="my-session-title">
          {session.title} - Session {session.sessionNumber}
        </h2>

        <div className="my-session-system-row">
          <span
            className="my-session-system"
            style={{ color: session.systemColor }}
          >
            {session.system}
          </span>

          <span
            className={`my-session-status ${
              session.status === "confirmed"
                ? "status-confirmed"
                : "status-pending"
            }`}
          >
            {session.status === "confirmed" ? "Confirmed" : "Pending"}
          </span>
        </div>

        <div
          className="my-session-divider"
          style={{ backgroundColor: session.systemColor }}
        />

        <p className="my-session-text">{session.date}</p>
        <p className="my-session-text">{session.location}</p>
        <p className="my-session-text">DM: {session.dmName}</p>

        <div className="my-session-player-row">
          <div className="my-session-player-bars">
            {Array.from({ length: session.playerLimit }).map((_, index) => {
              const isFilled = index < currentPlayerCount;

              return (
                <div
                  key={index}
                  className="my-session-player-bar"
                  style={{
                    backgroundColor: isFilled
                      ? session.systemColor
                      : "#6d6d6d",
                  }}
                />
              );
            })}
          </div>

          <p className="my-session-player-count">
            {currentPlayerCount}/{session.playerLimit} Players
          </p>
        </div>

        {(onJoin || onLeave || onDeleteLobby) && (
          <div className="my-session-action-row">
            {onJoin && (
              <button
                className="session-action-button join"
                onClick={(event) => {
                  event.stopPropagation();
                  onJoin(session.id);
                }}
              >
                JOIN
              </button>
            )}

            {isDm && onDeleteLobby && (
              <button
                className="session-action-button leave"
                onClick={(event) => {
                  event.stopPropagation();
                  onDeleteLobby(session.id);
                }}
              >
                DELETE LOBBY
              </button>
            )}

            {!isDm && onLeave && (
              <button
                className="session-action-button leave"
                onClick={(event) => {
                  event.stopPropagation();
                  onLeave(session.id);
                }}
              >
                LEAVE PARTY
              </button>
            )}
          </div>
        )}
      </div>
    </article>
  );
}