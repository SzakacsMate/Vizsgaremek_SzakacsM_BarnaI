import type { Location } from "../types/location";

type LocationCardProps = {
  location: Location;
};

export default function LocationCard({ location }: LocationCardProps) {
  return (
    <div className="location-card">
      <img
        src={location.image}
        alt={location.name}
        className="location-card-image"
      />

      <div className="location-card-content">
        <h2 className="location-card-title">{location.name}</h2>
        <p className="location-card-address">{location.address}</p>
        <p className="location-card-description">{location.description}</p>
        <button className="location-card-button">Reserve</button>
      </div>
    </div>
  );
}