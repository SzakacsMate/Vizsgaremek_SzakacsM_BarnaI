import { useState } from "react";
import type { Location } from "../types/location";
import SimpleCalendar from "./SimpleCalendar";

type ReservationDetailsCardProps = {
  location: Location;
  onBack: () => void;
  onFinalize: (reservationData: {
    location: Location;
    sessionTitle: string;
    description: string;
    gameSystem: string;
    minPlayers: number;
    maxPlayers: number;
    period: number;
    selectedDate: string;
  }) => void;
};

export default function ReservationDetailsCard({
  location,
  onBack,
  onFinalize,
}: ReservationDetailsCardProps) {
  const [selectedDate, setSelectedDate] = useState<string | null>(null);
  const [sessionTitle, setSessionTitle] = useState("");
  const [description, setDescription] = useState("");
  const [gameSystem, setGameSystem] = useState("");
  const [minPlayers, setMinPlayers] = useState("");
  const [maxPlayers, setMaxPlayers] = useState("");
  const [period, setPeriod] = useState("");
  const [errorMessage, setErrorMessage] = useState("");

  const handleFinalize = () => {
    if (
      !sessionTitle ||
      !gameSystem ||
      !minPlayers ||
      !maxPlayers ||
      !period ||
      !selectedDate
    ) {
      setErrorMessage("Please fill in every field before finalizing.");
      return;
    }

    const minPlayersNumber = Number(minPlayers);
    const maxPlayersNumber = Number(maxPlayers);
    const periodNumber = Number(period);

    if (Number.isNaN(minPlayersNumber) || minPlayersNumber <= 0) {
      setErrorMessage("Minimum players must be a valid number greater than 0.");
      return;
    }

    if (Number.isNaN(maxPlayersNumber) || maxPlayersNumber <= 0) {
      setErrorMessage("Max players must be a valid number greater than 0.");
      return;
    }

    if (minPlayersNumber > maxPlayersNumber) {
      setErrorMessage("Minimum players cannot be greater than maximum players.");
      return;
    }

    if (Number.isNaN(periodNumber) || periodNumber <= 0) {
      setErrorMessage("Period must be a valid number greater than 0.");
      return;
    }

    setErrorMessage("");

    onFinalize({
      location,
      sessionTitle,
      description,
      gameSystem,
      minPlayers: minPlayersNumber,
      maxPlayers: maxPlayersNumber,
      period: periodNumber,
      selectedDate,
    });
  };

  return (
    <section className="reservation-details-card">
      <div className="reservation-details-top">
        <img
          src={location.image}
          alt={location.name}
          className="reservation-details-image"
        />

        <div className="reservation-details-header-block">
          <div className="reservation-details-title-box">
            <h2 className="reservation-details-title">{location.name}</h2>
            <p className="reservation-details-address">{location.address}</p>
          </div>

          <div className="reservation-form-grid">
            <input
              className="reservation-input reservation-text-input"
              type="text"
              placeholder="Session Title"
              value={sessionTitle}
              onChange={(e) => setSessionTitle(e.target.value)}
            />

            <textarea
              className="reservation-input reservation-description-input"
              placeholder="Session description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
            />

            <select
              className="reservation-input reservation-select"
              value={gameSystem}
              onChange={(e) => setGameSystem(e.target.value)}
            >
              <option value="">Game System</option>
              <option value="D&D 5e">D&D 5e</option>
              <option value="Pathfinder 2e">Pathfinder 2e</option>
              <option value="Call of Cthulhu 7e">Call of Cthulhu 7e</option>
            </select>

            <div className="reservation-inline-input">
              <label>Min. Players:</label>
              <input
                className="reservation-input"
                type="number"
                min="1"
                max="12"
                value={minPlayers}
                onChange={(e) => setMinPlayers(e.target.value)}
              />
            </div>

            <div className="reservation-inline-input">
              <label>Max. Players:</label>
              <input
                className="reservation-input"
                type="number"
                min="1"
                max="12"
                value={maxPlayers}
                onChange={(e) => setMaxPlayers(e.target.value)}
              />
            </div>

            <SimpleCalendar
              bookedDates={location.bookedDates}
              selectedDate={selectedDate}
              onSelectDate={setSelectedDate}
            />

            <div className="reservation-inline-input">
              <label>Period:</label>
              <div className="reservation-period-group">
                <input
                  className="reservation-input"
                  type="number"
                  min="1"
                  max="12"
                  value={period}
                  onChange={(e) => setPeriod(e.target.value)}
                />
                <span>h</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      {errorMessage && <p className="reservation-error-message">{errorMessage}</p>}

      <div className="reservation-details-actions">
        <button className="details-back-button" onClick={onBack}>
          BACK
        </button>

        <button className="reservation-finalize-button" onClick={handleFinalize}>
          FINALIZE
        </button>
      </div>
    </section>
  );
}