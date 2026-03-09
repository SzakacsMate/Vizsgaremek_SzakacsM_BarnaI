import type { Location } from "../types/location";

type LocationCardProps = {
  location: Location;
  onOpen: (location: Location) => void;
};

export default function LocationCard({
  location,
  onOpen,
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
        className="location-card-button"
        onClick={(event) => {
          event.stopPropagation();
          alert(`Reserve clicked for: ${location.name}`);
        }}
      >
        RESERVE
      </button>
    </article>
  );
}