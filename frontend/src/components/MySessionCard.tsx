import type { Session } from "../types/session";

type MySessionCardProps = {
  session: Session;
  onOpen: (session: Session) => void;
};

export default function MySessionCard({
  session,
  onOpen,
}: MySessionCardProps) {
  const currentPlayerCount = session.players.length;

  return (
    <article className="my-session-card" onClick={() => onOpen(session)}>
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
            }`}>
            {session.status === "confirmed" ? "Confirmed" : "Pending"}
          </span>
        </div>

        <div
          className="my-session-divider"
          style={{ backgroundColor: session.systemColor }}
        />

        <p className="my-session-text">
          {session.date} ({session.duration})
        </p>
        <p className="my-session-text">{session.location}</p>
        <p className="my-session-text">DM: Placeholder DM</p>

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
      </div>
    </article>
  );
}