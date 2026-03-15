import type { Location } from "../types/location";

type LocationCardProps = {
  location: Location;
  onOpen: (location: Location) => void;
  onReserve: (location: Location) => void;
  canReserve: boolean;
};

export default function LocationCard({
  location,
  onOpen,
  onReserve,
  canReserve,
}: LocationCardProps) {
  return (
    <article className="location-card" onClick={() => onOpen(location)}>
      <img
        src={location.image}
        alt={location.name}
        className="location-card-image"
      />

      <div className="location-card-content">
        <h2 className="location-card-title">{location.name}</h2>
        <p className="location-card-address">{location.address}</p>
        <p className="location-card-description">{location.description}</p>
      </div>

      <button
        className={`location-card-button ${!canReserve ? "disabled-reserve-button" : ""}`}
        onClick={(event) => {
          event.stopPropagation();

          if (!canReserve) return;

          onReserve(location);
        }}
        disabled={!canReserve}>
        RESERVE
      </button>
    </article>
  );
}