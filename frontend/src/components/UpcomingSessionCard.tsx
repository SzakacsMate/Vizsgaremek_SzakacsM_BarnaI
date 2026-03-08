import type { Session } from "../types/session";

type UpcomingSessionCardProps = {
  session: Session;
};

export default function UpcomingSessionCard({
  session,
}: UpcomingSessionCardProps) {
  const currentPlayerCount = session.players.length;

  return (
    <div className="upcoming-card">
      <div
        className="upcoming-card-topline"
        style={{ backgroundColor: session.systemColor }}
      />

      <div className="upcoming-card-content">
        <h3 className="upcoming-card-title">
          {session.title} - Session {session.sessionNumber}
        </h3>

        <p
          className="upcoming-card-system"
          style={{ color: session.systemColor }}
        >
          {session.system}
        </p>

        <p className="upcoming-card-text">
          {session.date} ({session.duration})
        </p>

        <p className="upcoming-card-text">{session.location}</p>

        <div className="upcoming-player-row">
          <div className="upcoming-player-bars">
            {Array.from({ length: session.playerLimit }).map((_, index) => {
              const isFilled = index < currentPlayerCount;

              return (
                <div
                  key={index}
                  className="upcoming-player-bar"
                  style={{
                    backgroundColor: isFilled
                      ? session.systemColor
                      : "#8a8a8a",
                  }}
                />
              );
            })}
          </div>

          <p className="upcoming-card-players">
            {currentPlayerCount}/{session.playerLimit} Players
          </p>
        </div>
      </div>
    </div>
  );
}